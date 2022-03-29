using CA_LoggerUI.Views;
using System;
//using System.Windows.Forms;

//using System.Drawing;
//using System.Windows.Shell;
using CA_LoggerUI.Models;
using CA_Loger;
using System.Linq;
using System.Collections.Generic;
//using Microsoft.WindowsAPICodePack.Taskbar;
//using Microsoft.Win32;
//using Microsoft.WindowsAPICodePack.Taskbar;
using Spectre.Console;
//using Microsoft.WindowsAPICodePack;


using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;

public delegate void menuItemStartWork_MouseClick(object sender, EventArgs e);
public delegate void menuItemEndWork_MouseClick(object sender, EventArgs e);
public delegate void menuItemClose_MouseClick(object sender, EventArgs e);


namespace CA_LoggerUI
{

    //public event EventHandler Click;

    public class Program
    {


        public static ContextMenuStrip menu;
        public static ToolStripMenuItem mnuExit;
        public static NotifyIcon NotIcon { get; set; }
        //public static NotifyIcon NotIcon;

        //static NotifyIcon notifyIcon = new NotifyIcon();
        static bool Visible = true;
        //static JumpTask jumpTask1 = new JumpTask();

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public static void SetConsoleWindowVisibility(bool visible)
        {
            IntPtr hWnd = FindWindow(null, Console.Title);
            if (hWnd != IntPtr.Zero)
            {
                if (visible) ShowWindow(hWnd, 1); //1 = SW_SHOWNORMAL           
                else ShowWindow(hWnd, 0); //0 = SW_HIDE               
            }
        }


        //public static System.DirectoryServices.ActiveDirectory.

        /*
        private void AddTask(object sender, RoutedEventArgs e)
        {
            // Configure a new JumpTask.
            JumpTask jumpTask1 = new JumpTask();
            // Get the path to Calculator and set the JumpTask properties.
            jumpTask1.ApplicationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "calc.exe");
            jumpTask1.IconResourcePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "calc.exe");
            jumpTask1.Title = "Calculator";
            jumpTask1.Description = "Open Calculator.";
            jumpTask1.CustomCategory = "User Added Tasks";
            // Get the JumpList from the application and update it.
            JumpList jumpList1 = JumpList.GetJumpList(App.Current);
            jumpList1.JumpItems.Add(jumpTask1);
            JumpList.AddToRecentCategory(jumpTask1);
            jumpList1.Apply();
        }
        private void ClearJumpList(object sender, RoutedEventArgs e)
        {
            JumpList jumpList1 = JumpList.GetJumpList(App.Current);
            jumpList1.JumpItems.Clear();
            jumpList1.Apply();
        }
        private void SetNewJumpList(object sender, RoutedEventArgs e)
        {
            //Configure a new JumpTask
            JumpTask jumpTask1 = new JumpTask();
            // Get the path to WordPad and set the JumpTask properties.
            jumpTask1.ApplicationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "write.exe");
            jumpTask1.IconResourcePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "write.exe");
            jumpTask1.Title = "WordPad";
            jumpTask1.Description = "Open WordPad.";
            jumpTask1.CustomCategory = "Jump List 2";
            // Create and set the new JumpList.
            JumpList jumpList2 = new JumpList();
            jumpList2.JumpItems.Add(jumpTask1);
            JumpList.SetJumpList(App.Current, jumpList2);
        }


        */

        [STAThread]
        static void Main(string[] args)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();


           

         //= new NotifyIcon();

            // Standard message loop to catch click-events on notify icon
            // Code after this method will be running only after Application.Exit()





            //Console.Title = "Rejestrator czasu pracy - autor: Marcin Rutkowski";
            Console.WindowHeight = 52;

            string LocalIp = string.Empty;
            string Domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string Host = System.Net.Dns.GetHostName();

