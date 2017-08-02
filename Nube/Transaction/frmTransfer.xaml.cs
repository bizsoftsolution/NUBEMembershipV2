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
using Microsoft.Win32;
using System.Net.Mail;
using Nube.MasterSetup;

namespace Nube.Transaction
{
    /// <summary>
    /// Interaction logic for frmTransfer.xaml
    /// </summary>
    public partial class frmTransfer : MetroWindow
    {
        decimal dMember_Code = 0;
        nubebfsEntity db = new nubebfsEntity();
        Boolean bValidation = false;
        public frmTransfer(decimal dMembercode = 0)
        {
            InitializeComponent();
            dMember_Code = dMembercode;

            FormLoad();
            if (dMember_Code != 0)
            {
                FormFill();
            }
        }

        #region "BUTTON EVENTS"

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            frmMemberQuery frm = new frmMemberQuery("Transfer");
            this.Close();
            frm.ShowDialog();
        }

        private void txtMemberNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(txtMemberNo.Text))
            {
                decimal dMemberId = Convert.ToDecimal(txtMemberNo.Text);
                var mm = (from x in db.MASTERMEMBERs where x.MEMBER_ID == dMemberId select x).FirstOrDefault();
                if (mm != null)
                {
                    dMember_Code = mm.MEMBER_CODE;
                    FormFill();
                }
                else
                {
                    MessageBox.Show("No Record Found!");
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bValidation = false;
                BeforeUpdate();
                if (bValidation == false)
                {
                    MASTERMEMBER mm = (from mas in db.MASTERMEMBERs where mas.MEMBER_CODE == dMember_Code select mas).FirstOrDefault();
                    var OldData = new JSonHelper().ConvertObjectToJSon(mm);

                    if (mm != null)
                    {
                        mm.BANK_CODE = Convert.ToDecimal(cmbBankCode.SelectedValue);
                        mm.BRANCH_CODE = Convert.ToDecimal(cmbBranchCode.SelectedValue);
                        db.SaveChanges();

                        var NewData = new JSonHelper().ConvertObjectToJSon(mm);
                        AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "MASTERMEMBER");

                        MemberTransfer mt = new MemberTransfer();
                        mt.EntryDate = DateTime.Now.Date;
                        mt.MemberCode = dMember_Code;
                        mt.BankCodeBF = Convert.ToDecimal(txtBankCode.Text);
                        mt.BranchCodeBF = Convert.ToDecimal(txtBranchCode.Text);
                        mt.BankCodeAF = Convert.ToDecimal(txtBankCode.Text);
                        mt.BranchCodeAF = Convert.ToDecimal(txtBranchCode.Text);
                        mt.UserId = AppLib.iUserCode;

                        db.MemberTransfers.Add(mt);
                        db.SaveChanges();

                        var NewData1 = new JSonHelper().ConvertObjectToJSon(mt);
                        AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData1, "MEMBERTRANSFER");


                        MessageBox.Show(txtMemberName.Text + " Member Has Tranfered Sucessfully");



                        if (MessageBox.Show("Do you want to Send Mail to NUBE?", "EMAIL Confirmation ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            //string sMemberMailID = mm.EMAIL.ToString();
                            if (mm.EMAIL == null)
                            {
                                MessageBox.Show("This Member Does Not Contain any Email Address!");
                                fNew();
                                return;
                            }
                            else
                            {
                                progressBar1.Minimum = 1;
                                progressBar1.Maximum = 10;
                                progressBar1.Visibility = Visibility.Visible;
                                MailMessage mail = new MailMessage();
                                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                                var mn = (from m in db.MASTERNAMESETUPs select m).FirstOrDefault();

                                mail.To.Add(mm.EMAIL.ToString());
                                if (mn != null)
                                {
                                    mail.CC.Add(mn.CCEamilId1.ToString());
                                    mail.CC.Add(mn.CCEamilId1.ToString());
                                }
                                mail.From = new MailAddress(mn.SenderEmailId.ToString());

                                mail.Subject = "NUBE TRANSFER MAIL";
                                mail.Body = "Dear Mr/Ms." + txtMemberName.Text.ToString() + ",  \r" +
                                           " Your Bank Transfer has Successfully Transfered \r\r" +
                                           " Given Below the Bank & Branch Details are  \r" +
                                           " From " + txtBankName.Text.ToString() + " - " + txtBranchName.Text.ToString() + "\r" +
                                           " To " + cmbBankName.Text.ToString() + " - " + cmbBranchName.Text.ToString();

                                progressBar1.Value = 2;
                                System.Windows.Forms.Application.DoEvents();
                                if (MessageBox.Show("Do you want to Attach any Files in this Email?", "EMAIL Attachment", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                    progressBar1.Value = 4;
                                    System.Windows.Forms.Application.DoEvents();
                                    OpenFileDialog OpenDialogBox = new OpenFileDialog();
                                    OpenDialogBox.DefaultExt = ".txt";
                                    OpenDialogBox.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                                    var browsefile = OpenDialogBox.ShowDialog();
                                    if (browsefile == true)
                                    {
                                        progressBar1.Value = 6;
                                        System.Windows.Forms.Application.DoEvents();
                                        System.Net.Mail.Attachment attachment;
                                        attachment = new System.Net.Mail.Attachment(OpenDialogBox.FileName.ToString());
                                        mail.Attachments.Add(attachment);
                                    }
                                }
                                progressBar1.Value = 7;
                                System.Windows.Forms.Application.DoEvents();
                                SmtpServer.Port = 587;
                                SmtpServer.Credentials = new System.Net.NetworkCredential(mn.SenderEmailId.ToString(), mn.SenderPassword.ToString());
                                SmtpServer.EnableSsl = true;
                                progressBar1.Value = 9;
                                System.Windows.Forms.Application.DoEvents();

                                SmtpServer.Send(mail);
                                progressBar1.Value = 10;
                                System.Windows.Forms.Application.DoEvents();
                                MessageBox.Show("Mail Send Sucessfully!");
                                progressBar1.Visibility = Visibility.Hidden;
                            }
                        }
                        fNew();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            frmHomeMembership frm = new frmHomeMembership();
            this.Close();
            frm.ShowDialog();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                fNew();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSetBank_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmBankSetup frm = new frmBankSetup();
                frm.ShowDialog();

                cmbBankCode.ItemsSource = db.MASTERBANKs.ToList();
                cmbBankCode.SelectedValuePath = "BANK_CODE";
                cmbBankCode.DisplayMemberPath = "BANK_USERCODE";

                cmbBankName.ItemsSource = db.MASTERBANKs.ToList();
                cmbBankName.SelectedValuePath = "BANK_CODE";
                cmbBankName.DisplayMemberPath = "BANK_NAME";

                decimal dBankCode = Convert.ToDecimal(cmbBankCode.SelectedValue);
                if (dBankCode != 0)
                {
                    var mBnk = db.MASTERBANKs.Where(x => x.BANK_CODE == dBankCode).FirstOrDefault();
                    cmbBankName.SelectedValue = mBnk.BANK_CODE;

                    var mbr = db.MASTERBANKBRANCHes.Where(x => x.BANK_CODE == dBankCode).ToList();
                    cmbBranchCode.ItemsSource = mbr;
                    cmbBranchCode.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranchCode.DisplayMemberPath = "BANKBRANCH_USERCODE";

                    cmbBranchName.ItemsSource = mbr;
                    cmbBranchName.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranchName.DisplayMemberPath = "BANKBRANCH_NAME";
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSetBranch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmBranchSetup frm = new frmBranchSetup("");
                frm.ShowDialog();

                cmbBankCode.ItemsSource = db.MASTERBANKs.ToList();
                cmbBankCode.SelectedValuePath = "BANK_CODE";
                cmbBankCode.DisplayMemberPath = "BANK_USERCODE";

                cmbBankName.ItemsSource = db.MASTERBANKs.ToList();
                cmbBankName.SelectedValuePath = "BANK_CODE";
                cmbBankName.DisplayMemberPath = "BANK_NAME";

                decimal dBankCode = Convert.ToDecimal(cmbBankCode.SelectedValue);
                if (dBankCode != 0)
                {
                    var mBnk = db.MASTERBANKs.Where(x => x.BANK_CODE == dBankCode).FirstOrDefault();
                    cmbBankName.SelectedValue = mBnk.BANK_CODE;

                    var mbr = db.MASTERBANKBRANCHes.Where(x => x.BANK_CODE == dBankCode).ToList();
                    cmbBranchCode.ItemsSource = mbr;
                    cmbBranchCode.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranchCode.DisplayMemberPath = "BANKBRANCH_USERCODE";

                    cmbBranchName.ItemsSource = mbr;
                    cmbBranchName.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranchName.DisplayMemberPath = "BANKBRANCH_NAME";
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }


        #endregion

        #region "COMBO BOX EVENTS"

        private void cmbBankCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                decimal dBankCode = Convert.ToDecimal(cmbBankCode.SelectedValue);
                if (dBankCode != 0)
                {
                    var mBnk = db.MASTERBANKs.Where(x => x.BANK_CODE == dBankCode).FirstOrDefault();
                    cmbBankName.SelectedValue = mBnk.BANK_CODE;

                    var mbr = db.MASTERBANKBRANCHes.Where(x => x.BANK_CODE == dBankCode).OrderBy(x => x.BANKBRANCH_NAME).ToList();

                    cmbBranchCode.ItemsSource = mbr;
                    cmbBranchCode.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranchCode.DisplayMemberPath = "BANKBRANCH_USERCODE";

                    cmbBranchName.ItemsSource = mbr;
                    cmbBranchName.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranchName.DisplayMemberPath = "BANKBRANCH_NAME";

                    txtCAddress1.Text = "";
                    txtCAddress2.Text = "";
                    txtCAddress3.Text = "";
                    txtCCity.Text = "";
                    txtCState.Text = "";
                    txtCZipCode.Text = "";
                    txtCCountry.Text = "";
                    txtCMobileNo.Text = "";
                    txtCPhoneNo.Text = "";
                    txtCEmail.Text = "";
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void cmbBankName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                decimal dBankCode = Convert.ToDecimal(cmbBankName.SelectedValue);
                if (dBankCode != 0)
                {
                    var mBnk = db.MASTERBANKs.Where(x => x.BANK_CODE == dBankCode).FirstOrDefault();
                    cmbBankCode.SelectedValue = mBnk.BANK_CODE;

                    var mbr = db.MASTERBANKBRANCHes.Where(x => x.BANK_CODE == dBankCode).OrderBy(x => x.BANKBRANCH_NAME).ToList();

                    cmbBranchCode.ItemsSource = mbr;
                    cmbBranchCode.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranchCode.DisplayMemberPath = "BANKBRANCH_USERCODE";

                    cmbBranchName.ItemsSource = mbr;
                    cmbBranchName.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranchName.DisplayMemberPath = "BANKBRANCH_NAME";

                    txtCAddress1.Text = "";
                    txtCAddress2.Text = "";
                    txtCAddress3.Text = "";
                    txtCCity.Text = "";
                    txtCState.Text = "";
                    txtCZipCode.Text = "";
                    txtCCountry.Text = "";
                    txtCMobileNo.Text = "";
                    txtCPhoneNo.Text = "";
                    txtCEmail.Text = "";
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void cmbBranchCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                decimal dBrnkCode = Convert.ToDecimal(cmbBranchCode.SelectedValue);
                if (dBrnkCode != 0)
                {
                    var mbr = db.MASTERBANKBRANCHes.Where(x => x.BANKBRANCH_CODE == dBrnkCode).FirstOrDefault();
                    cmbBranchName.SelectedValue = mbr.BANKBRANCH_CODE;

                    var brnch = (from bc in db.MASTERBANKBRANCHes
                                 join ct in db.MASTERCITies on bc.BANKBRANCH_CITY_CODE equals ct.CITY_CODE
                                 join st in db.MASTERSTATEs on bc.BANKBRANCH_STATE_CODE equals st.STATE_CODE
                                 where bc.BANKBRANCH_CODE == mbr.BANKBRANCH_CODE

                                 select new
                                 {
                                     bc.BANKBRANCH_ADDRESS1,
                                     bc.BANKBRANCH_ADDRESS2,
                                     bc.BANKBRANCH_ADDRESS3,
                                     ct.CITY_NAME,
                                     st.STATE_NAME,
                                     bc.BANKBRANCH_ZIPCODE,
                                     bc.BANKBRANCH_COUNTRY,
                                     bc.BANKBRANCH_PHONE1,
                                     bc.BANKBRANCH_PHONE2,
                                     bc.BANKBRANCH_EMAIL
                                 }
                          ).FirstOrDefault();
                    if (brnch != null)
                    {
                        txtCAddress1.Text = brnch.BANKBRANCH_ADDRESS1;
                        txtCAddress2.Text = brnch.BANKBRANCH_ADDRESS2;
                        txtCAddress3.Text = brnch.BANKBRANCH_ADDRESS3;
                        txtCCity.Text = brnch.CITY_NAME;
                        txtCState.Text = brnch.STATE_NAME;
                        txtCZipCode.Text = brnch.BANKBRANCH_ZIPCODE;
                        txtCCountry.Text = brnch.BANKBRANCH_COUNTRY;
                        txtCPhoneNo.Text = brnch.BANKBRANCH_PHONE1;
                        txtCMobileNo.Text = brnch.BANKBRANCH_PHONE2;
                        txtCEmail.Text = brnch.BANKBRANCH_EMAIL;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void cmbBranchName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                decimal dBrnkCode = Convert.ToDecimal(cmbBranchName.SelectedValue);
                if (dBrnkCode != 0)
                {
                    var mbr = db.MASTERBANKBRANCHes.Where(x => x.BANKBRANCH_CODE == dBrnkCode).FirstOrDefault();
                    cmbBranchCode.SelectedValue = mbr.BANKBRANCH_CODE;

                    var brnch = (from bc in db.MASTERBANKBRANCHes
                                 join ct in db.MASTERCITies on bc.BANKBRANCH_CITY_CODE equals ct.CITY_CODE
                                 join st in db.MASTERSTATEs on bc.BANKBRANCH_STATE_CODE equals st.STATE_CODE
                                 where bc.BANKBRANCH_CODE == mbr.BANKBRANCH_CODE

                                 select new
                                 {
                                     bc.BANKBRANCH_ADDRESS1,
                                     bc.BANKBRANCH_ADDRESS2,
                                     bc.BANKBRANCH_ADDRESS3,
                                     ct.CITY_NAME,
                                     st.STATE_NAME,
                                     bc.BANKBRANCH_ZIPCODE,
                                     bc.BANKBRANCH_COUNTRY,
                                     bc.BANKBRANCH_PHONE1,
                                     bc.BANKBRANCH_PHONE2,
                                     bc.BANKBRANCH_EMAIL
                                 }
                          ).FirstOrDefault();
                    if (brnch != null)
                    {
                        txtCAddress1.Text = brnch.BANKBRANCH_ADDRESS1;
                        txtCAddress2.Text = brnch.BANKBRANCH_ADDRESS2;
                        txtCAddress3.Text = brnch.BANKBRANCH_ADDRESS3;
                        txtCCity.Text = brnch.CITY_NAME;
                        txtCState.Text = brnch.STATE_NAME;
                        txtCZipCode.Text = brnch.BANKBRANCH_ZIPCODE;
                        txtCCountry.Text = brnch.BANKBRANCH_COUNTRY;
                        txtCPhoneNo.Text = brnch.BANKBRANCH_PHONE1;
                        txtCMobileNo.Text = brnch.BANKBRANCH_PHONE2;
                        txtCEmail.Text = brnch.BANKBRANCH_EMAIL;
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

        void FormLoad()
        {
            try
            {
                cmbBankCode.ItemsSource = db.MASTERBANKs.OrderBy(x => x.BANK_USERCODE).ToList();
                cmbBankCode.SelectedValuePath = "BANK_CODE";
                cmbBankCode.DisplayMemberPath = "BANK_USERCODE";

                cmbBankName.ItemsSource = db.MASTERBANKs.OrderBy(x => x.BANK_NAME).ToList();
                cmbBankName.SelectedValuePath = "BANK_CODE";
                cmbBankName.DisplayMemberPath = "BANK_NAME";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void FormFill()
        {
            try
            {
                var qry = (from mm in db.MASTERMEMBERs
                           join mt in db.MASTERMEMBERTYPEs on mm.MEMBERTYPE_CODE equals mt.MEMBERTYPE_CODE
                           join bk in db.MASTERBANKs on mm.BANK_CODE equals bk.BANK_CODE
                           where mm.MEMBER_CODE == dMember_Code
                           select new
                           {
                               mt.MEMBERTYPE_NAME,
                               mm.MEMBER_CODE,
                               mm.MEMBER_NAME,
                               mm.DATEOFBIRTH,
                               mm.AGE_IN_YEARS,
                               mm.SEX,
                               mm.ICNO_OLD,
                               mm.ICNO_NEW,
                               mm.DATEOFJOINING,
                               bk.BANK_CODE,
                               bk.BANK_NAME,
                               bk.BANK_USERCODE,
                               mm.BRANCH_CODE,
                               mm.MEMBER_ID
                           }
                         ).FirstOrDefault();

                var brnch = (from bc in db.MASTERBANKBRANCHes
                             join ct in db.MASTERCITies on bc.BANKBRANCH_CITY_CODE equals ct.CITY_CODE
                             join st in db.MASTERSTATEs on bc.BANKBRANCH_STATE_CODE equals st.STATE_CODE
                             where bc.BANKBRANCH_CODE == qry.BRANCH_CODE
                             select new
                             {
                                 bc.BANKBRANCH_CODE,
                                 bc.BANKBRANCH_USERCODE,
                                 bc.BANKBRANCH_NAME,
                                 bc.BANKBRANCH_ADDRESS1,
                                 bc.BANKBRANCH_ADDRESS2,
                                 bc.BANKBRANCH_ADDRESS3,
                                 ct.CITY_NAME,
                                 st.STATE_NAME,
                                 bc.BANKBRANCH_ZIPCODE,
                                 bc.BANKBRANCH_COUNTRY,
                                 bc.BANKBRANCH_PHONE1,
                                 bc.BANKBRANCH_PHONE2,
                                 bc.BANKBRANCH_EMAIL
                             }
                             ).FirstOrDefault();
                if (qry != null)
                {
                    txtMemberType.Text = qry.MEMBERTYPE_NAME;
                    txtMemberNo.Text = qry.MEMBER_ID.ToString();
                    txtMemberName.Text = qry.MEMBER_NAME;
                    dtpDOB.SelectedDate = Convert.ToDateTime(qry.DATEOFBIRTH);
                    txtAge.Text = qry.AGE_IN_YEARS.ToString();
                    txtGender.Text = qry.SEX;
                    txtNewIC.Text = qry.ICNO_NEW;
                    txtOldIC.Text = qry.ICNO_OLD;
                    dtpDOJ.SelectedDate = Convert.ToDateTime(qry.DATEOFJOINING);

                    txtBankCode.Text = qry.BANK_USERCODE.ToString();
                    txtBankName.Text = qry.BANK_NAME;
                }

                if (brnch != null)
                {
                    txtBranchCode.Text = brnch.BANKBRANCH_USERCODE.ToString();
                    txtBranchName.Text = brnch.BANKBRANCH_NAME;
                    txtAddress1.Text = brnch.BANKBRANCH_ADDRESS1;
                    txtAddress2.Text = brnch.BANKBRANCH_ADDRESS2;
                    txtAddress3.Text = brnch.BANKBRANCH_ADDRESS3;
                    txtCity.Text = brnch.CITY_NAME;
                    txtState.Text = brnch.STATE_NAME;
                    txtZipCode.Text = brnch.BANKBRANCH_ZIPCODE;
                    txtCountry.Text = brnch.BANKBRANCH_COUNTRY;
                    txtPhoneNo.Text = brnch.BANKBRANCH_PHONE1;
                    txtMobileNo.Text = brnch.BANKBRANCH_PHONE2;
                    txtEmail.Text = brnch.BANKBRANCH_EMAIL;

                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        void fNew()
        {
            txtMemberType.Text = "";
            txtMemberNo.Text = "";
            txtMemberName.Text = "";
            txtAge.Text = "";
            txtState.Text = "";
            txtOldIC.Text = "";
            txtNewIC.Text = "";
            dtpDOB.Text = "";
            dtpDOJ.Text = "";

            txtBankCode.Text = "";
            txtBankName.Text = "";
            txtBranchCode.Text = "";

            txtBranchName.Text = "";
            txtAddress1.Text = "";
            txtAddress2.Text = "";
            txtAddress3.Text = "";
            txtCity.Text = "";
            txtState.Text = "";
            txtZipCode.Text = "";
            txtCountry.Text = "";
            txtPhoneNo.Text = "";
            txtMobileNo.Text = "";
            txtEmail.Text = "";

            cmbBankCode.Text = "";
            cmbBankName.Text = "";
            cmbBranchCode.Text = "";
            cmbBranchName.Text = "";
            txtCAddress1.Text = "";
            txtCAddress2.Text = "";
            txtCAddress3.Text = "";
            txtCCity.Text = "";
            txtCState.Text = "";
            txtCZipCode.Text = "";
            txtCCountry.Text = "";
            txtCPhoneNo.Text = "";
            txtCMobileNo.Text = "";
            txtCEmail.Text = "";
            dMember_Code = 0;
            FormLoad();
        }

        void BeforeUpdate()
        {
            if (string.IsNullOrEmpty(txtMemberNo.Text))
            {
                MessageBox.Show("Membership No is Empty!");
                txtMemberNo.Focus();
                bValidation = true;
                return;
            }
            else if (string.IsNullOrEmpty(cmbBankCode.Text))
            {
                MessageBox.Show("Bank Code is Empty!");
                cmbBankCode.Focus();
                bValidation = true;
                return;
            }
            else if (string.IsNullOrEmpty(cmbBankName.Text))
            {
                MessageBox.Show("Bank Name is Empty!");
                cmbBankName.Focus();
                bValidation = true;
                return;
            }
            else if (string.IsNullOrEmpty(cmbBranchCode.Text))
            {
                MessageBox.Show("Branch Code is Empty!");
                cmbBankName.Focus();
                bValidation = true;
                return;
            }
            else if (string.IsNullOrEmpty(cmbBranchName.Text))
            {
                MessageBox.Show("Branch Name is Empty!");
                cmbBranchName.Focus();
                bValidation = true;
                return;
            }
            else if (dMember_Code == 0)
            {
                MessageBox.Show("Please Select Any Member");
                btnSelect.Focus();
                bValidation = true;
                return;
            }
        }

        #endregion

    }
}
