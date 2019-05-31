using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nube.Model
{
    public class MonthEndClosed : INotifyPropertyChanged
    {
        #region Property  Changed Event

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }


        private void NotifyAllPropertyChanged()
        {
            foreach (var p in this.GetType().GetProperties()) NotifyPropertyChanged(p.Name);
        }

        #endregion


        private decimal bankCode;

        public decimal BankCode
        {
            get { return bankCode; }
            set { if (bankCode != value) { bankCode = value; NotifyPropertyChanged(nameof(BankCode)); } }
        }

        private String bankName;

        public String BankName
        {
            get { return bankName; }
            set { if (bankName != value) { bankName = value;NotifyPropertyChanged(nameof(BankName)); } }
        }


        private int paidA;

        public int PaidA
        {
            get { return paidA; }
            set { if (paidA != value) { paidA = value;NotifyPropertyChanged(nameof(PaidA)); } }
        }

        private int paidD;

        public int PaidD
        {
            get { return paidD; }
            set { if (paidD != value) { paidD = value;NotifyPropertyChanged(nameof(PaidD)); } }
        }

        private int paid;

        public int Paid
        {
            get { return paid; }
            set { if (paid != value) { paid = value;NotifyPropertyChanged(nameof(Paid)); } }
        }

        private int unpaidA;

        public int UnpaidA
        {
            get { return unpaidA; }
            set { if (unpaidA != value) { unpaidA = value;NotifyPropertyChanged(nameof(UnpaidA)); } }
        }

        private int unpaidD;

        public int UnpaidD
        {
            get { return unpaidD; }
            set { if(unpaidD != value){ unpaidD = value;NotifyPropertyChanged(nameof(UnpaidD)); } }
        }

        private int unpaid;

        public int Unpaid
        {
            get { return unpaid; }
            set { if (unpaid != value) { unpaid = value;NotifyPropertyChanged(nameof(Unpaid)); } }
        }


        private int total;

        public int Total
        {
            get { return total; }
            set { if (total != value) { total = value;NotifyPropertyChanged(nameof(Total)); } }
        }

        private int closed;

        public int Closed
        {
            get { return closed; }
            set { if (closed != value) { closed = value;NotifyPropertyChanged(nameof(Closed)); NotifyPropertyChanged(nameof(Status)); } }
        }        
        
        public String Status
        {
            get { return Total == Closed ? "Closed" : Closed>0? "Closing" : "Waiting"; }
        }

    }
}
