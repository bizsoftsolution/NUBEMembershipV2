using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SL.Cross;

namespace SL.Controllers
{
    public class GuardianController : Controller
    {
        DAL.nubebfsEntities db = new DAL.nubebfsEntities();
        // GET: Guardian
        public ActionResult Index()
        {
            return View();
        }


        [NubeCrossSiteAttribute]
        public JsonResult Insert(DAL.GuardianInsertBranch wm)
        {
            try
            {

                return Json(new { isSaved = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isSaved = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}