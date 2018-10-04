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
using System.Data.SqlClient;
using System.Data;
using Microsoft.Reporting.WinForms;

namespace Nube.Reports
{
	/// <summary>
	/// Interaction logic for TakafulReport.xaml
	/// </summary>
	public partial class TakafulReport : MetroWindow
	{
		nubebfsEntity db = new nubebfsEntity();
		public TakafulReport()
		{
			InitializeComponent();

			try
			{
				var NUBE = db.MASTERNUBEBRANCHes.OrderBy(x => x.NUBE_BRANCH_NAME).ToList();
				cmbNubeBranch.ItemsSource = NUBE;
				cmbNubeBranch.SelectedValuePath = "NUBE_BRANCH_CODE";
				cmbNubeBranch.DisplayMemberPath = "NUBE_BRANCH_NAME";

				var bank = db.MASTERBANKs.OrderBy(x => x.BANK_NAME).ToList();
				cmbBank.ItemsSource = bank;
				cmbBank.SelectedValuePath = "BANK_CODE";
				cmbBank.DisplayMemberPath = "BANK_NAME";
			}
			catch (Exception ex)
			{
				ExceptionLogging.SendErrorToText(ex);
			}
		}

		#region "BUTTON EVENTS"

		private void btnSearch_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (!string.IsNullOrEmpty(dtpDate.Text.ToString()))
				{
					if (dtpDate.SelectedDate <= Convert.ToDateTime("31/MAY/2017").Date)
					{
						MessageBox.Show("Please Select the Insurance Valid Date!");
						dtpDate.Focus();
					}
					else
					{
						FormLoad();
					}
				}
				else
				{
					MessageBox.Show("Please Select the Date!");
					dtpDate.Focus();
				}
			}
			catch (Exception ex)
			{
				ExceptionLogging.SendErrorToText(ex);
			}

		}

		private void btnClear_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				dtpDate.Text = "";
				cmbBank.Text = "";
				cmbBranch.Text = "";
				cmbNubeBranch.Text = "";
			}
			catch (Exception ex)
			{
				ExceptionLogging.SendErrorToText(ex);
			}

		}

		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				frmHomeReports frm = new frmHomeReports();
				this.Close();
				frm.ShowDialog();
			}
			catch (Exception ex)
			{
				ExceptionLogging.SendErrorToText(ex);
			}
		}

		private void cmbBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				Decimal d = Convert.ToDecimal(cmbBank.SelectedValue);
				var branch = db.MASTERBANKBRANCHes.Where(x => x.BANK_CODE == d).OrderBy(x => x.BANKBRANCH_NAME).ToList();
				cmbBranch.ItemsSource = branch;
				cmbBranch.SelectedValuePath = "BANKBRANCH_CODE";
				cmbBranch.DisplayMemberPath = "BANKBRANCH_NAME";
			}
			catch (Exception ex)
			{
				Nube.ExceptionLogging.SendErrorToText(ex);
			}
		}

		#endregion

		#region "USER DEFINED FUNCTION"

		void FormLoad()
		{
			string sWhere = "";
			string sWhere1 = "";
			string sWhere2 = "";

			if (!string.IsNullOrEmpty(dtpDate.Text.ToString()))
			{
				sWhere = string.Format(" WHERE FD.FEEYEAR={0} AND FD.FEEMONTH={1}  ", Convert.ToDateTime(dtpDate.SelectedDate).Year, Convert.ToDateTime(dtpDate.SelectedDate).Month);
			}

			sWhere = sWhere + " AND ISNOTMATCH=0 AND ISUNPAID=0 AND FD.STATUS='FEES ENTRY' ";

			if (!string.IsNullOrEmpty(cmbNubeBranch.Text))
			{
				if (!string.IsNullOrEmpty(sWhere))
				{
					sWhere = sWhere + " AND MB.NUBE_BRANCH_CODE=" + cmbNubeBranch.SelectedValue;
				}
			}


			decimal dBankCode = 0;
			if (!string.IsNullOrEmpty(cmbBank.Text))
			{
				dBankCode = Convert.ToDecimal(cmbBank.SelectedValue);
			}

			if (!string.IsNullOrEmpty(cmbBank.Text))
			{
				sWhere = sWhere + " AND FD.BANK_CODE=" + dBankCode;
			}

			if (!string.IsNullOrEmpty(cmbBranch.Text))
			{
				sWhere = sWhere + " AND MM.BRANCH_CODE=" + cmbBranch.SelectedValue;
			}

			DataTable dt = new DataTable();
			using (SqlConnection conn = new SqlConnection(AppLib.connStr))
			{
				string str = "";



				str = " SELECT ROW_NUMBER() OVER(ORDER BY MEMBER_NAME ASC) AS RNO,ISNULL(MM.MEMBER_ID, 0)MEMBERID, \r" +
				 " ISNULL(MM.MEMBER_NAME, '')MEMBER_NAME, CASE WHEN ISNULL(MM.ICNO_NEW, '') <> '' THEN MM.ICNO_NEW ELSE MM.ICNO_OLD END NRIC, \r" +
				 " FD.AMOUNTINS \r" +
				 " FROM FEESDETAILS FD(NOLOCK) \r" +
				 " LEFT JOIN FEESMASTER FM(NOLOCK) ON FM.FEEID = FD.FEEID \r" +
				 " LEFT JOIN MASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE = FD.MEMBERCODE \r" +
				 " LEFT JOIN MASTERBANKBRANCH MB(NOLOCK) ON MB.BANKBRANCH_CODE = MM.BRANCH_CODE \r" +
				 sWhere + sWhere2 + "\r";

				string sSummary = string.Format(" SELECT FM.FEEID,MB.BANK_USERCODE BANK,COUNT(*) TOTALMEMBERS,SUM(FD.AMOUNTINS)AMOUNT \r" +								
				 " FROM FEESDETAILS FD(NOLOCK) \r" +
				 " LEFT JOIN FEESMASTER FM(NOLOCK) ON FM.FEEID = FD.FEEID \r" +
				 " LEFT JOIN MASTERBANK MB(NOLOCK) ON MB.BANK_CODE=FM.BANKID     \r" +
				 " WHERE FD.FEEYEAR={0} AND FD.FEEMONTH={1} AND ISNOTMATCH=0 AND ISUNPAID=0 AND FD.STATUS='FEES ENTRY'  \r  GROUP BY FM.FEEID,MB.BANK_USERCODE", Convert.ToDateTime(dtpDate.SelectedDate).Year, Convert.ToDateTime(dtpDate.SelectedDate).Month);


				string sNewMEmber = string.Format(" SELECT ROW_NUMBER() OVER(ORDER BY MEMBER_NAME ASC) AS RNO,MB.BANK_USERCODE BANK,ISNULL(MM.MEMBER_ID, 0)MSHIP,  \r" +
				 " ISNULL(MM.MEMBER_NAME, '')NAME, CASE WHEN ISNULL(MM.ICNO_NEW, '') <> '' THEN MM.ICNO_NEW ELSE MM.ICNO_OLD END NRIC,10 AMOUNT \r" +
				 " FROM MASTERMEMBER MM (NOLOCK) \r" +
				 " LEFT JOIN MASTERBANK MB(NOLOCK) ON MB.BANK_CODE=MM.BANK_CODE \r" +
				 " WHERE YEAR(MM.DATEOFJOINING)={0} AND MONTH(MM.DATEOFJOINING)={1} ", Convert.ToDateTime(dtpDate.SelectedDate).Year, Convert.ToDateTime(dtpDate.SelectedDate).Month);


				SqlCommand cmd = new SqlCommand(str, conn);
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.SelectCommand.CommandTimeout = 0;
				da.Fill(dt);

				cmd = new SqlCommand(sSummary, conn);
				da = new SqlDataAdapter(cmd);
				da.SelectCommand.CommandTimeout = 0;
				DataTable dtSummary = new DataTable();
				da.Fill(dtSummary);

				cmd = new SqlCommand(sNewMEmber, conn);
				da = new SqlDataAdapter(cmd);
				da.SelectCommand.CommandTimeout = 0;
				DataTable dtNewMember = new DataTable();
				da.Fill(dtNewMember);

				// MEMBERSHIP REPORT
				if (dt.Rows.Count > 0)
				{
					MemberReport.Reset();
					ReportDataSource masterData = new ReportDataSource("TakafulMemberReport", dt);
					MemberReport.LocalReport.DataSources.Add(masterData);
					MemberReport.LocalReport.ReportEmbeddedResource = "Nube.Reports.TakafulMemberReport.rdlc";
					ReportParameter[] NB = new ReportParameter[3];
					NB[0] = new ReportParameter("BANK", cmbBank.Text);
					NB[1] = new ReportParameter("MONTH", string.Format("{0:MMM}", dtpDate.SelectedDate));
					NB[2] = new ReportParameter("YEAR", Convert.ToDateTime(dtpDate.SelectedDate).Year.ToString());
					MemberReport.LocalReport.SetParameters(NB);
					MemberReport.RefreshReport();
				}
				else
				{
					MessageBox.Show("No Data Foud!");
				}
				if (dtNewMember.Rows.Count > 0)
				{
					NewMemberReport.Reset();
					ReportDataSource masterDataNewMember = new ReportDataSource("TakafulNewMember", dtNewMember);
					NewMemberReport.LocalReport.DataSources.Add(masterDataNewMember);
					NewMemberReport.LocalReport.ReportEmbeddedResource = "Nube.Reports.TakafulNewMember.rdlc";
					ReportParameter[] NBNew = new ReportParameter[2];
					NBNew[0] = new ReportParameter("MONTH", string.Format("{0:MMM}", dtpDate.SelectedDate));
					NBNew[1] = new ReportParameter("YEAR", Convert.ToDateTime(dtpDate.SelectedDate).Year.ToString());
					NewMemberReport.LocalReport.SetParameters(NBNew);
					NewMemberReport.RefreshReport();
				}
				else
				{
					MessageBox.Show("New Member Nota Foud!");
				}

				if (dtSummary.Rows.Count > 0)
				{
					SummaryReport.Reset();
					ReportDataSource masterDataSummary = new ReportDataSource("TAKAFULSUMMARY", dtSummary);
					SummaryReport.LocalReport.DataSources.Add(masterDataSummary);
					SummaryReport.LocalReport.ReportEmbeddedResource = "Nube.Reports.TakafulSummary.rdlc";
					ReportParameter[] NBs = new ReportParameter[2];
					NBs[0] = new ReportParameter("MONTH", string.Format("{0:MMM}", dtpDate.SelectedDate));
					NBs[1] = new ReportParameter("YEAR", Convert.ToDateTime(dtpDate.SelectedDate).Year.ToString());
					SummaryReport.LocalReport.SetParameters(NBs);
					SummaryReport.RefreshReport();
				}
				else
				{
					MessageBox.Show("Summary No Data Foud!");
				}

			}
		}

		#endregion
	}
}
