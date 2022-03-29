using WPF_LoggerTray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_LoggerTray.Models
{

    public class WorkingDayBreaksModel
    {
        public string tabela = "WorkingDayBreaks";
        private List<string> list_zmienne = new List<string>();
        private List<string> list_wartosci = new List<string>();

        public void SqlCreate()
        {
            SqlLiteDataAcces.UtworzTabele($"CREATE TABLE IF NOT EXISTS {this.tabela} (Id INTEGER NOT NULL UNIQUE, Day INT, Month INT, Year INT, BreakTimeStart TEXT, BreakTimeEnd TEXT, Comment TEXT, Planing BOOL, PRIMARY KEY(Id AUTOINCREMENT))");
        }
        public void SqlUpdate()
        {
            SqlQuery();
            SqlLiteDataAcces.UpdateWierszId(this.tabela, this.list_zmienne, this.list_wartosci, this.Id);
        }
        public int SqlInsert()
        {
            SqlQuery();
            return SqlLiteDataAcces.WprowadzWierszPobierzId(this.tabela, this.list_zmienne, this.list_wartosci);
        }
        public void SqlQuery()
        {
            list_zmienne.Clear();
            list_wartosci.Clear();
            list_zmienne.Add("Day"); list_wartosci.Add(this.Day.ToString());
            list_zmienne.Add("Month"); list_wartosci.Add(this.Month.ToString());
            list_zmienne.Add("Year"); list_wartosci.Add(this.Year.ToString());
            list_zmienne.Add("BreakTimeStart"); list_wartosci.Add(LibDateTime.CorrectTimeFormat(this.BreakTimeStart.ToString()));
            list_zmienne.Add("BreakTimeEnd"); list_wartosci.Add(LibDateTime.CorrectTimeFormat(this.BreakTimeEnd.ToString()));
            list_zmienne.Add("Comment"); list_wartosci.Add(this.Comment.ToString());
            list_zmienne.Add("Planing"); list_wartosci.Add(this.Planing.ToString());

        }
        public int Id { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Comment { get; set; }
        public string BreakTimeStart { get; set; }
        public string BreakTimeEnd { get; set; }
        public bool Planing { get; set; }
    }
}
