using System;

namespace EQC.ViewModel
{
    public class EPCSuperviseFillVModel
    {//督導
        public int SEngSeq { get; set; }
        public DateTime? SuperviseDate { get; set; }
        public byte? SuperviseMode { get; set; }
        public int Seq { get; set; }
        public string Missing { get; set; }
        public DateTime? SchImprovDate { get; set; }
        public DateTime? ActImprovDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public string SuperviseModeStr
        {
            get
            {
                if (SuperviseMode.HasValue)
                {
                    switch(SuperviseMode.Value)
                    {
                        case 0: return "工程施工督導"; break;
                        case 1: return "走動式督導"; break;
                        case 2: return "異常工程督導"; break;
                    }

                }
                return "";
            }
        }
        public string SuperviseDateStr
        {
            get
            {
                 return SuperviseDate.HasValue ? SuperviseDate.Value.ToString("yyyy-MM-dd") : "";
            }
        }
        public string SchImprovDateStr
        {
            get
            {
                return SchImprovDate.HasValue ? SchImprovDate.Value.ToString("yyyy-MM-dd") : "";
            }
        }
        public string ActImprovDateStr
        {
            get
            {
                return ActImprovDate.HasValue ? ActImprovDate.Value.ToString("yyyy-MM-dd") : "";
            }
        }
        public string CloseDateStr
        {
            get
            {
                return CloseDate.HasValue ? CloseDate.Value.ToString("yyyy-MM-dd") : "";
            }
        }

    }
}
