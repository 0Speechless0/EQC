using System;

namespace EQC.Models
{
    public class PaymentRecordModel
    {//歷次付款資料
        public int Seq { get; set; }
        public int PrjXMLSeq { get; set; }
        public int? PRItemNo { get; set; }
        public decimal? PRTotalAmountPayable { get; set; }
        public string PRPayDate { get; set; }
        public decimal? PRPayAmount { get; set; }
        public string PRSchePayDate { get; set; }
        public decimal? PRSchePayAmount { get; set; }
        public string PRActualPayDate { get; set; }
        public decimal? PRActualPayAmount { get; set; }
        public decimal? PRAccuPayAmount { get; set; }
        public string PRMemo { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
