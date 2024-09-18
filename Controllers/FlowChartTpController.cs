using EQC.Common;
using EQC.Models;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
  
    [SessionFilter]
    public class FlowChartTpController : MyController
    {
        public ActionResult Index()
        {
            //ViewBag.Title = "清單及流程圖維護";
            return View("FlowChart");
        }
        public void DownloadExcel()
        {
            EngMaterialDeviceListTpService C5service = new EngMaterialDeviceListTpService();
            EquOperTestListTpService C6service = new EquOperTestListTpService();
            ConstCheckListTpService C701service = new ConstCheckListTpService();
            EnvirConsListTpService C702service = new EnvirConsListTpService();
            OccuSafeHealthListTpService C703service = new OccuSafeHealthListTpService();



            var p = new ExcelProcesser(0, (wookBook) => {
                for(int i = 0; i <5; i++)
                {
                    if( wookBook.NumberOfSheets  <  i+1 )
                    {
                        wookBook.CreateSheet();
                    }   
                    var a = wookBook.GetSheetAt(i);
                    var row = a.CreateRow(0);
                    new string[] {
                        "excel編號",
                        "流程圖名稱",
                    }.ToList()
                    .ForEach(colName =>
                    {
                        row.CreateCell(row.Cells.Count).SetCellValue(colName);
                    });

                }

            });


            var listC5 = C5service.GetList<EMDListTpEditModel>();
            p.setSheet(0).setSheetName("材料與設備抽驗程序送審", 0);
            p.insertOneCol(listC5.Select(r => r.ExcelNo), 0);
            p.insertOneCol(listC5.Select(r => r.MDName), 1);
            var listC6 = C6service.GetList<EOTListTpEditModel>();
            p.setSheet(1).setSheetName("設備功能運轉測試抽驗程序及標準", 1);
            p.insertOneCol(listC6.Select(r => r.ExcelNo), 0);
            p.insertOneCol(listC6.Select(r => r.ItemName), 1);

            var listC701 = C701service.GetList<CCListTpEditModel>();
            p.setSheet(2).setSheetName("施工抽查程序及標準", 2);
            p.insertOneCol(listC701.Select(r => r.ExcelNo), 0);
            p.insertOneCol(listC701.Select(r => r.ItemName), 1);
            var listC702 = C702service.GetList<ECListTpEditModel>();
            p.setSheet(3).setSheetName("施工抽查程序及標準(環境保育)", 3);
            p.insertOneCol(listC702.Select(r => r.ExcelNo), 0);
            p.insertOneCol(listC702.Select(r => r.ItemName), 1);
            var listC703 = C703service.GetList<OSHListTpEditModel>();
            p.setSheet(4).setSheetName("施工抽查程序及標準(職業安全衛生)", 4);
            p.insertOneCol(listC703.Select(r => r.ExcelNo), 0);
            p.insertOneCol(listC703.Select(r => r.ItemName), 1);

            DownloadFile(p.getTemplateStream(), "清單及流程圖維護.xlsx");
        }
        private string GetFlowChartPath()
        {
            return Utils.GetTemplateFolder();
        }
        //第五章 材料設備清冊範本
        public JsonResult Chapter5(int pageIndex, int perPage)
        {
            List<EMDListTpEditModel> result = new List<EMDListTpEditModel>();
            EngMaterialDeviceListTpService service = new EngMaterialDeviceListTpService();
            int total = service.GetListCount();
            if (total > 0)
            {
                result = service.ListAll(pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }

        public JsonResult getFlowChartTpDiagramJson(int id, string type)
        {
            try
            {
                FlowChartDiagramService service = new FlowChartDiagramService();
                string result = service.getFlowChartTpDiagramJson(id, type);
                return Json(new { status = "success", jsonStr = result }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult storeFlowChartTpDiagramJson(FormCollection value)
        {
            try
            {
                FlowChartDiagramService service = new FlowChartDiagramService();
                value.Set("Seq", value.Get("ItemId"));
                service.storeFlowChartTpDiagramJson(value);
                return Json(new { status = "success" });
            }
            catch (Exception e)
            {
                return Json(new { status = "failed" });
            }
        }
        //
        public JsonResult Chapter5NewItem()
        {
            EngMaterialDeviceListTpService service = new EngMaterialDeviceListTpService();
            EMDListTpEditModel item = new EMDListTpEditModel();
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
                List<EMDListTpEditModel> items = service.GetItemBySeq(newSeq);
                return Json(new
                {
                    result = 0,
                    message = "新增完成",
                    item = Json(items[0])
                });
            }
        }
        //
        public JsonResult Chapter5Save(EMDListTpEditModel item)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + item.MDTestItem);
            EngMaterialDeviceListTpService service = new EngMaterialDeviceListTpService();
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
                List<EMDListTpEditModel> items = service.GetItemBySeq(item.Seq);
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                    item = Json(items[0])
                });
            }
        }

        public JsonResult Chapter5Del(int seq)
        {
            EngMaterialDeviceListTpService service = new EngMaterialDeviceListTpService();
            List<EMDListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
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
                int result = 0;
                try
                {
                    result = service.Delete(seq);
                } catch(Exception e)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "已有抽驗標準管理項目, 不可刪除"
                    });
                }
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
                    string uniqueFileName = items[0].FlowCharUniqueFileName;
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

        public JsonResult Chapter5Upload(int seq)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                try
                {
                    EngMaterialDeviceListTpService service = new EngMaterialDeviceListTpService();
                    List<EMDListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
                    if(items.Count==0)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "資料錯誤無法上傳檔案"
                        });
                    }
                    string uniqueFileName = SaveFile(file, items[0], "Flow-C5-");
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
        
        public ActionResult Chapter5Download(int seq)
        {
            EngMaterialDeviceListTpService service = new EngMaterialDeviceListTpService();
            List<EMDListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            return DownloadFile(items[0]);
        }

        public JsonResult Chapter5Show(int seq)
        {
            EngMaterialDeviceListTpService service = new EngMaterialDeviceListTpService();
            List<EMDListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }

            string filePath = GetFlowChartPath();

            string uniqueFileName = items[0].FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
               
                return Json(new
                {
                    url = uniqueFileName
                });
            }

            return Json(new
            {
                result = -1,
                message = "失敗"
            }, JsonRequestBehavior.AllowGet);
        }


        //第六章 設備功能運轉測試抽驗程序及標準
        public JsonResult Chapter6(int pageIndex, int perPage)
        {
            List<EOTListTpEditModel> result = new List<EOTListTpEditModel>();
            EquOperTestListTpService service = new EquOperTestListTpService();
            int total = service.GetListCount();
            if (total > 0)
            {
                result = service.ListAll(pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        //
        public JsonResult Chapter6NewItem()
        {
            EquOperTestListTpService service = new EquOperTestListTpService();
            EOTListTpEditModel item = new EOTListTpEditModel();
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
                List<EOTListTpEditModel> items = service.GetItemBySeq(newSeq);
                return Json(new
                {
                    result = 0,
                    message = "新增完成",
                    item = Json(items[0])
                });
            }
        }
        //
        public JsonResult Chapter6Save(EOTListTpEditModel item)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + item.MDTestItem);
            EquOperTestListTpService service = new EquOperTestListTpService();
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
                List<EOTListTpEditModel> items = service.GetItemBySeq(item.Seq);
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                    item = Json(items[0])
                });
            }
        }

        public JsonResult Chapter6Del(int seq)
        {
            EquOperTestListTpService service = new EquOperTestListTpService();
            List<EOTListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
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
                int result = 0;
                try
                {
                    result = service.Delete(seq);
                }
                catch (Exception e)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "已有抽驗標準管理項目, 不可刪除"
                    });
                }
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
                    string uniqueFileName = items[0].FlowCharUniqueFileName;
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

        public JsonResult Chapter6Upload(int seq)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                try
                {
                    EquOperTestListTpService service = new EquOperTestListTpService();
                    List<EOTListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
                    if (items.Count == 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "資料錯誤無法上傳檔案"
                        });
                    }
                    string uniqueFileName = SaveFile(file, items[0], "Flow-C6-");
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
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                }
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }

        public ActionResult Chapter6Download(int seq)
        {
            EquOperTestListTpService service = new EquOperTestListTpService();
            List<EOTListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            return DownloadFile(items[0]);
        }

        public JsonResult Chapter6Show(int seq)
        {
            EquOperTestListTpService service = new EquOperTestListTpService();
            List<EOTListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            string filePath = GetFlowChartPath();
            string uniqueFileName = items[0].FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);

                return Json(new
                {
                    url = uniqueFileName
                });
            }

            return Json(new
            {
                result = -1,
                message = "失敗"
            }, JsonRequestBehavior.AllowGet);
        }



        //第七章 701 施工抽查程序及標準
        public JsonResult Chapter701(int pageIndex, int perPage)
        {
            List<CCListTpEditModel> result = new List<CCListTpEditModel>();
            ConstCheckListTpService service = new ConstCheckListTpService();
            int total = service.GetListCount();
            if (total > 0)
            {
                result = service.ListAll(pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        //
        public JsonResult Chapter701NewItem()
        {
            ConstCheckListTpService service = new ConstCheckListTpService();
            CCListTpEditModel item = new CCListTpEditModel();
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
                List<CCListTpEditModel> items = service.GetItemBySeq(newSeq);
                return Json(new
                {
                    result = 0,
                    message = "新增完成",
                    item = Json(items[0])
                });
            }
        }
        //
        public JsonResult Chapter701Save(CCListTpEditModel item)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + item.MDTestItem);
            ConstCheckListTpService service = new ConstCheckListTpService();
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
                List<CCListTpEditModel> items = service.GetItemBySeq(item.Seq);
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                    item = Json(items[0])
                });
            }
        }

        public JsonResult Chapter701Del(int seq)
        {
            ConstCheckListTpService service = new ConstCheckListTpService();
            List<CCListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
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
                int result = 0;
                try
                {
                    result = service.Delete(seq);
                }
                catch (Exception e)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "已有抽驗標準管理項目, 不可刪除"
                    });
                }
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
                    string uniqueFileName = items[0].FlowCharUniqueFileName;
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

        public JsonResult Chapter701Upload(int seq)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                try
                {
                    ConstCheckListTpService service = new ConstCheckListTpService();
                    List<CCListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
                    if (items.Count == 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "資料錯誤無法上傳檔案"
                        });
                    }
                    string uniqueFileName = SaveFile(file, items[0], "Flow-C701-");
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
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                }
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }

        public ActionResult Chapter701Download(int seq)
        {
            ConstCheckListTpService service = new ConstCheckListTpService();
            List<CCListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            return DownloadFile(items[0]);
        }

        public JsonResult Chapter701Show(int seq)
        {
            ConstCheckListTpService service = new ConstCheckListTpService();
            List<CCListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            string filePath = GetFlowChartPath();
            string uniqueFileName = items[0].FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);

                return Json(new
                {
                    url = uniqueFileName
                });
            }

            return Json(new
            {
                result = -1,
                message = "失敗"
            }, JsonRequestBehavior.AllowGet);
        }


        //第七章 702 環境保育抽查標準範本
        public JsonResult Chapter702(int pageIndex, int perPage)
        {
            List<ECListTpEditModel> result = new List<ECListTpEditModel>();
            EnvirConsListTpService service = new EnvirConsListTpService();
            int total = service.GetListCount();
            if (total > 0)
            {
                result = service.ListAll(pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        //
        public JsonResult Chapter702NewItem()
        {
            EnvirConsListTpService service = new EnvirConsListTpService();
            ECListTpEditModel item = new ECListTpEditModel();
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
                List<ECListTpEditModel> items = service.GetItemBySeq(newSeq);
                return Json(new
                {
                    result = 0,
                    message = "新增完成",
                    item = Json(items[0])
                });
            }
        }
        //
        public JsonResult Chapter702Save(ECListTpEditModel item)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + item.MDTestItem);
            EnvirConsListTpService service = new EnvirConsListTpService();
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
                List<ECListTpEditModel> items = service.GetItemBySeq(item.Seq);
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                    item = Json(items[0])
                });
            }
        }

        public JsonResult Chapter702Del(int seq)
        {
            EnvirConsListTpService service = new EnvirConsListTpService();
            List<ECListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
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
                int result = 0;
                try
                {
                    result = service.Delete(seq);
                }
                catch (Exception e)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "已有抽驗標準管理項目, 不可刪除"
                    });
                }
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
                    string uniqueFileName = items[0].FlowCharUniqueFileName;
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

        public JsonResult Chapter702Upload(int seq)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                try
                {
                    EnvirConsListTpService service = new EnvirConsListTpService();
                    List<ECListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
                    if (items.Count == 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "資料錯誤無法上傳檔案"
                        });
                    }
                    string uniqueFileName = SaveFile(file, items[0], "Flow-C702-");
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
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                }
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }

        public ActionResult Chapter702Download(int seq)
        {
            EnvirConsListTpService service = new EnvirConsListTpService();
            List<ECListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            return DownloadFile(items[0]);
        }

        public JsonResult Chapter702Show(int seq)
        {
            EnvirConsListTpService service = new EnvirConsListTpService();
            List<ECListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            string filePath = GetFlowChartPath();
            string uniqueFileName = items[0].FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);

                return Json(new
                {
                    url = uniqueFileName
                });
            }

            return Json(new
            {
                result = -1,
                message = "失敗"
            }, JsonRequestBehavior.AllowGet);
        }


        //第七章 703 職業安全衛生抽查標準範本
        public JsonResult Chapter703(int pageIndex, int perPage)
        {
            List<OSHListTpEditModel> result = new List<OSHListTpEditModel>();
            OccuSafeHealthListTpService service = new OccuSafeHealthListTpService();
            int total = service.GetListCount();
            if (total > 0)
            {
                result = service.ListAll(pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        //
        public JsonResult Chapter703NewItem()
        {
            OccuSafeHealthListTpService service = new OccuSafeHealthListTpService();
            OSHListTpEditModel item = new OSHListTpEditModel();
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
                List<OSHListTpEditModel> items = service.GetItemBySeq(newSeq);
                return Json(new
                {
                    result = 0,
                    message = "新增完成",
                    item = Json(items[0])
                });
            }
        }
        //
        public JsonResult Chapter703Save(OSHListTpEditModel item)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + item.MDTestItem);
            OccuSafeHealthListTpService service = new OccuSafeHealthListTpService();
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
                List<OSHListTpEditModel> items = service.GetItemBySeq(item.Seq);
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                    item = Json(items[0])
                });
            }
        }

        public JsonResult Chapter703Del(int seq)
        {
            OccuSafeHealthListTpService service = new OccuSafeHealthListTpService();
            List<OSHListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
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
                int result = 0;
                try
                {
                    result = service.Delete(seq);
                }
                catch (Exception e)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "已有抽驗標準管理項目, 不可刪除"
                    });
                }
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
                    string uniqueFileName = items[0].FlowCharUniqueFileName;
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

        public JsonResult Chapter703Upload(int seq)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                try
                {
                    OccuSafeHealthListTpService service = new OccuSafeHealthListTpService();
                    List<OSHListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
                    if (items.Count == 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "資料錯誤無法上傳檔案"
                        });
                    }
                    string uniqueFileName = SaveFile(file, items[0], "Flow-C703-");
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
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                }
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }

        public ActionResult Chapter703Download(int seq)
        {
            OccuSafeHealthListTpService service = new OccuSafeHealthListTpService();
            List<OSHListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            return DownloadFile(items[0]);
        }

        public JsonResult Chapter703Show(int seq)
        {
            OccuSafeHealthListTpService service = new OccuSafeHealthListTpService();
            List<OSHListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            string filePath = GetFlowChartPath();
            string uniqueFileName = items[0].FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);

                return Json(new
                {
                    url = uniqueFileName
                });
            }

            return Json(new
            {
                result = -1,
                message = "失敗"
            }, JsonRequestBehavior.AllowGet);
        }


        private string SaveFile(HttpPostedFileBase file, FlowChartFileModel m, string fileHeader)
        {
            try
            {
                string filePath = GetFlowChartPath();
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                //刪除已儲存原始檔案
                string uniqueFileName = m.FlowCharUniqueFileName;
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

        private ActionResult DownloadFile(FlowChartFileModel m)
        {
            string filePath = GetFlowChartPath();

            string uniqueFileName = m.FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                if (System.IO.File.Exists(fullPath))
                {
                    Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", m.FlowCharOriginFileName);
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