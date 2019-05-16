using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nube.Model
{
    public class MonthlySubs
    {

        public MonthlySubs()
        {
            DB = new nubebfsEntity();
            selectedDate = DateTime.Now.Date;
        }

        public MonthlySubs(DateTime dt)
        {
            DB = new nubebfsEntity();
            selectedDate = dt;
        }

        private nubebfsEntity db;
        public nubebfsEntity DB
        {
            get
            {
                if (db == null) db = new nubebfsEntity();
                return db;
            }
            set { db = value; }
        }


        private DateTime selectedDate;
        public DateTime SelecctedDate
        {
            get { return selectedDate; }
            set
            {
                if (selectedDate != value)
                {
                    selectedDate = value;
                    //MonthlySubscription = db.MonthlySubscriptions.Where(x=>)
                }
            }
        }

        private MonthlySubscription monthly;

        public MonthlySubscription MonthlySubscription
        {
            get { return monthly; }
            set { monthly = value; }
        }

    }
}
