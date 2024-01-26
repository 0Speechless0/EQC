using EQC.Common;
using System;

namespace EQC.ViewModel
{
    public class CEFEngsVModel
    {
        //工程清單
        public int Seq { get; set; }
        
        public string EngNo { get; set; }
        public string EngName { get; set; }
        public Int16 EngYear { get; set; }
        public string ExecUnitName { get; set; }
        public string ExecSubUnitName { get; set; }
        public DateTime? AwardDate { get; set; }
        public int? CarbonEmissionHeaderState { get; set; }
        public int? ApprovedCarbonQuantity { get; set; }
        public int? CarbonDesignQuantity { get; set; }
        public int? CarbonTradeQuantity { get; set; }
        
        public string AwardDateStr
        {
            get { 
                return Utils.ChsDate(AwardDate);
            }
        }
        public bool DiffYear
        {
            get
            {
                if (AwardDate.HasValue)
                {
                    return (AwardDate.Value.Year - 1911 != EngYear);
                }
                else
                    return false;
            }
        }
    }
}