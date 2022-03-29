using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.IO;
using Dapper;
using CA_LoggerUI.Models;

namespace CA_Loger
{

    public class SqlLiteDataAcces
    {

        public static string PlikBazyDanych  = "data.db";



        // DateStart TEXT, DateEnd TEXT, Comment TEXT, TypeOfAbsence TEXT,
        public List<AbsenceDatesModel> LoadAbsenceDates(int id = 0, string dateStart="", string TypeOfAbsence = "")
        {
            string tabela = new AbsenceDatesModel().tabela;
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                List<AbsenceDatesModel> output = new List<AbsenceDatesModel>();
                cnn.Open();
                if ((id != 0))
                {
                    output = cnn.Query<AbsenceDatesModel>($"select * from {tabela} where id={id}", new DynamicParameters()).ToList();
                }
                else if (TypeOfAbsence != "")
                {
                    output = cnn.Query<AbsenceDatesModel>($"select * from {tabela} where dateStart='{TypeOfAbsence}'", new DynamicParameters()).ToList();
                }
                else if (dateStart != "")
                {
                    output = cnn.Query<AbsenceDatesModel>($"select * from {tabela} where dateStart='{dateStart}'", new DynamicParameters()).ToList();
                }
                else
                {
                    output = cnn.Query<AbsenceDatesModel>($"select * from {tabela}", new DynamicParameters()).ToList();
                }
                cnn.Close();
                return output;
            }
        }

