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
    
    public partial class EnvirConsControlSt
    {
        public int Seq { get; set; }
        public Nullable<int> EnvirConsListSeq { get; set; }
        public byte ECCFlow1 { get; set; }
        public string ECCCheckItem1 { get; set; }
        public string ECCCheckItem2 { get; set; }
        public string ECCStand1 { get; set; }
        public string ECCStand2 { get; set; }
        public string ECCStand3 { get; set; }
        public string ECCStand4 { get; set; }
        public string ECCStand5 { get; set; }
        public string ECCCheckTiming { get; set; }
        public string ECCCheckMethod { get; set; }
        public string ECCCheckFeq { get; set; }
        public string ECCIncomp { get; set; }
        public string ECCManageRec { get; set; }
        public Nullable<byte> ECCType { get; set; }
        public Nullable<bool> ECCMemo { get; set; }
        public Nullable<int> ECCCheckFields { get; set; }
        public Nullable<int> ECCManageFields { get; set; }
        public Nullable<int> OrderNo { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> CreateUserSeq { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
        public Nullable<int> ModifyUserSeq { get; set; }
        public bool DataKeep { get; set; }
        public byte DataType { get; set; }
    
        public virtual EnvirConsList EnvirConsList { get; set; }
    }
}
