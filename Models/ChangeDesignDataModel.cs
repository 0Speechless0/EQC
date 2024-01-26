using System;

namespace EQC.Models
{
    public class ChangeDesignDataModel
    {//變更設計資料
        public int Seq { get; set; }
        public int PrjXMLSeq { get; set; }
        public string BudgetType { get; set; }
        public string CDChangeDate { get; set; }
        public string CDAnnoNo { get; set; }
        public string CDApprovalNo { get; set; }
        public Decimal? CDBeforeAmount { get; set; }
        public Decimal? CDAfterAmount { get; set; }
        public string CDBeforeCloseDate { get; set; }
        public string CDAfterCloseDate { get; set; }
        public int? CDApprovalDays { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
