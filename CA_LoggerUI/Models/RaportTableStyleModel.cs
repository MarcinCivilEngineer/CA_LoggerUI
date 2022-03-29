using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA_LoggerUI.Models
{
    public class Row
    {
        public string DayPre { get; set; }
        public string DayPost { get; set; }

        public string TimeFromPre { get; set; }
        public string TimeFromPost { get; set; }
        public string TimeToPre { get; set; }
        public string TimeToPost { get; set; }
        public string SymbolPre { get; set; }
        public string SymbolPost { get; set; }
        public string CommentPre { get; set; }
        public string CommentPost { get; set; }
        public string BreakTimePre { get; set; }
        public string BreakTimePost { get; set; }
        public string TimeAtWorkPre { get; set; }
        public string TimeAtWorkPost { get; set; }
    }
    public class RaportTableStyleModel
    {
        public Row Normal { get; set; } = new Row();
        public Row Planing { get; set; } = new Row();
        public Row Holiday { get; set; } = new Row();
        public Row Break { get; set; } = new Row();
        public Row Current { get; set; } = new Row();
        public Row Free { get; set; } = new Row();

        public RaportTableStyleModel(string typeTable)
        {
            if (typeTable == "spectre") {
                Normal = new Row
                {
                    DayPre = "",
                    DayPost = "",
                    TimeFromPre = "",
                    TimeFromPost = "",
                    TimeToPre = "",
                    TimeToPost = "",
                    SymbolPre = "",
                    SymbolPost = "",
                    CommentPre = "",
                    CommentPost = "",
                    BreakTimePre = "",
                    BreakTimePost = "",
                    TimeAtWorkPre = "",
                    TimeAtWorkPost = "",
                };
                Planing = new Row
                {
                    DayPre = "",
                    DayPost = "",
                    TimeFromPre = "[blue](",
                    TimeFromPost = ")[/]",
                    TimeToPre = "[blue](",
                    TimeToPost = ")[/]",
                    SymbolPre = "[blue](",
                    SymbolPost = ")[/]",
                    CommentPre = "[blue](",
                    CommentPost = ")[/]",
                    BreakTimePre = "[blue](",
                    BreakTimePost = ")[/]",
                    TimeAtWorkPre = "[blue](",
                    TimeAtWorkPost = ")[/]",
                };
                Holiday = new Row
                {
                    DayPre = "",
                    DayPost = "",
                    TimeFromPre = "",
                    TimeFromPost = "",
                    TimeToPre = "",
                    TimeToPost = "",
                    SymbolPre = "[darkorange]",
                    SymbolPost = "[/]",
                    CommentPre = "[darkorange]",
                    CommentPost = "[/]",
                    BreakTimePre = "",
                    BreakTimePost = "",
                    TimeAtWorkPre = "",
                    TimeAtWorkPost = "",
                };
                Break = new Row
                {
                    DayPre = "",
                    DayPost = "",
                    TimeFromPre = "[darkorange]",
                    TimeFromPost = "[/]",
                    TimeToPre = "[darkorange]",
                    TimeToPost = "[/]",
                    SymbolPre = "[darkorange]",
                    SymbolPost = "[/]",
                    CommentPre = "[darkorange]",
                    CommentPost = "[/]",
                    BreakTimePre = "[darkorange]",
                    BreakTimePost = "[/]",
                    TimeAtWorkPre = "[darkorange]",
                    TimeAtWorkPost = "[/]",
                };
                Current = new Row
                {
                    DayPre = "[yellow]",
                    DayPost = "[/]",
                    TimeFromPre = "[yellow]",
                    TimeFromPost = "[/]",
                    TimeToPre = "[yellow]",
                    TimeToPost = "[/]",
                    SymbolPre = "",
                    SymbolPost = "",
                    CommentPre = "",
                    CommentPost = "",
                    BreakTimePre = "",
                    BreakTimePost = "",
                    TimeAtWorkPre = "",
                    TimeAtWorkPost = "",
                };
                Free = new Row
                {
                    DayPre = "[red]",
                    DayPost = "[/]",
                    TimeFromPre = "",
                    TimeFromPost = "",
                    TimeToPre = "",
                    TimeToPost = "",
                    SymbolPre = "",
                    SymbolPost = "",
                    CommentPre = "",
                    CommentPost = "",
                    BreakTimePre = "",
                    BreakTimePost = "",
                    TimeAtWorkPre = "",
                    TimeAtWorkPost = "",
                };
            }

            if (typeTable == "html")
            {
                Normal = new Row
                {
                    DayPre = "",
                    DayPost = "",
                    TimeFromPre = "",
                    TimeFromPost = "",
                    TimeToPre = "",
                    TimeToPost = "",
                    SymbolPre = "",
                    SymbolPost = "",
                    CommentPre = "",
                    CommentPost = "",
                    BreakTimePre = "",
                    BreakTimePost = "",
                    TimeAtWorkPre = "",
                    TimeAtWorkPost = "",
                };
                Planing = new Row
                {
                    DayPre = "",
                    DayPost = "",
                    TimeFromPre = "<font style='color:blue'>(",
                    TimeFromPost = ")</font>",
                    TimeToPre = "<font style='color:blue'>(",
                    TimeToPost = ")</font>",
                    SymbolPre = "<font style='color:blue'>(",
                    SymbolPost = ")</font>",
                    CommentPre = "<font style='color:blue'>(",
                    CommentPost = ")</font>",
                    BreakTimePre = "<font style='color:blue'>(",
                    BreakTimePost = ")</font>",
                    TimeAtWorkPre = "<font style='color:blue'>(",
                    TimeAtWorkPost = ")</font>",
                };
                Holiday = new Row
                {
                    DayPre = "",
                    DayPost = "",
                    TimeFromPre = "",
                    TimeFromPost = "",
                    TimeToPre = "",
                    TimeToPost = "",
                    SymbolPre = "<font style='color:orange'>",
                    SymbolPost = "</font>",
                    CommentPre = "<font style='color:orange'>",
                    CommentPost = "</font>",
                    BreakTimePre = "",
                    BreakTimePost = "",
                    TimeAtWorkPre = "",
                    TimeAtWorkPost = "",
                };
                Break = new Row
                {
                    DayPre = "",
                    DayPost = "",
                    TimeFromPre = "<font style='color:orange'>",
                    TimeFromPost = "</font>",
                    TimeToPre = "<font style='color:orange'>",
                    TimeToPost = "</font>",
                    SymbolPre = "<font style='color:orange'>",
                    SymbolPost = "</font>",
                    CommentPre = "<font style='color:orange'>",
                    CommentPost = "</font>",
                    BreakTimePre = "<font style='color:orange'>",
                    BreakTimePost = "</font>",
                    TimeAtWorkPre = "<font style='color:orange'>",
                    TimeAtWorkPost = "</font>",
                };
                Current = new Row
                {
                    DayPre = "<font style='color:orange'><b>",
                    DayPost = "</b></font>",
                    TimeFromPre = "<font style='color:orange'><b>",
                    TimeFromPost = "</b></font>",
                    TimeToPre = "<font style='color:orange'><b>",
                    TimeToPost = "</b></font>",
                    SymbolPre = "",
                    SymbolPost = "",
                    CommentPre = "",
                    CommentPost = "",
                    BreakTimePre = "",
                    BreakTimePost = "",
                    TimeAtWorkPre = "",
                    TimeAtWorkPost = "",
                };
                Free = new Row
                {
                    DayPre = "<font style='color:red'>",
                    DayPost = "</font>",
                    TimeFromPre = "",
                    TimeFromPost = "",
                    TimeToPre = "",
                    TimeToPost = "",
                    SymbolPre = "",
                    SymbolPost = "",
                    CommentPre = "",
                    CommentPost = "",
                    BreakTimePre = "",
                    BreakTimePost = "",
                    TimeAtWorkPre = "",
                    TimeAtWorkPost = "",
                };
            }

            if (typeTable == "csv")
            {
                Normal = new Row
                {
                    DayPre = "",
                    DayPost = "",
                    TimeFromPre = "",
                    TimeFromPost = "",
                    TimeToPre = "",
                    TimeToPost = "",
                    SymbolPre = "",
                    SymbolPost = "",
                    CommentPre = "",
                    CommentPost = "",
                    BreakTimePre = "",
                    BreakTimePost = "",
                    TimeAtWorkPre = "",
                    TimeAtWorkPost = "",
                };
                Planing = new Row
                {
                    DayPre = "",
                    DayPost = "",
                    TimeFromPre = "(",
                    TimeFromPost = ")",
                    TimeToPre = "(",
                    TimeToPost = ")",
                    SymbolPre = "(",
                    SymbolPost = ")",
                    CommentPre = "(",
                    CommentPost = ")",
                    BreakTimePre = "(",
                    BreakTimePost = ")",
                    TimeAtWorkPre = "(",
                    TimeAtWorkPost = ")",
                };
                Holiday = new Row
                {
                    DayPre = "",
                    DayPost = "",
                    TimeFromPre = "",
                    TimeFromPost = "",
                    TimeToPre = "",
                    TimeToPost = "",
                    SymbolPre = "",
                    SymbolPost = "",
                    CommentPre = "",
                    CommentPost = "",
                    BreakTimePre = "",
                    BreakTimePost = "",
                    TimeAtWorkPre = "",
                    TimeAtWorkPost = "",
                };
                Break = new Row
                {
                    DayPre = "",
                    DayPost = "",
                    TimeFromPre = "",
                    TimeFromPost = "",
                    TimeToPre = "",
                    TimeToPost = "",
                    SymbolPre = "",
                    SymbolPost = "",
                    CommentPre = "",
                    CommentPost = "",
                    BreakTimePre = "",
                    BreakTimePost = "",
                    TimeAtWorkPre = "",
                    TimeAtWorkPost = "",
                };
                Current = new Row
                {
                    DayPre = "",
                    DayPost = "",
                    TimeFromPre = "",
                    TimeFromPost = "",
                    TimeToPre = "",
                    TimeToPost = "",
                    SymbolPre = "",
                    SymbolPost = "",
                    CommentPre = "",
                    CommentPost = "",
                    BreakTimePre = "",
                    BreakTimePost = "",
                    TimeAtWorkPre = "",
                    TimeAtWorkPost = "",
                };
                Free = new Row
                {
                    DayPre = "",
                    DayPost = "",
                    TimeFromPre = "",
                    TimeFromPost = "",
                    TimeToPre = "",
                    TimeToPost = "",
                    SymbolPre = "",
                    SymbolPost = "",
                    CommentPre = "",
                    CommentPost = "",
                    BreakTimePre = "",
                    BreakTimePost = "",
                    TimeAtWorkPre = "",
                    TimeAtWorkPost = "",
                };
            }

        }
    }
}
