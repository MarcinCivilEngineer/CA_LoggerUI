using Caliburn.Micro;
using System.Windows;
using WPF_LoggerTray.ViewModels;
using WPF_LoggerTray.Views;


using WPF_LoggerTray;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Data;
using System.Text.RegularExpressions;
using System.Reflection;
using WPF_LoggerTray.Models;
using System.Globalization;

namespace WPF_LoggerTray.ViewModels
{
    public class RaportViewModel : Conductor<object>
    {
        private string _edEditedMonth;

        public string EdEditedMonth
        {
            get { return _edEditedMonth; }
            set { _edEditedMonth = value; }
        }


        public RaportViewModel()
        {
            EdEditedMonth = $"{DateTime.Now.Year}-{DateTime.Now.Month}";
            Schow(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }


        public void CreateButton(string edDrewnoNazwa, string edDrewnoFmk, string edDrewnoKmod)
        {
            

        }

        public void UpdateButton(string edDrewnoNazwa, string edDrewnoFmk, string edDrewnoKmod)
        {

        }

        public void DeleteButton(string edNumerProjektu)
        {

        }





        public (Collection<RaportTableModel>, List<string>) GenerateRaport(int setupYear, int setupMonth, bool showStatus, string type, bool hideComment)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();

            List<string> outParameters = new();
            //List<RaportTableModel> raportTablePlaning = new List<RaportTableModel>();

            string freeDaysOfWorkCurrent = da.LoadSetupEntry(name: $"freeDaysOfWork{setupYear}-{setupMonth}").Value;
            string daysOfWorkCurrent = da.LoadSetupEntry(name: $"daysOfWork{setupYear}-{setupMonth}").Value;

            SetupViewModel defaultsSettings = new SetupViewModel();
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
                    raportTable.Add(new RaportTableModel() { colDay = rowDay, colFrom = rowWorkStart, colTo = rowWorkEnd, colType = rowType, colComment = hideComment ? $"{style.Normal.CommentPre}{style.Normal.CommentPost}" : rowComment, colBreakTime = rowBreakTime, colWorkTime = rowTimeAtWork });
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
                                raportTable.Add(new RaportTableModel() { colDay = tmpRowDay, colFrom = rowWorkStart, colTo = rowWorkEnd, colType = rowType, colComment = hideComment ? $"{style.Planing.CommentPre}{style.Planing.CommentPost}" : rowComment, colBreakTime = rowBreakTime, colWorkTime = rowTimeAtWork });
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
            

            if ((showStatus) && (type == "html"))
            {

                string tmpComment = "";
                string tmpTitle = "";
                //// koment
                SetupViewModel dayStatusView = new SetupViewModel();


                tmpComment += ($"<div>W miesiącu łącznie do przepracowania: <font style='color:green'>{(LibDateTime.HoursFromMinutes(MinutesToWorkInMonth(tmpWorkDays.Count() - da.LoadAbsenceDay(year: setupYear, month: setupMonth).Count())))}</font>, przepracowany czas: <font style='color:red'>{LibDateTime.HoursFromMinutes(minutesAtWork)}</font></div>");

                int whatStay = MinutesToWorkInMonth(tmpWorkDays.Count() - da.LoadAbsenceDay(year: setupYear, month: setupMonth).Count()) - minutesAtWork;
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


                tmpComment += ($"<div>W miesiącu łącznie do przepracowania: <font style='color:green'>{(LibDateTime.HoursFromMinutes(MinutesToWorkInMonth(tmpWorkDays.Count() - da.LoadAbsenceDay(year: setupYear, month: setupMonth).Count())))}</font>, przepracowany czas: <font style='color:red'>{LibDateTime.HoursFromMinutes(minutesAtWork)}</font></div>");

                int whatStay = MinutesToWorkInMonth(tmpWorkDays.Count() - da.LoadAbsenceDay(year: setupYear, month: setupMonth).Count()) - minutesAtWork;
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

        public int MinutesToWorkInMonth(int daysAtWork)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            LibDateTime ldt = new LibDateTime();

            return LibDateTime.MinutesBetweenTime(da.LoadSetupEntry(name: "workingPlanningTimeEnd").Value, da.LoadSetupEntry(name: "workingPlanningTimeStart").Value) * daysAtWork;
        }




        /*
        public void DayStatus(int setupYear, int setupMonth, int setupDay)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            LibDateTime ldt = new LibDateTime();

            DateTime tmpSetupDate = new DateTime(setupYear, setupMonth, setupDay);
            WorkingDayModel currentDayPlanning = new WorkingDayModel() { Id = 0, Comment = "", WorkTimeEnd = "", WorkTimeStart = "" };

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


                if (breaksTime != "00:00")
                {
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
                    AnsiConsole.MarkupLine($"Zalecane wyjście z pracy: [yellow]{LibDateTime.HoursAddMinutes(endTimeFirst, LibDateTime.MinutesFromTime(breaksTime))}[/]");
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

                if ((currentDayPlanning.WorkTimeStart != "") && (LibDateTime.MinutesFromTime(currentDayPlanning.WorkTimeStart) > 0))
                {
                    AnsiConsole.MarkupLine($"Planowane rozpoczęcie pracy: [yellow]{currentDayPlanning.WorkTimeStart}[/]");
                }

                if ((currentDayPlanning.WorkTimeEnd != "") && (LibDateTime.MinutesFromTime(currentDayPlanning.WorkTimeEnd) > 0))
                {
                    AnsiConsole.MarkupLine($"Planowane zakończenie pracy: [yellow]{currentDayPlanning.WorkTimeEnd}[/]");
                }

                if ((breaksTimePlaning != "") && (LibDateTime.MinutesFromTime(breaksTimePlaning) > 0))
                {
                    AnsiConsole.MarkupLine($"Planowane przerwy w pracy [yellow]{breaksTimePlaning}[/]");
                }

                if ((currentDayPlanning.WorkTimeStart != "") && (currentDayPlanning.WorkTimeEnd != ""))
                {
                    AnsiConsole.MarkupLine($"Planowany czas w pracy: [yellow]{ LibDateTime.HoursAddMinutes(LibDateTime.HoursBetweenTime(currentDayPlanning.WorkTimeEnd, currentDayPlanning.WorkTimeStart), -LibDateTime.MinutesFromTime(breaksTimePlaning))}[/]");
                }


            }

        }
        */

        public (string, List<string>) WorkedTimeFromTo(int year, int month, int from, int to, List<DateTime> workDates, bool planing)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            LibDateTime ldt = new LibDateTime();

            WorkingDayModel workingDayModel = new WorkingDayModel();
            List<WorkingDayBreaksModel> workingDayBreaksModel = new List<WorkingDayBreaksModel>();

            List<string> planingOrWithutTime = new List<string>();

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
                            planingOrWithutTime.Add($"{a}");
                        }
                    }
                    else if (da.LoadWorkingDay(year: year, month: month, day: a, planing: true).Count() > 0)
                    {
                        minutesAtWork = minutesAtWork + AtDayTimeWorkPlaning(year, month, a).Item1;
                    }
                }
                else
                {
                    minutesAtWork = minutesAtWork + AtDayTimeWork(year, month, a).Item1;
                    if ((AtDayTimeWork(year, month, a).Item2) && (workDates.Contains(new DateTime(year, month, a))))
                    {
                        planingOrWithutTime.Add($"{a}");
                    }
                }
            }

            for (int a = from; a <= to; a++)
            {
                minutesBreaks = minutesBreaks + AtDayBreaksTime(year, month, a, planing).Item1;
            }

            //Remove AbsenceDay
            List<string> planingOrWithutTimeAndAbsence = new List<string>();
            foreach (string withoutTimeDate in planingOrWithutTime)
            {
                if (da.LoadAbsenceDay(year: year, month: month, day: int.Parse(withoutTimeDate)).Count() == 0)
                {
                    planingOrWithutTimeAndAbsence.Add(withoutTimeDate);

                }
            }

            return (LibDateTime.HoursFromMinutes(minutesAtWork - minutesBreaks), planingOrWithutTimeAndAbsence);
        }



        public (int, bool) AtDayTimeWork(int setupYear, int setupMonth, int setupDay)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            LibDateTime ldt = new LibDateTime();
            WorkingDayModel workingDay = new WorkingDayModel();
            WorkingDayModel workingDayPlanning = new WorkingDayModel() { WorkTimeEnd = "", WorkTimeStart = "", Comment = "" };
            int minutes = 0;
            bool withoutTime = true;

            if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: false).Count() > 0)
            {
                workingDay = da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: false).First();
                if (da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).Count() > 0)
                {
                    workingDayPlanning = da.LoadWorkingDay(year: setupYear, month: setupMonth, day: setupDay, planing: true).First();
                }
                if ((workingDay.WorkTimeEnd != "") && (workingDay.WorkTimeStart != ""))
                {
                    minutes = LibDateTime.MinutesBetweenTime(workingDay.WorkTimeEnd, workingDay.WorkTimeStart);
                    withoutTime = false;
                }
                else if ((workingDay.WorkTimeEnd == "") && (workingDay.WorkTimeStart != "") && (workingDayPlanning.WorkTimeEnd != ""))
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

        private Collection<RaportTableModel> _raportTables = new Collection<RaportTableModel>(); 

        public Collection<RaportTableModel> RaportTables  
        {
            get { return _raportTables; }
            set { _raportTables = value; }
        }

        public void Schow(int setupYear, int setupMonth, int setupDay)
        {
//            var raportTableSpectre = new Table();
//            raportTableSpectre.Title(new TableTitle($"{setupYear}, {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(setupMonth)}"));
//            raportTableSpectre.AddColumns("Dzień", "Od", "Do", "Symbol", "Komentarz", "Czas przerwy", "Czas w pracy");

            
            List<string> tmpParametrs = new List<string>();
            (RaportTables, tmpParametrs) = GenerateRaport(setupYear, setupMonth, false, "spectre", false);




//            AnsiConsole.Write(raportTableSpectre);

            GenerateRaport(setupYear, setupMonth, true, "spectre", false);


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
