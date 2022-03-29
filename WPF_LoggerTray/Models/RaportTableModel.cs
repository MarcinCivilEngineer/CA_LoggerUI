using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WPF_LoggerTray
{
    
    public class RaportTableModel
    {
        public string colDay { get; set; }
        public string colFrom { get; set; }
        //public string colFromPlaning { get; set; } = "";
        public string colTo { get; set; }
        //public string colToPlaning { get; set; } = "";
        public string colType { get; set; }
        public string colComment { get; set; }
        //public string colCommentPlaning { get; set; }
        public string colBreakTime { get; set; }
        public string colWorkTime { get; set; }

        public string[] ConvertToArray()
        {
            string[] aa = { colDay, colFrom, colTo, colType, colComment, colBreakTime, colWorkTime };
            return aa;
        }
    }
}