using EQC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Services;
using EQC.Models;
using EQC.ViewModel;
using EQC.EDMXModel;
using System.IO;
using NPOI.HSSF.Util;
using Microsoft.VisualBasic.FileIO;
using NPOI.SS.Util;

namespace EQC.Controllers
{
    [SessionFilter]
    public class TreeController : MyController
    {
        private readonly TreePlantService service;
        private readonly UserInfo _user;
        public TreeController()
        {
            service = new TreePlantService();
            _user = new SessionManager().GetUser();

        }
        // GET: Tree
        [Route("Tree/new")]
        public ActionResult Index(string route = "new")
        {
            Utils.setUserClass(this);
            ViewBag.noHeader = true;
            ViewBag.route = route;
            ViewBag.Title = "新增案件(植樹專區)";
            //ViewBag.mainLeftClass = "main-left menu_bg_B";
            //ViewBag.navClass = "navbar navbar-expand-lg fixed-top navbar-bg-B";
            //ViewBag.cardClass = "card whiteBG mb-4 colorset_B";
            ViewBag.hasCard = false;

            return View("Index");
        }

        [Route("Tree/list")]
        public ActionResult list(string route = "list")
        {
            Utils.setUserClass(this);
            ViewBag.noHeader = true;
            ViewBag.route = route;
            ViewBag.Title = "植樹總彙整表";
            //ViewBag.navClass = "navbar navbar-expand-lg fixed-top navbar-bg-B";
            //ViewBag.mainLeftClass = "main-left menu_bg_B";
            //ViewBag.cardClass = "card whiteBG mb-4 colorset_B";
            ViewBag.hasCard = false;
            return View("Index");
        }

        public JsonResult GetTenderListByUser(int page = 1, int perPage = 10, int createType = 1, int? year = null, string subUnit = "", string engNameKeyWord = "")
        {

            year = year ?? DateTime.Now.Year - 1911;
            var list = service
                .GetTenderListByUser(createType)
                //.Where(row => row.EngYear >= DateTime.Now.Year - 1913)
                .ToList();
            var aflterYearFilteList = list
                    .Where(row => row.EngName.Contains(engNameKeyWord) || engNameKeyWord == "" )
                    .Where(row => row.execUnitName == subUnit || subUnit == "")
                    .Where(row => (row.EngYear == year && createType ==1 ) || 
                        (Convert.ToDateTime(row.ActualBidAwardDate).Year -1911  == year && createType == 2) || 
                        year == 0)
                    .ToList();
            var a = list.Select(row => row.execUnitName);
            var b = a.OrderBy(row => row);
            return Json(new {
                list = aflterYearFilteList

                    .GetRange((page - 1) * perPage,
                    (page - 1) * perPage + perPage > aflterYearFilteList.Count
                    ? aflterYearFilteList.Count - (page - 1) * perPage : perPage),
                count = aflterYearFilteList.Count,
                yearOption = service.GetTenderListYearOption(list, createType),
                unitOption = list
                .OrderBy(row => row.execUnitSeq ?? int.MaxValue )
                .Select(row => row.execUnitName)
                .Distinct()
     
            });
        }



        public JsonResult GetSelectTender(int engSeq, int createType)
        {
            return Json(service.GetTender(engSeq, createType));
        }
        public JsonResult GetEng(int engSeq, int createType)
        {
            return Json(new
            {
                eng = service.GetTender(engSeq, createType),
                engTypeOption = service.GetTreePlantEngTypes(createType),
                treeTypOption = service.GetTreePlantTypes()

            });
        }
        public JsonResult GetEngEditList(int page, int perPage, int year = 0, int TPEngTypeSeq = 0, string[] subUnit = null)
        {
            using(var context = new EQC_NEW_Entities())
            {
                var originList = service.getTreePlantMainList()
                    //.Where(row => row.EngYear >= DateTime.Now.Year - 1913)
                    .GroupJoin(context.Unit, l => l.ExecUnitSeq, unit => unit.Seq,
                        (l, unit) => {
                            l.OrderNo = unit.FirstOrDefault()?.OrderNo ;
                            return l;
                        })
                    .OrderBy(row => row.OrderNo)
                    .ToList();
                var list = originList

                    .Where(row => (row.EngYear == year || year == 0))
                    .Where(row => row.TPEngTypeSeq == TPEngTypeSeq || TPEngTypeSeq == 0)
                    .Where(row => row.execUnitName == subUnit[0] || subUnit[0] == "")
                    .Where(row => row.execSubUnitName == subUnit[1] || subUnit[1] == "")
                    .ToList();

                return Json(new
                {
                    list = list.GetRange((page - 1) * perPage,
                        (page - 1) * perPage + perPage > list.Count
                        ? list.Count - (page - 1) * perPage : perPage),
                    count = list.Count,
                    yearOptions = service.TreePlantYearOption(originList),
                    unitOptions = service.TreePlantUnitOption(originList, subUnit ?? new string[] { "", "" }),
                    TPEngTypeOption = service.TPEngTypeOption(originList)
                });
            }

        }

        public JsonResult getTreeList()
        {
            return Json(service.getTreeList());
        }

