using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using EQC.Common;
using System.Web.Routing;
using System.Configuration;
using EQC.Detection;
namespace EQC.Common
{
    public class SessionFilterBase : ActionFilterAttribute
    {
        private UserService userService = new UserService();
        private MenuService menuService = new MenuService();
        private HttpContext httpContext = HttpContext.Current;

        public static SessionManager _sm
        {
            get
            {
                return new SessionManager();
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext httpcontext = HttpContext.Current;

            string currentControllerName = (filterContext.ActionDescriptor).ControllerDescriptor.ControllerName + "/";
            string currentActionName = (filterContext.ActionDescriptor).ActionName;

            // 判斷權限
            string controllerName = string.Empty;
            UserInfo userInfo;
            userInfo = _sm.GetUser();

            if (ConfigurationManager.AppSettings.Get("Debug") == null)
            {

                bool isUse = false;
                int? systemType = userInfo.SystemList?.FirstOrDefault(row => row.PathName == HttpContext.Current.Request.Path.Remove(0, 1))?.SystemType;
                List<VMenu> vMenuList = userInfo.MenuList;
                if (systemType != null)
                {
                    HttpContext.Current.Session["SystemType"] = systemType;
                }
                else if (HttpContext.Current.Request.Path.Contains("/Portal"))
                {
                    HttpContext.Current.Session["SystemType"] = 0;
                }
                // 確認目前要求的HttpSessionState
                if (httpcontext.Session != null)
                {
                    if (_sm.LoginStatus == true || currentControllerName == "API/")
                    {
                        //背景 controller 依據主 controller 來檢查權限 shioulo 20210520
                        currentControllerName = currentControllerName.Replace("QCStdSt", "SupervisionPlan");//監造計畫 < 抽查管理標準

                        if (userInfo != null)
                        {
                            if (vMenuList != null)
                            {
                                if (vMenuList.Any(w => w.Url != null && w.Url.Contains(currentControllerName)))
                                {
                                    isUse = true;
                                }
                            }
                            else if (currentControllerName == "API/")
                            {
                                isUse = true;
                            }
                        }
                        else
                        {
                            LogOut(filterContext);
                        }
                    }
                    else
                    {
                        if (filterContext.RequestContext.HttpContext.Request.RequestType == "POST")
                            PostLogOut(filterContext);
                        else
                            LogOut(filterContext);
                    }
                    if (isUse)
                    {
                        base.OnActionExecuting(filterContext);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("非選單權限: Ctl:{0}, Action:{1}", currentControllerName, currentActionName));
                        //shioulo 20210523 由於其它功能像有跨 Controller 且不在權限控制項內, 故先暫停嚴管機制
                        //toHome(filterContext);
                    }
                }
            }

        }

        /// <summary> 回首頁 </summary>
        /// <param name="filterContext"></param>
        private void toHome(ActionExecutingContext filterContext) //shioulo 20210519
        {
            HttpContext context = HttpContext.Current;
            string url = string.Empty;
            RouteValueDictionary dictionary = new RouteValueDictionary(
            new
            {
                controller = "FrontDesk",
                action = "",
                returnUrl = filterContext.HttpContext.Request.RawUrl
            });
            filterContext.Result = new RedirectToRouteResult(dictionary);
            base.OnActionExecuting(filterContext);
        }

        /// <summary> 系統登出 </summary>
        /// <param name="filterContext"></param>
        private void LogOut(ActionExecutingContext filterContext)
        {
            HttpContext context = HttpContext.Current;
            string url = string.Empty;
            RouteValueDictionary dictionary = new RouteValueDictionary(
            new
            {
                controller = "Login",
                action = "LoginOut",
                returnUrl = filterContext.HttpContext.Request.RawUrl
            });
            filterContext.Result = new RedirectToRouteResult(dictionary);
            base.OnActionExecuting(filterContext);
        }
        private void PostLogOut(ActionExecutingContext filterContext)
        {
            JsonResult jr = new JsonResult();
            //jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //jr.MaxJsonLength = int.MaxValue;
            var link = new UrlHelper(filterContext.RequestContext).Action("LoginOut", "Login");
            jr.Data = new
            {
                result = -1919,
                url = link
            };
            filterContext.Result = jr;
            base.OnActionExecuting(filterContext);
        }

        private void CheckSession(System.Web.SessionState.HttpSessionState _session)
        {
            SessionManager sm = new SessionManager();
            string account = _session["Account"].ToString();
            // 傳進來Session的值與存在SessionManager不一樣時, 將SessionManager重新取值
            if (sm.GetUser().UserNo != account)
            {
                UserInfo userData = userService.GetUserByAccount(account).FirstOrDefault();//shioulo 20210707
                List<Role> roleData = null;
                List<VMenu> menuData = null;
                if (userData != null)
                {
                    roleData = userService.GetRoleByAccount(account);
                    menuData = menuService.LoadMenu(_sm.SystemTypeSeq, _sm.GetUser().Seq);
                    SetSession(userData, roleData, menuData);
                }
            }
        }

        private void SetSession(UserInfo userData, List<Role> roleData, List<VMenu> menuData)
        {
            _sm.LoginStatus = true;
            //shioulo 20210707
            userData.Role = roleData;
            userData.MenuList = menuData;
            _sm.SetUser(userData);
            /*_sm.SetUser(new EQC.Common.UserInfo()
            {
                Seq = userData.Seq,
                UserNo = userData.UserNo,
                DisplayName = userData.DisplayName,
                //UnitSeq = userData.UnitSeq,
                //UnitName = userData.UnitName,
                //PositionSeq = userData.PositionSeq,
                PositionName = userData.PositionName,
                Role = roleData,
                MenuList = menuData
            });*/
        }
    }
}