        public List<LogEntryModel> LoadLogEntry(int id = 0, string typEntry = "")
        {
            string tabela = new LogEntryModel().tabela;
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                List<LogEntryModel> output = new List<LogEntryModel>();
                cnn.Open();
                if ((id != 0))
                {
                    output = cnn.Query<LogEntryModel>($"select * from {tabela} where id={id}", new DynamicParameters()).ToList();
                }
                else if (typEntry != "")
                {
                    output = cnn.Query<LogEntryModel>($"select * from {tabela} where name='{typEntry}'", new DynamicParameters()).ToList();
                }
                else
                {
                    output = cnn.Query<LogEntryModel>($"select * from {tabela}", new DynamicParameters()).ToList();
                }
                cnn.Close();
                return output;
            }
        }

        public List<AbsenceDayModel> LoadAbsenceDay(int id = 0, int idAbsenceDay=0, int year = 0, int month = 0, int day = 0, string typeOfAbsence="")
        {
            string tabela = new AbsenceDayModel().tabela;
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                List<AbsenceDayModel> output = new List<AbsenceDayModel>();
                cnn.Open();
                if ((id != 0))
                {
                    output = cnn.Query<AbsenceDayModel>($"select * from {tabela} where id={id}{(typeOfAbsence!=""?$" and TypeOfAbsence='{typeOfAbsence}'":"")}", new DynamicParameters()).ToList();
                }
                else if (idAbsenceDay != 0)
                {
                    output = cnn.Query<AbsenceDayModel>($"select * from {tabela} where IdAbsenceDates={idAbsenceDay}", new DynamicParameters()).ToList();
                }
                else if ((year != 0) && (month != 0) && (day != 0))
                {
                    output = cnn.Query<AbsenceDayModel>($"select * from {tabela} where Year='{year}' and Month='{month}' and Day='{day}'{(typeOfAbsence != "" ? $" and TypeOfAbsence='{typeOfAbsence}'" : "")}", new DynamicParameters()).ToList();
                }
                else if ((year != 0) && (month != 0))
                {
                    output = cnn.Query<AbsenceDayModel>($"select * from {tabela} where Year='{year}' and Month='{month}'{(typeOfAbsence != "" ? $" and TypeOfAbsence='{typeOfAbsence}'" : "")}", new DynamicParameters()).ToList();
                }
                else if (year != 0)
                {
                    output = cnn.Query<AbsenceDayModel>($"select * from {tabela} where Year='{year}'{(typeOfAbsence != "" ? $" and TypeOfAbsence='{typeOfAbsence}'" : "")}", new DynamicParameters()).ToList();
                }
                else
                {
                    output = cnn.Query<AbsenceDayModel>($"select * from {tabela}{(typeOfAbsence != "" ? $" and TypeOfAbsence='{typeOfAbsence}'" : "")}", new DynamicParameters()).ToList();
                }
                cnn.Close();
                return output;
            }
        }

        public List<WorkingDayModel> LoadWorkingDay(int id = 0, int year = 0, int month = 0, int day = 0, bool planing=false)
        {
            string tabela = new WorkingDayModel().tabela;
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                List<WorkingDayModel> output = new List<WorkingDayModel>();
                cnn.Open();
                if ((id != 0))
                {
                    output = cnn.Query<WorkingDayModel>($"select * from {tabela} where id={id}", new DynamicParameters()).ToList();
                }
                else if ((year != 0)&&(month != 0) &&(day != 0))
                {
                    output = cnn.Query<WorkingDayModel>($"select * from {tabela} where Year='{year}' and Month='{month}' and Day='{day}' and Planing='{planing}'", new DynamicParameters()).ToList();
                }
                else if ((year != 0) && (month != 0))
                {
                    output = cnn.Query<WorkingDayModel>($"select * from {tabela} where Year='{year}' and Month='{month}' and Planing='{planing}'", new DynamicParameters()).ToList();
                }
                else if (year != 0)
                {
                    output = cnn.Query<WorkingDayModel>($"select * from {tabela} where Year='{year}' and Planing='{planing}'", new DynamicParameters()).ToList();
                }
                else
                {
                    output = cnn.Query<WorkingDayModel>($"select * from {tabela}", new DynamicParameters()).ToList();
                }
                cnn.Close();
                return output;
            }
        }

        public List<WorkingDayBreaksModel> LoadWorkingDayBreaks(int id = 0, int year = 0, int month = 0, int day = 0, bool planing = false)
        {
            string tabela = new WorkingDayBreaksModel().tabela;
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                List<WorkingDayBreaksModel> output = new List<WorkingDayBreaksModel>();
                cnn.Open();
                if ((id != 0))
                {
                    output = cnn.Query<WorkingDayBreaksModel>($"select * from {tabela} where id={id}", new DynamicParameters()).ToList();
                }
                else if ((year != 0) && (month != 0) && (day != 0))
                {
                    output = cnn.Query<WorkingDayBreaksModel>($"select * from {tabela} where Year='{year}' and Month='{month}' and Day='{day}' and Planing='{planing}'", new DynamicParameters()).ToList();
                }
                else if ((year != 0) && (month != 0))
                {
                    output = cnn.Query<WorkingDayBreaksModel>($"select * from {tabela} where Year='{year}' and Month='{month}' and Planing='{planing}'", new DynamicParameters()).ToList();
                }
                else if (year != 0)
                {
                    output = cnn.Query<WorkingDayBreaksModel>($"select * from {tabela} where Year='{year}' and Planing='{planing}'", new DynamicParameters()).ToList();
                }
                else
                {
                    output = cnn.Query<WorkingDayBreaksModel>($"select * from {tabela}", new DynamicParameters()).ToList();
                }
                cnn.Close();
                return output;
            }
        }

        public SetupEntryModel LoadSetupEntry(int id = 0, string name="")
        {
            string tabela = new SetupEntryModel().tabela;
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                List<SetupEntryModel> output = new List<SetupEntryModel>();
                cnn.Open();
                if ((id != 0))
                {
                    output = cnn.Query<SetupEntryModel>($"select * from {tabela} where id={id}", new DynamicParameters()).ToList();
                }
                else if (name != "")
                {
                    output = cnn.Query<SetupEntryModel>($"select * from {tabela} where Name='{name}'", new DynamicParameters()).ToList();
                }
                else
                {
                    output = cnn.Query<SetupEntryModel>($"select * from {tabela}", new DynamicParameters()).ToList();
                }
                cnn.Close();
                if (output.Count()== 0)
                {
                    return new SetupEntryModel() { Id = 0, Name = name, Value=null };
                } 
                else { 
                    return output.First();
                }
            }
        }

        public void SaveSetupEntry(string name, string value="")
        {
            
            string tabela = new SetupEntryModel().tabela;
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                List<SetupEntryModel> output = new List<SetupEntryModel>();
                cnn.Open();
                output = cnn.Query<SetupEntryModel>($"select * from {tabela} where name='{name}'", new DynamicParameters()).ToList();
                if (output.Count() == 0)
                {
                    new SetupEntryModel() { Id = 0, Name = name, Value = value }.SqlInsert();
                } 
                else {
                    new SetupEntryModel() { Id = LoadSetupEntry(name: name).Id, Name = name, Value = value }.SqlUpdate();
                }

                cnn.Close();
            }
        }

