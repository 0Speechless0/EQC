using System;

namespace EQC.Models
{
    public class PriceIndexItemModel
    {//物價指數
        public int Seq { get; set; }
        public int PriceIndexKindId { get; set; }
        public DateTime PIDate { get; set; }
        public decimal PriceIndex { get; set; }
    }
}
