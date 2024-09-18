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

        private string unit;
        private string test;
        private string testEmail;
        private Timer aTimer;
        private string loginHost;
        APIService apiService ;
        private string logFilePath;
        public EmailForDepUser(string taskName = "")
        {
            int minSec = 1000;

            aTimer = new Timer(minSec);
            aTimer.Elapsed += action ;
            //logFilePath = $"LogFiles\\{DateTime.Now.ToString("yyyy-MM-dd")}自動寄發log.txt";
            //logFilePath.writeLogToFile("自動寄發程式啟動");
            log4net.LogManager.GetLogger("Service").Info("自動發送email啟動");
            testEmail = ConfigurationManager.AppSettings["emailTimerTestEmail"]?.ToString();
            execTime = ConfigurationManager.AppSettings["emailTimerRunTime"]?.ToString();
            execDay = (DayOfWeek)Int32.Parse(ConfigurationManager.AppSettings["emailTimerRunDay"].ToString());
            unit = ConfigurationManager.AppSettings["emailTimerRunForUnit"]?.ToString();
            loginHost = ConfigurationManager.AppSettings["loginHost"]?.ToString();
            apiService = new APIService();
            aTimer.AutoReset = true;
            if (ConfigurationManager.AppSettings["emailTimerRun"].ToString() == "Y") aTimer.Enabled = true;
        }

        protected void SendEmail(EQC.Models.UserMain user, string emailBody, string token)
        {
            log4net.LogManager.GetLogger("Service").Info("Email發送" + user.EmailAddress);
            Utils.Email(user.EmailAddress, "各局管理者事件清單通知 ^_^", emailBody, null, "");
          
        }
        private void action(Object source, ElapsedEventArgs e)
        {
            List<EQC.Models.UserMain> users = new List<EQC.Models.UserMain>();

            if (DateTime.Now.DayOfWeek != execDay || DateTime.Now.ToString("HH:mm:ss") != execTime) return;


            using (EQC_NEW_Entities context = new EQC_NEW_Entities())
            {
                users =
                    context.Role.Where(r => r.Seq <= 3)
                    .Select(r => r.UserUnitPosition)
                .ToList()

                .Aggregate(new List<UserMain>(), (last, userUnitPos) => {
                    last.AddRange(userUnitPos
                        .Where(r => (r.UnitSeq.ToString() == unit || r.Unit.ParentSeq.ToString() == unit || unit == null )
                        )
                        .Select(r => r.UserMain).ToList());
                    return last;
                })
                .Select(
                    row => new EQC.Models.UserMain
                    {
                        EmailAddress = row.Email,
                        DisplayName = row.DisplayName,
                        UserNo = row.UserNo,
                        PassWord = row.PassWord,
                        Seq = row.Seq
                    }
                ).ToList();


              }
            PortalDepUserService depUserSerivce = new PortalDepUserService();
            users = users.Where(r => (r.EmailAddress == testEmail || testEmail == null)).ToList();
           
            foreach (EQC.Models.UserMain user in users)
            {
                
                string execUnitName = Utils.GetUserUnitName(user.Seq);
                List<EADEngStaVModel> depUserImportantList = depUserSerivce.GetImportantEventSta<EADEngStaVModel>(execUnitName);
                string emailBody = String.Format("@" +
                    "{0}，您好! <br>" +
                    "我是水利署服務的自動寄發郵件程式，需提醒您以下事項 : <br>", user.DisplayName);
                var token = apiService.addToken(user.UserNo, user.PassWord);
                var _loginHost = loginHost + "?token=" + token.Value;
                foreach (EADEngStaVModel importItem in depUserImportantList)
                {
                    string color = importItem.engCount > 0 ? "blue" : "black";
                    emailBody += importItem.level + $"<a href='{_loginHost}'><span style='color:{color}'> "+importItem.engCount + "</span> </a> 件。<br>";
                }

                emailBody += "<br><br>祝您工作愉快";


               
                SendEmail(user, emailBody, token.Value);

                if (test == "Y") break;

            }
        }



    }
}