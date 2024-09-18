using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Common;
using EQC.Services;
using EQC.ViewModel;
using EQC.Models;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using System.Text;
using EQC.EDMXModel;
using System.Threading;
using EQC.Detection;

namespace EQC.Controllers
{
    public class LoginController : Controller
    {

        private UserService userService = new UserService();
        private ConfigManager configManager = new ConfigManager();
        private SessionManager sm = new SessionManager();
        private APIService apiService = new APIService();


        public static string userHomeUrlSwitch(UserInfo userInfo)
        {
            string homeUrl = "";
            List<Models.Role> roles = userInfo.Role;
            if (roles.Count == 1)
            {
                int roleSeq = roles[0].Seq;
                if (roleSeq == 1) //系統管理者
                { 
                    homeUrl = "/Users";
                }
                else if (roleSeq == ConfigManager.DepartmentAdmin_RoleSeq) //署管理者
                {
                    userInfo.IsDepartmentAdmin = true;
                    homeUrl = "/PortalDepAdmin/Index";
                }
                else if (roleSeq == ConfigManager.DepartmentUser_RoleSeq) //署使用者
                {
                    userInfo.IsDepartmentUser = true;
                    homeUrl = "/PortalDepUser/Index";
                }
                else if (roleSeq == ConfigManager.BuildContractor_RoleSeq) //施工廠商
                {
                    userInfo.IsBuildContractor = true;
                    homeUrl = "/EPCTender/Index3";
                }
                else if (roleSeq == ConfigManager.SupervisorUnit_RoleSeq) //監造單位
                {
                    userInfo.IsSupervisorUnit = true;
                    homeUrl = "/EPCTender/Index3";
                }
                else if (roleSeq == ConfigManager.OutsourceDesign_RoleSeq) //委外設計
                {
                    userInfo.IsOutsourceDesign = true;
                    homeUrl = "/TenderPlan/TenderPlanList";
                }
                else if (roleSeq == ConfigManager.Committee_RoleSeq) //委員
                {
                    userInfo.IsCommittee = true;
                    homeUrl = "/EngSupervise";
                }
                else if (roles[0].Id == "20") //署內執行者
                {
                    homeUrl = "/TenderPlan/TenderPlanList";
                }
            }

            var targetUrl = userInfo.MenuList
                .ToList().FirstOrDefault(r => ( "/" + r.Url ).StartsWith(homeUrl));

            if (targetUrl != null)
                new SessionManager().currentSystemSeq = targetUrl.SystemTypeSeq?.ToString();
            else if (!homeUrl.StartsWith("/Portal") )
            {
                homeUrl = "/" + userInfo.MenuList.FirstOrDefault()?.Url;
            }
            return homeUrl;
        }

        public ActionResult NoPermission()
        {
            return View();
        }

        /// <summary> 登入頁 </summary>
        /// <returns> View </returns>
        public ActionResult Index()
        {

            Session.Abandon();
            if (ConfigurationManager.AppSettings.Get("TestUserNo")?.ToString() == null)
                return View();


            var path = UploadFilesProcesser.ReadTxTFromFile("App_Data", "debug_url.txt")
                .Split('/');
            var action = path.Length > 1 ? path[1] : "Index";
            var controllerName = path[0] != "" ? path[0] : "Users";
            var userNo = ConfigurationManager.AppSettings.Get("TestUserNo")?.ToString();
            string selectedEngSeq = null;
            string systemType = null;
            if (path.Length > 2)
            {
                var envirnVar = path[2].Split(':');

                if (envirnVar.Length > 0)
                {
                    systemType = envirnVar[0];
                }
                if(envirnVar.Length > 1)
                {
                    selectedEngSeq = envirnVar[1];
                }
            }
            SetTestInfo(userNo, systemType, selectedEngSeq);

            return RedirectToAction(action, controllerName);
        }

