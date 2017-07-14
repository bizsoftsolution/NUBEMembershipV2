using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace Nube.Reports
{
    /// <summary>
    /// Interaction logic for frmFinancialHalfShareReport.xaml
    /// </summary>
    public partial class frmFinancialHalfShareReport : MetroWindow
    {
        DataTable dt = new DataTable();       
        public frmFinancialHalfShareReport()
        {
            InitializeComponent();
        }

        #region Button Events

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(dtpDate.Text))
                {
                    MessageBox.Show("Date is Empty!");
                    dtpDate.Focus();
                }
                else
                {
                    if (dt.Rows.Count > 0)
                    {
                        dgvHalfShare.ItemsSource = dt.DefaultView;
                    }
                    else
                    {
                        progressBar1.Minimum = 1;
                        progressBar1.Maximum = 10;
                        progressBar1.Visibility = Visibility.Visible;
                        DateTime date = dtpDate.SelectedDate.Value;
                        using (SqlConnection con = new SqlConnection(AppLib.connStr))
                        {
                            SqlCommand cmd;
                            string FEEDATE = string.Format("{0:ddMMMyyyy}", dtpDate.SelectedDate.Value);
                            progressBar1.Value = 5;
                            System.Windows.Forms.Application.DoEvents();
                            cmd = new SqlCommand("SPHALFSHARE", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@FEEDATE", FEEDATE));
                            SqlDataAdapter adp = new SqlDataAdapter(cmd);
                            progressBar1.Value = 7;
                            System.Windows.Forms.Application.DoEvents();
                            adp.SelectCommand.CommandTimeout = 0;
                            adp.Fill(dt);
                            dgvHalfShare.ItemsSource = dt.DefaultView;
                            progressBar1.Value = 10;
                            System.Windows.Forms.Application.DoEvents();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dt.Rows.Count > 0)
                {                  
                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.InitialDirectory = Environment.CurrentDirectory;
                    dlg.Title = "Half Share";
                    dlg.DefaultExt = ".xlsx";
                    dlg.Filter = "XL files|*.xls;*.xlsx|All files|*.*";
                    dlg.FileName = "HalfShare";
                    if (dlg.ShowDialog() == true)
                    {                        
                        Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
                        Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
                        Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
                        app.Visible = true;
                        worksheet = workbook.Sheets["Sheet1"];

                        worksheet = workbook.ActiveSheet;
                        worksheet.Name = "Half Share Report";

                        worksheet.Cells[3, 4] = "HALF SHARE REPORT ";
                        worksheet.Cells[3, 4].Font.Bold = true;
                        for (int i = 1; i < dt.Columns.Count + 1; i++)
                        {
                            worksheet.Cells[5, i + 2] = dt.Columns[i - 1].ColumnName;
                            worksheet.Cells[5, i + 2].Font.Bold = true;
                        }

                        for (int i = 0; i < dt.Rows.Count - 1; i++)
                        {
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                worksheet.Cells[i + 6, j + 3] = dt.Rows[i][j].ToString();
                            }
                        }

                        workbook.SaveAs(dlg.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                        app.Quit();
                        MessageBox.Show("Exported Sucessfully !", "Sucess");
                    }                   
                }
                else
                {
                    MessageBox.Show("No Data Found !", "Empty");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            dtpDate.Text = "";          
            dt.Rows.Clear();
            dgvHalfShare.ItemsSource = null;
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            frmHomeReports frm = new frmHomeReports();
            this.Close();
            frm.ShowDialog();
        }      

        #endregion

    }
}
