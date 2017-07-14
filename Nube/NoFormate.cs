using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nube
{
    class NoFormate
    {
        public static string sMobileNo(string MobileNo="")
        {
            string[] split = MobileNo.Split(new char[] { '-', '(', ')' });// remove all old format,if your phone number is like(001)123-456-789
            StringBuilder sb = new StringBuilder();
            foreach (string s in split)
            {
                if (s.Trim() != "")
                {
                    sb.Append(s);
                }
            }
            if (MobileNo.Length == 14)
            {
                MobileNo = String.Format("{0:0-000-0000000000}", double.Parse(sb.ToString()));
            }
            return MobileNo;
        }

        public static string sPhoneNo(string PhoneNo = "")
        {
            string[] split = PhoneNo.Split(new char[] { '-', '(', ')' });// remove all old format,if your phone number is like(001)123-456-789
            StringBuilder sb = new StringBuilder();
            foreach (string s in split)
            {
                if (s.Trim() != "")
                {
                    sb.Append(s);
                }
            }
            if (PhoneNo.Length == 11)
            {
                PhoneNo = String.Format("{0:0-00-00000000}", double.Parse(sb.ToString()));
            }
            return PhoneNo;
        }
    }
}
