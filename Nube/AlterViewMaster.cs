using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nube
{
    class AlterView
    {
        public static void ViewMemberTotalMonthsDue(int i = 0)
        {
            //int iMonth = 12 + i;

            //string str = " ALTER VIEW [dbo].[ViewTotalMonthsDue]\r" +
            //             " AS\r" +
            //             " SELECT MEMBER_CODE,LASTPAIDDATE, TOTALMOTHSDUE,TOTALDUE,TOTALDUEPAY FROM\r" +
            //             " (SELECT MM.MEMBER_CODE, ISNULL(LD.LDATE, MM.LASTPAYMENT_DATE) AS LASTPAIDDATE,\r" +
            //             " CASE WHEN '2017-03-31 12:00:00.000' > MM.DATEOFJOINING THEN\r" +
            //             " (DATEDIFF(MM, MM.DATEOFJOINING, GETDATE()) + 1) - (MM.TOTALMONTHSPAID + ISNULL(LD.TOTALDUEPAY, 0))\r" +
            //             " ELSE 0 END TOTALMOTHSDUE,\r" +
            //             " (DATEDIFF(MM, MM.DATEOFJOINING, GETDATE()) + 1)TOTALDUE,\r" +
            //             " (MM.TOTALMONTHSPAID + ISNULL(LD.TOTALDUEPAY, 0))TOTALDUEPAY\r" +
            //             " FROM DBO.MASTERMEMBER AS MM WITH(NOLOCK)\r" +
            //             " LEFT OUTER JOIN DBO.VIEWMEMBERLASTPAIDDATE AS LD WITH(NOLOCK) ON LD.MEMBER_CODE = MM.MEMBER_CODE\r" +
            //             " LEFT JOIN FEESDETAILS FD(NOLOCK) ON FD.MEMBERCODE = LD.MEMBER_CODE\r" +
            //             " WHERE(MM.RESIGNED = 0) AND FD.ISNOTMATCH = 0 AND ISSTRUCKOFF = 0\r" +
            //             " )  AS T1\r" +
            //             " WHERE DATEDIFF(MM, LASTPAIDDATE, GETDATE())<= " + iMonth + "\r" +
            //             " GROUP BY MEMBER_CODE,LASTPAIDDATE, TOTALMOTHSDUE,TOTALDUE,TOTALDUEPAY";


            //using (SqlConnection con = new SqlConnection(AppLib.connStr))
            //{
            //    SqlCommand cmd = new SqlCommand(str, con);
            //    cmd.Connection.Open();
            //    cmd.CommandTimeout = 0;
            //    cmd.ExecuteNonQuery();
            //}
        }

        public static void ViewMasterMember(int i = 0)
        {
            //int iMonth = 3 + i;
            //string str = string.Format("ALTER VIEW [DBO].[VIEWMASTERMEMBER]\r" +
            //        " AS\r" +
            //        " SELECT  MM.MEMBER_CODE,MM.MEMBER_NAME,MM.MEMBER_ID,MM.MEMBER_TITLE,ISNULL(MM.MEMBER_INITIAL, '')MEMBER_INITIAL,\r" +
            //        " ISNULL(MM.ICNO_OLD, '')ICNO_OLD,ISNULL(MM.ICNO_NEW, '')ICNO_NEW,MB.BANK_NAME,MB.BANK_USERCODE,ISNULL(MM.ADDRESS1, '')ADDRESS1,\r" +
            //        " ISNULL(MM.ADDRESS2, '')ADDRESS2,ISNULL(MM.ADDRESS3, '')ADDRESS3, MC.CITY_NAME,MS.STATE_NAME,ISNULL(MM.COUNTRY, '')COUNTRY,\r" +
            //        " ISNULL(MM.ZIPCODE, '')ZIPCODE,ISNULL(MM.PHONE, '')PHONE,ISNULL(MM.MOBILE, 0)MOBILE,ISNULL(MM.EMAIL, '')EMAIL,MM.DATEOFJOINING,\r" +
            //        " MM.DATEOFBIRTH,MM.AGE_IN_YEARS,MM.DATEOFEMPLOYMENT,MT.MEMBERTYPE_NAME,MM.SEX,MR.RACE_NAME,MM.REJOINED,MST.STATUS_NAME,\r" +
            //        " (MM.TOTALMONTHSPAID + SUM(ISNULL(FD.TOTALMONTHSPAID, 0))) TOTALMONTHSPAID,MM.ENTRANCEFEE,MM.HQFEE,MM.MONTHLYBF,\r" +
            //        " MM.MONTHLYSUBSCRIPTION,(MM.ACCBF + SUM(ISNULL(FD.AMOUNTBF, 0)))ACCBF,(MM.ACCSUBSCRIPTION + SUM(ISNULL(FD.AMTSUBS, 0)))ACCSUBSCRIPTION,\r" +
            //        " MM.ACCBENEFIT,MM.CURRENT_YTDBF,MM.CURRENT_YTDSUBSCRIPTION,MM.RESIGNED,ISNULL(VT.LASTPAIDDATE, MM.LASTPAYMENT_DATE)LASTPAYMENT_DATE,\r" +
            //        " MM.STRUCKOFF,MM.BANK_CODE,MM.BRANCH_CODE,MM.CITY_CODE,MM.STATE_CODE,MM.MEMBERTYPE_CODE,MM.RACE_CODE,MM.STATUS_CODE,\r" +
            //        " VB.BRANCHNAME,VB.ADDRESS1 AS BRANCHADR1,VB.ADDRESS2 AS BRANCHADR2,VB.ADDRESS3 AS BRANCHADR3,VB.CITYNAME AS BRANCHCITY,\r" +
            //        " VB.STATENAME AS BRANCHSTATE, VB.BRANCHUSERCODE,                                            	   \r" +
            //        " (CASE WHEN MM.RESIGNED <> 0 THEN 'RESIGNED'\r" +
            //        " ELSE CASE WHEN '2017-03-31 12:00:00.000' > MM.DATEOFJOINING THEN\r" +
            //        " CASE WHEN VT.TOTALMOTHSDUE IS NULL THEN 'STRUCKOFF'\r" +
            //        " WHEN VT.TOTALMOTHSDUE <= {0} THEN 'ACTIVE'\r" +
            //        " ELSE 'DEFAULTER' END\r" +
            //        " ELSE 'ACTIVE' END END   ) MEMBERSTATUS,                                            \r" +
            //        " (CASE WHEN MM.RESIGNED <> 0 THEN 6\r" +
            //        " ELSE CASE WHEN '2017-03-31 12:00:00.000' > MM.DATEOFJOINING THEN\r" +
            //        " CASE WHEN VT.TOTALMOTHSDUE IS NULL THEN 3\r" +
            //        " WHEN VT.TOTALMOTHSDUE <= {0} THEN 1\r" +
            //        " ELSE 2 END\r" +
            //        " ELSE 1 END END) MEMBERSTATUSCODE,                                                                        \r" +
            //        " DATEDIFF(MM, MM.DATEOFJOINING, GETDATE()) - MM.TOTALMONTHSPAID + 1 MONDUE,VB.NUBEBANCHCODE NUBEBRANCH, VB.ZIPCODE BRANCHZIPCODE,\r" +
            //        " VB.NUBEBANCHNAME,VB.NUBEBANCHCODE,MM.BATCHAMT,DUE.TOTALMOTHSDUE,ISNULL(MM.LEVY, '')LEVY,ISNULL(MM.MEMBERNAME_BYBANK, '')MEMBERNAME_BYBANK,\r" +
            //        " ISNULL(MM.NRIC_BYBANK, '')NRIC_BYBANK,ISNULL(MM.BANKCODE_BYBANK, 0)BANKCODE_BYBANK,ISNULL(MM.LEVY_AMOUNT, 0)LEVY_AMOUNT,\r" +
            //        " ISNULL(MM.TDF_AMOUNT, 0)TDF_AMOUNT,CASE WHEN ISNULL(MEMBER_INITIAL, '') = 'TDF' THEN 'YES' ELSE 'NO' END TDF,\r" +
            //        " ISNULL(RG.RESIGNATION_DATE, '')RESIGNATION_DATE,ISNULL(RG.ENTRY_DATE, '')ENTRY_DATE\r" +
            //        " FROM MASTERMEMBER MM(NOLOCK)\r" +
            //        " LEFT JOIN FEESDETAILS FD(NOLOCK) ON FD.MEMBERCODE = MM.MEMBER_CODE\r" +
            //        " LEFT JOIN MASTERBANK MB(NOLOCK) ON MB.BANK_CODE = MM.BANK_CODE      \r" +
            //        " LEFT JOIN MASTERCITY MC(NOLOCK) ON MC.CITY_CODE = MM.CITY_CODE      \r" +
            //        " LEFT JOIN MASTERSTATE MS(NOLOCK) ON MS.STATE_CODE = MM.STATE_CODE      \r" +
            //        " LEFT JOIN MASTERMEMBERTYPE MT(NOLOCK) ON MT.MEMBERTYPE_CODE = MM.MEMBERTYPE_CODE     \r" +
            //        " LEFT JOIN MASTERRACE MR(NOLOCK) ON MR.RACE_CODE = MM.RACE_CODE      \r" +
            //        " LEFT JOIN MASTERSTATUS MST(NOLOCK) ON MST.STATUS_CODE = MM.STATUS_CODE\r" +
            //        " LEFT JOIN VIEWBANKBRANCH VB(NOLOCK) ON MM.BRANCH_CODE = VB.BRANCHCODE      \r" +
            //        " LEFT JOIN VIEWMEMBERTOTALMONTHSDUE VT(NOLOCK) ON VT.MEMBER_CODE = MM.MEMBER_CODE      \r" +
            //        " LEFT JOIN VIEWTOTALDUE DUE(NOLOCK) ON DUE.MEMBER_CODE = MM.MEMBER_CODE      \r" +
            //        " LEFT JOIN RESIGNATION RG(NOLOCK) ON RG.MEMBER_CODE = MM.MEMBER_CODE      \r" +
            //        " GROUP BY MM.MEMBER_CODE, MM.MEMBER_NAME, MM.MEMBER_ID, MM.MEMBER_TITLE, MM.MEMBER_INITIAL, MM.ICNO_OLD, MM.ICNO_NEW, MB.BANK_NAME,\r" +
            //        " MB.BANK_USERCODE, MM.ADDRESS1, MM.ADDRESS2, MM.ADDRESS3, MC.CITY_NAME, MS.STATE_NAME, MM.COUNTRY, MM.ZIPCODE, MM.PHONE,\r" +
            //        " MM.MOBILE, MM.EMAIL, MM.DATEOFJOINING, MM.DATEOFBIRTH, MM.AGE_IN_YEARS, MM.DATEOFEMPLOYMENT, MT.MEMBERTYPE_NAME, MM.SEX,\r" +
            //        " MR.RACE_NAME, MM.REJOINED, MST.STATUS_NAME, MM.TOTALMONTHSPAID, MM.ENTRANCEFEE, MM.HQFEE, MM.MONTHLYBF, MM.MONTHLYSUBSCRIPTION,\r" +
            //        " MM.ACCBF, MM.ACCSUBSCRIPTION, MM.ACCBENEFIT, MM.CURRENT_YTDBF, MM.CURRENT_YTDSUBSCRIPTION, MM.RESIGNED, VT.LASTPAIDDATE,\r" +
            //        " MM.LASTPAYMENT_DATE, MM.STRUCKOFF, MM.BANK_CODE, MM.BRANCH_CODE, MM.CITY_CODE, MM.STATE_CODE, MM.MEMBERTYPE_CODE, MM.RACE_CODE,\r" +
            //        " MM.STATUS_CODE, VB.BRANCHNAME, VB.ADDRESS1, VB.ADDRESS2, VB.ADDRESS3, VB.CITYNAME, VB.STATENAME, VB.BRANCHUSERCODE, VT.TOTALMOTHSDUE,\r" +
            //        " MM.DATEOFJOINING, VB.NUBEBANCHCODE, VB.ZIPCODE, VB.NUBEBANCHNAME, VB.NUBEBANCHCODE, MM.BATCHAMT, DUE.TOTALMOTHSDUE, MM.LEVY,\r" +
            //        " MM.MEMBERNAME_BYBANK, MM.NRIC_BYBANK, MM.BANKCODE_BYBANK, MM.LEVY_AMOUNT, MM.TDF_AMOUNT, MM.TDF, RG.RESIGNATION_DATE, RG.ENTRY_DATE", iMonth);
            //using (SqlConnection con = new SqlConnection(AppLib.connStr))
            //{
            //    SqlCommand cmd = new SqlCommand(str, con);
            //    cmd.Connection.Open();
            //    cmd.CommandTimeout = 0;
            //    cmd.ExecuteNonQuery();
            //}
        }

        public static void ExecuteSPREFRESH()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppLib.connStr))
                {
                    SqlCommand cmd = new SqlCommand("SPREFRESH", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    System.Windows.Forms.Application.DoEvents();
                    cmd.Connection.Open();
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        public static void DefaultMemberTotalMonthsDue()
        {
         
        }

        public static void DefaultMasterMember()
        {            
        }
    }
}

