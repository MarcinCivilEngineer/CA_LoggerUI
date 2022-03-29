using Caliburn.Micro;
using WPF_LoggerTray.ViewModels;
using WPF_LoggerTray.Views;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;


namespace WPF_LoggerTray.ViewModels
{

    public class ShellViewModel :Conductor<object>
    {

  

        public ShellViewModel()
        {
            //this.windowManager = theWindowManager;
            SqlLiteDataAcces da = new SqlLiteDataAcces();




            LoadPageRaport();


            //Projekty.Add(new ProjektModel { Id = 1, NumerProjektu = "BET021_21", NazwaProjektu = "Nivea", OpisProjektu = "Takie tam" });
            //Projekty.Add(new ProjektModel { Id = 2, NumerProjektu = "BET024_21", NazwaProjektu = "Lidl", OpisProjektu = "Takie tam mniejsze" });




        }

        public void LoadPageRaport()
        {
            ActivateItemAsync(new RaportViewModel());
        }
        

            public void LoadPageSetup()
        {
            ActivateItemAsync(new SetupViewModel());
        }
        /*
        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);

            ShellView shellView = (ShellView)view;
            shellView.CommandBindings.Add(new CommandBinding( , SaveAsCommandHandler));
        }
        */


    }

}
