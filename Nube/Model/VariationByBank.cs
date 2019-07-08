using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nube.Model
{
    public class VariationByBank
    {
        private int msBankIdCurrent;

        public int MSBankIdCureent
        {
            get { return msBankIdCurrent; }
            set { msBankIdCurrent = value; }
        }

        private int msBankIdPrevious;

        public int MSBankIdPrevious
        {
            get { return msBankIdPrevious; }
            set { msBankIdPrevious = value; }
        }
        
        private string bankName;
        public string BankName
        {
            get { return bankName; }
            set { bankName = value; }
        }

        private int noOfMemberPrevious;

        public int NoOfMemberPrevious
        {
            get { return noOfMemberPrevious; }
            set { noOfMemberPrevious = value; }
        }

        private int noOfMemberCurrent;

        public int NoOfMemberCurrent
        {
            get { return noOfMemberCurrent; }
            set { noOfMemberCurrent = value; }
        }

        private decimal totalAmountPrevious;

        public decimal TotalAmountPrevious
        {
            get { return totalAmountPrevious; }
            set { totalAmountPrevious = value; }
        }

        private decimal totalAmountCurrent;

        public decimal TotalAmountCurrent
        {
            get { return totalAmountCurrent; }
            set { totalAmountCurrent = value; }
        }


        private int different;

        public int Different
        {
            get { return different; }
            set { different = value; }
        }


        private int unpaid;

        public int Unpaid
        {
            get { return unpaid; }
            set { unpaid = value; }
        }

        private int newPaid;

        public int NewPaid
        {
            get { return newPaid; }
            set { newPaid = value; }
        }

        private string unpaidNRIC;

        public string UnpaidNRIC
        {
            get { return unpaidNRIC; }
            set { unpaidNRIC = value; }
        }


        private string newPaidNRIC;

        public string NewPaidNRIC
        {
            get { return newPaidNRIC; }
            set { newPaidNRIC = value; }
        }


        private int sno;

        public int SNo
        {
            get { return sno; }
            set { sno = value; }
        }

        private string nric;

        public string NRIC
        {
            get { return nric; }
            set { nric = value; }
        }

        private string memberName;

        public string Membername
        {
            get { return memberName; }
            set { memberName = value; }
        }

        private decimal amount;

        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        private string varStatus;

        public string VarStatus
        {
            get { return varStatus; }
            set { varStatus = value; }
        }

        private string msStatus;

        public string MSStatus
        {
            get { return msStatus; }
            set { msStatus = value; }
        }

    }
}
