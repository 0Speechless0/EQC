using System;

namespace EQC.Models
{
    public class ChapterModel
    {//章節
        public int Seq { get; set; }
        public int? ChapterNo { get; set; }
        public string ChapterName { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
