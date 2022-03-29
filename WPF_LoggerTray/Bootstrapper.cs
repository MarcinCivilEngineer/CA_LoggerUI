using Caliburn.Micro;
using System.Windows;
using WPF_LoggerTray.ViewModels;

namespace WPF_LoggerTray
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
