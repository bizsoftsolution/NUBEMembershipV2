using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SL.Cross;

namespace SL.Controllers
{
    public class MasterBranchController : Controller
    {
        DAL.nubebfsEntities db = new DAL.nubebfsEntities();

        // GET: MasterBranch       
        public ActionResult Index()
        {
            return View();
        }

        [NubeCrossSiteAttribute]
        public JsonResult ToList(string term, int? Bank_Code)
        {
            var l1 = db.MASTERBANKBRANCHes.Where(x => x.DELETED == 0).OrderBy(x => x.BANKBRANCH_NAME).ToList();
            if (Bank_Code != null)
            {
                l1 = db.MASTERBANKBRANCHes.OrderBy(x => x.BANKBRANCH_NAME).Where(x => x.BANK_CODE == Bank_Code).ToList();
                if (!string.IsNullOrWhiteSpace(term))
                {
                    l1 = l1.Where(x => x.BANKBRANCH_NAME.ToLower().Contains(term.ToLower())).ToList();
                }
            }
            else if (!string.IsNullOrWhiteSpace(term))
            {
                l1 = l1.Where(x => x.BANKBRANCH_NAME.ToLower().Contains(term.ToLower())).ToList();
            }


            return Json(l1.Take(100).Select(x => new { x.BANKBRANCH_CODE, x.BANKBRANCH_NAME }).ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}