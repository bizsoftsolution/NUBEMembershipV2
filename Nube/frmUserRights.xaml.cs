using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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

namespace Nube
{
    /// <summary>
    /// Interaction logic for frmUserRights.xaml
    /// </summary>
    public partial class frmUserRights : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        int ID = 0;
        string sFormName = "";
        frmHomeMaster frmMaster = new frmHomeMaster();
        frmHomeMembership frmTran = new frmHomeMembership();
        frmHomeReports frmRpt = new frmHomeReports();
        frmMain frmMain = new frmMain();
        frmUserPrevilage frmUsrPLG = new frmUserPrevilage();
        //Accounts.frmHomeAccounts frmHAcc = new Accounts.frmHomeAccounts();
        //Accounts.frmHomeAccReports frmHRpt = new Accounts.frmHomeAccReports();
        //Accounts.frmAccountMaster frmHMst = new Accounts.frmAccountMaster();
        List<UserPrevilage> lstUserPrevilage = new List<UserPrevilage>();

        public frmUserRights(int Id = 0, string Form = "")
        {
            InitializeComponent();
            try
            {
                var coun = db.UserTypes.ToList();
                cmbUserType.ItemsSource = coun;
                cmbUserType.DisplayMemberPath = "UserType1";
                cmbUserType.SelectedValuePath = "Id";
                ID = Id;
                sFormName = Form;
                if (ID != 0)
                {
                    cmbUserType.SelectedValue = ID;
                    cmbUserType.IsReadOnly = true;
                }
                LoadWindow();
                AppLib.sProjectName = frmMain.btnUser.Tag.ToString();
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        #region "BUTTON EVENTS"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (sFormName == "HOME")
            {
                frmUserPrevilage frm = new frmUserPrevilage();
                this.Close();
                frm.ShowDialog();
            }
            else
            {
                this.Close();
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ID = Convert.ToInt32(cmbUserType.SelectedValue);

                if (ID != 3)
                {
                    lstUserPrevilage = new List<UserPrevilage>();

                    //User Previlage
                    UserPrevilage UP = new UserPrevilage
                    {
                        UsertypeId = ID,
                        ProjectName = frmMain.btnUser.Tag.ToString(),
                        FormName = frmMain.btnUser.Tag.ToString(),
                        AddNew = false,
                        Edit = false,
                        Show = ckbUPVView.IsChecked,
                        Remove = false
                    };
                    lstUserPrevilage.Add(UP);
                    fUserPrevilage();

                    //DB BACK UP
                    UserPrevilage DB = new UserPrevilage
                    {
                        UsertypeId = ID,
                        ProjectName = frmMain.btnDataBase.Tag.ToString(),
                        FormName = frmMain.btnDataBase.Tag.ToString(),
                        AddNew = false,
                        Edit = false,
                        Show = ckbBCKView.IsChecked,
                        Remove = false
                    };
                    lstUserPrevilage.Add(DB);

                    //Membership
                    UserPrevilage MS = new UserPrevilage
                    {
                        UsertypeId = ID,
                        ProjectName = frmMain.btnMember.Tag.ToString(),
                        FormName = frmMain.btnMember.Tag.ToString(),
                        AddNew = false,
                        Edit = false,
                        Show = ckbMSPView.IsChecked,
                        Remove = false,
                    };
                    lstUserPrevilage.Add(MS);
                    fMembership();

                    //ACCOUNTS
                    UserPrevilage ACN = new UserPrevilage
                    {
                        UsertypeId = ID,
                        ProjectName = frmMain.btnAccounts.Tag.ToString(),
                        FormName = frmMain.btnAccounts.Tag.ToString(),
                        AddNew = false,
                        Edit = false,
                        Show = ckbACTView.IsChecked,
                        Remove = false
                    };
                    lstUserPrevilage.Add(ACN);
                    //fAccounts();

                    if (lstUserPrevilage.Count > 0)
                    {
                        var user = (from m in db.UserPrevilages where m.UsertypeId == ID select m).ToList();
                        var OldData = new JSonHelper().ConvertObjectToJSon(user);
                        if (user != null)
                        {
                            db.UserPrevilages.RemoveRange(db.UserPrevilages.Where(x => x.UsertypeId == ID));
                            db.SaveChanges();
                        }

                        db.UserPrevilages.AddRange(lstUserPrevilage);
                        db.SaveChanges();

                        var NewData = new JSonHelper().ConvertObjectToJSon(lstUserPrevilage);
                        AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "UserPrevilage");
                        MessageBox.Show("Changes Saved Sucessfully!");
                    }
                }
                else
                {
                    MessageBox.Show("You Can't Change This! , Contact Administrator!", "User Restricted!", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }


            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void cmbUserType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadWindow();
        }

        #endregion     

        #region "CHECK BOX EVENTS"

        private void ckbCAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbCAll.IsChecked == true)
            {
                ckbCAdd.IsChecked = true;
                ckbCEdit.IsChecked = true;
                ckbCView.IsChecked = true;
                ckbCDelete.IsChecked = true;
            }
            else
            {
                ckbCAdd.IsChecked = false;
                ckbCEdit.IsChecked = false;
                ckbCView.IsChecked = false;
                ckbCDelete.IsChecked = false;

            }
        }

