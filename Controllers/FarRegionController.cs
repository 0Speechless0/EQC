using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Controllers.Common;
using EQC.EDMXModel;
namespace EQC.Controllers
{
    public class FarRegionController : ExcelImportController<FarRegion>
    {

        public ActionResult Index()
        {
            return View();
        }
        // GET: FarRegion
        private static string[,] excelHeaderMap = new string[,]
        {
            { "TownCity", "縣市鄉鎮" },
        };
        public FarRegionController()
            :base(
                 "FarRegion",
                 "TownCity",
                 excelHeaderMap,
                 (context, uniqueKey) => context.FarRegion.Find(uniqueKey) ,
                 (target) => target.TownCity ,
                 (context) => context.FarRegion.ToList(),
                 (target, keyWord) =>  target.TownCity.Contains(keyWord),
                 (process) =>
                 {
                     process
                        .setStartRow(1)
                        .setLogFlag(false);
                 }
                )
        {

        }

    }
}