        public JsonResult getTreePlantMain(int createType, int treePlantMainEngSeq = 0, int treeMainSeq = 0)
        {
            return Json(service.getTreePlantMain(createType, treePlantMainEngSeq, treeMainSeq));
        }
        public JsonResult insertTreeMain(TreePlantMain m, int engSeq)
        {
            if (m.EngCreatType == 2)
            {
                var TownCity = service.GetTender(m.EngSeq ?? 0, 2).execUnitName;
                service.setExecUnitSeq(m, TownCity);
            }
            int insertSeq = service.insertTreeMain(m, engSeq);

            return Json(new { insertSeq = insertSeq});
        }

        public JsonResult insertTreeMainSelf(TreePlantMain m)
        {
            int insertSeq = service.insertTreeMain(m);
            return Json(new { insertSeq = insertSeq });
        }
        public JsonResult updateTreeMain(TreePlantMain m)
        {
            if(m.EngCreatType == 2)
            {
                var TownCity = service.GetTender(m.EngSeq ?? 0, 2).execUnitName;
                service.setExecUnitSeq(m, TownCity);
            }

            service.updateTreeMain(m);
            
            return Json(true);
        }

        public  JsonResult removeTreeMain(int id)
        {
            service.deleteTreeMain(id);
            return Json(true);
        }

        public JsonResult getTreePlantMonths(int engSeq = 0, int treePlantMainSeq = 0)
        {
            return Json(
                service.getTreePlantMonths(engSeq, treePlantMainSeq)?
                            .Select(row => {
                                row.TreePlantMain = null;
                                return row;
                            } ) 
                );
        }


