using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CA_LoggerUI.Validator
{
    public class TimeValidator
    {
        public static bool IsHour(string enter)
        {
            bool isHour;
            isHour = int.TryParse(enter, out int a);
            if (isHour)
            {
                return false;
            } else
            {
                return TimeSpan.TryParse(enter, out TimeSpan t);
            }
        }

        public static bool IsAddMinutes(string enter)
        {
            if (enter.Length > 0)
            {
                if (enter[0] == '+')
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            }
            public static bool IsMinusMinutes(string enter)
        {
            if (enter.Length > 0)
            {
                if (enter[0] == '-')
            {
                return true;
            }
            else
            {
                return false;
            }
            }
            else
            {
                return false;
            }
        }
        public static bool TimeMoreThen(string end, string start)
        {
            return true;
        }
        public static bool IsExualTo(string enter, List<string> patern)
        {
            if (patern.Contains(enter)) { return true; } else { return false; }
        }

    }
}
