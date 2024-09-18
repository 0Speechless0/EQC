using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;


namespace EQC.Controllers
{
    [SessionFilter]
    public class ESSuperviseStaController : MyController
    {//工程督導 - 督導統計
        SuperviseStaService iService = new SuperviseStaService();
        public ActionResult Index()
        {
            Utils.setUserClass(this, 1);
            return View("Index");
        }
        //期別查詢
        public JsonResult SearchPhase(string keyWord)
        {
            var service = new SupervisePhaseService();
            List<SupervisePhaseModel> list = service.GetPhaseCode(keyWord);
            var phaseOption = service.GetPhaseOptions(keyWord.Substring(0, 3));
            if (list.Count > 0)
            {
                return Json(new
                {
                    result = 0,
                    item = list[0],
                    phaseOption = phaseOption
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

        //施工錯誤態樣統計
        public ActionResult GetStaList(List<string> docNo, DateTime sDate, DateTime eDate)
        {
            List<ESStaListVModel> items = iService.GetStaList<ESStaListVModel>(docNo, sDate, eDate);
            int num = iService.GetStaNum(docNo, sDate, eDate);
            int total = iService.GetStaTotal(sDate, eDate);
            return Json(new
            {
                result = 0,
                items = items,
                staNum = num,
                staTotal = total
            });
        }

        public void DownloadStaExcel(string docNo, int docType, DateTime sDate, DateTime eDate)
        {
            List<string> docNoList = docNo.Split(',').ToList();
            List<ESStaListVModel> items = iService.GetStaList<ESStaListVModel>(docNoList, sDate, eDate);
            var excelProcess = new ExcelProcesser(0, (workBook) =>
            {
                var sheet = workBook.GetSheetAt(0);
                var newRow = sheet.CreateRow(0);
                int i = 0;
                new List<string>() {
                    "缺失編號",
                    "缺失內容",
                    "缺失件數",
                    "缺失比率"
                }.ForEach(title =>
                {

                    var newCell = newRow.CreateCell(i++);
                    newCell.SetCellValue(title);
                });
                var sumaryRow = sheet.CreateRow(items.Count + 1);
                var c = sumaryRow.CreateCell(2); 
                
                c.SetCellFormula($"SUM(C2:C{items.Count + 1})");
                c = sumaryRow.CreateCell(3);
                var avergeMissRate = items.Sum(r => r.MissingCnt) / items.Sum(r => r.Total)*100;
                c.SetCellValue(decimal.Round(avergeMissRate, 2).ToString());
            });

            excelProcess.insertOneCol(items.Select(r => r.MissingNo), 0);
            excelProcess.insertOneCol(items.Select(r => r.Content), 1);
            excelProcess.insertOneCol(items.Select(r => r.MissingCnt), 2);
            excelProcess.insertOneCol(items.Select(r => r.MissingRate), 3);
            excelProcess.evaluateSheet(0);
            string filename = "";
            int year = sDate.Year-1911;
            int month = sDate.Month;
            string season = "第4季";
            if (month < 4) season = "第1季";
            else if (month < 7) season = "第2季";
            else if (month < 10) season = "第3季";
            if (docType == 1) 
            {
                filename = String.Format("{0}年{1}督導統計.ods.docx", year, season);
                base.DownloadFile(excelProcess.getConvertedTemplateStream(), filename);
            }
            if (docType == 0)
            {
                filename = String.Format("{0}年{1}督導統計.xlsx", year, season);
                base.DownloadFile(excelProcess.getTemplateStream(), filename);
            }



        }
        public ActionResult DownloadSta(string docNo, int docType, DateTime sDate, DateTime eDate)
        {
            List<string> docNoList = docNo.Split(',').ToList();
            List<ESStaListVModel> items = iService.GetStaList<ESStaListVModel>(docNoList, sDate, eDate);
            if(items.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "查無資料"
                }, JsonRequestBehavior.AllowGet);
            }
            int total = iService.GetStaTotal(sDate, eDate);

            Microsoft.Office.Interop.Word.Application wordApp = null;
            Document doc = null;
            string filename = "";
            string tableName = "";

            if (docNoList.Count > 1)
            {
                filename = "品質管制與施工品質";
                tableName = "品質管理制度缺失與施工品質缺失";
            }
            else if (docNoList[0] == "1")
            {
                filename = "品質管制";
                tableName = "品質管理制度缺失";
            }
            else if (docNoList[0] == "2")
            {
                filename = "施工品質";
                tableName = "施工品質缺失";
            }
            try
            {
                string tarfile = CopyTemplateFile("督導統計-公共工程施工查核常見缺失態樣統計表.docx", ".docx");
                wordApp = new Microsoft.Office.Interop.Word.Application();
                doc = wordApp.Documents.Open(tarfile);
                Table table = doc.Tables[1];
                table.Cell(1, 1).Range.Text = String.Format("（{0}）", tableName);
                table.Cell(2, 1).Range.Text = String.Format("期間：自{1}年{2}月{3}日至{4}年{5}月{6}日　　總件數　{0}件"
                    , total
                    , sDate.Year - 1911, sDate.Month, sDate.Day
                    , eDate.Year - 1911, eDate.Month, eDate.Day);
                //
                int row = 4, inx = 1;
                object beforeRow = table.Rows[row];
                for(int i=1; i< items.Count; i++) table.Rows.Add(ref beforeRow);

                foreach(ESStaListVModel m in items)
                {
                    table.Cell(row, 1).Range.Text = inx.ToString();
                    table.Cell(row, 2).Range.Text = m.MissingNo;
                    table.Cell(row, 3).Range.Text = m.Content;
                    table.Cell(row, 4).Range.Text = ((int)m.MissingCnt).ToString();
                    table.Cell(row, 5).Range.Text = m.MissingRate.ToString() +"%";
                    row++;
                    inx++;
                }
                //
                int month = sDate.Month;
                string season = "第4季";
                if(month<4) season = "第1季";
                else if (month < 7) season = "第2季";
                else if (month < 10) season = "第3季";
                table.Cell(row+1, 1).Range.Text = String.Format("1.本表係統計{0}年{1}工程施工查核（{2}）缺失數量前50名常見態樣。",sDate.Year- 1911, season, filename);
                doc.Save();
                if (docType == 1)
                {
                    filename = String.Format("{1}年{2}施工錯誤態樣統計-{0}.docx", filename, sDate.Year - 1911, season);
                }
                else if (docType == 2)
                {
                    filename = String.Format("{1}年{2}施工錯誤態樣統計-{0}.pdf", filename, sDate.Year - 1911, season);
                    tarfile = Path.Combine(Utils.GetTempFolder(), Guid.NewGuid().ToString("B").ToUpper()+".pdf");
                    doc.ExportAsFixedFormat(tarfile, WdExportFormat.wdExportFormatPDF);
                }
                else if (docType == 3)
                {
                    filename = String.Format("{1}年{2}施工錯誤態樣統計-{0}.odt", filename, sDate.Year - 1911, season);
                    tarfile = Path.Combine(Utils.GetTempFolder(), Guid.NewGuid().ToString("B").ToUpper()+".odt");
                    doc.SaveAs2(tarfile, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatOpenDocumentText);
                }
                //
                if (doc != null) doc.Close();
                if (wordApp != null) wordApp.Quit();

                Stream iStream = new FileStream(tarfile, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", filename);
            }
            catch (Exception e)
            {
                if (doc != null) doc.Close();
                if (wordApp != null) wordApp.Quit();
                return Json(new
                {
                    result = -1,
                    message = "製表失敗"
                }, JsonRequestBehavior.AllowGet);
            }

            if (doc != null) doc.Close();
            if (wordApp != null) wordApp.Quit();
            return Json(new
            {
                result = -1,
                message = "請求錯誤"
            }, JsonRequestBehavior.AllowGet);
        }
        //
        public ActionResult Download(int id, int docType)
        {
            List<SuperviseEngSchedule1VModel> engList = iService.GetEngForShcedule<SuperviseEngSchedule1VModel>(id);

            if (engList.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "無法取得資料"
                });
            }
            if (docType == 1)
                return CreateExcel(engList[0]);
            if (docType == 2 || docType == 3)
                return CreateWord(engList[0], docType);

            return Json(new
            {
                result = -1,
                message = "請求錯誤"
            }, JsonRequestBehavior.AllowGet);
        }
        private ActionResult CreateWord(SuperviseEngSchedule1VModel eng, int mode)
        {
            SuperviseUtils superviseUtils = new SuperviseUtils();
            string filename;
            try
            {
                string tempFolder = Utils.GetTempFolder();
                filename = superviseUtils.fillBaseWord(eng, mode, tempFolder);
                filename = superviseUtils.fillScheduleWord(eng, mode, tempFolder);
                filename = superviseUtils.fillFeeWord(eng, mode, tempFolder);
                filename = superviseUtils.fillSignInWord(eng, mode, tempFolder);
                filename = superviseUtils.fillCommentWord(eng, mode, tempFolder);
                filename = superviseUtils.fillRecordWord(eng, mode, tempFolder);

                //int inx = filename.LastIndexOf(".");
                //return DownloadFile(filename, filename.Substring(inx + 1, filename.Length - inx - 1));

                string zipFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("B").ToUpper() + ".zip");
                ZipFile.CreateFromDirectory(tempFolder, zipFile);// AddFiles(files, "ProjectX");
                Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", "經濟部水利署督導作業表單.zip");
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = -1,
                    message = "製表失敗"
                }, JsonRequestBehavior.AllowGet);
            }
            
        }
        private ActionResult CreateExcel(SuperviseEngSchedule1VModel eng)
        {
            string filename = CopyTemplateFile("督導統計-經濟部水利署督導作業.xlsx", ".xlsx");
            Dictionary<string, Worksheet> dict = new Dictionary<string, Worksheet>();
            Microsoft.Office.Interop.Excel.Application appExcel = null;
            Workbook workbook = null;
            //開啟 Excel 檔案
            try
            {
                appExcel = new Microsoft.Office.Interop.Excel.Application();
                workbook = appExcel.Workbooks.Open(filename);
                
                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    dict.Add(worksheet.Name, worksheet);
                }
                fillBaseSheet(dict["基本資訊表"], eng);
                fillScheduleSheet(dict["督導行程表"], eng);
                fillFeeSheet(dict["委員出席費請領單"], eng);
                fillSignInSheet(dict["簽到簿"], eng);
                fillCommentSheet(dict["督導意見表(河海組及生態委員用)"], eng);
                fillRecordSheet(dict["督導人員記錄表"], eng);
                workbook.Save();
                workbook.Close();
                appExcel.Quit();
            }
            catch(Exception e)
            {
                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                return Json(new
                {
                    result = -1,
                    message = "Excel 製表失敗"
                }, JsonRequestBehavior.AllowGet);
            }

            return DownloadFile(filename, "xlsx");
        }

        //基本資訊表
        private void fillBaseSheet(Worksheet sheet, SuperviseEngSchedule1VModel eng)
        {
            if (sheet == null) return;

            sheet.Cells[2, 3] = eng.SuperviseMode.HasValue ? SuperviseUtils.SuperviseModeStr(eng.SuperviseMode.Value) : "";// "@督導方式";
            sheet.Cells[2, 5] = eng.PhaseCode;// "@督導期別";
            sheet.Cells[3, 3] = eng.EngName;// "@督導工程名稱";
            sheet.Cells[4, 3] = eng.SuperviseDate.HasValue ? eng.SuperviseDate.Value.ToString("yyyy/M/d") : "";// "@督導日期";
            sheet.Cells[5, 3] = eng.BelongPrj;// "@計畫別";
            sheet.Cells[6, 3] = eng.ExecUnitName;// "@執行機關";
            sheet.Cells[7, 3] = eng.EngPlace;// "@督導地點";
            sheet.Cells[7, 5] = eng.EngNo;// "@標案編號";
            sheet.Cells[8, 3] = eng.LeaderName;// "@領隊";
            sheet.Cells[9, 3] = eng.OutCommittee;// "@外聘";
            sheet.Cells[10, 3] = eng.InsideCommittee;// "@內聘";
            sheet.Cells[11, 3] = eng.OfficerName;// "@幹事";
            sheet.Cells[13, 3] = eng.BriefingPlace;// "@簡報地點";
            sheet.Cells[14, 3] = eng.BriefingAddr;// "@簡報地址";
            sheet.Cells[15, 3] = eng.IsVehicleDisp ? "派車" : "無";// "@搭車資訊-署內派車";

            List<SuperviseEngTHSRVModel> lists = new SupervisePhaseService().GetEngTHSR(eng.Seq);
            string sp = "";
            string engTHSR = "";
            foreach (SuperviseEngTHSRVModel m in lists)
            {
                engTHSR += sp + m.CarNo + " " + m.Memo;
                sp = "\n";
            }
            sheet.Cells[16, 3] = engTHSR;// "@搭車資訊-建議高鐵車次";
            sheet.Cells[17, 3] = eng.AdminContact;// "@署承辦人(姓名及電話)";
            sheet.Cells[17, 5] = eng.AdminMobile;// "@署承辦人(手機)";
            sheet.Cells[18, 3] = eng.RiverBureauContact + " " + eng.RiverBureauTel;// "@河川局聯絡人(姓名及電話)";
            sheet.Cells[18, 5] = eng.RiverBureauMobile;// "@河川局聯絡人(手機)";
            sheet.Cells[19, 3] = eng.LocalGovContact + " "+ eng.LocalGovTel;// "@地方政府聯絡人(姓名及電話)";
            sheet.Cells[19, 5] = eng.LocalGovMobile;// "@地方政府聯絡人(手機)";
            sheet.Cells[20, 3] = eng.ToBriefingDrive;// "@工區至簡報地點車程(分鐘)";
            sheet.Cells[21, 3] = eng.SuperviseStartTimeStr;// "@督導開始時間";
            sheet.Cells[22, 3] = eng.SuperviseOrder.HasValue ? (eng.SuperviseOrder==1 ? "會議室簡報優先" : "現場督導優先") : "";// "@督導順序";
        }

        //督導行程表
        private void fillScheduleSheet(Worksheet sheet, SuperviseEngSchedule1VModel eng)
        {
            if (sheet == null) return;

            if (eng.SuperviseDate.HasValue)
            {
                sheet.Cells[1, 1] = String.Format("經濟部水利署{0}年{1}月份工程督導行程表", eng.SuperviseDate.Value.Year - 1911, eng.SuperviseDate.Value.Month);
            }

            sheet.Cells[2, 4] = eng.BelongPrj+"-"+eng.EngName;// "@工程名稱";
            sheet.Cells[3, 4] = eng.ExecUnitName;// "@執行機關";
            sheet.Cells[4, 4] = eng.SuperviseDate.HasValue ? eng.SuperviseDate.Value.ToString("yyyy/M/d") : "";// "@督導日期";
            string sp = "";
            string users = "";
            if(!String.IsNullOrEmpty(eng.LeaderName))
            {
                users += sp+eng.LeaderName.Substring(0,1) + "領隊" + eng.LeaderName.Substring(1, eng.LeaderName.Length-1);
                sp = ",";
            }
            if (!String.IsNullOrEmpty(eng.OutCommittee))
            {
                string[] names = eng.OutCommittee.Split(',');
                foreach (string item in names)
                {
                    users += sp + item.Substring(0, 1) + "委員" + item.Substring(1, item.Length - 1);
                    sp = ",";
                }
            }
            if (!String.IsNullOrEmpty(eng.InsideCommittee))
            {
                string[] names = eng.InsideCommittee.Split(',');
                foreach (string item in names)
                {
                    users += sp + item.Substring(0, 1) + "委員" + item.Substring(1, item.Length - 1);
                    sp = ",";
                }
            }
            if (!String.IsNullOrEmpty(eng.OfficerName))
            {
                string[] names = eng.OfficerName.Split(',');
                foreach (string item in names)
                {
                    users += sp + item.Substring(0, 1) + "幹事" + item.Substring(1, item.Length - 1);
                    sp = ",";
                }
            }
            sheet.Cells[13, 5] = users;
        }

        //委員出席費請領單
        private void fillFeeSheet(Worksheet sheet, SuperviseEngSchedule1VModel eng)
        {
            if (sheet == null) return;

            sheet.Cells[1, 10] = eng.SuperviseDate.HasValue? eng.SuperviseDate.Value.ToString("督導日期：\nyyyy年MM月dd日") : "";// "@督導日期";
            int fee = 0, inx = 0, row = 0;
            List<ExpertCommitteeModel> list = iService.GetOutCommitte<ExpertCommitteeModel>(eng.Seq);
            foreach (ExpertCommitteeModel m in list) {
                sheet.Cells[row + 5, 1] = inx+1;//"@編號";
                sheet.Cells[row + 5, 2] = m.ECName;//"@姓名";
                sheet.Cells[row + 5, 3] = 2500;//"@出席費";
                sheet.Cells[row + 5, 9] = m.ECAddr1;//"@戶籍地址";
                sheet.Cells[row + 5, 10] = m.ECMemo;// "@備註";
                sheet.Cells[row + 6, 9] = m.ECId;// "@身分證號碼";
                sheet.Cells[row + 7, 9] = m.ECBankNo;// "@匯款帳號";
                fee += 2500;
                inx++;
                row = inx*3;
                if (row > 6) break;
            }
            //sheet.Cells[14, 3] =fee;// "@小計"; 自動加總
        }

        //簽到簿
        private void fillSignInSheet(Worksheet sheet, SuperviseEngSchedule1VModel eng)
        {
            if (sheet == null) return;

            if (eng.SuperviseDate.HasValue)
            {
                sheet.Cells[1, 1] = String.Format("經濟部水利署{0}年{1}月份工程督導會議簽到簿", eng.SuperviseDate.Value.Year - 1911, eng.SuperviseDate.Value.Month);
            }

            sheet.Cells[2, 3] = eng.EngName;// "@工程名稱";
            sheet.Cells[3, 3] = eng.SuperviseDate.HasValue ? eng.SuperviseDate.Value.ToString("yyyy/M/d") : "";// "@日期";
            sheet.Cells[4, 3] = eng.EngPlace;// "@地點";
            //shioulo 20220710 下資料不填
            /*sheet.Cells[5, 3] = eng.LeaderName;// "@領隊";
            string sp = "";
            string users = "";
            if(!String.IsNullOrEmpty(eng.OutCommittee))
            {
                users += sp + eng.OutCommittee;
                sp = ",";
            }
            if (!String.IsNullOrEmpty(eng.InsideCommittee))
            {
                users += sp + eng.InsideCommittee;
            }
            sheet.Cells[6, 3] = users;// "@督導委員";
            sheet.Cells[6, 7] = eng.OfficerName;// "@幹事";*/
        }

        //督導意見表(河海組及生態委員用)
        private void fillCommentSheet(Worksheet sheet, SuperviseEngSchedule1VModel eng)
        {
            if (sheet == null) return;

            string formType = "□工程施工督導□走動式督導□專案督導 意見表";
            if(eng.SuperviseMode.HasValue)
            {
                switch(eng.SuperviseMode.Value)
                {
                    case 0: formType = "■工程施工督導□走動式督導□專案督導 意見表"; break;
                    case 1: formType = "□工程施工督導■走動式督導□專案督導 意見表"; break;
                    case 2: formType = "□工程施工督導□走動式督導■專案督導 意見表"; break;
                }
            }
            sheet.Cells[3, 1] = formType;
            sheet.Cells[4, 3] = eng.ExecUnitName;// "@受督導單位";
            //sheet.Cells[4, 5] = eng.SuperviseDate.HasValue ? eng.SuperviseDate.Value.ToString("yyyy/M/d") : "";// "@督導日期"; 自帶
            sheet.Cells[5, 3] = eng.EngName;// "@工程名稱";
            string sp = "";
            string users = "";
            if (!String.IsNullOrEmpty(eng.LeaderName))
            {
                users += sp + eng.LeaderName;
                sp = ",";
            }
            if (!String.IsNullOrEmpty(eng.OfficerName))
            {
                users += sp + eng.OfficerName;
                sp = ",";
            }
            if (!String.IsNullOrEmpty(eng.OutCommittee))
            {
                users += sp + eng.OutCommittee;
                sp = ",";
            }
            if (!String.IsNullOrEmpty(eng.InsideCommittee))
            {
                users += sp + eng.InsideCommittee;
            }
            sheet.Cells[6, 3] = users;// "@督導人員";
        }

        //督導人員記錄表
        private void fillRecordSheet(Worksheet sheet, SuperviseEngSchedule1VModel eng)
        {
            if (sheet == null) return;

            sheet.Cells[3, 4] = eng.BelongPrj;// "@列管計畫名稱";
            sheet.Cells[4, 4] = eng.EngName;// "@標案名稱";
            sheet.Cells[5, 4] = eng.SuperviseDate.HasValue ? eng.SuperviseDate.Value.ToString("yyyy/M/d") : "";// "@督導日期";
        }

        private ActionResult DownloadFile(string fullPath, string fileExt)
        {
            if (System.IO.File.Exists(fullPath))
            {
                Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", "經濟部水利署督導作業."+ fileExt);
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }
        private string CopyTemplateFile(string filename, string extFile)
        {
            string tempFile = Utils.GetTempFile(extFile);
            string srcFile = Path.Combine(Utils.GetTemplateFilePath(), filename);
            System.IO.File.Copy(srcFile, tempFile);
            return tempFile;
        }
    }
}