using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nube.Model
{
    public class MonthSubsUnPaidMember
    {
        private decimal memberCode;

        public decimal MemberCode
        {
            get { return memberCode; }
            set { memberCode = value; }
        }

        private decimal memberId;

        public decimal MemberId
        {
            get { return memberId; }
            set { memberId = value; }
        }

        private String memberName;

        public String MemberName
        {
            get { return memberName; }
            set { memberName = value; }
        }

        private String nric;

        public String NRIC
        {
            get { return nric; }
            set { nric = value; }
        }

        private DateTime lastPaid;

        public DateTime LastPaid
        {
            get { return lastPaid; }
            set { lastPaid = value; }
        }

        private String status;

        public String Status
        {
            get { return status; }
            set { status = value; }
        }

        private decimal memberStatusId;

        public decimal MemberStatusId
        {
            get { return memberStatusId; }
            set { memberStatusId = value; }
        }

        private Boolean isPaid;

        public Boolean IsPaid
        {
            get { return isPaid; }
            set { isPaid = value; }
        }


        private String bankUserCode;

        public String BankUserCode
        {
            get { return bankUserCode; }
            set { bankUserCode = value; }
        }


    }
}
