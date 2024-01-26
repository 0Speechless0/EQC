using EQC.Common;
using EQC.Models;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class ChapterChartController : Controller
    {
        public ActionResult Index()
        {
            return View("ChapterChart");//shioulo 20220514
            //ViewBag.Title = "圖表維護";
            //return View("ChapterChart", "_BackendLayout");
        }

        private string GetFlowChartPath()
        {
            return Utils.GetTemplateFolder();
            /*string folderName = "FileUploads/Tp";
            string webRootPath = Server.MapPath("~");
            return Path.Combine(webRootPath, folderName);*/
        }
        //章節清單
        public JsonResult GetChapterOptions()
        {
            ChapterService service = new ChapterService();
            List<ChapterListModel> result = service.GetChapterList<ChapterListModel>();
            result.Insert(0, new ChapterListModel() {
                ChapterName = "全部",
                Seq = -1
            });
            return Json(result);
        }
        //圖表
        public JsonResult Chapter(int chapter, int pageIndex, int perPage)
        {
            List<CMEditModel> result = new List<CMEditModel>();
            ChapterService service = new ChapterService();
            int total = service.GetListCount(chapter);
            if (total > 0)
            {
                result = result = service.GetChartMaintainTpByChapterSeq(chapter, pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        //
        public JsonResult ChapterNewItem(int chapter)
        {
            ChapterService service = new ChapterService();
            CMEditModel item = new CMEditModel();
            item.ChapterSeq = chapter;
            item.OrderNo = 999;
            int newSeq = service.Add(item);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "新增失敗"
                });
            }
            else
            {
                List<CMEditModel> items = service.GetItemBySeq(newSeq);
                return Json(new
                {
                    result = 0,
                    message = "新增完成",
                    item = Json(items[0])
                });
            }
        }
        //
        public JsonResult ChapterSave(CMEditModel item)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + item.MDTestItem);
            ChapterService service = new ChapterService();
            if (service.Update(item) == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
            else
            {
                List<CMEditModel> items = service.GetItemBySeq(item.Seq);
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                    item = Json(items[0])
                });
            }
        }

        public JsonResult ChapterDel(int seq)
        {
            ChapterService service = new ChapterService();
            List<CMEditModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0)
            {
                return Json(new
                {
                    result = 0,
                    message = "資料刪除完成"
                });
            }
            else
            {
                int result = service.Delete(seq);
                if (result == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "資料刪除失敗"
                    });
                }
                else
                {
                    //刪除已儲存檔案
                    string uniqueFileName = items[0].UniqueFileName;
                    if (uniqueFileName != null && uniqueFileName.Length > 0)
                    {
                        System.IO.File.Delete(Path.Combine(GetFlowChartPath(), uniqueFileName));
                    }
                    return Json(new
                    {
                        result = 0,
                        message = "資料刪除完成",
                    });
                }
            }
        }

        public JsonResult ChapterUpload(int seq)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                try
                {
                    ChapterService service = new ChapterService();
                    List<CMEditModel> items = service.GetItemFileInfoBySeq(seq);
                    if(items.Count==0)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "資料錯誤無法上傳檔案"
                        });
                    }
                    string uniqueFileName = SaveFile(file, items[0], "ChapterChart-");
                    if (uniqueFileName != null)
                    {
                        string originFileName = file.FileName.ToString().Trim();
                        if (service.UpdateUploadFile(seq, originFileName, uniqueFileName) == 0)
                        {
                            return Json(new
                            {
                                result = -1,
                                message = "上傳檔案失敗"
                            });
                        }
                        else
                        {
                            items = service.GetItemBySeq(seq);
                            return Json(new
                            {
                                result = 0,
                                message = "上傳檔案完成",
                                item = Json(items[0])
                            });

                        }
                    }
                } catch(Exception e) {
                    System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                }
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }
        
        public ActionResult ChapterDownload(int seq)
        {
            ChapterService service = new ChapterService();
            List<CMEditModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].UniqueFileName == null || items[0].UniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            return DownloadFile(items[0]);
        }                        

        private string SaveFile(HttpPostedFileBase file, CMEditModel m, string fileHeader)
        {
            try
            {
                string filePath = GetFlowChartPath();
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                //刪除已儲存原始檔案
                string uniqueFileName = m.UniqueFileName;
                if (uniqueFileName != null && uniqueFileName.Length > 0)
                {
                    System.IO.File.Delete(Path.Combine(filePath, uniqueFileName));
                }

                string originFileName = file.FileName.ToString().Trim();
                int inx = originFileName.LastIndexOf(".");
                uniqueFileName = String.Format("{0}{1}{2}", fileHeader, m.Seq, originFileName.Substring(inx));
                string fullPath = Path.Combine(filePath, uniqueFileName);
                file.SaveAs(fullPath);

                return uniqueFileName;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                return null;
            }
        }

        private ActionResult DownloadFile(CMEditModel m)
        {
            string filePath = GetFlowChartPath();

            string uniqueFileName = m.UniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                if (System.IO.File.Exists(fullPath))
                {
                    Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", m.OriginFileName);
                }
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}