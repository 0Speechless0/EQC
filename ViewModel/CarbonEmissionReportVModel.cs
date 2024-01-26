using Newtonsoft.Json;
using System.Linq;
using EQC.Models;
using System.Collections.Generic;
using System;
using EQC.ViewModel.Interface;

namespace EQC.ViewModel
{
    public class CarbonEmissionReportVModel : UnitTreeJsonConvertor
    {
        //碳報表
        public string execUnitName { get; set; }
        public string EngName { get; set; }

        public Nullable<short> ExecUnitSeq { get; set; }

        public Nullable<short> ExecSubUnitSeq { get; set; }

        public string awardStatus { get; set; }
        public decimal? AwardTotalBudget { get; set; }
        public decimal? _TotalBudget { get; set; }
        public decimal? TotalBudget { 
            get
            {
                return (_TotalBudget ?? 0) / 1000;
            }
            set {
                _TotalBudget = value;
            } 
        }
        public decimal? Co2TotalItem { get; set; }

        public decimal CarbonReduction { get; set; }

        public int? _CarbonDemandQuantity { get; set; }
        public int? CarbonDemandQuantity {
            get {
                return  (_CarbonDemandQuantity ?? 0 ) / 1000;
            } 
            set {

                _CarbonDemandQuantity = value;
            } 
        }
        public int? _ApprovedCarbonQuantity { get; set; }


        public int? ApprovedCarbonQuantity
        {
            get
            {
                return  (_ApprovedCarbonQuantity ?? 0  ) / 1000;
            }
            set
            {

                _ApprovedCarbonQuantity  = value;
            }
        }

        public int? awardCnt { get; set; }
        public int? engCnt { get; set; }
        public decimal? _Co2Total { get; set; }


        public decimal? Co2Total
        {
            get
            {
                return decimal.Round(((decimal) (_Co2Total ?? 0)) / 1000, 2);
            }
            set
            {

                _Co2Total = value;
            }
        }
        public decimal? GreenFunding { get; set; } 
        public decimal? greenFundingRate { get; set; }
        public string Tree02931 { 
            get {

                return JsonConvert.DeserializeObject<List<CETreeModel>>(Tree02931Json ?? "")?
                .Aggregate("", (a, c) =>  $"{a}{c.T ?? "其他"}*{c.A}\n" );
            } 
        }
        public string Tree02932 { 
            get {
                return JsonConvert.DeserializeObject<List<CETreeModel>>(Tree02932Json ?? "")?
                    .Aggregate("", (a, c) => $"{a}{c.T ?? "其他"}*{c.A}\n");

            } 
        }
        public decimal? Tree02931Total { 
            
            get {
                if (Tree02931JsonList == null) return null;
                return Tree02931JsonList.Sum(e => JsonConvert.DeserializeObject<List<CETreeModel>>(e ?? "") ?
                    .Sum(r => r.A) ?? 0);
            } 
           
        }

        public decimal? Tree02932Total
        {

            get
            {
                if (Tree02931JsonList == null) return null;
                return Tree02932JsonList.Sum(e => JsonConvert.DeserializeObject<List<CETreeModel>>(e ?? "") ?
                    .Sum(r => r.A) ?? 0);
                

            }

        }
        public int? F1108Area { get; set; }
        public int? F1109Length { get; set; }
        public decimal? co2TotalRate { get; set; }

        public string Tree02931Json { get; set; }
        public string Tree02932Json { get; set; }

        public  List<string> Tree02931JsonList { get; set; }
        public  List<string> Tree02932JsonList { get; set; }

        private string _GreenFundingValue;
        public string GreenFundingValue { 
            get
            {
               if( _GreenFundingValue != null)
               {
                    var arr = _GreenFundingValue.Split(',');

                    return $"再生材料:\n{arr[0]}\n減碳:\n{arr[1]}\n營建自動化:\n{arr[2]}\n";
               }
                return "";
            }
            set
            {
                _GreenFundingValue = value;
            }
        }

        public string Remark { get; set; }
        
        public string ReductionStrategy { get; set; }

        public bool DredgingEng { get; set; }

    }
}