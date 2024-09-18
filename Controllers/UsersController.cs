using EQC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Models;
using EQC.ViewModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using EQC.Common;
using EQC.ViewModel.Common;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using EQC.EDMXModel;

namespace EQC.Controllers
{
    [SessionFilter]
    public class UsersController : MyController
    {
        private UserService userService = new UserService();
        private UnitService unitService = new UnitService();
        public UsersController()
        {
          
        }
        // GET: Users
        public ActionResult Index()
        {
            new SessionManager().currentSystemSeq = "2";
            return View();
        }


        public void DownloadUser()
        {
            List<VUserMain> list = userService.GetListV2(1, null, false);
            var p = new ExcelProcesser(0, (wookBook ) => {
                var sheet = wookBook.GetSheetAt(0);
                var row = sheet.CreateRow(0);
                new string[] { 
                    "姓名",
                    "帳號",
                    "機關/單位",
                    "角色",
                    "APP下載時間",                   
                    "最後登入時間",
 
                }.ToList()
                .ForEach(colName =>
                {
                    row.CreateCell(row.Cells.Count).SetCellValue(colName);
                });
            });
            p.insertOneCol(list.Select(r => r.DisplayName), 0);
            p.insertOneCol(list.Select(r => r.UserNo), 1);
            p.insertOneCol(list.Select(r => $"{r.UnitName1}{r.UnitName2}"), 2);
            p.insertOneCol(list.Select(r => $"{r.RoleName}"), 3);
            p.insertOneCol(list.Select(r => $"{r.ConstCheckAppCreateTime?.ToString("yyyy-MM-dd HH:mm:ss")}"), 4);
            p.insertOneCol(list.Select(r => $"{r.LastLoginTime?.ToString("yyyy-MM-dd HH:mm:ss")  ?? (r.ModifyTime?.ToString("yyyy-MM-dd HH:mm:ss")  ?? "" )}"), 5);
            DownloadFile(p.getTemplateStream(), "使用者清單.xlsx");
        }
        public JsonResult GetUserInfo(int? Seq = null)
        {
            using(var context = new EQC_NEW_Entities())
            {
                VUserMain.SetPositonDic(context.Position);
            }

            SessionManager sessionManager = new SessionManager();
            UserInfo userInfo = sessionManager.GetUser();
            VUserMain vuserInfo = userService.GetUserInfo(Seq ?? userInfo.Seq).First();
            var _createUserInfo = userService.GetUserInfo(vuserInfo.CreateUserSeq);
            VUserMain createUserInfo = _createUserInfo.Count > 0 ? _createUserInfo.First() : new VUserMain();
            return Json(new
            {
                userInfo = vuserInfo,
                isLastLevel =
                    (vuserInfo.RoleSeq == 4 || vuserInfo.RoleSeq == 5 || vuserInfo.RoleSeq == 6) &&
                    vuserInfo.UserNo.StartsWith(createUserInfo.UserNo)  &&
                    createUserInfo.UserNo.Length < vuserInfo.UserNo.Length 
,
                isOutsource = userInfo.IsBuildContractor || userInfo.IsSupervisorUnit || userInfo.IsOutsourceDesign
            });

        }
        public JsonResult unLockConstCheckApp(int constCheckAppLockSeq)
        {
            using (var context = new EQC_NEW_Entities())
            {
                context.ConstCheckAppLock.Remove(context.ConstCheckAppLock.Find(constCheckAppLockSeq));
                context.SaveChanges();
                return Json(true);
            }
        }

