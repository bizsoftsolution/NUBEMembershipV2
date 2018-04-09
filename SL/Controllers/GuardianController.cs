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
                DAL.GuardianInsertBranch nm = new DAL.GuardianInsertBranch();

                nm.MEMBER_CODE = wm.MEMBER_CODE;
                nm.ICNO_OLD = wm.ICNO_OLD;
                nm.ICNO_NEW = wm.ICNO_NEW;
                nm.NAME = wm.NAME;
                nm.SEX = wm.SEX;
                nm.AGE = wm.AGE;
                nm.RELATION_CODE = wm.RELATION_CODE;
                nm.ADDRESS1 = wm.ADDRESS1;
                nm.ADDRESS2 = wm.ADDRESS2;
                nm.ADDRESS3 = wm.ADDRESS3;
                nm.CITY_CODE = wm.CITY_CODE;
                nm.STATE_CODE = wm.STATE_CODE;
                nm.COUNTRY = wm.COUNTRY;
                nm.ZIPCODE = wm.ZIPCODE;
                nm.PHONE = wm.PHONE;
                nm.MOBILE = wm.MOBILE;
                nm.USER_CODE = wm.USER_CODE;
                nm.ENTRY_DATE = wm.ENTRY_DATE;
                nm.ENTRY_TIME = wm.ENTRY_TIME;

                db.GuardianInsertBranches.Add(nm);
                db.SaveChanges();

                return Json(new { isSaved = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isSaved = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}