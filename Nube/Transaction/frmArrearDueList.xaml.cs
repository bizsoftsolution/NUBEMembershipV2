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
using System.Reflection;
using Nube.MasterSetup;

namespace Nube.Transaction
{
    /// <summary>
    /// Interaction logic for frmArrearDueList.xaml
    /// </summary>
    public partial class frmArrearDueList : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        decimal dMember_Code = 0;
        string sFormName = "";
        DataTable dtMember = new DataTable();

        public frmArrearDueList(decimal dMembercode = 0, string sForm = "", string sMemberType = "", string sMemberName = "", string stxtMemberID = "")
        {
            InitializeComponent();
            dMember_Code = dMembercode;
            sFormName = sForm;
            txtMemberType.Text = sMemberType;
            txtMemberName.Text = sMemberName;
            txtMemberNo.Text = stxtMemberID;

            if (dMember_Code != 0)
            {
                FormFill();
            }
        }

        #region "CLICK EVENTS"

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            if (sFormName == "PreArrear")
            {
                frmArrearPre16 frm = new frmArrearPre16(dMember_Code, 0);
                this.Close();
                frm.ShowDialog();
            }
            else if (sFormName == "PostArrear")
            {
                frmArrearPost16 frm = new frmArrearPost16(dMember_Code, 0);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void dgArrearMater_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dgArrearMater.SelectedItem != null)
                {
                    DataRowView drv = (DataRowView)dgArrearMater.SelectedItem;
                    int iID = Convert.ToInt32(drv["ID"]);
                    if (sFormName == "PreArrear")
                    {
                        frmArrearPre16 frm = new frmArrearPre16(dMember_Code, iID);
                        this.Close();
                        frm.ShowDialog();
                    }
                    else if (sFormName == "PostArrear")
                    {
                        frmArrearPost16 frm = new frmArrearPost16(dMember_Code, iID);
                        this.Close();
                        frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void dgArrearMater_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dgArrearMater.SelectedItem != null)
                {
                    if (sFormName == "PreArrear")
                    {
                        DataRowView drv = (DataRowView)dgArrearMater.SelectedItem;
                        int iID = Convert.ToInt32(drv["ID"]);

                        if (iID != 0)
                        {
                            var qry = db.ArrearPreDetails.Where(x => x.MasterId == iID).ToList();
                            dgArrearPreDtl.ItemsSource = qry;
                        }
                    }
                    else if (sFormName == "PostArrear")
                    {
                        DataRowView drv = (DataRowView)dgArrearMater.SelectedItem;
                        int iID = Convert.ToInt32(drv["ID"]);

                        if (iID != 0)
                        {
                            var qry = db.ArrearPostDetails.Where(x => x.MasterId == iID).ToList();
                            dgArrearPostDtl.ItemsSource = qry;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #endregion

        #region "USER DEFINED FUNCTIONS"

        private void FormFill()
        {
            try
            {
                if (sFormName == "PreArrear")
                {
                    var qry = (from am in db.ArrearPreMasters
                               where am.MemberCode == dMember_Code
                               select new
                               {
                                   am.ID,
                                   EntryDate = am.EntryDate,
                                   am.BeforeLastPaymentDate,
                                   am.BeforeTotalMonthsPaid
                               }).ToList();
                    dtMember = AppLib.LINQResultToDataTable(qry);
                    dgArrearMater.ItemsSource = dtMember.DefaultView;
                    dgArrearPostDtl.Visibility = Visibility.Collapsed;
                }
                else if (sFormName == "PostArrear")
                {
                    var qry = (from am in db.ArrearPostMasters
                               where am.MemberCode == dMember_Code
                               select new
                               {
                                   am.ID,
                                   EntryDate = am.EntryDate,
                                   am.BeforeLastPaymentDate,
                                   am.BeforeTotalMonthsPaid
                               }).ToList();
                    dtMember = AppLib.LINQResultToDataTable(qry);
                    dgArrearMater.ItemsSource = dtMember.DefaultView;
                    dgArrearPreDtl.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }   

        #endregion
    }
}