        /// <summary> 取得人員列表 </summary>
        /// <param name="page"> 頁數 </param>
        /// <param name="per_page"> 跳頁 </param>
        /// <param name="unitSeq"> 單位序號 </param>
        /// <returns></returns>
        /// 
        public JsonResult GetListV2(int page, int per_page, string nameSearch, string[] subUnit = null, bool hasConstCheckApp =false)
        {
            SessionManager sessionManager = new SessionManager();
            UserInfo userInfo = sessionManager.GetUser();
            List<VUserMain> list =  userService.GetListV2(1, nameSearch, hasConstCheckApp);
            List<string> _subUnit = new List<string>(subUnit.Length);
            subUnit = subUnit ?? new string[0] { };
            List<string> unitOptions;
            int count = 0;
            _subUnit.AddRange(subUnit);
            for (int j = 1; j < subUnit.Length; j++) _subUnit[j] = subUnit[j] != "" ? "/" + subUnit[j] : "";
            int i = 0;

            if(userInfo.IsAdmin || userInfo.IsDepartmentAdmin)
            {
                i = 0;
            }

            else if(userInfo.IsDepartmentUser)
            {
                subUnit[0] = userInfo.UnitName1;

            }
            else
            {
                //其他角色 只能編輯自己的資料
                list = userService.GetUserBelong(userInfo.Seq, hasConstCheckApp);
            }

            if(userInfo.IsAdmin || userInfo.IsDepartmentAdmin ||userInfo.IsDepartmentUser)
            {
                list = _subUnit.Aggregate(list, (current, unit) =>
                {
                    i++;
                    return unit == "" ? current :
                    current
                    .Where(row => row.GetType().GetProperty($"UnitName{i}")?.GetValue(row, null).ToString() ==  unit.ToString() && row.RoleSeq != 1 )
                        .ToList();

                });
                count = list.Count;

                list = list.getPagination<VUserMain>(page, per_page);
            }

            unitOptions = unitService.GetUnitList(subUnit, subUnit.Length);
            return Json(new
            {
                l = list,
                t = count,
                unitOptions = unitService.GetUnitList(subUnit, subUnit.Length),
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSelfUnitList()
        {
            SessionManager sessionManager = new SessionManager();
            UserInfo userInfo = sessionManager.GetUser();
            List<VUserMain> list = new List<VUserMain>();
            int? roleSeq = userInfo.Role.FirstOrDefault()?.Seq;
            int unitSeq = (int)userInfo.UnitSeq1;
            if (roleSeq == 1)
            {
                list = userService.GetList(unitSeq, "", 0, Int32.MaxValue);
            }
            else if (roleSeq == 3)
            {
                list = userService.GetDepartmentUser(unitSeq, "", 0, Int32.MaxValue);
            }
            else
            {
                //其他角色 只能編輯自己的資料
                list = userService.GetUser(userInfo.Seq);
            }
            int rows;
            Object totalRows = null;
            if (userInfo.IsAdmin == true)
                totalRows = userService.GetCount(unitSeq);
            else if (userInfo.IsDepartmentUser == true)
            {
                totalRows = userService.GetDepartmentUserCount(unitSeq);
            }
            else
            {
                totalRows = 1;
            }

            return Json(list.Select(r => new SelectVM
            {
                Text = r.DisplayName,
                Value = r.Seq.ToString()
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetList(string nameSearch, int unitSeq, int page , int per_page)
        {
            SessionManager sessionManager = new SessionManager();
            UserInfo userInfo = sessionManager.GetUser();
            List<VUserMain> list = new List<VUserMain>();
            unitSeq = unitSeq == 0 ? (int)userInfo.UnitSeq1 : unitSeq;
            if (userInfo.IsAdmin == true)
            {
                list = userService.GetList(unitSeq, nameSearch, page - 1, per_page);
            }
            else if (userInfo.IsDepartmentUser)
            {
                list = userService.GetDepartmentUser(unitSeq,nameSearch,  page - 1, per_page);
            }
            else
            {
                //其他角色 只能編輯自己的資料
                list = userService.GetUser(userInfo.Seq);
            }
            int rows;
            Object totalRows = null;
            if (userInfo.IsAdmin == true)
                totalRows = userService.GetCount(unitSeq);
            else if(userInfo.IsDepartmentUser ==  true)
            {
                totalRows = userService.GetDepartmentUserCount(unitSeq);
            }
            else
            {
                totalRows = 1;
            }

            return Json(new
            {
                l = list,
                t = totalRows,
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChildList(int page, int per_page)
        {
            var list = userService.GetChildList(page-1 , per_page);
            return Json(new
            {
                l = list,
                t = userService.GetChildListCount()
            });

        } 

        /// <summary> 儲存人員資料 </summary>
        /// <param name="vUserMain"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(VUserMain vUserMain)
        {
            SaveChangeStatus result = new SaveChangeStatus(true, StatusCode.Save);
            if (vUserMain.Seq == 0)
            {
                result = userService.AddUser(vUserMain);
            }
            else
            {
                result = userService.Update(vUserMain);
            }
            return Json(result);
        }

        /// <summary> 簽名檔 上傳 </summary>
        /// <param name="userSeq"> UserMain.Seq </param>
        /// <returns></returns>
        public JsonResult SignatureFileUpload(int userSeq)
        {
            SaveChangeStatus result = new SaveChangeStatus(true, StatusCode.Save);
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file.ContentLength > 0)
                {
                    SignatureFileVM signatureFileVM = new SignatureFileVM();
                    signatureFileVM.UserMainSeq = userSeq;
                    signatureFileVM.FilePath = string.Format(@"\FileUploads\SignatureFiles\{0}", userSeq);
                    string sFilePath = Server.MapPath(signatureFileVM.FilePath);
                    if (!Directory.Exists(sFilePath))
                    {
                        Directory.CreateDirectory(sFilePath);
                    }
                    signatureFileVM.DisplayFileName = file.FileName.ToString().Trim();
                    int inx = signatureFileVM.DisplayFileName.LastIndexOf(".");
                    signatureFileVM.FileName = String.Format("{0}{1}", Guid.NewGuid(), signatureFileVM.DisplayFileName.Substring(inx));
                    string fullPath = Path.Combine(sFilePath, signatureFileVM.FileName);
                    file.SaveAs(fullPath);

                    result = userService.SaveSignatureFile(signatureFileVM);
                }
            }
            return Json(result);
        }

        /// <summary> 簽名檔 下載 </summary>
        /// <param name="userSeq"> UserMain.Seq </param>
        /// <returns></returns>
        public ActionResult SignatureFileDownload(int userSeq)
        {
            List<SignatureFileVM> signatureFileVMList = userService.GetSignatureFileByUserSeq(userSeq);
            if (signatureFileVMList.Count > 0)
            {
                SignatureFileVM signatureFileVM = signatureFileVMList.First();
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                string fullPath = Path.Combine(Server.MapPath(signatureFileVM.FilePath), signatureFileVM.FileName);
                Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(stream, "application/octet-stream", signatureFileVM.DisplayFileName);
            }
            return null;
        }

        /// <summary> 取得單位(下拉選單資料) </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetPositionList()
        {
            List<SelectVM> list = userService.GetPositionList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary> 刪除使用者 </summary>
        /// <param name="Seq"> Seq </param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteUser(int Seq)
        {
            var result = userService.DeleteUser(Seq);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserBySubUnit(int subUnitSeq)
        {
            int userCount = userService.GetDepartmentUserCount(subUnitSeq);
            List<VUserMain> users = userService.GetDepartmentUser(subUnitSeq, "%", 0, userCount);
            List<SelectVM> option = users.Select(user => new SelectVM
            {
                Value = user.Seq.ToString(),
                Text = user.DisplayName
            }).ToList();
            return Json(option);

        }
        public void GetUserByAccountKeyWord(string keyWord, int role)
        {

            var list = userService.GetUserByAccountKeyWord(keyWord, role);
            ResponseJson(list);
        }
    }
}
