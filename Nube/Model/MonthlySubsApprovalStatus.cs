using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nube.Model
{
    public class MonthlySubsApprovalStatus
    {
        public MonthlySubsApprovalStatus()
        {

        }

        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private int approved;

        public int Approved
        {
            get { return approved; }
            set { approved = value; }
        }

        private int pending;

        public int Pending
        {
            get { return pending; }
            set { pending = value; }
        }

        private int noOfMember;

        public int NoOfMember
        {
            get { return noOfMember; }
            set { noOfMember = value; }
        }


    }
}
