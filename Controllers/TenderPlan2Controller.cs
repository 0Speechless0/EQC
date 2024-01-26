using EQC.EDMXModel;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    public class TenderPlan2Controller : MyController
    {
        // GET: TenderPlan2

        public void UpdateDirectorUserName(string directorName,int engSeq, string keyWord)
        {
            
            using(var context = new EQC_NEW_Entities())
            {
                
                var originEng = context.EngMain.Find(engSeq);
                var orgType = originEng.GetType();
                var userNoDic = context
                    .UserMain
                    .GroupBy(r => r.UserNo)
                    .ToList()
                    .Select(r => r.First() )
                    .ToDictionary(r => r.UserNo, r => r);
                userNoDic.TryGetValue(keyWord, out EDMXModel.UserMain director);
                if (director != null)
                {
                    director.DisplayName = (string)directorName;
                }
                context.SaveChanges();
            }

            ResponseJson(true);
        }
    }
}