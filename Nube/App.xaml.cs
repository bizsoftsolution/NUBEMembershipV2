using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Nube
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static frmMain frmMain;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            frmWelcome frm = new frmWelcome();
            frm.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
    }
}
