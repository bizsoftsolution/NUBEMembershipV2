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

namespace Nube.Transaction
{
    /// <summary>
    /// Interaction logic for frmMonthlySubscriptionMemberApproval.xaml
    /// </summary>
    public partial class frmMonthlySubscriptionMemberApproval : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        decimal monthlySubsMemberId;
        public frmMonthlySubscriptionMemberApproval(long MonthlySubsMemberId)
        {
            InitializeComponent();
            monthlySubsMemberId = MonthlySubsMemberId;
            LoadData();
        }
        void LoadData()
        {
            var lst = db.MonthlySubscriptionMemberMatchingResults.Where(x => x.MonthlySubscriptionMemberId == monthlySubsMemberId)
                                    .Select(x => new Model.MonthlySubsMemberApproval()
                                    {
                                        Id = x.Id,
                                        Description = x.MonthlySubscriptionMatchingType.Name + "\r\n" + x.Description,
                                        IsApproved = x.UserAccount == null ? false : true,
                                        ApprovalBy = x.UserAccount == null ? "" : x.UserAccount.UserName,
                                        MonthlySubsMatchingTypeId= x.MonthlySubscriptionMatchingTypeId
                                    }).ToList();
            dgvMemberMatching.ItemsSource = lst;
            HideUpdateBox();
        }
        void HideUpdateBox()
        {
            grdMismatchName.Visibility = Visibility.Collapsed;
        }
        private void dgvMemberMatching_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {

                HideUpdateBox();
                var mm = dgvMemberMatching.SelectedItem as Model.MonthlySubsMemberApproval;
                if (mm != null)
                {
                    if(mm.MonthlySubsMatchingTypeId == (int)AppLib.MonthlySubscriptionMatchingType.MismatchedMemberName)
                    {
                        grdMismatchName.Visibility = Visibility.Visible;
                        var d = db.MonthlySubscriptionMemberMatchingResults.FirstOrDefault(x => x.Id == mm.Id);
                        txtNameFromBank.Text = d.MonthlySubscriptionMember.MemberName;
                        txtNameFromNUBE.Text = d.MonthlySubscriptionMember.MASTERMEMBER.MEMBER_NAME;
                    }                    
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void dgvMemberMatching_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                e.Row.Header = (e.Row.GetIndex() + 1).ToString();
            }
            catch (Exception ex) { }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var mm = dgvMemberMatching.SelectedItem as Model.MonthlySubsMemberApproval;
                if (mm != null)
                {
                    var d = db.MonthlySubscriptionMemberMatchingResults.FirstOrDefault(x => x.Id == mm.Id);
                    d.ApprovedBy = AppLib.iUserCode;
                    db.SaveChanges();
                    LoadData();
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var mm = dgvMemberMatching.SelectedItem as Model.MonthlySubsMemberApproval;
                if (mm != null)
                {
                    var d = db.MonthlySubscriptionMemberMatchingResults.FirstOrDefault(x => x.Id == mm.Id);
                    d.ApprovedBy = null;
                    db.SaveChanges();
                    LoadData();
                }
            }
            catch(Exception ex)
            {

            }
            
        }

        private void BtnUpdateMemberName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(MessageBox.Show("Do you want to update member name?","Member Update", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var mm = dgvMemberMatching.SelectedItem as Model.MonthlySubsMemberApproval;
                    if (mm != null)
                    {
                        if (mm.MonthlySubsMatchingTypeId == (int)AppLib.MonthlySubscriptionMatchingType.MismatchedMemberName)
                        {
                            var d = db.MonthlySubscriptionMemberMatchingResults.FirstOrDefault(x => x.Id == mm.Id);
                            MonthlySubscriptionMemberUpdate data = new MonthlySubscriptionMemberUpdate()
                            {
                                MemberCode = d.MonthlySubscriptionMember.MemberCode,
                                MonthlySubscriptionMatchingTypeId = d.MonthlySubscriptionMatchingTypeId,
                                OldValue = txtNameFromNUBE.Text,
                                NewValue = txtNameFromBank.Text,
                                UpdateBy = AppLib.iUserCode,
                                UpdateAt = DateTime.Now
                            };
                            db.MonthlySubscriptionMemberUpdates.Add(data);
                            db.MonthlySubscriptionMemberMatchingResults.Remove(d);
                            db.SaveChanges();
                            LoadData();
                            MessageBox.Show("Updated");
                        }
                    }
                }                
            }
            catch(Exception ex)
            {
                MessageBox.Show("Not Updated");
            }
        }
    }
}
