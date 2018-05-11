using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows.Input;
using System.Windows.Forms;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Nube
{
    public static class AppLib
    {
        public static string sProjectName = "";
        public static int iUserCode = 0;
        public static Boolean bIsSuperAdmin = false;
        public static string sAccFundName = "";
        public static string AppName = "";
        public static string DBBFS = "nubebfs";
        public static string DBStatus = "nubestatus";
        public static string SLURL = "http://localhost/MembershipTest/";
        public static int iFundId = 0;
        //public static NubeAccountReport frmNubeAccountReport = new NubeAccountReport();
        public static string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["NUBEBFS"].ConnectionString;
        public static string connstatus = System.Configuration.ConfigurationManager.ConnectionStrings["NUBESTATUS"].ConnectionString;
        public static List<UserPrevilage> lstUsreRights = new List<UserPrevilage>();
        public static List<TVMASTERMEMBER> lstTVMasterMember = new List<TVMASTERMEMBER>();
        public static List<ViewMasterMember> lstMstMember = new List<ViewMasterMember>();        

        public static DataTable dtMemberQuery = new DataTable();        
        public static DataTable dtAnnualStatement = new DataTable();
        public static nubebfsEntity db = new nubebfsEntity();

        public static void CheckIsNumeric(TextCompositionEventArgs e)
        {
            try
            {
                Regex regex = new Regex("[^0-9.9]+");
                e.Handled = regex.IsMatch(e.Text);
                //int result;
                //if (!(int.TryParse(e.Text, out result) || e.Text == "."))
                //{
                //    e.Handled = true;
                //}
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        public static void EventHistory(string FormName = "", int Event = 0, string OldData = "", string NewData = "", string TblName = "", string Remarks = "")
        {
            try
            {
                EventHistory eh = new EventHistory();
                eh.UserId = AppLib.iUserCode;
                eh.FormName = FormName;
                eh.Event = Event;
                eh.UpdatedOn = DateTime.Now;
                eh.BeforeData = OldData;
                eh.ModifiedData = NewData;
                eh.TableName = TblName;
                eh.Remarks = Remarks;

                db.EventHistories.Add(eh);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        public static int MonthDiff(this DateTime date1, DateTime date2)
        {
            return (int)((date1.Year - date2.Year) * 12) + (date1.Month - date2.Month);
        }

        public static DataTable LINQResultToDataTable<T>(IEnumerable<T> Linqlist)
        {
            DataTable dt = new DataTable();

            PropertyInfo[] columns = null;

            if (Linqlist == null) return dt;

            foreach (T Record in Linqlist)
            {

                if (columns == null)
                {
                    columns = ((Type)Record.GetType()).GetProperties();
                    foreach (PropertyInfo GetProperty in columns)
                    {
                        Type colType = GetProperty.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dt.Columns.Add(new DataColumn(GetProperty.Name, colType));
                    }
                }

                DataRow dr = dt.NewRow();

                foreach (PropertyInfo pinfo in columns)
                {
                    dr[pinfo.Name] = pinfo.GetValue(Record, null) == null ? DBNull.Value : pinfo.GetValue
                    (Record, null);
                }

                dt.Rows.Add(dr);
            }
            return dt;
        }

    }
}
