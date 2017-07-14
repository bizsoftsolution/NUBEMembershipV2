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
    /// Interaction logic for frmTDF.xaml
    /// </summary>
    public partial class frmTDF : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        decimal dMember_Code = 0;
        Boolean bValidation = false;
        int iTotalMonthsPaid = 0;

        public frmTDF(decimal dMembercode = 0)
        {
            InitializeComponent();
            dMember_Code = dMembercode;
            txtMemberNo.Focus();
            if (dMember_Code != 0)
            {
                FormFill();
            }
        }

        #region "BUTTON EVENTS"

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            txtMemberNo.Text = "";
            frmMemberQuery frm = new frmMemberQuery("TDF");
            this.Close();
            frm.ShowDialog();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                fClear();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
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
                    MASTERMEMBER mmMst = (from mas in db.MASTERMEMBERs where mas.MEMBER_CODE == dMember_Code select mas).FirstOrDefault();
                    var OldData = new JSonHelper().ConvertObjectToJSon(mmMst);
                    mmMst.Tdf_PaymentDate = Convert.ToDateTime(dtpPaymentDate.SelectedDate);
                    mmMst.TDF_AMOUNT = Convert.ToDecimal(txtAmount.Text);
                    db.SaveChanges();                    

                    var NewData = new JSonHelper().ConvertObjectToJSon(mmMst);
                    AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "MASTERMEMBER","TDF MANUAL ENTRY");
                    MessageBox.Show("Saved Sucessfully");
                    fClear();
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

        private void txtMemberNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    txtAmount.Text = "";
                    dMember_Code = 0;
                    dtpPaymentDate.Text = "";
                    FormFill();
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
                if (!string.IsNullOrEmpty(txtMemberNo.Text))
                {
                    decimal dtxtMember_ID = Convert.ToDecimal(txtMemberNo.Text);
                    var qry = (from mm in db.MASTERMEMBERs
                               join mt in db.MASTERMEMBERTYPEs on mm.MEMBERTYPE_CODE equals mt.MEMBERTYPE_CODE
                               join bk in db.MASTERBANKs on mm.BANK_CODE equals bk.BANK_CODE
                               join bc in db.MASTERBANKBRANCHes on mm.BRANCH_CODE equals bc.BANKBRANCH_CODE
                               join rc in db.MASTERRACEs on mm.RACE_CODE equals rc.RACE_CODE
                               where mm.MEMBER_ID == dtxtMember_ID
                               select new
                               {
                                   mm.MEMBER_ID,
                                   mt.MEMBERTYPE_NAME,
                                   mm.MEMBER_CODE,
                                   mm.MEMBER_NAME,
                                   mm.DATEOFBIRTH,
                                   mm.AGE_IN_YEARS,
                                   rc.RACE_NAME,
                                   mm.SEX,
                                   mm.ICNO_OLD,
                                   mm.ICNO_NEW,
                                   mm.DATEOFJOINING,
                                   bk.BANK_CODE,
                                   bk.BANK_NAME,
                                   mm.BRANCH_CODE,
                                   bc.BANKBRANCH_NAME,
                                   mm.LASTPAYMENT_DATE,
                                   mm.TOTALMONTHSPAID,
                                   mm.TDF_AMOUNT,
                                   mm.Tdf_PaymentDate,
                                   mm.TDF
                               }
                         ).FirstOrDefault();
                    if (qry != null)
                    {
                        dMember_Code = qry.MEMBER_CODE;
                        txtMemberType.Text = qry.MEMBERTYPE_NAME;
                        txtMemberNo.Text = qry.MEMBER_ID.ToString();
                        txtMemberName.Text = qry.MEMBER_NAME;
                        dtpDOB.SelectedDate = qry.DATEOFBIRTH;
                        txtAge.Text = qry.AGE_IN_YEARS.ToString();
                        txtRace.Text = qry.RACE_NAME.ToString();

                        txtAmount.Text = qry.TDF_AMOUNT.ToString();
                        dtpPaymentDate.SelectedDate = qry.Tdf_PaymentDate;

                        txtGender.Text = qry.SEX;
                        txtBankCode.Text = qry.BANK_CODE.ToString();
                        txtBankName.Text = qry.BANK_NAME.ToString();
                        txtBranchCode.Text = qry.BRANCH_CODE.ToString();
                        txtBranchName.Text = qry.BANKBRANCH_NAME;
                        txtOldIC.Text = qry.ICNO_OLD;
                        txtNewIC.Text = qry.ICNO_NEW;
                        dtpDOJ.SelectedDate = qry.DATEOFJOINING;
                        DateTime dt = Convert.ToDateTime(qry.LASTPAYMENT_DATE);
                        txtLastPaymentDate.Text = dt.Day.ToString() + "-" + dt.Month.ToString() + "-" + dt.Year.ToString();
                        DateTime dJoiningDate = Convert.ToDateTime(qry.DATEOFJOINING);
                        DateTime dToday = DateTime.Now;
                        int iTotalMonth = Convert.ToInt32(((dToday.Year - dJoiningDate.Year) * 12) + dToday.Month - dJoiningDate.Month);
                        int iBalanceDue = (iTotalMonth - Convert.ToInt32(qry.TOTALMONTHSPAID));
                        iTotalMonthsPaid = Convert.ToInt32(qry.TOTALMONTHSPAID);
                        txtBalanceDue.Text = iBalanceDue.ToString();
                    }
                }
                else if (dMember_Code != 0)
                {
                    var qry = (from mm in db.MASTERMEMBERs
                               join mt in db.MASTERMEMBERTYPEs on mm.MEMBERTYPE_CODE equals mt.MEMBERTYPE_CODE
                               join bk in db.MASTERBANKs on mm.BANK_CODE equals bk.BANK_CODE
                               join bc in db.MASTERBANKBRANCHes on mm.BRANCH_CODE equals bc.BANKBRANCH_CODE
                               join rc in db.MASTERRACEs on mm.RACE_CODE equals rc.RACE_CODE
                               where mm.MEMBER_CODE == dMember_Code
                               select new
                               {
                                   mt.MEMBERTYPE_NAME,
                                   mm.MEMBER_ID,
                                   mm.MEMBER_NAME,
                                   mm.DATEOFBIRTH,
                                   mm.AGE_IN_YEARS,
                                   rc.RACE_NAME,
                                   mm.SEX,
                                   mm.ICNO_OLD,
                                   mm.ICNO_NEW,
                                   mm.DATEOFJOINING,
                                   bk.BANK_CODE,
                                   bk.BANK_NAME,
                                   mm.BRANCH_CODE,
                                   bc.BANKBRANCH_NAME,
                                   mm.LASTPAYMENT_DATE,
                                   mm.TOTALMONTHSPAID,
                                   mm.TDF_AMOUNT,
                                   mm.Tdf_PaymentDate,
                                   mm.TDF
                               }
                         ).FirstOrDefault();
                    if (qry != null)
                    {
                        txtMemberType.Text = qry.MEMBERTYPE_NAME;
                        txtMemberNo.Text = qry.MEMBER_ID.ToString();
                        txtMemberName.Text = qry.MEMBER_NAME;
                        dtpDOB.SelectedDate = qry.DATEOFBIRTH;
                        txtAge.Text = qry.AGE_IN_YEARS.ToString();
                        txtRace.Text = qry.RACE_NAME.ToString();

                        txtAmount.Text = qry.TDF_AMOUNT.ToString();
                        dtpPaymentDate.SelectedDate = qry.Tdf_PaymentDate;

                        txtGender.Text = qry.SEX;
                        txtBankCode.Text = qry.BANK_CODE.ToString();
                        txtBankName.Text = qry.BANK_NAME.ToString();
                        txtBranchCode.Text = qry.BRANCH_CODE.ToString();
                        txtBranchName.Text = qry.BANKBRANCH_NAME;
                        txtOldIC.Text = qry.ICNO_OLD;
                        txtNewIC.Text = qry.ICNO_NEW;
                        dtpDOJ.SelectedDate = qry.DATEOFJOINING;
                        DateTime dt = Convert.ToDateTime(qry.LASTPAYMENT_DATE);
                        txtLastPaymentDate.Text = dt.Day.ToString() + "-" + dt.Month.ToString() + "-" + dt.Year.ToString();
                        DateTime dJoiningDate = Convert.ToDateTime(qry.DATEOFJOINING);
                        DateTime dToday = DateTime.Now;
                        int iTotalMonth = Convert.ToInt32(((dToday.Year - dJoiningDate.Year) * 12) + dToday.Month - dJoiningDate.Month);
                        int iBalanceDue = (iTotalMonth - Convert.ToInt32(qry.TOTALMONTHSPAID));
                        iTotalMonthsPaid = Convert.ToInt32(qry.TOTALMONTHSPAID);
                        txtBalanceDue.Text = iBalanceDue.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }  

        void fClear()
        {
            txtMemberType.Text = "";
            txtMemberNo.Text = "";
            txtMemberName.Text = "";
            dtpDOB.Text = "";
            txtAge.Text = "";

            txtGender.Text = "";
            txtBankCode.Text = "";
            txtBankName.Text = "";
            txtBranchCode.Text = "";
            txtBranchName.Text = "";
            txtOldIC.Text = "";
            txtNewIC.Text = "";
            dtpDOJ.Text = "";
            txtBalanceDue.Text = "";
            txtLastPaymentDate.Text = "";

            txtAmount.Text = "";
            dtpPaymentDate.Text = "";
            txtAmount.Text = "";
            dMember_Code = 0;
            txtRace.Text = "";
            txtMemberNo.Focus();
        }

        void BeforeUpdate()
        {
            if (dMember_Code == 0)
            {
                MessageBox.Show("Please Select Any Member!");
                txtMemberNo.Focus();
                bValidation = true;
            }
            else if (string.IsNullOrEmpty(txtAmount.Text))
            {
                MessageBox.Show("Please Enter the Amount!");
                txtAmount.Focus();
                bValidation = true;
            }
            else if (Convert.ToInt32(txtAmount.Text) == 0)
            {
                MessageBox.Show("Please Enter Valid Amount!");
                txtAmount.Focus();
                bValidation = true;
            }
            else if (string.IsNullOrEmpty(dtpPaymentDate.Text))
            {
                MessageBox.Show("Please Enter Payment Date!");
                dtpPaymentDate.Focus();
                bValidation = true;
            }
        }

        #endregion

    }
}
