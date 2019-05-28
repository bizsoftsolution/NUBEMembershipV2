using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nube.Model
{
    public class MonthlySubsMemberApproval
    {
        private long id;

        public long Id
        {
            get { return id; }
            set { id = value; }
        }

        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private bool isApproved;

        public bool IsApproved
        {
            get { return isApproved; }
            set { isApproved = value; }
        }

        private string approvalBy;

        public string ApprovalBy
        {
            get { return approvalBy; }
            set { approvalBy = value; }
        }


        private int monthlySubsMatchingTypeId;

        public int MonthlySubsMatchingTypeId
        {
            get { return monthlySubsMatchingTypeId; }
            set { monthlySubsMatchingTypeId = value; }
        }

    }
}
