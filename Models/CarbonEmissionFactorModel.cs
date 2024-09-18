using EQC.ViewModel.Interface;
using System;

namespace EQC.Models
{
    public class CarbonEmissionFactorModel : CarbonReduction
    {//碳排係數維護
        public int Seq { get; set; }

        public string Code { get; set; }
        public string Type2 { get; set; }
        public string Item { get; set; }
        public decimal? KgCo2e { get; set; }
        public string Unit { get; set; }
        public byte Kind { get; set; }
        public bool IsEnabled { get; set; }
        public string SubCode { get; set; }
        public string Memo { get; set; }
        public string KeyCode1 { get; set; }
        public string KeyCode2 { get; set; }
        public string KeyCode3 { get; set; }
        public bool Green { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}
