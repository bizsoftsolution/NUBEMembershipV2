using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SL.Cross;

namespace SL.Controllers
{
    public class MasterRelationController : Controller
    {
        DAL.nubebfsEntities db = new DAL.nubebfsEntities();
        // GET: MasterRelation
        public ActionResult Index()
        {
            return View();
        }

        [NubeCrossSiteAttribute]

        public JsonResult ToList(string term)
        {
            var l1 = db.MASTERRELATIONs.OrderBy(x => x.RELATION_NAME).ToList();
            if (!string.IsNullOrWhiteSpace(term))
            {
                l1 = l1.Where(x => x.RELATION_NAME.ToLower().Contains(term.ToLower())).ToList();
            }
            return Json(l1, JsonRequestBehavior.AllowGet);
        }       
    }
}