            if (Domain.Contains("betard", StringComparison.InvariantCultureIgnoreCase))
            {
               AnsiConsole.MarkupLine("[red]Komputer z poza domeny posiadającej licencje na użytkowanie.[/]");
                AnsiConsole.MarkupLine("Celem zdobycia licencji proszę o kontak z autorem programu: [green]m.rutkowski@betard.pl[/]");
                Console.ReadKey();
            }
            else {

                NotIcon = new NotifyIcon();
                ShowTrayIcon();

                /*
                foreach (string ar in args)
                {
                    Console.WriteLine(ar);
                }
                Console.ReadKey();
                */


                if (args.Length > 0)
                {
                    var command = args[0];
                    int minutes = 0;
                    minutes = GetMinutesFromDataBase(command);

                    if (args.Length > 1)
                    {
                        int.TryParse(args[1], out minutes);
                    }

                    bool rewriteTime = false;
                    WorkingDayModel workingDay = new WorkingDayModel();
                    switch (command)
                    {
                        case "StartWork":
                            {

                                if (da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).Count() > 0)
                                {
                                    workingDay = da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).First();
                                    if (workingDay.WorkTimeStart != "")
                                    {
                                        rewriteTime = AnsiConsole.Confirm($"Godzina rozpoczecia pracy wprowadzona już na [blue]{workingDay.WorkTimeStart}[/], nadpisać na [green]{LibDateTime.HoursAddMinutes($"{DateTime.Now.Hour}:{DateTime.Now.Minute}", minutes)}[/]", true);
                                    }
                                }
                                if (((da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).Count() > 0) ? (da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).First().WorkTimeStart == "") : false) || (rewriteTime) || (da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).Count() == 0))
                                {
                                    EventStartWork(minutes);
                                    BalonMessage(NotIcon, "Rejestr zdarzenia", $"Rozpoczęto pracę: {DateTime.Now.AddMinutes(minutes)}", "WORK START");
                                }

