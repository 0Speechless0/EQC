using System;

namespace EQC.Models
{
    public class EngRiskFrontSubProjectListModel
    {//施工風險內容及施作順序
        public int Seq { get; set; }
        public int EngRiskFrontSeq { get; set; }
        public string ExcelNo { get; set; }
        public string SubProjectName { get; set; }
        public string SubProjectJson { get; set; }
        public string SubProjectFile { get; set; }
        public string ExistingProtectiveFacilities { get; set; }
        public string Equipment { get; set; }
        public string EngControl { get; set; }
        public string ManagementControl { get; set; }
        public string PersonalProtectiveEquipment { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
        public bool IsEnabled { get; set; }
    }
}