        /// <summary> 系統登出 </summary>
        /// <returns> 系統登出 </returns>
        public ActionResult LoginOut()
        {
            //Session.RemoveAll();
            System.Web.Security.FormsAuthentication.SignOut();
            //系統登入頁
            return RedirectToAction("Index", "Login");
        }

  
        public JsonResult APILogin(string userNo, string phone, string clientBindingCode = null)
        {
            try
            {
                int count = userService.CheckUser4(userNo, phone);
                   



                if (count > 0)
                {
                    APIService.Token token = null;
                    if (clientBindingCode != null)
                    {
                         token = apiService.addTokenWithBindingCode<ConstCheckAppLock>(
                                userNo,
                                phone,
                                clientBindingCode,
                                context => context.ConstCheckAppLock
                        );
                    }
                    else
                    {
                        token = apiService.addToken(userNo, phone);
                    }
                    string signatureUrl = ConfigurationManager.AppSettings["signatureRegisterUrl"].ToString();
                    using (HttpClient client = new HttpClient())
                    {
                        StringContent jsonContent = new StringContent(
                            JsonConvert.SerializeObject(token),
                            Encoding.UTF8,
                            "application/json");
                        var jsonResponse = client.PostAsync(signatureUrl, jsonContent).Result.StatusCode;
                    }

                    object userInfo = userService.GetMobileUserInfo(userNo);
                    return Json(new { token = token.Value, status = "success", status_code = 0, userInfo = userInfo, isSDirector = Utils.CheckDirectorOfSupervision(userNo) });
                }
                return Json(new { status = "登入驗證失敗", status_code = 1 });
            }
            catch (Exception e)
            {
                if(e.Message.StartsWith("routine"))
                {
                    return Json(new { status = $"{e.Message.Split(':')[1] }", status_code = 2 });
                }
                else
                {
                    return Json(new { status = $"failed: {e.Message}", status_code = 1 });
                }

            }



        }

