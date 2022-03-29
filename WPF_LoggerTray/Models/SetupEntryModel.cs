using WPF_LoggerTray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_LoggerTray.Models
{
    
    public class LogEntryModel
    {
        public string tabela = "LogEntry";
        private List<string> list_zmienne = new List<string>();
        private List<string> list_wartosci = new List<string>();

        public void SqlCreate()
        {
            SqlLiteDataAcces.UtworzTabele($"CREATE TABLE IF NOT EXISTS {this.tabela} (Id INTEGER NOT NULL UNIQUE, Type TEXT, DateEvent DATETIME, PRIMARY KEY(Id AUTOINCREMENT))");
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
            list_zmienne.Add("Type"); list_wartosci.Add(this.Type.ToString());
            list_zmienne.Add("DateEvent"); list_wartosci.Add(this.DateEvent.ToString());
        }

        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime DateEvent { get; set; }

    }
}
