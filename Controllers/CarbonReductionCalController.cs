using EQC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EQC.EDMXModel;
using System.Web.Mvc;
using EQC.Common;

namespace EQC.Controllers
{
    public class CarbonReductionCalController : MyController
    {

        CarbonReductionCalService carbonReductionCalService;
        public CarbonReductionCalController()
        {
            carbonReductionCalService = new CarbonReductionCalService();
        }

        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View();
        }
        // 1 : 挖土機
        // 2 : 卡車
        // 3 : 能源
        public void GetList(int engSeq, int type =1)
        {
            decimal result = 0;
            if (type == 1)
            {
                var list = carbonReductionCalService
                    .GetItem(db => db.CarbonReductionNavvyFactor, engSeq, 1);
                
                ResponseJson(list);
            }
            if (type == 2)
            {
                var list = carbonReductionCalService.GetItem(db => db.CarbonReductionTruckFactor, engSeq, 2);
                ResponseJson(list);
            }
            if (type == 3)
            {
                var list = carbonReductionCalService.GetCarbonReductionFactor(engSeq);
                list.ForEach(e => e.Seq = -e.Seq);
                ResponseJson(list);
            }

        }
        public void GetCalResult(int engSeq)
        {
            var result = carbonReductionCalService.GetCalResult(engSeq);
            ResponseJson(result);
        }
        public void saveCalResult(int engSeq, decimal result, List<CarbonReductionCal> items)
        {
            carbonReductionCalService.saveCalResult(engSeq, result);
            carbonReductionCalService.saveCalItems(engSeq, items);
            ResponseJson(true);
        }
        public void getReductionCalTag(int engSeq)
        {
            using (var context = new EQC_NEW_Entities())
            {
                var result = context.EngMain.Find(engSeq)?.PrjXML?.PrjXMLTag?.CarbonReductionCalTag;
                ResponseJson(result ?? true);
            }
        }

        public void setReductionCalTag(int engSeq, bool tag)
        {
            using (var context = new EQC_NEW_Entities())
            {
                var prjXML = context.EngMain.Find(engSeq)?.PrjXML;
                if (prjXML.PrjXMLTag != null)
                {
                    prjXML.PrjXMLTag.CarbonReductionCalTag = tag;
                }
                else if( prjXML != null)
                {
                    prjXML.PrjXMLTag = new PrjXMLTag
                    {
                        CarbonReductionCalTag = tag
                    };
                }
                context.SaveChanges();
                ResponseJson(true);
            }
        }
        public void GetTypeList(int engSeq)
        {
            ResponseJson(new
            {
                navvy = carbonReductionCalService.GetFactorTypeDic(db => db.CarbonReductionNavvyFactor, engSeq),
                truck = carbonReductionCalService.GetFactorTypeDic(db => db.CarbonReductionTruckFactor, engSeq),
                energy = carbonReductionCalService.GetFactorTypeDic(db => db.CarbonReductionFactor, engSeq)

            });
        }
    }
}