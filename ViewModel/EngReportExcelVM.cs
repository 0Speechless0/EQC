using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EQC.EDMXModel;
namespace EQC.ViewModel
{
    public class EngReportExcelVM
    {
        public EngReportList _row;
        static EQC_NEW_Entities _dbContext = new EQC_NEW_Entities();

        public EngReportExcelVM()
        {

        }
        public string CreoleDetail
        {
            get
            {
                if(_row.Coastal != null)
                {
                    return _row.Coastal;
                }
                if (DrainName != null)
                {
                    return DrainName;
                }
                else
                {
                    return RiverName;
                }
            }
        }
        public string DrainName
        {
            get
            {
                return _dbContext.DrainList.Find(_row.DrainSeq)?.Name;
            }
        }
        public int ReportTypeSeq
        {
            get
            {
                return _row.ProposalReviewTypeSeq ?? 0;
            }
        }
        public int ReportAttrSeq
        {
            get
            {
                return _row.ProposalReviewAttributesSeq ?? 0;
            }
        }
        public string ReportType
        {
            get
            {
                return _dbContext.ProposalReviewType.Find(ReportTypeSeq)?.TypeName;
            }
        }
        public string ReportAttr
        {
            get
            {
                return _dbContext.ProposalReviewAttributes.Find(ReportAttrSeq)?.AttributesName;
            }

        }

        public string Coord
        {
            get
            {
                return $"{_row.CoordX}, {_row.CoordY}";
            }
        }
        private int[] _YearCost;
        public int[] YearCost
        {
            get
            {
                if(_YearCost == null)
                {
                    _YearCost = new int[3];
                    _row
                        .EngReportEstimatedCost
                        .Where(row => row.Year < _row.RptYear + 3)
                        .GroupBy(row => row.Year)
                        .OrderBy(row => row.Key)
                        .Select(row => row.Sum(r => r.Price ?? 0))
                        .ToArray().CopyTo(_YearCost, 0);
                }


                return _YearCost;
            }
        }
        public int YearCostTotal
        {
            get
            {
                return YearCost.Sum();
            }
        }

        public string WorkMemo
        {
            get
            {
                var work = _row.EngReportMainJobDescription
                    .Where(row => row.RptJobDescriptionSeq == 9)
                    .FirstOrDefault();
                if(work !=null)
                    return work.OtherJobDescription + work.Num.ToString();
                return "";
            }
        }

        static Dictionary<int, int[]> ReportJobSeqDic = new Dictionary<int, int[]>()
                {
                    {1, new int[]{1, 2 } },
                    {2, new int[]{1, 2 } },
                    {3, new int[]{3, 4 } },
                    {4, new int[]{5, 6 } },
                    {5, new int[]{1, 2 } },
                    {6, new int[]{1, 2, 7, 8} },
        };

        private decimal[] _WorkNumArr;
        public decimal[] WorkNumArr
        {
            get
            {
                if(_WorkNumArr == null)
                {
                    _WorkNumArr = new decimal[4];


                    _row.EngReportMainJobDescription
                       .Where(row => ReportJobSeqDic[ReportTypeSeq].Contains(row.RptJobDescriptionSeq ?? 0))
                       .OrderBy(row => row.RptJobDescriptionSeq)
                       .Select(row => row.Num ?? 0)
                       .Take(4)
                       .ToArray().CopyTo(_WorkNumArr, 0);
                }


                return _WorkNumArr;
            }
        }
        public string CityName
        {
            get
            {
                return _dbContext.City.Find(_row.CitySeq)?.CityName;
            }
        }

        public string TownName
        {
            get
            {
                return _dbContext.Town.Find(_row.TownSeq)?.TownName;
            }
        }
        public string RiverName
        {
            get
            {
                return _dbContext.RiverList.Find(_row.RiverSeq)?.Name;
            }
        }

        public string EngName
        {
            get
            {
                return _row.RptName;
            }
        }
        public string ExecUnitName
        {
            get
            {
                if(_row.ExecUnitSeq >= 23 && _row.ExecUnitSeq <=32)
                {
                    switch(_row.ExecUnitSeq)
                    {
                        case 23: return "一";
                        case 24: return "二";
                        case 25: return "三";
                        case 26: return "四";
                        case 27: return "五";
                        case 28: return "六";
                        case 29: return "七";
                        case 30: return "八";
                        case 31: return "九";
                        case 32: return "十";
                    }
                    return "";
                }
                else
                {
                    return _dbContext.Unit.Find(_row.ExecUnitSeq)?.Name;
                }
            }
        }

    }
}