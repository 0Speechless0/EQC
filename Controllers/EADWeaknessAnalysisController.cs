using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EADWeaknessAnalysisController : Controller
    {//品質管制弱面分析 s20230316
        EADWeaknessAnalysisService iService = new EADWeaknessAnalysisService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }
        //廠商弱面統計
        public virtual JsonResult GetContractorWeakness(int mode, string unit)
        {
            return Json(new
            {
                result = 0,
                keywordOptions = iService.GetContractorList(mode, unit)
            });
        }
        //構面分析
        public virtual JsonResult GetWeaknessOriented(int mode, string unit)
        {
            List<WeaknessOrientedModel> orienteds = iService.GetWeaknessOriented();
            List<EADWeaknessOrientedVModel> items = iService.GetOrientedSta<EADWeaknessOrientedVModel>(mode, unit);
            List<ChartPieVModel> weaknessOriented = new List<ChartPieVModel>();
            List<ChartPieVModel> engOriented = new List<ChartPieVModel>();
            List<ChartPieVModel> companyOriented = new List<ChartPieVModel>();
            if (items.Count > 0)
            {
                EADWeaknessOrientedVModel m = items[0];
                weaknessOriented.Add(new ChartPieVModel() { name = "有弱面", y = m.WeaknessTotal });
                weaknessOriented.Add(new ChartPieVModel() { name = "無弱面", y = (m.EngCount - m.WeaknessTotal) });

                foreach (WeaknessOrientedModel o in orienteds)
                {
                    int total = -1;
                    switch(o.Id)
                    {
                        case 1: total = m.W1; break;
                        case 2: total = m.W2; break;
                        case 3: total = m.W3; break;
                        case 4: total = m.W4; break;
                        case 5: total = m.W5; break;
                        case 6: total = m.W6; break;
                        case 7: total = m.W7; break;
                        case 8: total = m.W8; break;
                        case 9: total = m.W9; break;
                        case 10: total = m.W10; break;
                        case 11: total = m.W11; break;
                        case 12: total = m.W12; break;
                        case 13: total = m.W13; break;
                        case 14: total = m.W14; break;
                    }
                    if(total > -1)
                    {
                        if(o.ItemType == 0)//標案
                        {
                            engOriented.Add(new ChartPieVModel() { name = o.ItemName, y = total });
                        }
                        else if (o.ItemType == 1)//廠商
                        {
                            companyOriented.Add(new ChartPieVModel() { name = o.ItemName, y = total });
                        }
                    }
                }
            }
            return Json(new
            {
                result = 0,
                weaknessOriented = weaknessOriented,
                engOriented = engOriented,
                companyOriented = companyOriented
            });
        }
        //十四項指標
        public virtual JsonResult GetWeakness(int mode, string unit)
        {
            List<WeaknessOrientedModel> orienteds = iService.GetWeaknessOriented();
            Dictionary<int, string> dicWeakness = new Dictionary<int, string>();
            foreach (WeaknessOrientedModel o in orienteds)
            {
                dicWeakness.Add(o.Id, o.ItemName);
            }
            List<EADWeaknessVModel> items = iService.GetWeaknessSta<EADWeaknessVModel>(mode, unit);
            List<ChartPieVModel> datas = new List<ChartPieVModel>();
            foreach (EADWeaknessVModel m in items)
            {
                if(m.W1 > 0) datas.Add(new ChartPieVModel() { name= dicWeakness[1], y=m.W1 });
                if (m.W2 > 0) datas.Add(new ChartPieVModel() { name = dicWeakness[2], y = m.W2 });
                if (m.W3 > 0) datas.Add(new ChartPieVModel() { name = dicWeakness[3], y = m.W3 });
                if (m.W4 > 0) datas.Add(new ChartPieVModel() { name = dicWeakness[4], y = m.W4 });
                if (m.W5 > 0) datas.Add(new ChartPieVModel() { name = dicWeakness[5], y = m.W5 });
                if (m.W6 > 0) datas.Add(new ChartPieVModel() { name = dicWeakness[6], y = m.W6 });
                if (m.W7 > 0) datas.Add(new ChartPieVModel() { name = dicWeakness[7], y = m.W7 });
                if (m.W8 > 0) datas.Add(new ChartPieVModel() { name = dicWeakness[8], y = m.W8 });
                if (m.W9 > 0) datas.Add(new ChartPieVModel() { name = dicWeakness[9], y = m.W9 });
                if (m.W10 > 0) datas.Add(new ChartPieVModel() { name = dicWeakness[10], y = m.W10 });
                if (m.W11 > 0) datas.Add(new ChartPieVModel() { name = dicWeakness[11], y = m.W11 });
                if (m.W12 > 0) datas.Add(new ChartPieVModel() { name = dicWeakness[12], y = m.W12 });
                if (m.W13 > 0) datas.Add(new ChartPieVModel() { name = dicWeakness[13], y = m.W13 });
                if (m.W14 > 0) datas.Add(new ChartPieVModel() { name = dicWeakness[14], y = m.W14 });
                break;
            }
            return Json(new
            {
                result = 0,
                data = datas
            });
        }
        //工程清單
        public virtual JsonResult GetUnitEngAmount(int mode)
        {
            List<EADUnitEngAmountVModel> unitEngAmount = iService.GetTenderList<EADUnitEngAmountVModel>(mode);
            List<string> units = new List<string>();
            List<int> amount = new List<int>();
            foreach(EADUnitEngAmountVModel m in unitEngAmount)
            {
                units.Add(m.UnitName);
                amount.Add(m.Amount);
            }
            return Json(new
            {
                result = 0,
                categories = units,
                data = amount
            });
        }
    }
}