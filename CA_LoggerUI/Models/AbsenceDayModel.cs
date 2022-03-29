using CA_Loger;
using System.Collections.Generic;

namespace CA_LoggerUI.Models
{

    public class AbsenceDayModel
    {
        public string tabela = "AbsenceDay";
        private List<string> list_zmienne = new List<string>();
        private List<string> list_wartosci = new List<string>();

        public void SqlCreate()
        {
            SqlLiteDataAcces.UtworzTabele($"CREATE TABLE IF NOT EXISTS {this.tabela} (Id INTEGER NOT NULL UNIQUE, IdAbsenceDates INT,  Day INT, Month INT, Year INT, TypeOfAbsence TEXT, PRIMARY KEY(Id AUTOINCREMENT))");
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
            list_zmienne.Add("IdAbsenceDates"); list_wartosci.Add(this.IdAbsenceDates.ToString());
            list_zmienne.Add("Day"); list_wartosci.Add(this.Day.ToString());
            list_zmienne.Add("Month"); list_wartosci.Add(this.Month.ToString());
            list_zmienne.Add("Year"); list_wartosci.Add(this.Year.ToString());
            list_zmienne.Add("TypeOfAbsence"); list_wartosci.Add(this.TypeOfAbsence.ToString());
        }
        public int Id { get; set; }
        public int IdAbsenceDates { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string TypeOfAbsence { get; set; }
    }
}
