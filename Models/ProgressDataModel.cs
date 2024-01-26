using System;
using System.Collections.Generic;

namespace EQC.Models
{
    public class ProgressDataModel
    {//進度資料
        public int Seq { get; set; }
        public int PrjXMLSeq { get; set; }
        public int? PDYear { get; set; }
        public int? PDMonth { get; set; }
        public decimal? PDAccuScheProgress { get; set; }
        public decimal? PDAccuActualProgress { get; set; }
        public decimal? PDAccuScheCloseAmount { get; set; }
        public decimal? PDAccuActualCloseAmount { get; set; }
        public decimal? PDYearAccuScheProgress { get; set; }
        public decimal? PDYearAccuActualProgress { get; set; }
        public decimal? PDYearAccuScheCloseAmount { get; set; }
        public decimal? PDYearAccuActualCloseAmount { get; set; }
        public decimal? PDAccuEstValueAmount { get; set; }
        public int? PDEstValueRetentAmount { get; set; }
        public string PDExecState { get; set; }
        public string PDActualExecMemo { get; set; }
        public decimal? PDAccountPayable { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
        public int? AccuDay { get; set; }
        public string LatePaymentReason { get; set; }
        public string ScheExecMemo { get; set; }
        public string TerminationReason { get; set; }
        public int? SupervisionTimes { get; set; }
        public decimal? DiffProgress { get; set; }

        public List<BudgetKindModel> BudgetKindList = new List<BudgetKindModel>();
    }
}
