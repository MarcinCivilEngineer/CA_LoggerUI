using CA_Loger;
using CA_LoggerUI.Models;
using CA_LoggerUI.Validator;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CA_LoggerUI.Views
{
    public class InsertHoursView
    {
        public InsertHoursView()
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();

        }
        public void InsertWorkingTime(int setupYear, int setupMonth, int setupDay, bool planning)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            LibDateTime ldt = new LibDateTime();
            DaysModel days = new DaysModel();

            string enter = "";
            bool validate = false;


            DateTime tmpSetupDate = new DateTime(setupYear, setupMonth, setupDay);
            AnsiConsole.MarkupLine($"Wprowadź godziny dla dnia [green]{setupYear}-{setupMonth}-{setupDay} {days.ToPolish(tmpSetupDate.DayOfWeek)}[/]");

            string startTimePlaning = da.LoadSetupEntry(name: "workingPlanningTimeStart").Value;

            (string logUser, DateTime logLastLogin) = LibUser.getMachineLogonName(".").Last();

            string startTimeFirst = null;
            if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: planning).Count() > 0)
            {
                startTimeFirst = da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: planning).First().WorkTimeStart;
            }
            else if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).Count() > 0)
            {
                startTimeFirst = da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).First().WorkTimeStart;
            }

            if (startTimeFirst == null)
            {
                if (logLastLogin.Date == tmpSetupDate.Date)
                {
                    startTimeFirst = $"{logLastLogin.Hour}:{logLastLogin.Minute}";
                }
                else
                {
                    if ((da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).Count() > 0))
                    {
                        startTimePlaning = da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).First().WorkTimeStart;
                        startTimeFirst = startTimePlaning;
                    }
                    else if (logLastLogin.Date != tmpSetupDate.Date)
                    {
                        startTimeFirst = startTimePlaning;
                    }
                    else
                    {

                        startTimeFirst = $"{DateTime.Now.Hour}:{DateTime.Now.Minute}";
                    }
                }
            }

            string startTime = "";
            validate = false;
            while (!validate)
            {
                enter = startTime = AnsiConsole.Ask<string>($"Rozpoczęcie pracy (wg planu {startTimePlaning}), '[green]x[/]' usuń wpis:", startTimeFirst);
                if (TimeValidator.IsHour(enter)) { validate = true; }
                if (TimeValidator.IsExualTo(enter, new List<string>() { "x" })) { validate = true; }
                if (validate == false)
                {
                    AnsiConsole.Markup("[red]Błąd: Niepoprawny format, dopuszczalne: 00:00, x[/] ");
                }
            }

            if (startTimeFirst.Count() > 0)
            {
                if (startTime[0] == '+' || startTime[0] == '-')
                {
                    startTime = LibDateTime.HoursAddMinutes(startTimeFirst, int.Parse(startTime));
                }
            }

            if (startTime == "x")
            {
                startTime = "";
            }


            string endTimeFirst = "";
            if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay).Count() > 0)
            {
                endTimeFirst = da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay).First().WorkTimeEnd;
            }

            if ((startTime != "") && (Regex.Replace(startTime, "[^0-9:]", "").Split(":").Count() == 2))
            {
                //DateTime tmpStartTime = new DateTime(setupYear, setupMonth, setupDay, int.Parse(startTime.Split(":")[0]), int.Parse(startTime.Split(":")[1]), 0);
                int tmpMinutesAtWork = LibDateTime.MinutesBetweenTime(da.LoadSetupEntry(name: "workingPlanningTimeEnd").Value, da.LoadSetupEntry(name: "workingPlanningTimeStart").Value);
                endTimeFirst = LibDateTime.HoursAddMinutes(startTime, tmpMinutesAtWork);
            }


            if (new DateTime(setupYear, setupMonth, setupDay).Date == DateTime.Now.Date)
            {
                if (endTimeFirst != "")
                {
                    if (LibDateTime.MinutesBetweenTime($"{DateTime.Now.Hour}:{DateTime.Now.Minute}", endTimeFirst) > 0)
                    {
                        endTimeFirst = $"{DateTime.Now.Hour}:{DateTime.Now.Minute}";
                    }
                }
                else
                {
                    endTimeFirst = $"{DateTime.Now.Hour}:{DateTime.Now.Minute}";

                }
            }
            string endTimePlanning = da.LoadSetupEntry(name: "workingPlanningTimeEnd").Value;
            if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: planning).Count() > 0)
            {
                endTimeFirst = da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: planning).First().WorkTimeEnd;
            }

            string endTime = "";
            validate = false;
            while (!validate)
            {
                enter = endTime = AnsiConsole.Ask<string>($"Zakończenie pracy (wg planu {endTimePlanning}), '[green]x[/]' usuń wpis", endTimeFirst);
                if (TimeValidator.IsHour(enter)) { validate = true; }
                if (TimeValidator.IsExualTo(enter, new List<string>() { "x" })) { validate = true; }
                if (TimeValidator.IsAddMinutes(enter)) { validate = true; }
                if (TimeValidator.IsMinusMinutes(enter)) { validate = true; }
                if (validate == false)
                {
                    AnsiConsole.Markup("[red]Błąd: Niepoprawny format, dopuszczalne: 00:00, x, +0, -0, 0[/] ");
                }
            }

            if (endTime.Count() > 0)
            {
                if (endTime[0] == '+' || endTime[0] == '-')
                {
                    endTime = LibDateTime.HoursAddMinutes(endTimeFirst, int.Parse(endTime));
                }
            }
            if (endTime == "x")
            {
                endTime = "";
            }

            if ((endTime != "") && (startTime != "") && (Regex.Replace(endTime, "[^0-9:]", "").Split(":").Count() == 2) && (Regex.Replace(startTime, "[^0-9:]", "").Split(":").Count() == 2))
            {
                string timeAtWork = LibDateTime.HoursBetweenTime(endTime, startTime);

                AnsiConsole.MarkupLine($"Czas w pracy{(planning == true ? " (planowany)" : "")}: [red]{timeAtWork}[/]");
            }

            string commentFirst = "";
            if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).Count() > 0)
            {
                commentFirst = da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).First().Comment;
            }
            if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: false).Count() > 0)
            {
                commentFirst = da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: false).First().Comment;
            }

            string comment = AnsiConsole.Ask<string>($"Komentarz dnia: '[green]x[/]' usuń komentarz", commentFirst);
            if (comment == "x")
            {
                comment = "";
            }


            if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: planning).Count() > 0)
            {

                new WorkingDayModel() { Id = da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: planning).First().Id, Year = setupYear, Month = setupMonth, Day = setupDay, Comment = comment, WorkTimeEnd = endTime, WorkTimeStart = startTime, Planing = planning }.SqlUpdate();
                if ((startTime == "") || (endTime == ""))
                {
                    da.UsunWorkingDay(da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: planning).First().Id);
                }
            }
            else
            {
                if ((startTime != "") || (endTime != ""))
                {
                    new WorkingDayModel() { Id = 0, Year = setupYear, Month = setupMonth, Day = setupDay, Comment = comment, WorkTimeEnd = endTime, WorkTimeStart = startTime, Planing = planning }.SqlInsert(); ;
                }
            }




            List<WorkingDayBreaksModel> breaksAtWork = new List<WorkingDayBreaksModel>(); ;
            breaksAtWork.AddRange(da.LoadWorkingDayBreaks(year: setupYear, month: setupMonth, day: setupDay, planing: false));

            List<WorkingDayBreaksModel> breaksAtWorkPlaning = new List<WorkingDayBreaksModel>();
            breaksAtWorkPlaning.AddRange(da.LoadWorkingDayBreaks(year: setupYear, month: setupMonth, day: setupDay, planing: true));



            if ((breaksAtWork.Count() > 0) | (breaksAtWorkPlaning.Count() > 0))
            {
                string commentBreakAtWork = "";
                WorkingDayBreaksModel tmpWorkingDayBreaks = new WorkingDayBreaksModel();
                WorkingDayBreaksModel tmpWorkingDayBreaksPlaning = new WorkingDayBreaksModel();
                for (int a = 0; a < Math.Max(breaksAtWork.Count(), breaksAtWorkPlaning.Count()); a++)
                {
                    int tmpId = 0;
                    if (breaksAtWork.Count() > a)
                    {
                        tmpWorkingDayBreaks = breaksAtWork.ElementAt(a);
                        tmpId = tmpWorkingDayBreaks.Id;
                    }
                    else
                    {
                        tmpWorkingDayBreaks = new WorkingDayBreaksModel() { BreakTimeEnd = "", BreakTimeStart = "", Comment = "" };
                    }

                    if (breaksAtWorkPlaning.Count() > a)
                    {
                        tmpWorkingDayBreaksPlaning = breaksAtWorkPlaning.ElementAt(a);
                        tmpId = tmpWorkingDayBreaksPlaning.Id;
                    }
                    else
                    {
                        tmpWorkingDayBreaksPlaning = new WorkingDayBreaksModel() { BreakTimeEnd = "", BreakTimeStart = "", Comment = "" };
                    }

                    string breakTimeStartFirst = "";
                    if (tmpWorkingDayBreaks.BreakTimeStart != "")
                    {
                        breakTimeStartFirst = tmpWorkingDayBreaks.BreakTimeStart;
                    }
                    else if (new DateTime(setupYear, setupMonth, setupDay).CompareTo(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)) == 0)
                    {
                        breakTimeStartFirst = DateTime.Now.Hour.ToString();
                    }
                    else
                    {
                        breakTimeStartFirst = startTime;
                    }

                    string startBreakAtWork = "";
                    validate = false;
                    while (!validate)
                    {
                        enter = startBreakAtWork = AnsiConsole.Ask<string>($"Rozpoczęcie przerwy w pracy [green]'x'[/] usuń", tmpWorkingDayBreaksPlaning.BreakTimeStart != "" ? tmpWorkingDayBreaksPlaning.BreakTimeStart : breakTimeStartFirst);
                        if (TimeValidator.IsHour(enter)) { validate = true; }
                        if (TimeValidator.IsExualTo(enter, new List<string>() { "x" })) { validate = true; }
                        if (validate == false)
                        {
                            AnsiConsole.Markup($"[red]Błąd: Niepoprawny format, dopuszczalne: 00:00 > {startTime}, x[/] ");
                        }
                    }

                    if (startBreakAtWork == "x")
                    {
                        startBreakAtWork = "";
                    }

                    string endBreakAtWork = "";
                    validate = false;
                    while (!validate)
                    {
                        enter = endBreakAtWork = AnsiConsole.Ask<string>($"Zakończenie przerwy w pracy [green]'x'[/] usuń", tmpWorkingDayBreaksPlaning.BreakTimeEnd != "" ? tmpWorkingDayBreaksPlaning.BreakTimeEnd : tmpWorkingDayBreaks.BreakTimeEnd);
                        if (TimeValidator.IsHour(enter)) { validate = true; }
                        if (TimeValidator.IsExualTo(enter, new List<string>() { "x" })) { validate = true; }
                        if (TimeValidator.IsAddMinutes(enter)) { validate = true; }
                        if (TimeValidator.IsMinusMinutes(enter)) { validate = true; }
                        if (validate == false)
                        {
                            AnsiConsole.Markup("[red]Błąd: Niepoprawny format, dopuszczalne: 00:00, x, +0, -0[/] ");
                        }
                    }
                        if (endBreakAtWork == "x")
                    {
                        endBreakAtWork = "";
                    }
                    if (endBreakAtWork.Count() > 0)
                    {
                        if (endBreakAtWork[0] == '+' || endBreakAtWork[0] == '-')
                        {
                            endBreakAtWork = LibDateTime.HoursAddMinutes(startBreakAtWork, int.Parse(endBreakAtWork));
                        }
                    }

                    if ((startBreakAtWork != "") && (endBreakAtWork != ""))
                    {
                        string timeAtWorkBreak = LibDateTime.HoursBetweenTime(endBreakAtWork, startBreakAtWork);

                        AnsiConsole.MarkupLine($"Czas przerwy w pracy{(planning == true ? " (planowany)" : "")}: [red]{timeAtWorkBreak}[/]");
                    }

                    commentBreakAtWork = AnsiConsole.Ask<string>($"Komentarz do przerwy [green]'x'[/] usuń", tmpWorkingDayBreaksPlaning.Comment != "" ? tmpWorkingDayBreaksPlaning.Comment : tmpWorkingDayBreaks.Comment);
                    if (commentBreakAtWork.Length > 0)
                    {
                        if (commentBreakAtWork == "x")
                        {
                            commentBreakAtWork = "";
                        }
                    }

                    if ((startBreakAtWork == "") && (endBreakAtWork == ""))
                    {
                        da.UsunWorkingDayBreaks(id: tmpId);
                    }
                    else
                    {
                        new WorkingDayBreaksModel() { Id = tmpId, BreakTimeStart = startBreakAtWork, BreakTimeEnd = endBreakAtWork, Comment = commentBreakAtWork, Year = setupYear, Month = setupMonth, Day = setupDay, Planing = planning }.SqlUpdate();
                    }
                }
            }


            bool defineBreaks = true;
            while (defineBreaks)
            {
                breaksAtWork = da.LoadWorkingDayBreaks(year: setupYear, month: setupMonth, day: setupDay, planing: false);
                breaksAtWorkPlaning = da.LoadWorkingDayBreaks(year: setupYear, month: setupMonth, day: setupDay, planing: true);
                defineBreaks = AnsiConsole.Confirm($"Wprowadzić nową przerwę w pracy?", ((planning == false) && (breaksAtWork.Count() < breaksAtWorkPlaning.Count()) ? true : false));
                if (defineBreaks)
                {
                    string startBreakAtWork = "";
                    validate = false;
                    while (!validate)
                    {
                        enter = startBreakAtWork = AnsiConsole.Ask<string>($"Rozpoczęcie przerwy w pracy:", startTime);
                        if (TimeValidator.IsHour(enter)) { validate = true; }
                        if (TimeValidator.IsExualTo(enter, new List<string>() { "x" })) { validate = true; }
                        if (TimeValidator.IsAddMinutes(enter)) { validate = true; startBreakAtWork = LibDateTime.HoursAddMinutes(startTime, int.Parse(startBreakAtWork)); }
                        if (TimeValidator.IsMinusMinutes(enter)) { validate = true; startBreakAtWork = LibDateTime.HoursAddMinutes(startTime, int.Parse(startBreakAtWork)); }
                        if (validate == false)
                        {
                            AnsiConsole.Markup("[red]Błąd: Niepoprawny format, dopuszczalne: 00:00, x, +0, -0, 0[/] ");
                        }
                    }

                    string endBreakAtWork = "";
                    validate = false;
                    while (!validate)
                    {
                        enter = endBreakAtWork = AnsiConsole.Ask<string>($"Zakończenie przerwy w pracy:", startBreakAtWork);
                        if (TimeValidator.IsHour(enter)) { validate = true; }
                        if (TimeValidator.IsExualTo(enter, new List<string>() { "x" })) { validate = true; }
                        if (TimeValidator.IsAddMinutes(enter)) { validate = true; }
                        if (TimeValidator.IsMinusMinutes(enter)) { validate = true; }
                        if (validate == false)
                        {
                            AnsiConsole.Markup("[red]Błąd: Niepoprawny format, dopuszczalne: 00:00, x, +0, -0, 0[/] ");
                        }
                    }
                    if (endBreakAtWork.Count() > 0)
                    {
                        if (endBreakAtWork[0] == '+' || endBreakAtWork[0] == '-')
                        {
                            endBreakAtWork = LibDateTime.HoursAddMinutes(startBreakAtWork, int.Parse(endBreakAtWork));
                        }
                    }
                    if ((startBreakAtWork != "") && (endBreakAtWork != ""))
                    {
                        string timeAtWorkBreak = LibDateTime.HoursBetweenTime(endBreakAtWork, startBreakAtWork);

                        AnsiConsole.MarkupLine($"Czas przerwy w pracy{(planning == true ? " (planowany)" : "")}: [red]{timeAtWorkBreak}[/]");
                    }
                    string commentBreakAtWork = AnsiConsole.Ask<string>($"Komentarz do przerwy:", "");
                    new WorkingDayBreaksModel() { Id = 0, BreakTimeStart = startBreakAtWork, BreakTimeEnd = endBreakAtWork, Comment = commentBreakAtWork, Year = setupYear, Month = setupMonth, Day = setupDay, Planing = planning }.SqlInsert();
                } 
                else
                {
                    AnsiConsole.MarkupLine("");
                }
            }

        }


    }
}
