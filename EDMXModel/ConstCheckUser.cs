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
    
    public partial class ConstCheckUser
    {
        public Nullable<int> UserSeq { get; set; }
        public int EngSeq { get; set; }
        public Nullable<short> UnitSeq { get; set; }
    
        public virtual Unit Unit { get; set; }
        public virtual UserMain UserMain { get; set; }
        public virtual EngMain EngMain { get; set; }
    }
}