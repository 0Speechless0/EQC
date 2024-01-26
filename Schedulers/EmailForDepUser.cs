using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Timers;
using EQC.Common;
using EQC.EDMXModel;
using EQC.Services;
using EQC.ViewModel;

namespace EQC.Scheduler
{
    public class EmailForDepUser
    {
        private string execTime;
        private DayOfWeek execDay;

        private int? unit;
        public EmailForDepUser(string taskName = "")
        {
            int minSec = Int32.Parse(ConfigurationManager.AppSettings["emailSendFrequency"].ToString() )*1000;

            Timer aTimer = new Timer(minSec);
            aTimer.Elapsed += action ;
            aTimer.AutoReset = true;
            execTime = ConfigurationManager.AppSettings["emailTimerRunTime"].ToString();
            execDay = (DayOfWeek)Int32.Parse(ConfigurationManager.AppSettings["emailTimerRunDay"].ToString());
            unit = Int32.Parse(ConfigurationManager.AppSettings["emailTimerRunForUnit"]?.ToString() );
            if (ConfigurationManager.AppSettings["emailTimerRun"].ToString() == "Y") aTimer.Enabled = true;
        }
        private void action(Object source, ElapsedEventArgs e)
        {
            List<EQC.Models.UserMain> depUser = new List<EQC.Models.UserMain>();

            if (DateTime.Now.DayOfWeek != execDay || DateTime.Now.ToString("HH:mm:ss") != execTime) return;

            using (EQC_NEW_Entities context = new EQC_NEW_Entities())
            {

                List<UserUnitPosition> mapList = (
                    from row in context.Role
                    
                    where row.Id == "03"  select row.UserUnitPosition
                ).First().Where(r => r.UnitSeq == unit|| unit == null).ToList();

                List<EQC.Models.UserMain> orgUserMain = context.UserMain.Select(
                    row => new EQC.Models.UserMain
                    {
                        EmailAddress = row.Email,
                        DisplayName = row.DisplayName,
                        Seq = row.Seq
                    }
                ).ToList();

                depUser = orgUserMain.Where(row =>
                    mapList.Select(unitRole => unitRole.UserMainSeq)
                    .ToList()
                    .Contains(row.Seq)
                ).ToList();



            }
            PortalDepUserService depUserSerivce = new PortalDepUserService();

            foreach (EQC.Models.UserMain user in depUser)
            {
                string execUnitName = Utils.GetUserUnitName(user.Seq);
                List<EADEngStaVModel> depUserImportantList = depUserSerivce.GetImportantEventSta<EADEngStaVModel>(execUnitName);
                string emailBody = String.Format("@" +
                    "{0}，您好! <br>" +
                    "我是水利署服務的自動寄發郵件程式，需提醒您以下事項 : <br>", user.DisplayName);
                foreach(EADEngStaVModel importItem in depUserImportantList)
                {
                    emailBody += importItem.level + importItem.engCount + "件。<br>";
                }

                emailBody += "以上，我會再寄來，祝您工作愉快";


                Utils.Email(user.EmailAddress, "各局管理者事件清單通知 ^_^", emailBody);

            }
        }



    }
}