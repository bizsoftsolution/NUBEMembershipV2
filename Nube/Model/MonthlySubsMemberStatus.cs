using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nube.Model
{
    public class MonthlySubsMemberStatus
    {
        public MonthlySubsMemberStatus()
        {

        }

        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private int noOfMember;

        public int NoOfMember
        {
            get { return noOfMember; }
            set { noOfMember = value; }
        }

        private decimal amount;

        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }

    }
}
