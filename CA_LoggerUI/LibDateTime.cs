using System;

namespace CA_LoggerUI
{
    public class LibDateTime
    {
        public static int MinutesBetweenTime(string punchOut, string punchIn)
        {
            if ((punchOut!="")&& (punchIn != "")) { 
            DateTime endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(punchOut.Split(":")[0]), int.Parse(punchOut.Split(":")[1]), 0);
            DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(punchIn.Split(":")[0]), int.Parse(punchIn.Split(":")[1]), 0);
            TimeSpan timeWorked = new TimeSpan();
            timeWorked = endTime - startTime;
            return (int)timeWorked.TotalMinutes;
            } else { return 0; }
        }

        public static int MinutesFromTime(string punch="00:00")
        {
            if (punch != "") { 
            return int.Parse(punch.Split(":")[0]) * 60 + int.Parse(punch.Split(":")[1]);
            }
            else
            {
                return 0;
            }
        }

        public static string HoursFromMinutes(int minutes)
        {
            int hour = (int)Math.Round((decimal)minutes / 60, 0, MidpointRounding.ToZero);
            int minut = minutes - hour * 60;

            return $"{hour:d2}:{minut:d2}";
        }


        public static string HoursAddMinutes(string punch, int minutes)
        {
            DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(punch.Split(":")[0]), int.Parse(punch.Split(":")[1]), 0);
            startTime = startTime.AddMinutes(minutes);

            return $"{startTime.Hour:d2}:{startTime.Minute:d2}";
        }

        public static string HoursBetweenTime(DateTime punchOut, DateTime punchIn)
        {

            TimeSpan timeWorked = new TimeSpan();
            timeWorked = punchOut - punchIn;

            int elapsedMinutes;
            if (punchOut.Minute >= punchIn.Minute)
                elapsedMinutes = punchOut.Minute - punchIn.Minute;
            else
            {
                int secondsTo60 = 60 - punchIn.Minute;
                elapsedMinutes = punchOut.Minute + secondsTo60;
            }

            string lblTimeWorked = string.Format("{0:00}:{1:00}", (int)timeWorked.TotalHours, elapsedMinutes);
            return lblTimeWorked;
        }
        public static string HoursBetweenTime(string punchOutString, string punchInString)
        {
            DateTime punchOut = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(punchOutString.Split(":")[0]), int.Parse(punchOutString.Split(":")[1]), 0);
            DateTime punchIn = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(punchInString.Split(":")[0]), int.Parse(punchInString.Split(":")[1]), 0);
            TimeSpan timeWorked = new TimeSpan();
            timeWorked = punchOut - punchIn;

            int elapsedMinutes;
            if (punchOut.Minute >= punchIn.Minute)
                elapsedMinutes = punchOut.Minute - punchIn.Minute;
            else
            {
                int secondsTo60 = 60 - punchIn.Minute;
                elapsedMinutes = punchOut.Minute + secondsTo60;
            }

            string lblTimeWorked = string.Format("{0:00}:{1:00}", (int)timeWorked.TotalHours, elapsedMinutes);
            return lblTimeWorked;
        }


        public static DateTime DateFromString(string data)
        {
            return new DateTime(int.Parse(data.Split("-")[0]), int.Parse(data.Split("-")[1]), int.Parse(data.Split("-")[2]));
        }
        public static string StringFromDate(DateTime date)
        {
            return $"{date.Year:d4}-{date.Month:D2}-{date.Day:D2}";
        }

        public static string CorrectTimeFormat(string inn)
        {
            string sss = "";
            if (inn != "") { 
            sss = $"{int.Parse(inn.Split(":")[0]):d2}:{int.Parse(inn.Split(":")[1]):d2}";
            }
            return sss;
        }
    }
}
