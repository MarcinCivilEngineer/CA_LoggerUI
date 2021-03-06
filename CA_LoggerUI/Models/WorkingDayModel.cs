using CA_Loger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA_LoggerUI.Models
{

    public class WorkingDayModel
    {
        public string tabela = "WorkingDay";
        private List<string> list_zmienne = new List<string>();
        private List<string> list_wartosci = new List<string>();

        public void SqlCreate()
        {
            SqlLiteDataAcces.UtworzTabele($"CREATE TABLE IF NOT EXISTS {this.tabela} (Id INTEGER NOT NULL UNIQUE, Day INT, Month INT, Year INT, WorkTimeStart TEXT, WorkTimeEnd TEXT, Comment TEXT, Planing BOOL, PRIMARY KEY(Id AUTOINCREMENT))");
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
            list_zmienne.Add("WorkTimeStart"); list_wartosci.Add(LibDateTime.CorrectTimeFormat(this.WorkTimeStart.ToString()));
            list_zmienne.Add("WorkTimeEnd"); list_wartosci.Add(LibDateTime.CorrectTimeFormat(this.WorkTimeEnd.ToString()));
            list_zmienne.Add("Comment"); list_wartosci.Add(this.Comment.ToString());
            list_zmienne.Add("Planing"); list_wartosci.Add(this.Planing.ToString());

        }
        public int Id { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Comment { get; set; }
        public string WorkTimeStart { get; set; }
        public string WorkTimeEnd { get; set; }
        public bool Planing { get; set; }
    }
}


