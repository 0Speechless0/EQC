using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EQC.EDMXModel;
using EQC.Models;

namespace EQC.Services
{
    public class TenderCalFormService :BaseService
    {

        EQC_NEW_Entities dbContext;
        CarbonEmissionPayItemService CarbonEmissionPayItemService;
        EMDAuditService emdAuditService;
        public static Dictionary<string, EngMain> EngNoDic;
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
                         DataType = r.DataType,
                         CreateTime = r.CreateTime,
                         ModifyTime = r.ModifyTime
                     }))
                .Where(r => r.DataType == type && r.ItemDate >= engStartDate 
                && r.ItemDate <= engEndDate && r.CreateTime != r.ModifyTime)
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

        public List<SelectOptionModel> GetEngCECheckResult(int? startYear = null)
        {
            int _startYear = startYear ?? DateTime.Now.Year - 1911;
            string sql = @"
                select a.EngNo Value, 
                       CAST(cek.Seq as nvarchar(50)) Text
                from EngMain a
                left join CarbonEmissionHeader ch on ch.EngMainSeq = a.Seq
                left join CECheckTable cek on cek.CarbonEmissionHeaderSeq = ch.Seq
                where a.EngYear >= @startYear
                order by a.ExecUnitSeq, a.Seq
            ";
            var cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", _startYear);
            return db.GetDataTableWithClass<SelectOptionModel>(cmd);
        }
        public List<SelectOptionModel> GetEngReductionResult(int? startYear = null)
        {
            int _startYear = startYear ?? DateTime.Now.Year - 1911;
            string sql = @"
       
				       select
						    a.EngNo Value,
                            CAST(dd.Result as nvarchar(50)) Text

                        from EngMain a
                        inner join Unit b on(b.ParentSeq is null and b.Seq = a.ExecUnitSeq)
                        left outer join CarbonReductionCalResult dd on(dd.EngMainSeq=a.Seq)
	                    where a.EngYear >= @startYear
                    order by a.ExecUnitSeq, a.Seq
            ";
            var cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", _startYear);
            return db.GetDataTableWithClass<SelectOptionModel>(cmd);
        }
        public List<SelectOptionModel> GetEngMetarialAddrResult(int? startYear = null)
        {
            int _startYear = startYear ?? DateTime.Now.Year - 1911;
            string sql = @"
       
                select 
	                a.EngNo Value, 
	                em.VendorAddr Text 
                from EngMain a
                inner join Unit b on b.Seq = a.ExecUnitSeq

                --已下資料表用於顯示 工程履約 > 品質查證 > 材料送審 
                left join EngMaterialDeviceList el on el.EngMainSeq  = a.Seq
                left join EngMaterialDeviceSummary em on em.EngMaterialDeviceListSeq = el.Seq
                where a.EngYear >= @startYear and em.VendorAddr is not null
                order by a.ExecUnitSeq, a.Seq
            ";
            var cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", _startYear);
            return db.GetDataTableWithClass<SelectOptionModel>(cmd);
        }

        public List<SelectOptionModel> GetEngEnergySavingCarbonResult(int? startYear = null)
        {
            int _startYear = startYear ?? DateTime.Now.Year - 1911;
            string sql = @"
       
                select 
                    a.EngNo Value, 
                    CAST(   Sum(case when cr.Seq  is null then 0 else 1 end ) as nvarchar(50) ) Text

                from EngMain a
                inner join Unit b on(b.ParentSeq is null and b.Seq = a.ExecUnitSeq)
                --EnvirConsList 代表環境保育清單項目前台資料表(顯示在監造計畫產製那頁) 要關連到 EngConstruction(施工項目主表) 才能關連到EngMain
                --ConstCheckRec 施工抽查主表 要關連到 EnvirConsList ，
                --到工程履約 > 品質查證 > 施工抽查 新增抽查會存於ConstCheckRec
                inner join EnvirConsList evr on (evr.EngMainSeq = a.Seq and evr.ItemName Like '%節能減碳%')
                left join ConstCheckRec cr on  ( evr.Seq = cr.ItemSeq and cr.CCRCheckType1 =4 and evr.Seq = cr.ItemSeq )
                where a.EngYear >= @startYear

                group by a.Seq, a.EngName, a.EngNo ,a.ExecUnitSeq
                order by a.ExecUnitSeq, a.Seq
            ";
            var cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", _startYear);
            return db.GetDataTableWithClass<SelectOptionModel>(cmd);
        }
        public List<SelectOptionModel> GetEngMachineLoadingResult(int? startYear = null)
        {
            int _startYear = startYear ?? DateTime.Now.Year - 1911;
            string sql = @"
       
                select 
	                a1.EngNo Value, 
	             	CAST ( (ISNULL(a1.FillCount, 0)+ ISNULL(a2.FillCount, 0) )  as nvarchar(50) ) Text

                from
                (select 
                Max(z.ExecUnitSeq) ExecUnitSeq,
                Max(z.Seq) Seq,
                Max(z.Name) ExecUnitName,
                Max(z.EngName) EngName,
                Max(z.EngNo) EngNo,
                Sum(z.FillCount ) as FillCount 
                from (

	                select a.EngName, a.EngNo , a.ExecUnitSeq, a.Seq,
	                b.Name,

	                --機具人員材料已填寫天數
	                case when g1.Seq is null then 0 else 1 end  as FillCount 

	                from

	                (
		                select sp.Seq from 

			                --SupDailyDate 是儲存工程施工日誌及監造報表填寫狀況的資料表

			                --以下資料表顯示於 工程履約 > 進度管理 > 施工日誌下方其他頁簽
			                --SupDailyReportConstructionEquipment 機具資料表
			                --SupDailyReportConstructionMaterial 材料資料表
			                --SupDailyReportConstructionPerson 人員資料表
			                SupDailyDate sp

			                --這裡left join 出來的資料筆數並不重要， 只看這些資料中有無 不為null且大於0 的欄位，有則代表已填寫過
			                left join SupDailyReportConstructionEquipment sdr on (sp.Seq = sdr.SupDailyDateSeq ) 
			                left join SupDailyReportConstructionMaterial sdm on (sdm.SupDailyDateSeq = sp.Seq ) 
			                left join SupDailyReportConstructionPerson sdp on (sdp.SupDailyDateSeq = sp.Seq)
                            left join  SupDailyReportMiscConstruction sdd on sdd.SupDailyDateSeq = sp.Seq
			                --若三者其中之一TodayQuantity大於0代表填寫過
			                where   ( 
                                sdr.TodayQuantity > 0 or 
                                sdm.TodayQuantity > 0 or 
                                sdp.TodayQuantity > 0  or
                                sdr.CreateTime != sdr.ModifyTime or
                                sdm.CreateTime != sdm.ModifyTime or
                                sdp.CreateTime != sdp.ModifyTime or
                                sdd.CreateTime != sdd.ModifyTime
                            )  and sp.DataType = 2
			                group by sp.Seq
	                ) g1


                    inner join SupDailyDate sp on (sp.Seq = g1.Seq and sp.CreateTime != sp.ModifyTime)
                    right join EngMain a on a.Seq = sp.EngMainSeq	                
                    inner join Unit b on(b.ParentSeq is null and b.Seq = a.ExecUnitSeq)
                    where a.EngYear >= @startYear 
                        and sp.ItemDate >= a.StartDate
                        and sp.ItemDate <= a.SchCompDate

                ) z
                group by   z.Seq , z.ExecUnitSeq
                ) a1
                left join 

                (
                select 

                Max(z.ExecUnitSeq) ExecUnitSeq,
                Max(z.Seq) Seq,
                Max(z.Name) ExecUnitName,
                Max(z.EngName) EngName,
                Max(z.EngNo) EngNo,
                Sum(z.FillCount ) as FillCount 
                from (


	                select a.EngName, a.EngNo , a.ExecUnitSeq,  a.Seq,
	                b.Name,
	                --機具人員材料已填寫天數
	                case when g1.Seq is null then 0 else 1 end as FillCount 

	                from

	                (
	                select sp.Seq from 

		                --SupDailyDate 是儲存工程施工日誌及監造報表填寫狀況的資料表

		                --以下資料表顯示於 工程履約 > 進度管理 > 施工日誌下方其他頁簽
		                --SupDailyReportConstructionEquipment 機具資料表
		                --SupDailyReportConstructionMaterial 材料資料表
		                --SupDailyReportConstructionPerson 人員資料表
		                EC_SupDailyDate sp

		                --這裡left join 出來的資料筆數並不重要， 只看這些資料中有無 不為null且大於0 的欄位，有則代表已填寫過
		                left join EC_SupDailyReportConstructionEquipment sdr on (sp.Seq = sdr.EC_SupDailyDateSeq ) 
		                left join EC_SupDailyReportConstructionMaterial sdm on (sdm.EC_SupDailyDateSeq = sp.Seq ) 
		                left join EC_SupDailyReportConstructionPerson sdp on (sdp.EC_SupDailyDateSeq = sp.Seq)
                        left join  EC_SupDailyReportMiscConstruction sdd on sdd.EC_SupDailyDateSeq = sp.Seq
		                --若三者其中之一TodayQuantity大於0代表填寫過
		                where ( 
                            sdr.TodayQuantity > 0 or 
                            sdm.TodayQuantity > 0 or 
                            sdp.TodayQuantity > 0  or
                            sdr.CreateTime != sdr.ModifyTime or
                            sdm.CreateTime != sdm.ModifyTime or
                            sdp.CreateTime != sdp.ModifyTime or
                            sdd.CreateTime != sdd.ModifyTime
                        )   
                        and sp.DataType = 2
		                group by sp.Seq
	                ) g1

                    inner join EC_SupDailyDate sp on (sp.Seq = g1.Seq and sp.CreateTime != sp.ModifyTime)
	                right join EngMain a on a.Seq = sp.EngMainSeq
	                inner join Unit b on(b.ParentSeq is null and b.Seq = a.ExecUnitSeq)

                    where a.EngYear >= @startYear
                        and sp.ItemDate >= ISNULL(a.EngChangeStartDate, a.StartDate)
                        and sp.ItemDate <= ISNULL(a.EngChangeSchCompDate, a.SchCompDate)
                ) z
                group by   z.Seq , z.ExecUnitSeq
                ) a2 on a1.EngNo = a2.EngNo


                order by a1.ExecUnitSeq, a1.Seq
            ";
            var cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startYear", _startYear);
            return db.GetDataTableWithClass<SelectOptionModel>(cmd);
        }
    }
}