/*
public List<WorkTimeModel> LoadWorkTime(int id = 0,int idDayOfWork=0, bool breakAtWork=false)
{
    string tabela = new WorkTimeModel().tabela;
    using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
    {

        List<WorkTimeModel> output = new List<WorkTimeModel>();
        cnn.Open();
        if ((id != 0))
        {

            output = cnn.Query<WorkTimeModel>($"select * from {tabela} where id={id}", new DynamicParameters()).ToList();
        }
        else if ((idDayOfWork != 0))
        {
            output = cnn.Query<WorkTimeModel>($"select * from {tabela} where IdDayOfWork={idDayOfWork} AND BreakAtWork='{breakAtWork}'", new DynamicParameters()).ToList();
        }
        else
        {
            output = cnn.Query<WorkTimeModel>($"select * from {tabela}", new DynamicParameters()).ToList();

        }
        cnn.Close();
        return output;
    }
}


public List<DayOfWorkModel> LoadDayOfWork(int id = 0, int idMonth = 0)
{
    string tabela = new DayOfWorkModel().tabela;
    using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
    {

        List<DayOfWorkModel> output = new List<DayOfWorkModel>();
        cnn.Open();
        if ((id != 0))
        {

            output = cnn.Query<DayOfWorkModel>($"select * from {tabela} where id={id}", new DynamicParameters()).ToList();
        }
        else if ((idMonth != 0))
        {
            output = cnn.Query<DayOfWorkModel>($"select * from {tabela} where IdMonth={idMonth}", new DynamicParameters()).ToList();
        }
        else
        {
            output = cnn.Query<DayOfWorkModel>($"select * from {tabela}", new DynamicParameters()).ToList();

        }
        cnn.Close();
        return output;
    }
}

public List<MonthModel> LoadMonth(int id = 0, int idMonth = 0, int year=0, int month=0, string typ="")
{
    string tabela = new MonthModel().tabela;
    using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
    {
        List<MonthModel> output = new List<MonthModel>();
        cnn.Open();
        if ((id != 0))
        {
            output = cnn.Query<MonthModel>($"select * from {tabela} where id={id}", new DynamicParameters()).ToList();
        }
        else if ((idMonth != 0))
        {
            output = cnn.Query<MonthModel>($"select * from {tabela} where idMonth={idMonth}", new DynamicParameters()).ToList();
        }
        else if ((year > 0) && (month > 0))
        {
            output = cnn.Query<MonthModel>($"select * from {tabela} where Month={month} AND Year={year} AND Type='{typ}'", new DynamicParameters()).ToList();
        }
        else
        {
            output = cnn.Query<MonthModel>($"select * from {tabela}", new DynamicParameters()).ToList();

        }
        cnn.Close();
        return output;
    }
}
*/
public void UsunWorkingDayBreaks(int id)
{
    using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
    {
        cnn.Open();
        cnn.Execute($"delete from {new WorkingDayBreaksModel().tabela} where id = " + id);
        cnn.Close();
    }
}
        public void UsunWorkingDay(int id = 0)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                if (id > 0)
                {
                    cnn.Execute($"delete from {new WorkingDayBreaksModel().tabela} where id = " + id);
                }
                else
                {
                    cnn.Execute($"delete from {new WorkingDayBreaksModel().tabela}");
                }
                cnn.Close();
            }
        }

        public void UsunAbsenceDay(int id=0, int idAbsence=0)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                if (id > 0) { 
                cnn.Execute($"delete from {new AbsenceDayModel().tabela} where id = " + id);
                }
                else if (idAbsence > 0)
                {
                    cnn.Execute($"delete from {new AbsenceDayModel().tabela} where IdAbsenceDates='{idAbsence}'");
                }
                else
                {
                    cnn.Execute($"delete from {new AbsenceDayModel().tabela}");
                }
                cnn.Close();
            }
        }

        public void UsunAbsenceDates(int id = 0)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                if (id > 0)
                {
                    cnn.Execute($"delete from {new AbsenceDatesModel().tabela} where id = " + id);
                }
                else
                {
                    cnn.Execute($"delete from {new AbsenceDatesModel().tabela}");
                }
                cnn.Close();
            }
        }

        public static void WprowadzWiersz(string table, List<string> zmienne,List<string> wartosci)
        {

            string qZmienne = string.Join(",", zmienne);
            string qWartosci = string.Join("','", wartosci);

            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                cnn.Execute("insert into "+ table + " ("+ qZmienne + ") values ('"+qWartosci+"')");
                cnn.Close();
            }
        }
        public static int WprowadzWierszPobierzId(string table, List<string> zmienne, List<string> wartosci)
        {

            string qZmienne = string.Join(",", zmienne);
            string qWartosci = string.Join("','", wartosci);

            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                cnn.Execute("insert into " + table + " (" + qZmienne + ") values ('" + qWartosci + "')");
                int id = int.Parse(cnn.LastInsertRowId.ToString());
                cnn.Close();
                return id;
            }
        }
        public static void UpdateWierszId(string table, List<string> zmienne, List<string> wartosci, int id)
        {

            //string qZmienne = string.Join(",", zmienne);
            
            List<string> linia=new List<string>();

            for (int a = 0; a < zmienne.Count; a++)
            {
                linia.Add($" {zmienne[a]} = '{wartosci[a]}'");
            }

            string qWyrazenie = string.Join(",", linia);

            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                 cnn.Execute($"update {table} SET {qWyrazenie} WHERE id = {id}");
                cnn.Close();
            }
        }
        public static void UsunTabele(List<string> tabele)
        {
            using (SQLiteConnection con = new SQLiteConnection(LoadConnectionString()))
            {
                using (SQLiteCommand com = new SQLiteCommand(con))
                {
                    con.Open();                             // Open the connection to the database
                    foreach (string tabela in tabele)
                    {
                        com.CommandText = $"DROP TABLE IF EXISTS {tabela};";     // Set CommandText to our query that will create the table
                        com.ExecuteNonQuery();                  // Execute the query
                    }
                    con.Close();                            // Close the connection to the database
                }
            }
        }

        public static void UtworzTabele(string wiersz)
        {
            using (SQLiteConnection con = new SQLiteConnection(LoadConnectionString()))
            {
                using (SQLiteCommand com = new SQLiteCommand(con))
                {
                    con.Open();                             // Open the connection to the database
                    com.CommandText = wiersz;               // Set CommandText to our query that will create the table
                    com.ExecuteNonQuery();                  // Execute the query
                    con.Close();                            // Close the connection to the database
                }
            }

        }
        
        public static void UtworzPustaBazeDanych(string plik)
        {
            SQLiteConnection.CreateFile(plik);


            // update sql
        }
        public static void UpdateTable()
        {
            new SetupEntryModel().SqlCreate();
            new WorkingDayBreaksModel().SqlCreate();
            new WorkingDayModel().SqlCreate();
            new AbsenceDayModel().SqlCreate();
            new AbsenceDatesModel().SqlCreate();
            new LogEntryModel().SqlCreate();
        }
        private static string LoadConnectionString()
        {
            
            if (!File.Exists(PlikBazyDanych))
            {
                UtworzPustaBazeDanych(PlikBazyDanych);
            }
            return "Data Source=.\\"+PlikBazyDanych;

            


        }
        public SqlLiteDataAcces()
        {
            UpdateTable();
        }
    }
}
