using CA_Loger;
using CA_LoggerUI.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CA_LoggerUI.Views
{
    public class DayStatusView
    {
        public DayStatusView()
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();

        }
        public void ShortStatus(int setupYear, int setupMonth)
        {
            RaportView raportView = new RaportView();
            raportView.GenerateRaport(setupYear, setupMonth, true,"spectre",false);
        }
        public void DayStatus(int setupYear, int setupMonth, int setupDay)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            LibDateTime ldt = new LibDateTime();

            DateTime tmpSetupDate = new DateTime(setupYear, setupMonth, setupDay);
            WorkingDayModel currentDayPlanning = new WorkingDayModel() { Id = 0, Comment = "", WorkTimeEnd = "", WorkTimeStart = "" };

            int tmpTo=0;
            
            if ((tmpSetupDate.Year == DateTime.Now.Year) && (tmpSetupDate.Month == DateTime.Now.Month))
            {
                tmpTo = DateTime.Now.Day;
            } 
            else if (tmpSetupDate> DateTime.Now)
            {
                tmpTo = 0;
            } else
            {
                tmpTo = DateTime.DaysInMonth(setupYear, setupMonth);
            }


            AnsiConsole.MarkupLine($"Status z dnia [red]{tmpSetupDate.Year}-{tmpSetupDate.Month}-{tmpSetupDate.Day}, {(CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(new DateTime(tmpSetupDate.Year, tmpSetupDate.Month, tmpSetupDate.Day).DayOfWeek))}[/]");

            if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).Count() > 0)
            {
                currentDayPlanning = da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).First();
            }


            if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: false).Count() > 0)
            {
                WorkingDayModel currentDay = da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: false).First();

                

                if (currentDay.WorkTimeStart != "")
                {
                    AnsiConsole.MarkupLine($"Rozpoczęcie pracy: [red]{currentDay.WorkTimeStart}[/]{(currentDayPlanning.WorkTimeStart != "" ? $", wg planu [yellow]{currentDayPlanning.WorkTimeStart}[/]" : "")}");
                }



                string breaksTime = "00:00";
                if (da.LoadWorkingDayBreaks(year: setupYear, month: setupMonth, day: setupDay, planing: false).Count() > 0)
                {

                    foreach (WorkingDayBreaksModel workingDayBreak in da.LoadWorkingDayBreaks(year: setupYear, month: setupMonth, day: setupDay, planing: false))
                    {
                        breaksTime = LibDateTime.HoursAddMinutes(breaksTime, LibDateTime.MinutesBetweenTime(workingDayBreak.BreakTimeEnd, workingDayBreak.BreakTimeStart));
                    }
                }

                string breaksTimePlaning = "00:00";
                if (da.LoadWorkingDayBreaks(year: setupYear, month: setupMonth, day: setupDay, planing: true).Count() > 0)
                {
                    foreach (WorkingDayBreaksModel workingDayBreak in da.LoadWorkingDayBreaks(year: setupYear, month: setupMonth, day: setupDay, planing: true))
                    {
                        breaksTimePlaning = LibDateTime.HoursAddMinutes(breaksTimePlaning, LibDateTime.MinutesBetweenTime(workingDayBreak.BreakTimeEnd, workingDayBreak.BreakTimeStart));
                    }
                }


                if (breaksTime != "00:00") { 
                    AnsiConsole.MarkupLine($"Łączne przerwy w pracy: [red]{breaksTime}[/]{(breaksTimePlaning != "00:00" ? $", wg planu [yellow]{breaksTimePlaning}[/]" : "")}");
                }  


                if (currentDay.WorkTimeEnd != "")
                {
                    AnsiConsole.MarkupLine($"Zakończnie pracy: [red]{currentDay.WorkTimeEnd}[/]{(currentDayPlanning.WorkTimeEnd != "" ? $", wg planu [yellow]{currentDayPlanning.WorkTimeEnd}[/]" : "")}");
                }
                else if (currentDay.WorkTimeStart != "")
                {
                    int tmpMinutesAtWork = LibDateTime.MinutesBetweenTime(da.LoadSetupEntry(name: "workingPlanningTimeEnd").Value, da.LoadSetupEntry(name: "workingPlanningTimeStart").Value);
                    string endTimeFirst = LibDateTime.HoursAddMinutes(currentDay.WorkTimeStart, tmpMinutesAtWork);
                    AnsiConsole.MarkupLine($"Zalecane wyjście z pracy: [yellow]{LibDateTime.HoursAddMinutes(endTimeFirst,LibDateTime.MinutesFromTime(breaksTime))}[/]");
                }




                if ((currentDay.WorkTimeEnd != "") && (currentDay.WorkTimeStart != ""))
                {

                    AnsiConsole.MarkupLine($"Czas w pracy: [red]{LibDateTime.HoursAddMinutes(LibDateTime.HoursBetweenTime(currentDay.WorkTimeEnd, currentDay.WorkTimeStart), -LibDateTime.MinutesFromTime(breaksTime))}[/]");

                }

            }

        
            else if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).Count() > 0)
            {
                
                string breaksTimePlaning = "00:00";
                if (da.LoadWorkingDayBreaks(year: setupYear, month: setupMonth, day: setupDay, planing: true).Count() > 0)
                {
                    foreach (WorkingDayBreaksModel workingDayBreak in da.LoadWorkingDayBreaks(year: setupYear, month: setupMonth, day: setupDay, planing: true))
                    {
                        breaksTimePlaning = LibDateTime.HoursAddMinutes(breaksTimePlaning, LibDateTime.MinutesBetweenTime(workingDayBreak.BreakTimeEnd, workingDayBreak.BreakTimeStart));
                    }
                }

                if ((currentDayPlanning.WorkTimeStart != "")&& (LibDateTime.MinutesFromTime(currentDayPlanning.WorkTimeStart) > 0))
                {
                    AnsiConsole.MarkupLine($"Planowane rozpoczęcie pracy: [yellow]{currentDayPlanning.WorkTimeStart}[/]");
                }

                if ((currentDayPlanning.WorkTimeEnd != "")&& (LibDateTime.MinutesFromTime(currentDayPlanning.WorkTimeEnd) > 0))
                {
                    AnsiConsole.MarkupLine($"Planowane zakończenie pracy: [yellow]{currentDayPlanning.WorkTimeEnd}[/]");
                }

                if ((breaksTimePlaning != "")&&(LibDateTime.MinutesFromTime(breaksTimePlaning)>0))
                {
                    AnsiConsole.MarkupLine($"Planowane przerwy w pracy [yellow]{breaksTimePlaning}[/]");
                }

                if ((currentDayPlanning.WorkTimeStart != "") && (currentDayPlanning.WorkTimeEnd != ""))
                {
                    AnsiConsole.MarkupLine($"Planowany czas w pracy: [yellow]{ LibDateTime.HoursAddMinutes(LibDateTime.HoursBetweenTime(currentDayPlanning.WorkTimeEnd, currentDayPlanning.WorkTimeStart), -LibDateTime.MinutesFromTime(breaksTimePlaning))}[/]");
                }
                

            }

        }

        public int MinutesToWorkInMonth(int daysAtWork)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            LibDateTime ldt = new LibDateTime();

            return LibDateTime.MinutesBetweenTime(da.LoadSetupEntry(name: "workingPlanningTimeEnd").Value, da.LoadSetupEntry(name: "workingPlanningTimeStart").Value) * daysAtWork;
        }

        public (string, List<string>) WorkedTimeFromTo(int year, int month, int from, int to, List<DateTime> workDates, bool planing)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            LibDateTime ldt = new LibDateTime();

            WorkingDayModel workingDayModel = new WorkingDayModel();
            List<WorkingDayBreaksModel> workingDayBreaksModel = new List<WorkingDayBreaksModel>();

            List<string> planingOrWithoutTime = new List<string>();

            int minutesAtWork = 0;
            int minutesBreaks = 0;
            for (int a = from; a <= to; a++)
            {
                if (planing == true)
                {
                    if (workDates.Contains(new DateTime(year, month, a)))
                    {
                        minutesAtWork = minutesAtWork + AtDayTimeWorkPlaning(year, month, a).Item1;
                        if ((AtDayTimeWorkPlaning(year, month, a).Item2))
                        {
                            planingOrWithoutTime.Add($"{a}");
                        }
                    }
                    else if (da.LoadWorkingDay(year: year, month: month, day: a, planing: true).Count()>0)
                    {
                        minutesAtWork = minutesAtWork + AtDayTimeWorkPlaning(year, month, a).Item1;
                    }
                }
                else
                {
                    minutesAtWork = minutesAtWork + AtDayTimeWork(year, month, a).Item1;
                    if ((AtDayTimeWork(year, month, a).Item2) && (workDates.Contains(new DateTime(year, month, a))))
                    {
                        planingOrWithoutTime.Add($"{a}");
                    }
                }
            }

            for (int a = from; a <= to; a++)
            {
                minutesBreaks = minutesBreaks + AtDayBreaksTime(year,month,a,planing).Item1;
            }

            //Remove AbsenceDay
            List<string> planingOrWithutTimeAndAbsence = new List<string>();
            foreach (string withoutTimeDate in planingOrWithoutTime)
            {
                if (da.LoadAbsenceDay(year:year, month:month, day: int.Parse(withoutTimeDate)).Count()==0)
                {
                    planingOrWithutTimeAndAbsence.Add(withoutTimeDate);

                }
            }

            return (LibDateTime.HoursFromMinutes(minutesAtWork - minutesBreaks), planingOrWithutTimeAndAbsence);
        }

        public (int,bool) AtDayTimeWork(int setupYear, int setupMonth, int setupDay)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            LibDateTime ldt = new LibDateTime();
            WorkingDayModel workingDay = new WorkingDayModel();
            WorkingDayModel workingDayPlanning = new WorkingDayModel() { WorkTimeEnd = "", WorkTimeStart = "", Comment = "" };
            int minutes = 0;
            bool withoutTime=true;

            if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: false).Count() > 0)
            {
                workingDay = da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: false).First();
                if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).Count() > 0) { 
                    workingDayPlanning = da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).First();
                }
                if ((workingDay.WorkTimeEnd != "") && (workingDay.WorkTimeStart != ""))
                {
                    minutes = LibDateTime.MinutesBetweenTime(workingDay.WorkTimeEnd, workingDay.WorkTimeStart);
                    withoutTime = false;
                } 
                else if ((workingDay.WorkTimeEnd == "") && (workingDay.WorkTimeStart != "")&&(workingDayPlanning.WorkTimeEnd!=""))
                {
                    if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).Count() > 0)
                    {
                        minutes = LibDateTime.MinutesBetweenTime(workingDayPlanning.WorkTimeEnd, workingDay.WorkTimeStart);
                        withoutTime = true;
                    }
                }
                else if ((workingDay.WorkTimeEnd == "") && (workingDay.WorkTimeStart != "") && (workingDayPlanning.WorkTimeEnd == ""))//
                {
                    if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).Count() > 0)
                    {
                        minutes = LibDateTime.MinutesBetweenTime(da.LoadSetupEntry(name: "workingPlanningTimeEnd").Value, workingDay.WorkTimeStart);
                        withoutTime = true;
                    }
                }
                else if ((workingDay.WorkTimeEnd != "") && (workingDay.WorkTimeStart == "") && (workingDayPlanning.WorkTimeStart != ""))
                {
                    if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).Count() > 0)
                    {
                        minutes = LibDateTime.MinutesBetweenTime(workingDay.WorkTimeEnd, workingDayPlanning.WorkTimeStart);
                        withoutTime = true;
                    }
                }
                else if ((workingDay.WorkTimeEnd != "") && (workingDay.WorkTimeStart == "") && (workingDayPlanning.WorkTimeStart != ""))
                {
                    if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).Count() > 0)
                    {
                        minutes = LibDateTime.MinutesBetweenTime(workingDay.WorkTimeEnd, da.LoadSetupEntry(name: "workingPlanningTimeStart").Value);
                        withoutTime = true;
                    }
                }
            }
            return (minutes, withoutTime);
        }


        public (int, bool) AtDayTimeWorkPlaning(int setupYear, int setupMonth, int setupDay)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            LibDateTime ldt = new LibDateTime();
            WorkingDayModel workingDay = new WorkingDayModel();
            int minutes = 0;
            bool withoutTime = true;

            if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).Count() > 0)
            {
                workingDay = da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).First();

                if ((workingDay.WorkTimeEnd != "") && (workingDay.WorkTimeStart != ""))
                {
                    minutes = LibDateTime.MinutesBetweenTime(workingDay.WorkTimeEnd, workingDay.WorkTimeStart);
                    withoutTime = false;
                }

                else if ((workingDay.WorkTimeEnd == "") && (workingDay.WorkTimeStart != ""))//
                {
                    if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).Count() > 0)
                    {
                        minutes = LibDateTime.MinutesBetweenTime(da.LoadSetupEntry(name: "workingPlanningTimeEnd").Value, workingDay.WorkTimeStart);
                        withoutTime = true;
                    }
                }

                else if ((workingDay.WorkTimeEnd != "") && (workingDay.WorkTimeStart == ""))
                {
                    if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).Count() > 0)
                    {
                        minutes = LibDateTime.MinutesBetweenTime(workingDay.WorkTimeEnd, da.LoadSetupEntry(name: "workingPlanningTimeStart").Value);
                        withoutTime = true;
                    }
                }
            }
            else
            {
                minutes = LibDateTime.MinutesBetweenTime(da.LoadSetupEntry(name: "workingPlanningTimeEnd").Value, da.LoadSetupEntry(name: "workingPlanningTimeStart").Value);
                withoutTime = true;
            }
            
            return (minutes, withoutTime);
        }


        public (int, bool) AtDayBreaksTime(int setupYear, int setupMonth, int setupDay, bool planing)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            LibDateTime ldt = new LibDateTime();
            List<WorkingDayBreaksModel> workingDayBreaks = new List<WorkingDayBreaksModel>();
            List<WorkingDayBreaksModel> workingDayBreaksPlanning = new List<WorkingDayBreaksModel>();
            int minutes = 0;
            bool withoutTime = true;

            if (da.LoadWorkingDayBreaks(year: setupYear, month: setupMonth, day: setupDay, planing: false).Count() > 0)
            {
                workingDayBreaks.AddRange(da.LoadWorkingDayBreaks(year: setupYear, month: setupMonth, day: setupDay, planing: planing));
                foreach (WorkingDayBreaksModel breaks in workingDayBreaks)
                {
                    if ((breaks.BreakTimeEnd != "") && (breaks.BreakTimeStart != ""))
                    {
                        minutes = LibDateTime.MinutesBetweenTime(breaks.BreakTimeEnd, breaks.BreakTimeStart);
                        withoutTime = false;
                    }
                }
            }
            return (minutes, withoutTime);
        }



    }
}
