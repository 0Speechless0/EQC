using EQC.Common;
using System;

namespace EQC.ViewModel
{
    public class EHI_EngList2VModel : EHI_EngListBModel
    {//工程清單
        public int Seq { get; set; }
        public int? PrjXMLSeq { get; set; }
        public string BuildContractorName { get; set; }
        public string BuildContractorTaxId { get; set; }
        public decimal? TotalBudget { get; set; }
        public string Location { get; set; }
        public string ActualCompletionDate { get; set; }
        public string completionDate {
            get
            {
                return Utils.ChsDateFormat(ActualCompletionDate);
            }
        }
        public string ActualAacceptanceCompletionDate { get; set; }
        public string acceptanceCompletionDate
        {
            get
            {
                return Utils.ChsDateFormat(ActualAacceptanceCompletionDate);
            }
        }


    }
}