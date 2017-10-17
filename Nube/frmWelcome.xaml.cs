using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Nube
{
    /// <summary>
    /// Interaction logic for frmWelcome.xaml
    /// </summary>
    public partial class frmWelcome : Window
    {
        public frmWelcome()
        {
            InitializeComponent();
            lblUpdated.Content = string.Format("UPDATED ON - 17-OCT-2017");    
        }

        private void image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            frmLogin frm = new frmLogin();
            frm.ShowDialog();
            //this.Close();
        }

        private void image_MouseEnter(object sender, MouseEventArgs e)
        {
            //frmLogin frm = new frmLogin();
            //frm.ShowDialog();
            ////this.Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure to exit?", "Exit Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                e.Cancel = true;
            }           
        }
    }
}