        public void getTreePlantMainFromPcces(int engSeq)
        {

            EQC.EDMXModel.EngMain eng = null;
            using (var context = new EQC_NEW_Entities())
            {
                eng = context.EngMain.Find(engSeq);

            }


            //優化版
            var service = new TreePlantService(engSeq);
            List<TreePlantMonth> monthsAreaSumList;
            List<TreePlantMonth> result = new List<TreePlantMonth>();
            var userSeq = new SessionManager().GetUser().Seq;
            decimal[] monthsAreaSumList1;
            decimal[] monthsAreaSumList2;
            int[] monthsAclNumSumList;
            int i = 0;

            //依預定進度判斷資料長度
            int year =  eng.EngYear ?? (DateTime.Now.Year - 1911) ;

            TreePlantMain treeMain = null; 

            treeMain = new TreePlantMain() { 
                EngCreatType = 1
            };




            monthsAreaSumList1 = service
                .getTreePlantSchMonthsAreaSumFromPcces(engSeq, year, treeMain)
                .Skip((treeMain.ScheduledPlantDate?.Month - 1) ?? 0)
                .ToArray();
            int startMonth;
            int monthDiff;
            do
            {

                startMonth =
                    service.getTreePlantYearStartMonth(treeMain, year);
                int endMonth =
                    service.getTreePlantYearEndMonth(treeMain, year);

                monthsAreaSumList1 =
                    monthsAreaSumList1.Take(
                        endMonth - startMonth + 1)
                    .ToArray();




                monthsAreaSumList =
                monthsAreaSumList1
                    .Select(num => new TreePlantMonth
                    {
                        Year = year,
                        ScheduledArea = num
                    }).ToList();

                int j = 0;
                monthsAreaSumList2 = service
                    .getTreePlantAclMonthsAreaSumFromPcces(engSeq, year, treeMain);


                monthsAreaSumList.ForEach(e => e.ActualArea = monthsAreaSumList2[startMonth + (j++) - 1]);
                monthsAclNumSumList = service
                    .getAclNumProgressMonthSumFromPcces(engSeq, year);
                j = 0;
                monthsAreaSumList.ForEach(e => e.ActualPlantNum = monthsAclNumSumList[startMonth + (j++) - 1]);
                result.AddRange(monthsAreaSumList);

                year = (eng.EngYear ?? (DateTime.Now.Year - 1911)) + (++i);
                monthsAreaSumList1 = service
                    .getTreePlantSchMonthsAreaSumFromPcces(engSeq, year, treeMain);

            } while (year <= ((treeMain.ScheduledCompletionDate?.Year - 1911) ?? year - 1));


            var treeSchNumUpdateList =
                service.getTreePlantNumFromPcces(engSeq)
                .ToList();
            var treeAclNumUpdateList =
                service.getTreePlantAclNumFromPcces(engSeq)
                .ToList();

            var treeNumHandledList =
                treeSchNumUpdateList.Join(treeAclNumUpdateList,
                    row => row.Key,
                    row => row.Key,
                    (schRow, aclRow) => new TreePlantNumList
                    {
                        ScheduledPlantNum = schRow.Value,
                        ActualPlantNum = aclRow.Value,
                        TreeTypeSeq = schRow.Key,
                        ModifyTime = DateTime.Now,
                        ModifyUserSeq = userSeq,
                    }
                ).ToList();


            //service.insertTreePlantMonth(
            //    result, engSeq);


            //service.insertTreePlantNumList(
            //    treeNumHandledList,
            //    engSeq
            //);
            ; treeMain.EngMain = null;
            treeMain.ScheduledPlantDate =  DateTime.Parse($"{eng.EngYear + 1911}-1-1");
            treeMain.ScheduledCompletionDate = treeMain.ScheduledPlantDate?.AddMonths(result.Count);
            ResponseJson(new
            {
                monthDiff  = result.Count,
                startMonth = startMonth,
                treePlantMain = treeMain,
                treeMonthList = result,
                treeNumList = treeNumHandledList
            });
            

        }
        public JsonResult UpdateTreePlantDataFromPcces(int engSeq, int treeMainSeq, int? EngYear = null, bool update = false)
        {

            //優化版
            var service = new TreePlantService(engSeq);
            List<TreePlantMonth> monthsAreaSumList;
            List<TreePlantMonth> result = new List<TreePlantMonth>();
            var userSeq = new SessionManager().GetUser().Seq;
            decimal[] monthsAreaSumList1;
            decimal[] monthsAreaSumList2;
            int[] monthsAclNumSumList;
            int i = 0;

            //依預定進度判斷資料長度
            int year =  EngYear ?? (DateTime.Now.Year - 1911) ;


            TreePlantMain treeMain = service.getTreePlantMain(1, 0, treeMainSeq);
            monthsAreaSumList1 = service
                .getTreePlantSchMonthsAreaSumFromPcces(engSeq, year)
                .Skip( (treeMain.ScheduledPlantDate?.Month -1 ) ?? 0)
                .ToArray();
            do
            {

                int startMonth =
                    service.getTreePlantYearStartMonth(treeMain, year) ;
                int endMonth =
                    service.getTreePlantYearEndMonth(treeMain, year);

                monthsAreaSumList1 =
                    monthsAreaSumList1.Take(
                        endMonth - startMonth + 1 )
                    .ToArray();




                monthsAreaSumList =
                monthsAreaSumList1
                    .Select(num => new TreePlantMonth
                    {
                        Year = year,
                        ScheduledArea = num
                    }).ToList();

                int j = 0;
                monthsAreaSumList2 = service
                    .getTreePlantAclMonthsAreaSumFromPcces(engSeq, year);


                monthsAreaSumList.ForEach(e => e.ActualArea = monthsAreaSumList2[startMonth + (j++) -1]);
                monthsAclNumSumList = service
                    .getAclNumProgressMonthSumFromPcces(engSeq, year);
                j = 0;
                monthsAreaSumList.ForEach(e => e.ActualPlantNum = monthsAclNumSumList[startMonth + (j++) - 1]);
                result.AddRange(monthsAreaSumList);

                year = (EngYear ?? (DateTime.Now.Year - 1911 ) ) + (++i);
                monthsAreaSumList1 = service
                    .getTreePlantSchMonthsAreaSumFromPcces(engSeq, year);

            } while (year <= ( (treeMain.ScheduledCompletionDate?.Year-1911) ?? year-1) );
            
            var treeSchNumUpdateList =
                service.getTreePlantNumFromPcces(engSeq)
                .ToList();
            var treeAclNumUpdateList =
                service.getTreePlantAclNumFromPcces(engSeq)
                .ToList();

            var treeNumHandledList =
                treeSchNumUpdateList.Join(treeAclNumUpdateList,
                    row => row.Key,
                    row => row.Key,
                    (schRow, aclRow) => new TreePlantNumList
                    {
                        ScheduledPlantNum = schRow.Value,
                        ActualPlantNum = aclRow.Value,
                        TreePlantSeq = treeMainSeq,
                        TreeTypeSeq = schRow.Key,
                        ModifyTime = DateTime.Now,
                        ModifyUserSeq = userSeq,
                    }
                ).ToList();

            if (update)
            {

                service.updateTreePlantMonth(result, engSeq);
                service.updateTreePlantNumList(treeNumHandledList
                , engSeq);
            }
            else{
                service.insertTreePlantMonth(
                    result, engSeq);
                service.insertTreePlantNumList(
                    treeNumHandledList, 
                    engSeq
                );
            }
            return Json(true);

        }
        public JsonResult getTreePlantMonthsPcces(int engSeq)
        {
            //優化版
            var service = new TreePlantService(engSeq);
            List<SchEngProgressPayItem> payItems = service.getPayItem(engSeq);


            if(payItems == null || payItems.Count == 0)
            {
                return Json(-1);
            }
            List<int> aclNum = service.getTreePlantXMLMonthsAclNum(engSeq, payItems.Count);
            List<string> aclValue = service.getTreePlantXMLMonthsAcl(engSeq, payItems, payItems.Count);
            List<int> schNum =
                service.getTreePlantXMLMonthsSchNum(engSeq, payItems.Count);
            List<string> schValue = service.getTreePlantXMLMonthsSch(engSeq, payItems, payItems.Count);


            if (aclNum.Count != schNum.Count) return Json(-2);

            List<TreePlantMonth> resList = new List<TreePlantMonth>();
            List<TreeList> treeList = service.getTreeList();

            for(int j = 0; j< aclNum.Count; j ++)
            {
                resList.Add(new TreePlantMonthVM()
                {
                    
                    ActualPlantNum= aclNum[j],
                    ScheduledPlant = schValue[j],
                    ScheduledPlantNum = schNum[j]

                });
            }
            int i = -1;
            int AccActualNum = 0;
            int AccSchNum = 0;
            var a = resList.Select(row =>
            {
                i++;
                AccSchNum += schNum[i];
                AccActualNum += aclNum[i];
                return new
                {
                    PayItem = payItems[i].PayItem,
                    TreeName = treeList.Find(row2 => payItems[i].RefItemCode.StartsWith(row2.Type + row2.Code))?.Name,
                    Unit = payItems[i].Unit,    
                    TreeAmount = payItems[i].Amount,
                    Quality = payItems[i].Quantity,
                    ActualPlantNum = aclNum[i],
                    ActualPlantValue = aclValue[i] ?? "",
                    AccSchNum = AccSchNum,
                    AccActualNum = AccActualNum,
                    ScheduledPlantNum = schNum[i],
                    ScheduledPlantValue = schValue[i] ?? ""
                };
            }).ToList();
            return Json(a );
        }

