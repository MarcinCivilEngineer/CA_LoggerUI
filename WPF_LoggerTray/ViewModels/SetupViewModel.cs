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
using System.IO;

namespace WPF_LoggerTray.ViewModels
{
    public class SetupViewModel : Conductor<object>
    {
        IWindowManager manager = new WindowManager();

        public string eMailToSend { get; set; }
        public int whenSentRaport { get; set; }

        private bool _day1;

        public bool Day1
        {
            get { return _day1; }
            set { _day1 = value; }
        }
        private bool _day2;

        public bool Day2
        {
            get { return _day2; }
            set { _day2 = value; }
        }
        private bool _day3;

        public bool Day3
        {
            get { return _day3; }
            set { _day3 = value; }
        }
        private bool _day4;

        public bool Day4
        {
            get { return _day4; }
            set { _day4 = value; }
        }
        private bool _day5;

        public bool Day5
        {
            get { return _day5; }
            set { _day5 = value; }
        }

        private bool _day6;

        public bool Day6
        {
            get { return _day6; }
            set { _day6 = value; }
        }

        private bool _day7;

        public bool Day7
        {
            get { return _day7; }
            set { _day7 = value; }
        }

        public DaysModel daysModel { get; set; } = new DaysModel();
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

        private BindableCollection<string> _freeDaysOfW;

        public BindableCollection<string> FreeDaysOfW
        {
            get { return _freeDaysOfW; }
            set { _freeDaysOfW = value; }
        }

        private string _edeMailToSend;

        public string EdeMailToSend
        {
            get { return _edeMailToSend; }
            set { _edeMailToSend = value; }
        }

        private string _edWhenSentRaport;

        public string EdWhenSentRaport
        {
            get { return _edWhenSentRaport; }
            set { _edWhenSentRaport = value; }
        }
        private string _edWorkingPlanningTimeStart;

        public string EdWorkingPlanningTimeStart
        {
            get { return _edWorkingPlanningTimeStart; }
            set { _edWorkingPlanningTimeStart = value; }
        }
        private string _edWorkingPlanningTimeEnd;

        public string EdWorkingPlanningTimeEnd
        {
            get { return _edWorkingPlanningTimeEnd; }
            set { _edWorkingPlanningTimeEnd = value; }
        }
        private string _edHolidaysAtYear;

        public string EdHolidaysAtYear
        {
            get { return _edHolidaysAtYear; }
            set { _edHolidaysAtYear = value; }
        }

        private string _edHolidaysByKids;

        public string EdHolidaysByKids
        {
            get { return _edHolidaysByKids; }
            set { _edHolidaysByKids = value; }
        }

        private string _edMinutesStartWork;

        public string EdMinutesStartWork
        {
            get { return _edMinutesStartWork; }
            set { _edMinutesStartWork = value; }
        }

        private string _edMinutesEndWork;

        public string EdMinutesEndWork
        {
            get { return _edMinutesEndWork; }
            set { _edMinutesEndWork = value; }
        }

        private BindableCollection<bool> _chkDays = new BindableCollection<bool>();

        public BindableCollection<bool> ChkDays
        {
            get { return _chkDays; }
            set { _chkDays = value; }
        }

        public SetupViewModel()
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
            if (eMailToSend != null && whenSentRaport != 0 && workingPlanningTimeEnd != null && workingPlanningTimeStart != null)
            {
                isLoadedFromSQL = true;
            }

            string eMailToSendFirst = da.LoadSetupEntry(name: $"eMailToSend").Value == null ? "l.andrzejewski@betard.pl" : da.LoadSetupEntry(name: $"eMailToSend").Value;
            int whenSentRaportFirst = da.LoadSetupEntry(name: $"whenSentRaport").Value == null ? 25 : int.Parse(da.LoadSetupEntry(name: $"whenSentRaport").Value);
            string workingPlanningTimeStartFirst = da.LoadSetupEntry(name: $"workingPlanningTimeStart").Value == null ? "7:00" : da.LoadSetupEntry(name: $"workingPlanningTimeStart").Value;
            int minutestAtWorkFirst = (int)Math.Round(8.5 * 60, 0);

            string holidaysAtYearFirst = da.LoadSetupEntry(name: $"holidaysAtYear").Value == null ? "26" : da.LoadSetupEntry(name: $"holidaysAtYear").Value;
            string holidaysByKidsFirst = da.LoadSetupEntry(name: $"holidaysByKids").Value == null ? "2" : da.LoadSetupEntry(name: $"holidaysByKids").Value;


            string workingPlanningTimeEndFirst = da.LoadSetupEntry(name: $"workingPlanningTimeEnd").Value == null ? LibDateTime.HoursAddMinutes(workingPlanningTimeStart, minutestAtWorkFirst) : da.LoadSetupEntry(name: $"workingPlanningTimeEnd").Value;
            workingPlanningTimeEnd = workingPlanningTimeEndFirst;

