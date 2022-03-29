using CA_Loger;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace CA_LoggerUI.Views
{
    public class MainMenuView : LibUser
    {
        public string CA_LoggerUI { get; set; } = "0.9: 2022-01";
        public int setupYear { get; set; } = DateTime.Now.Year;
        public int setupMonth { get; set; } = DateTime.Now.Month;
        public int setupDay { get; set; } = DateTime.Now.Day;
        public bool planing
        {
            get
            {
                if (new DateTime(setupYear, setupMonth, setupDay) > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public MainMenuView()
        {

        }
        public bool Schow()
        {
            DefaultsSettingsView defaultsSettings = new DefaultsSettingsView();
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            LibDateTime ldt = new LibDateTime();
            
            AnsiConsole.Clear();

            if (da.LoadSetupEntry(name: $"freeDaysOfWork{setupYear}-{setupMonth}").Value == null)
            {
                string tmpHolidays = "";
                if (setupMonth == 1) { tmpHolidays = "1 6"; }
                if (setupMonth == 4) { tmpHolidays = "17 18"; }
                if (setupMonth == 5) { tmpHolidays = "1 3"; }
                if (setupMonth == 6) { tmpHolidays = "5 16"; }
                if (setupMonth == 8) { tmpHolidays = "15"; }
                if (setupMonth == 11) { tmpHolidays = "1 11"; }
                if (setupMonth == 12) { tmpHolidays = "25 26"; }
                if (tmpHolidays!="")
                {
                    AnsiConsole.MarkupLine($"Wprowadzono w miesiącu dni wolne dla: [green]{tmpHolidays}[/]\n");
                }
                da.SaveSetupEntry($"freeDaysOfWork{setupYear}-{setupMonth}", tmpHolidays);
                da.SaveSetupEntry($"daysOfWork{setupYear}-{setupMonth}", "");
            }

            (var logUser, var logLastLogin) = LibUser.getMachineLogonName(".").Last();

            var rule = new Rule($"[white]Bieżąca data: [green]{DateTime.Now.Day} {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month)} {DateTime.Now.Year}[/] Użytkownik: [green]{logUser}[/], ostatnie logowanie: [green]{logLastLogin}[/][/]");
            rule.Alignment = Justify.Center;
            rule.Style = Style.Parse("green dim");
            AnsiConsole.Write(rule);

            //AnsiConsole.MarkupLine();

            DayStatusView dayStatusView = new DayStatusView();
            dayStatusView.DayStatus(setupYear, setupMonth, setupDay);

            var rule2 = new Rule();
            rule2.Style = Style.Parse("green dim");
            AnsiConsole.Write(rule2);

            DateTime tmpSetupDate = new DateTime(setupYear, setupMonth, setupDay);


            // Calculate list of days without date
            int tmpTo = 0;
            if ((tmpSetupDate.Year == DateTime.Now.Year) && (tmpSetupDate.Month == DateTime.Now.Month))
            {
                tmpTo = DateTime.Now.Day;
            }
            else if (tmpSetupDate > DateTime.Now)
            {
                tmpTo = 0;
            }
            else
            {
                tmpTo = DateTime.DaysInMonth(setupYear, setupMonth);
            }
            string freeDaysOfWorkCurrent = da.LoadSetupEntry(name: $"freeDaysOfWork{setupYear}-{setupMonth}").Value;
            string daysOfWorkCurrent = da.LoadSetupEntry(name: $"daysOfWork{setupYear}-{setupMonth}").Value;
            (List<DateTime> tmpDays, List<DateTime> tmpWorkDays) = defaultsSettings.CreateCalender(setupYear: setupYear, setupMonth: setupMonth, freeDaysOfWeek: defaultsSettings.LoadListOfFreeDays(), freeDaysOfWork: freeDaysOfWorkCurrent, daysOfWork: daysOfWorkCurrent, false);
            DayStatusView dayStatus = new DayStatusView();
            (string allTimeAtWork, List<string> dayWithoutTime) = dayStatus.WorkedTimeFromTo(setupYear, setupMonth, 1, tmpTo, tmpWorkDays, false);
            if (dayWithoutTime.Contains(setupDay.ToString()) && (DateTime.Compare(new DateTime(setupYear, setupMonth, setupDay), new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)) == 0))
            {
                dayWithoutTime.Remove(setupDay.ToString());
            }

            string whenSendRaport = da.LoadSetupEntry(name: $"whenSentRaport").Value;
            if (int.Parse(whenSendRaport)>DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
            {
                whenSendRaport = tmpWorkDays.Last().Day.ToString();
            }
            if (da.LoadSetupEntry(name: "eMailToSend").Value != "")
            {
                if (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).CompareTo(new DateTime(DateTime.Now.Year, DateTime.Now.Month, int.Parse(whenSendRaport))) >= 0)
                {
                    string isSendedRaport = da.LoadSetupEntry(name: $"isSendedRaport").Value;
                    if (isSendedRaport != $"{DateTime.Now.Year}-{DateTime.Now.Month}_True")
                    {
                        bool sendRaport = AnsiConsole.Confirm($"Wysłać raport do [yellow]{String.Join(", ", defaultsSettings.eMailToSend.Split(" "))}[/]", true);
                        if (sendRaport)
                        {
                            bool longRaportComment = false;
                            bool longRaport = AnsiConsole.Confirm($"Generować pełny raport ?", false);
                            if (longRaport)
                            {
                                longRaportComment = AnsiConsole.Confirm($"Ukryć komentarze ?", true);
                            }
                            RaportView raportView = new RaportView();
                            //raportView.GenerateRaportSpectre(setupYear, setupMonth, true);

                            (var logFulname, var logLastLoginn) = LibUser.getMachineLogonFullName(".").Last();
                            //string passMail = AnsiConsole.Ask<string>($"Wprowadź hasło do skrzynki [green]{logUser}@betard.pl {logFulname}[/]:");
                            string passMail = AnsiConsole.Prompt(
                                new TextPrompt<string>($"Wprowadź hasło do skrzynki [green]{logUser}@betard.pl {logFulname}[/]:").Secret());
                            bool isSendedRaportAttend = raportView.SendMail(setupYear, setupMonth, setupDay, longRaport, $"{logUser}@betard.pl {logFulname}", String.Join(", ", defaultsSettings.eMailToSend.Split(" ")), passMail, longRaportComment);
                            raportView.ExportHtml(setupYear, setupMonth, setupDay, false);
                            if (isSendedRaportAttend)
                            {
                                da.SaveSetupEntry("isSendedRaport", $"{DateTime.Now.Year}-{DateTime.Now.Month}_True");
                            }
                        }
                    }
                }
            }
            var menuMain = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Wybierz z menu[/]")
                .PageSize(15)
                .MoreChoicesText("[grey](Strzałkami góra/dół przełącz opcję)[/]")
                .AddChoices(new[] {

                    $"Wprowadź godziny pracy w dniu [green]{setupYear}-{setupMonth}-{setupDay} {(planing==true?"(planowane)":"")}[/]",
                    $"Zmień dzień",
                    $"Wprowadź nieobecność od [green]{setupYear}-{setupMonth}-{setupDay}[/]",
                    dayWithoutTime.Count() > 0?$"Wprowadź zaległe godziny pracy w dniach [green]{String.Join(", ", dayWithoutTime)}[/]":null,
                    "-----------------------------------------------------------",
                    "Wyświetl zestawienie miesiąca",
                    da.LoadSetupEntry(name: "eMailToSend").Value != ""?$"Wyślij raport":null,
                    "Eksportuj raport do pliku",
                    "-----------------------------------------------------------",
                    "Edytuj dni wolne bieżącego miesiąca",
                    "Ustawienia globalne",
                    "Wyjście",
                    "-----------------------------------------------------------",
                    "Zgłoś usterkę lub uwagę do autora programu"
            }));

            if (menuMain == $"Wprowadź godziny pracy w dniu [green]{setupYear}-{setupMonth}-{setupDay} {(planing == true ? "(planowane)" : "")}[/]")
            {
                InsertHoursView insertHoursView = new InsertHoursView();
                insertHoursView.InsertWorkingTime(setupYear, setupMonth, setupDay, planing);

            }


            if (menuMain == $"Wprowadź nieobecność od [green]{setupYear}-{setupMonth}-{setupDay}[/]")
            {
                InsertHolidayView holidayEntry = new InsertHolidayView();
                holidayEntry.Menu(setupYear, setupMonth, setupDay);
            }

            if (menuMain == $"Wprowadź zaległe godziny pracy w dniach [green]{String.Join(", ", dayWithoutTime)}[/]")
            {
                InsertHoursView insertHoursView = new InsertHoursView();

                foreach (string day in dayWithoutTime)
                {
                    insertHoursView.InsertWorkingTime(setupYear, setupMonth, int.Parse(day), planing);
                }

            }


            if (menuMain == "Wyświetl zestawienie miesiąca")
            {
                RaportView raportView = new RaportView();
                raportView.Schow(setupYear, setupMonth, setupDay);

                var rulee = new Rule($"[grey](wciśnij dowolny klawisz)[/]");
                rulee.Alignment = Justify.Center;
                rulee.Style = Style.Parse("gray dim");
                AnsiConsole.Write(rulee);

                Console.ReadKey();
            }

            if (menuMain == $"Wyślij raport")
            {
                AnsiConsole.MarkupLine($"Wysyłanie raportu do [yellow]{ String.Join(", ", defaultsSettings.eMailToSend.Split(" "))}[/]\n");
                bool longRaport = AnsiConsole.Confirm($"Generować pełny raport ?", false);

                bool longRaportComment = false;
                if (longRaport)
                {
                    longRaportComment = AnsiConsole.Confirm($"Ukryć komentarze ?", true);
                }

                RaportView raportView = new RaportView();
                //raportView.GenerateRaportSpectre(setupYear, setupMonth, true);

                (var logFulname, var logLastLoginn) = LibUser.getMachineLogonFullName(".").Last();
                //string passMail = AnsiConsole.Ask<string>($"Wprowadź hasło do skrzynki [green]{logUser}@betard.pl {logFulname}[/]:");
                string passMail = AnsiConsole.Prompt(
                    new TextPrompt<string>($"Wprowadź hasło do skrzynki [green]{logUser}@betard.pl {logFulname}[/]:").Secret());
                bool isSendedRap = raportView.SendMail(setupYear, setupMonth, setupDay, longRaport, $"{logUser}@betard.pl {logFulname}", String.Join(", ", defaultsSettings.eMailToSend.Split(" ")), passMail, longRaportComment);
                raportView.ExportHtml(setupYear, setupMonth, setupDay, false);


                if (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).CompareTo(new DateTime(DateTime.Now.Year, DateTime.Now.Month, int.Parse(whenSendRaport))) >= 0)
                {
                    string isSendedRaport = da.LoadSetupEntry(name: $"isSendedRaport").Value;
                    if (isSendedRaport != $"{DateTime.Now.Year}-{DateTime.Now.Month}_True")
                    {
                        if (isSendedRap)
                        {
                            da.SaveSetupEntry("isSendedRaport", $"{DateTime.Now.Year}-{DateTime.Now.Month}_True");
                        }
                    }
                }
            }

            if (menuMain == "Eksportuj raport do pliku")
            {
                var menuExport = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Wybierz format pliku")
                    .PageSize(4)
                    .MoreChoicesText("[grey](Strzałkami góra/dół przełącz opcję)[/]")
                    .AddChoices(new[] {"Format dla excela (*.csv)",
                        "Format (*.html)",
                        $"--------------------",
                        $"<= Wróć"
                    }));

                if (menuExport == "Format (*.html)")
                {
                    RaportView raportView = new RaportView();
                    raportView.ExportHtml(setupYear, setupMonth, setupDay);
                }

                if (menuExport == "Format dla excela (*.csv)")
                {
                    RaportView raportView = new RaportView();
                    raportView.ExportCSV(setupYear, setupMonth, setupDay);
                }
                var ruleee = new Rule($"[blue]Wygenerowano plik raportu[/] [grey](wciśnij dowolny klawisz)[/]");
                ruleee.Alignment = Justify.Center;
                ruleee.Style = Style.Parse("gray dim");
                AnsiConsole.Write(ruleee);

                Console.ReadKey();
            }


            if (menuMain == "Ustawienia globalne")
            {

                if (File.Exists(SqlLiteDataAcces.PlikBazyDanych))
                {
                    bool defineEraseData = AnsiConsole.Confirm($"Wyczyścić dane z programu?", false);
                    if (defineEraseData)
                    {
                        File.Delete(SqlLiteDataAcces.PlikBazyDanych);
                        da = new SqlLiteDataAcces();
                        SqlLiteDataAcces.UtworzPustaBazeDanych(SqlLiteDataAcces.PlikBazyDanych);
                    }
                }
                defaultsSettings.GetSetup();
                
            }


            if (menuMain == $"Zmień dzień")
            {

                var calenderPreview = new Spectre.Console.Calendar(setupMonth - 1 == 0 ? setupYear - 1 : setupYear, setupMonth - 1 == 0 ? 12 : setupMonth - 1);
                calenderPreview.Culture("pl-PL");
                AnsiConsole.Write(calenderPreview);

                calenderPreview = new Spectre.Console.Calendar(setupYear, setupMonth);
                calenderPreview.AddCalendarEvent(DateTime.Now);
                calenderPreview.Culture("pl-PL");
                AnsiConsole.Write(calenderPreview);

                bool validate = false;
                string tmpCurrentDate = $"{setupYear}-{setupMonth}-{setupDay}";
                while (!validate)
                {
                    string currentDate = AnsiConsole.Ask<string>($"Wprowadź dane dla dnia:", tmpCurrentDate);

                    if (currentDate.Split("-").Count() == 3)
                    {
                        setupYear = int.Parse(currentDate.Split("-")[0]);

                            setupMonth = int.Parse(currentDate.Split("-")[1]);

                        setupDay = int.Parse(currentDate.Split("-")[2]);
                        validate = true;
                    }
                    if (currentDate.Split("-").Count() == 2)
                    {
                        setupMonth = int.Parse(currentDate.Split("-")[0]);
                        setupDay = int.Parse(currentDate.Split("-")[1]);
                        validate = true;
                    }
                    if (currentDate.Split("-").Count() == 1)
                    {
                        if (int.TryParse(currentDate, out int a) == true)
                        {
                            setupDay = int.Parse(currentDate);
                            validate = true;
                        }
                    }

                    
                    if (!((validate==true)&&(setupMonth<=12)&&(1 <=setupMonth)&&(setupDay<=DateTime.DaysInMonth(setupYear,setupMonth)))) {
                        validate = false;
                    }
                    
                    if (validate == false)
                    {
                        AnsiConsole.Markup("[red]Błąd: Niepoprawny format lub zakres daty: 0000-00-00, 00-00, 00[/]");
                    }
                }
            }

            if (menuMain == "Edytuj dni wolne bieżącego miesiąca")
            {
                bool defineAgainFreeDays = true;
                while (defineAgainFreeDays == true)
                {
                    defaultsSettings.GetFreeDays(setupYear, setupMonth);
                    defineAgainFreeDays = AnsiConsole.Confirm($"Zdefiniować ponownie dni wolne?", false);
                }
            }

            if (menuMain == "Zgłoś usterkę lub uwagę do autora programu")
            {
                AnsiConsole.Write(
                    new FigletText("BETARD")
                        .RightAligned()
                        .Color(Color.Blue));
                AnsiConsole.MarkupLine($"Program [blue]CA_LoggerUI[/], wersja: [green]{CA_LoggerUI}[/]");
                AnsiConsole.MarkupLine("Autor programu: [blue]Marcin Rutkowski[/], [green]m.rutkowski@betard.pl[/], [green]marcinrutkowski@budowlaniec.net[/]");
                string attentionMail = AnsiConsole.Ask<string>($"Opisz usterkę lub podaj swoją uwagę do działania programu:");

                LibMail lm = new LibMail();
                (var logFulname, var logLastLoginn) = LibUser.getMachineLogonFullName(".").Last();
                string passMail = AnsiConsole.Prompt(
                new TextPrompt<string>($"Wprowadź hasło do skrzynki [green]{logUser}@betard.pl {logFulname}[/]:").Secret());
                lm.WyslijWiadomosc($"{logUser}@betard.pl {logFulname}", "m.rutkowski@betard.pl, marcinrutkowski@budowlaniec.net", "", "", $"CA_LoggerUI {CA_LoggerUI} (bug)", attentionMail, new List<string>(), "", passMail);

                var rulee = new Rule($"[blue]Wiadomość wysłano.[/] [grey](wciśnij dowolny klawisz)[/]");
                rulee.Alignment = Justify.Center;
                rulee.Style = Style.Parse("gray dim");
                AnsiConsole.Write(rulee);

                Console.ReadKey();
            }

            if (menuMain == "Wyjście")
            {
                return false;
            }
            else
            {
                return true;
            }


        }
    }
}
