using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EQC.EDMXModel;
using EQC.Models;

namespace EQC.Services
{
    public class TenderCalFormService 
    {

        EQC_NEW_Entities dbContext;
        CarbonEmissionPayItemService CarbonEmissionPayItemService;
        EMDAuditService emdAuditService;
        Dictionary<string, EngMain> EngNoDic;
        public TenderCalFormService(EQC_NEW_Entities _dbContext= null)
        {
            if(_dbContext == null)
            {
                dbContext = new EQC_NEW_Entities();
            }
            else
            {
                dbContext = _dbContext;
            }

            CarbonEmissionPayItemService = new CarbonEmissionPayItemService();
            emdAuditService = new EMDAuditService();
            EngNoDic = dbContext.EngMain.ToDictionary(r => r.EngNo, r => r);
        }


        ~TenderCalFormService()
        {
            dbContext.Dispose();
        }
        public int GetMetrailSummaryNum(string engNo, int total)
        {
            int engSeq = EngNoDic[engNo].Seq;
            if (total > 0)
                return
                    emdAuditService.GetEMDSummaryList<EngMaterialDeviceSummaryVModel>(engSeq, 1, total)
                    .Where(r => r.ModifyTime != r.CreateTime)
                    .Count();
            else return 0;
        }
        
        public int GetMeterailSummaryTotal(string engNo)
        {
            int engSeq = EngNoDic[engNo].Seq;
            return emdAuditService.GetEMDSummaryListCount(engSeq);
        }
        public int GetMetrailTestNum(string engNo, int total)
        {
            int engSeq = EngNoDic[engNo].Seq;
            if (total > 0)
                return
                    emdAuditService.GetEMDTestSummaryList<EngMaterialDeviceTestSummaryVModel>(engSeq, 1, total)
                        .Where(r => r.ModifyTime != r.CreateTime)
                        .Count();
            else return 0;
        }

        public int DismantlingRate(string engNo, decimal co2ItemTotal)
        {
            decimal? SubContractingBudget = EngNoDic[engNo].SubContractingBudget;
            if(SubContractingBudget is decimal _SubContractingBudget  && _SubContractingBudget > 0 )
            {
                return (int)Math.Round(co2ItemTotal * 100 / _SubContractingBudget, 0);
            }
            return 0;
        }

        public int GetMeterailTestTotal(string engNo)
        {
            int engSeq = EngNoDic[engNo].Seq;
            return emdAuditService.GetEMDTestSummaryListCount(engSeq);
        }

        public int GetCarbonCo2Total(string engNo, ref decimal? Co2Total, ref decimal? Co2ItemTotal)
        {
            int engSeq = EngNoDic[engNo].Seq;
            CarbonEmissionPayItemService.CalCarbonTotal(engSeq, ref  Co2Total,ref Co2ItemTotal);

            return (int)(Co2Total ?? 0);
        }

        public int[] GetReportFillCount(string engNo, int type)
        {
            var eng = EngNoDic[engNo];

            var engStartDate = eng.EngChangeStartDate ?? eng.StartDate ;
            var engEndDate = eng.EngChangeSchCompDate ?? eng.SchCompDate;
            var allDay =
                 eng.SupDailyDate.ToList().Concat(
                     eng.EC_SupDailyDate.Select(r => new SupDailyDate
                     {
                         ItemDate = r.ItemDate,
                         DataType = r.DataType
                     }))
                .Where(r => r.DataType == type && r.ItemDate >= engStartDate && r.ItemDate <= engEndDate)
                .GroupBy(r => r.ItemDate);
            var fillDay = allDay.Where(r => r.Key <= DateTime.Now);

            if (engStartDate is DateTime _engStartDate && engEndDate is DateTime _engEndDate)
            {
                return new int[2]
                {
                    Math.Min( (int)(engEndDate.Value - engStartDate.Value).TotalDays,
                    Math.Max((DateTime.Now -  ( engStartDate ?? DateTime.Now) ).Days, -1) ) +1,
                    fillDay.Count()
                };
            }
            else
            {
                return new int[2]
                { 
                    0,0
                };
            }

        }



        public int GetReportCount(string engNo)
        {
            var eng = EngNoDic[engNo];

            var engStartDate = eng.EngChangeStartDate ?? eng.StartDate;
            var engEndDate = eng.EngChangeSchCompDate ?? eng.SchCompDate;
            if(engStartDate is DateTime _engStartDate && engEndDate is DateTime _engEndDate )
            {
                return (int)(_engEndDate - _engStartDate).TotalDays +1;
            }
            else
            {
                return 0;
            }

        }

        public int GetNeededContCheckNum(string engNo)
        {
            var eng = EngNoDic[engNo];
            return
                dbContext.EquOperTestList.Where(r => r.EngMainSeq == eng.Seq && r.DataKeep).Count() +
                dbContext.ConstCheckList.Where(r => r.EngMainSeq == eng.Seq && r.DataKeep).Count() +
                dbContext.OccuSafeHealthList.Where(r => r.EngMainSeq == eng.Seq && r.DataKeep).Count() +
                dbContext.EnvirConsList.Where(r => r.EngMainSeq == eng.Seq && r.DataKeep).Count();
        }

        public int GetContCheckShouldNum(string engNo)
        {
            var eng = EngNoDic[engNo];
            return dbContext.EngConstruction.Where(r => r.EngMainSeq == eng.Seq)
                .ToList()
                .Aggregate(new List<ConstCheckRec>(), (a, c) =>
                {
                    a.AddRange(c.ConstCheckRec);
                    return a;
                })
                .GroupBy(r => r.CCRCheckType1)
                .Sum(r => r.GroupBy(rr => rr.ItemSeq).Count());
        }



    }
}