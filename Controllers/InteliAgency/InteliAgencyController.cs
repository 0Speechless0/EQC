using EQC.EDMXModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    public class InteliAgencyController : MyController
    {
        // GET: InteliAgency
        public  void GetUserMessagesNewToOld(int userSeq, int skip, int count)
        {
            using(var context = new EQC_NEW_Entities())
            {
                var userMessages = context
                    .InteliAgencyMessage
                    .Where(r => r.UserMainSeq == userSeq);
                ResponseJson(new {
                    list = context
                    .InteliAgencyMessage
                    .Where(r => r.UserMainSeq == userSeq)
                    .OrderByDescending(r => r.Seq)
                    .Skip(skip)
                    .Take(count)
                    .ToList(),
                    count = userMessages.Count()
                }, "yyyy/MM/dd HH:mm:ss");
            }
        }

        public void StoreUserMessage(InteliAgencyMessage vm)
        {
        
            using (var context = new EQC_NEW_Entities())
            {
                vm.CreateTIme = DateTime.Now;
                vm.AgreeLevel = 0;  
                context.InteliAgencyMessage.Add(vm);
                context.SaveChanges();
                ResponseJson(vm.Seq);
            }
           
        }

        public void SuggestAgencyMessage(InteliAgencyMessage vm)
        {

            using (var context = new EQC_NEW_Entities())
            {
                vm.CreateTIme = DateTime.Now;
                var target = context.InteliAgencyMessage.Find(vm.Seq);
                target.AgreeLevel = vm.AgreeLevel;
                target.Suggestion = vm.Suggestion;
                context.SaveChanges();
                ResponseJson(vm.Seq);
            }

        }
    }
}