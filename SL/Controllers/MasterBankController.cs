using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SL.Cross;

namespace SL.Controllers
{
    public class MasterBankController : Controller
    {
        // GET: MasterBank
        DAL.nubebfsEntities db = new DAL.nubebfsEntities();
        public ActionResult Index()
        {
            return View();
        }

        [NubeCrossSiteAttribute]

        public JsonResult ToList(string term)
        {
            var l1 = db.MASTERBANKs.OrderBy(x => x.BANK_NAME).ToList();
            if (!string.IsNullOrWhiteSpace(term))
            {
                l1 = l1.Where(x => x.BANK_NAME.ToLower().Contains(term.ToLower())).ToList();
            }
            return Json(l1, JsonRequestBehavior.AllowGet);
        }
    }
}