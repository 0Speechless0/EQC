using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace EQC.Common
{
    public class SuperviseUtils
    {
        //基本資訊表
        public string fillBaseWord(SuperviseEngSchedule1VModel eng, int mode, string tempFolder)
        {
            string tarfile = CopyTemplateFile("督導統計-基本資訊.docx", ".docx");
            Microsoft.Office.Interop.Word.Application wordApp = null;
            Document doc = null;
            try
            {
                wordApp = new Microsoft.Office.Interop.Word.Application();
                doc = wordApp.Documents.Open(tarfile);
                Table sheet = doc.Tables[1];

                sheet.Cell(2, 2).Range.Text = eng.SuperviseMode.HasValue ? SuperviseModeStr(eng.SuperviseMode.Value) : "";// "@督導方式";
                sheet.Cell(2, 4).Range.Text = eng.PhaseCode;// "@督導期別";
                sheet.Cell(3, 2).Range.Text = eng.EngName;// "@督導工程名稱";
                string sd = "";// "@督導日期" s20230316
                if (eng.SuperviseEndDate.HasValue)
                {
                    sd = String.Format("{0}~{1}",
                        eng.SuperviseDate.HasValue ? Utils.ChsDate(eng.SuperviseDate.Value) : "",
                        eng.SuperviseEndDate.HasValue ? Utils.ChsDate(eng.SuperviseEndDate.Value) : "");
                }
                else
                {
                    sd = eng.SuperviseDate.HasValue ? Utils.ChsDate(eng.SuperviseDate.Value) : "";
                }
                sheet.Cell(4, 2).Range.Text = sd;// "@督導日期";
                sheet.Cell(5, 2).Range.Text = eng.BelongPrj;// "@計畫別";
                sheet.Cell(6, 2).Range.Text = eng.ExecUnitName;// "@執行機關";
                sheet.Cell(7, 2).Range.Text = eng.EngPlace;// "@督導地點";
                sheet.Cell(7, 4).Range.Text = eng.EngNo;// "@標案編號";
                sheet.Cell(8, 3).Range.Text = eng.LeaderName;// "@領隊";
                sheet.Cell(9, 3).Range.Text = eng.OutCommittee;// "@外聘";
                sheet.Cell(10, 3).Range.Text = eng.InsideCommittee;// "@內聘";
                sheet.Cell(11, 3).Range.Text = eng.OfficerName;// "@幹事";
                sheet.Cell(13, 3).Range.Text = eng.BriefingPlace;// "@簡報地點";
                sheet.Cell(14, 3).Range.Text = eng.BriefingAddr;// "@簡報地址";
                sheet.Cell(15, 3).Range.Text = eng.IsVehicleDisp ? "派車" : "無";// "@搭車資訊-署內派車";

                List<SuperviseEngTHSRVModel> lists = new SupervisePhaseService().GetEngTHSR(eng.Seq);
                string sp = "";
                string engTHSR = "";
                foreach (SuperviseEngTHSRVModel m in lists)
                {
                    engTHSR += sp + m.CarNo + " " + m.Memo;
                    sp = "\n";
                }
                sheet.Cell(16, 3).Range.Text = engTHSR;// "@搭車資訊-建議高鐵車次";
                sheet.Cell(17, 3).Range.Text = eng.AdminContact;// "@署承辦人(姓名及電話)";
                sheet.Cell(17, 5).Range.Text = eng.AdminMobile;// "@署承辦人(手機)";
                sheet.Cell(18, 3).Range.Text = eng.RiverBureauContact + " " + eng.RiverBureauTel;// "@河川局聯絡人(姓名及電話)";
                sheet.Cell(18, 5).Range.Text = eng.RiverBureauMobile;// "@河川局聯絡人(手機)";
                sheet.Cell(19, 3).Range.Text = eng.LocalGovContact + " " + eng.LocalGovTel;// "@地方政府聯絡人(姓名及電話)";
                sheet.Cell(19, 5).Range.Text = eng.LocalGovMobile;// "@地方政府聯絡人(手機)";
                sheet.Cell(20, 3).Range.Text = eng.ToBriefingDrive.ToString();// "@工區至簡報地點車程(分鐘)";
                sheet.Cell(21, 3).Range.Text = eng.SuperviseStartTimeStr;// "@督導開始時間";
                sheet.Cell(22, 3).Range.Text = eng.SuperviseOrder.HasValue ? (eng.SuperviseOrder == 1 ? "會議室簡報優先" : "現場督導優先") : "";// "@督導順序";
                doc.Save();

                if (mode == 2)
                {
                    tarfile = Path.Combine(tempFolder, "基本資訊.pdf");
                    doc.ExportAsFixedFormat(tarfile, WdExportFormat.wdExportFormatPDF);
                }
                else if (mode == 3)
                {
                    tarfile = Path.Combine(tempFolder, "基本資訊.odt");
                    doc.SaveAs2(tarfile, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatOpenDocumentText);
                }
            }
            catch (Exception e)
            {
                BaseService.log.Info("ESSuperviseStaController.基本資訊: " + e.Message);
            }

            if (doc != null) doc.Close();
            if (wordApp != null) wordApp.Quit();
            return tarfile;
        }

        //督導行程表
        public string fillScheduleWord(SuperviseEngSchedule1VModel eng, int mode, string tempFolder)
        {
            //s20230519
            SupervisePhaseService supervisePhaseService = new SupervisePhaseService();
            List<SuperviseScheduleFormModel> scheduleForm = supervisePhaseService.GetScheduleForm(eng.Seq);
            if (scheduleForm.Count == 0)
            {
                supervisePhaseService.InitScheduleForm(eng.Seq);
                scheduleForm = supervisePhaseService.GetScheduleForm(eng.Seq);
            }
            if(scheduleForm.Count >0 && String.IsNullOrEmpty(scheduleForm[0].StartTime))
            {//s20230519
                TimeSpan dt = eng.SuperviseStartTime.Value;
                foreach(SuperviseScheduleFormModel m in scheduleForm)
                {
                    m.StartTime = dt.ToString().Substring(0, 5);
                    dt = dt.Add(new TimeSpan(0, 0, m.ActivityTime, 0));
                    m.EndTime = dt.ToString().Substring(0, 5);
                }
            }

            string tarfile = CopyTemplateFile("督導統計-督導行程表.docx", ".docx");
            Microsoft.Office.Interop.Word.Application wordApp = null;
            Document doc = null;
            try
            {
                wordApp = new Microsoft.Office.Interop.Word.Application();
                doc = wordApp.Documents.Open(tarfile);
                Table sheet = doc.Tables[1];

                if (eng.SuperviseDate.HasValue)
                {
                    Microsoft.Office.Interop.Word.Range range = sheet.Cell(1, 1).Range;
                    range.Text = String.Format("經濟部水利署{0}年{1}月份工程督導行程表", eng.SuperviseDate.Value.Year - 1911, eng.SuperviseDate.Value.Month);
                    range.Font.Name = "標楷體";
                    range.Font.Size = 16;
                    range.Font.Bold = 1;
                }

                sheet.Cell(2, 2).Range.Text = eng.BelongPrj + "-" + eng.EngName;// "@工程名稱";
                sheet.Cell(3, 2).Range.Text = eng.ExecUnitName;// "@執行機關";
                string sd = "";// "@督導日期" s20230316
                if (eng.SuperviseEndDate.HasValue)
                {
                    sd = String.Format("{0}~{1}",
                    eng.SuperviseDate.HasValue? Utils.Chs1Date(eng.SuperviseDate.Value) + $"({Utils.GetDayOfWeek(eng.SuperviseDate.Value)})" : "",
                        eng.SuperviseEndDate.HasValue? Utils.Chs1Date(eng.SuperviseEndDate.Value) + $"({Utils.GetDayOfWeek(eng.SuperviseEndDate.Value)})" : "");
                }
                else
                {
                    sd = eng.SuperviseDate.HasValue ? Utils.Chs1Date(eng.SuperviseDate.Value) + $"({Utils.GetDayOfWeek(eng.SuperviseDate.Value)})" : "";
                }
                sheet.Cell(4, 2).Range.Text = sd  ;
                
                if (eng.SuperviseStartTime.HasValue && scheduleForm.Count>0)
                {
                    int inx = 0;
                    foreach (SuperviseScheduleFormModel m in scheduleForm)
                    {//s20230519
                        sheet.Cell(6 + inx, 1).Range.Text = m.StartTime;
                        sheet.Cell(6 + inx, 3).Range.Text = m.EndTime;
                        sheet.Cell(6 + inx, 4).Range.Text = String.Format("{0} 分鐘", m.ActivityTime);
                        sheet.Cell(6 + inx, 5).Range.Text = m.Summary;
                        inx++;
                    }
                    sheet.Cell(11, 1).Range.Text = scheduleForm[scheduleForm.Count-1].EndTime; //會議結束

                    /*TimeSpan dt = eng.SuperviseStartTime.Value;
                    //50 分鐘	領隊及單位主管致詞(5分鐘)
                    sheet.Cell(6, 1).Range.Text = dt.ToString().Substring(0, 5);
                    dt = dt.Add(new TimeSpan(0, 0, 50, 0));
                    sheet.Cell(6, 3).Range.Text = dt.ToString().Substring(0, 5);
                    //90 分鐘	現勘(含來回車程約60分鐘)
                    sheet.Cell(7, 1).Range.Text = dt.ToString().Substring(0, 5);
                    dt = dt.Add(new TimeSpan(0, 0, 90, 0));
                    sheet.Cell(7, 3).Range.Text = dt.ToString().Substring(0, 5);
                    //30 分鐘	中午用餐
                    sheet.Cell(8, 1).Range.Text = dt.ToString().Substring(0, 5);
                    dt = dt.Add(new TimeSpan(0, 0, 30, 0));
                    sheet.Cell(8, 3).Range.Text = dt.ToString().Substring(0, 5);
                    //60 分鐘	文件審閱
                    sheet.Cell(9, 1).Range.Text = dt.ToString().Substring(0, 5);
                    dt = dt.Add(new TimeSpan(0, 0, 60, 0));
                    sheet.Cell(9, 3).Range.Text = dt.ToString().Substring(0, 5);
                    //70 分鐘	綜合檢討及扣點會議
                    sheet.Cell(10, 1).Range.Text = dt.ToString().Substring(0, 5);
                    dt = dt.Add(new TimeSpan(0, 0, 70, 0));
                    sheet.Cell(10, 3).Range.Text = dt.ToString().Substring(0, 5);
                    //會議結束
                    sheet.Cell(11, 1).Range.Text = dt.ToString().Substring(0, 5);*/
                }

                string sp = "";
                string users = "";
                if (!String.IsNullOrEmpty(eng.LeaderName))
                {
                    users += sp + eng.LeaderName.Substring(0, 1) + "領隊" + eng.LeaderName.Substring(1, eng.LeaderName.Length - 1);
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
                if(!String.IsNullOrEmpty(eng.InsideCommittee) && !String.IsNullOrEmpty(eng.OfficerName))
                {
                    string[] names = eng.InsideCommittee.Split(',');
                    string[] names2 = eng.OfficerName.Split(',');
                    var insideCommitteeNames =  names.GroupJoin(names2, r1 => r1, r2 => r2, (r1, r2) => r2.FirstOrDefault() == null ? r1 : null );
                    var OfficerNames = names2.GroupJoin(names, r1 => r1, r2 => r2, (r1, r2) => r2.FirstOrDefault() == null ? r1 : null);
                    var OfficerAndCommittee = names.Join(names2, r1 => r1, r2 => r2, (r1, r2) => r1);
                    if (!String.IsNullOrEmpty(eng.InsideCommittee))
                    {
                        foreach (string item in insideCommitteeNames)
                        {
                            if (item == null) continue;
                            users += sp + item.Substring(0, 1) + "委員" + item.Substring(1, item.Length - 1);
                            sp = ",";
                        }
                    }
                    if (!String.IsNullOrEmpty(eng.OfficerName))
                    {
                        foreach (string item in OfficerNames)
                        {
                            if (item == null) continue;
                            users += sp + item.Substring(0, 1) + "幹事" + item.Substring(1, item.Length - 1);
                            sp = ",";
                        }
                    }
                    if (!String.IsNullOrEmpty(eng.OfficerName) && !String.IsNullOrEmpty(eng.InsideCommittee))
                    {
                        foreach (string item in OfficerAndCommittee)
                        {
                            users += sp + item.Substring(0, 1) + "委員兼幹事" + item.Substring(1, item.Length - 1);
                            sp = ",";
                        }
                    }

                }

                sheet.Cell(13, 3).Range.Text = users;
                sheet.Cell(14, 3).Range.Text = String.Format("{0}({1})", eng.BriefingPlace, eng.BriefingAddr);// "@簡報地點(地址)";

                List<SuperviseEngTHSRVModel> lists = new SupervisePhaseService().GetEngTHSR(eng.Seq);
                sp = "";
                string engTHSR = "";
                foreach (SuperviseEngTHSRVModel m in lists)
                {
                    engTHSR += sp + m.CarNo + " " ;
                    sp = "\n";
                }
                sheet.Cell(17, 2).Range.Text = String.IsNullOrEmpty(engTHSR) ? "" : "※搭乘高鐵建議-\n" + engTHSR;

                //s20230421
                sheet.Cell(23, 3).Range.Text = String.Format("{0} {1} ({2})", eng.AdminContact, eng.AdminTel, eng.AdminMobile);
                sheet.Cell(24, 3).Range.Text = String.Format("{0} {1} ({2})", eng.RiverBureauContact, eng.RiverBureauTel, eng.RiverBureauMobile);

                doc.Save();

                if (mode == 2)
                {
                    tarfile = Path.Combine(tempFolder, "督導行程表.pdf");
                    doc.ExportAsFixedFormat(tarfile, WdExportFormat.wdExportFormatPDF);
                }
                else if (mode == 3)
                {
                    tarfile = Path.Combine(tempFolder, "督導行程表.odt");
                    doc.SaveAs2(tarfile, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatOpenDocumentText);
                } else
                {
                    System.IO.File.Copy(tarfile, Path.Combine(tempFolder, "督導行程表.docx"));
                }
            }
            catch (Exception e)
            {
                BaseService.log.Info("ESSuperviseStaController.督導行程表: " + e.Message);
            }

            if (doc != null) doc.Close();
            if (wordApp != null) wordApp.Quit();
            return tarfile;
        }

        //委員出席費請領單
        public string fillFeeWord(SuperviseEngSchedule1VModel eng, int mode, string tempFolder)
        {
            string tarfile = CopyTemplateFile("督導統計-委員出席費請領單.docx", ".docx");
            Microsoft.Office.Interop.Word.Application wordApp = null;
            Document doc = null;
            try
            {
                wordApp = new Microsoft.Office.Interop.Word.Application();
                doc = wordApp.Documents.Open(tarfile);
                Table sheet = doc.Tables[1];
                string sd = "";// "@督導日期" s20230316
                if (eng.SuperviseEndDate.HasValue)
                {
                    sd = String.Format("督導日期：\n{0}~{1}",
                        eng.SuperviseDate.HasValue ? Utils.Chs1Date( eng.SuperviseDate.Value): "",
                        eng.SuperviseEndDate.HasValue ? Utils.Chs1Date(eng.SuperviseEndDate.Value) : "");
                }
                else
                {
                    sd = eng.SuperviseDate.HasValue ? Utils.Chs1Date(eng.SuperviseDate.Value) : "";
                }
                sheet.Cell(1, 2).Range.Text = sd;// "@督導日期";
                int fee = 0, inx = 0, row = 0;
                List<ExpertCommitteeModel> list = new SuperviseStaService().GetOutCommitte<ExpertCommitteeModel>(eng.Seq);
                foreach (ExpertCommitteeModel m in list)
                {
                    sheet.Cell(row + 5, 1).Range.Text = (inx + 1).ToString();//"@編號";
                    sheet.Cell(row + 5, 2).Range.Text = m.ECName;//"@姓名";
                    sheet.Cell(row + 5, 3).Range.Text = "2,500";//"@出席費";
                    sheet.Cell(row + 5, 8).Range.Text = m.ECAddr2;//"@戶籍地址";
                    sheet.Cell(row + 5, 9).Range.Text = eng.BelongPrjNoNum;// "@備註";
                    sheet.Cell(row + 6, 8).Range.Text = m.ECId;// "@身分證號碼";
                    sheet.Cell(row + 7, 8).Range.Text = m.ECBankNo;// "@匯款帳號";
                    fee += 2500;
                    inx++;
                    row = inx * 3;
                    if (row > 6) break;
                }
                sheet.Cell(14, 2).Range.Text = fee.ToString("N0");// "@小計"; 自動加總

                doc.Save();
                if (mode == 2)
                {
                    tarfile = Path.Combine(tempFolder, "委員出席費請領單.pdf");
                    doc.ExportAsFixedFormat(tarfile, WdExportFormat.wdExportFormatPDF);
                }
                else if (mode == 3)
                {
                    tarfile = Path.Combine(tempFolder, "委員出席費請領單.odt");
                    doc.SaveAs2(tarfile, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatOpenDocumentText);
                }
                else
                {
                    System.IO.File.Copy(tarfile, Path.Combine(tempFolder, "委員出席費請領單.docx"));
                }
            }
            catch (Exception e)
            {
                BaseService.log.Info("ESSuperviseStaController.委員出席費請領單: " + e.Message);
            }

            if (doc != null) doc.Close();
            if (wordApp != null) wordApp.Quit();
            return tarfile;
        }

        //簽到簿
        public string fillSignInWord(SuperviseEngSchedule1VModel eng, int mode, string tempFolder)
        {
            string tarfile = CopyTemplateFile("督導統計-簽到簿.docx", ".docx");
            Microsoft.Office.Interop.Word.Application wordApp = null;
            Document doc = null;
            try
            {
                wordApp = new Microsoft.Office.Interop.Word.Application();
                doc = wordApp.Documents.Open(tarfile);
                Table sheet = doc.Tables[1];

                if (eng.SuperviseDate.HasValue)
                {
                    Microsoft.Office.Interop.Word.Range range = sheet.Cell(1, 1).Range;
                    range.Text = String.Format("經濟部水利署{0}年{1}月份工程督導會議簽到簿", eng.SuperviseDate.Value.Year - 1911, eng.SuperviseDate.Value.Month);
                    range.Font.Name = "標楷體";
                    range.Font.Size = 16;
                    range.Font.Bold = 1;
                }
                sheet.Cell(2, 2).Range.Text = eng.EngName;// "@工程名稱";
                string sd = "";// "@督導日期" s20230316
                if (eng.SuperviseEndDate.HasValue)
                {
                    sd = String.Format("{0}~{1}",
                        eng.SuperviseDate.HasValue ? Utils.Chs1Date(eng.SuperviseDate.Value) + $"({Utils.GetDayOfWeek(eng.SuperviseDate.Value)})" : "",
                        eng.SuperviseEndDate.HasValue ? Utils.Chs1Date(eng.SuperviseEndDate.Value) + $"({Utils.GetDayOfWeek(eng.SuperviseEndDate.Value)})" : "");
                }
                else
                {
                    sd = eng.SuperviseDate.HasValue ? Utils.Chs1Date(eng.SuperviseDate.Value) + $"({Utils.GetDayOfWeek(eng.SuperviseDate.Value)})" : "";
                }
                sheet.Cell(3, 2).Range.Text = sd;// "@日期";
                sheet.Cell(4, 2).Range.Text = eng.EngPlace;// "@地點";
                //shioulo 20220710 下資料不填
                /*sheet.Cell(5, 2).Range.Text = eng.LeaderName;// "@領隊";
                string sp = "";
                string users = "";
                if (!String.IsNullOrEmpty(eng.OutCommittee))
                {
                    users += sp + eng.OutCommittee;
                    sp = ",";
                }
                if (!String.IsNullOrEmpty(eng.InsideCommittee))
                {
                    users += sp + eng.InsideCommittee;
                }
                sheet.Cell(6, 2).Range.Text = users;// "@督導委員";
                sheet.Cell(6, 4).Range.Text = eng.OfficerName;// "@幹事";*/

                doc.Save();
                if (mode == 2)
                {
                    tarfile = Path.Combine(tempFolder, "簽到簿.pdf");
                    doc.ExportAsFixedFormat(tarfile, WdExportFormat.wdExportFormatPDF);
                }
                else if (mode == 3)
                {
                    tarfile = Path.Combine(tempFolder, "簽到簿.odt");
                    doc.SaveAs2(tarfile, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatOpenDocumentText);
                }
                else
                {
                    System.IO.File.Copy(tarfile, Path.Combine(tempFolder, "簽到簿.docx"));
                }
            }
            catch (Exception e)
            {
                BaseService.log.Info("ESSuperviseStaController.簽到簿: " + e.Message);
            }

            if (doc != null) doc.Close();
            if (wordApp != null) wordApp.Quit();
            return tarfile;
        }

        //督導意見表(河海組及生態委員用)
        public string fillCommentWord(SuperviseEngSchedule1VModel eng, int mode, string tempFolder)
        {
            string tarfile = CopyTemplateFile("督導統計-督導意見表.docx", ".docx");
            Microsoft.Office.Interop.Word.Application wordApp = null;
            Document doc = null;
            try
            {
                wordApp = new Microsoft.Office.Interop.Word.Application();
                doc = wordApp.Documents.Open(tarfile);
                Table sheet = doc.Tables[1];

                string formType = "□工程施工督導□走動式督導□專案督導 意見表";
                if (eng.SuperviseMode.HasValue)
                {
                    switch (eng.SuperviseMode.Value)
                    {
                        case 0: formType = "■工程施工督導□走動式督導□專案督導 意見表"; break;
                        case 1: formType = "□工程施工督導■走動式督導□專案督導 意見表"; break;
                        case 2: formType = "□工程施工督導□走動式督導■專案督導 意見表"; break;
                    }
                }
                sheet.Cell(1, 1).Range.Text = formType;
                sheet.Cell(2, 2).Range.Text = eng.ExecUnitName;// "@受督導單位";
                string sd = "";// "@督導日期" s20230316
                if (eng.SuperviseEndDate.HasValue)
                {
                    sd = String.Format("{0}~{1}",
                        eng.SuperviseDate.HasValue ? Utils.Chs1Date(eng.SuperviseDate.Value) : "",
                        eng.SuperviseEndDate.HasValue ? Utils.Chs1Date(eng.SuperviseDate.Value) : "");
                }
                else
                {
                    sd = eng.SuperviseDate.HasValue ? Utils.Chs1Date(eng.SuperviseDate.Value) : "";
                }
                sheet.Cell(2, 4).Range.Text = sd;// "@督導日期";
                sheet.Cell(3, 2).Range.Text = eng.EngName;// "@工程名稱";
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
                sheet.Cell(4, 2).Range.Text = users;// "@督導人員";

                doc.Save();
                if (mode == 2)
                {
                    tarfile = Path.Combine(tempFolder, "督導意見表.pdf");
                    doc.ExportAsFixedFormat(tarfile, WdExportFormat.wdExportFormatPDF);
                }
                else if (mode == 3)
                {
                    tarfile = Path.Combine(tempFolder, "督導意見表.odt");
                    doc.SaveAs2(tarfile, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatOpenDocumentText);
                }
                else
                {
                    System.IO.File.Copy(tarfile, Path.Combine(tempFolder, "督導意見表.docx"));
                }
            }
            catch (Exception e)
            {
                BaseService.log.Info("ESSuperviseStaController.督導意見表: " + e.Message);
            }

            if (doc != null) doc.Close();
            if (wordApp != null) wordApp.Quit();
            return tarfile;
        }

        //督導人員記錄表
        public string fillRecordWord(SuperviseEngSchedule1VModel eng, int mode, string tempFolder)
        {
            string tarfile = CopyTemplateFile("督導統計-督導人員紀錄表.docx", ".docx");
            Microsoft.Office.Interop.Word.Application wordApp = null;
            Document doc = null;
            try
            {
                wordApp = new Microsoft.Office.Interop.Word.Application();
                doc = wordApp.Documents.Open(tarfile);
                Table sheet = doc.Tables[1];

                sheet.Cell(1, 2).Range.Text = eng.BelongPrj;// "@列管計畫名稱";
                sheet.Cell(2, 2).Range.Text = eng.EngName;// "@標案名稱";
                string sd = "";// "@督導日期" s20230316
                if (eng.SuperviseEndDate.HasValue)
                {
                    sd = String.Format("{0}~{1}",
                        eng.SuperviseDate.HasValue ? Utils.Chs1Date(eng.SuperviseDate.Value) + $"({Utils.GetDayOfWeek(eng.SuperviseDate.Value)})" : "",
                        eng.SuperviseEndDate.HasValue ? Utils.Chs1Date(eng.SuperviseEndDate.Value) + $"({Utils.GetDayOfWeek(eng.SuperviseEndDate.Value)})" : "");
                }
                else
                {
                    sd = eng.SuperviseDate.HasValue ? Utils.Chs1Date(eng.SuperviseDate.Value) + $"({Utils.GetDayOfWeek(eng.SuperviseDate.Value)})" : "";
                }
                sheet.Cell(3, 2).Range.Text = sd;// "@督導日期";

                doc.Save();
                if (mode == 2)
                {
                    tarfile = Path.Combine(tempFolder, "督導人員紀錄表.pdf");
                    doc.ExportAsFixedFormat(tarfile, WdExportFormat.wdExportFormatPDF);
                }
                else if (mode == 3)
                {
                    tarfile = Path.Combine(tempFolder, "督導人員紀錄表.odt");
                    doc.SaveAs2(tarfile, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatOpenDocumentText);
                }
                else
                {
                    System.IO.File.Copy(tarfile, Path.Combine(tempFolder, "督導人員紀錄表.docx"));
                }
            }
            catch (Exception e)
            {
                BaseService.log.Info("ESSuperviseStaController.督導人員紀錄表: " + e.Message);
            }

            if (doc != null) doc.Close();
            if (wordApp != null) wordApp.Quit();
            return tarfile;
        }

        //督導方式 代碼2文字
        public static string SuperviseModeStr(byte mode)
        {
            switch (mode)
            {
                case 0: return "工程施工督導"; break;
                case 1: return "走動式督導"; break;
                case 2: return "異常工程督導"; break;
                default: return "";
            }
        }

        public string CopyTemplateFile(string filename, string extFile)
        {
            string tempFile = Utils.GetTempFile(extFile);
            string srcFile = Path.Combine(Utils.GetTemplateFilePath(), filename);
            System.IO.File.Copy(srcFile, tempFile);
            return tempFile;
        }
    }
}