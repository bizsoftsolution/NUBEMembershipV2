using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SL.Cross;
using DAL;

namespace SL.Controllers
{
    public class TestController : Controller
    {
        DAL.nubebfsEntities db = new DAL.nubebfsEntities();
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        [NubeCrossSiteAttribute]
        public JsonResult Attachment(int iApproveState)
        {
            try
            {
                DAL.MembershipAttachment d = new MembershipAttachment();
                
                return Json(new { ErrMsg = "No Records Found" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { isSaved = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}