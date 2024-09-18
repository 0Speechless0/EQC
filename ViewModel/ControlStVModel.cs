
using EQC.EDMXModel;
using System;
using System.Linq;

namespace EQC.Models
{//抽查標準
    public class ControlStVModel : ConstCheckRecResultModel
    {
        public string CheckItem1 { get; set; }
        public string CheckItem2 { get; set; }
        public string Stand1 { get; set; }
        public string Stand2 { get; set; }
        public string Stand3 { get; set; }
        public string Stand4 { get; set; }
        public string Stand5 { get; set; }
        public int CheckFields { get; set; }

        public int rowSpan { get; set; }
        public int rowSpanStd1 { get; set; }
        public bool rowShow { get; set; }
        public bool changed { get; set; }
        public int itemType { get; set; } //s20231016
        public string RecResultRemark { get;  set; }

        public string StandardValuesStr { get; set; }

        public string StandardFilled(EQC_NEW_Entities standardValesContext) { 
            
                var standardFilled = Stand1.Aggregate("", (a, c) => {
                    if (c != '_')
                        a += c;
                    if (a.Length > 0 && a[a.Length - 1] != '_' && c == '_')
                        a += c;
                    return a;
                });
                standardValesContext?.ConstCheckRecResultStandard.Where(r => r.ConstCheckRecResultSeq == Seq)
                    .ToList()
                    .ForEach(e =>
                    {
                        standardFilled = standardFilled.Replace("_", e.Value);
                    });
                return standardFilled ==  "" ? null : standardFilled; 
        }
    }
}