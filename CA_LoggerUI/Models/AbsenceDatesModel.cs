using CA_Loger;
using System;
using System.Collections.Generic;

namespace CA_LoggerUI.Models
{

    public class AbsenceDatesModel
    {
        public string tabela = "AbsenceDates";
        private List<string> list_zmienne = new List<string>();
        private List<string> list_wartosci = new List<string>();

        public void SqlCreate()
        {
            SqlLiteDataAcces.UtworzTabele($"CREATE TABLE IF NOT EXISTS {this.tabela} (Id INTEGER NOT NULL UNIQUE, DateStart TEXT, DateEnd TEXT, Comment TEXT, TypeOfAbsence TEXT, PRIMARY KEY(Id AUTOINCREMENT))");
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
            list_zmienne.Add("DateStart"); list_wartosci.Add(this.DateStart.ToString());
            list_zmienne.Add("DateEnd"); list_wartosci.Add(this.DateEnd.ToString());
            list_zmienne.Add("Comment"); list_wartosci.Add(this.Comment.ToString());
            list_zmienne.Add("TypeOfAbsence"); list_wartosci.Add(this.TypeOfAbsence.ToString());
        }
        public int Id { get; set; }
        public string DateStart { get; set; }
        public DateTime Start
        {
            get
            {
                return new DateTime( int.Parse(DateStart.Split("-")[0]), int.Parse(DateStart.Split("-")[1]), int.Parse(DateStart.Split("-")[2]));
            }
        }

        public string DateEnd { get; set; }
        public DateTime End
        {
            get
            {
                return new DateTime(int.Parse(DateEnd.Split("-")[0]), int.Parse(DateEnd.Split("-")[1]), int.Parse(DateEnd.Split("-")[2]));
            }
        }

        public string Comment { get; set; }
        public string TypeOfAbsence { get; set; }
    }
}
