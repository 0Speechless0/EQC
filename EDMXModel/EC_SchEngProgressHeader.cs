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
    
    public partial class EC_SchEngProgressHeader
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EC_SchEngProgressHeader()
        {
            this.EC_SchEngProgressPayItem = new HashSet<EC_SchEngProgressPayItem>();
        }
    
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public int Version { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> SchCompDate { get; set; }
        public int SPState { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> CreateUserSeq { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
        public Nullable<int> ModifyUserSeq { get; set; }
        public int ChangeType { get; set; }
        public Nullable<int> SupDailyReportExtensionSeq { get; set; }
        public Nullable<int> SupDailyReportWorkSeq { get; set; }
    
        public virtual EngMain EngMain { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EC_SchEngProgressPayItem> EC_SchEngProgressPayItem { get; set; }
    }
}