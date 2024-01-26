using System;
using System.Collections.Generic;

namespace EQC.Models
{
    public class BudgetingModel
    {//預算編列
        public int Seq { get; set; }
        public int PrjXMLSeq { get; set; }
        public string BudgetYear { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }

        public List<BudgetKindModel> BudgetKindList = new List<BudgetKindModel>();
    }
}
