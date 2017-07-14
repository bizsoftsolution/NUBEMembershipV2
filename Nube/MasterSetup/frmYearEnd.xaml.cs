using MahApps.Metro.Controls;
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
using System.Data;
using System.Data.SqlClient;

namespace Nube.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmYearEnd.xaml
    /// </summary>
    public partial class frmYearEnd : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        string connStatus = AppLib.connstatus;
        string conn = AppLib.connStr;

        public frmYearEnd()
        {
            InitializeComponent();
            this.KeyDown += new System.Windows.Input.KeyEventHandler(Window_KeyDown);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                this.Close();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure to exit?", "Exit Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            frmHomeMaster frm = new frmHomeMaster();
            this.Close();
            frm.ShowDialog();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Do You want to save this Record?", "YearEnd Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {

                    DateTime dtime = Convert.ToDateTime(dtpDate.SelectedDate);
                    dtime = new DateTime(dtime.Year, 12, 31);
                    progressBar1.Minimum = 1;
                    progressBar1.Maximum = 12;
                    progressBar1.Visibility = Visibility.Visible;

                    //using (SqlConnection con = new SqlConnection(conn))
                    //{
                    for (int i = 1; i <= 12; i++)
                    {
                        //        progressBar1.Value = i;
                        //        System.Windows.Forms.Application.DoEvents();
                        //       SqlCommand cmd = new SqlCommand("SPMEMBERSHIP", con);
                        //    }
                    }
                    MessageBox.Show("Already Year End Closed!","Sorry");
                }
            }

            catch (Exception ex)
            {

                ExceptionLogging.SendErrorToText(ex);
            }
        }
    }
}