        public JsonResult insertTreePlantMonths(List<TreePlantMonth> list, int engSeq = 0, int treePlantSeq =0)
        {
            service.insertTreePlantMonth(list ?? new List<TreePlantMonth>(), engSeq, treePlantSeq);
            return Json(true);
        }
        public JsonResult updateTreePlantMonths(List<TreePlantMonth> list, int engSeq = 0, int treePlantSeq = 0)
        {
            service.updateTreePlantMonth(list ?? new List<TreePlantMonth>(), engSeq, treePlantSeq);
            return Json(true);
        }

        public JsonResult insertTreeNumList(List<TreePlantNumList> list, int engSeq = 0, int treePlantSeq = 0)
        {
            
            service.insertTreePlantNumList(list?? new  List<TreePlantNumList>() , engSeq, treePlantSeq);
            return Json(true);

        }

        public JsonResult updateTreeNumList(List<TreePlantNumList> list, int engSeq = 0, int treePlantSeq = 0)
        {
            service.updateTreePlantNumList(list ?? new List<TreePlantNumList>(), engSeq, treePlantSeq);
            return Json(true);
        }

        public JsonResult deleteTreeNumList(int treeNumSeq)
        {
            service.deleteTreePlantNumList(treeNumSeq);
            return Json(true);
        }

