using DocumentFormat.OpenXml.Office2010.Excel;
using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using EQC_CarbonEmissionCal.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EQC_CarbonEmissionCal
{
    public class CarbonEmissionCalXMLController : Controller
    {
        public CarbonEmissionCalXMLService iService;
        public EngMainService engService;
        public PriceAdjustmentService priceService;
        private PCCESSMainService pmS;
        private TenderPlanService tenderPlanService;

        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View();
        }
        public CarbonEmissionCalXMLController()
        {
            iService = new CarbonEmissionCalXMLService();
            engService = new EngMainService();
            priceService = new PriceAdjustmentService();
            pmS = new PCCESSMainService();
            tenderPlanService = new TenderPlanService();
        }

        public JsonResult GetCarbonCalInfo(int id, int perPage = 1, int pageIndex = 1, string keyword = "")
        {
            int total = iService.GetListTotal(id, keyword);
            List<CarbonEmissionPayItemVModel> ceList = iService.GetList<CarbonEmissionPayItemVModel>(id, perPage, pageIndex, keyword);

                //iService.CreatePayItems(id);
                iService.CalCarbonEmissions(id);
                total = iService.GetListTotal(id, "");
                if (total == 0)
                {
                    return Json(new CarbonCalInfo()
                    {
                        success = false,
                        message = "無初始資料, 請重新匯入",
                        totalRows = total
                    });
                }
                ceList = iService.GetList<CarbonEmissionPayItemVModel>(id, perPage, pageIndex, "");

            decimal? co2Total = null;
            decimal? co2ItemTotal = null;
            if (total > 0)
            {
                iService.CalCarbonTotal(id, ref co2Total, ref co2ItemTotal);
            }
            return Json(new CarbonCalInfo()
            {
                totalRows = total,
                items = ceList,
                co2Total = co2Total ?? 0,
                co2ItemTotal = co2ItemTotal ?? 0
            });
        }
        public JsonResult GetEngMain(int id)
        {
            List<EngMainEditVModel> items = iService.GetItemBySeq<EngMainEditVModel>(id);
            if (items.Count == 1)
            {
                return Json(items[0]);

            }
            return Json(null);
        }
        public JsonResult UploadXML(HttpPostedFileBase file)
        {
            string errMsg = "";
            int engMainSeq = -1;
            if (file.ContentLength > 0)
            {
                try
                {
                    string tempPath = Path.GetTempPath();
                    string fullPath = Path.Combine(tempPath, file.FileName);
                    
                    engMainSeq =  processXML(file.InputStream, file.FileName,  ref errMsg);
                    string engFolder = Utils.GetEngMainFolder(engMainSeq);




                }
                catch (Exception ex)
                {
                    return Json(-2);
                    //System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);

                }
            }
            return Json(new { 
                status = "success",
                engId = engMainSeq
            });

        }

        //public ActionResult DownloadXML(int engId)
        //{
        //    var carbonHeader = iService.GetHeaderList<CarbonEmissionHeaderModel>(engId).FirstOrDefault();

        //    var engFolder = Utils.GetEngMainFolder(carbonHeader.EngMainSeq);
        //    var outputPoccessor = new PccesXMLUpdatedOuput(Path.Combine(engFolder, carbonHeader.PccesXMLFile));
        //    var payItems = iService.GetAllPayItem(carbonHeader.Seq);
        //    using(var ms = new MemoryStream())
        //    {
        //        outputPoccessor.exportUpdatedPayItemPCCESFile(carbonHeader.Seq, payItems).CopyTo(ms);

        //        Response.AddHeader("Content-Disposition", $"attachment; filename={file.Name}");
        //        Response.BinaryWrite(ms.ToArray());
        //    }


        //}

        private int processXML(Stream inputStream,  string fileName, ref string errMsg)
        {
            //System.Diagnostics.Debug.WriteLine(fileName);
            //
            iService.ReFresh();
            PccseXML pccseXML = new PccseXML(inputStream);
            EngMainModel engMainModel = pccseXML.CreateEngMainModel(ref errMsg);
            if (engMainModel == null) return -1;
            pccseXML.GetGrandTotalInformation();
            PCCESSMainModel pccessMainModel = pccseXML.pccessMainModel;
            List<PCCESPayItemModel> payItems = pccseXML.payItems;
            //return -1;
            string xmlFileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
            engMainModel.PccesXMLFile = xmlFileName;
            engMainModel.PostCompDate = DateTime.Now;


            int engMainSeq = iService.AddEngItem(pccessMainModel, engMainModel, ref errMsg);
            iService.CreatePayItems(engMainSeq, pccseXML, xmlFileName);
            return engMainSeq;

            return 0;
        }
        

        public JsonResult updatePayItemMemo(CarbonEmissionPayItemVModel m)
        {
            try
            {
                iService.updatePayItem(m);
            }
            catch(Exception e)
            {
                return Json(null);
            }
            return Json(true);
        }


    }
}
