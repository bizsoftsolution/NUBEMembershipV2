using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace Nube.Transaction
{
    /// <summary>
    /// Interaction logic for frmArrearEntry.xaml
    /// </summary>
    public partial class frmArrearEntry : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        List<ArrearEntry> lstArrearEntry = new List<ArrearEntry>();
        public frmArrearEntry()
        {
            InitializeComponent();
        }

        #region "BUTTON EVENTS"

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtMemberNo.Text))
                {
                    decimal dMemberNo = Convert.ToDecimal(txtMemberNo.Text);
                    var mm = (from x in db.MASTERMEMBERs where x.MEMBER_ID == dMemberNo select x).FirstOrDefault();
                    if (mm != null)
                    {
                        decimal dTotalAmount = Convert.ToDecimal(txtTotalAmount.Text);
                        decimal dTotalMonths = Convert.ToDecimal(txtTotalMonthsPaid.Text);

                        ArrearEntry ar = new ArrearEntry();
                        ar.MEMBERCODE = mm.MEMBER_CODE;
                        ar.MEMBERID = mm.MEMBER_ID;
                        ar.NRIC = mm.ICNO_NEW == null ? mm.ICNO_OLD : mm.ICNO_NEW;
                        ar.MEMBERNAME = mm.MEMBER_NAME;
                        ar.TOTALMONTHSPAID = dTotalMonths;
                        ar.TOTALAMOUNT = dTotalAmount;
                        ar.AMOUNTBF = dTotalMonths * 3;
                        ar.SUBS = dTotalAmount - (dTotalMonths * 3);
                        lstArrearEntry.Add(ar);
                        DataTable dt = AppLib.LINQResultToDataTable(lstArrearEntry);
                        int i = 0;
                        dTotalAmount = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            i++;
                            dr["ID"] = i;
                            dTotalAmount = dTotalAmount + Convert.ToInt32(dr["TOTALAMOUNT"]);
                        }
                        dgFeeDetails.ItemsSource = dt.DefaultView;
                        txtTotal.Text = dTotalAmount.ToString();
                        FormNew();
                    }
                    else
                    {
                        MessageBox.Show("No Record Found!");
                        txtMemberNo.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter the Membership No!");
                    txtMemberNo.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FormNew();
                lstArrearEntry.Clear();
                dgFeeDetails.ItemsSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(dtDate.Text))
                {
                    MessageBox.Show("Please Select the Date!");
                    dtDate.Focus();
                    return;
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt = ((DataView)dgFeeDetails.ItemsSource).ToTable();
                    if (dt.Rows.Count > 0)
                    {
                        var FeeMst = (from x in db.FeesMasters where x.XLFileName == "Arrear Entry" && x.FeeDate == dtDate.SelectedDate select x).FirstOrDefault();

                        if (FeeMst != null)
                        {
                            if (MessageBox.Show("This Month Arrear Details are already in DB, Do You want to Save Once Again ?", "Save Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                db.FeesMasters.Remove(FeeMst);
                                db.SaveChanges();

                                var fdtl = (from x in db.FeesDetails where x.FeeId == FeeMst.FeeId select x).ToList();
                                if (fdtl != null)
                                {
                                    db.FeesDetails.RemoveRange(db.FeesDetails.Where(x => x.FeeId == FeeMst.FeeId));
                                    db.SaveChanges();
                                }
                            }
                        }
                        FeesMaster FeesMst = new FeesMaster
                        {
                            BankId = 0,
                            FeeDate = Convert.ToDateTime(dtDate.SelectedDate).Date,
                            XLFileName = "Arrear Entry",
                            XLPassword = "",
                            UpdatedStatus = "Not Updated",
                        };
                        db.FeesMasters.Add(FeesMst);
                        db.SaveChanges();

                        var sFid = db.FeesMasters.Max(x => x.FeeId).ToString();

                        List<FeesDetail> lstFeesDetls = new List<FeesDetail>();
                        progressBar1.Minimum = 0;
                        progressBar1.Maximum = dt.Rows.Count;
                        progressBar1.Visibility = Visibility.Visible;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            progressBar1.Value = i;
                            System.Windows.Forms.Application.DoEvents();
                            FeesDetail FeesDtl = new FeesDetail
                            {
                                FeeId = Convert.ToInt32(sFid),
                                MemberCode = Convert.ToDecimal(dt.Rows[i]["MEMBERCODE"]),
                                TotalAmount = Convert.ToDecimal(dt.Rows[i]["TOTALAMOUNT"]),
                                AmountBF = Convert.ToDecimal(dt.Rows[i]["AMOUNTBF"]),
                                AmountIns = 0,
                                AmtSubs = Convert.ToDecimal(dt.Rows[i]["SUBS"]),
                                Dept = "",
                                UpdatedStatus = "Not Updated",
                                FeeYear = Convert.ToInt32(Convert.ToDateTime(dtDate.SelectedDate).Year),
                                FeeMonth = Convert.ToInt32(Convert.ToDateTime(dtDate.SelectedDate).Month),
                                Reason = "",
                                IsUnPaid = false,
                                IsNotMatch = false,
                                MemberId = Convert.ToDecimal(dt.Rows[i]["MEMBERID"]),
                                Status = "Arrear Entry",
                                TotalMonthsPaid = Convert.ToInt32(dt.Rows[i]["TOTALMONTHSPAID"])
                            };
                            lstFeesDetls.Add(FeesDtl);
                        }

                        if (lstFeesDetls != null)
                        {
                            db.FeesDetails.AddRange(lstFeesDetls);
                            db.SaveChanges();

                            MessageBox.Show("Details Saved Successfull!");
                            FormNew();
                            lstArrearEntry.Clear();
                            dgFeeDetails.ItemsSource = null;
                            dtDate.Text = "";
                        }
                    }
                    else
                    {
                        MessageBox.Show("No Record Found!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            frmHomeMembership frm = new frmHomeMembership();
            frm.Show();
            this.Close();
        }

        private void txtMemberNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtTotalAmount.Focus();
            }
        }

        private void txtTotalAmount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtTotalMonthsPaid.Focus();
            }
        }

        private void txtTotalMonthsPaid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnAdd_Click("", e);
                txtMemberNo.Focus();
            }
        }

        private void dgFeeDetails_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (dgFeeDetails.SelectedItem != null)
                {
                    int dTotalAmount = 0;
                    int i = 0;
                    DataTable dt = new DataTable();
                    dt = ((DataView)dgFeeDetails.ItemsSource).ToTable();
                    foreach (DataRow dr in dt.Rows)
                    {
                        i++;
                        dr["ID"] = i;
                        dTotalAmount = dTotalAmount + Convert.ToInt32(dr["TOTALAMOUNT"]);
                    }
                    dgFeeDetails.ItemsSource = dt.DefaultView;
                    txtTotal.Text = dTotalAmount.ToString();
                }
            }
        }

        #endregion

        #region "USER DEFINED FUNCTIONS"

        class ArrearEntry
        {
            public int ID { get; set; }
            public Nullable<decimal> MEMBERCODE { get; set; }
            public Nullable<decimal> MEMBERID { get; set; }
            public string NRIC { get; set; }
            public string MEMBERNAME { get; set; }
            public Nullable<decimal> TOTALMONTHSPAID { get; set; }
            public Nullable<decimal> TOTALAMOUNT { get; set; }
            public Nullable<decimal> AMOUNTBF { get; set; }
            public Nullable<decimal> SUBS { get; set; }
        }

        void FormNew()
        {
            txtMemberNo.Text = "";
            txtTotalMonthsPaid.Text = "";
            txtTotalAmount.Text = "";
        }

        #endregion
    }
}
