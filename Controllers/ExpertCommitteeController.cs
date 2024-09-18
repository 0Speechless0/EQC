using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EQC.Common;
using EQC.EDMXModel;
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
    public class ExpertCommitteeController : Controller
    {//專家委員
        ExpertCommitteeService iService = new ExpertCommitteeService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View();
        }
        public ActionResult EditCommittee(int seq)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Edit", "ExpertCommittee");
            string tarUrl = redirectUrl.ToString() + "?id=" + seq;
            return Json(new { Url = tarUrl });
        }
        public ActionResult Edit()
        {
            Utils.setUserClass(this);
            return View("Edit");
        }
        //委員清單
        public JsonResult GetList()
        {
            List<ExpertCommitteeModel> lists = iService.GetList<ExpertCommitteeModel>();
            DateTime lastUpdate = iService.getLastUpdateTime("ExpertCommittee");
            return Json( new { data = lists, lastUpdate = lastUpdate});
        }
        public JsonResult GetCommittee(int id)
        {
            if(id == -1)
            {
                return Json(new
                {
                    result = 0,
                    item = new ExpertCommitteeVModel() { Seq = -1}
                });
            }
            List<ExpertCommitteeVModel> lists = iService.GetCommittee<ExpertCommitteeVModel>(id);
            if(lists.Count == 1)
            {
                return Json(new
                {
                    result = 0,
                    item = lists[0]
                });
            }
            return Json(new
            {
                result = -1,
                msg = "資料取得錯誤"
            });
        }
        public ActionResult DelCommittee(int seq)
        {
            int? prjSeq = iService.Del(seq);
            
            if(prjSeq == null)
            {
                
                return Json(new
                {
                    result = 0,
                    msg = "刪除成功"
                });
            } else
            {
                using (var context = new EQC_NEW_Entities())
                {
                    var tar = context.SuperviseEng.Where(r => r.PrjXMLSeq == prjSeq).FirstOrDefault();
                    var phrase = new SupervisePhaseService().GetPhaseEngList<SupervisePhaseModel>(tar.SupervisePhaseSeq ?? 0).FirstOrDefault();
                    return Json(new
                    {
                        result = -1,
                        msg = $"刪除失敗，委員可能已新增在工程督導, 工程: {  tar?.PrjXML?.TenderName }, 期別: {phrase.PhaseCode}" 
                      
                    });
                }


            }
        }
        public JsonResult Save(ExpertCommitteeVModel m)
        {
            if(m.Seq == -1)
            {
                int seq = iService.Add(m);
                if(seq>0) {
                    return Json(new
                    {
                        result = 0,
                        id = seq,
                        msg = "儲存成功"
                    });
                }
            } else
            {
                int cnt = iService.Update(m);
                if (cnt == 1)
                {
                    return Json(new
                    {
                        result = 0,
                        msg = "儲存成功"
                    });
                }
            }
            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }

        public void Download()
        {
            string templateFileName = Path.Combine(Utils.GetTemplateFilePath(), "專家委員匯出資料範本.xlsx");

            var processer = new ExcelProcesser(templateFileName, 0);
            using(var context = new EQC_NEW_Entities())
            {

                var list =  context.ExpertCommittee.ToList();
                var i = 1;
                processer.copyOutSideRowStyle(0, 1, list.Count());
                processer.insertOneCol(list.Select(r => i++).ToList(), 0);
                processer.insertOneCol(list.Select(r => r.ECUnit), 1);
                processer.insertOneCol(list.Select(r => r.ECName).ToList(), 2);
                processer.insertOneCol(list.Select(r => $"{r.ECBirthday?.Year - 1911 }.{r.ECBirthday?.ToString("MM")}.{r.ECBirthday?.ToString("dd") }").ToList(), 4);
                processer.insertOneCol(list.Select(r => r.ECId).ToList(), 5);
                processer.insertOneCol(list.Select(r => r.ECPosition).ToList(), 7);
                processer.insertOneCol(list.Select(r => r.ECTel ).ToList(), 8);
                processer.insertOneCol(list.Select(r => r.ECMobile).ToList(), 10);
                processer.insertOneCol(list.Select(r => r.ECFax).ToList(), 12);
                processer.insertOneCol(list.Select(r => r.ECAddr1).ToList(), 14);
                processer.insertOneCol(list.Select(r => r.ECAddr2).ToList(), 15);
                processer.insertOneCol(list.Select(r => r.ECEmail).ToList(), 16);
                processer.insertOneCol(list.Select(r => r.ECBankNo).ToList(), 17);
                processer.insertOneCol(list.Select(r => r.ECNeed).ToList(), 18);
                processer.insertOneCol(list.Select(r => r.ECDiet == 1 ? "葷" : "素" ).ToList(), 19);
                processer.insertOneCol(list.Select(r => r.ECMemo).ToList(), 20);
                processer.insertOneCol(list.Select(r => r.ECMainSkill).ToList(), 27);
                processer.insertOneCol(list.Select(r => r.ECSecSkill).ToList(), 28);


            }
            Response.AddHeader("Content-Disposition", $"attachment; filename=專家委員匯出資料{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx");
            Response.BinaryWrite(processer.getTemplateStream().ToArray());
        }
        //匯入 excel
        public JsonResult Upload()
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                string fileName;
                try
                {
                    fileName = SaveFile(file);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("上傳檔案儲存失敗: " + e.Message);
                    return Json(new
                    {
                        result = -1,
                        message = "上傳檔案儲存失敗"
                    });
                }
                List<ExpertCommitteeModel> items = new List<ExpertCommitteeModel>();
                try
                {
                    readExcelData(fileName, items);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Excel解析發生錯誤: " + e.Message);
                    return Json(new
                    {
                        result = -1,
                        message = "Excel解析發生錯誤"
                    });
                }

                int iCnt = 0, uCnt = 0;
                string errCnt = "";
                iService.ImportData(items, ref iCnt, ref uCnt, ref errCnt);
                if (errCnt != "") errCnt = "\n失敗編號:" + errCnt;
                return Json(new
                {
                    result = 0,
                    message = String.Format("讀入{0}筆, 新增:{1} 更新:{2} {3}", items.Count, iCnt, uCnt, errCnt)
                });
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }
        private void readExcelData(string fileName, List<ExpertCommitteeModel> items)
        {
            int inx;
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fileName, true, UriRelationshipErrorHandler.OpenSettings()))
            {
                var sheets = doc.WorkbookPart.WorksheetParts;
                SharedStringTable strTable = doc.WorkbookPart.SharedStringTablePart.SharedStringTable;//取得共用字串表
                foreach (var wp in sheets)
                {
                    Worksheet sheet = wp.Worksheet;
                    List<Row> rows = sheet.Descendants<Row>().ToList();
                    for (int i = 1; i < rows.Count; i++)
                    {
                        List<Cell> cells = rows[i].Descendants<Cell>().ToList();
                        if (cells.Count >= 7)
                        {
                            string str = Utils.oxCellStr(cells[0], strTable);
                            if (int.TryParse(str, out inx))
                            {
                                ExpertCommitteeModel m = new ExpertCommitteeModel() { Seq = -1, ECKind = 3 };
                                m.ECName = Utils.oxCellStr(cells[2], strTable);
                                m.ECBirthday = Birthday(Utils.oxCellStr(cells[4], strTable));
                                m.ECPosition = Utils.oxCellStr(cells[7], strTable);
                                m.ECId = Utils.oxCellStr(cells[5], strTable);
                                m.ECUnit = Utils.oxCellStr(cells[1], strTable);
                                m.ECEmail = Utils.oxCellStr(cells[16], strTable);
                                m.ECTel = Utils.oxCellStr(cells[8], strTable);
                                m.ECMobile = Utils.oxCellStr(cells[10], strTable);
                                m.ECFax = Utils.oxCellStr(cells[12], strTable);
                                m.ECAddr1 = Utils.oxCellStr(cells[14], strTable);
                                m.ECAddr2 = Utils.oxCellStr(cells[15], strTable);
                                m.ECMainSkill = Utils.oxCellStr(cells[27], strTable);
                                m.ECSecSkill = Utils.oxCellStr(cells[28], strTable);//特定專長
                                m.ECBankNo = Utils.oxCellStr(cells[17], strTable);
                                m.ECDiet = Utils.oxCellStr(cells[17], strTable) == "素" ? (byte)2 : (byte)1;
                                m.ECMemo = Utils.oxCellStr(cells[20], strTable);
                                items.Add(m);
                            }
                            else
                                break;
                        }
                    }
                }
                doc.Close();
            }
        }
        private DateTime? Birthday(string dateStr)
        {
            if (String.IsNullOrEmpty(dateStr)) return null;

            string[] v = dateStr.Split('.');

            if (v.Length != 3) return null;
            int year;
            if(int.TryParse(v[0], out year))
            {
                year += 1911;
                try
                {
                    return DateTime.Parse(String.Format("{0}/{1}/{2}", year, v[1], v[2]));
                }
                catch { }
            }
            return null;

        }
        private string SaveFile(HttpPostedFileBase file)
        {
            string filePath = Path.GetTempPath();
            string originFileName = file.FileName.ToString().Trim();
            int inx = originFileName.LastIndexOf(".");
            string uniqueFileName = String.Format("{0}{1}", Guid.NewGuid(), originFileName.Substring(inx));
            string fullPath = Path.Combine(filePath, uniqueFileName);
            file.SaveAs(fullPath);

            return fullPath;
        }
        class UriRelationshipErrorHandler : RelationshipErrorHandler
        {
            public override string Rewrite(Uri partUri, string id, string uri)
            {
                System.Diagnostics.Debug.WriteLine("Excel Open錯誤: " + id + " " + uri);
                return "https://broken-link";
            }

            public static OpenSettings OpenSettings()
            {
                return new OpenSettings
                {
                    RelationshipErrorHandlerFactory = package =>
                    {
                        return new UriRelationshipErrorHandler();
                    }
                };
            }
        }
    }
}