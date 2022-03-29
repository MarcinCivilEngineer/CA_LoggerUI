using CA_Loger;
using CA_LoggerUI.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA_LoggerUI.Views
{
    public class DefaultsSettingsView
    {
        public string eMailToSend { get; set; }
        public int whenSentRaport { get; set; }
        public DaysModel days { get; set; } = new DaysModel();
        List<string> freeDaysOfWeek { get; set; } = new List<string>();
        public string workingPlanningTimeStart { get; set; }
        public string daysOfWorkCurrent { get; set; }
        public string workingPlanningTimeEnd { get; set; }
        public string holidaysAtYear { get; set; }
        public string holidaysByKids { get; set; }
        public bool isLoadedFromSQL { get; set; } = false;
        public string freeDaysOfWorkCurrent { get; set; }
        public string minutesStartBreak { get; set; }
        public string minutesEndBreak { get; set; }
        public string minutesStartWork { get; set; }
        public string minutesEndWork { get; set; }

        public DefaultsSettingsView()
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            eMailToSend = da.LoadSetupEntry(name: "eMailToSend").Value;

            string tmpWhenSentRaport = da.LoadSetupEntry(name: "whenSentRaport").Value;
            if (tmpWhenSentRaport != null)
            {
                whenSentRaport = int.Parse(tmpWhenSentRaport);
            }



            workingPlanningTimeStart = da.LoadSetupEntry(name: "workingPlanningTimeStart").Value;
            workingPlanningTimeEnd = da.LoadSetupEntry(name: "workingPlanningTimeEnd").Value;
            holidaysAtYear = da.LoadSetupEntry(name: "holidaysAtYear").Value;
            holidaysByKids = da.LoadSetupEntry(name: "holidaysByKids").Value;
            freeDaysOfWorkCurrent = da.LoadSetupEntry(name: $"freeDaysOfWork{DateTime.Now.Year}-{DateTime.Now.Month}").Value;
            daysOfWorkCurrent = da.LoadSetupEntry(name: $"daysOfWork{DateTime.Now.Year}-{DateTime.Now.Month}").Value;
            if (eMailToSend != null && whenSentRaport != 0 && workingPlanningTimeEnd!=null && workingPlanningTimeStart!=null)
            {
                isLoadedFromSQL = true;
            }
        }
        public void GetSetup()
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            

            string eMailToSendFirst = da.LoadSetupEntry(name: $"eMailToSend").Value == null ? "l.andrzejewski@betard.pl" : da.LoadSetupEntry(name: $"eMailToSend").Value;
            int whenSentRaportFirst =  da.LoadSetupEntry(name: $"whenSentRaport").Value==null?25: int.Parse(da.LoadSetupEntry(name: $"whenSentRaport").Value);
            string workingPlanningTimeStartFirst = da.LoadSetupEntry(name: $"workingPlanningTimeStart").Value == null ? "7:00" : da.LoadSetupEntry(name: $"workingPlanningTimeStart").Value;
            int minutestAtWorkFirst = (int)Math.Round(8.5 * 60, 0);

            string holidaysAtYearFirst = da.LoadSetupEntry(name: $"holidaysAtYear").Value == null ? "26" : da.LoadSetupEntry(name: $"holidaysAtYear").Value;
            string holidaysByKidsFirst = da.LoadSetupEntry(name: $"holidaysByKids").Value == null ? "2" : da.LoadSetupEntry(name: $"holidaysByKids").Value;


            AnsiConsole.MarkupLine("[bold]Ustawienia początkowe[/]\n");

            eMailToSend = AnsiConsole.Ask<string>($"Podaj maile rozdzielone spacją do rozesłania raportu lub '[green]x[/]' nie wysyłaj raportu:", eMailToSendFirst);
            if (eMailToSend == "x")
            {
                eMailToSend = "";
            }
            if (eMailToSend != "") { 
            whenSentRaport = AnsiConsole.Ask<int>($"Dzień wysłania raportu:", whenSentRaportFirst);
            }
            MultiSelectionPrompt<string> multiSelectionPrompt = new MultiSelectionPrompt<string>();
            multiSelectionPrompt.Title("Wolne dni tygodnia")
                .NotRequired()
                .PageSize(7)
                .MoreChoicesText("(Strzałkami góra/dół przemieszczaj się po dniach tygodnia)")
                .InstructionsText("(Wybierz <spacja> aby oznaczyć dzień wolny 'X', <enter> aby zaakceptować wybór)")
                .AddChoices(days.inPolish);
            
            
            foreach (string day in days.inPolish)
            {
                if (da.LoadSetupEntry(name: $"freeDaysOfWeek{day}").Value == "True")
                {
                    multiSelectionPrompt.Select(day);
                }
            }

            if (isLoadedFromSQL == false)
            {
                multiSelectionPrompt.Select(days.inPolish[0]);
                multiSelectionPrompt.Select(days.inPolish[6]);
            }



            freeDaysOfWeek = AnsiConsole.Prompt(multiSelectionPrompt);


            AnsiConsole.MarkupLine($"Dni wolne: [green]{String.Join(", ", freeDaysOfWeek)}[/]");

            bool defineOtherFreeDays = AnsiConsole.Confirm($"Zdefiniować dni wolne?",false);
            if (defineOtherFreeDays == true)
            {
                int setupYear = DateTime.Now.Year;
                int setupMonth = DateTime.Now.Month;

                bool defineAgainFreeDays = true;
                while (defineAgainFreeDays == true) { 
                    GetFreeDays(setupYear, setupMonth);
                    defineAgainFreeDays = AnsiConsole.Confirm($"Zdefiniować ponownie dni wolne?", false);
                }

            }

            workingPlanningTimeStart = AnsiConsole.Ask<string>($"Zakładana godzina rozpoczęcia pracy:", workingPlanningTimeStartFirst);

            string workingPlanningTimeEndFirst = da.LoadSetupEntry(name: $"workingPlanningTimeEnd").Value == null ? LibDateTime.HoursAddMinutes(workingPlanningTimeStart, minutestAtWorkFirst) : da.LoadSetupEntry(name: $"workingPlanningTimeEnd").Value;
            workingPlanningTimeEnd = AnsiConsole.Ask<string>($"Zakładana godzina zakończenia pracy:", workingPlanningTimeEndFirst);

            holidaysAtYear =  AnsiConsole.Ask<string>($"Ilość przysługujących dni urlopu [yellow]wypoczynkowego[/] (dla stażu <10 lat 20 dni):", holidaysAtYearFirst);
            holidaysByKids = AnsiConsole.Ask<string>($"Ilość przysługujących dni urlopu [yellow]opiekuńczego[/] z racji wychowywania dzieci na rodzica:", holidaysByKidsFirst);

            string minutesStartWorkFirst = da.LoadSetupEntry(name: $"minutesStartWork").Value == null ? "-5" : da.LoadSetupEntry(name: $"minutesStartWork").Value;
            minutesStartWork = AnsiConsole.Ask<string>($"Poprawka na czas rozpoczęcia pracy w minutach:", minutesStartWorkFirst);
            string minutesEndWorkFirst = da.LoadSetupEntry(name: $"minutesEndWork").Value == null ? "5" : da.LoadSetupEntry(name: $"minutesEndWork").Value;
            minutesEndWork = AnsiConsole.Ask<string>($"Poprawka na czas zakończenie pracy w minutach:", minutesEndWorkFirst);
            string minutesStartBreakFirst = da.LoadSetupEntry(name: $"minutesStartBreak").Value == null ? "0" : da.LoadSetupEntry(name: $"minutesStartBreak").Value;
            minutesStartBreak = AnsiConsole.Ask<string>($"Poprawka na czas rozpopczęcia przerwy w minutach:", minutesStartBreakFirst);
            string minutesEndBreakFirst = da.LoadSetupEntry(name: $"minutesEndBreak").Value == null ? "0" : da.LoadSetupEntry(name: $"minutesEndBreak").Value;
            minutesEndBreak = AnsiConsole.Ask<string>($"Poprawka na czas zakończenie przerwy w minutach:", minutesEndBreakFirst);
            SaveSetup();

            AnsiConsole.Clear();
        }
        public void SaveSetup()
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();

            da.SaveSetupEntry("eMailToSend", eMailToSend);
            da.SaveSetupEntry("whenSentRaport", whenSentRaport.ToString());

            foreach (string day in days.inPolish)
            {
                bool tmpIsFree=false;
                foreach (string free in freeDaysOfWeek)
                {
                    if (free==day)
                    {
                        tmpIsFree = true;
                    }
                }
                da.SaveSetupEntry($"freeDaysOfWeek{day}", tmpIsFree.ToString());
            }

            da.SaveSetupEntry("workingPlanningTimeStart", workingPlanningTimeStart);
            da.SaveSetupEntry("workingPlanningTimeEnd", workingPlanningTimeEnd);
            da.SaveSetupEntry("holidaysAtYear", holidaysAtYear);
            da.SaveSetupEntry("holidaysByKids", holidaysByKids);
            da.SaveSetupEntry("minutesStartBreak", minutesStartBreak);
            da.SaveSetupEntry("minutesEndBreak", minutesEndBreak);
            da.SaveSetupEntry("minutesStartWork", minutesStartWork);
            da.SaveSetupEntry("minutesEndWork", minutesEndWork);

            isLoadedFromSQL = true;
        }

        public List<string> LoadListOfFreeDays()
        {
            DaysModel nameDaysAtWeek = new DaysModel();
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            List<string> list = new List<string>();

            foreach (string nameOfDay in nameDaysAtWeek.inPolish)
            {
                if (da.LoadSetupEntry(name: $"freeDaysOfWeek{nameOfDay}").Value == "True")
                {
                    list.Add(nameOfDay);
                }
                
            }
            return list;

        }
        public void GetFreeDays(int setupYear, int setupMonth)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            string[] tmpWorkDays;

            string currentMonth = AnsiConsole.Ask<string>($"Wprowadź dni wolne dla miesiąca:", $"{setupYear}-{setupMonth}");

            if (currentMonth != $"{setupYear}-{setupMonth}")
            {
                setupYear = int.Parse(currentMonth.Split("-")[0]);
                setupMonth = int.Parse(currentMonth.Split("-")[1]);
            }

            string freeDaysOfWorkCurrent = da.LoadSetupEntry(name: $"freeDaysOfWork{setupYear}-{setupMonth}").Value;
            string daysOfWorkCurrent = da.LoadSetupEntry(name: $"daysOfWork{setupYear}-{setupMonth}").Value;
            CreateCalender(setupYear: setupYear, setupMonth: setupMonth, freeDaysOfWeek:((freeDaysOfWeek.Count() > 0)? freeDaysOfWeek: LoadListOfFreeDays()), freeDaysOfWork: freeDaysOfWorkCurrent, daysOfWork: daysOfWorkCurrent, true);

            if (freeDaysOfWorkCurrent == null)
            {
                freeDaysOfWorkCurrent = "";
            }
            string tmpFreeDaysOfWorkCurrent = AnsiConsole.Ask<string>($"Podaj dni wolne od pracy oddzielone spacją, [green]'x'[/] usuń dni:", freeDaysOfWorkCurrent);
            string tmpDaysOfWorkCurrent = AnsiConsole.Ask<string>($"Podaj dni pracujące oddzielone spacją, [green]'x'[/] usuń dni:", daysOfWorkCurrent);
            if (tmpFreeDaysOfWorkCurrent == "x")
            {
                tmpFreeDaysOfWorkCurrent = "";
            }
            if (tmpDaysOfWorkCurrent == "x")
            {
                tmpDaysOfWorkCurrent = "";
            }


            // TODO - remove absence if freeDay
            if ((tmpFreeDaysOfWorkCurrent == freeDaysOfWorkCurrent) && (tmpDaysOfWorkCurrent == daysOfWorkCurrent))
            {
                freeDaysOfWorkCurrent = tmpFreeDaysOfWorkCurrent;
                daysOfWorkCurrent = tmpDaysOfWorkCurrent;
            }
            else
            {
                // Remove from free days if is working day
                var tmpFreeDays = tmpFreeDaysOfWorkCurrent.Split(" ");
                tmpWorkDays = tmpDaysOfWorkCurrent == null ? new string[0] : tmpDaysOfWorkCurrent.Split(" ");
                if ((tmpFreeDays.Count() > 0) && (tmpWorkDays.Count() > 0))
                {
                    freeDaysOfWorkCurrent = "";
                    foreach (string tmpDay in tmpFreeDays)
                    {
                        if (tmpDay != "")
                        {
                            if (!tmpWorkDays.Contains(tmpDay))
                            {
                                if (freeDaysOfWorkCurrent == "")
                                {
                                    freeDaysOfWorkCurrent = tmpDay;
                                }
                                else
                                {
                                    freeDaysOfWorkCurrent = $"{freeDaysOfWorkCurrent} {tmpDay}";
                                }

                            }
                        }
                    }
                }
                else
                {
                    freeDaysOfWorkCurrent = tmpFreeDaysOfWorkCurrent;
                }

                if (tmpDaysOfWorkCurrent == null)
                {
                    daysOfWorkCurrent = "";
                }
                else
                {
                    daysOfWorkCurrent = tmpDaysOfWorkCurrent;
                }


                CreateCalender(setupYear: setupYear, setupMonth: setupMonth, freeDaysOfWeek: ((freeDaysOfWeek.Count() > 0) ? freeDaysOfWeek : LoadListOfFreeDays()), freeDaysOfWork: freeDaysOfWorkCurrent, daysOfWork: daysOfWorkCurrent, true);

                da.SaveSetupEntry($"freeDaysOfWork{setupYear}-{setupMonth}", freeDaysOfWorkCurrent);
                da.SaveSetupEntry($"daysOfWork{setupYear}-{setupMonth}", daysOfWorkCurrent);


            }
        }

        public (List<DateTime>, List<DateTime>) CreateCalender(int setupYear, int setupMonth, List<string> freeDaysOfWeek, string freeDaysOfWork, string daysOfWork,  bool showCallender)
        {
            DaysModel days=new DaysModel();
            List<DateTime> listDaysInMonth = new List<DateTime>();
            listDaysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(setupYear, setupMonth))
                                    .Select(day => new DateTime(setupYear, setupMonth, day)).ToList();

            List<DateTime> listWorkingDays = new List<DateTime>();
            listWorkingDays.AddRange(listDaysInMonth);

            // Add free days from days of week
            foreach (DayOfWeek dayOfWeek in days.DaysOfWeek(freeDaysOfWeek))
            {
                listWorkingDays.RemoveAll(x => (x.DayOfWeek == dayOfWeek));
            }

            // Add working days
            if (daysOfWork != null)
            {
                if (daysOfWork != "")
                {
                    {
                        if (daysOfWork.Split(" ").Count() > 0)
                        {
                            foreach (var tmpWorkingDay in daysOfWork.Split(" "))
                            {
                                listWorkingDays.Add(new DateTime(setupYear, setupMonth, int.Parse(tmpWorkingDay)));
                            }
                        }
                    }
                }
            }

            if (freeDaysOfWork != null)
            {
                if (freeDaysOfWork != "")
                {
                    {
                        foreach (string freeDay in freeDaysOfWork.Split(" "))
                        {
                            if (freeDay != "")
                            {
                                listWorkingDays.RemoveAll(x => x.Day == int.Parse(freeDay));
                            }
                        }
                    }
                }
            }

            var calender = new Calendar(setupYear, setupMonth);
            calender.Culture("pl-PL");

            // Select free days of week
            foreach (DateTime dayOfWeek in listDaysInMonth)
            {
                if (!listWorkingDays.Contains(dayOfWeek))
                {
                    calender.AddCalendarEvent(dayOfWeek);
                }
            }

            if (showCallender) 
            {
                AnsiConsole.Write(calender);
            }

            return (listDaysInMonth, listWorkingDays);
        }
    }
}

