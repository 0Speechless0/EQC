using EQC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Services;
using EQC.ViewModel;
using System.Configuration;

namespace EQC.Common
{
    [Serializable]
    public class SessionManager
    {
        private System.Web.SessionState.HttpSessionState _session;
        private UserService userService = new UserService();
        private MenuService menuService = new MenuService();
        static UserInfo _userInfo = new UserInfo();
        public SessionManager()
        {
            if (HttpContext.Current != null)
            {
                _session = HttpContext.Current.Session;
                if (_session["UserInfo"] == null)
                {
                    _session["UserInfo"] = new UserInfo();
                }
            }
        }
        public string GetUserHome()
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Session["UserhomeUrl"].ToString();
            } else
                return "";
        }

        /// <summary>
        /// 系統別序號
        /// </summary>
        public byte SystemTypeSeq
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// 登入狀態
        /// </summary>
        public bool LoginStatus
        {
            get
            {
                if (_session["LoginStatus"] == null)
                {
                    return false;
                }
                return bool.Parse(_session["LoginStatus"].ToString());
            }
            set
            {
                _session["LoginStatus"] = value;
            }
        }

        #region UserInfo

        /// <summary>
        /// 取得使用者
        /// </summary>
        /// <returns></returns>
        public UserInfo GetUser()
        {
            if (ConfigurationManager.AppSettings.Get("Debug") == null)
            {
                return (UserInfo)_session["UserInfo"];
            }
            return _userInfo;
          
        }

        /// <summary>
        /// 設定使用者
        /// </summary>
        /// <param name="userInfo"></param>
        public void SetUser(UserInfo userInfo)
        {
            _session["UserInfo"] = userInfo;//shioulo 20210707
            _session["PttInfo"] = true;

            //用於Debug
            _userInfo = userInfo;

            /*UserInfo _userInfo = new UserInfo();
            _userInfo.Seq = userInfo.Seq;
            _userInfo.UserNo = userInfo.UserNo;
            _userInfo.DisplayName = userInfo.DisplayName;
            _userInfo.UnitName = userInfo.UnitName;
            //_userInfo.UnitSeq = userInfo.UnitSeq;
            _userInfo.PositionName = userInfo.PositionName;
            //_userInfo.PositionSeq = userInfo.PositionSeq;
            _userInfo.Role = userInfo.Role;
            _userInfo.MenuList = userInfo.MenuList;
            _session["UserInfo"] = _userInfo;*/
        }

        #endregion UserInfo

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="filterContext"></param>
        public void LogOut(ActionExecutingContext filterContext)
        {
            HttpContext context = System.Web.HttpContext.Current;
            string url = string.Empty;
            if (!context.Request.Url.Host.Equals("localhost"))
            {
                url = "http://" + context.Request.Url.Authority + ConfigManager.PortalSite;
            }
            else
            {
                url = "https://" + context.Request.Url.Host + ":" + ConfigManager.PortalSiteLocalhostPort + ConfigManager.PortalSite;
            }
            HttpContext httpContext = System.Web.HttpContext.Current;
            httpContext.Session.Clear();
            filterContext.Result = new RedirectResult(url);
        }

        public void CheckSession(System.Web.SessionState.HttpSessionState _session)
        {
            SessionManager sm = new SessionManager();
            string account = _session["Account"].ToString();
            // 傳進來Session的值與存在SessionManager不一樣時, 將SessionManager重新取值
            if (sm.GetUser().UserNo != account )
            {
                UserInfo userData = userService.GetUserByAccount(account).FirstOrDefault();
                List<Role> roleData = null; 
                List<VMenu> menuData = null;
                if (userData != null)
                {
                    roleData = userService.GetRoleByAccount(account);//shioulo 20210707 
                    menuData = menuService.LoadMenu(1, userData.Seq);
                    menuData.AddRange(menuService.LoadMenu(2, userData.Seq));
                    menuData.AddRange(menuService.LoadMenu(3, userData.Seq));//shioulo 20220426 
                    
                    menuData.AddRange(menuService.LoadMenu(4, userData.Seq)); //shioulo 20230530
                    //menuData.AddRange(menuService.LoadMenu(5, userData.Seq));
                    //menuData.AddRange(menuService.LoadMenu(6, userData.Seq));
                    //menuData.AddRange(menuService.LoadMenu(7, userData.Seq));
                    menuData.AddRange(menuService.LoadMenu(8, userData.Seq));
                    menuData.AddRange(menuService.LoadMenu(9, userData.Seq));
                    menuData.AddRange(menuService.LoadMenu(10, userData.Seq));
                    menuData.AddRange(menuService.LoadMenu(11, userData.Seq));
                    menuData.AddRange(menuService.LoadMenu(12, userData.Seq));//s20230311
                    menuData.AddRange(menuService.LoadMenu(20, userData.Seq));//alex20230327vm 

                    //在此新增節點進入權依據 menuData.AddRange(menuService.LoadMenu(20, userData.Seq))
                    SetSession(userData, roleData, menuData);
                }
            }
        }

        private void SetSession(UserInfo userData, List<Role> roleData, List<VMenu> menuData)
        {
            LoginStatus = true;

            //shioulo 20210707
            userData.Role = roleData;
            userData.MenuList = menuData;
            userData.SystemList = menuService.LoadSystemMenu(userData.Seq);
            SetUser(userData);
            /*SetUser(new EQC.Common.UserInfo() 
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
            //shioulo 20210707
        }
    }
}