            string minutesStartWorkFirst = da.LoadSetupEntry(name: $"minutesStartWork").Value == null ? "-5" : da.LoadSetupEntry(name: $"minutesStartWork").Value;
            minutesStartWork = minutesStartWorkFirst;
            string minutesEndWorkFirst = da.LoadSetupEntry(name: $"minutesEndWork").Value == null ? "5" : da.LoadSetupEntry(name: $"minutesEndWork").Value;
            minutesEndWork = minutesEndWorkFirst;
            string minutesStartBreakFirst = da.LoadSetupEntry(name: $"minutesStartBreak").Value == null ? "0" : da.LoadSetupEntry(name: $"minutesStartBreak").Value;
            minutesStartBreak = minutesStartBreakFirst;
            string minutesEndBreakFirst = da.LoadSetupEntry(name: $"minutesEndBreak").Value == null ? "0" : da.LoadSetupEntry(name: $"minutesEndBreak").Value;
            minutesEndBreak = minutesEndBreakFirst;



            EdeMailToSend = eMailToSendFirst;
            EdHolidaysAtYear = holidaysAtYearFirst;
            EdHolidaysByKids = holidaysByKidsFirst;
            EdMinutesEndWork = minutesEndWorkFirst;
            EdMinutesStartWork = minutesStartWorkFirst;
            EdWhenSentRaport = whenSentRaportFirst.ToString();
            EdWorkingPlanningTimeStart = workingPlanningTimeStartFirst;
            EdWorkingPlanningTimeEnd = workingPlanningTimeEnd;




            Day1 = da.LoadSetupEntry(name: $"freeDaysOfWeek{daysModel.inPolish[0]}").Value == "True" ? true : false;
            Day2 = da.LoadSetupEntry(name: $"freeDaysOfWeek{daysModel.inPolish[1]}").Value == "True" ? true : false;
            Day3 = da.LoadSetupEntry(name: $"freeDaysOfWeek{daysModel.inPolish[2]}").Value == "True" ? true : false;
            Day4 = da.LoadSetupEntry(name: $"freeDaysOfWeek{daysModel.inPolish[3]}").Value == "True" ? true : false;
            Day5 = da.LoadSetupEntry(name: $"freeDaysOfWeek{daysModel.inPolish[4]}").Value == "True" ? true : false;
            Day6 = da.LoadSetupEntry(name: $"freeDaysOfWeek{daysModel.inPolish[5]}").Value == "True" ? true : false;
            Day7 = da.LoadSetupEntry(name: $"freeDaysOfWeek{daysModel.inPolish[6]}").Value == "True" ? true : false;
            NotifyOfPropertyChange();

        }

