//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace EQC.EDMXModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class SupDailyReportConstructionMaterial
    {
        public int Seq { get; set; }
        public Nullable<int> SupDailyDateSeq { get; set; }
        public string MaterialName { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> ContractQuantity { get; set; }
        public decimal TodayQuantity { get; set; }
        public string Memo { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> CreateUserSeq { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
        public Nullable<int> ModifyUserSeq { get; set; }
    
        public virtual SupDailyDate SupDailyDate { get; set; }
    }
}