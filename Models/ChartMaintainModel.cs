using System;

namespace EQC.Models
{
    public class ChartMaintainModel
    {//圖表維護
        public int Seq { get; set; }
        public int ChapterSeq { get; set; }
        public int? OrderNo { get; set; }
        public int? ExcelNo { get; set; }
        public byte? ChartKind { get; set; }
        public string ChartName { get; set; }
        public DateTime? RevisionDate { get; set; }
        public string OriginFileName { get; set; }
        public string UniqueFileName { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
