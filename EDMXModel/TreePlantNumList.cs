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
    
    public partial class TreePlantNumList
    {
        public int Seq { get; set; }
        public Nullable<int> TreePlantSeq { get; set; }
        public Nullable<int> TreeTypeSeq { get; set; }
        public Nullable<int> ScheduledPlantNum { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> CreateUserSeq { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
        public Nullable<int> ModifyUserSeq { get; set; }
        public Nullable<int> ActualPlantNum { get; set; }
        public string TreeTypeName { get; set; }
    
        public virtual TreePlantMain TreePlantMain { get; set; }
    }
}