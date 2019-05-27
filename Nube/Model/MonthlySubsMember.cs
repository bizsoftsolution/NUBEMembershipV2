using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nube.Model
{
    public class MonthlySubsMember
    {

        private long id;

        public long Id
        {
            get { return id; }
            set { id = value; }
        }
        
        private string memberName;

        public string MemberName
        {
            get { return memberName; }
            set { memberName = value; }
        }

        private decimal? memberCode;

        public decimal? Membercode
        {
            get { return memberCode; }
            set { memberCode = value; }
        }

        private decimal? memberId;

        public decimal? MemberId
        {
            get { return memberId; }
            set { memberId = value; }
        }

        private string bankUserCode;

        public string BankUserCode
        {
            get { return bankUserCode; }
            set { bankUserCode = value; }
        }

        private string nric;

        public string NRIC
        {
            get { return nric; }
            set { nric = value; }
        }

        private decimal amount;

        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        private string memberStatus;

        public string MemberStatus
        {
            get { return memberStatus; }
            set { memberStatus = value; }
        }


        private int memberStatusId;

        public int MemberStatusId
        {
            get { return memberStatusId; }
            set { memberStatusId = value; }
        }

        private int isApproved;

        public int IsApproved
        {
            get { return isApproved; }
            set { isApproved = value; }
        }

        private decimal? dueMonth;

        public decimal? DueMonth
        {
            get { return dueMonth; }
            set { dueMonth = value; }
        }


    }
}
