//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace EQC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TechnicalReply
    {
        public int TechnicalCommentSeq { get; set; }
        public string Text { get; set; }
        public int Author { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
    
        public virtual TechnicalComment TechnicalComment { get; set; }
    }
}