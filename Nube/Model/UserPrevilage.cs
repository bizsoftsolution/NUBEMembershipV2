using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nube
{
    public partial class UserPrevilage
    {    
        public UserPrevilage()
        {

        }    
        public UserPrevilage(string FormName)
        {
            var up = AppLib.lstUsreRights.Where(x => x.ProjectName == AppLib.sProjectName && x.FormName == FormName).FirstOrDefault();
            if (up != null)
            {
                this.AddNew = up.AddNew;
                this.Edit = up.Edit;
                this.Show = up.Show;
                this.Remove = up.Remove;
               
            }
        }
    }
}