        private void ckbSAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbSAll.IsChecked == true)
            {
                ckbSAdd.IsChecked = true;
                ckbSEdit.IsChecked = true;
                ckbSView.IsChecked = true;
                ckbSDelete.IsChecked = true;
            }
            else
            {
                ckbSAdd.IsChecked = false;
                ckbSEdit.IsChecked = false;
                ckbSView.IsChecked = false;
                ckbSDelete.IsChecked = false;

            }
        }

        private void ckbCoAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbCoAll.IsChecked == true)
            {
                ckbCoSAdd.IsChecked = true;
                ckbCoEdit.IsChecked = true;
                ckbCoView.IsChecked = true;
                ckbCoDelete.IsChecked = true;
            }
            else
            {
                ckbCoSAdd.IsChecked = false;
                ckbCoEdit.IsChecked = false;
                ckbCoView.IsChecked = false;
                ckbCoDelete.IsChecked = false;

            }
        }

        private void ckbRAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbRAll.IsChecked == true)
            {
                ckbRAdd.IsChecked = true;
                ckbREdit.IsChecked = true;
                ckbRView.IsChecked = true;
                ckbRDelete.IsChecked = true;
            }
            else
            {
                ckbRAdd.IsChecked = false;
                ckbREdit.IsChecked = false;
                ckbRView.IsChecked = false;
                ckbRDelete.IsChecked = false;
            }
        }

        private void ckbRSAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbRSAll.IsChecked == true)
            {
                ckbRSAdd.IsChecked = true;
                ckbRSEdit.IsChecked = true;
                ckbRSView.IsChecked = true;
                ckbRSDelete.IsChecked = true;
            }
            else
            {
                ckbRSAdd.IsChecked = false;
                ckbRSEdit.IsChecked = false;
                ckbRSView.IsChecked = false;
                ckbRSDelete.IsChecked = false;
            }
        }

        private void ckbBAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbBAll.IsChecked == true)
            {
                ckbBAdd.IsChecked = true;
                ckbBEdit.IsChecked = true;
                ckbBView.IsChecked = true;
                ckbBDelete.IsChecked = true;
            }
            else
            {
                ckbBAdd.IsChecked = false;
                ckbBEdit.IsChecked = false;
                ckbBView.IsChecked = false;
                ckbBDelete.IsChecked = false;
            }
        }

        private void ckbBrAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbBrAll.IsChecked == true)
            {
                ckbBrAdd.IsChecked = true;
                ckbBrEdit.IsChecked = true;
                ckbBrView.IsChecked = true;
                ckbBrDelete.IsChecked = true;
            }
            else
            {
                ckbBrAdd.IsChecked = false;
                ckbBrEdit.IsChecked = false;
                ckbBrView.IsChecked = false;
                ckbBrDelete.IsChecked = false;
            }
        }

        private void ckbNBAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbNBAll.IsChecked == true)
            {
                ckbNBAdd.IsChecked = true;
                ckbNBEdit.IsChecked = true;
                ckbNBView.IsChecked = true;
                ckbNBDelete.IsChecked = true;
            }
            else
            {
                ckbNBAdd.IsChecked = false;
                ckbNBEdit.IsChecked = false;
                ckbNBView.IsChecked = false;
                ckbNBDelete.IsChecked = false;
            }
        }

        private void ckbNAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbNAll.IsChecked == true)
            {
                ckbNAdd.IsChecked = true;
                ckbNEdit.IsChecked = true;
                ckbNView.IsChecked = true;
                ckbNDelete.IsChecked = true;
            }
            else
            {
                ckbNAdd.IsChecked = false;
                ckbNEdit.IsChecked = false;
                ckbNView.IsChecked = false;
                ckbNDelete.IsChecked = false;
            }
        }

        private void ckbPTAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbPTAll.IsChecked == true)
            {
                ckbPTAdd.IsChecked = true;
                ckbPTEdit.IsChecked = true;
                ckbPTView.IsChecked = true;
                ckbPTDelete.IsChecked = true;
            }
            else
            {
                ckbPTAdd.IsChecked = false;
                ckbPTEdit.IsChecked = false;
                ckbPTView.IsChecked = false;
                ckbPTDelete.IsChecked = false;
            }
        }

        private void ckbSALAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbSALAll.IsChecked == true)
            {
                ckbSALAdd.IsChecked = true;
                ckbSALEdit.IsChecked = true;
                ckbSALView.IsChecked = true;
                ckbSALDelete.IsChecked = true;
            }
            else
            {
                ckbSALAdd.IsChecked = false;
                ckbSALEdit.IsChecked = false;
                ckbSALView.IsChecked = false;
                ckbSALDelete.IsChecked = false;
            }
        }

        private void ckbMAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbMAll.IsChecked == true)
            {
                ckbMAdd.IsChecked = true;
                ckbMEdit.IsChecked = true;
                ckbMView.IsChecked = true;
                ckbMDelete.IsChecked = true;
            }
            else
            {
                ckbMAdd.IsChecked = false;
                ckbMEdit.IsChecked = false;
                ckbMView.IsChecked = false;
                ckbMDelete.IsChecked = false;
            }
        }

        private void ckbUAAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbUAAll.IsChecked == true)
            {
                ckbUAAdd.IsChecked = true;
                ckbUAEdit.IsChecked = true;
                ckbUAView.IsChecked = true;
                ckbUADelete.IsChecked = true;
            }
            else
            {
                ckbUAAdd.IsChecked = false;
                ckbUAEdit.IsChecked = false;
                ckbUAView.IsChecked = false;
                ckbUADelete.IsChecked = false;
            }
        }

        private void ckbURAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbURAll.IsChecked == true)
            {
                ckbURAdd.IsChecked = true;
                ckbUREdit.IsChecked = true;
                ckbURView.IsChecked = true;
                ckbURDelete.IsChecked = true;
            }
            else
            {
                ckbURAdd.IsChecked = false;
                ckbUREdit.IsChecked = false;
                ckbURView.IsChecked = false;
                ckbURDelete.IsChecked = false;
            }
        }

        private void ckbFAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbFAll.IsChecked == true)
            {
                ckbFAdd.IsChecked = true;
                ckbFEdit.IsChecked = true;
                ckbFView.IsChecked = true;
                ckbFDelete.IsChecked = true;
            }
            else
            {
                ckbFAdd.IsChecked = false;
                ckbFEdit.IsChecked = false;
                ckbFView.IsChecked = false;
                ckbFDelete.IsChecked = false;
            }
        }

        private void ckbRGAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbRGAll.IsChecked == true)
            {
                ckbRGAdd.IsChecked = true;
                ckbRGEdit.IsChecked = true;
                ckbRGView.IsChecked = true;
                ckbRGDelete.IsChecked = true;
            }
            else
            {
                ckbRGAdd.IsChecked = false;
                ckbRGEdit.IsChecked = false;
                ckbRGView.IsChecked = false;
                ckbRGDelete.IsChecked = false;
            }
        }

        private void ckbMTAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbMTAll.IsChecked == true)
            {
                ckbMTAdd.IsChecked = true;
                ckbMTEdit.IsChecked = true;
                ckbMTView.IsChecked = true;
                ckbMTDelete.IsChecked = true;
            }
            else
            {
                ckbMTAdd.IsChecked = false;
                ckbMTEdit.IsChecked = false;
                ckbMTView.IsChecked = false;
                ckbMTDelete.IsChecked = false;
            }
        }

        private void ckbPRAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbPRAll.IsChecked == true)
            {
                ckbPRAdd.IsChecked = true;
                ckbPREdit.IsChecked = true;
                ckbPRView.IsChecked = true;
                ckbPRDelete.IsChecked = true;
            }
            else
            {
                ckbPRAdd.IsChecked = false;
                ckbPREdit.IsChecked = false;
                ckbPRView.IsChecked = false;
                ckbPRDelete.IsChecked = false;
            }
        }

        private void ckbPOAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbPOAll.IsChecked == true)
            {
                ckbPOAdd.IsChecked = true;
                ckbPOEdit.IsChecked = true;
                ckbPOView.IsChecked = true;
                ckbPODelete.IsChecked = true;
            }
            else
            {
                ckbPOAdd.IsChecked = false;
                ckbPOEdit.IsChecked = false;
                ckbPOView.IsChecked = false;
                ckbPODelete.IsChecked = false;
            }
        }

        private void ckbLYAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbLYAll.IsChecked == true)
            {
                ckbLYAdd.IsChecked = true;
                ckbLYEdit.IsChecked = true;
                ckbLYView.IsChecked = true;
                ckbLYDelete.IsChecked = true;
            }
            else
            {
                ckbLYAdd.IsChecked = false;
                ckbLYEdit.IsChecked = false;
                ckbLYView.IsChecked = false;
                ckbLYDelete.IsChecked = false;
            }
        }

        private void ckbTDAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbTDAll.IsChecked == true)
            {
                ckbTDAdd.IsChecked = true;
                ckbTDEdit.IsChecked = true;
                ckbTDView.IsChecked = true;
                ckbTDDelete.IsChecked = true;
            }
            else
            {
                ckbTDAdd.IsChecked = false;
                ckbTDEdit.IsChecked = false;
                ckbTDView.IsChecked = false;
                ckbTDDelete.IsChecked = false;
            }
        }

        private void ckbLGAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbLGAll.IsChecked == true)
            {
                ckbLGAdd.IsChecked = true;
                ckbLGEdit.IsChecked = true;
                ckbLGView.IsChecked = true;
                ckbLGDelete.IsChecked = true;
            }
            else
            {
                ckbLGAdd.IsChecked = false;
                ckbLGEdit.IsChecked = false;
                ckbLGView.IsChecked = false;
                ckbLGDelete.IsChecked = false;
            }
        }

        private void ckbGHAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbGHAll.IsChecked == true)
            {
                ckbGHAdd.IsChecked = true;
                ckbGHEdit.IsChecked = true;
                ckbGHView.IsChecked = true;
                ckbGHDelete.IsChecked = true;
            }
            else
            {
                ckbGHAdd.IsChecked = false;
                ckbGHEdit.IsChecked = false;
                ckbGHView.IsChecked = false;
                ckbGHDelete.IsChecked = false;
            }
        }

        private void ckbPVAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbPVAll.IsChecked == true)
            {
                ckbPVAdd.IsChecked = true;
                ckbPVEdit.IsChecked = true;
                ckbPVView.IsChecked = true;
                ckbPVDelete.IsChecked = true;
            }
            else
            {
                ckbPVAdd.IsChecked = false;
                ckbPVEdit.IsChecked = false;
                ckbPVView.IsChecked = false;
                ckbPVDelete.IsChecked = false;
            }
        }

        private void ckbRVAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbRVAll.IsChecked == true)
            {
                ckbRVAdd.IsChecked = true;
                ckbRVEdit.IsChecked = true;
                ckbRVView.IsChecked = true;
                ckbRVDelete.IsChecked = true;
            }
            else
            {
                ckbRVAdd.IsChecked = false;
                ckbRVEdit.IsChecked = false;
                ckbRVView.IsChecked = false;
                ckbRVDelete.IsChecked = false;
            }
        }

        private void ckbJNAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbJNAll.IsChecked == true)
            {
                ckbJNAdd.IsChecked = true;
                ckbJNEdit.IsChecked = true;
                ckbJNView.IsChecked = true;
                ckbJNDelete.IsChecked = true;
            }
            else
            {
                ckbJNAdd.IsChecked = false;
                ckbJNEdit.IsChecked = false;
                ckbJNView.IsChecked = false;
                ckbJNDelete.IsChecked = false;
            }
        }

        private void ckbLOAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbLOAll.IsChecked == true)
            {
                ckbLOAdd.IsChecked = true;
                ckbLOEdit.IsChecked = true;
                ckbLOView.IsChecked = true;
                ckbLODelete.IsChecked = true;
            }
            else
            {
                ckbLOAdd.IsChecked = false;
                ckbLOEdit.IsChecked = false;
                ckbLOView.IsChecked = false;
                ckbLODelete.IsChecked = false;
            }
        }

        private void ckbRCAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbRCAll.IsChecked == true)
            {
                ckbRCAdd.IsChecked = true;
                ckbRCEdit.IsChecked = true;
                ckbRCView.IsChecked = true;
                ckbRCDelete.IsChecked = true;
            }
            else
            {
                ckbRCAdd.IsChecked = false;
                ckbRCEdit.IsChecked = false;
                ckbRCView.IsChecked = false;
                ckbRCDelete.IsChecked = false;
            }
        }


        //private void ckbACTAll_Click(object sender, RoutedEventArgs e)
        //{
        //    if (ckbACTAll.IsChecked == true)
        //    {
        //        ckbACTAdd.IsChecked = true;
        //        ckbACTEdit.IsChecked = true;
        //        ckbACTView.IsChecked = true;
        //        ckbACTDelete.IsChecked = true;
        //    }
        //    else
        //    {
        //        ckbACTAdd.IsChecked = false;
        //        ckbACTEdit.IsChecked = false;
        //        ckbACTView.IsChecked = false;
        //        ckbACTDelete.IsChecked = false;
        //    }
        //}

        private void ckbIRCAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbIRCAll.IsChecked == true)
            {
                ckbIRCAdd.IsChecked = true;
                ckbIRCEdit.IsChecked = true;
                ckbIRCView.IsChecked = true;
                ckbIRCDelete.IsChecked = true;
            }
            else
            {
                ckbIRCAdd.IsChecked = false;
                ckbIRCEdit.IsChecked = false;
                ckbIRCView.IsChecked = false;
                ckbIRCDelete.IsChecked = false;
            }
        }

        private void ckbUTAll_Click(object sender, RoutedEventArgs e)
        {
            if (ckbUTAll.IsChecked == true)
            {
                ckbUTAdd.IsChecked = true;
                ckbUTEdit.IsChecked = true;
                ckbUTView.IsChecked = true;
                ckbUTDelete.IsChecked = true;
            }
            else
            {
                ckbUTAdd.IsChecked = false;
                ckbUTEdit.IsChecked = false;
                ckbUTView.IsChecked = false;
                ckbUTDelete.IsChecked = false;
            }
        }

        private void ckbURAll_Click_1(object sender, RoutedEventArgs e)
        {
            if (ckbURAll.IsChecked == true)
            {
                ckbURAdd.IsChecked = true;
                ckbUREdit.IsChecked = true;
                ckbURView.IsChecked = true;
                ckbURDelete.IsChecked = true;
            }
            else
            {
                ckbURAdd.IsChecked = false;
                ckbUREdit.IsChecked = false;
                ckbURView.IsChecked = false;
                ckbURDelete.IsChecked = false;
            }
        }

        #endregion

        #region "USER DEFIENED FUNCTIONS"

        void LoadWindow()
        {
            try
            {
                ID = Convert.ToInt32(cmbUserType.SelectedValue);
                if (ID != 0)
                {
                    var UsrPrv = (from up in db.UserPrevilages where up.UsertypeId == ID select up).ToList();
                    DataTable dtUser = AppLib.LINQResultToDataTable(UsrPrv);
                    foreach (DataRow dr in dtUser.Rows)
                    {
                        if (dr["ProjectName"].ToString() == frmMain.btnUser.Tag.ToString() && dr["FormName"].ToString() == frmMain.btnUser.Tag.ToString())
                        {
                            ckbUPVView.IsChecked = Convert.ToBoolean(dr["Show"]);
                        }
                        else if (dr["ProjectName"].ToString() == frmMain.btnUser.Tag.ToString() && dr["FormName"].ToString() == frmUsrPLG.btnUserAccount.Tag.ToString())
                        {
                            ckbUAAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbUAEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbUAView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbUADelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }
                        else if (dr["ProjectName"].ToString() == frmMain.btnUser.Tag.ToString() && dr["FormName"].ToString() == frmUsrPLG.btnUserType.Tag.ToString())
                        {
                            ckbUTAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbUTEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbUTView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbUTDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnUser.Tag.ToString() && dr["FormName"].ToString() == frmUsrPLG.btnUserRights.Tag.ToString())
                        {
                            ckbURAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbUREdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbURView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbURDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }
                        else if (dr["ProjectName"].ToString() == frmMain.btnDataBase.Tag.ToString() && dr["FormName"].ToString() == frmMain.btnDataBase.Tag.ToString())
                        {
                            ckbBCKView.IsChecked = Convert.ToBoolean(dr["Show"]);                            
                        }
                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmMain.btnMember.Tag.ToString())
                        {
                            ckbMSPView.IsChecked = Convert.ToBoolean(dr["Show"]);
                        }
                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmMaster.btnCity.Tag.ToString())
                        {
                            ckbCAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbCEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbCView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbCDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmMaster.btnStateSetup.Tag.ToString())
                        {
                            ckbSAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbSEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbSView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbSDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmMaster.btnCountry.Tag.ToString())
                        {
                            ckbCoSAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbCoEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbCoView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbCoDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmMaster.btnBank.Tag.ToString())
                        {
                            ckbBAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbBEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbBView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbBDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmMaster.btnBranch.Tag.ToString())
                        {
                            ckbBrAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbBrEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbBrView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbBrDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmMaster.btnNUBEBranch.Tag.ToString())
                        {
                            ckbNBAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbNBEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbNBView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbNBDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmMaster.btnRelationSetup.Tag.ToString())
                        {
                            ckbRAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbREdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbRView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbRDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmMaster.btnReasonSetup.Tag.ToString())
                        {
                            ckbRSAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbRSEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbRSView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbRSDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmMaster.btnNameSetup.Tag.ToString())
                        {
                            ckbNAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbNEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbNView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbNDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmMaster.btnPersonTitle.Tag.ToString())
                        {
                            ckbPTAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbPTEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbPTView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbPTDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmMaster.btnSalutation.Tag.ToString())
                        {
                            ckbSALAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbSALEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbSALView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbSALDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmMaster.btnMonthEndClosing.Tag.ToString())
                        {
                            ckbMMAdd.IsChecked = Convert.ToBoolean(dr["Show"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmMaster.btnYearEndClosing.Tag.ToString())
                        {
                            ckbMYAdd.IsChecked = Convert.ToBoolean(dr["Show"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmTran.btnMemberRegistration.Tag.ToString())
                        {
                            ckbMAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbMEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbMView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbMDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmTran.btnFeeEntry.Tag.ToString())
                        {
                            ckbFAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbFEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbFView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbFDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmTran.btnResingation.Tag.ToString())
                        {
                            ckbRGAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbRGEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbRGView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbRGDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmTran.btnMemberQuery.Tag.ToString())
                        {
                            ckbMQView.IsChecked = Convert.ToBoolean(dr["Show"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmTran.btnPreApr16.Tag.ToString())
                        {
                            ckbPRAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbPREdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbPRView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbPRDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmTran.btnPostApr16.Tag.ToString())
                        {
                            ckbPOAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbPOEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbPOView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbPODelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmTran.btnLevy.Tag.ToString())
                        {
                            ckbLYAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbLYEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbLYView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbLYDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmTran.btnTDF.Tag.ToString())
                        {
                            ckbTDAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbTDEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbTDView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbTDDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmTran.btnTransfer.Tag.ToString())
                        {
                            ckbMTAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                            ckbMTEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                            ckbMTView.IsChecked = Convert.ToBoolean(dr["Show"]);
                            ckbMTDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmRpt.btnActiveMember.Tag.ToString())
                        {
                            ckbACReport.IsChecked = Convert.ToBoolean(dr["Show"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmRpt.btnBranchAdvice.Tag.ToString())
                        {
                            ckbBAReport.IsChecked = Convert.ToBoolean(dr["Show"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmRpt.btnDefaultMember.Tag.ToString())
                        {
                            ckbDMReport.IsChecked = Convert.ToBoolean(dr["Show"]);
                        }

                        //else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmRpt.btnMonthlyStmt.Tag.ToString())
                        //{
                        //    ckbMSReport.IsChecked = Convert.ToBoolean(dr["Show"]);
                        //}

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmRpt.btnStruckOff.Tag.ToString())
                        {
                            ckbSMReport.IsChecked = Convert.ToBoolean(dr["Show"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmRpt.btnHalfShareReport.Tag.ToString())
                        {
                            ckbFHReport.IsChecked = Convert.ToBoolean(dr["Show"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmRpt.btnResign.Tag.ToString())
                        {
                            ckbRMReport.IsChecked = Convert.ToBoolean(dr["Show"]);
                        }

                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmRpt.btnBankStaticsToBranch.Tag.ToString())
                        {
                            ckbBSReport.IsChecked = Convert.ToBoolean(dr["Show"]);
                        }
                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmRpt.btnComparitionReport.Tag.ToString())
                        {
                            ckbCMReport.IsChecked = Convert.ToBoolean(dr["Show"]);
                        }
                        else if (dr["ProjectName"].ToString() == frmMain.btnMember.Tag.ToString() && dr["FormName"].ToString() == frmRpt.btnFeeReport.Tag.ToString())
                        {
                            ckbFEReport.IsChecked = Convert.ToBoolean(dr["Show"]);
                        }
                        else if (dr["ProjectName"].ToString() == frmMain.btnAccounts.Tag.ToString() && dr["FormName"].ToString() == frmMain.btnAccounts.Tag.ToString())
                        {
                            ckbACTView.IsChecked = Convert.ToBoolean(dr["Show"]);
                        }
                        //else if (dr["ProjectName"].ToString() == frmMain.btnAccounts.Tag.ToString() && dr["FormName"].ToString() == frmHMst.btnLedger.Tag.ToString())
                        //{
                        //    ckbLGAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                        //    ckbLGEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                        //    ckbLGView.IsChecked = Convert.ToBoolean(dr["Show"]);
                        //    ckbLGDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        //}
                        //else if (dr["ProjectName"].ToString() == frmMain.btnAccounts.Tag.ToString() && dr["FormName"].ToString() == frmHMst.btnGroupHead.Tag.ToString())
                        //{
                        //    ckbGHAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                        //    ckbGHEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                        //    ckbGHView.IsChecked = Convert.ToBoolean(dr["Show"]);
                        //    ckbGHDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        //}
                        //else if (dr["ProjectName"].ToString() == frmMain.btnAccounts.Tag.ToString() && dr["FormName"].ToString() == frmHMst.btnYearEnd.Tag.ToString())
                        //{
                        //    ckbAYAdd.IsChecked = Convert.ToBoolean(dr["Show"]);
                        //}
                        //else if (dr["ProjectName"].ToString() == frmMain.btnAccounts.Tag.ToString() && dr["FormName"].ToString() == frmHAcc.btnPayment.Tag.ToString())
                        //{
                        //    ckbPVAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                        //    ckbPVEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                        //    ckbPVView.IsChecked = Convert.ToBoolean(dr["Show"]);
                        //    ckbPVDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        //}
                        //else if (dr["ProjectName"].ToString() == frmMain.btnAccounts.Tag.ToString() && dr["FormName"].ToString() == frmHAcc.btnReceipt.Tag.ToString())
                        //{
                        //    ckbRVAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                        //    ckbRVEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                        //    ckbRVView.IsChecked = Convert.ToBoolean(dr["Show"]);
                        //    ckbRVDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        //}
                        //else if (dr["ProjectName"].ToString() == frmMain.btnAccounts.Tag.ToString() && dr["FormName"].ToString() == frmHAcc.btnJournal.Tag.ToString())
                        //{
                        //    ckbJNAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                        //    ckbJNEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                        //    ckbJNView.IsChecked = Convert.ToBoolean(dr["Show"]);
                        //    ckbJNDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        //}
                        //else if (dr["ProjectName"].ToString() == frmMain.btnAccounts.Tag.ToString() && dr["FormName"].ToString() == frmHAcc.btnLedgerOpening.Tag.ToString())
                        //{
                        //    ckbLOAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                        //    ckbLOEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                        //    ckbLOView.IsChecked = Convert.ToBoolean(dr["Show"]);
                        //    ckbLODelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        //}
                        //else if (dr["ProjectName"].ToString() == frmMain.btnAccounts.Tag.ToString() && dr["FormName"].ToString() == frmHAcc.btnBankReconcilation.Tag.ToString())
                        //{
                        //    ckbRCAdd.IsChecked = Convert.ToBoolean(dr["AddNew"]);
                        //    ckbRCEdit.IsChecked = Convert.ToBoolean(dr["Edit"]);
                        //    ckbRCView.IsChecked = Convert.ToBoolean(dr["Show"]);
                        //    ckbRCDelete.IsChecked = Convert.ToBoolean(dr["Remove"]);
                        //}
                        //else if (dr["ProjectName"].ToString() == frmMain.btnAccounts.Tag.ToString() && dr["FormName"].ToString() == frmHRpt.btnTrailBalance.Tag.ToString())
                        //{
                        //    ckbTBView.IsChecked = Convert.ToBoolean(dr["Show"]);
                        //}
                        //else if (dr["ProjectName"].ToString() == frmMain.btnAccounts.Tag.ToString() && dr["FormName"].ToString() == frmHRpt.btnBalanceSheet.Tag.ToString())
                        //{
                        //    ckbBSView.IsChecked = Convert.ToBoolean(dr["Show"]);
                        //}
                        //else if (dr["ProjectName"].ToString() == frmMain.btnAccounts.Tag.ToString() && dr["FormName"].ToString() == frmHRpt.btnActivityReport.Tag.ToString())
                        //{
                        //    ckbARView.IsChecked = Convert.ToBoolean(dr["Show"]);
                        //}
                        //else if (dr["ProjectName"].ToString() == frmMain.btnAccounts.Tag.ToString() && dr["FormName"].ToString() == frmHRpt.btnGeneralLedger.Tag.ToString())
                        //{
                        //    ckbGLView.IsChecked = Convert.ToBoolean(dr["Show"]);
                        //}
                        //else if (dr["ProjectName"].ToString() == frmMain.btnAccounts.Tag.ToString() && dr["FormName"].ToString() == frmHRpt.btnReceiptPaymentReport.Tag.ToString())
                        //{
                        //    ckbRPView.IsChecked = Convert.ToBoolean(dr["Show"]);
                        //}
                        //else if (dr["ProjectName"].ToString() == frmMain.btnAccounts.Tag.ToString() && dr["FormName"].ToString() == frmHRpt.btnVoucherReport.Tag.ToString())
                        //{
                        //    ckbVRView.IsChecked = Convert.ToBoolean(dr["Show"]);
                        //}
                        //else if (dr["ProjectName"].ToString() == frmMain.btnAccounts.Tag.ToString() && dr["FormName"].ToString() == frmHRpt.btnProfitLoss.Tag.ToString())
                        //{
                        //    ckbPLView.IsChecked = Convert.ToBoolean(dr["Show"]);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }     

        void fUserPrevilage()
        {
            //User Accounts
            UserPrevilage UA = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnUser.Tag.ToString(),
                FormName = frmUsrPLG.btnUserAccount.Tag.ToString(),
                AddNew = ckbUAAdd.IsChecked,
                Edit = ckbUAEdit.IsChecked,
                Show = ckbUAView.IsChecked,
                Remove = ckbUADelete.IsChecked,
            };
            lstUserPrevilage.Add(UA);

            //User Type
            UserPrevilage UT = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnUser.Tag.ToString(),
                FormName = frmUsrPLG.btnUserType.Tag.ToString(),
                AddNew = ckbUTAdd.IsChecked,
                Edit = ckbUTEdit.IsChecked,
                Show = ckbUTView.IsChecked,
                Remove = ckbUTDelete.IsChecked,
            };
            lstUserPrevilage.Add(UT);

            //User Rights
            UserPrevilage UR = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnUser.Tag.ToString(),
                FormName = frmUsrPLG.btnUserRights.Tag.ToString(),
                AddNew = ckbURAdd.IsChecked,
                Edit = ckbUREdit.IsChecked,
                Show = ckbURView.IsChecked,
                Remove = ckbURDelete.IsChecked,
            };
            lstUserPrevilage.Add(UR);
        }

        void fMembership()
        {
            //#################### MASTER SETUP #####################  
            //City Setup
            UserPrevilage sd = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmMaster.btnCity.Tag.ToString(),
                AddNew = ckbCAdd.IsChecked,
                Edit = ckbCEdit.IsChecked,
                Show = ckbCView.IsChecked,
                Remove = ckbCDelete.IsChecked
            };
            lstUserPrevilage.Add(sd);

            //State Setup
            UserPrevilage ds = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmMaster.btnStateSetup.Tag.ToString(),
                AddNew = ckbSAdd.IsChecked,
                Edit = ckbSEdit.IsChecked,
                Show = ckbSView.IsChecked,
                Remove = ckbSDelete.IsChecked
            };
            lstUserPrevilage.Add(ds);

            //Country Setup
            UserPrevilage co = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmMaster.btnCountry.Tag.ToString(),
                AddNew = ckbCoSAdd.IsChecked,
                Edit = ckbCoEdit.IsChecked,
                Show = ckbCoView.IsChecked,
                Remove = ckbCoDelete.IsChecked,
            };
            lstUserPrevilage.Add(co);

            //Bank Setup
            UserPrevilage B = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmMaster.btnBank.Tag.ToString(),
                AddNew = ckbBAdd.IsChecked,
                Edit = ckbBEdit.IsChecked,
                Show = ckbBView.IsChecked,
                Remove = ckbBDelete.IsChecked,
            };
            lstUserPrevilage.Add(B);

            //Branch Setup
            UserPrevilage Br = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmMaster.btnBranch.Tag.ToString(),
                AddNew = ckbBrAdd.IsChecked,
                Edit = ckbBrEdit.IsChecked,
                Show = ckbBrView.IsChecked,
                Remove = ckbBrDelete.IsChecked
            };
            lstUserPrevilage.Add(Br);

            //NUBE Branch Setup
            UserPrevilage NB = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmMaster.btnNUBEBranch.Tag.ToString(),
                AddNew = ckbNBAdd.IsChecked,
                Edit = ckbNBEdit.IsChecked,
                Show = ckbNBView.IsChecked,
                Remove = ckbNBDelete.IsChecked,
            };
            lstUserPrevilage.Add(NB);

            //Relation Setup
            UserPrevilage RS = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmMaster.btnRelationSetup.Tag.ToString(),
                AddNew = ckbRAdd.IsChecked,
                Edit = ckbREdit.IsChecked,
                Show = ckbRView.IsChecked,
                Remove = ckbRDelete.IsChecked
            };
            lstUserPrevilage.Add(RS);

            //Reason Setup
            UserPrevilage R = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmMaster.btnReasonSetup.Tag.ToString(),
                AddNew = ckbRSAdd.IsChecked,
                Edit = ckbRSEdit.IsChecked,
                Show = ckbRSView.IsChecked,
                Remove = ckbRSDelete.IsChecked,
            };
            lstUserPrevilage.Add(R);

            //Name Setup
            UserPrevilage N = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmMaster.btnNameSetup.Tag.ToString(),
                AddNew = ckbNAdd.IsChecked,
                Edit = ckbNEdit.IsChecked,
                Show = ckbNView.IsChecked,
                Remove = ckbNDelete.IsChecked,
            };
            lstUserPrevilage.Add(N);

            //Person Title Setup
            UserPrevilage T = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmMaster.btnPersonTitle.Tag.ToString(),
                AddNew = ckbPTAdd.IsChecked,
                Edit = ckbPTEdit.IsChecked,
                Show = ckbPTView.IsChecked,
                Remove = ckbPTDelete.IsChecked,
            };
            lstUserPrevilage.Add(T);

            //Salutation Setup
            UserPrevilage SAL = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmMaster.btnSalutation.Tag.ToString(),
                AddNew = ckbSALAdd.IsChecked,
                Edit = ckbSALEdit.IsChecked,
                Show = ckbSALView.IsChecked,
                Remove = ckbSALDelete.IsChecked,
            };
            lstUserPrevilage.Add(SAL);

            //Month End
            UserPrevilage ME = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmMaster.btnMonthEndClosing.Tag.ToString(),
                AddNew = false,
                Edit = false,
                Show = ckbMMAdd.IsChecked,
                Remove = false,
            };
            lstUserPrevilage.Add(ME);

            //Year End
            UserPrevilage YE = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmMaster.btnYearEndClosing.Tag.ToString(),
                AddNew = false,
                Edit = false,
                Show = ckbMYAdd.IsChecked,
                Remove = false,
            };
            lstUserPrevilage.Add(YE);

            //#################### TRANSACTION #####################            
            //Member Registration
            UserPrevilage M = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmTran.btnMemberRegistration.Tag.ToString(),
                AddNew = ckbMAdd.IsChecked,
                Edit = ckbMEdit.IsChecked,
                Show = ckbMView.IsChecked,
                Remove = ckbMDelete.IsChecked,
            };
            lstUserPrevilage.Add(M);

            //Fees Entry
            UserPrevilage F = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmTran.btnFeeEntry.Tag.ToString(),
                AddNew = ckbFAdd.IsChecked,
                Edit = ckbFEdit.IsChecked,
                Show = ckbFView.IsChecked,
                Remove = ckbFDelete.IsChecked,
            };
            lstUserPrevilage.Add(F);

            //Resignation
            UserPrevilage RG = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmTran.btnResingation.Tag.ToString(),
                AddNew = ckbRGAdd.IsChecked,
                Edit = ckbRGEdit.IsChecked,
                Show = ckbRGView.IsChecked,
                Remove = ckbRGDelete.IsChecked,
            };
            lstUserPrevilage.Add(RG);

            //Member Query
            UserPrevilage MQ = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmTran.btnMemberQuery.Tag.ToString(),
                AddNew = false,
                Edit = false,
                Show = ckbMQView.IsChecked,
                Remove = false,
            };
            lstUserPrevilage.Add(MQ);

            //Pre Arrear 16
            UserPrevilage PR = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmTran.btnPreApr16.Tag.ToString(),
                AddNew = ckbPRAdd.IsChecked,
                Edit = ckbPREdit.IsChecked,
                Show = ckbPRView.IsChecked,
                Remove = ckbPRDelete.IsChecked,
            };
            lstUserPrevilage.Add(PR);

            //Post Arrear 16
            UserPrevilage PO = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmTran.btnPostApr16.Tag.ToString(),
                AddNew = ckbPOAdd.IsChecked,
                Edit = ckbPOEdit.IsChecked,
                Show = ckbPOView.IsChecked,
                Remove = ckbPODelete.IsChecked,
            };
            lstUserPrevilage.Add(PO);

            //LEVY
            UserPrevilage LY = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmTran.btnLevy.Tag.ToString(),
                AddNew = ckbLYAdd.IsChecked,
                Edit = ckbLYEdit.IsChecked,
                Show = ckbLYView.IsChecked,
                Remove = ckbLYDelete.IsChecked,
            };
            lstUserPrevilage.Add(LY);

            //TDF
            UserPrevilage TD = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmTran.btnTDF.Tag.ToString(),
                AddNew = ckbTDAdd.IsChecked,
                Edit = ckbTDEdit.IsChecked,
                Show = ckbTDView.IsChecked,
                Remove = ckbTDDelete.IsChecked,
            };
            lstUserPrevilage.Add(TD);

            //Member Transfer
            UserPrevilage MT = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmTran.btnTransfer.Tag.ToString(),
                AddNew = ckbMTAdd.IsChecked,
                Edit = ckbMTEdit.IsChecked,
                Show = ckbMTView.IsChecked,
                Remove = ckbMTDelete.IsChecked,
            };
            lstUserPrevilage.Add(MT);

            //#################### REPORTS #####################
            //Active Member Reports
            UserPrevilage AC = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmRpt.btnActiveMember.Tag.ToString(),
                AddNew = false,
                Edit = false,
                Show = ckbACReport.IsChecked,
                Remove = false
            };
            lstUserPrevilage.Add(AC);

            //Branch Advice List
            UserPrevilage BA = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmRpt.btnBranchAdvice.Tag.ToString(),
                AddNew = false,
                Edit = false,
                Show = ckbBAReport.IsChecked,
                Remove = false
            };
            lstUserPrevilage.Add(BA);

            //Default Members
            UserPrevilage DM = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmRpt.btnDefaultMember.Tag.ToString(),
                AddNew = false,
                Edit = false,
                Show = ckbDMReport.IsChecked,
                Remove = false
            };
            lstUserPrevilage.Add(DM);

            ////Monthly Statement
            //UserPrevilage MST = new UserPrevilage
            //{
            //    UsertypeId = ID,
            //    ProjectName = frmMain.btnMember.Tag.ToString(),
            //    FormName = frmRpt.btnMonthlyStmt.Tag.ToString(),
            //    AddNew = false,
            //    Edit = false,
            //    Show = ckbMSReport.IsChecked,
            //    Remove = false
            //};
            //lstUserPrevilage.Add(MST);

            //StruckOff Members
            UserPrevilage SM = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmRpt.btnStruckOff.Tag.ToString(),
                AddNew = false,
                Edit = false,
                Show = ckbSMReport.IsChecked,
                Remove = false
            };
            lstUserPrevilage.Add(SM);

            //Financial Half Share Report
            UserPrevilage FH = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmRpt.btnHalfShareReport.Tag.ToString(),
                AddNew = false,
                Edit = false,
                Show = ckbFHReport.IsChecked,
                Remove = false
            };
            lstUserPrevilage.Add(FH);

            //Resigned Members
            UserPrevilage RM = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmRpt.btnResign.Tag.ToString(),
                AddNew = false,
                Edit = false,
                Show = ckbRMReport.IsChecked,
                Remove = false
            };
            lstUserPrevilage.Add(RM);

            //Bank Statistics To Branch
            UserPrevilage BS = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmRpt.btnBankStaticsToBranch.Tag.ToString(),
                AddNew = false,
                Edit = false,
                Show = ckbBSReport.IsChecked,
                Remove = false
            };
            lstUserPrevilage.Add(BS);

            //PAID AND UNPAID REPORT -- ComparitionReport
            UserPrevilage CM = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmRpt.btnComparitionReport.Tag.ToString(),
                AddNew = false,
                Edit = false,
                Show = ckbCMReport.IsChecked,
                Remove = false
            };
            lstUserPrevilage.Add(CM);

            //FEE REPORT
            UserPrevilage FEE = new UserPrevilage
            {
                UsertypeId = ID,
                ProjectName = frmMain.btnMember.Tag.ToString(),
                FormName = frmRpt.btnFeeReport.Tag.ToString(),
                AddNew = false,
                Edit = false,
                Show = ckbFEReport.IsChecked,
                Remove = false
            };
            lstUserPrevilage.Add(FEE);
        }

        //void fAccounts()
        //{
        //    //################### MASTER SETUP ######################
        //    //Ledger Setup
        //    UserPrevilage LG = new UserPrevilage
        //    {
        //        UsertypeId = ID,
        //        ProjectName = frmMain.btnAccounts.Tag.ToString(),
        //        FormName = frmHMst.btnLedger.Tag.ToString(),
        //        AddNew = ckbLGAdd.IsChecked,
        //        Edit = ckbLGEdit.IsChecked,
        //        Show = ckbLGView.IsChecked,
        //        Remove = ckbLGDelete.IsChecked,
        //    };
        //    lstUserPrevilage.Add(LG);

        //    //Group Head Setup
        //    UserPrevilage GH = new UserPrevilage
        //    {
        //        UsertypeId = ID,
        //        ProjectName = frmMain.btnAccounts.Tag.ToString(),
        //        FormName = frmHMst.btnGroupHead.Tag.ToString(),
        //        AddNew = ckbGHAdd.IsChecked,
        //        Edit = ckbGHEdit.IsChecked,
        //        Show = ckbGHView.IsChecked,
        //        Remove = ckbGHDelete.IsChecked,
        //    };
        //    lstUserPrevilage.Add(GH);

        //    //Accounts Year End
        //    UserPrevilage AY = new UserPrevilage
        //    {
        //        UsertypeId = ID,
        //        ProjectName = frmMain.btnAccounts.Tag.ToString(),
        //        FormName = frmHMst.btnYearEnd.Tag.ToString(),
        //        AddNew = false,
        //        Edit = false,
        //        Show = ckbAYAdd.IsChecked,
        //        Remove = false,
        //    };
        //    lstUserPrevilage.Add(AY);

        //    //######################### TRANSACTION ########################
        //    //Payment Voucher
        //    UserPrevilage PV = new UserPrevilage
        //    {
        //        UsertypeId = ID,
        //        ProjectName = frmMain.btnAccounts.Tag.ToString(),
        //        FormName = frmHAcc.btnPayment.Tag.ToString(),
        //        AddNew = ckbPVAdd.IsChecked,
        //        Edit = ckbPVEdit.IsChecked,
        //        Show = ckbPVView.IsChecked,
        //        Remove = ckbPVDelete.IsChecked,
        //    };
        //    lstUserPrevilage.Add(PV);

        //    //Receipt Voucher
        //    UserPrevilage RV = new UserPrevilage
        //    {
        //        UsertypeId = ID,
        //        ProjectName = frmMain.btnAccounts.Tag.ToString(),
        //        FormName = frmHAcc.btnReceipt.Tag.ToString(),
        //        AddNew = ckbRVAdd.IsChecked,
        //        Edit = ckbRVEdit.IsChecked,
        //        Show = ckbRVView.IsChecked,
        //        Remove = ckbRVDelete.IsChecked,
        //    };
        //    lstUserPrevilage.Add(RV);

        //    //Journal Entries
        //    UserPrevilage JN = new UserPrevilage
        //    {
        //        UsertypeId = ID,
        //        ProjectName = frmMain.btnAccounts.Tag.ToString(),
        //        FormName = frmHAcc.btnJournal.Tag.ToString(),
        //        AddNew = ckbJNAdd.IsChecked,
        //        Edit = ckbJNEdit.IsChecked,
        //        Show = ckbJNView.IsChecked,
        //        Remove = ckbJNDelete.IsChecked,
        //    };
        //    lstUserPrevilage.Add(JN);

        //    //Ledger Opening
        //    UserPrevilage LO = new UserPrevilage
        //    {
        //        UsertypeId = ID,
        //        ProjectName = frmMain.btnAccounts.Tag.ToString(),
        //        FormName = frmHAcc.btnLedgerOpening.Tag.ToString(),
        //        AddNew = ckbLOAdd.IsChecked,
        //        Edit = ckbLOEdit.IsChecked,
        //        Show = ckbLOView.IsChecked,
        //        Remove = ckbLODelete.IsChecked,
        //    };
        //    lstUserPrevilage.Add(LO);

        //    //Bank Reconciliation
        //    UserPrevilage RC = new UserPrevilage
        //    {
        //        UsertypeId = ID,
        //        ProjectName = frmMain.btnAccounts.Tag.ToString(),
        //        FormName = frmHAcc.btnBankReconcilation.Tag.ToString(),
        //        AddNew = ckbRCAdd.IsChecked,
        //        Edit = ckbRCEdit.IsChecked,
        //        Show = ckbRCView.IsChecked,
        //        Remove = ckbRCDelete.IsChecked,
        //    };
        //    lstUserPrevilage.Add(RC);

        //    //################ REPORTS #####################
        //    //Trial Balance
        //    UserPrevilage TB = new UserPrevilage
        //    {
        //        UsertypeId = ID,
        //        ProjectName = frmMain.btnAccounts.Tag.ToString(),
        //        FormName = frmHRpt.btnTrailBalance.Tag.ToString(),
        //        AddNew = false,
        //        Edit = false,
        //        Show = ckbTBView.IsChecked,
        //        Remove = false,
        //    };
        //    lstUserPrevilage.Add(TB);

        //    //Balance Sheet
        //    UserPrevilage BS = new UserPrevilage
        //    {
        //        UsertypeId = ID,
        //        ProjectName = frmMain.btnAccounts.Tag.ToString(),
        //        FormName = frmHRpt.btnBalanceSheet.Tag.ToString(),
        //        AddNew = false,
        //        Edit = false,
        //        Show = ckbBSView.IsChecked,
        //        Remove = false,
        //    };
        //    lstUserPrevilage.Add(BS);

        //    //Activity Report
        //    UserPrevilage AR = new UserPrevilage
        //    {
        //        UsertypeId = ID,
        //        ProjectName = frmMain.btnAccounts.Tag.ToString(),
        //        FormName = frmHRpt.btnActivityReport.Tag.ToString(),
        //        AddNew = false,
        //        Edit = false,
        //        Show = ckbARView.IsChecked,
        //        Remove = false,
        //    };
        //    lstUserPrevilage.Add(AR);

        //    //General Ledger
        //    UserPrevilage GL = new UserPrevilage
        //    {
        //        UsertypeId = ID,
        //        ProjectName = frmMain.btnAccounts.Tag.ToString(),
        //        FormName = frmHRpt.btnGeneralLedger.Tag.ToString(),
        //        AddNew = false,
        //        Edit = false,
        //        Show = ckbGLView.IsChecked,
        //        Remove = false,
        //    };
        //    lstUserPrevilage.Add(GL);

        //    //Receipt And Payment Report
        //    UserPrevilage RP = new UserPrevilage
        //    {
        //        UsertypeId = ID,
        //        ProjectName = frmMain.btnAccounts.Tag.ToString(),
        //        FormName = frmHRpt.btnReceiptPaymentReport.Tag.ToString(),
        //        AddNew = false,
        //        Edit = false,
        //        Show = ckbRPView.IsChecked,
        //        Remove = false,
        //    };
        //    lstUserPrevilage.Add(RP);

        //    //Voucher Report
        //    UserPrevilage VR = new UserPrevilage
        //    {
        //        UsertypeId = ID,
        //        ProjectName = frmMain.btnAccounts.Tag.ToString(),
        //        FormName = frmHRpt.btnVoucherReport.Tag.ToString(),
        //        AddNew = false,
        //        Edit = false,
        //        Show = ckbVRView.IsChecked,
        //        Remove = false,
        //    };
        //    lstUserPrevilage.Add(VR);

        //    //Profit And Loss
        //    UserPrevilage PL = new UserPrevilage
        //    {
        //        UsertypeId = ID,
        //        ProjectName = frmMain.btnAccounts.Tag.ToString(),
        //        FormName = frmHRpt.btnProfitLoss.Tag.ToString(),
        //        AddNew = false,
        //        Edit = false,
        //        Show = ckbPLView.IsChecked,
        //        Remove = false,
        //    };
        //    lstUserPrevilage.Add(PL);
        //}

        #endregion
    }
}
