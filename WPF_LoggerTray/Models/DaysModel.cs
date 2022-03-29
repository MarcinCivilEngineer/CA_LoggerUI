using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_LoggerTray.Models
{
    public class DaysModel
    {
        public List<string> inPolish = new List<string>(){ "Niedziela", "Poniedziałek", "Wtorek", "Środa", "Czwartek", "Piątek", "Sobota" };
        public string FromAndToPolish(string dayInEnglish)
        {
            string outName="";
            switch (dayInEnglish)
            {
                case "Mon":
                    outName = "Poniedziałek";
                    break;

            }
            return outName;
        }

        public DayOfWeek FromPolishToEnglish(string dayInPolish)
        {
            string outName = "";
            DayOfWeek dayOfWeek=new DayOfWeek();
            switch (dayInPolish)
            {
                case "Niedziela":
                    outName = "Sunday";
                    dayOfWeek = DayOfWeek.Sunday;
                    break;

                case "Poniedziałek":
                    outName = "Monday";
                    dayOfWeek = DayOfWeek.Monday;
                    break;

                case "Wtorek":
                    outName = "Tuesday";
                    dayOfWeek = DayOfWeek.Tuesday;
                    break;

                case "Środa":
                    outName = "Wednesday";
                    dayOfWeek = DayOfWeek.Wednesday;
                    break;

                case "Czwartek":
                    outName = "Thursday";
                    dayOfWeek = DayOfWeek.Thursday;
                    break;

                case "Piątek":
                    outName = "Friday";
                    dayOfWeek = DayOfWeek.Friday;
                    break;

                case "Sobota":
                    outName = "Saturday";
                    dayOfWeek = DayOfWeek.Saturday;
                    break;
                    
            }
            return dayOfWeek;
        }




        public string ToEnglish(DayOfWeek day)
        {

            string outName = "";
            switch (day)
            {
                case DayOfWeek.Sunday:
                    outName = "Sunday";
                    break;

                case DayOfWeek.Monday:
                    outName = "Monday";
                    break;

                case DayOfWeek.Tuesday:
                    outName = "Tuesday";
                    break;

                case DayOfWeek.Wednesday:
                    outName = "Wednesday";
                    break;

                case DayOfWeek.Thursday:
                    outName = "Thursday";
                    break;

                case DayOfWeek.Friday:
                    outName = "Friday";
                    break;

                case DayOfWeek.Saturday:
                    outName = "Saturday";
                    break;
            }
            return outName;
        }


        public string ToPolish(DayOfWeek day)
        {
            
            string outName = "";
            switch (day)
            {
                case DayOfWeek.Sunday:
                    outName = "Niedziela";
                    break;

                case DayOfWeek.Monday:
                    outName = "Poniedziałek";
                    break;

                case DayOfWeek.Tuesday:
                    outName = "Wtorek";
                    break;

                case DayOfWeek.Wednesday:
                    outName = "Środa";
                    break;

                case DayOfWeek.Thursday:
                    outName = "Czwartek";
                    break;

                case DayOfWeek.Friday:
                    outName = "Piątek";
                    break;

                case DayOfWeek.Saturday:
                    outName = "Sobota";
                    break;
            }
            return outName;
        }

        public List<DayOfWeek> DaysOfWeek(List<string> listOfDays)
        {
            List<DayOfWeek> output = new List<DayOfWeek>();
            foreach (string day in listOfDays)
            {
                output.Add(FromPolishToEnglish(day));
            }
            return output;
        }


        
        public DaysModel()
        {

        }
    }
}
