using EQC.Common;
using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    public class EQMCarbonEmissionCalController : Controller
    {
        private DBConn dbcon = new DBConn();

        // GET: EQMCarbonEmssionCal

        private class VM : EQMCalCarbonEmissionsVModel
        {
            public string   Description{ get; set; }
            public string   Memo{ get; set; }
            public string   ResultUnit{ get; set; }
            public decimal?   ResultKgCo2e{ get; set; }
            public bool error { get; set; } = true;
        }
        public ActionResult Index()
        {
            Utils.setUserClass(this, 2);
            return View("Index");
        }

        //private string getMemoForStaticStructor(VM row, string unit) 
        //{
        //    if (row.ResultUnit != unit && unit != "%") return "單位代碼錯誤";
        //    if (row.RefItemCode.Length < 10) return "不足10碼";

        //    else row.error = false;
        //    return row.Memo;
        //}
        //private string getMemoForTempStructor(string code, string compare_unit, string unit)
        //{
        //    if (compare_unit != unit && compare_unit != "%") return "單位代碼錯誤";
        //    else if (code.Length < 10) return "不足10碼";
        //    return "查無編碼";
            
        //}
        private void getItemMemo(VM item)
        {
            
            switch(item.RStatusCode)
            {
                case CarbonEmissionPayItemService._C10NotMatchUnit:
                    item.Memo = "單位代碼錯誤";break;
                case CarbonEmissionPayItemService._NotMatch:
                    item.Memo = "查無編碼"; break;
                case CarbonEmissionPayItemService._C10NotMatch:
                    item.Memo = "查無單位係數"; break;
                default:  item.error = false; break;
            }
        }

        public JsonResult getFactor(string compare_unit, string code = "")
        {
            //int unitCode = code.Length > 0 ? code[code.Length - 1] - 48 : 0;
            code = code.Trim();
            List<string> unitList = Utils.getCarbonUnit();
            //string unit = "";

            //if (unitCode > 0 && unitCode < 10)
            //{
            //    unit = unitList[unitCode - 1];
            //}
            CarbonEmissionPayItemService iservice = new CarbonEmissionPayItemService();
            VM item = iservice.CompareCalCarbonEmissions<VM>(code, compare_unit);
            getItemMemo(item);
            //string sql = @"
            //    Select 
            //        cf.Code,
            //        cf.Item,
            //        cf.Memo,
            //        cf.Unit,
            //        cf.KgCo2e
            //        from CarbonEmissionFactor cf
            //        where 
            //        SUBSTRING(@code,len(@code)-9, 10) Like cf.KeyCode1 
            //        or
            //        (
            //            SUBSTRING(@code,len(@code)-9, 10) not Like cf.KeyCode1 
            //            and
            //            SUBSTRING(@code,len(@code)-9, 10) Like cf.KeyCode3
            //        )
            //        order by Memo 
            //";

            //var cmd = dbcon.getcommand(sql);
            //cmd.parameters.addwithvalue("@code", code);

            //var data = dbcon.getdatatable(cmd).rows.cast<datarow>().select(row => {

            //    var vm = new vm
            //    {
            //        refitemcode = row.field<string>("code"),
            //        description = row.field<string>("item"),
            //        memo = row.field<string>("memo") ,
            //        resultunit = row.field<string>("unit"),
            //        resultkgco2e = unitcode == 0 ? 0 : row.field<decimal?>("kgco2e")

            //    };

            //    vm.memo = getmemoforstaticstructor(vm, compare_unit);
            //    return vm;

            //}).cast<vm>().tolist();

            //if(unitcode == 0)
            //{
            //    data = data
            //        .where(row => !unitlist.contains(row.memo)).tolist();
            //}
            //var result = data.tolist().firstordefault();
            //if(result ==null)
            //{
            //    result = new vm();

            //    result.memo = getmemofortempstructor(code, compare_unit, unit);
                

            //}

            return Json(new
            {
                data = item,
                unitOption = unitList
            }, JsonRequestBehavior.AllowGet);
            
        }
    }
}