        public void GenerateFreeDaysInYear(int setupYear)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            string tmpHolidays = "";
            for (int setupMonth = 1; setupMonth <= 12; setupMonth++)
            {

                if (da.LoadSetupEntry(name: $"freeDaysOfWork{setupYear}-{setupMonth}").Value == null)
                {
                    if (setupMonth == 1) { tmpHolidays = "1 6"; }
                    if (setupMonth == 4) { tmpHolidays = "17 18"; }
                    if (setupMonth == 5) { tmpHolidays = "1 3"; }
                    if (setupMonth == 6) { tmpHolidays = "5 16"; }
                    if (setupMonth == 8) { tmpHolidays = "15"; }
                    if (setupMonth == 11) { tmpHolidays = "1 11"; }
                    if (setupMonth == 12) { tmpHolidays = "25 26"; }

                    da.SaveSetupEntry($"freeDaysOfWork{setupYear}-{setupMonth}", tmpHolidays);
                    da.SaveSetupEntry($"daysOfWork{setupYear}-{setupMonth}", "");
                }
            }
        }

        public void ButtonDeleteDatabase()
        {
            if (File.Exists(SqlLiteDataAcces.PlikBazyDanych))
            {
                var defineEraseData = MessageBox.Show("Czy jesteś pewnie że usunąc bazę danych?", "", MessageBoxButton.YesNo);
                if (defineEraseData == MessageBoxResult.Yes)
                {
                    File.Move(SqlLiteDataAcces.PlikBazyDanych,$"{DateTime.Now.Year:d2}{DateTime.Now.Month:d2}{DateTime.Now.Day}_{SqlLiteDataAcces.PlikBazyDanych}");
                    new SqlLiteDataAcces();
                    SqlLiteDataAcces.UtworzPustaBazeDanych(SqlLiteDataAcces.PlikBazyDanych);
                }
            }
        }
        public void InsertDays()
        {
            int setupYear = DateTime.Now.Year;
            int setupMonth = DateTime.Now.Month;
            GetFreeDays(setupYear, setupMonth);
        }

        public void SaveButton()
        {
            SaveSetup();
            MessageBox.Show("Zapisano dane do bazy");
        }

        public void SaveSetup()
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();

            da.SaveSetupEntry("eMailToSend", EdeMailToSend);
            da.SaveSetupEntry("whenSentRaport", EdWhenSentRaport.ToString());

            da.SaveSetupEntry($"freeDaysOfWeek{daysModel.inPolish[0]}", Day1.ToString());
            da.SaveSetupEntry($"freeDaysOfWeek{daysModel.inPolish[1]}", Day2.ToString());
            da.SaveSetupEntry($"freeDaysOfWeek{daysModel.inPolish[2]}", Day3.ToString());
            da.SaveSetupEntry($"freeDaysOfWeek{daysModel.inPolish[3]}", Day4.ToString());
            da.SaveSetupEntry($"freeDaysOfWeek{daysModel.inPolish[4]}", Day5.ToString());
            da.SaveSetupEntry($"freeDaysOfWeek{daysModel.inPolish[5]}", Day6.ToString());
            da.SaveSetupEntry($"freeDaysOfWeek{daysModel.inPolish[6]}", Day7.ToString());
            


            da.SaveSetupEntry("workingPlanningTimeStart", EdWorkingPlanningTimeStart);
            da.SaveSetupEntry("workingPlanningTimeEnd", EdWorkingPlanningTimeEnd);
            da.SaveSetupEntry("holidaysAtYear", EdHolidaysAtYear);
            da.SaveSetupEntry("holidaysByKids", EdHolidaysByKids);
            //da.SaveSetupEntry("minutesStartBreak", minutesStartBreak);
            //da.SaveSetupEntry("minutesEndBreak", minutesEndBreak);
            da.SaveSetupEntry("minutesStartWork", EdMinutesStartWork);
            da.SaveSetupEntry("minutesEndWork", EdMinutesEndWork);

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

            DialogInputViewModel getMonth = new DialogInputViewModel("Wprowadź dane", "Podaj rok i miesiąc dla dni wolnych", $"{setupYear}-{setupMonth}");
            var resultMonth = manager.ShowDialogAsync(getMonth, null, null);
            
                        string currentMonth = (resultMonth.Result==true? getMonth.Value: $"{setupYear}-{setupMonth}");

            if (currentMonth != $"{setupYear}-{setupMonth}")
            {
                setupYear = int.Parse(currentMonth.Split("-")[0]);
                setupMonth = int.Parse(currentMonth.Split("-")[1]);
            }

            string freeDaysOfWorkCurrent = da.LoadSetupEntry(name: $"freeDaysOfWork{setupYear}-{setupMonth}").Value;
            string daysOfWorkCurrent = da.LoadSetupEntry(name: $"daysOfWork{setupYear}-{setupMonth}").Value;
            CreateCalender(setupYear: setupYear, setupMonth: setupMonth, freeDaysOfWeek: ((freeDaysOfWeek.Count() > 0) ? freeDaysOfWeek : LoadListOfFreeDays()), freeDaysOfWork: freeDaysOfWorkCurrent, daysOfWork: daysOfWorkCurrent, true);

            if (freeDaysOfWorkCurrent == null)
            {
                freeDaysOfWorkCurrent = "";
            }

            DialogInputViewModel getFreeDaysInMonth = new DialogInputViewModel("Wprowadź dane", "Podaj dni wolne w miesiącu", freeDaysOfWorkCurrent);
            var resultFreeDaysInMonth = manager.ShowDialogAsync(getFreeDaysInMonth, null, null);
            freeDaysOfWorkCurrent = getFreeDaysInMonth.Value;

            DialogInputViewModel getWorkDaysInMonth = new DialogInputViewModel("Wprowadź dane", "Podaj dni pracujące w miesiącu", daysOfWorkCurrent);
            var resultWorkDaysInMonth = manager.ShowDialogAsync(getWorkDaysInMonth, null, null);
            daysOfWorkCurrent = getWorkDaysInMonth.Value;

            string tmpFreeDaysOfWorkCurrent = freeDaysOfWorkCurrent;
            string tmpDaysOfWorkCurrent = daysOfWorkCurrent;
 

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

        public (List<DateTime>, List<DateTime>) CreateCalender(int setupYear, int setupMonth, List<string> freeDaysOfWeek, string freeDaysOfWork, string daysOfWork, bool showCallender)
        {
            DaysModel days = new DaysModel();
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


            //// CALENDER
            /*var calender = new Calendar(setupYear, setupMonth);
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
            */
            return (listDaysInMonth, listWorkingDays);
        }


    }
}
