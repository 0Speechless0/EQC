using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Controllers;
using EQC.EDMXModel;

namespace EQC.API
{
    public class SuggestionController : MyController
    {
        public void GetSuggestion(int function, int functionSeq)
        {
            using(var context = new EQC_NEW_Entities())
            {
                var list = context
                    .SuggestionHead.Where(r =>
                        r.ParentSeq == functionSeq &&
                        r.ParentClass == function
                    ).Join(context.Suggestion, r1 => r1.SuggestionSeq, r2 => r2.Seq, (r1, r2) => r2)
                    .Select(r => new { 
                        Text = r.Text,
                        ClassName = r.SuggestionClass.Name,
                        OrderNo = r.OrderNo
                    })
                    .OrderBy(r => r.OrderNo)
                    .GroupBy(r => r.ClassName)
                    .ToDictionary(r => r.Key, r => r);

                ResponseJson(list);
            }
        }
    }
}  