                                break;
                            }
                        case "EndWork":
                            {
                                if (da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).Count() > 0)
                                {
                                    workingDay = da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).First();
                                    if (workingDay.WorkTimeEnd != "")
                                    {
                                        rewriteTime = AnsiConsole.Confirm($"Godzina zakończenia pracy wprowadzona już na [blue]{workingDay.WorkTimeEnd}[/], nadpisać na [green]{LibDateTime.HoursAddMinutes($"{DateTime.Now.Hour}:{DateTime.Now.Minute}", minutes)}[/]", true);
                                    }
                                }
                                int sasf = da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).Count;
                                if (((da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).Count() > 0) ? (da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).First().WorkTimeEnd == "") : false) || (rewriteTime) || (da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).Count() == 0))
                                {

                                    EventEndWork(minutes);
                                    BalonMessage(NotIcon, "Rejestr zdarzenia", $"Zakończono pracę: {DateTime.Now.AddMinutes(minutes)}", "WORK END");
                                }

                                break;
                            }
                        case "StartBreak":
                            {
                                EventStartBreak(minutes);
                                BalonMessage(NotIcon, "Rejestr zdarzenia", $"Rozpoczęto przerwę: {DateTime.Now.AddMinutes(minutes)}", "BREAK START");
                                break;
                            }
                        case "EndBreak":
                            {
                                EventEndBreak(minutes);
                                BalonMessage(NotIcon, "Rejestr zdarzenia", $"Zakończono przerwę: {DateTime.Now.AddMinutes(minutes)}", "BREAK END");
                                break;
                            }
                        default:
                            {
                                Console.WriteLine("Invalid command");

                                break;
                            }
                    }
                }
                else
                {
                    BalonMessage(NotIcon, "Rejestr zdarzenia", $"Uruchomino program: {DateTime.Now}", "PROGRAM START");

                    /*
                    ContextMenuStrip contextMenu = new ContextMenuStrip();
                    ToolStripMenuItem toolStripMenuStopWork = new ToolStripMenuItem();
                    toolStripMenuStopWork.Text = "Zakończ pracę";
                    toolStripMenuStopWork.Click += new EventHandler(toolStripMenuStopWork_Click);
                    
                    contextMenu.Items.Add(toolStripMenuStopWork);

                    ToolStripMenuItem toolStripMenuStartWork = new ToolStripMenuItem();
                    toolStripMenuStartWork.Text = "Rozpocznij pracę";
                    //toolStripMenuStartWork.Name = "StartWork";
                    
                    contextMenu.Items.Add(toolStripMenuStartWork);

                    ToolStripMenuItem toolStripMenuClose = new ToolStripMenuItem();
                    toolStripMenuClose.Text = "Zamknij program";
                    //toolStripMenuClose.Name = "Close";
                    contextMenu.Items.Add(toolStripMenuClose);
                    

                    NotIcon.ContextMenuStrip = contextMenu;
                    */

                    DefaultsSettingsView defaultsSettings = new DefaultsSettingsView();
                    if (!defaultsSettings.isLoadedFromSQL)
                    {
                        defaultsSettings.GetSetup();
                    }

                    MainMenuView mainMenu = new MainMenuView();

                    System.Timers.Timer timer = new System.Timers.Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);
                    timer.AutoReset = true;
                    timer.Elapsed += new System.Timers.ElapsedEventHandler(SetGetStatus);
                    timer.Start();
                    
                    bool defineAgainChoose = true;
                    while (defineAgainChoose == true)
                    {
                        NotIcon.Text = SetStatus();
                        defineAgainChoose = mainMenu.Schow();
                    }
                    BalonMessage(NotIcon, "Rejestr zdarzenia", $"Zamknięto program: {DateTime.Now}", "PROGRAM END");
                    NotIcon.Visible = false;
                    
                }




                ///
                /// Notify from tray
                /// 





                /*
                var contextMenu = new ContextMenu();
                MenuItem menuItem = new MenuItem();
                menuItem.Name = "ssaa";
                menuItem.Text = "asafsa";
                contextMenu.MenuItems.Add(menuItem);
                icon.ContextMenu = contextMenu;
                */




                /* ###########################################
                MenuItem[] menuList = new MenuItem[]{new MenuItem("Sign In"),
                new MenuItem("Get Help"), new MenuItem("Open")};
                ContextMenu clickMenu = new ContextMenu(menuList);
                NotIcon.ContextMenu = clickMenu;
                NotIcon.Click += new System.EventHandler(Program.Main, NotIcon_Click);
                */

                /*
                JumpList jumpList = new JumpList();

                //JumpList.SetJumpList(Application.Current, jumpList);

                JumpTask helloTask = new JumpTask();
                helloTask.Title = "Wprowadź rozpoczęcie pracy";
                helloTask.Description = "Komentarz";
                helloTask.ApplicationPath = Assembly.GetEntryAssembly().Location;
                helloTask.Arguments = "-StartWork 0";
                jumpList.JumpItems.Add(helloTask);

                jumpList.Apply();
                */

                //SystemEvents.SessionEnding += (SystemEvents_SessionEnding);
                //SystemEvents.SessionEnded += (SystemEvents_SessionEnded);


                //AppDomain.CurrentDomain.ProcessExit()


            }


            /*
                static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
            {
                Console.WriteLine("aa"+e.Reason);
                Console.ReadKey();
                //code to run on when user is about to logoff or shutdown
            }
            */

            /*
            static void SystemEvents_SessionEnded(object sender, SessionEndedEventArgs e)
            {
                Console.WriteLine("aa "+e.Reason);
                Console.ReadKey();
                //code to run on when user is about to logoff or shutdown
            }
            */
        }

        private static int GetMinutesFromDataBase(string command)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            int minutes;
            switch (command)
            {
                case "StartWork":
                    minutes = int.Parse(da.LoadSetupEntry(name: $"minutesStartWork").Value == null ? "0" : da.LoadSetupEntry(name: $"minutesStartWork").Value);
                    break;
                case "EndWork":// when args.Length == 3 && args[1] == "-m":
                    minutes = int.Parse(da.LoadSetupEntry(name: $"minutesEndWork").Value == null ? "0" : da.LoadSetupEntry(name: $"minutesEndWork").Value);
                    break;
                case "StartBreak":
                    minutes = int.Parse(da.LoadSetupEntry(name: $"minutesStartBreak").Value == null ? "0" : da.LoadSetupEntry(name: $"minutesStartBreak").Value);
                    break;
                case "EndBreak":
                    minutes = int.Parse(da.LoadSetupEntry(name: $"minutesEndBreak").Value == null ? "0" : da.LoadSetupEntry(name: $"minutesEndBreak").Value);
                    break;
                default:
                    minutes = int.Parse("0");
                    break;
            }

            return minutes;
        }

        static void mnuExit_Click(object sender, EventArgs e)
        {
            NotIcon.Dispose();
            Application.Exit();
        }

        public static string SetStatus()
        {

            SqlLiteDataAcces da = new SqlLiteDataAcces();
            string stat = "Logger";
            if (da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).Count() > 0)
            {
                WorkingDayModel workingDayModel = new WorkingDayModel();
                workingDayModel = da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).First();
                int breakMinutes = 0;

                if (workingDayModel.WorkTimeStart != "")
                {
                    List<WorkingDayBreaksModel> workingDayBreaks = new List<WorkingDayBreaksModel>();
                    workingDayBreaks.AddRange(da.LoadWorkingDayBreaks(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false));

                    if (workingDayBreaks.Count() > 0)
                    {
                        foreach (WorkingDayBreaksModel dayBreaksModel in workingDayBreaks)
                        {
                            if (dayBreaksModel.BreakTimeStart != "" && dayBreaksModel.BreakTimeEnd != "")
                            {
                                if (LibDateTime.MinutesBetweenTime($"{DateTime.Now.Hour}:{DateTime.Now.Minute}", dayBreaksModel.BreakTimeEnd) > 0)
                                {
                                    breakMinutes = breakMinutes + LibDateTime.MinutesBetweenTime(dayBreaksModel.BreakTimeEnd, dayBreaksModel.BreakTimeStart);

                                }
                                else
                                    breakMinutes = breakMinutes + LibDateTime.MinutesBetweenTime($"{DateTime.Now.Hour}:{DateTime.Now.Minute}", dayBreaksModel.BreakTimeStart);
                            }
                        }
                    }
                }

                if (workingDayModel.WorkTimeStart != "" && workingDayModel.WorkTimeEnd != "")
                {
                    if (LibDateTime.MinutesBetweenTime($"{DateTime.Now.Hour}:{DateTime.Now.Minute}", workingDayModel.WorkTimeEnd) > 0)
                    {
                        stat = $"Rozpoczęto pracę: {workingDayModel.WorkTimeStart}, zakończono {workingDayModel.WorkTimeEnd} w pracy {LibDateTime.HoursBetweenTime(LibDateTime.HoursAddMinutes(workingDayModel.WorkTimeEnd, -breakMinutes), workingDayModel.WorkTimeStart)} godzin";
                    }
                    else
                    {
                        stat = $"Rozpoczęto pracę: {workingDayModel.WorkTimeStart}, zakończono {DateTime.Now.Hour}:{DateTime.Now.Minute} w pracy {LibDateTime.HoursBetweenTime(LibDateTime.HoursAddMinutes($"{DateTime.Now.Hour}:{DateTime.Now.Minute}", -breakMinutes), workingDayModel.WorkTimeStart)} godzin";
                    }
                }
                else if (workingDayModel.WorkTimeStart != "")
                {
                    stat = $"Rozpoczęto pracę: {workingDayModel.WorkTimeStart}, w pracy {LibDateTime.HoursBetweenTime(LibDateTime.HoursAddMinutes($"{DateTime.Now.Hour}:{DateTime.Now.Minute}", -breakMinutes), workingDayModel.WorkTimeStart)} godzin";
                }
            }
            return stat;
        }
        private static void SetGetStatus(object sender, ElapsedEventArgs e)
        {
         
            
            NotIcon.Text = SetStatus();
            //return Task.CompletedTask;
        }

        public static bool BalonMessage(NotifyIcon notIcon, string titleBalloon, string textBalloon, string typeLog="")
        {
            NotIcon.BalloonTipText = textBalloon;
            NotIcon.BalloonTipTitle = titleBalloon;
            NotIcon.BalloonTipIcon = ToolTipIcon.Info;

            NotIcon.ShowBalloonTip(1500);
            if (typeLog!="")
            {
                new LogEntryModel() { Id = 0, Type = typeLog, DateEvent = DateTime.Now }.SqlInsert();
            }

            return true;
        }
        private static NotifyIcon TrayIcon;
        private static ContextMenuStrip TrayIconContextMenu;
        private static ToolStripMenuItem CloseMenuItem;
        public static void ShowTrayIcon()
        {
            
            TrayIcon = new NotifyIcon();

            TrayIconContextMenu = new ContextMenuStrip();
            CloseMenuItem = new ToolStripMenuItem();
            TrayIconContextMenu.SuspendLayout();

            // 
            // TrayIconContextMenu
            // 
            TrayIconContextMenu.Items.AddRange(new ToolStripItem[] {
            CloseMenuItem});
            TrayIconContextMenu.Name = "TrayIconContextMenu";
            //TrayIconContextMenu.Size = new Size(153, 70);
            // 
            // CloseMenuItem
            // 
            CloseMenuItem.Name = "CloseMenuItem";
           // CloseMenuItem.Size = new Size(152, 22);
            CloseMenuItem.Text = "Close the tray icon program";
            CloseMenuItem.Click += new EventHandler(CloseMenuItem_Click);

            TrayIconContextMenu.ResumeLayout(false);

            //NotIcon.ContextMenuStrip = TrayIconContextMenu;

            ContextMenu contextMenu = new ContextMenu();
            
            MenuItem menuItemCloseWork = new MenuItem("Zamknij");
            menuItemCloseWork.Click += new EventHandler(menuItemClose_MouseClick);
            contextMenu.MenuItems.Add(menuItemCloseWork);

            MenuItem menuItemEndWork = new MenuItem("Zakończ pracę");
            menuItemEndWork.Click += new EventHandler(menuItemEndWork_MouseClick);
            contextMenu.MenuItems.Add(menuItemEndWork);

            MenuItem menuItemStartWork = new MenuItem("Rozpocznij prace");
            menuItemStartWork.Click += new EventHandler( menuItemStartWork_MouseClick);
            contextMenu.MenuItems.Add(menuItemStartWork);

            NotIcon.ContextMenu = contextMenu;

            NotIcon.Icon = new System.Drawing.Icon("prog.ico");
           // NotIcon.ContextMenuStrip = menu;
            NotIcon.Text = "Logger";
            
            //NotIcon.Click += new EventHandler(mnuExit_Click);
            //NotIcon.




            NotIcon.Visible = true;
            
        }

        private static void menuItemStartWork_MouseClick(object sender, EventArgs e)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            int minutes = GetMinutesFromDataBase("StartWork");

            WorkingDayModel workingDay = new WorkingDayModel();
            DialogResult dialogResult = new DialogResult();
            if (da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).Count() > 0)
            {
                workingDay = da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).First();
                if (workingDay.WorkTimeStart != "")
                {
                    dialogResult = MessageBox.Show($"Zarejestrowana godzina pracy jest na {workingDay.WorkTimeStart}, \nnadpisać na {LibDateTime.HoursAddMinutes($"{DateTime.Now.Hour}:{DateTime.Now.Minute}", minutes)}", "Czy nadpisać czas pracy?", MessageBoxButtons.YesNo);
                }
            }
            if (((da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).Count() > 0) ? (da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).First().WorkTimeStart == "") : false) || (dialogResult == DialogResult.Yes) || (da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).Count() == 0))
            {
                EventStartWork(minutes);
                BalonMessage(NotIcon, "Rejestr zdarzenia", $"Rozpoczęto pracę: {DateTime.Now.AddMinutes(minutes)}", "WORK START");
            }
            NotIcon.Text = SetStatus();
        }
        private static void menuItemEndWork_MouseClick(object sender, EventArgs e)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            int minutes = GetMinutesFromDataBase("EndWork");

            WorkingDayModel workingDay = new WorkingDayModel();
            DialogResult dialogResult = new DialogResult();
            if (da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).Count() > 0)
            {
                workingDay = da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).First();
                if (workingDay.WorkTimeEnd != "")
                {
                    dialogResult = MessageBox.Show($"Godzina zakończenia pracy wprowadzona już na {workingDay.WorkTimeEnd}, \nnadpisać na {LibDateTime.HoursAddMinutes($"{DateTime.Now.Hour}:{DateTime.Now.Minute}", minutes)}", "Czy nadpisać czas pracy?", MessageBoxButtons.YesNo);
                }
            }
            int sasf = da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).Count;
            if (((da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).Count() > 0) ? (da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).First().WorkTimeEnd == "") : false) || (dialogResult==DialogResult.Yes) || (da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).Count() == 0))
            {

                EventEndWork(minutes);
                BalonMessage(NotIcon, "Rejestr zdarzenia", $"Zakończono pracę: {DateTime.Now.AddMinutes(minutes)}", "WORK END");
            }
            NotIcon.Text = SetStatus();
        }
        private static void menuItemClose_MouseClick(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            //Here, you can do stuff if the tray icon is doubleclicked
            TrayIcon.ShowBalloonTip(10000);
        }

        private static void CloseMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to close me?",
                    "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }


        public static async Task PeriodicAsync(Func<Task> action, TimeSpan interval,
    CancellationToken cancellationToken = default)
        {
            while (true)
            {
                var delayTask = Task.Delay(interval, cancellationToken);
                await action();
                await delayTask;
            }
        }

        /*
        public async Task StartTimer(NotifyIcon notify, CancellationToken cancellationToken)
        {

            await Task.Run(async () =>
            {
                while (true)
                {
                    SetGetStatus();
                    await Task.Delay(10000, cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                        break;
                }
            });

        }
        */





        public static void EventStartWork (int addMinutes = 0)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            if(da.LoadWorkingDay(year:DateTime.Now.Year, month:DateTime.Now.Month, day: DateTime.Now.Day, planing: false).Count>0){
                
                WorkingDayModel workingDay = new WorkingDayModel();
                workingDay = da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).First();
                new WorkingDayModel() { Id = workingDay.Id, Comment = workingDay.Comment, Day = workingDay.Day, Month = workingDay.Month, Planing = false, WorkTimeStart = LibDateTime.HoursAddMinutes( $"{DateTime.Now.Hour:d2}:{DateTime.Now.Minute}",addMinutes), WorkTimeEnd=workingDay.WorkTimeEnd, Year = workingDay.Year }.SqlUpdate();
            }
            else { 
            new WorkingDayModel() { Id = 0, Comment = "", Day = DateTime.Now.Day, Month = DateTime.Now.Month, Planing = false, WorkTimeStart = LibDateTime.HoursAddMinutes($"{DateTime.Now.Hour:d2}:{DateTime.Now.Minute}", addMinutes), WorkTimeEnd = "", Year = DateTime.Now.Year }.SqlInsert();
            }
        }

        public static void EventStartBreak( int addMinutes=0)
        {

                new WorkingDayBreaksModel() { Id = 0, Comment = "", Day = DateTime.Now.Day, Month = DateTime.Now.Month, Planing = false, BreakTimeStart = LibDateTime.HoursAddMinutes($"{DateTime.Now.Hour:d2}:{DateTime.Now.Minute}", addMinutes), BreakTimeEnd = "", Year = DateTime.Now.Year }.SqlInsert();
        }

        public static void EventEndBreak(int addMinutes=0)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            
            if (da.LoadWorkingDayBreaks(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).Count > 0)
            {
                List<WorkingDayBreaksModel> workingBreakDays = new List<WorkingDayBreaksModel>();
                workingBreakDays = da.LoadWorkingDayBreaks(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false);
                
                foreach (WorkingDayBreaksModel workingBreakDay in workingBreakDays) {

                    // TODO - find last started break and update end of break
                    //new WorkingDayBreaksModel() { Id = workingBreakDay.Id, Comment = workingBreakDay.Comment, Day = workingBreakDay.Day, Month = workingBreakDay.Month, Planing = false, BreakTimeStart = LibDateTime.HoursAddMinutes($"{DateTime.Now.Hour:d2}-{DateTime.Now.Minute}", addMinutes), BreakTimeEnd = workingBreakDay.BreakTimeEnd, Year = workingBreakDay.Year }.SqlUpdate();
                }


            }

        }
        public static void EventEndWork(int addMinutes=0)
        {
            SqlLiteDataAcces da = new SqlLiteDataAcces();
            if (da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).Count > 0)
            {
                WorkingDayModel workingDay = new WorkingDayModel();
                workingDay = da.LoadWorkingDay(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, planing: false).First();
                new WorkingDayModel() { Id = workingDay.Id, Comment = workingDay.Comment, Day = workingDay.Day, Month = workingDay.Month, Planing = false, WorkTimeStart =workingDay.WorkTimeStart , WorkTimeEnd = LibDateTime.HoursAddMinutes($"{DateTime.Now.Hour:d2}:{DateTime.Now.Minute}", addMinutes), Year = workingDay.Year }.SqlUpdate();
            }
            else
            {
                new WorkingDayModel() { Id = 0, Comment = "", Day = DateTime.Now.Day, Month = DateTime.Now.Month, Planing = false, WorkTimeStart = "", WorkTimeEnd = LibDateTime.HoursAddMinutes($"{DateTime.Now.Hour:d2}:{DateTime.Now.Minute}", addMinutes), Year = DateTime.Now.Year }.SqlInsert();
            }
        }
        /*
private void NotIcon_Click(object sender, System.EventArgs e)
{
   System.Drawing.Size windowSize =
       SystemInformation.PrimaryMonitorMaximizedWindowSize;
   System.Drawing.Point menuPoint =
       new System.Drawing.Point(windowSize.Width - 180,
       windowSize.Height - 5);
   menuPoint = this.PointToClient(menuPoint);

   NotIcon.ContextMenu.Show(this, menuPoint);
}
*/
    }
}



