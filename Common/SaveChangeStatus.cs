using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Common
{
    /// <summary> 操作動作代碼 </summary>
    public enum StatusCode
    {
        Save = 0,
        Delete = 1,
        Validation = 2,
        Import = 3,
        Export = 4,
        Send = 5,
        BackUp = 6
    }

    /// <summary> 資料儲存狀態 </summary>
    public class SaveChangeStatus
    {
        #region Constructor

        /// <summary> 資料異動狀態 </summary>
        /// <param name="isSuccess"> 是否成功 </param>
        /// <param name="code"> 操作動作 </param>
        /// <param name="ex"> 例外訊息 </param>
        public SaveChangeStatus(bool isSuccess, StatusCode code, Exception ex = null)
        {
            //是否成功
            this.IsSuccess = isSuccess;
            //操作動作
            this.Code = code;
            //設定Error訊息
            if (ex != null)
            {
                SetModifyStatusWithException(ex);
            }

            //設定結果訊息
            this.Message = GetMessage(isSuccess, code);
        }

        /// <summary> 資料異動狀態 </summary>
        /// <param name="message"> 訊息 </param>
        public SaveChangeStatus(string message)
        {
            //是否成功
            this.IsSuccess = false;
            this.Message = message;
        }

        #endregion Constructor

        /// <summary> 是否成功 </summary>
        public bool IsSuccess { get; set; }

        /// <summary> 訊息 </summary>
        public string Message { get; set; }

        /// <summary> 例外訊息 </summary>
        public string ExceptionMessage { get; set; }

        /// <summary> 例外訊息(取得呼叫堆疊上之立即框架的字串表示) </summary>
        public string ExceptionStackTrace { get; set; }

        /// <summary> 是否為Debug </summary>
        public bool IsDebug
        {
            get
            {
                try
                {
                    return bool.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["IsDebug"]);
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary> 回傳的資料 </summary>
        public dynamic Data { get; set; }

        /// <summary> 操作動作 </summary>
        public StatusCode Code { get; set; }

        /// <summary> 驗證結果，前端使用 </summary>
        public List<VaildationVM> ValidationFrontEnd { get; set; }

        /// <summary> 設定更深層的例外訊息 </summary>
        /// <param name="ex"> 例外訊息 </param>
        private void SetModifyStatusWithException(Exception ex)
        {
            if (this.IsDebug)
            {
                //if (ex.Message.Contains("請參閱內部例外狀況"))
                if (ex.InnerException != null)
                {
                    this.ExceptionMessage = ex.GetBaseException().Message;
                    this.ExceptionStackTrace = ex.StackTrace;
                }
                else
                {
                    this.ExceptionMessage = ex.Message;
                    this.ExceptionStackTrace = ex.StackTrace;
                }
            }
        }

        /// <summary> 取得訊息 </summary>
        /// <param name="isSuccess"> 是否成功 </param>
        /// <param name="code"> 操作動作 </param>
        /// <returns> 訊息 </returns>
        private string GetMessage(bool isSuccess, StatusCode code)
        {
            string message = string.Empty;
            switch (code)
            {
                case StatusCode.Save:
                    message = isSuccess ? "儲存成功" : "儲存失敗";
                    break;

                case StatusCode.Delete:
                    message = isSuccess ? "刪除成功" : "刪除失敗";
                    break;

                case StatusCode.Validation:
                    message = isSuccess ? "驗證成功" : "驗證失敗";
                    break;

                case StatusCode.Import:
                    message = isSuccess ? "匯入成功" : "匯入失敗";
                    break;

                case StatusCode.Export:
                    message = isSuccess ? "匯出成功" : "匯出失敗";
                    break;

                case StatusCode.Send:
                    message = isSuccess ? "發送成功" : "發送失敗";
                    break;

                case StatusCode.BackUp:
                    message = isSuccess ? "備份成功" : "備份失敗";
                    break;

                default:
                    break;
            }
            return message;
        }
    }

    /// <summary> 驗證資訊 </summary>
    public class VaildationVM
    {
        /// <summary> 欄位名稱 </summary>
        public string FieldName { get; set; }

        /// <summary> 驗證失敗原因 </summary>
        public string ErrorMessage { get; set; }
    }
}