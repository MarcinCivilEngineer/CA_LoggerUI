using CA_Loger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;
using CA_LoggerUI.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.IO;
using System.Diagnostics;

namespace CA_LoggerUI.Views
{
    public class RaportView
    {
        private SqlLiteDataAcces da = new SqlLiteDataAcces();
        public RaportView()
        {

        }

        public (Collection<RaportTableModel>, List<string>) GenerateRaport(int setupYear, int setupMonth, bool showStatus, string type,bool hideComment)
        {
            List<string> outParameters = new();
            //List<RaportTableModel> raportTablePlaning = new List<RaportTableModel>();

            string freeDaysOfWorkCurrent = da.LoadSetupEntry(name: $"freeDaysOfWork{setupYear}-{setupMonth}").Value;
            string daysOfWorkCurrent = da.LoadSetupEntry(name: $"daysOfWork{setupYear}-{setupMonth}").Value;

            DefaultsSettingsView defaultsSettings = new DefaultsSettingsView();
            (List<DateTime> tmpDays, List<DateTime> tmpWorkDays) = defaultsSettings.CreateCalender(setupYear: setupYear, setupMonth: setupMonth, freeDaysOfWeek: defaultsSettings.LoadListOfFreeDays(), freeDaysOfWork: freeDaysOfWorkCurrent, daysOfWork: daysOfWorkCurrent, false);



            RaportTableStyleModel style = new RaportTableStyleModel(type);
            Collection<RaportTableModel> raportTable = new Collection<RaportTableModel>();

            bool printedDay = false;

            int minutesAtWork = 0;
            int breaksAtWork = 0;
            int minutesAtWorkPlaning = 0;
            int breaksAtWorkPlaning = 0;
            int minutesToWorkAtMonth = 0;

            for (int day = 1; day <= DateTime.DaysInMonth(setupYear, setupMonth); day++)
            {
                printedDay = false;
                WorkingDayModel workingDayModel = new WorkingDayModel() { Comment = "", WorkTimeEnd = "", WorkTimeStart = "" };
                WorkingDayModel workingDayModelPlaning = new WorkingDayModel() { Comment = "", WorkTimeEnd = "", WorkTimeStart = "" };
                List<WorkingDayBreaksModel> workingDayBreaksModel = new List<WorkingDayBreaksModel>();
                List<WorkingDayBreaksModel> workingDayBreaksModelPlaning = new List<WorkingDayBreaksModel>();
                AbsenceDayModel absenceDayModel = new AbsenceDayModel();
                AbsenceDatesModel absenceDatesModel = new AbsenceDatesModel();

                string workTimeStartString = "";
                string workTimeEndString = "";

                string workTimeBreakStartString = "";
                string workTimeBreakEndString = "";

                DateTime dayOfmonth = new DateTime(setupYear, setupMonth, day);

                if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: day, planing: false).Count() > 0)
                {
                    workingDayModel = da.LoadWorkingDay(year: setupYear, month: setupMonth, day: day, planing: false).First();
                }
                if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: day, planing: true).Count() > 0)
                {
                    workingDayModelPlaning = da.LoadWorkingDay(year: setupYear, month: setupMonth, day: day, planing: true).First();
                }
                if (da.LoadWorkingDayBreaks(year: setupYear, month: setupMonth, day: day, planing: false).Count() > 0)
                {
                    workingDayBreaksModel = da.LoadWorkingDayBreaks(year: setupYear, month: setupMonth, day: day, planing: false);
                }
                if (da.LoadWorkingDayBreaks(year: setupYear, month: setupMonth, day: day, planing: true).Count() > 0)
                {
                    workingDayBreaksModelPlaning = da.LoadWorkingDayBreaks(year: setupYear, month: setupMonth, day: day, planing: true);
                }
                if (da.LoadAbsenceDay(year: setupYear, month: setupMonth, day: day).Count() > 0)
                {
                    absenceDayModel = da.LoadAbsenceDay(year: setupYear, month: setupMonth, day: day).First();
                    absenceDatesModel = da.LoadAbsenceDates(id: absenceDayModel.IdAbsenceDates).First();
                }


                string rowDay = $"{style.Normal.DayPre}{style.Normal.DayPost}";
                if (!tmpWorkDays.Contains(dayOfmonth))
                {
                    rowDay = $"{style.Free.DayPre}{CultureInfo.CurrentCulture.DateTimeFormat.GetShortestDayName(dayOfmonth.DayOfWeek)} {day:d2}{style.Free.DayPost}";
                }
                else if (dayOfmonth == new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day))
                {
                    rowDay = $"{style.Current.DayPre}{CultureInfo.CurrentCulture.DateTimeFormat.GetShortestDayName(dayOfmonth.DayOfWeek)} {day:d2}{style.Current.DayPost}";
                }
                else
                {
                    rowDay = $"{style.Normal.DayPre}{CultureInfo.CurrentCulture.DateTimeFormat.GetShortestDayName(dayOfmonth.DayOfWeek)} {day:d2}{style.Normal.DayPost}";
                }

                string rowWorkStart = $"{style.Normal.TimeFromPre}{style.Normal.TimeFromPost}";
                if (workingDayModel.WorkTimeStart != "")
                {
                    rowWorkStart = $"{style.Normal.TimeFromPre}{workingDayModel.WorkTimeStart}{style.Normal.TimeFromPost}";
                    workTimeStartString = workingDayModel.WorkTimeStart;
                }
                else if
                    (workingDayModelPlaning.WorkTimeStart != "")
                {
                    rowWorkStart = $"{style.Planing.TimeFromPre}{workingDayModelPlaning.WorkTimeStart}{style.Planing.TimeFromPost}";
                    workTimeStartString = workingDayModelPlaning.WorkTimeStart;
                }
                else if ((tmpWorkDays.Contains(dayOfmonth)) && (da.LoadAbsenceDay(year: setupYear, month: setupMonth, day: day).Count() == 0))
                {
                    rowWorkStart = $"{style.Planing.TimeFromPre}{da.LoadSetupEntry(name: "workingPlanningTimeStart").Value}{style.Planing.TimeFromPost}";
                    workTimeStartString = da.LoadSetupEntry(name: "workingPlanningTimeStart").Value;
                }

                string rowWorkEnd = $"{style.Normal.TimeToPre}{style.Normal.TimeToPost}";
                if (workingDayModel.WorkTimeEnd != "")
                {
                    rowWorkEnd = $"{style.Normal.TimeToPre}{workingDayModel.WorkTimeEnd}{style.Normal.TimeToPost}";
                    workTimeEndString = workingDayModel.WorkTimeEnd;
                }
                else if
                    (workingDayModelPlaning.WorkTimeEnd != "")
                {
                    rowWorkEnd = $"{style.Planing.TimeToPre}{workingDayModelPlaning.WorkTimeEnd}{style.Planing.TimeToPost}";
                    workTimeEndString = workingDayModelPlaning.WorkTimeEnd;
                }
                else if ((tmpWorkDays.Contains(dayOfmonth)) && (da.LoadAbsenceDay(year: setupYear, month: setupMonth, day: day).Count() == 0))
                {
                    rowWorkEnd = $"{style.Planing.TimeToPre}{da.LoadSetupEntry(name: "workingPlanningTimeEnd").Value}{style.Planing.TimeToPost}";
                    workTimeEndString = da.LoadSetupEntry(name: "workingPlanningTimeEnd").Value;
                }

                string rowComment = $"{style.Normal.CommentPre}{style.Normal.CommentPost}";
                if (workingDayModel.Comment != "")
                {
                    rowComment = $"{style.Normal.CommentPre}{workingDayModel.Comment}{style.Normal.CommentPost}";
                }
                if (workingDayModelPlaning.Comment != "")
                {
                    rowComment = $"{style.Planing.CommentPre}{workingDayModelPlaning.Comment}{style.Planing.CommentPost}";
                }

                string rowTimeAtWork = $"{style.Normal.TimeAtWorkPre}{style.Normal.TimeAtWorkPost}";
                if ((workingDayModel.WorkTimeEnd != "") && (workingDayModel.WorkTimeStart != ""))
                {
                    int tmpMinutesAtWork = LibDateTime.MinutesBetweenTime(workTimeEndString, workTimeStartString) - LibDateTime.MinutesFromTime(BreakHoursAtWork(workingDayBreaksModel));
                    rowTimeAtWork = $"{style.Normal.TimeAtWorkPre}{LibDateTime.HoursFromMinutes(tmpMinutesAtWork)}{style.Normal.TimeAtWorkPost}";
                    minutesAtWork = minutesAtWork + tmpMinutesAtWork;
                }
                else
                {
                    if ((workTimeEndString != "") && (workTimeStartString != ""))
                    {
                        if (dayOfmonth.CompareTo(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)) < 0)
                        {
                            int tmpMinutesAtWork = LibDateTime.MinutesBetweenTime(workTimeEndString, workTimeStartString) - LibDateTime.MinutesFromTime(BreakHoursAtWork(workingDayBreaksModel));
                            rowTimeAtWork = $"{style.Planing.TimeAtWorkPre}{LibDateTime.HoursFromMinutes(tmpMinutesAtWork)}{style.Planing.TimeAtWorkPost}";
                            minutesAtWork = minutesAtWork + tmpMinutesAtWork;
                        }
                        else
                        {
                            int tmpMinutesAtWork = LibDateTime.MinutesBetweenTime(workTimeEndString, workTimeStartString) - LibDateTime.MinutesFromTime(BreakHoursAtWorkPlaning(workingDayBreaksModelPlaning, dayOfmonth));
                            rowTimeAtWork = $"{style.Planing.TimeAtWorkPre}{LibDateTime.HoursFromMinutes(tmpMinutesAtWork)}{style.Planing.TimeAtWorkPost}";
                            minutesAtWorkPlaning = minutesAtWorkPlaning + tmpMinutesAtWork;
                        }
                    }
                }

                string rowType = $"{style.Normal.SymbolPre}{style.Normal.SymbolPost}";
                string rowBreakTime = $"{style.Normal.BreakTimePre}{style.Normal.BreakTimePost}";

                if ((rowWorkStart != $"{style.Normal.TimeFromPre}{style.Normal.TimeFromPost}") || (rowWorkEnd != $"{style.Normal.TimeToPre}{style.Normal.TimeToPost}"))
                {
                    raportTable.Add(new RaportTableModel() { colDay = rowDay, colFrom = rowWorkStart, colTo = rowWorkEnd, colType = rowType, colComment = hideComment?$"{style.Normal.CommentPre}{style.Normal.CommentPost}" : rowComment, colBreakTime = rowBreakTime, colWorkTime = rowTimeAtWork });
                    //rowDay = $"{style.Normal.DayPre}{CultureInfo.CurrentCulture.DateTimeFormat.GetShortestDayName(dayOfmonth.DayOfWeek)} {day:d2}{style.Normal.DayPost}";
                    rowType = $"{style.Normal.SymbolPre}{style.Normal.SymbolPost}";
                    rowWorkStart = $"{style.Normal.TimeFromPre}{style.Normal.TimeFromPost}";
                    rowWorkEnd = $"{style.Normal.TimeToPre}{style.Normal.TimeToPost}";
                    rowComment = $"{style.Normal.CommentPre}{style.Normal.CommentPost}";
                    rowBreakTime = $"{style.Normal.BreakTimePre}{style.Normal.BreakTimePost}";
                    rowTimeAtWork = $"{style.Normal.TimeAtWorkPre}{style.Normal.TimeAtWorkPost}";
                    printedDay = true;
                }

                
                //rowDay = $"{style.Break.DayPre}{CultureInfo.CurrentCulture.DateTimeFormat.GetShortestDayName(dayOfmonth.DayOfWeek)} {day:d2}{style.Break.DayPost}";
                rowType = $"{style.Break.SymbolPre}{style.Break.SymbolPost}";
                rowWorkStart = $"{style.Break.TimeFromPre}{style.Break.TimeFromPost}";
                rowWorkEnd = $"{style.Break.TimeToPre}{style.Break.TimeToPost}";
                rowComment = $"{style.Break.CommentPre}{style.Break.CommentPost}";
                rowBreakTime = $"{style.Break.BreakTimePre}{style.Break.BreakTimePost}";
                rowTimeAtWork = $"{style.Break.TimeAtWorkPre}{style.Break.TimeAtWorkPost}";

                rowType = $"{style.Break.SymbolPre}{style.Break.SymbolPost}";
                if (workingDayBreaksModel.Count() > 0)
                {
                    foreach (WorkingDayBreaksModel workingDayBreaks in workingDayBreaksModel)
                    {
                        rowWorkStart = $"{style.Break.TimeFromPre}{style.Break.TimeFromPost}";
                        if (workingDayBreaks.BreakTimeStart != "")
                        {
                            rowWorkStart = $"{style.Break.TimeFromPre}{workingDayBreaks.BreakTimeStart}{style.Break.TimeFromPost}";
                            workTimeBreakStartString = workingDayBreaks.BreakTimeStart;
                        }

                        rowWorkEnd = $"{style.Break.TimeToPre}{style.Break.TimeToPost}";
                        if (workingDayBreaks.BreakTimeEnd != "")
                        {
                            rowWorkEnd = $"{style.Break.TimeToPre}{workingDayBreaks.BreakTimeEnd}{style.Break.TimeToPost}";
                            workTimeBreakEndString = workingDayBreaks.BreakTimeEnd;
                        }

                        rowComment = $"{style.Break.CommentPre}{style.Break.CommentPost}";
                        if (workingDayBreaks.Comment != "")
                        {
                            rowComment = $"{style.Break.CommentPre}{workingDayBreaks.Comment}{style.Break.CommentPost}";
                        }

                        rowBreakTime = $"{style.Break.BreakTimePre}{style.Break.BreakTimePost}";
                        if ((workingDayBreaks.BreakTimeEnd != "") && (workingDayBreaks.BreakTimeStart != ""))
                        {
                            rowBreakTime = $"{style.Break.TimeAtWorkPre}{LibDateTime.HoursBetweenTime(workingDayBreaks.BreakTimeEnd, workingDayBreaks.BreakTimeStart)}{style.Break.TimeAtWorkPost}";
                            //minutesAtWork = minutesAtWork - LibDateTime.MinutesBetweenTime(workTimeBreakEndString, workTimeBreakStartString);
                        }

                        if ((rowWorkStart != $"{style.Break.TimeFromPre}{style.Break.TimeFromPost}") || (rowWorkEnd != $"{style.Break.TimeToPre}{style.Break.TimeToPost}"))
                        {
                            rowType = $"{style.Break.SymbolPre}PRZERWA{style.Break.SymbolPost}";
                            string tmpRowDay = rowDay;
                            if (printedDay) { tmpRowDay = $"{style.Break.DayPre}{style.Break.DayPost}"; }
                            raportTable.Add(new RaportTableModel() { colDay = tmpRowDay, colFrom = rowWorkStart, colTo = rowWorkEnd, colType = rowType, colComment = hideComment ? $"{style.Break.CommentPre}{style.Break.CommentPost}" : rowComment, colBreakTime = rowBreakTime, colWorkTime = rowTimeAtWork });
                            //rowDay = $"{style.Break.DayPre}{style.Break.DayPost}";
                            rowType = $"{style.Break.SymbolPre}{style.Break.SymbolPost}";
                            rowWorkStart = $"{style.Break.TimeFromPre}{style.Break.TimeFromPost}";
                            rowWorkEnd = $"{style.Break.TimeToPre}{style.Break.TimeToPost}";
                            rowComment = $"{style.Break.CommentPre}{style.Break.CommentPost}";
                            rowBreakTime = $"{style.Break.BreakTimePre}{style.Break.BreakTimePost}";
                            rowTimeAtWork = $"{style.Break.TimeAtWorkPre}{style.Break.TimeAtWorkPost}";
                            printedDay = true;
                        }
                    }
                }

                //rowDay = $"{style.Planing.DayPre}{style.Planing.DayPost}";
                rowType = $"{style.Planing.SymbolPre}{style.Planing.SymbolPost}";
                rowWorkStart = $"{style.Planing.TimeFromPre}{style.Planing.TimeFromPost}";
                rowWorkEnd = $"{style.Planing.TimeToPre}{style.Planing.TimeToPost}";
                rowComment = $"{style.Planing.CommentPre}{style.Planing.CommentPost}";
                rowBreakTime = $"{style.Planing.BreakTimePre}{style.Planing.BreakTimePost}";
                rowTimeAtWork = $"{style.Planing.TimeAtWorkPre}{style.Planing.TimeAtWorkPost}";

                rowType = $"{style.Planing.SymbolPre}{style.Planing.SymbolPost}";
                if (workingDayBreaksModelPlaning.Count() > 0)
                {
                    foreach (WorkingDayBreaksModel workingDayBreaks in workingDayBreaksModelPlaning)
                    {
                        if (dayOfmonth.CompareTo(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)) > 0)
                        {

                            rowWorkStart = $"{style.Planing.TimeFromPre}{style.Planing.TimeFromPost}";
                            if (workingDayBreaks.BreakTimeStart != "")
                            {
                                rowWorkStart = $"{style.Planing.TimeFromPre}{workingDayBreaks.BreakTimeStart}{style.Planing.TimeFromPost}";
                                workTimeBreakStartString = workingDayBreaks.BreakTimeStart;
                            }

                            rowWorkEnd = $"{style.Planing.TimeToPre}{style.Planing.TimeToPost}";
                            if (workingDayBreaks.BreakTimeEnd != "")
                            {
                                rowWorkEnd = $"{style.Planing.TimeToPre}{workingDayBreaks.BreakTimeEnd}{style.Planing.TimeToPost}";
                                workTimeBreakEndString = workingDayBreaks.BreakTimeEnd;
                            }

                            rowComment = $"{style.Planing.CommentPre}{style.Planing.CommentPost}";
                            if (workingDayBreaks.Comment != "")
                            {
                                rowComment = $"{style.Planing.CommentPre}{workingDayBreaks.Comment}{style.Planing.CommentPost}";
                            }

                            rowBreakTime = $"{style.Planing.TimeAtWorkPre}{style.Planing.TimeAtWorkPost}";
                            if ((workingDayBreaks.BreakTimeEnd != "") && (workingDayBreaks.BreakTimeStart != ""))
                            {
                                rowBreakTime = $"{style.Planing.TimeAtWorkPre}{LibDateTime.HoursBetweenTime(workingDayBreaks.BreakTimeEnd, workingDayBreaks.BreakTimeStart)}{style.Planing.TimeAtWorkPost}";
                                //minutesAtWorkPlaning = minutesAtWorkPlaning - LibDateTime.MinutesBetweenTime(workTimeBreakEndString,workTimeBreakStartString);
                            }

                            if ((rowWorkStart != $"{style.Planing.TimeFromPre}{style.Planing.TimeFromPost}") || (rowWorkEnd != $"{style.Planing.TimeToPre}{style.Planing.TimeToPost}"))
                            {
                                rowType = $"{style.Planing.SymbolPre}PRZERWA{style.Planing.SymbolPost}";
                                string tmpRowDay = rowDay;
                                if (printedDay) { tmpRowDay = $"{style.Planing.DayPre}{style.Planing.DayPost}"; }
                                raportTable.Add(new RaportTableModel() { colDay = tmpRowDay, colFrom = rowWorkStart, colTo = rowWorkEnd, colType = rowType, colComment = hideComment ? $"{style.Planing.CommentPre}{style.Planing.CommentPost}" : rowComment, colBreakTime = rowBreakTime, colWorkTime =rowTimeAtWork });
                                //rowDay = $"{style.Planing.DayPre}{style.Planing.DayPost}";
                                rowType = $"{style.Planing.SymbolPre}{style.Planing.SymbolPost}";
                                rowWorkStart = $"{style.Planing.TimeFromPre}{style.Planing.TimeFromPost}";
                                rowWorkEnd = $"{style.Planing.TimeToPre}{style.Planing.TimeToPost}";
                                rowComment = $"{style.Planing.CommentPre}{style.Planing.CommentPost}";
                                rowBreakTime = $"{style.Planing.BreakTimePre}{style.Planing.BreakTimePost}";
                                rowTimeAtWork = $"{style.Planing.TimeAtWorkPre}{style.Planing.TimeAtWorkPost}";
                                printedDay = true;
                            }
                        }
                    }
                }


                //rowDay = $"{style.Holiday.DayPre}{style.Holiday.DayPost}";
                rowType = $"{style.Holiday.SymbolPre}{style.Holiday.SymbolPost}";
                rowWorkStart = $"{style.Holiday.TimeFromPre}{style.Holiday.TimeFromPost}";
                rowWorkEnd = $"{style.Holiday.TimeToPre}{style.Holiday.TimeToPost}";
                rowComment = $"{style.Holiday.CommentPre}{style.Holiday.CommentPost}";
                rowBreakTime = $"{style.Holiday.BreakTimePre}{style.Holiday.BreakTimePost}";
                rowTimeAtWork = $"{style.Holiday.TimeAtWorkPre}{style.Holiday.TimeAtWorkPost}";

                if (da.LoadAbsenceDay(year: setupYear, month: setupMonth, day: day).Count() > 0)
                {
                    string tmpRowDay = rowDay;
                    if (printedDay) { tmpRowDay = $"{style.Holiday.DayPre}{style.Holiday.DayPost}"; }
                    raportTable.Add(new RaportTableModel() { colDay = tmpRowDay, colFrom = rowWorkStart, colTo = rowWorkEnd, colType = $"{style.Holiday.SymbolPre}{absenceDayModel.TypeOfAbsence}{style.Holiday.SymbolPost}", colComment = $"{style.Holiday.CommentPre}{absenceDatesModel.Comment}{style.Holiday.CommentPost}", colBreakTime = rowBreakTime, colWorkTime = rowTimeAtWork });
                    printedDay = true;
                }
                if (printedDay == false)
                {
                    raportTable.Add(new RaportTableModel() { colDay = rowDay, colFrom = rowWorkStart, colTo = rowWorkEnd, colType = rowType, colComment = rowComment, colBreakTime = rowBreakTime, colWorkTime = rowTimeAtWork });
                }
            }

            //// koment
            ///
            if ((showStatus) && (type == "spectre"))
            {
                DayStatusView dayStatusView = new DayStatusView();
                AnsiConsole.MarkupLine($"W miesiącu łącznie do przepracowania: [green]{(LibDateTime.HoursFromMinutes(dayStatusView.MinutesToWorkInMonth(tmpWorkDays.Count() - da.LoadAbsenceDay(year: setupYear, month: setupMonth).Count())))}[/], przepracowany czas: [red]{LibDateTime.HoursFromMinutes(minutesAtWork)}[/]");

                int whatStay = dayStatusView.MinutesToWorkInMonth(tmpWorkDays.Count() - da.LoadAbsenceDay(year: setupYear, month: setupMonth).Count()) - minutesAtWork;
                if (whatStay >= 0)
                {
                    AnsiConsole.MarkupLine($"Pozostało do przepracowania: [red]{(LibDateTime.HoursFromMinutes(whatStay))}[/] zaplanowano: [yellow]{LibDateTime.HoursFromMinutes(minutesAtWorkPlaning)}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"Nadgodziny: [green]{(LibDateTime.HoursFromMinutes(-whatStay))}[/]{(minutesAtWorkPlaning > 0 ? $" zaplanowano: [yellow]{LibDateTime.HoursFromMinutes(minutesAtWorkPlaning)}[/]" : "")}");
                }
                if ((DateTime.Now.Year == setupYear) && (DateTime.Now.Month == setupMonth))
                {
                    int restDayToWork = 0;


                    if ((da.LoadWorkingDay(year: setupYear, month: setupMonth, day: DateTime.Now.Day, planing: false).Count() > 0) && (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: DateTime.Now.Day, planing: false).First().WorkTimeStart != "") && (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: DateTime.Now.Day, planing: false).First().WorkTimeEnd != ""))
                    {
                        restDayToWork = (tmpWorkDays.Count(x => x.Day > DateTime.Now.Day) - da.LoadAbsenceDay(year: setupYear, month: setupMonth).Count(x => x.Day >= DateTime.Now.Day));
                    }

                    else
                    {
                        restDayToWork = (tmpWorkDays.Count(x => x.Day >= DateTime.Now.Day) - da.LoadAbsenceDay(year: setupYear, month: setupMonth).Count(x => x.Day >= DateTime.Now.Day));
                    }
                    AnsiConsole.MarkupLine($"Zalecany dzienny czas pracy: [yellow]{(LibDateTime.HoursFromMinutes(int.Parse((Math.Round(double.Parse((whatStay / restDayToWork).ToString()), 0).ToString()).ToString())))}, {restDayToWork} dni roboczych[/]");
                }
            }

            if ((showStatus) && (type == "html"))
            {

                string tmpComment = "";
                string tmpTitle = "";
                //// koment
                DayStatusView dayStatusView = new DayStatusView();


                tmpComment += ($"<div>W miesiącu łącznie do przepracowania: <font style='color:green'>{(LibDateTime.HoursFromMinutes(dayStatusView.MinutesToWorkInMonth(tmpWorkDays.Count() - da.LoadAbsenceDay(year: setupYear, month: setupMonth).Count())))}</font>, przepracowany czas: <font style='color:red'>{LibDateTime.HoursFromMinutes(minutesAtWork)}</font></div>");

                int whatStay = dayStatusView.MinutesToWorkInMonth(tmpWorkDays.Count() - da.LoadAbsenceDay(year: setupYear, month: setupMonth).Count()) - minutesAtWork;
                if (whatStay >= 0)
                {
                    tmpComment += ($"<div>Pozostało do przepracowania: <font style='color:red'>{(LibDateTime.HoursFromMinutes(whatStay))}</font> zaplanowano: <font style='color:orange'>{LibDateTime.HoursFromMinutes(minutesAtWorkPlaning)}</font></div>");
                    tmpTitle = $", do przepracowania: {(LibDateTime.HoursFromMinutes(whatStay))}, zaplanowano: {LibDateTime.HoursFromMinutes(minutesAtWorkPlaning)}";

                }
                else
                {
                    tmpComment += ($"<div>Nadgodziny: <font style='color:green'>{(LibDateTime.HoursFromMinutes(-whatStay))}</font>{(minutesAtWorkPlaning > 0 ? $" zaplanowano: <font style='color:orange'>{LibDateTime.HoursFromMinutes(minutesAtWorkPlaning)}</font>" : "")}</div>");
                    tmpTitle = $", nadgodziny: {(LibDateTime.HoursFromMinutes(-whatStay))}";
                }
                if ((DateTime.Now.Year == setupYear) && (DateTime.Now.Month == setupMonth))
                {
                    int restDayToWork = 0;


                    if ((da.LoadWorkingDay(year: setupYear, month: setupMonth, day: DateTime.Now.Day, planing: false).Count() > 0) && (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: DateTime.Now.Day, planing: false).First().WorkTimeStart != "") && (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: DateTime.Now.Day, planing: false).First().WorkTimeEnd != ""))
                    {
                        restDayToWork = (tmpWorkDays.Count(x => x.Day > DateTime.Now.Day) - da.LoadAbsenceDay(year: setupYear, month: setupMonth).Count(x => x.Day >= DateTime.Now.Day));
                    }

                    else
                    {
                        restDayToWork = (tmpWorkDays.Count(x => x.Day >= DateTime.Now.Day) - da.LoadAbsenceDay(year: setupYear, month: setupMonth).Count(x => x.Day >= DateTime.Now.Day));
                    }
                    tmpComment += ($"<div>Zalecany dzienny czas pracy: <font style='color:orange'>{(LibDateTime.HoursFromMinutes(int.Parse((Math.Round(double.Parse((whatStay / restDayToWork).ToString()), 0).ToString()).ToString())))}, {restDayToWork} dni roboczych</font></div>");
                }

                int absenceTypeUW = da.LoadAbsenceDay(year: setupYear, month: setupMonth, typeOfAbsence: "UW").Count();
                int absenceTypeUO = da.LoadAbsenceDay(year: setupYear, month: setupMonth, typeOfAbsence: "UO").Count();
                int absenceTypeUD = da.LoadAbsenceDay(year: setupYear, month: setupMonth, typeOfAbsence: "UD").Count();
                int absenceTypeL4 = da.LoadAbsenceDay(year: setupYear, month: setupMonth, typeOfAbsence: "L4").Count();
                int absenceTypeUB = da.LoadAbsenceDay(year: setupYear, month: setupMonth, typeOfAbsence: "UB").Count();

                if (absenceTypeUW + absenceTypeUO + absenceTypeUD + absenceTypeL4 + absenceTypeUB > 0)
                {
                    tmpComment += "<div>Zestawienie urlopów w zestawianym miesiącu:</div>";
                    if (absenceTypeUW > 0) { tmpComment += $"<div> - (UW) Urlop wypoczynkowy: <font style='color:green'>{absenceTypeUW}</font></div>"; }
                    if (absenceTypeUO > 0) { tmpComment += $"<div> - (UO) Urlop okolicznościowy: <font style='color:green'>{absenceTypeUO}</font></div>"; }
                    if (absenceTypeUD > 0) { tmpComment += $"<div> - (UD) Urlop opiekuńcz: <font style='color:green'>{absenceTypeUD}</font></div>"; }
                    if (absenceTypeL4 > 0) { tmpComment += $"<div> - (L4) Urlop chorobowy: <font style='color:red'>{absenceTypeL4}</font></div>"; }
                    if (absenceTypeUB > 0) { tmpComment += $"<div> - (UB) Urlop bezpłatny: <font style='color:red'>{absenceTypeUB}</font></div>"; }
                }

                outParameters.Add(tmpTitle);
                outParameters.Add(tmpComment);

            }


            if ((showStatus) && (type == "csv"))
            {

                string tmpComment = "";
                string tmpTitle = "";
                //// koment
                DayStatusView dayStatusView = new DayStatusView();


                tmpComment += ($"<div>W miesiącu łącznie do przepracowania: <font style='color:green'>{(LibDateTime.HoursFromMinutes(dayStatusView.MinutesToWorkInMonth(tmpWorkDays.Count() - da.LoadAbsenceDay(year: setupYear, month: setupMonth).Count())))}</font>, przepracowany czas: <font style='color:red'>{LibDateTime.HoursFromMinutes(minutesAtWork)}</font></div>");

                int whatStay = dayStatusView.MinutesToWorkInMonth(tmpWorkDays.Count() - da.LoadAbsenceDay(year: setupYear, month: setupMonth).Count()) - minutesAtWork;
                if (whatStay >= 0)
                {
                    tmpComment += ($"<div>Pozostało do przepracowania: <font style='color:red'>{(LibDateTime.HoursFromMinutes(whatStay))}</font> zaplanowano: <font style='color:orange'>{LibDateTime.HoursFromMinutes(minutesAtWorkPlaning)}</font></div>");
                    tmpTitle = $", do przepracowania: {(LibDateTime.HoursFromMinutes(whatStay))}, zaplanowano: {LibDateTime.HoursFromMinutes(minutesAtWorkPlaning)}";

                }
                else
                {
                    tmpComment += ($"<div>Nadgodziny: <font style='color:green'>{(LibDateTime.HoursFromMinutes(-whatStay))}</font>{(minutesAtWorkPlaning > 0 ? $" zaplanowano: <font style='color:orange'>{LibDateTime.HoursFromMinutes(minutesAtWorkPlaning)}</font>" : "")}</div>");
                    tmpTitle = $", nadgodziny: {(LibDateTime.HoursFromMinutes(-whatStay))}";
                }
                if ((DateTime.Now.Year == setupYear) && (DateTime.Now.Month == setupMonth))
                {
                    int restDayToWork = 0;


                    if ((da.LoadWorkingDay(year: setupYear, month: setupMonth, day: DateTime.Now.Day, planing: false).Count() > 0) && (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: DateTime.Now.Day, planing: false).First().WorkTimeStart != "") && (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: DateTime.Now.Day, planing: false).First().WorkTimeEnd != ""))
                    {
                        restDayToWork = (tmpWorkDays.Count(x => x.Day > DateTime.Now.Day) - da.LoadAbsenceDay(year: setupYear, month: setupMonth).Count(x => x.Day >= DateTime.Now.Day));
                    }

                    else
                    {
                        restDayToWork = (tmpWorkDays.Count(x => x.Day >= DateTime.Now.Day) - da.LoadAbsenceDay(year: setupYear, month: setupMonth).Count(x => x.Day >= DateTime.Now.Day));
                    }
                    tmpComment += ($"<div>Zalecany dzienny czas pracy: <font style='color:orange'>{(LibDateTime.HoursFromMinutes(int.Parse((Math.Round(double.Parse((whatStay / restDayToWork).ToString()), 0).ToString()).ToString())))}, {restDayToWork} dni roboczych</font></div>");
                }

                int absenceTypeUW = da.LoadAbsenceDay(year: setupYear, month: setupMonth, typeOfAbsence: "UW").Count();
                int absenceTypeUO = da.LoadAbsenceDay(year: setupYear, month: setupMonth, typeOfAbsence: "UO").Count();
                int absenceTypeUD = da.LoadAbsenceDay(year: setupYear, month: setupMonth, typeOfAbsence: "UD").Count();
                int absenceTypeL4 = da.LoadAbsenceDay(year: setupYear, month: setupMonth, typeOfAbsence: "L4").Count();
                int absenceTypeUB = da.LoadAbsenceDay(year: setupYear, month: setupMonth, typeOfAbsence: "UB").Count();

                if (absenceTypeUW + absenceTypeUO + absenceTypeUD + absenceTypeL4 + absenceTypeUB > 0)
                {
                    tmpComment += "<div>Zestawienie urlopów w zestawianym miesiącu:</div>";
                    if (absenceTypeUW > 0) { tmpComment += $"<div> - (UW) Urlop wypoczynkowy: <font style='color:green'>{absenceTypeUW}</font></div>"; }
                    if (absenceTypeUO > 0) { tmpComment += $"<div> - (UO) Urlop okolicznościowy: <font style='color:green'>{absenceTypeUO}</font></div>"; }
                    if (absenceTypeUD > 0) { tmpComment += $"<div> - (UD) Urlop opiekuńcz: <font style='color:green'>{absenceTypeUD}</font></div>"; }
                    if (absenceTypeL4 > 0) { tmpComment += $"<div> - (L4) Urlop chorobowy: <font style='color:red'>{absenceTypeL4}</font></div>"; }
                    if (absenceTypeUB > 0) { tmpComment += $"<div> - (UB) Urlop bezpłatny: <font style='color:red'>{absenceTypeUB}</font></div>"; }
                }

                outParameters.Add(tmpTitle);
                outParameters.Add(tmpComment);

            }


            return (raportTable, outParameters);
        }


        public string BreakHoursAtWork(List<WorkingDayBreaksModel> workingDayBreaksModel)
        {
            string breakTimeAtWork = "00:00";
            if (workingDayBreaksModel.Count() > 0)
            {
                foreach (WorkingDayBreaksModel workingDayBreaks in workingDayBreaksModel)
                {
                    breakTimeAtWork = "";
                    if ((workingDayBreaks.BreakTimeEnd != "") && (workingDayBreaks.BreakTimeStart != ""))
                    {
                        breakTimeAtWork = LibDateTime.HoursBetweenTime(workingDayBreaks.BreakTimeEnd, workingDayBreaks.BreakTimeStart);
                    }
                }
            }
            return breakTimeAtWork;
        }

        public string BreakHoursAtWorkPlaning(List<WorkingDayBreaksModel> workingDayBreaksModelPlaning, DateTime dayOfmonth)
        {
            string breakTimeAtWorkPlaning = "00:00";
            if (dayOfmonth.CompareTo(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)) > 0)
            {
                if (workingDayBreaksModelPlaning.Count() > 0)
                {
                    foreach (WorkingDayBreaksModel workingDayBreaks in workingDayBreaksModelPlaning)
                    {
                        breakTimeAtWorkPlaning = "";
                        if ((workingDayBreaks.BreakTimeEnd != "") && (workingDayBreaks.BreakTimeStart != ""))
                        {
                            breakTimeAtWorkPlaning = LibDateTime.HoursBetweenTime(workingDayBreaks.BreakTimeEnd, workingDayBreaks.BreakTimeStart);
                        }
                    }
                }
            }
            return breakTimeAtWorkPlaning;
        }

        public void Schow(int setupYear, int setupMonth, int setupDay)
        {
            var raportTableSpectre = new Table();

            raportTableSpectre.Title(new TableTitle($"{setupYear}, {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(setupMonth)}"));
            raportTableSpectre.AddColumns("Dzień", "Od", "Do", "Symbol", "Komentarz", "Czas przerwy", "Czas w pracy");

            Collection<RaportTableModel> raportTables = new Collection<RaportTableModel>();
            List<string> tmpParametrs = new List<string>();
            (raportTables, tmpParametrs) = GenerateRaport(setupYear, setupMonth,false, "spectre",false);

            foreach (RaportTableModel raportTable in raportTables)
            {
                raportTableSpectre.AddRow(raportTable.ConvertToArray());
            }
            AnsiConsole.Write(raportTableSpectre);

            GenerateRaport(setupYear, setupMonth, true, "spectre",false);


        }

        public bool SendMail(int setupYear, int setupMonth, int setupDay, bool longRaport, string mailFrom, string mailTo, string mailPass, bool longRaportComment)
        {
            DateTime dateTodayToCompare = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime generateDate = new DateTime(setupYear, setupMonth, setupDay);
            string emailHtml="<html><body>";
            List<string> tmpParametrs = new List<string>();


            emailHtml += $"<p>{setupYear}, {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(setupMonth)}</p>";

            Collection<RaportTableModel> raportTables = new Collection<RaportTableModel>();

            if (longRaport==true)
            {
                emailHtml += "<table border='1' cellspacing='0' cellpadding='5'>" +
                "<tr align=center><td><p><b>Dzień</b></p></td><td><p><b>Od</b></p></td><td><p><b>Do</b></p></td><td><p><b>Symbol</b></p></td><td><p><b>Komentarz</b></p></td><td><p><b>Czas przerwy</b></p></td><td><p><b>Czas w pracy</b></p></td></tr>"; ;

                (raportTables, tmpParametrs) = GenerateRaport(setupYear, setupMonth, false, "html",longRaportComment);

                foreach (RaportTableModel raportTable in raportTables)
                {

                    emailHtml += $"<tr align=center><td>{raportTable.colDay}</td><td>{raportTable.colFrom}</td><td>{raportTable.colTo}</td><td>{raportTable.colType}</td><td>{raportTable.colComment}</td><td>{raportTable.colBreakTime}</td><td>{raportTable.colWorkTime}</td></tr>";
                }

                emailHtml += "</table>";
            }

            

            //string commentHtml;
            //string title;

            (raportTables, tmpParametrs) = GenerateRaport(setupYear, setupMonth, true,"html",false);
            emailHtml += tmpParametrs[1];
            emailHtml += "</body></html>";

            LibMail lm = new LibMail();

            string tmpSubject;
            if ((new DateTime(setupYear, setupMonth,setupDay).Year==DateTime.Now.Year)&& (new DateTime(setupYear, setupMonth, setupDay).Month == DateTime.Now.Month))
            {
                tmpSubject = $"Raport z obecności: {setupYear}-{setupMonth}-{DateTime.Now.Day} {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(setupMonth)}{tmpParametrs[0]}";
            }
            else
            {
                tmpSubject = $"Raport z obecności: {setupYear}-{setupMonth} {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(setupMonth)}{tmpParametrs[0]}";
            }




            lm.WyslijWiadomosc(mailFrom, "", "", mailTo, tmpSubject, emailHtml, new List<string>() , "", mailPass);

            var rulee = new Rule($"[blue]Wiadomość wysłano.[/] [grey](wciśnij dowolny klawisz)[/]");
            rulee.Alignment = Justify.Center;
            rulee.Style = Style.Parse("gray dim");
            AnsiConsole.Write(rulee);

            Console.ReadKey();
            return true;
        }


        public bool ExportHtml(int setupYear, int setupMonth, int setupDay, bool schowFolder=true)
        {
            string src = LibSystem.CreateFolderToStore("Export");

            DateTime dateTodayToCompare = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime generateDate = new DateTime(setupYear, setupMonth, setupDay);


            string tmpSubject;
            if ((new DateTime(setupYear, setupMonth, setupDay).Year == DateTime.Now.Year) && (new DateTime(setupYear, setupMonth, setupDay).Month == DateTime.Now.Month))
            {
                tmpSubject = $"ListaObecnosci_{setupYear}-{setupMonth}-{DateTime.Now.Day}.html";
            }
            else
            {
                tmpSubject = $"ListaObecnosci_{setupYear}-{setupMonth}.html";
            }


            using StreamWriter file = new(src + tmpSubject, append: false, Encoding.UTF8);
            file.WriteLine($"<html><body>");

            List<string> tmpParametrs = new List<string>();


            file.WriteLine($"<p>{setupYear}, {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(setupMonth)}</p>");

            Collection<RaportTableModel> raportTables = new Collection<RaportTableModel>();


            file.WriteLine("<table border='1' cellspacing='0' cellpadding='5'>" +
                "<tr align=center><td><p><b>Dzień</b></p></td><td><p><b>Od</b></p></td><td><p><b>Do</b></p></td><td><p><b>Symbol</b></p></td><td><p><b>Komentarz</b></p></td><td><p><b>Czas przerwy</b></p></td><td><p><b>Czas w pracy</b></p></td></tr>");

                (raportTables, tmpParametrs) = GenerateRaport(setupYear, setupMonth, false, "html", false);

                foreach (RaportTableModel raportTable in raportTables)
                {

                file.WriteLine($"<tr align=center><td>{raportTable.colDay}</td><td>{raportTable.colFrom}</td><td>{raportTable.colTo}</td><td>{raportTable.colType}</td><td>{raportTable.colComment}</td><td>{raportTable.colBreakTime}</td><td>{raportTable.colWorkTime}</td></tr>");
                }

            file.WriteLine("</table>");
            



            //string commentHtml;
            //string title;

            (raportTables, tmpParametrs) = GenerateRaport(setupYear, setupMonth, true, "html", false);
            file.WriteLine(tmpParametrs[1]);
            file.WriteLine("</body></html>");


            file.Close();
            if (schowFolder)
            {
                Process.Start("explorer.exe", src);
            }
            return true;
        }

        public bool ExportCSV(int setupYear, int setupMonth, int setupDay)
        {

            string src = LibSystem.CreateFolderToStore("Export");


            DateTime dateTodayToCompare = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime generateDate = new DateTime(setupYear, setupMonth, setupDay);
            string exportCSV = "";
            List<string> tmpParametrs = new List<string>();


            Collection<RaportTableModel> raportTables = new Collection<RaportTableModel>();

            (raportTables, tmpParametrs) = GenerateRaport(setupYear, setupMonth, false, "csv", false);

            string tmpSubject;
            if ((new DateTime(setupYear, setupMonth, setupDay).Year == DateTime.Now.Year) && (new DateTime(setupYear, setupMonth, setupDay).Month == DateTime.Now.Month))
            {
                tmpSubject = $"ListaObecnosci_{setupYear}-{setupMonth}-{DateTime.Now.Day}.csv";
            }
            else
            {
                tmpSubject = $"ListaObecnosci_{setupYear}-{setupMonth}.csv";
            }


            using StreamWriter file = new(src + tmpSubject, append: false,Encoding.UTF8);
            file.WriteLine($"{setupYear}, {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(setupMonth)}");

            file.WriteLine("Dzień;Od;Do;Symbol;Komentarz;Czas przerwy;Czas w pracy;");

            foreach (RaportTableModel raportTable in raportTables)
            {
                file.WriteLine($"{raportTable.colDay};{raportTable.colFrom};{raportTable.colTo};{raportTable.colType};{raportTable.colComment};{raportTable.colBreakTime};{raportTable.colWorkTime};");
            }

            (raportTables, tmpParametrs) = GenerateRaport(setupYear, setupMonth, true, "csv", false);
            exportCSV += tmpParametrs[1];

            //SaveFile(src, tmpSubject);
            file.Close();
            Process.Start("explorer.exe", src);
            return true;
        }
    }
}

