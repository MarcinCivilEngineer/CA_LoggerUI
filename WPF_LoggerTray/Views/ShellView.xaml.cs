using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF_LoggerTray.Views
{
    /// <summary>
    /// Logika interakcji dla klasy ShellView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            
            InitializeComponent();
        }

        private void ActiveItem_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }


        /*
        private void Projekty_selected(object sender, RoutedEventArgs e)
        {
            string aa = "a";
        }
        */

    }

}
