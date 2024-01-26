using EQC.Common;
using System;

namespace EQC.ViewModel
{
    public class EPCEngTenderListVMode
    {//工程標案清單
        public int Seq { get; set; }
        public string TenderName { get; set; }
        public string ExecUnitName { get; set; }
        public DateTime ModifyTime { get; set; }
        public string ExecState { get; set; }
        public decimal PRTotalAmountPayable { get; set; }
        public string TotalAmountPayable
        {
            get
            {
                decimal value = PRTotalAmountPayable / 10;
                decimal rate = Math.Round(value, 0);
                return rate.ToString() + "萬";
            }
        }
        public decimal PRAccuPayAmount { get; set; }
        public string AccuPayAmount
        {
            get
            {
                decimal value = PRAccuPayAmount / 10;
                decimal rate = Math.Round(value, 0);
                return rate.ToString() + "萬";
            }
        }
        public string PayRate {
            get {
                if (PRTotalAmountPayable == 0)
                    return "";
                else {
                    decimal value = PRAccuPayAmount * 100 / PRTotalAmountPayable;
                    decimal rate = Math.Round(value, 0);
                    return rate.ToString()+"%";
                }
            }
        }
        public string LastDate
        {
            get
            {
                return this.ModifyTime.ToString("yyyy-MM-dd");
            }
        }
    }
}
