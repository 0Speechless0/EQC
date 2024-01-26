using EQC.Models;
using System;

namespace EQC.ViewModel
{//工程核定資料匯入 s20231006
    public class EngApprovalImportVModel:EngApprovalImportModel
    {
        public string ExecUnitName { get; set; }
        public string ExecSubUnitName { get; set; }
        public string ExecUserName { get; set; }
    }
}