        public JsonResult Reject(string message)
          {
            return Json(new
            {
                message = message,
                status = 1
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult APILogOut()
        {

            string token = HttpContext.Request.Headers["token"];

            int status_code = apiService.removeToken(token);
            string message = "";
            switch(status_code)
            {
                case 0: message = "無效token !";break;
                case 1: message = "登出 !"; break;
            }

            return Json(new
            {
                status_code = status_code,
                message = message,
                status = "success"
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary> 開發用階段 檢查使用者 登入帳號、密碼 </summary>
        /// <param name="account"> 帳號 </param>
        /// <param name="passWd"> 密碼 </param>
        /// <returns> 登入結果 </returns>
        public JsonResult CheckUserForDebug(string userNo, string passWd, string token=null)
        {
            try
            {
                APIService.Token _token = null;
                    
                if(token != null)
                    _token = apiService.GetTokenData(token);
                if (_token != null)
                {
                    userNo = _token.userNo;
                    passWd = _token.passWd;
                    apiService.removeToken(token);
                }

                int count;
                string homeUrl = string.Empty;
                count = userService.CheckUser(userNo, passWd);
                if (count > 0)
                {
                    SetSessionManager(userNo);
                    if (ConfigurationManager.AppSettings.Get("Debug") != null) return Json(new SessionManager().GetUser() );
                    SharedController sc = new SharedController();
                    MenuController mc = new MenuController();
                    homeUrl = mc.GetTopMenuUrl();
                    //shioulo 20220511
                    System.Web.SessionState.HttpSessionState _session = System.Web.HttpContext.Current.Session;
                    _session["SystemType"] = 10;
                    if (_session["UserInfo"] != null)
                    {
                        UserInfo userInfo = (UserInfo)_session["UserInfo"];
                        homeUrl = userHomeUrlSwitch(userInfo);
                        using (var context = new EQC_NEW_Entities())
                        {
                            context.UserMain.Find(userInfo.Seq).LastLoginTime = DateTime.Now;
                            context.UserLoginRecord.Add(new
                            UserLoginRecord
                            {
                                UserMainSeq = userInfo.Seq,
                                CreateTime = DateTime.Now,
                                OriginIP = Request.UserHostAddress
                            });
                            context.SaveChanges();
                        }
                    }

                    Response.Cookies.Set(new HttpCookie("DateCookieExample"));
                   _session["UserhomeUrl"] = homeUrl;

 
                        return Json(new
                    {
                        result = 1,
                        isLogin = true,
                        errorMessage = string.Empty,
                        homeUrl = homeUrl,
                        sessionId = _session.SessionID
                    });
                }
                return Json(new
                {
                    result  = -1,
                    msg = "帳號密碼錯誤, 登入失敗!! ",
                    isLogin = false,
                    errorMessage = string.Empty,
                    homeUrl = homeUrl
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    result = -1,
                    isLogin = false,
                    errorMessage = "系統發生錯誤 登入失敗!! ",
                    homeUrl = string.Empty
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CheckUserLevel1(string userNo = null, string passWd = null, string token = null)
        {//shioulo 20220706


            if (ConfigManager.LoingEmailCodeDisabled || token != null) return CheckUserForDebug(userNo, passWd, token);//shioulo 20220710

            try
            {
                int count; 
                string homeUrl = string.Empty;
                List<VUserMain> users = userService.GetUser(userNo, passWd);
                if (users.Count == 1)
                {
                    System.Web.SessionState.HttpSessionState _session = System.Web.HttpContext.Current.Session;
                    VUserMain user = users[0];
                    if (String.IsNullOrEmpty(user.Email))
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "沒有提供電子信箱, 請聯絡系統管理員",
                        }, JsonRequestBehavior.AllowGet);
                    }
                    string[] mail = user.Email.Split('@');
                    if (mail.Length != 2)
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "電子信箱錯誤, 請聯絡系統管理員",
                        }, JsonRequestBehavior.AllowGet);
                    }
                    _session["Level2_Account"] = user.UserNo;
                    _session["Level2_PassWd"] = passWd;
                    _session["Level2_EMail"] = user.Email;
                    string mailCode = Math.Abs(Guid.NewGuid().GetHashCode()).ToString().Substring(0, 6);
                    _session["Level2_MailCode"] = mailCode;
                    //System.Diagnostics.Debug.WriteLine("mailCode:"+ mailCode);

                    string email = String.Format("驗證碼已發送至:{0}..@{1}....", mail[0].Substring(0, 2), mail[1].Substring(0, 2));
                    if (Utils.Email(user.Email, "電子郵件驗證碼:" + mailCode, "請回系統輸入 驗證碼:" + mailCode))
                    {
                        return Json(new
                        {
                            result = 0,
                            msg = email,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new
                    {
                        result = 0,
                        msg = email,
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new
                {
                    result = -1,
                    msg = "郵件發送失敗, 請聯絡系統管理員!!",
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    result = -1,
                    msg = "系統發生錯誤 登入失敗!! ",
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CheckUserLevel2(string userNo, string passWd, string mailCode)
        {
            //shioulo 20220706
            System.Web.SessionState.HttpSessionState _session = System.Web.HttpContext.Current.Session;
            string mCode = _session["Level2_MailCode"].ToString();
            if (String.IsNullOrEmpty(mCode))
            {
                return Json(new
                {
                    result = -2,
                    msg = "請重新登入"
                });
            }
            if (mCode != mailCode)
            {
                return Json(new
                {
                    result = -3,
                    msg = "驗證碼錯誤"
                });
            }
            try
            {
                int count;
                string homeUrl = string.Empty;
                count = userService.CheckUser(userNo, passWd);
                if (count > 0)
                {
                    SetSessionManager(userNo);
                    SharedController sc = new SharedController();
                    MenuController mc = new MenuController();
                    homeUrl = mc.GetTopMenuUrl();
                    if (_session["UserInfo"] != null)
                    {
                        UserInfo userInfo = (UserInfo)_session["UserInfo"];
                        homeUrl = userHomeUrlSwitch(userInfo);
                        using (var context = new EQC_NEW_Entities())
                        {
                            context.UserLoginRecord.Add(new
                            UserLoginRecord
                            {
                                UserMainSeq = userInfo.Seq,
                                CreateTime = DateTime.Now,
                                OriginIP = Request.UserHostAddress
                            });
                            context.SaveChanges();
                        }
                    }
                    _session["UserhomeUrl"] = homeUrl;
                    return Json(new
                    {
                        result = 0,
                        homeUrl = homeUrl
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new
                {
                    result = -2,
                    msg = "登入失敗",
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    result = -1,
                    msg = "系統發生錯誤 登入失敗!! ",
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public static void SetSessionManager(string userNo)
        {
            System.Web.SessionState.HttpSessionState _session = System.Web.HttpContext.Current.Session;
            HttpContext httpContext = System.Web.HttpContext.Current;
            var sm = new SessionManager();
            _session["Account"] = userNo;
            if (httpContext.Session != null)
            {
                if (_session["Account"] != null)
                {
                    sm.CheckSession(_session);
                }
                else
                {
                    sm.LogOut(null);
                }
            }
        }

        public static int? systemType;
        public static string selectedEngSeq;
        public static void SetTestInfo(string userNo, string _systemType = null, string _selectedEngSeq = null)
        {
            Int32.TryParse(_systemType, out int s);
            systemType = s;
            selectedEngSeq = _selectedEngSeq;
            System.Web.SessionState.HttpSessionState _session = System.Web.HttpContext.Current.Session;
            HttpContext httpContext = System.Web.HttpContext.Current;
            var sm = new SessionManager();
            _session["Account"] = userNo;
            if (httpContext.Session != null)
            {
                if (_session["Account"] != null)
                {
                    sm.CheckSession(_session);
                }
                else
                {
                    sm.LogOut(null);
                }
            }
        }

        protected ConstCheckRecService constCheckRecService = new ConstCheckRecService();
        [Route("Login/GetPost")]
        [HttpPost]
        //接收android端傳入post值，判斷登入

        public ActionResult GetPost(string UserNo, string Mobile, string EngName, string EngItemName, string XYPosLati, string XYPosLong, string PosDesc, string CheckDate, string CheckItem, string RealCheckCond, string CheckResult, string CheckImage1, string CheckImage2, string CheckImage3, string CheckUser)
        {
            //Log接收到Post資料
            //BaseService.log.Info("userno：" + UserNo);
            //BaseService.log.Info("mobile：" + Mobile);
            //BaseService.log.Info("engname：" + EngName);
            //BaseService.log.Info("engitemname：" + EngItemName);
            //BaseService.log.Info("xyposlati：" + XYPosLati);
            //BaseService.log.Info("xyposlong：" + XYPosLong);
            //BaseService.log.Info("posdesc：" + PosDesc);
            //BaseService.log.Info("checkdate：" + CheckDate);
            //BaseService.log.Info("checkitem：" + CheckItem);
            //BaseService.log.Info("realcheckcond：" + RealCheckCond);
            //BaseService.log.Info("checkresult：" + CheckResult);
            //BaseService.log.Info("checkuser：" + CheckUser);
            try
            {
                int count;
                string homeUrl = string.Empty;
                count = userService.CheckUser2(UserNo, Mobile);

                
                if (count > 0)
                {
                    List<EngConstructionListVModel> AppList = new List<EngConstructionListVModel>();
                    string[] GetEnItenSeq = EngName.Split(',', '，');
                    string[] GetEnItenSeq1 = EngItemName.Split(',', '，');
                    string EngName1 = GetEnItenSeq[0];
                    string EngNo = GetEnItenSeq[1];
                    string EngItemName1 = GetEnItenSeq1[0];
                    string EnItenSeq = GetEnItenSeq1[1];
                    AppList = constCheckRecService.GetEngPostCreatedList<EngConstructionListVModel>(EnItenSeq, EngNo);
                    SetSessionManager(UserNo);
                    SharedController sc = new SharedController();
                    MenuController mc = new MenuController();
                    EngConstructionListVModel A = AppList[0];
                    int subEngNameSeq = A.subEngNameSeq;
                    homeUrl = "/MSamplingInspectionRec/Edit?id=" + subEngNameSeq;
                    Session["AppList"] = AppList;
                    Session["App_UserNo"] = UserNo;
                    Session["App_Mobile"] = Mobile;
                    Session["App_EngName"] = EngName;
                    Session["App_EngItemName"] = EngItemName;
                    Session["App_XYPosLati"] = XYPosLati;
                    Session["App_XYPosLong"] = XYPosLong;
                    Session["App_PosDesc"] = PosDesc;
                    Session["App_CheckDate"] = CheckDate;
                    Session["App_CheckItem" ]= CheckItem;
                    Session["App_RealCheckCond"]= RealCheckCond;
                    Session["App_CheckResult"]= CheckResult;
                    Session["App_CheckImage1"]= CheckImage1;
                    Session["App_CheckImage2"]= CheckImage2;
                    Session["App_CheckImage3"]= CheckImage3;
                    Session["App_CheckUser"] = CheckUser;
                    //BaseService.log.Info("Login/APPData：" + JsonConvert.SerializeObject(Session));

                }
                return Json(new
                {
                    isLogin = count > 0 ? true : false,
                    errorMessage = string.Empty,
                    homeUrl = homeUrl

                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isLogin = false,
                    errorMessage = "系統發生錯誤 登入失敗!! ",
                    exMessage = ex.Message,
                    exStackTrace = ex.StackTrace,
                    homeUrl = string.Empty
                }, JsonRequestBehavior.AllowGet);
            }
        }

        //給水利署APP進入點
        public ActionResult MobileEntryPoint(string userNo, string Mobile, int route)
        {
            string message = "";
            string homeUrl = "";
            System.Web.SessionState.HttpSessionState _session = System.Web.HttpContext.Current.Session;
            int count = userService.CheckUser2(userNo, Mobile);
            int role;
            if (userService.GetUserByAccount(userNo).FirstOrDefault(row => true) is UserInfo userInfo )
            {
                role = userInfo.RoleSeq;
            }
            else
            {
                role = 0;
            }

            if( role!= 1 && role != 2) {
                message = "你沒有權限";
            }
            else if (count > 0)
            {
                switch(route)
                {
                    //工程資訊彙整分析
                    case 0: homeUrl = "/EngAnalysisDecision";break;
                    //水利工程淨零碳排
                    case 1: homeUrl = "/EADCarbonEmission"; break;
                    //工程採購評委
                    case 2: homeUrl = "/EADCommittee"; break;
                    //品質管制弱面
                    case 3: homeUrl = "/EADPlaneWeakness"; break;
                    //廠商履歷評估
                    case 4: homeUrl = "/EADManufacturer"; break;
                    //水利工程履約風險
                    case 5: homeUrl = "/EADRisk"; break;
                        
                    default: homeUrl = "/EngAnalysisDecision"; break;

                }
                _session["UserhomeUrl"] = homeUrl;
                SetSessionManager(userNo);
                message = "登入成功";
            }
            else
            {
                message = "登入失敗";
            }
            return Json(new
            {
                isLogin = count > 0 ? true : false,
                message = message,
                homeUrl = homeUrl

            });
        }
        [Route("Login/OauthVerify")]
        [HttpPost]
        public ActionResult OauthVerify(string grant_type, string client_id, string client_secret, string redirect_uri, string code)
        {
            string token = null;
            SaveChangeStatus changeData = null;
            JObject res_array2 = new JObject();
            try
            {
                //取得 access_token
                string targetUrl = "https://cloud.wra.gov.tw/oauth2ServerToken.do";
                string parame = "grant_type=" + grant_type + "&client_id=" + client_id + "&client_secret=" + client_secret + "&code=" + code + "&redirect_uri=" + redirect_uri;
                byte[] postData = System.Text.Encoding.UTF8.GetBytes(parame);


                HttpWebRequest request = HttpWebRequest.Create(targetUrl) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 30000;
                request.ContentLength = postData.Length;
                // 寫入 Post Body Message 資料流
                using (Stream st = request.GetRequestStream())
                {
                    st.Write(postData, 0, postData.Length);
                }

                string result = "";
                //string token = null;
                string errorMsg = null;
                // 取得回應資料
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                        JObject res_array = JObject.Parse(result);
                        token = (string)res_array.GetValue("access_token");

                        if (token == "error")
                        {
                            errorMsg = (string)res_array.GetValue("errorMessage");
                            return Json(new
                            {
                                isLogin = false,
                                errorMessage = "驗證失敗 " + errorMsg,
                                homeUrl = string.Empty
                            }, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                string responseBody = "";
                /******************************************************************/
                //取得使用者資訊
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var url = "https://cloud.wra.gov.tw/oauth2ServerInfo.do";
                    HttpContent content = new StringContent(result);
                    HttpResponseMessage response2 = httpClient.PostAsync(url, content).Result;
                    responseBody = response2.Content.ReadAsStringAsync().Result;
                    res_array2 = JObject.Parse(responseBody);

                }

                int count = 0;
                string Unit = null;//單位 ex:
                string UpperUnit1 = null;//上層單位
                string UpperUnit2 = null;//最上層單位
                int ouCount = 0;//計數用最多會有3層單位
                string homeUrl = string.Empty;
                string userNo = (string)res_array2["sAMAccountName"];//帳號
                string Mail = (string)res_array2["mail"];//信箱
                string Name = (string)res_array2["displayName"];//姓名
                string Phone = (string)res_array2["mobile"];//連絡電話
                string jobTitle = (string)res_array2["title"];//職稱
                string UnitData = (string)res_array2["dn"];//部門資料
                string[] UnitArray = UnitData.Split(',').ToArray();
                foreach (string arr in UnitArray)
                {
                    string[] arr2 = arr.Split('=');
                    if (arr2.Length < 2) continue;
                    string key = arr2[0].Trim(); // 修剪空白字符
                    string value = arr2[1].Trim(); // 修剪空白字符
                    if (key == "OU")
                    {
                        ouCount++;
                        if (ouCount == 1)
                        {
                            Unit = value;
                        }
                        else if (ouCount == 2)
                        {
                            UpperUnit1 = value;
                        }
                        else if (ouCount == 3)
                        {
                            UpperUnit2 = value;
                        }
                    }
                }
                if (Unit == null)
                {
                    BaseService.log.Info($"AD Response for {client_id}:{responseBody}");

                }
                count = userService.CheckUser3(userNo, Name);
                if (count == 0)
                {
                    count = userService.OauthAddUser(userNo, Mail, Name, Phone, Unit, UpperUnit1, UpperUnit2);
                }

                if (count > 0) {

                    SetSessionManager(userNo);
                    //SharedController sc = new SharedController();
                    //MenuController mc = new MenuController();
                    System.Web.SessionState.HttpSessionState _session = System.Web.HttpContext.Current.Session;
                    UserInfo userInfo = (UserInfo)_session["UserInfo"];
                    changeData = userService.OauthUpdateUserData(userInfo.Seq, Mail, Name, Phone, Unit, UpperUnit1, UpperUnit2);
                    using (var context = new EQC_NEW_Entities())
                    {
                        context.UserLoginRecord.Add(new
                        UserLoginRecord
                        {
                            UserMainSeq = userInfo.Seq,
                            CreateTime = DateTime.Now,
                            OriginIP = Request.UserHostAddress
                        });
                        context.SaveChanges();
                    }
                    homeUrl = userHomeUrlSwitch(userInfo);
                    _session["UserhomeUrl"] = homeUrl;
                }

                return Json(new
                {
                    isLogin = count > 0 ? true : false,
                    errorMessage = string.Empty,
                    homeUrl = homeUrl,
                    changeData = changeData
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isLogin = false,
                    errorMessage = "系統發生錯誤 登入失敗!! ",
                    exMessage = ex.Message,
                    exStackTrace = ex.StackTrace,
                    token = token,
                    res_array2 = JsonConvert.SerializeObject(res_array2),
                    homeUrl = string.Empty
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Download(int Length)
        {
            if (Length == 1)
            {
                string folderName = "FileUploads/Tp/水利署水利工程品管系統_教育訓練簡報-v6-F1.pdf";
                string webRootPath = System.Web.HttpContext.Current.Server.MapPath("~");
                string fullPath = Path.Combine(webRootPath, folderName);
                if (System.IO.File.Exists(fullPath))
                {
                    Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", "水利署教育訓練手冊.pdf");
                }
                return Json(new
                {
                    result = -1,
                    message = "下載失敗"
                }, JsonRequestBehavior.AllowGet);
            }
            else if(Length == 2)
            {
                string folderName = "FileUploads/Tp/水利署水利工程品管系統_系統操作手冊(web).mp4";
                string webRootPath = System.Web.HttpContext.Current.Server.MapPath("~");
                string fullPath = Path.Combine(webRootPath, folderName);
                if (System.IO.File.Exists(fullPath))
                {
                    Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", "系統操作影片(web).mp4");
                }
                return Json(new
                {
                    result = -1,
                    message = "下載失敗"
                }, JsonRequestBehavior.AllowGet);
            }
            else if (Length == 3)
            {
                string folderName = "FileUploads/Tp/水利署水利工程品管系統_系統操作手冊(android).mp4";
                string webRootPath = System.Web.HttpContext.Current.Server.MapPath("~");
                string fullPath = Path.Combine(webRootPath, folderName);
                if (System.IO.File.Exists(fullPath))
                {
                    Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", "系統操作影片(android).mp4");
                }
                return Json(new
                {
                    result = -1,
                    message = "下載失敗"
                }, JsonRequestBehavior.AllowGet);
            }
            else if (Length == 4)
            {
                string folderName = "FileUploads/Tp/水利署水利工程品管系統_系統操作手冊(ios).mp4";
                string webRootPath = System.Web.HttpContext.Current.Server.MapPath("~");
                string fullPath = Path.Combine(webRootPath, folderName);
                if (System.IO.File.Exists(fullPath))
                {
                    Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", "系統操作影片(ios).mp4");
                }
                return Json(new
                {
                    result = -1,
                    message = "下載失敗"
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult APIUserRegister(string name)
        {
            try
            {
                string authCode = Request.Headers.Get("token");
                var apiUserService = new APIUserService();
                string actionList = apiUserService.getAuthCodeData(authCode);

                apiUserService.register(name, actionList, authCode);
                return Json(new
                {
                    status = 0,
                    message = "註冊成功"
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    status = 1,
                    message = "註冊失敗: "+ e.Message
                });
            }
        }
    }
}
