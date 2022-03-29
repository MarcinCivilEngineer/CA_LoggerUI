using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace WPF_LoggerTray
{
    public class LibUser
    {
        public static Dictionary<string, DateTime> getMachineLogonName(string machine)
        {
            var loggedOnUsers = new Dictionary<string, DateTime>();


            ManagementScope scope = new ManagementScope(String.Format(@"\\{0}\root\cimv2", machine));

            SelectQuery sessionQuery = new SelectQuery("Win32_LogonSession");

            using (ManagementObjectSearcher sessionSearcher = new ManagementObjectSearcher(scope, sessionQuery))
            using (ManagementObjectCollection sessionMOs = sessionSearcher.Get())
            {

                foreach (var sessionMO in sessionMOs)
                {
                    // Interactive sessions
                    if ((UInt32)sessionMO.Properties["LogonType"].Value == 2)
                    {
                        var logonId = (string)sessionMO.Properties["LogonId"].Value;
                        var startTimeString = (string)sessionMO.Properties["StartTime"].Value;
                        var startTime = DateTime.ParseExact(startTimeString.Substring(0, 21), "yyyyMMddHHmmss.ffffff", System.Globalization.CultureInfo.InvariantCulture);

                        WqlObjectQuery userQuery = new WqlObjectQuery(@"ASSOCIATORS OF {Win32_LogonSession.LogonId='" + logonId + @"'} WHERE AssocClass=Win32_LoggedOnUser");

                        using (var userSearcher = new ManagementObjectSearcher(scope, userQuery))
                        using (var userMOs = userSearcher.Get())
                        {
                            var username = userMOs.OfType<ManagementObject>().Select(u => (string)u.Properties["Name"].Value).FirstOrDefault();

                            if (!loggedOnUsers.ContainsKey(username))
                            {
                                loggedOnUsers.Add(username, startTime);
                            }
                            else if (loggedOnUsers[username] > startTime)
                            {
                                loggedOnUsers[username] = startTime;
                            }
                        }
                    }
                }

            }

            return loggedOnUsers;
        }

        public static Dictionary<string, DateTime> getMachineLogonFullName(string machine)
        {
            var loggedOnUsers = new Dictionary<string, DateTime>();


            ManagementScope scope = new ManagementScope(String.Format(@"\\{0}\root\cimv2", machine));

            SelectQuery sessionQuery = new SelectQuery("Win32_LogonSession");

            using (ManagementObjectSearcher sessionSearcher = new ManagementObjectSearcher(scope, sessionQuery))
            using (ManagementObjectCollection sessionMOs = sessionSearcher.Get())
            {

                foreach (var sessionMO in sessionMOs)
                {
                    // Interactive sessions
                    if ((UInt32)sessionMO.Properties["LogonType"].Value == 2)
                    {
                        var logonId = (string)sessionMO.Properties["LogonId"].Value;
                        var startTimeString = (string)sessionMO.Properties["StartTime"].Value;
                        var startTime = DateTime.ParseExact(startTimeString.Substring(0, 21), "yyyyMMddHHmmss.ffffff", System.Globalization.CultureInfo.InvariantCulture);

                        WqlObjectQuery userQuery = new WqlObjectQuery(@"ASSOCIATORS OF {Win32_LogonSession.LogonId='" + logonId + @"'} WHERE AssocClass=Win32_LoggedOnUser");

                        using (var userSearcher = new ManagementObjectSearcher(scope, userQuery))
                        using (var userMOs = userSearcher.Get())
                        {
                            var username = userMOs.OfType<ManagementObject>().Select(u => (string)u.Properties["FullName"].Value).FirstOrDefault();

                            if (!loggedOnUsers.ContainsKey(username))
                            {
                                loggedOnUsers.Add(username, startTime);
                            }
                            else if (loggedOnUsers[username] > startTime)
                            {
                                loggedOnUsers[username] = startTime;
                            }
                        }
                    }
                }

            }

            return loggedOnUsers;
        }
    }
}