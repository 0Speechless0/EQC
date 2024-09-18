using System;

namespace EQC.ViewModel
{
    public class CarbonEmissionCTVModel
    {
        //水利署碳排管制表
        public string execUnitName { get; set; }
        
        public DateTime? AwardDate { get; set; }
        public string EngNo { get; set; }

        public int EngYear { get; set; }
        public int ExecUnitSeq { get; set; }
        public string EngName { get; set; }
        public string TenderNo { get; set; }
        public string TenderName { get; set; }
        public int awardStatus { get; set; }
        public int createEng { get; set; }
        public int pccesXML { get; set; }
        public decimal detachableRate { get; set; }
        public int engMaterialDevice { get; set; }
        public int detachableRateCnt { get; set; }
        public int engMaterialDeviceCount { get; set; }
        public int engMaterialDeviceSummaryCount { get; set; }
        public int supDaily { get; set; }
        public int checkRec { get; set; }



        public string IsBuild
        {
            get
            {
                if(createEng > 0)
                {
                    return "已完成";
                }
                return "未完成";
            }
        }
        public string _CECheckResult;
        public string CECheckResult
        {
            get
            {
                if(_CECheckResult == "未開工"  || _CECheckResult is null)
                {
                    return "未開工";
                }
                return "已完成";
            }
            set
            {
                _CECheckResult = value;
            }
        }

        private  string _ReductionResult;
        public string ReductionResult
        {
            get
            {
                if (_ReductionResult == "未開工" || _ReductionResult is null)
                {
                    return "未開工";
                }
                return "已完成";
            }
            set
            {
                _ReductionResult = value;
            }
        }

        private string _MetarialAddrResult;
        public string MetarialAddrResult
        {
            get
            {
                if (_MetarialAddrResult is null || _MetarialAddrResult == "未開工")
                {
                    return "未開工";
                }
                return "已完成";
            }
            set
            {
                _MetarialAddrResult = value;
            }
        }
        private string _MachineLoadingResult;
        public string MachineLoadingResult
        {
            get
            {
                if (_MachineLoadingResult == "未開工" || _MachineLoadingResult is null)
                {
                    return "0";
                }
                return _MachineLoadingResult;
            }
            set
            {
                _MachineLoadingResult = value;
            }
        }

        private string _EnergySavingCarbonResult;
        public string EnergySavingCarbonResult
        {
            get
            {
                return _EnergySavingCarbonResult;
            }
            set
            {
                _EnergySavingCarbonResult = value;
            }
        }

    }
}