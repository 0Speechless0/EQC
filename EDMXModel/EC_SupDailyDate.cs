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
    
    public partial class EC_SupDailyDate
    {
        public int Seq { get; set; }
        public Nullable<int> EngMainSeq { get; set; }
        public System.DateTime ItemDate { get; set; }
        public byte DataType { get; set; }
        public int ItemState { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> CreateUserSeq { get; set; }
        public string Weather1 { get; set; }
        public string Weather2 { get; set; }
        public Nullable<System.DateTime> FillinDate { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
        public Nullable<int> ModifyUserSeq { get; set; }
    
        public virtual EngMain EngMain { get; set; }
    }
}
