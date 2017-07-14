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
using MahApps.Metro.Controls;
using Nube;

namespace Nube.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmNameSetup.xaml
    /// </summary>
    public partial class frmNameSetup : MetroWindow
    {
        public Boolean bIsEdit = false;
        nubebfsEntity db = new nubebfsEntity();
        int ID = 0;
        string sFormName = "";

        public frmNameSetup(string sForm_Name = "")
        {
            InitializeComponent();
            sFormName = sForm_Name;

            dtpFeeEntry.Visibility = Visibility.Hidden;
            lblDate.Visibility = Visibility.Hidden;
            LoadWindow();
            this.KeyDown += new System.Windows.Input.KeyEventHandler(Window_KeyDown);

        }

        private void LoadWindow()
        {
            try
            {
                MASTERNAMESETUP ms = db.MASTERNAMESETUPs.FirstOrDefault();

                txtOrganisationName.Text = ms.ORGANISATION_NAME;
                txtAddress1.Text = ms.ADDRESS1;
                txtAddress2.Text = ms.ADDRESS2;
                txtAddress3.Text = ms.ADDRESS3;
                txtCity.Text = ms.CITY;
                txtState.Text = ms.STATE;
                txtCountry.Text = ms.COUNTRY;
                txtZipCode.Text = ms.ZIPCODE;
                txtPhoneNo.Text = ms.PHONE;
                txtExePath.Text = ms.NEWEXEPATH;
                txtNewImagePath.Text = ms.NEWIMAGESPATH;
                txtNewReportPath.Text = ms.NEWREPORTSPATH;
                txtServerName.Text = ms.SERVERNAME;
                txtVersion.Text = ms.VERSION;

                txtBuildingFund.Text = ms.BuildingFund.ToString();
                txtEntranceFeed.Text = ms.EnterenceFees.ToString();
                txtBadgeAmt.Text = ms.BadgeAmount.ToString();
                txtBF.Text = ms.BF.ToString();
                txtBuildingFund.Text = ms.BuildingFund.ToString();
                txtSubscription.Text = ms.Subscription.ToString();
                txtInsurance.Text = ms.Insurance.ToString();
                txtLevyAmount.Text = ms.LevyAmount.ToString();
                txtTDFAmount.Text = ms.TDFAmount.ToString();
                txtRejoinAmount.Text = ms.RejoinAmount.ToString();

                txtEmailID.Text = ms.SenderEmailId.ToString();
                txtPassword.Password = ms.SenderPassword.ToString();
                txtCCEmailID1.Text = ms.CCEamilId1.ToString();
                txtCCEmailID2.Text = ms.CCEamilId2.ToString();

                txtPrinterName.Text = ms.PrinterName.ToString();
                txtPath.Text = ms.BackUpPath.ToString();                
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtOrganisationName.Text == "")
                {
                    MessageBox.Show("Enter Organisation ...", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtOrganisationName.Focus();
                }
                else
                {
                    if (MessageBox.Show("Do you want to save this record?", "SAVE CONFIRMATION", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (db.MASTERNAMESETUPs.Where(x => x.ORGANISATION_NAME == txtOrganisationName.Text).ToString() == txtOrganisationName.Text.ToString())
                        {
                            MessageBox.Show("'" + txtOrganisationName.Text + "' Already exist.Enter new relation.", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                            txtOrganisationName.Focus();
                        }
                        else
                        {
                            if (txtOrganisationName.Text != "")
                            {
                                MASTERNAMESETUP ms = db.MASTERNAMESETUPs.Where(x => x.ORGANISATION_NAME == txtOrganisationName.Text).FirstOrDefault();
                                var OldData = new JSonHelper().ConvertObjectToJSon(ms);

                                ms.ORGANISATION_NAME = txtOrganisationName.Text;
                                ms.ADDRESS1 = txtAddress1.Text;
                                ms.ADDRESS2 = txtAddress2.Text;
                                ms.ADDRESS3 = txtAddress3.Text;
                                ms.CITY = txtCity.Text;
                                ms.STATE = txtState.Text;
                                ms.COUNTRY = txtCountry.Text;
                                ms.PHONE = txtPhoneNo.Text;
                                ms.ZIPCODE = txtZipCode.Text;
                                ms.SERVERNAME = txtServerName.Text;
                                ms.NEWEXEPATH = txtExePath.Text;
                                ms.NEWIMAGESPATH = txtNewImagePath.Text;
                                ms.NEWREPORTSPATH = txtNewReportPath.Text;
                                ms.SERVERNAME = txtServerName.Text;
                                ms.VERSION = txtVersion.Text;


                                ms.EnterenceFees = Convert.ToDecimal(txtEntranceFeed.Text);
                                ms.BadgeAmount = Convert.ToDecimal(txtBadgeAmt.Text);
                                ms.BF = Convert.ToDecimal(txtBF.Text);
                                ms.BuildingFund = Convert.ToDecimal(txtBuildingFund.Text);
                                ms.Insurance = Convert.ToDecimal(txtInsurance.Text);
                                ms.Subscription = Convert.ToDecimal(txtSubscription.Text);

                                ms.LevyAmount = Convert.ToDecimal(txtLevyAmount.Text);
                                ms.TDFAmount = Convert.ToDecimal(txtTDFAmount.Text);
                                ms.RejoinAmount = Convert.ToDecimal(txtRejoinAmount.Text);

                                ms.SenderEmailId = txtEmailID.Text;
                                ms.SenderPassword = txtPassword.Password;

                                ms.CCEamilId1 = txtCCEmailID1.Text;
                                ms.CCEamilId2 = txtCCEmailID2.Text;

                                ms.PrinterName = txtPrinterName.Text;
                                ms.BackUpPath = txtPath.Text;                               

                                db.SaveChanges();

                                var NewData = new JSonHelper().ConvertObjectToJSon(ms);
                                AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "MASTERNAMESETUP");
                                MessageBox.Show("Saved Successfully!", "SAVED", MessageBoxButton.OK, MessageBoxImage.Information);
                                if (sFormName == "HOME")
                                {
                                    frmHomeMaster frm = new frmHomeMaster();                                    
                                    this.Close();
                                    frm.ShowDialog();
                                }
                                else
                                {
                                    this.Close();
                                }

                            }
                            else
                            {
                                MASTERNAMESETUP ms = new MASTERNAMESETUP();
                                ms.ORGANISATION_NAME = txtOrganisationName.Text;
                                ms.ADDRESS1 = txtAddress1.Text;
                                ms.ADDRESS2 = txtAddress2.Text;
                                ms.ADDRESS3 = txtAddress3.Text;
                                ms.CITY = txtCity.Text;
                                ms.STATE = txtState.Text;
                                ms.COUNTRY = txtCountry.Text;
                                ms.PHONE = txtPhoneNo.Text;
                                ms.ZIPCODE = txtZipCode.Text;
                                ms.SERVERNAME = txtServerName.Text;
                                ms.NEWEXEPATH = txtExePath.Text;
                                ms.NEWIMAGESPATH = txtNewImagePath.Text;
                                ms.NEWREPORTSPATH = txtNewReportPath.Text;
                                ms.SERVERNAME = txtServerName.Text;
                                ms.VERSION = txtVersion.Text;


                                ms.EnterenceFees = Convert.ToDecimal(txtEntranceFeed.Text);
                                ms.BadgeAmount = Convert.ToDecimal(txtBadgeAmt.Text);
                                ms.BF = Convert.ToDecimal(txtBF.Text);
                                ms.BuildingFund = Convert.ToDecimal(txtBuildingFund.Text);
                                ms.Insurance = Convert.ToDecimal(txtInsurance.Text);
                                ms.Subscription = Convert.ToDecimal(txtSubscription.Text);

                                ms.LevyAmount = Convert.ToDecimal(txtLevyAmount.Text);
                                ms.TDFAmount = Convert.ToDecimal(txtTDFAmount.Text);
                                ms.RejoinAmount = Convert.ToDecimal(txtRejoinAmount.Text);

                                ms.SenderEmailId = txtEmailID.Text;
                                ms.SenderPassword = txtPassword.Password;

                                ms.CCEamilId1 = txtCCEmailID1.Text;
                                ms.CCEamilId2 = txtCCEmailID2.Text;

                                ms.PrinterName = txtPrinterName.Text;
                                ms.BackUpPath = txtPath.Text;                                

                                db.MASTERNAMESETUPs.Add(ms);
                                db.SaveChanges();

                                var NewData = new JSonHelper().ConvertObjectToJSon(ms);
                                AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData, "MASTERNAMESETUP");
                                MessageBox.Show("Saved Successfully!", "SAVED", MessageBoxButton.OK, MessageBoxImage.Information);
                                if (sFormName == "HOME")
                                {
                                    frmHomeMaster frm = new frmHomeMaster();
                                    this.Close();
                                    frm.ShowDialog();
                                }
                                else
                                {
                                    this.Close();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sFormName == "HOME")
                {
                    frmHomeMaster frm = new frmHomeMaster();
                    this.Close();
                    frm.ShowDialog();
                }
                else
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void txtCCEmailID_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtPhoneNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtPhoneNo.Text.Length == 11)
            {
                txtPhoneNo.Text = NoFormate.sPhoneNo(txtPhoneNo.Text);
            }
        }

        private void txtPhoneNo_TextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtEntranceFeed_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtBuildingFund_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtBF_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtSubscription_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtInsurance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtBadgeAmt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtLevyAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtTDFAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtRejoinAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void btnOpenDialogBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.InitialDirectory = Environment.CurrentDirectory;
                dlg.Title = "NUBE Back Up";
                dlg.Filter = "All files|*.bak;*.BAK|All files|*.*";
                dlg.FileName = "NUBEBFSVII";
                if (dlg.ShowDialog() == true)
                {
                    txtPath.Text = dlg.FileName;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }
    }
}