        public JsonResult getTreeNumList(int engSeq = 0 , int treePlantMainSeq = 0)
        {
            return Json(
            service.getTreePlantNumList(engSeq, treePlantMainSeq)?
            .Select(row => {
                row.TreePlantMain = null;
                return row;
                }) 
            );
        }
        public void getTreeCollectionArea(int? Year = null, string ExecUnitName = "")
        {
            FileStream fs = new FileStream(Path.Combine(Utils.GetTemplateFilePath(), "TreeSchAreaTreeCollection.xlsx"), FileMode.Open, FileAccess.Read, FileShare.Read);
            int year = Year ?? ( DateTime.Now.Year - 1911);
            ExcelProcesser processer = new ExcelProcesser(fs, 1)
                .setSheet(0)
                .setSheetCell(0, 21, $"資料統計至{year}.{DateTime.Now.Month}.{DateTime.Now.Day}")
                .setSheetCell(0, 1, $"附件4-({year}年度)植樹各月份預期種植面積總彙整表 ");
            var service = new TreePlantService();
            List<TreePlantMainVM> list = service.getAllTreePlantMain()
                .Where(r => r._row.ScheduledPlantDate?.Year - 1911 <= Year && r._row.ScheduledCompletionDate?.Year - 1911 >= Year).ToList();
            int groupIndex = 1; int groupRowIndex = 1;

            List<int> sumRowIndex = new List<int>();

            Dictionary<string, List<TreePlantMainVM>> treeUnitSlot;
            using (var context = new EQC_NEW_Entities())
            {
                //所屬機關資料插槽
                treeUnitSlot = context

                    .Unit
                    .Join(context.TreePlantMain, e => e.Seq, e2 => e2.ExecUnitSeq, (e, e2) => e)

                    .OrderBy(r => r.OrderNo)
                    .Where(r => r.ParentSeq == null)
                    .Select(r => r.Name).ToList()
                    .Distinct()
                    .ToDictionary(r => r, r => new List<TreePlantMainVM>());
            }
            list = list
                .OrderBy(row => row.OrderNo)
                .Where(row => row.execUnitName == ExecUnitName || ExecUnitName == "")
                .ToList();

            foreach (var row in list)
            {
                if (treeUnitSlot.ContainsKey(row.execUnitName))
                    treeUnitSlot[row.execUnitName].Add(row);
            }
            treeUnitSlot
                .ToList()
                .ForEach(pair => {
                    var e = pair.Value;
                    if(e.Count() == 0 )
                    {
                        groupIndex++;
                        return ;
                    }
                    groupRowIndex = 0;
                    processer.copyOutSideRowStyle(1, 0, e.Count() + 1);
                    processer.insertOneCol(
                        e.Select(r => $"{groupIndex}-{++groupRowIndex}")
                        , 0
                    );

                    processer.insertOneCol(
                        e.Select(r => r.execUnitName)
                        , 1);

                    processer.insertOneCol(
                        e.Select(r => r._row.EngName)
                        , 2);

                    processer.insertOneCol(
                        e.Select(r => r.TPEngTypeName)
                        , 3);
                    processer.insertOneCol(
                        e.Select(r => service.getTreeSchArea(r._row.Seq, year))
                    , 4);
                    processer.insertOneCol(
                        e.Select(r => r.ScheduledPlantDateStr)
                        , 5);
                    processer.insertOneCol(
                        e.Select(r => r.ScheduledCompletionDateStr)
                        , 6);
                    processer.insertOneCol(
                        e.Select(r => service.getTreeAclArea(r._row.Seq, year).ToString() )
                        , 7);
                    var treePlantsSchMonthsSum = e.Select(treeMain =>
                        service.getTreePlantSchMonthsAreaSum(treeMain._row.Seq, year));
                    processer.insertSection(
                        treePlantsSchMonthsSum
                             
                         , 8);
                    processer.insertOneCol(
                        treePlantsSchMonthsSum.Select(row => row.Sum()), 20);
                    processer.insertOneCol(
                        e.Select(r => r._row.ExecutionStatusDescription)
                    , 21);

                    processer.fowardStartRow(groupRowIndex);

                    int lastSumRow = sumRowIndex.Count > 0 ? sumRowIndex.Last() : 2; 

                    string[] sumRow = new string[21];
                    sumRow[1] = e.FirstOrDefault().execUnitName ?? "";
                    sumRow[2] = "合計";

                    sumRow[4] = processer.getSumFormula(lastSumRow+1 , groupRowIndex, "E");
                    sumRow[5] = processer.getSumFormula(lastSumRow+1 , groupRowIndex, "F");
                    sumRow[6] = processer.getSumFormula(lastSumRow+1 , groupRowIndex, "G");
                    sumRow[7] = processer.getSumFormula(lastSumRow+1 , groupRowIndex, "H");
                    sumRow[8] = processer.getSumFormula(lastSumRow+1 , groupRowIndex, "I");
                    sumRow[9] = processer.getSumFormula(lastSumRow+1 , groupRowIndex, "J");
                    sumRow[10] = processer.getSumFormula(lastSumRow+1 , groupRowIndex, "K");
                    sumRow[11] = processer.getSumFormula(lastSumRow+1 , groupRowIndex, "L");
                    sumRow[12] = processer.getSumFormula(lastSumRow+1 , groupRowIndex, "M");
                    sumRow[13] = processer.getSumFormula(lastSumRow+1 , groupRowIndex, "N");
                    sumRow[14] = processer.getSumFormula(lastSumRow+1 , groupRowIndex, "O");
                    sumRow[15] = processer.getSumFormula(lastSumRow+1 , groupRowIndex, "P");
                    sumRow[16] = processer.getSumFormula(lastSumRow+1 , groupRowIndex, "Q");
                    sumRow[17] = processer.getSumFormula(lastSumRow+1 , groupRowIndex, "R");
                    sumRow[18] = processer.getSumFormula(lastSumRow+1 , groupRowIndex, "S");
                    sumRow[19] = processer.getSumFormula(lastSumRow+1 , groupRowIndex, "T");
                    sumRow[20] = processer.getSumFormula(lastSumRow+1 , groupRowIndex, "U");

                    sumRowIndex.Add(lastSumRow + groupRowIndex + 1);
                    processer.insertRowContent(
                        sumRow
                        ,
                        HSSFColor.LightOrange.Index,
                        HSSFColor.Red.Index
                    );
                    groupIndex++;
                });
            string[] totalRow = new string[21];
            totalRow[2] = "合計";
            totalRow[4] = processer.getPlusFormula(sumRowIndex, "E");
            totalRow[7] = processer.getPlusFormula(sumRowIndex, "H");
            totalRow[8] = processer.getPlusFormula(sumRowIndex, "I");
            totalRow[9] = processer.getPlusFormula(sumRowIndex, "J");
            totalRow[10] = processer.getPlusFormula(sumRowIndex, "K");
            totalRow[11] = processer.getPlusFormula(sumRowIndex, "L");
            totalRow[12] = processer.getPlusFormula(sumRowIndex, "M");
            totalRow[13] = processer.getPlusFormula(sumRowIndex, "N");
            totalRow[14] = processer.getPlusFormula(sumRowIndex, "O");
            totalRow[15] = processer.getPlusFormula(sumRowIndex, "P");
            totalRow[16] = processer.getPlusFormula(sumRowIndex, "Q");
            totalRow[17] = processer.getPlusFormula(sumRowIndex, "R");
            totalRow[18] = processer.getPlusFormula(sumRowIndex, "S");
            totalRow[19] = processer.getPlusFormula(sumRowIndex, "T");
            totalRow[20] = processer.getPlusFormula(sumRowIndex, "U");
            processer.copyOutSideRowStyle(1, 0, 1);
            processer.insertRowContent(
                totalRow
                ,
                HSSFColor.LightGreen.Index,
                HSSFColor.Red.Index
            );
            processer.removeSheet(1);
            Response.AddHeader("Content-Disposition", $"attachment; filename=植樹面積表{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx");
            Response.BinaryWrite(processer.getTemplateStream().ToArray());

        }
        public void getTreeCollection(int? Year = null, string ExecUnitName= "")
        {
            FileStream fs = new FileStream(Path.Combine(Utils.GetTemplateFilePath(), "TreeCollection.xlsx"), FileMode.Open, FileAccess.Read, FileShare.Read);
            int year = Year ?? (DateTime.Now.Year - 1911 );

            ExcelProcesser processer = new ExcelProcesser(fs, 1)
                .setSheet(1)
                .setSheetCell(0, 21, $"資料統計至{year}.{DateTime.Now.Month}.{DateTime.Now.Day}")
                .setSheetCell(0, 1, $"附件3-({year}年度)植樹總彙整表   ");

            List<TreePlantMainVM> list = service.getAllTreePlantMain()
                .Where(r => r._row.ScheduledPlantDate?.Year -1911 <= Year && r._row.ScheduledCompletionDate?.Year - 1911 >= Year)
                .ToList(); ;
            List<string> unitList = new List<string>();
            Dictionary<string, List<TreePlantMainVM>> treeUnitSlot;
            using (var context = new EQC_NEW_Entities())
            {
                //所屬機關資料插槽
                treeUnitSlot = context

                    .Unit
                    .Join(context.TreePlantMain, e => e.Seq, e2 => e2.ExecUnitSeq, (e, e2) => e)

                    .OrderBy(r => r.OrderNo)
                    .Where(r => r.ParentSeq == null)
                    .Select(r => r.Name).ToList()
                    .Distinct()
                    .ToDictionary(r => r, r => new List<TreePlantMainVM>() ) ;
            }

            List<int> sumRowIndex = new List<int>();
            var SummarySheet = processer.getSheet(0);
            int groupIndex = 1;int groupRowIndex = 0;
            int summaryCurrentRowIndex = 2;
            var execUnitTreeGroup = list.GroupBy(t => t.execUnitName)
                .Where(row => row == null || row.Key == ExecUnitName || ExecUnitName == "");

            foreach(var treeUnitGroup in  execUnitTreeGroup)
            {
                if(treeUnitSlot.ContainsKey(treeUnitGroup.Key))
                    treeUnitSlot[treeUnitGroup.Key].AddRange(treeUnitGroup);
            }

;

            treeUnitSlot.ToList().ForEach(pair => {

                var e = pair.Value;
                if(e.Count() == 0 )
                {
                    processer.insertRange(CellRangeAddress.ValueOf("A1:N3"),  processer.getSheet(3), summaryCurrentRowIndex, SummarySheet);
                    var row = SummarySheet.GetRow(summaryCurrentRowIndex);


                    row.GetCell(0).SetCellValue(groupIndex);
                    row.GetCell(1).SetCellValue(pair.Key);
                    summaryCurrentRowIndex += 3;
                    groupIndex++;
                    return ;
                }

                decimal[] treePlantComplateArea = new decimal[e.Count()];
                Dictionary<string, int>[] treePlantTypeAclNum = new Dictionary<string, int>[e.Count()];
                Dictionary<string, int>[] treePlantTypeNum = new Dictionary<string, int>[e.Count()];
                Dictionary<string, string>[] treeNumExport = new Dictionary<string, string>[e.Count()];
                string[] NonNormalTreeNumExport = new string[e.Count()];
                int j = 0;
                foreach (var i in e)
                {
                    treePlantTypeAclNum[j] = service
                        .getTreePlantTypeAclNum(i._row.Seq);
                    treePlantTypeNum[j] = service
                        .getTreePlantTypeSchNum(i._row.Seq);
                    treeNumExport[j] = service.getTreeNumExport(i._row.Seq);
                    //NonNormalTreeNumExport[j] = service.getNonNormalTreeNumExport(i._row.Seq);
                    j++;

                }

                groupRowIndex = 0;
                processer.copyOutSideRowStyle(2, 0, e.Count()+1);
                processer.insertOneCol(
                    e.Select(r => $"{groupIndex}-{++groupRowIndex}")
                    , 0
                );

                processer.insertOneCol(
                    e.Select(r => r.execUnitName)
                    , 1);

                processer.insertOneCol(
                    e.Select(r => r._row.EngName)
                    , 2);

                processer.insertOneCol(
                    e.Select(r => r.TPEngTypeName)
                    , 3);

                processer.insertOneCol(
                    e.Select(r => r.TownCity)
                    , 4);

                processer.insertOneCol(
                    e.Select(r => r._row.ContractExpenses / 1000)
                    , 5);

                processer.insertOneCol(
                    e.Select(r => r._row.PlantExpenses / 1000)
                    , 6);
                processer.insertOneCol(
                    e.Select(r => service.getTreeSchArea(r._row.Seq, year))
                , 7);
                //processer.insertOneCol(
                //    e.Select(r => r._row.ScheduledPlantTotalArea)
                //    , 7);
                processer.insertOneCol(
                    treeNumExport.Select(r => r.ContainsKey("喬木") ? r["喬木"] : "")
                    , 8);
                processer.insertOneCol(
                    treePlantTypeNum.Select(row => row.ContainsKey("喬木") ? row["喬木"] : 0), 9);
                processer.insertOneCol(
                    treeNumExport.Select(r => r.ContainsKey("灌木") ? r["灌木"] : "")
                    , 10);
                processer.insertOneCol(
                    treePlantTypeNum.Select(row => row.ContainsKey("灌木") ? row["灌木"] : 0), 11);

                processer.insertOneCol(
                    e.Select(r => r.BidAwardDateStr)
                    , 12);
                processer.insertOneCol(
                    e.Select(r => r.TreePlantTypeName)
                    , 13);
                processer.insertOneCol(
                    e.Select(r => r.ScheduledPlantDateStr)
                    , 14);
                processer.insertOneCol(
                    e.Select(r => r.ScheduledCompletionDateStr)
                    , 15);
                processer.insertOneCol(
                    e.Select(r => r.ActualCompletionDateStr)
                    , 16);
                processer.insertOneCol(
                    e.Select(r => service.getTreePlantComplateArea(r._row.Seq, year)), 17);
                processer.insertOneCol(
                    treePlantTypeAclNum.Select(types => types.ContainsKey("喬木") ? types["喬木"] : 0), 18);
                processer.insertOneCol(
                    treePlantTypeAclNum.Select(types => types.ContainsKey("灌木") ? types["灌木"] : 0), 19);
                processer.insertOneCol(
                    e.Select(r => r.ContractorNameResult)
                    , 20);
                processer.insertOneCol(
                    e.Select(r => r._row.PlantContractor)
                    , 21);
                //processer.insertOneCol(
                //    NonNormalTreeNumExport
                //    , 21);
                processer.insertOneCol(
                    e.Select(r => r._row.ExecutionStatusDescription)
                    , 23);

                processer.fowardStartRow(groupRowIndex);


                int lastSumRow = sumRowIndex.Count > 0 ? sumRowIndex.Last() : 2;

                string[] sumRow = new string[22];
                sumRow[1] = "合計";

                sumRow[5] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "F");
                sumRow[6] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "G");
                sumRow[7] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "H");
                sumRow[9] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "J");
                sumRow[11] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "L");
                sumRow[17] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "R");
                sumRow[18] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "S");
                sumRow[19] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "T");

                processer.insertRowContent(
                    sumRow,
                    HSSFColor.LightOrange.Index,
                    HSSFColor.Red.Index
                );
                sumRowIndex.Add(lastSumRow + groupRowIndex + 1);



                processer.insertRange(CellRangeAddress.ValueOf("A1:N3"),  processer.getSheet(3), summaryCurrentRowIndex, SummarySheet);
                var summaryCurrentRow = SummarySheet.GetRow(summaryCurrentRowIndex);


                summaryCurrentRow.GetCell(0).SetCellValue(groupIndex);
                summaryCurrentRow.GetCell(1).SetCellValue(pair.Key);

                //總表單一項目(所屬機關群組)
                int ii = 0;
                int col = 0;
                string cellFormula;
                for ( ii = 0; ii < 2; ii++)
                {
                    //start col index
                    col = 3;
                    summaryCurrentRow = SummarySheet.GetRow(summaryCurrentRowIndex + ii);

                    new string[] { "F", "G", "H", "R", "J", "S", "L", "T" }
                    .ToList()
                    .ForEach(colChar =>
                    {
                        cellFormula = summaryCurrentRow.GetCell(col).CellFormula;
                        if (ii == 0)
                        {
                            cellFormula = cellFormula.Replace(",7,", ",\"轄管土地\",");
                        }
                        if (ii == 1)
                        {
                            cellFormula = cellFormula.Replace(",\"<>7\",", ",\"<>轄管土地\",");
                        }

                        cellFormula = cellFormula
                        .Replace($"${colChar}$3:${colChar}$35", $"${colChar}${lastSumRow + 1}:${colChar}${lastSumRow + groupRowIndex}")
                        .Replace("$D$3:$D$35", $"$D${lastSumRow + 1}:$D${lastSumRow + groupRowIndex}")
                        .Replace("$B$3:$B$35", $"$B${lastSumRow + 1}:$B${lastSumRow + groupRowIndex}")
                        .Replace(",$B1)", $",$B{summaryCurrentRowIndex+1})");
                        summaryCurrentRow.GetCell(col).SetCellFormula(cellFormula);
                        col++;

                    });

                    //完成率
                    cellFormula = summaryCurrentRow.GetCell(11).CellFormula;
                    cellFormula = cellFormula.Replace($"G{ii + 1}/F{ii + 1}", $"G{summaryCurrentRowIndex + ii +1}/F{summaryCurrentRowIndex + ii +1}");
                    summaryCurrentRow.GetCell(11).SetCellFormula(cellFormula);

                }
                //total
                summaryCurrentRow = SummarySheet.GetRow(summaryCurrentRowIndex + ii);
                col = 3;
                new string[] { "D", "E", "F", "G", "H", "I", "J", "K" }
                .ToList()
                .ForEach(colChar =>
                {
                    cellFormula = summaryCurrentRow.GetCell(col).CellFormula;
                    cellFormula = cellFormula.Replace($"{colChar}1:{colChar}2", $"{colChar}{summaryCurrentRowIndex +1}:{colChar}{summaryCurrentRowIndex +2}");
                    summaryCurrentRow.GetCell(col).SetCellFormula(cellFormula);
                    col++;

                });
                //total 完成率
                cellFormula = summaryCurrentRow.GetCell(11).CellFormula;
                cellFormula = cellFormula.Replace($"G3/F3", $"G{summaryCurrentRowIndex + ii}/F{summaryCurrentRowIndex + ii}");
                summaryCurrentRow.GetCell(11).SetCellFormula(cellFormula);

                groupIndex++;
                summaryCurrentRowIndex +=  3;
            });


            //總表統計
            processer.insertRange(CellRangeAddress.ValueOf("A1:N3"),  processer.getSheet(4), summaryCurrentRowIndex, SummarySheet);
            var summaryTotalRow = SummarySheet.GetRow(summaryCurrentRowIndex );
            for (int i = 0; i < 3; i++)
            {
                summaryTotalRow = SummarySheet.GetRow(summaryCurrentRowIndex + i);
                int col = 3;
                string cellFormula;
                new string[] { "D", "E", "F", "G", "H", "I", "J", "K" }
                   .ToList()
                   .ForEach(colChar =>
                   {

                       cellFormula = summaryTotalRow.GetCell(col).CellFormula;
                       if (i < 2)
                       {
                           cellFormula = cellFormula
                            .Replace("#REF!,", $"C3:C{summaryCurrentRowIndex },")
                            .Replace("#REF!)", $"{colChar}3:{colChar}{summaryCurrentRowIndex })");
                       }
                       else
                       {
                           cellFormula = cellFormula
                            .Replace($"{colChar}1:{colChar}2", $"{colChar}{summaryCurrentRowIndex + 1}:{colChar}{summaryCurrentRowIndex +2}");
                       }
                       summaryTotalRow.GetCell(col).SetCellFormula(cellFormula);
                       col++;
                   });
                //統計 完成率
                cellFormula = summaryTotalRow.GetCell(11).CellFormula;
                cellFormula = cellFormula.Replace($"G{i + 1}/F{i + 1}", $"G{summaryCurrentRowIndex + i}/F{summaryCurrentRowIndex + i}");
                summaryTotalRow.GetCell(11).SetCellFormula(cellFormula);

            }

            processer.copyOutSideRowStyle(2, 0, 1);
            string[] totalRow = new string[22];
            totalRow[1] = "合計";
            totalRow[5] = processer.getPlusFormula(sumRowIndex, "F");
            totalRow[6] = processer.getPlusFormula(sumRowIndex, "G");
            totalRow[7] = processer.getPlusFormula(sumRowIndex, "H");
            totalRow[9] = processer.getPlusFormula(sumRowIndex, "J");
            totalRow[11] = processer.getPlusFormula(sumRowIndex, "L");
            totalRow[17] = processer.getPlusFormula(sumRowIndex, "R");
            totalRow[18] = processer.getPlusFormula(sumRowIndex, "S");
            totalRow[19] = processer.getPlusFormula(sumRowIndex, "T");
            processer.copyOutSideRowStyle(2, 0, 1);
            processer.insertRowContent(
                totalRow
                ,
                HSSFColor.LightGreen.Index,
                HSSFColor.Red.Index
            );


            processer.removeSheet(4);
            processer.removeSheet(3);
            processer.removeSheet(2);
            processer.removeSheet(2);
            //processer.removeSheet(0);
            processer
            .setSheet(0)
            .setSheetCell(0, 12, $"資料統計至{year}.{DateTime.Now.Month}.{DateTime.Now.Day}")
            .setSheetCell(0, 0, $"附件3-{year}年度植樹總彙整表總表");
            Response.AddHeader("Content-Disposition", $"attachment; filename=植樹彙整表{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx");
            Response.BinaryWrite(processer.getTemplateStream().ToArray());
            
                
        }


        public JsonResult UploadFiles(string seq)
        {


            var files1 = Request.Files.GetMultiple("photo.SchTreePhoto");
            var files2 = Request.Files.GetMultiple("photo.AclTreePhoto");
            files1.UploadFilesToFolder($"TreePlant/{seq}.0");
            files2.UploadFilesToFolder($"TreePlant/{seq}.1");
            return Json(true);
        }

        public JsonResult RenamePhotoDir(string source, string target)
        {
            Directory.GetDirectories("TreePlant".getUploadDir())
                .Select(dir => new DirectoryInfo(dir))
                .Where(dir => dir.CreationTime.AddMinutes(30) < DateTime.Now && dir.Name.StartsWith("temp.") )
                .ToList()
                .ForEach(dir => Directory.Delete(dir.FullName, true));
            FileSystem.RenameDirectory($"TreePlant/{source}".getUploadDir(), target);
            return Json(true);
        }
        public JsonResult GetDownloadList(string seq)
        {
            string host = $"{Request.Url.Scheme}://{Request.Url.Authority}";
            var fileList = $"TreePlant/{seq}"
                .GetDownloadListByFolder()
                .Select(row => new { 
                    fileName = row,
                    fileLink = $"{host}/FileUploads/TreePlant/{seq}/{row}"
                });
            return Json(fileList);
        }

        public ActionResult OneDownload(string seq, string fileName)
        {
            Stream iStream = $"TreePlant/{seq}".GetFileStream(fileName);
            return File(iStream, "application/blob", fileName);
        }

        public ActionResult DownloadAll(string seq)
        {
            string type = seq.Split('.')[1];
            string zipName = "";
            switch( type )
            {
                case "0": zipName = $"{seq}-種植前照片";break;
                case "1": zipName = $"{seq}-種植後照片";break;
            }
            string zipPath = $"TreePlant/{seq}".GetDownloadListByFolder(true)
                .downloadFilesByZip($"TreePlant/{seq}/zip", zipName);

            Stream stream = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, "application/blob", $"{zipName}.zip");
        }

        public void DeleteFile(string seq, string fileName)
        {
            $"TreePlant/{seq}".DeleteFile(fileName);
        }




    }
}
