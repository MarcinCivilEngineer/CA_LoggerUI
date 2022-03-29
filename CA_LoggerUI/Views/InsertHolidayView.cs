using CA_Loger;
using CA_LoggerUI.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA_LoggerUI.Views
{
    public class InsertHolidayView
    {
        private LibDateTime ldt = new LibDateTime();
        public int setupYear { get; set; }
        public int setupMonth { get; set; }
        public int setupDay { get; set; }
        public InsertHolidayView()
        {

        }

        

        public void Menu(int year, int month, int day)
        {
            setupYear = year;
            setupMonth = month;
            setupDay = day;
            DateTime dateStart = new DateTime(setupYear,setupMonth, setupDay);
            DateTime dateEnd = new DateTime(setupYear, setupMonth, setupDay );
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            int enterYear;
            int enterMonth;
            int enterDay;
            List<AbsenceDatesModel> absenceDatesEdited = new List<AbsenceDatesModel>();
            absenceDatesEdited.AddRange(IsOnHollidays($"{setupYear:d4}-{setupMonth:D2}-{setupDay:D2}"));

            string holidayFromFirst = $"{setupYear:d4}-{setupMonth:D2}-{setupDay:D2}";
            string holidayToFirst = $"{setupYear:d4}-{setupMonth:D2}-{setupDay:D2}";
            string holidayCommentFirst = "";

            AnsiConsole.MarkupLine($"Dni urlopu do wykorzystania: [red][/]");

            AnsiConsole.MarkupLine("======================================\n");

            string menuHoliday;
            if (absenceDatesEdited.Count() > 0)
            {
                menuHoliday = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Wybierz rodzaj urlopu do wprowadzenia/edycji")
                    .PageSize(15)
                    .MoreChoicesText("[grey](Strzałkami góra/dół przełącz opcję)[/]")
                    .AddChoices(new[] {
                                    $"Edytuj nieobecność{(absenceDatesEdited.Count()>0?$" ({absenceDatesEdited.Count()})":"")}",
                                                            $"--------------------",
                        $"<= Wróć"
                }));


                if ((menuHoliday == $"<= Wróć") || (menuHoliday == $"--------------------"))
                {

                } else
                {

                    foreach (AbsenceDatesModel absenceEdit in absenceDatesEdited)
                    {

                        bool askForDelete = AnsiConsole.Confirm($"Czy usunąć urlop [green]{absenceEdit.TypeOfAbsence}[/] od [green]{absenceEdit.DateStart}[/] do [green]{absenceEdit.DateEnd}[/], czyli [green]{AbsenceBetweenDates(absenceEdit.DateStart, absenceEdit.DateEnd).Count()}[/] dni urlopu", false);

                        if (askForDelete == true)
                        {
                            

                            
                            da.UsunAbsenceDay( idAbsence: absenceEdit.Id);

                            da.UsunAbsenceDates(absenceEdit.Id);

                        }
                        else
                        {
                            string holidayFrom = AnsiConsole.Ask<string>($"Podaj nową datę rozpoczęcia nieobecności:", absenceEdit.DateStart);
                            string holidayTo = AnsiConsole.Ask<string>($"Podaj nową datę zakończenia nieboecności, lub poprzedź liczbę znakiem '[green]+[/]' aby dodać dni kalendarzowe:", absenceEdit.DateEnd);

                            holidayTo = CalculateToInputedDate(holidayFrom: holidayFrom, holidayTo: holidayTo);

                            string holidayComment = AnsiConsole.Ask<string>($"Komentarz do nieobecności:", holidayCommentFirst);

                            absenceEdit.DateEnd = holidayTo;
                            absenceEdit.DateStart = holidayFrom;
                            absenceEdit.Comment = holidayComment;

                            absenceEdit.SqlUpdate();

                            CreateAbsenceFromTo(absenceEdit.Id, holidayFrom, holidayTo, absenceEdit.TypeOfAbsence);
                        }


                    }

                }


            }
            else
            {

                FindTypeOfAbsenceAtYear(setupYear.ToString()).Select(x => x.TypeOfAbsence = "UW").Count();

                menuHoliday = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Wybierz rodzaj urlopu do wprowadzenia/edycji")
                    .PageSize(15)
                    .MoreChoicesText("[grey](Strzałkami góra/dół przełącz opcję)[/]")
                    .AddChoices(new[] {
                    $"UW - Urlop wypoczynkowy, pozostało [green]{int.Parse(da.LoadSetupEntry(name:"holidaysAtYear").Value) - da.LoadAbsenceDay(year:setupYear,typeOfAbsence:"UW").Count()}[/]",
                    $"UO - Urlop okolicznościowy, wykorzystano [green]{da.LoadAbsenceDay(year:setupYear,typeOfAbsence:"UO").Count()}[/]",
                    $"UD - Urlop opiekuńczy, pozostało [green]{int.Parse(da.LoadSetupEntry(name:"holidaysByKids").Value) - da.LoadAbsenceDay(year:setupYear,typeOfAbsence:"UD").Count()}[/]",
                    $"L4 - Urlop chorobowy, wykorzystano [red]{da.LoadAbsenceDay(year : setupYear, typeOfAbsence : "L4").Count()}[/]",
                        $"UB - Urlop bezpłatny, wykorzystano [red]{da.LoadAbsenceDay(year : setupYear, typeOfAbsence : "UB").Count()}[/]",
                    
                        $"--------------------",
                        $"<= Wróć"
                }));

                if ((menuHoliday == $"<= Wróć")|| (menuHoliday == $"--------------------"))
                {

                }
                else
                {
                    string typeAbsence = menuHoliday.Split(" - ")[0];

                    string holidayFrom = AnsiConsole.Ask<string>($"Podaj datę rozpoczęcia nieobecności:", holidayFromFirst);
                    string holidayTo = AnsiConsole.Ask<string>($"Podaj datę zakończenia nieboecności, lub poprzedź liczbę znakiem '[green]+[/]' aby dodać dni kalendarzowe:", holidayFrom);

                    holidayTo = CalculateToInputedDate(holidayFrom: holidayFrom, holidayTo: holidayTo);

                    string holidayComment = AnsiConsole.Ask<string>($"Komentarz do nieobecności:", holidayCommentFirst);

                    int idAbsenceDates = new AbsenceDatesModel() { Id = 0, DateEnd = holidayTo, DateStart = holidayFrom, Comment = holidayComment, TypeOfAbsence = typeAbsence, }.SqlInsert();

                    CreateAbsenceFromTo(idAbsenceDates, holidayFrom, holidayTo, typeAbsence);
                }

            }

        }

        public List<AbsenceDayModel> AbsenceBetweenDates (string from, string to)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            List<AbsenceDayModel> listOfAbsence = new List<AbsenceDayModel>();

            foreach (AbsenceDayModel absenceDay in da.LoadAbsenceDay(year: setupYear))
            {
                DateTime dataAbsence = new DateTime(absenceDay.Year, absenceDay.Month, absenceDay.Day);


                if ((dataAbsence.CompareTo(LibDateTime.DateFromString(from)) >= 0) && (dataAbsence.CompareTo(LibDateTime.DateFromString(to)) <= 0))
                {
                    listOfAbsence.Add(absenceDay);

                }

            }
            return listOfAbsence;
        }

        public void CreateAbsenceFromTo (int idAbsence, string startDate, string endDate, string typeOfAbsenc)
        {
            DateTime dateFrom = LibDateTime.DateFromString(startDate);
            DateTime dateTo = LibDateTime.DateFromString(endDate);

            SqlLiteDataAcces da = new SqlLiteDataAcces();


            da.UsunAbsenceDay(idAbsence: idAbsence);
            
            // TOD - load idAbsence and clear all 

            DefaultsSettingsView defaultsSettings = new DefaultsSettingsView();

            (List<DateTime> datesInMonth, List<DateTime> datesWorkingInMonth) = defaultsSettings.CreateCalender(setupYear: setupYear, setupMonth: setupMonth, freeDaysOfWeek: defaultsSettings.LoadListOfFreeDays(), freeDaysOfWork: da.LoadSetupEntry(name: $"freeDaysOfWork{setupYear}-{setupMonth}").Value, daysOfWork: da.LoadSetupEntry(name: $"daysOfWork{setupYear}-{setupMonth}").Value, false);

            foreach (DateTime dayInMonth in datesInMonth)
            {
                if ((dayInMonth.CompareTo(dateFrom) >=0)&&(dayInMonth.CompareTo(dateTo) <= 0))
                {
                    if (datesWorkingInMonth.Contains(dayInMonth))
                    {
                        new AbsenceDayModel() { Id = 0, IdAbsenceDates = idAbsence, TypeOfAbsence = typeOfAbsenc, Year = dayInMonth.Year, Month = dayInMonth.Month, Day = dayInMonth.Day }.SqlInsert();
                    }
                }
            }
            

        }

        public List<AbsenceDatesModel> FindTypeOfAbsenceAtYear(string year)
        {
            LibDateTime ldt = new LibDateTime();
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            List<AbsenceDatesModel> absenceDates = new List<AbsenceDatesModel>();
            absenceDates.AddRange(da.LoadAbsenceDates());
            List<AbsenceDatesModel> absenceDatesOfYear = new List<AbsenceDatesModel>();

            foreach (AbsenceDatesModel absence in absenceDates)
            {
                if ((LibDateTime.DateFromString(absence.DateStart).Year == int.Parse(year))|| (LibDateTime.DateFromString(absence.DateEnd).Year == int.Parse(year)))
                {
                    absenceDatesOfYear.Add(absence);
                }
            }
            return absenceDatesOfYear;
        }
        public string CalculateToInputedDate(string holidayFrom, string holidayTo)
        {
            LibDateTime ldt = new LibDateTime();
            int enterYear;
            int enterMonth;
            int enterDay;

            if (holidayTo.Count() > 0)
            {
                if (holidayTo[0] == '+')
                {
                    holidayTo = LibDateTime.StringFromDate(LibDateTime.DateFromString(holidayFrom).AddDays(int.Parse(holidayTo)));
                }

                else if (holidayTo.Split("-").Count() == 3)
                {
                    enterYear = int.Parse(holidayTo.Split("-")[0]);
                    enterMonth = int.Parse(holidayTo.Split("-")[1]);
                    enterDay = int.Parse(holidayTo.Split("-")[2]);
                    holidayTo = $"{enterYear:d4}-{enterMonth:D2}-{enterDay:D2}";
                }
                else if (holidayTo.Split("-").Count() == 2)
                {
                    enterMonth = int.Parse(holidayTo.Split("-")[0]);
                    enterDay = int.Parse(holidayTo.Split("-")[1]);
                    holidayTo = $"{setupYear:d4}-{enterMonth:D2}-{enterDay:D2}";
                }
                else if (holidayTo.Split("-").Count() == 1)
                {
                    enterDay = int.Parse(holidayTo);
                    holidayTo = $"{setupYear:d4}-{setupMonth:D2}-{enterDay:D2}";
                }
            }
            return holidayTo;
        }
        public List<AbsenceDatesModel> IsOnHollidays(string date)
        {
            LibDateTime ldt = new LibDateTime();

            SqlLiteDataAcces da = new SqlLiteDataAcces();
            List<AbsenceDatesModel> absenceDates = new List<AbsenceDatesModel>();
            List<AbsenceDatesModel> absenceDatesAll = new List<AbsenceDatesModel>();

            absenceDatesAll.AddRange(da.LoadAbsenceDates());

            foreach (AbsenceDatesModel absence in absenceDatesAll)
            {
                if ((DateTime.Compare(LibDateTime.DateFromString(date), LibDateTime.DateFromString(absence.DateStart)) >= 0) && (DateTime.Compare(LibDateTime.DateFromString(date), LibDateTime.DateFromString(absence.DateEnd)) <= 0))
                    absenceDates.Add(absence);
            }

            return absenceDates;
        }

    }
}
