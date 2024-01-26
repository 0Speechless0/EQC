using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using EQC.Common;
using EQC.Models;
using EQC.Services;
using Newtonsoft.Json;

namespace EQC.Controllers
{
    [SessionFilter]
    public class TechnicalController : Controller
    {
        // GET: api/Technical

        TechnicalService service = new TechnicalService();
        public JsonResult GetArticals()
        {
            try
            {
                var articals = service.getAllTechnicalArtical();
                return Json(new { status = "success", data = articals, authors = service.getTechnicalArticalAuthorName() }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {

                return Json(new { status="failed"}, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult getArticalData(int id)
        {
            try
            {
                List<object> data = service.getArticalCommentWithReply(id);
                //return Json(new { status = "success", data = data, isAdmin = new SessionManager().GetUser().IsAdmin }, JsonRequestBehavior.AllowGet);
                return Json(new { status = "success", data = data, isAdmin = new SessionManager().GetUser().IsAdmin }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getAllArticalFileName(int id)
        {
            try
            {
                List<string> data = service.getAllFileNameInDir(id, "TechnicalArticals");
                return Json(new { status = "success", data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult getArticalDataFiles(int id)
        {
            try
            {
                List<object> data = service.getCommentAndReplyPathByArtical(id);
                return Json(new { status = "success", data = data, } , JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult getAllReplyFileName(int id)
        {
            try
            {
                List<string> data = service.getAllFileNameInDir(id, "TechnicalReplys");
                return Json(new { status = "success", data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: api/Technical

        public JsonResult Store(FormCollection value)
        {
            try
            {
                List<int> tags = JsonConvert.DeserializeObject<List<int>>(Request.Form.Get("tags"));
                int insertId = service.addArtical(value);
                service.storeArticalTags(insertId, tags);
                if (Request.Files.Count > 0)
                    service.saveUploadFile(Request.Files[0], insertId, "TechnicalArticals");
                return Json(new { status = "success" });
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" });
            }

        }

        public JsonResult StoreComment(TechnicalComment value)
        {
            try
            {
            
                int insertId = service.addComment(value);
                if (Request.Files.Count > 0)
                    service.saveUploadFile(Request.Files[0], insertId, "TechnicalComments");
                return Json(new { status = "success" });
            }
            catch (Exception e)
            {
                return Json(new { status = "failed" });
            }
        }

        public JsonResult StoreReply(TechnicalReply value)
        {
            try
            {
                int insertId = service.addReply(value);
                if (Request.Files.Count > 0)
                    service.saveUploadFile(Request.Files[0], insertId, "TechnicalReplys");
                return Json(new { status = "success" });
            }
            catch (Exception e)
            {
                return Json(new { status = "failed" });
            }
        }

        public JsonResult updateArtical(int id, TechnicalArtical value, List<int> tags)
        {
            try
            {
                service.updateArtical(id, value);
                
                if(tags != null) service.resetArticalTags(id, tags);
                return Json(new { status = "success" });
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" });
            }
        }
        public JsonResult updateComment(int id, TechnicalComment value)
        {
            try
            {
                service.updateComment(id, value);
                return Json(new { status = "success" });
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" });
            }
        }
        public JsonResult updateReply(int id, TechnicalReply value)
        {
            try
            {
                service.updateReply(id, value);
                return Json(new { status = "success" });
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" });
            }
        }

        // DELETE: api/Technical/5
        public JsonResult deleteArtical(int id)
        {
            try
            {
                service.deleteArtical(id);
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult deleteComment(int id)
        {
            try
            {
                service.deleteComment(id);
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult deleteReply(int id)
        {
            try
            {
                service.deleteReply(id);
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult giveThumb(int id)
        {
            try
            {
                service.giveThumb(id);
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult recoveryThumb(int id)
        {
            try
            {
                service.recoveryThumb(id);
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult getReplyThumb(int id)
        {
            try
            {
                int thumbCount = service.getReplyThumb(id);
                bool isUserThumb = service.checkReplyThumb(id);
                return Json(new { status = "success", thumbCount = thumbCount, isUserThumb = isUserThumb }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult storeTag(FormCollection value)
        {
            try
            {
                service.storeTag(value["Text"]);
                return Json(new { status = "success" });
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" });
            }
        }
        public JsonResult getAllTag()
        {
            try
            {
                var result = service.getAllTag();
                return Json(new { status = "success", data= result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult deleteTags(List<int> tags)
        {
            try
            {
                service.deleteTags(tags);
                return Json(new { status = "success"});
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" });
            }
        }


    }
}
