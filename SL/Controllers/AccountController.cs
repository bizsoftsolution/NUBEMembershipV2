using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SL.Cross;

namespace SL.Controllers
{
    public class AccountController : Controller
    {
        DAL.nubebfsEntities db = new DAL.nubebfsEntities();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [NubeCrossSiteAttribute]
        public JsonResult Login(string UserId, string Password)
        {
            try
            {
                var l1 = db.UserAccounts.Where(x => x.UserName == UserId && x.Password == Password).FirstOrDefault();
                if (l1 != null)
                {
                    return Json(new { isSaved = true, Msg = "Valid User" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var l2 = db.UserAccounts.Where(x => x.UserName == UserId).FirstOrDefault();
                    if (l2 == null)
                    {
                        return Json(new { isSaved = false, Msg = "Invalid User" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { isSaved = false, Msg = "Invalid Password" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { isSaved = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }


        }
    }
}