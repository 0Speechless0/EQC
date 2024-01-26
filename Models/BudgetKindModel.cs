using System;

namespace EQC.Models
{
    public class BudgetKindModel
    {//預算別
        public int Seq { get; set; }
        public int BudgetingSeq { get; set; }
        public string BudgetType { get; set; }
        public string BudgetSource { get; set; }
        public string BudgetAccount { get; set; }
        public Decimal? LegalBudget { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
