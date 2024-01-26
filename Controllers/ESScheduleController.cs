using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class ESScheduleController : Controller
    {//工程督導 - 行程安排

        SupervisePhaseService iService = new SupervisePhaseService();
        public ActionResult Index()
        {
            Utils.setUserClass(this, 1);
            return View("Index");
        }

        //日程表 s20230519
        public JsonResult GetScheduleForm(int id)
        {
            List<SuperviseScheduleFormModel> scheduleForm = iService.GetScheduleForm(id);
            if(scheduleForm.Count == 0)
            {
                if(!iService.InitScheduleForm(id))
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "初始化 日程表 失敗"
                    });
                }
                scheduleForm = iService.GetScheduleForm(id);
            }
            return Json(new
            {
                result = 0,
                items = scheduleForm
            });
        }

        public JsonResult GetThsrByFilterVM(ThsrFilterVM filter, bool getOption = false)
        {
            try
            {
                List<SuperviseEngTHSRVModel> l = iService.GetThsrList().Where(
                        r => r.Direction == filter.direction  && 
                        (r.CarNo.Contains(filter.carNo ?? "") || filter.carNo == null ) ).ToList();
                var thsrService = new ThsrService();
                var ll = l.Where(r => r.StartStationName.Contains(filter.start ?? "") || filter.start == null);
                var startOptionList = l.GroupBy(r => r.StartStationName).Select(r => r.Key)
                    .ToList().SortListByMap(r => r, Order.TrainOrderMap, true); 
                var lll  = ll.Where(r => r.EndingStationName.Contains(filter.end ?? "") || filter.end == null);
                var endOptionList = ll.GroupBy(r => r.EndingStationName).Select(r => r.Key)
                    .ToList().SortListByMap(r => r, Order.TrainOrderMap, true);
                return Json(new
                {
                    status = "success",
                    list = !getOption ? lll : new SuperviseEngTHSRVModel[0] ,
                    startOptionList = startOptionList,
                    endOptionList = endOptionList
                });
            }
            catch (Exception e)
            {
                return Json(new { status = "failed" });
            }
        }

        //期別查詢
        public JsonResult SearchPhase(string keyWord)
        {
            List<SupervisePhaseModel> list = iService.GetPhaseCode(keyWord);
            if(list.Count==1)
            {
                return Json(new
                {
                    result = 0,
                    item = list[0]
                });
            }
            return Json(new
            {
                result = -1,
                msg = "查無此期別"
            });
        }
        //期間工程清單
        public JsonResult GetPhaseEngList(int id, int pageRecordCount, int pageIndex)
        {
            List<SuperviseEng1VModel> engList = new List<SuperviseEng1VModel>();
            int total = iService.GetPhaseEngList1Count(id);
            
            if (total > 0)
            {
                engList = iService.GetPhaseEngList1<SuperviseEng1VModel>(id, pageRecordCount, pageIndex);
            }
            return Json(new
            {
                pTotal = total,
                items = engList
            });
        }

        //期間工程
        public JsonResult GetEng(int id)
        {
            List<SuperviseEngScheduleVModel> engList = iService.GetEngForShcedule<SuperviseEngScheduleVModel>(id);
            if (engList.Count == 1)
            {
                SuperviseEngScheduleVModel m = engList[0];
                if (m.AdminContact == null)
                {
                    m.AdminContact = m.OfficerName;
                    if (m.AdminContact == null) m.AdminContact = "";
                    m.AdminTel = m.OfficerTel;
                    m.AdminMobile = m.OfficerMobile;
                    if(!string.IsNullOrEmpty(m.ExecUnitName))
                    {
                        if( m.ExecUnitName.IndexOf("縣")>0 || m.ExecUnitName.IndexOf("市") > 0)
                        {
                            m.LocalGovContact = m.ContactName;
                            m.LocalGovTel = m.ContactPhone;
                        } else
                        {
                            m.RiverBureauContact = m.ContactName;
                            m.RiverBureauTel = m.ContactPhone;
                        }
                    }
                }
                return Json(new
                {
                    result = 0,
                    item = m
                });
            } else
            {
                return Json(new
                {
                    result = -1,
                    msg = "無法取得資料"
                });
            }
            
        }
        
        public JsonResult SaveEng(SuperviseEngScheduleVModel m, List<SuperviseScheduleFormModel> sf)
        {
            int state = iService.SaveEngForShcedule(m, sf);
            if (state == 1)
            {
                return Json(new
                {
                    result = 0,
                    msg = "儲存成功"
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "儲存失敗"
                });
            }

        }
        //高鐵資訊
        public JsonResult GetEngTHSR(int id)
        {
            List<SuperviseEngTHSRVModel> lists = iService.GetEngTHSR(id);
            return Json(new
            {
                items = lists
            });
        }
        public JsonResult AddEngTHSR(SuperviseEngTHSRVModel m)
        {
            int state = iService.AddEngTHSR(m);
            if (state == 1)
            {
                return Json(new
                {
                    result = 0,
                    msg = "加入成功"
                }); ;
            }
            return Json(new
            {
                result = -1,
                msg = "加入失敗"
            });
        }
        public JsonResult DelEngTHSR(int id)
        {
            int state = iService.DelEngTHSR(id);
            if (state == 1)
            {
                return Json(new
                {
                    result = 0,
                    msg = "移除成功"
                }); ;
            }
            return Json(new
            {
                result = -1,
                msg = "移除失敗"
            });
        }
        public JsonResult GetTHSR()
        {
            List<SuperviseEngTHSRVModel> lists = iService.GetTHSR();
            return Json(new
            {
                items = lists
            });
        }

        //
        public ActionResult Download(int id, int docNo, int docType)
        {
            List<SuperviseEngSchedule1VModel> engList = new SuperviseStaService().GetEngForShcedule<SuperviseEngSchedule1VModel>(id);

            if (engList.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "無法取得資料"
                });
            }

            SuperviseEngSchedule1VModel eng = engList[0];

            string tempFolder = Utils.GetTempFolder();
            System.Diagnostics.Debug.WriteLine(tempFolder);
            SuperviseUtils superviseUtils = new SuperviseUtils();
            string tarFile = null, fName = "";
            try
            {
                switch (docNo)
                {
                    case 1:
                        //tarFile = superviseUtils.fillBaseWord(eng, docType, tempFolder);
                        fName = "工程督導行前製表資訊";
                        break;
                    case 2:
                        tarFile = superviseUtils.fillScheduleWord(eng, docType, tempFolder);
                        fName = "督導行程表";
                        break;
                    case 3:
                        tarFile = superviseUtils.fillFeeWord(eng, docType, tempFolder);
                        fName = "委員出席費請領單";
                        break;
                    case 4:
                        tarFile = superviseUtils.fillSignInWord(eng, docType, tempFolder);
                        fName = "簽到單";
                        break;
                    case 5:
                        tarFile = superviseUtils.fillCommentWord(eng, docType, tempFolder);
                        fName = "督導意見表";
                        break;
                    case 6:
                        tarFile = superviseUtils.fillRecordWord(eng, docType, tempFolder);
                        fName = "督導人員記錄表";
                        break;
                    /* shioulo 20220711 取消
                    case 7:
                        //??? tarFile = superviseUtils.fillRecordWord(eng, docType, tempFolder);
                        fName = "委員意見統計表";
                        break;*/
                    case 99:
                        tarFile = superviseUtils.fillScheduleWord(eng, docType, tempFolder); //督導行程表
                        tarFile = superviseUtils.fillFeeWord(eng, docType, tempFolder); //委員出席費請領單
                        tarFile = superviseUtils.fillSignInWord(eng, docType, tempFolder); //簽到單
                        tarFile = superviseUtils.fillCommentWord(eng, docType, tempFolder); //督導意見表
                        tarFile = superviseUtils.fillRecordWord(eng, docType, tempFolder); //督導人員記錄表
                        
                        string zipFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("B").ToUpper() + ".zip");
                        ZipFile.CreateFromDirectory(tempFolder, zipFile);// AddFiles(files, "ProjectX");
                        Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                        return File(iStream, "application/blob", "經濟部水利署 行程安排表單.zip");
                        break;
                }

                if (tarFile != null)
                {
                    int inx = tarFile.LastIndexOf(".");
                    return DownloadFile(tarFile, fName+tarFile.Substring(inx, tarFile.Length - inx));
                }
            }
            catch
            {
                return Json(new
                {
                    result = -1,
                    message = "製表失敗"
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                result = -1,
                message = "請求錯誤"
            }, JsonRequestBehavior.AllowGet);
        }
        private ActionResult DownloadFile(string fullPath, string fileName)
        {
            if (System.IO.File.Exists(fullPath))
            {
                Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", fileName);
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }
        //下載行程表
        public ActionResult DnSchedule(int id)
        {
            string filename = Utils.CopyTemplateFile("水利署工程督導行程表.xlsx", ".xlsx");
            Dictionary<string, Microsoft.Office.Interop.Excel.Worksheet> dict = new Dictionary<string, Microsoft.Office.Interop.Excel.Worksheet>();
            Microsoft.Office.Interop.Excel.Application appExcel = null;
            Microsoft.Office.Interop.Excel.Workbook workbook = null;
            //開啟 Excel 檔案
            try
            {
                appExcel = new Microsoft.Office.Interop.Excel.Application();
                workbook = appExcel.Workbooks.Open(filename);

                foreach (Microsoft.Office.Interop.Excel.Worksheet worksheet in workbook.Worksheets)
                {
                    dict.Add(worksheet.Name, worksheet);
                }
                Microsoft.Office.Interop.Excel.Worksheet sheet = dict["行程表"];

                List<SuperviseEngSchedule2VModel> items = iService.GetSupervisePhaseScheduleList(id);
                int row = 3, inx = 1;
                string phaseCode = "";
                Microsoft.Office.Interop.Excel.Range excelRange;
                for (int i = 1; i < items.Count; i++)
                {
                    sheet.Rows[3].Insert();
                }
                foreach (SuperviseEngSchedule2VModel m in items)
                {
                    phaseCode = m.PhaseCode;
                    sheet.Cells[row, 1] = inx;
                    sheet.Cells[row, 2] = m.OrganizerName;
                    sheet.Cells[row, 3] = m.BelongPrj;
                    sheet.Cells[row, 4] = m.TenderName;
                    sheet.Cells[row, 5] = m.Location;
                    string sd = "";// "@督導日期" s20230316
                    if (m.SuperviseEndDate.HasValue)
                    {
                        sd = String.Format("{0}~{1}",
                            !m.SuperviseDate.HasValue ? "" : String.Format("{0}{1}\n({2})", m.SuperviseDate.Value.Year - 1911, m.SuperviseDate.Value.ToString("MMdd"), toChsWeek(m.SuperviseDate.Value.DayOfWeek)),
                            !m.SuperviseEndDate.HasValue ? "" : String.Format("{0}{1}\n({2})", m.SuperviseEndDate.Value.Year - 1911, m.SuperviseEndDate.Value.ToString("MMdd"), toChsWeek(m.SuperviseEndDate.Value.DayOfWeek)));
                    }
                    else
                    {
                        sd = !m.SuperviseDate.HasValue ? "" : String.Format("{0}{1}\n({2})", m.SuperviseDate.Value.Year - 1911, m.SuperviseDate.Value.ToString("MMdd"), toChsWeek(m.SuperviseDate.Value.DayOfWeek));
                    }
                    sheet.Cells[row, 6] = sd;
                    sheet.Cells[row, 7] = m.BidAmount;
                    sheet.Cells[row, 8] = m.ScheCompletionDate;
                    sheet.Cells[row, 9] = m.ActualProgress;
                    sheet.Cells[row, 10] =  m.DiffProgress;
                    sheet.Cells[row, 11] = m.LeaderName == null ? "" : m.LeaderName.Replace(",","\n");
                    sheet.Cells[row, 12] = m.OutCommittee == null ? "" : m.OutCommittee.Replace(",", "\n");
                    sheet.Cells[row, 13] = m.InsideCommittee == null ? "" : m.InsideCommittee.Replace(",", "\n");
                    sheet.Cells[row, 14] = m.OfficerName == null ? "" : m.OfficerName.Replace(",", "\n");
                    sheet.Cells[row, 15] = m.Memo;

                    /*excelRange = sheet.Range[sheet.Cells[row, 1], sheet.Cells[row, 16]];
                    excelRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    excelRange.Borders.ColorIndex = 1;*/
                    inx++;
                    row++;
                }
                phaseCode = String.Format("水利署工程督導小組{0}年{1}月份預定督導行程表", phaseCode.Substring(0, 3), phaseCode.Substring(3, 2));
                sheet.Cells[1, 1] = phaseCode;
                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                Stream iStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("{0}.xlsx", phaseCode));
            }
            catch (Exception e)
            {
                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                return Json(new
                {
                    result = -1,
                    message = "Excel 製表失敗\n" + e.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private string toChsWeek(DayOfWeek week)
        {
            switch (week)
            {
                case DayOfWeek.Monday: return "一"; break;
                case DayOfWeek.Tuesday: return "二"; break;
                case DayOfWeek.Wednesday: return "三"; break;
                case DayOfWeek.Thursday: return "四"; break;
                case DayOfWeek.Friday: return "五"; break;
                case DayOfWeek.Saturday: return "六"; break;
                default:
                    return "日";
            }
        }
    }
}