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
    
    public partial class CarbonEmissionPayItem
    {
        public int Seq { get; set; }
        public int CarbonEmissionHeaderSeq { get; set; }
        public string PayItem { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string ItemKey { get; set; }
        public string ItemNo { get; set; }
        public string RefItemCode { get; set; }
        public Nullable<decimal> KgCo2e { get; set; }
        public Nullable<decimal> ItemKgCo2e { get; set; }
        public string Memo { get; set; }
        public string RStatus { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> CreateUserSeq { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
        public Nullable<int> ModifyUserSeq { get; set; }
        public Nullable<int> RStatusCode { get; set; }
        public Nullable<int> GreenFundingSeq { get; set; }
        public string GreenFundingMemo { get; set; }
    
        public virtual CarbonEmissionHeader CarbonEmissionHeader { get; set; }
    }
}
