using System;

namespace EQC.Models
{
    public class EngMaterialDeviceTestSummaryModel
    {//材料設備檢驗管制總表
        public int Seq { get; set; }
        public int EngMaterialDeviceListSeq { get; set; }
        public int OrderNo { get; set; }
        public string ItemNo { get; set; }
        public string MDName { get; set; }
        public DateTime? SchTestDate { get; set; }
        public DateTime? RealTestDate { get; set; }
        public Decimal TestQty { get; set; }
        public DateTime? SampleDate { get; set; }

        public Decimal SampleQty { get; set; }
        public string SampleFeq { get; set; }
        public Decimal AccTestQty { get; set; }
        public Decimal AccSampleQty { get; set; }
        public int TestResult { get; set; }
        public string Coworkers { get; set; }
        public string ArchiveNo { get; set; }

        public DateTime? CreateTime { get; set; }
        public int CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int ModifyUserSeq { get; set; }

        public byte ItemType { get; set; } //s20230308
        public int RefSeq { get; set; }//s20230308
    }
}