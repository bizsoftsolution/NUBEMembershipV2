using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nube.Model
{
    public class MonthlySubsBank
    {
        public MonthlySubsBank()
        {

        }

        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string bankName;

        public string BankName
        {
            get { return bankName; }
            set { bankName = value; }
        }


        private int noOfMember;

        public int NoOfMember
        {
            get { return noOfMember; }
            set { noOfMember = value; }
        }

        private decimal totalAmount;

        public decimal TotalAmount
        {
            get { return totalAmount; }
            set { totalAmount = value; }
        }

        private decimal activeAmount;

        public decimal ActiveAmount
        {
            get { return activeAmount; }
            set { activeAmount = value; }
        }

        private decimal defaulterAmount;

        public decimal DefaulterAmount
        {
            get { return defaulterAmount; }
            set { defaulterAmount = value; }
        }

        private decimal struckOffAmount;

        public decimal StruckOffAmount
        {
            get { return struckOffAmount; }
            set { struckOffAmount = value; }
        }

        private decimal resignedAmount;

        public decimal ResignedAmount
        {
            get { return resignedAmount; }
            set { resignedAmount = value; }
        }

        private decimal sundryCreditorAmount;

        public decimal SundryCreditorAmount
        {
            get { return sundryCreditorAmount; }
            set { sundryCreditorAmount = value; }
        }



    }
}