/*
 * event into Tray Icon
 * 
 using System;
using System.Drawing;
using System.Windows.Forms;

public class Form1 : System.Windows.Forms.Form
{
    private System.Windows.Forms.NotifyIcon notifyIcon1;
    private System.Windows.Forms.ContextMenu contextMenu1;
    private System.Windows.Forms.MenuItem menuItem1;
    private System.ComponentModel.IContainer components;

    [STAThread]
    static void Main() 
    {
        Application.Run(new Form1());
    }

    public Form1()
    {
        this.components = new System.ComponentModel.Container();
        this.contextMenu1 = new System.Windows.Forms.ContextMenu();
        this.menuItem1 = new System.Windows.Forms.MenuItem();

        // Initialize contextMenu1
        this.contextMenu1.MenuItems.AddRange(
                    new System.Windows.Forms.MenuItem[] {this.menuItem1});

        // Initialize menuItem1
        this.menuItem1.Index = 0;
        this.menuItem1.Text = "E&xit";
        this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);

        // Set up how the form should be displayed.
        this.ClientSize = new System.Drawing.Size(292, 266);
        this.Text = "Notify Icon Example";

        // Create the NotifyIcon.
        this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);

        // The Icon property sets the icon that will appear
        // in the systray for this application.
        notifyIcon1.Icon = new Icon("appicon.ico");

        // The ContextMenu property sets the menu that will
        // appear when the systray icon is right clicked.
        notifyIcon1.ContextMenu = this.contextMenu1;

        // The Text property sets the text that will be displayed,
        // in a tooltip, when the mouse hovers over the systray icon.
        notifyIcon1.Text = "Form1 (NotifyIcon example)";
        notifyIcon1.Visible = true;

        // Handle the DoubleClick event to activate the form.
        notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
    }

    protected override void Dispose( bool disposing )
    {
        // Clean up any components being used.
        if( disposing )
            if (components != null)
                components.Dispose();            

        base.Dispose( disposing );
    }

    private void notifyIcon1_DoubleClick(object Sender, EventArgs e) 
    {
        // Show the form when the user double clicks on the notify icon.

        // Set the WindowState to normal if the form is minimized.
        if (this.WindowState == FormWindowState.Minimized)
            this.WindowState = FormWindowState.Normal;

        // Activate the form.
        this.Activate();
    }

    private void menuItem1_Click(object Sender, EventArgs e) {
        // Close the form, which closes the application.
        this.Close();
    }
}
*/