using System;

namespace EQC.ViewModel
{
    public class EHI_PlaneWeaknessVModel : EHI_PrjXMLVModel
    {//工程品質弱面分析
        public decimal? BidAmount { get; set; }
        public string Location { get; set; }
        public byte W1 { get; set; }
        public byte W2 { get; set; }
        public byte W3 { get; set; }
        public byte W4 { get; set; }
        public byte W5 { get; set; }
        public byte W6 { get; set; }
        public byte W7 { get; set; }
        public byte W8 { get; set; }
        public byte W9 { get; set; }
        public byte W10 { get; set; }
        public byte W11 { get; set; }
        public byte W12 { get; set; }
        public byte W13 { get; set; }
        public byte W14 { get; set; }

        public string getWeakStr {
            get
            {
                string str = "", sp = "";
                if(W1 > 0)
                {
                    str += sp + "1";
                    sp = ",";
                }
                if (W2 > 0)
                {
                    str += sp + "2";
                    sp = ",";
                }
                if (W3 > 0)
                {
                    str += sp + "3";
                    sp = ",";
                }
                if (W4 > 0)
                {
                    str += sp + "4";
                    sp = ",";
                }
                if (W5 > 0)
                {
                    str += sp + "5";
                    sp = ",";
                }
                if (W6 > 0)
                {
                    str += sp + "6";
                    sp = ",";
                }
                if (W7 > 0)
                {
                    str += sp + "7";
                    sp = ",";
                }
                if (W8 > 0)
                {
                    str += sp + "8";
                    sp = ",";
                }
                if (W9 > 0)
                {
                    str += sp + "9";
                    sp = ",";
                }
                if (W10 > 0)
                {
                    str += sp + "10";
                    sp = ",";
                }
                if (W11 > 0)
                {
                    str += sp + "11";
                    sp = ",";
                }
                if (W12 > 0)
                {
                    str += sp + "12";
                    sp = ",";
                }
                if (W13 > 0)
                {
                    str += sp + "13";
                    sp = ",";
                }
                if (W14 > 0)
                {
                    str += sp + "14";
                    sp = ",";
                }

                return str;
            }
        }
    }
}
