using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nube.Model
{
    public class VariationByBank
    {
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

    }
}
