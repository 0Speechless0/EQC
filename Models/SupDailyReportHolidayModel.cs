using System;

namespace EQC.Models
{
    public class SupDailyReportHolidayModel
    {//監造(施工)日誌_設定假日計工期
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public byte DaySet { get; set; }
        public string Descript { get; set; }
        /*public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }*/
    }
}
