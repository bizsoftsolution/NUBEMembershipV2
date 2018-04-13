using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SL.Cross;
using DAL;
using System.IO;

namespace SL.Controllers
{
    public class MasterMemberController : Controller
    {
        DAL.nubebfsEntities db = new DAL.nubebfsEntities();
        // GET: MasterMember
        public ActionResult Index()
        {
            return View();
        }

        [NubeCrossSiteAttribute]
        public JsonResult Insert(MemberInsertBranch wm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(wm.MEMBER_NAME))
                {
                    return Json(new { isSaved = false, ErrMsg = "Member Name is Empty" }, JsonRequestBehavior.AllowGet);
                }
                else if (string.IsNullOrWhiteSpace(wm.ICNO_NEW))
                {
                    return Json(new { isSaved = false, ErrMsg = "ICNO is Empty" }, JsonRequestBehavior.AllowGet);
                }
                else if (wm.BANK_CODE == 0 || string.IsNullOrWhiteSpace(wm.BANK_CODE.ToString()))
                {
                    return Json(new { isSaved = false, ErrMsg = "Bank is Empty" }, JsonRequestBehavior.AllowGet);
                }
                else if (wm.BANKBRANCH_CODE == 0 || string.IsNullOrWhiteSpace(wm.BANKBRANCH_CODE.ToString()))
                {
                    return Json(new { isSaved = false, ErrMsg = "Branch is Empty" }, JsonRequestBehavior.AllowGet);
                }
                else if (wm.Salary == 0 || string.IsNullOrWhiteSpace(wm.Salary.ToString()))
                {
                    return Json(new { isSaved = false, ErrMsg = "Salary is Empty" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    int i = 0;
                    var mn = db.MASTERNAMESETUPs.FirstOrDefault();
                    decimal age = 0;
                    if (wm.DATEOFBIRTH != null)
                    {
                        TimeSpan ts = DateTime.Now - Convert.ToDateTime(wm.DATEOFBIRTH);
                        age = Convert.ToInt32(ts.Days) / 365;
                    }

                    DAL.MemberInsertBranch mm = new DAL.MemberInsertBranch();

                    mm.MEMBERTYPE_CODE = wm.MEMBERTYPE_CODE;
                    mm.MEMBER_ID = 0;
                    mm.MEMBER_TITLE = wm.MEMBER_TITLE;
                    mm.MEMBER_NAME = wm.MEMBER_NAME;
                    mm.DATEOFBIRTH = wm.DATEOFBIRTH;
                    mm.AGE_IN_YEARS = age;
                    mm.SEX = wm.SEX;
                    mm.REJOINED = wm.REJOINED;
                    mm.RACE_CODE = wm.RACE_CODE;
                    mm.ICNO_NEW = wm.ICNO_NEW;
                    mm.ICNO_OLD = wm.ICNO_OLD;
                    mm.DATEOFJOINING = DateTime.Today;
                    mm.BANK_CODE = wm.BANK_CODE;
                    mm.BANKBRANCH_CODE = wm.BANKBRANCH_CODE;
                    mm.DATEOFEMPLOYMENT = wm.DATEOFEMPLOYMENT;
                    mm.Salary = wm.Salary;
                    mm.LEVY = "N/A";
                    mm.TDF = "N/A";
                    mm.LEVY_AMOUNT = 0;
                    mm.TDF_AMOUNT = 0;
                    //mm.LevyPaymentDate = DateTime.Now;
                    //mm.Tdf_PaymentDate = DateTime.Now;

                    if (mn != null)
                    {
                        if (wm.REJOINED == 1)
                        {
                            mm.ENTRANCEFEE = mn.EnterenceFees + mn.RejoinAmount;
                        }
                        else
                        {
                            mm.ENTRANCEFEE = mn.EnterenceFees;
                        }

                        mm.MONTHLYBF = mn.BF;
                        mm.ACCBF = mn.BF;
                        mm.CURRENT_YTDBF = mn.BF;

                        decimal dSalary = Convert.ToDecimal(wm.Salary);
                        if (dSalary > 0)
                        {
                            decimal dAmount = ((dSalary * Convert.ToDecimal(mn.Subscription)) / 100);
                            decimal dSubscription = dAmount - Convert.ToDecimal(mn.BF + mn.Insurance);

                            mm.MONTHLYSUBSCRIPTION = dSubscription;
                            mm.ACCSUBSCRIPTION = dSubscription;
                            mm.CURRENT_YTDSUBSCRIPTION = dSubscription;
                        }
                    }

                    mm.ACCBENEFIT = 0;
                    mm.TOTALMONTHSPAID = 1;
                    mm.ADDRESS1 = wm.ADDRESS1;
                    mm.ADDRESS2 = wm.ADDRESS2;
                    mm.ADDRESS3 = wm.ADDRESS3;
                    mm.PHONE = wm.PHONE;
                    mm.MOBILE = wm.MOBILE;
                    mm.EMAIL = wm.EMAIL;
                    mm.CITY_CODE = wm.CITY_CODE;
                    mm.ZIPCODE = wm.ZIPCODE;
                    mm.STATE_CODE = wm.STATE_CODE;
                    mm.COUNTRY = wm.COUNTRY;
                    mm.UpdatedBy = 1;
                    mm.UpdatedOn = DateTime.Now;
                    mm.IsApproved = 0;
                    mm.Occupation = wm.Occupation;

                    db.MemberInsertBranches.Add(mm);
                    db.SaveChanges();

                    return Json(new
                    {
                        isSaved = true,
                        MEMBER_CODE = mm.MEMBER_CODE
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return Json(new { isSaved = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [NubeCrossSiteAttribute]
        public JsonResult ToList(int iApproveState)
        {
            try
            {
                var l1 = db.SPMEMBERSHIPTOLIST(iApproveState).ToList();
                if (l1 != null)
                {
                    return Json(l1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { ErrMsg = "No Records Found" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { isSaved = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [NubeCrossSiteAttribute]
        public JsonResult Approve(decimal MemberCode)
        {
            try
            {
                var wm = (from x in db.MemberInsertBranches where x.MEMBER_CODE == MemberCode select x).FirstOrDefault();
                if (wm != null)
                {
                    decimal dMemberId = Convert.ToDecimal(db.MASTERMEMBERs.Max(x => x.MEMBER_ID)) + 1;

                    MASTERMEMBER mm = new MASTERMEMBER();

                    mm.MEMBERTYPE_CODE = wm.MEMBERTYPE_CODE;
                    mm.MEMBER_ID = dMemberId;
                    mm.MEMBER_TITLE = wm.MEMBER_TITLE;
                    mm.MEMBER_NAME = wm.MEMBER_NAME;
                    mm.DATEOFBIRTH = wm.DATEOFBIRTH;
                    mm.AGE_IN_YEARS = wm.AGE_IN_YEARS;
                    mm.SEX = wm.SEX;
                    mm.REJOINED = wm.REJOINED;
                    mm.RACE_CODE = wm.RACE_CODE;
                    mm.ICNO_NEW = wm.ICNO_NEW;
                    mm.ICNO_OLD = wm.ICNO_OLD;
                    mm.DATEOFJOINING = wm.DATEOFJOINING;

                    mm.BANK_CODE = wm.BANK_CODE;
                    mm.BRANCH_CODE = wm.BANKBRANCH_CODE;
                    mm.DATEOFEMPLOYMENT = wm.DATEOFEMPLOYMENT;
                    mm.Salary = wm.Salary;
                    mm.LEVY = wm.LEVY;
                    mm.TDF = wm.TDF;
                    mm.LEVY_AMOUNT = wm.LEVY_AMOUNT;
                    mm.TDF_AMOUNT = wm.TDF_AMOUNT;
                    //mm.LevyPaymentDate = DateTime.Now;
                    //mm.Tdf_PaymentDate = DateTime.Now;

                    mm.ENTRANCEFEE = wm.ENTRANCEFEE;
                    mm.MONTHLYBF = wm.MONTHLYBF;
                    mm.ACCBF = wm.ACCBF;
                    mm.CURRENT_YTDBF = wm.CURRENT_YTDBF;
                    mm.MONTHLYSUBSCRIPTION = wm.MONTHLYSUBSCRIPTION;
                    mm.ACCSUBSCRIPTION = wm.ACCSUBSCRIPTION;
                    mm.CURRENT_YTDSUBSCRIPTION = wm.CURRENT_YTDSUBSCRIPTION;
                    mm.ACCBENEFIT = wm.ACCBENEFIT;
                    mm.TOTALMONTHSPAID = wm.TOTALMONTHSPAID;
                    mm.ADDRESS1 = wm.ADDRESS1;
                    mm.ADDRESS2 = wm.ADDRESS2;
                    mm.ADDRESS3 = wm.ADDRESS3;
                    mm.PHONE = wm.PHONE;
                    mm.MOBILE = wm.MOBILE;
                    mm.EMAIL = wm.EMAIL;
                    mm.CITY_CODE = wm.CITY_CODE;
                    mm.ZIPCODE = wm.ZIPCODE;
                    mm.STATE_CODE = wm.STATE_CODE;
                    mm.COUNTRY = wm.COUNTRY;
                    mm.UpdatedBy = 0;
                    mm.UpdatedOn = DateTime.Now;
                    mm.IsBranchRegister = true;
                    mm.BranchMemberCode = Convert.ToInt32(MemberCode);

                    db.MASTERMEMBERs.Add(mm);

                    wm.IsApproved = 1;
                    db.SaveChanges();

                    int iMemberCode = Convert.ToInt32(db.MASTERMEMBERs.Max(x => x.MEMBER_CODE));

                    var ni = (from x in db.NomineeInsertBranches where x.MEMBER_CODE == MemberCode select x).FirstOrDefault();
                    if (ni != null)
                    {
                        MASTERNOMINEE mn = new MASTERNOMINEE();
                        mn.MEMBER_CODE = iMemberCode;
                        mn.NAME = ni.NAME;
                        mn.ICNO_NEW = ni.ICNO_NEW;
                        mn.SEX = ni.SEX;
                        mn.AGE = ni.AGE;
                        mn.RELATION_CODE = ni.RELATION_CODE;
                        mn.ADDRESS1 = ni.ADDRESS1;
                        mn.ADDRESS2 = ni.ADDRESS2;
                        mn.CITY_CODE = ni.CITY_CODE;
                        mn.STATE_CODE = ni.STATE_CODE;
                        mn.COUNTRY = ni.COUNTRY;
                        db.MASTERNOMINEEs.Add(mn);
                        db.SaveChanges();
                    }

                    var gi = (from x in db.GuardianInsertBranches where x.MEMBER_CODE == MemberCode select x).FirstOrDefault();
                    if (gi != null)
                    {
                        MASTERGUARDIAN mg = new MASTERGUARDIAN();
                        mg.MEMBER_CODE = iMemberCode;
                        mg.NAME = gi.NAME;
                        mg.ICNO_NEW = gi.ICNO_NEW;
                        mg.SEX = gi.SEX;
                        mg.AGE = gi.AGE;
                        mg.RELATION_CODE = gi.RELATION_CODE;
                        mg.ADDRESS1 = gi.ADDRESS1;
                        mg.ADDRESS2 = gi.ADDRESS2;
                        mg.CITY_CODE = gi.CITY_CODE;
                        mg.STATE_CODE = gi.STATE_CODE;
                        mg.COUNTRY = gi.COUNTRY;
                        db.MASTERGUARDIANs.Add(mg);
                        db.SaveChanges();
                    }
                    return Json(new { MemberId = mm.MEMBER_ID, Msg = "Approved Sucessfuly" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { ErrMsg = "No Records Found" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { isSaved = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [NubeCrossSiteAttribute]
        public JsonResult Declain(decimal MemberCode)
        {
            try
            {
                var mm = (from x in db.MASTERMEMBERs where x.BranchMemberCode == MemberCode select x).FirstOrDefault();
                if (mm != null)
                {
                    var nm = (from x in db.MASTERNOMINEEs where x.MEMBER_CODE == mm.MEMBER_CODE select x).FirstOrDefault();
                    if (nm != null)
                    {
                        db.MASTERNOMINEEs.Remove(nm);
                    }

                    var gr = (from x in db.MASTERGUARDIANs where x.MEMBER_CODE == mm.MEMBER_CODE select x).FirstOrDefault();
                    if (gr != null)
                    {
                        db.MASTERGUARDIANs.Remove(gr);
                    }

                    db.MASTERMEMBERs.Remove(mm);
                    db.SaveChanges();

                    var wm = (from x in db.MemberInsertBranches where x.MEMBER_CODE == MemberCode select x).FirstOrDefault();
                    if (wm != null)
                    {
                        wm.IsApproved = 2;
                        db.SaveChanges();
                    }
                    return Json(new { MemberId = mm.MEMBER_ID, Msg = "Declained Sucessfuly" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { ErrMsg = "No Records Found" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { isSaved = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [NubeCrossSiteAttribute]
        public JsonResult AttachmentUpload(int MemberCode,string AttachmentName,HttpPostedFileBase AttachmentData)
        {
            try
            {

                byte[] fDatas;
                using (BinaryReader br = new BinaryReader(AttachmentData.InputStream))
                {
                    fDatas = br.ReadBytes(AttachmentData.ContentLength);
                }
                DAL.MembershipAttachment d = new MembershipAttachment();
                d.MemberCode = MemberCode;
                d.FileName = AttachmentData.FileName;
                d.FileType = AttachmentData.ContentType;
                d.AttachmentName = AttachmentName;
                d.AttachmentData = fDatas;
                d.EntryDate = DateTime.Today;
                db.MembershipAttachments.Add(d);
                db.SaveChanges();
                return Json(new { ErrMsg = "No Records Found" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { isSaved = false, ErrMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public FileResult AttachmentDownload(int MemberCode, string AttachmentName)
        {
            try
            {
                nubebfsEntities db = new nubebfsEntities();
                var d= db.MembershipAttachments.FirstOrDefault(x => x.MemberCode == MemberCode && x.AttachmentName == AttachmentName);
                return File(d.AttachmentData, d.FileType, d.FileName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
