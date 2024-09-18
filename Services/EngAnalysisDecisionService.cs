using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EngAnalysisDecisionService : BaseService
    {//工程資訊彙整分析及決策資訊儀表板
        //其他補助&縣市政府 各單位在建工程件數統計
        public List<T> GetEngCntForGovUnit<T>(int m)
        {
            string unitSql = "";
            if(m == 1)
            {
                unitSql = @"
                    select c1.Seq OrderNo, c1.Country ExecUnitName, 1 constructionCount, 0 behindCount
                    from PrjXML a
                    inner join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230505
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
        
                    union all   
         
                    select c1.Seq OrderNo, c1.Country ExecUnitName, 0 constructionCount, 1 behindCount
                    from PrjXML a
                    inner join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230505
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and ISNULL(d.PDAccuScheProgress,0) > ISNULL(d.PDAccuActualProgress,0)
                ";
            } else if (m == 2)
            {
                unitSql = @"
                    select 0 OrderNo, a.ExecUnitName, 1 constructionCount, 0 behindCount
                    from PrjXML a
                    left outer join Unit c on(c.ParentSeq is null and c.Name=a.ExecUnitName)
                    left outer join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230505
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and c1.Country is null
                    and c.Name is null
        
                    union all
         
                    select 0 OrderNo, a.ExecUnitName, 0 constructionCount, 1 behindCount
                    from PrjXML a
                    left outer join Unit c on(c.ParentSeq is null and c.Name=a.ExecUnitName)
                    left outer join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230505
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and ISNULL(d.PDAccuScheProgress,0) > ISNULL(d.PDAccuActualProgress,0)
                    and c1.Country is null
                    and c.Name is null
                ";
            }
            string sql = @"
                select z.OrderNo, z.ExecUnitName, sum(z.constructionCount) constructionCount, sum(z.behindCount) behindCount
                from ("
                + unitSql +
                @"
                ) z
                group by z.OrderNo, z.ExecUnitName
                --order by z.OrderNo    
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //其他補助&縣市政府 施工中清單
        public List<T> GetConstructionEngGovList<T>(int m, string unit)
        {
            string sql = @"
                select distinct
                    a.Seq
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230505
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                ";

            string sql2 = @"
                select 
                    a.Seq,
                    a.TenderNo,
                    a.TenderName,
                    a.OutsourcingBudget,
                    ISNULL(b.DesignChangeContractAmount, a.BidAmount) BidAmount,
                    a.QualityControlFee,
                    a.ActualStartDate,
                    a.ScheCompletionDate,
                    a.ExecUnitName,
                    c.Seq EngSeq
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                left outer join EngMain c on(c.PrjXMLSeq=a.Seq ) where a.Seq in (" + sql+")";
            if (m == 1)
            {
                sql2 += @"
                        and substring(a.ExecUnitName,1,3)=@unit
                        order by a.ExecUnitName";
            }
            else if (m == 2)
            {
                sql2 += @"
                        and a.ExecUnitName=@unit
                        order by a.ExecUnitName";
            }
            else
            {
                sql2 += @" and (1=0)";
            }
            SqlCommand cmd = db.GetCommand(sql2);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@unit", unit);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //其他補助&縣市政府 落後清單
        public List<T> GetBehindEngGovList<T>(int m, string unit)
        {
            string sql = @"

                select distinct
                    a.Seq
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230505
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                and ISNULL(d.PDAccuScheProgress,0) > ISNULL(d.PDAccuActualProgress,0)
                ";
            string sql2 = @"

                select
                    a.Seq,
                    a.TenderNo,
                    a.TenderName,
                    a.OutsourcingBudget,
                    ISNULL(b.DesignChangeContractAmount, a.BidAmount) BidAmount,
                    a.QualityControlFee,
                    a.ActualStartDate,
                    a.ScheCompletionDate,
                    a.ExecUnitName,
                    c.Seq EngSeq
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                left outer join EngMain c on(c.PrjXMLSeq=a.Seq ) where a.Seq in (" + sql+")";
            if (m == 1)
            {
                sql2 += @"
                        and substring(a.ExecUnitName,1,3)=@unit
                        order by a.ExecUnitName";
            }
            else if (m == 2)
            {
                sql2 += @"
                        and a.ExecUnitName=@unit
                        order by a.ExecUnitName";
            }
            else
            {
                sql2 += @" and (1=0)";
            }
            SqlCommand cmd = db.GetCommand(sql2);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@unit", unit);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //長官需要知道的事
        public List<T> GetImportantEventSta<T>()
        {
            /*string sql = @"
                select 1 mode, '1. 標案歸屬計畫名稱未填報或填報錯誤' level, count(a.Seq) engCount
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and (
                    b.BelongPrj is null
                    or b.BelongPrj=''
                    or b.BelongPrj not in (select ProjectName from wraControlPlanNo)
                )
        
                union all
                select 2 mode, '2. 預算來源機關填報錯誤' level, sum(z.cnt) engCount
                from(
                    select count(a.Seq) cnt
                    from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where a.FundingSourceName is null
                    and a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    or (a.FundingSourceName is not null 
                        and (
                            a.ExecUnitName like '%河川局' and not exists (
                                select value from string_split(a.FundingSourceName, ',')
                                where value Like  '%' +a.ExecUnitName +'%'      
                            )
                        )
                    )
            
                    union all
            
                    select count(a.Seq) cnt
                    from PrjXML a
                    inner join Country2WRAMapping c on(
                        substring(a.ExecUnitName,1,3)=c.Country
			            or substring( replace(a.ExecUnitName, '台', '臺') ,1,3)=c.Country
                    )
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq  )
                    --where a.ExecUnitName like '%政府'
                    where a.FundingSourceName is not null
                    and a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and not exists(
                        select value from string_split(a.FundingSourceName, ',')
                        where value Like  '%' + c.RiverBureau + '%'
                    )         
                ) z
        
                union all
                select 3 mode, '3. 標案未填XY座標' level, count(a.Seq) engCount
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and (a.CoordX is null or a.CoordY is null)
        
                union all
                select 4 mode, '4. 已屆預定開工日期，實際開工日期未填' level, count(a.Seq) engCount
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                where ISNULL(a.ScheStartDate,'')<>''
                and a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and CONVERT(date, GetDate())>dbo.ChtDate2Date(a.ScheStartDate)
                and (a.ActualStartDate is null or a.ActualStartDate='')
        
                union all
                select 5 mode, '5. 執行進度100%，未填實際完工日期' level, count(a.Seq) engCount
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL((select PDAccuActualProgress from dbo.fPrjXMLProgress(a.Seq)), 0) = 100
        
                union all
                select 6 mode, '6. 執行進度100%，狀態欄位卻填「施工中」' level, count(a.Seq) engCount
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL((select PDAccuActualProgress from dbo.fPrjXMLProgress(a.Seq)), 0) = 100
                and b.Status='施工中'
        
                union all
                select 7 mode, '7. 請至署內水利工程計畫透明網-生態檢核專區公開生態檢核(設計階段)資料' level, count(a.Seq) engCount
                from PrjXML a
                inner join EngMain c1 on(c1.PrjXMLSeq=a.Seq)
                left outer join EcologicalChecklist c2 on(c2.EngMainSeq=c1.Seq and c2.Stage=1)
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                where ISNULL(a.ActualBidAwardDate,'')<>''
                and a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and (
                    c2.Seq is null or(
                        c2.Seq is not null
                        AND(
                            c2.ToDoChecklit<=2 and (
                                ISNULL(c2.SelfEvalFilename,'')='' or ISNULL(c2.PlanDesignRecordFilename,'')='' or ISNULL(c2.MemberDocFilename,'')=''
                                or ISNULL(c2.DataCollectDocFilename,'')='' or ISNULL(c2.ConservMeasFilename,'')='' or ISNULL(c2.SOCFilename,'')=''
                            )
                            or (c2.ToDoChecklit>2 and ISNULL(c2.ChecklistFilename,'')='')
                        )
                    )
                )
        
                union all
                select 8 mode, '8. 請至標管系統填寫「公共工程生態檢核自評表」' level, count(a.Seq) engCount
                from PrjXML a
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                where ISNULL(a.ActualBidAwardDate, '')<>''
                and a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and b.EcologicalCheck='N'

                union all
                select 9 mode, '9.品管人員即將到期，尚未回訓' level, count(a.Seq) engCount
                from PrjXML a
                inner join EngMain c on(c.PrjXMLSeq=a.Seq and DATEDIFF(day, GETDATE(), c.SupervisorCommPerson4LicenseExpires) <= 90 )
                where a.TenderYear>106 and ISNULL(a.ActualBidAwardDate, '')<>''

                union all
                select 10 mode, '10.職安人員即將到期，尚未回訓' level, count(a.Seq) engCount
                from PrjXML a
                inner join EngMain c on(c.PrjXMLSeq=a.Seq and DATEDIFF(day, GETDATE(), c.SupervisorCommPerson3LicenseExpires) <= 90 )
                where a.TenderYear>106 and ISNULL(a.ActualBidAwardDate, '')<>''

                union all
                select 11 mode, '11.標案保固期限即將到期，請處理保固金發還作業' level, count(a.Seq) engCount
                from PrjXML a
                inner join EngMain c on(c.PrjXMLSeq=a.Seq
                	and DATEDIFF(day, GETDATE(), c.WarrantyExpires) > 0
                	and DATEDIFF(day, GETDATE(), c.WarrantyExpires) <= 30
                )
                where a.TenderYear>106 and ISNULL(a.ActualBidAwardDate, '')<>''

                union all
                select 12 mode, '12.設計階段未上傳生態檢核相關文件，均會出現提醒，尚未上傳', count(a.Seq) engCount
                from EngMain a 
                inner join EcologicalChecklist ec on ec.EngMainSeq = a.Seq
                where ec.Stage = 2  and (
                    ec.SelfEvalFilename is null or
                    ec.PlanDesignRecordFilename is null or
                    ec.ConservMeasFilename is null
                )
                and ec.ToDoChecklit <= 2

                union all
                select 13 mode, '13.施工進度達10%，未上傳(施工階段)生態檢核相關文件，均會出現提醒，尚未上傳', count(a.Seq) engCount
                from EngMain a 
                inner join EcologicalChecklist ec on ec.EngMainSeq = a.Seq
                where ec.Stage = 2  
                and ec.ToDoChecklit <= 2
                and ISNULL((select PDAccuActualProgress from dbo.fPrjXMLProgress(a.PrjXMLSeq)), 0) >= 20
                and (
                    ec.SelfEvalFilename is null or
                    ec.PlanDesignRecordFilename is null or
                    ec.ConservMeasFilename is null
                )
                ";*/
            //s20230510
            string sql = @"
                DECLARE @tmp_PrjXML table (Seq INT)

                INSERT INTO @tmp_PrjXML(Seq)
                select a.Seq from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230510
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                order by a.Seq


                select 1 mode, '1. 標案歸屬計畫名稱未填報或填報錯誤' level, count(a.Seq) engCount
                from PrjXML a
                inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                where a.TenderYear>106
                and (
                    b.BelongPrj is null
                    or b.BelongPrj=''
                    or b.BelongPrj not in (select ProjectName from wraControlPlanNo)
                )
                and ISNULL(pt.ExcludeControlCode & 2, 0 ) = 0
                union all
                select 2 mode, '2. 預算來源機關填報錯誤' level, sum(z.cnt) engCount
                from(
                    select count(a.Seq) cnt
                    from PrjXML a
                    inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                    where a.FundingSourceName is null
                    and ISNULL(pt.ExcludeControlCode & 4, 0 ) = 0
                    and a.TenderYear>106
                    or (a.FundingSourceName is not null 
                        and (
                            a.ExecUnitName like '%河川局' and not exists (
                                select value from string_split(a.FundingSourceName, ',')
                                where value Like  '%' +a.ExecUnitName +'%'      
                            )
                        )
                    )
            
                    union all
            
                    select count(a.Seq) cnt
                    from PrjXML a
                    inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                    left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                    inner join Country2WRAMapping c on(
                        substring(a.ExecUnitName,1,3)=c.Country
                        or substring( replace(a.ExecUnitName, '台', '臺') ,1,3)=c.Country
                    )
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq  )
                    --where a.ExecUnitName like '%政府'
                    where a.FundingSourceName is not null
                    and a.TenderYear>106
                    and not exists(
                        select value from string_split(a.FundingSourceName, ',')
                        where value Like  '%' + c.RiverBureau + '%'
                    )
                    and ISNULL(pt.ExcludeControlCode & 4, 0 ) = 0
                ) z
        
                union all
                select 3 mode, '3. 標案未填XY座標' level, count(a.Seq) engCount
                from PrjXML a
                inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                where a.TenderYear>106
                and ISNULL(pt.ExcludeControlCode & 8, 0 ) = 0
                and (a.CoordX is null or a.CoordY is null)
        
                union all
                select 4 mode, '4. 已屆預定開工日期，實際開工日期未填' level, count(a.Seq) engCount
                from PrjXML a
                inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                where ISNULL(a.ScheStartDate,'')<>''
                and a.TenderYear>106
                and CONVERT(date, GetDate())>dbo.ChtDate2Date(a.ScheStartDate)
                and (a.ActualStartDate is null or a.ActualStartDate='')
                and ISNULL(pt.ExcludeControlCode & 16, 0 ) = 0
                union all
                select 5 mode, '5. 執行進度100%，未填實際完工日期' level, count(a.Seq) engCount
                from PrjXML a
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL((select PDAccuActualProgress from dbo.fPrjXMLProgress(a.Seq)), 0) = 100
                and ISNULL(pt.ExcludeControlCode & 32, 0 ) = 0       
                union all
                select 6 mode, '6. 執行進度100%，狀態欄位卻填「施工中」' level, count(a.Seq) engCount
                from PrjXML a
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL((select PDAccuActualProgress from dbo.fPrjXMLProgress(a.Seq)), 0) = 100
                and b.Status='施工中'
                and ISNULL(pt.ExcludeControlCode & 64, 0 ) = 0
        
                /*union all
                select 7 mode, '7. 請至署內水利工程計畫透明網-生態檢核專區公開生態檢核(設計階段)資料' level, count(a.Seq) engCount
                from PrjXML a
                inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                inner join EngMain c1 on(c1.PrjXMLSeq=a.Seq)
                left outer join EcologicalChecklist c2 on(c2.EngMainSeq=c1.Seq and c2.Stage=1)
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                where ISNULL(a.ActualBidAwardDate,'')<>''
                and a.TenderYear>106
                and (
                    c2.Seq is null or(
                        c2.Seq is not null
                        AND(
                            c2.ToDoChecklit<=2 and (
                                ISNULL(c2.SelfEvalFilename,'')='' or ISNULL(c2.PlanDesignRecordFilename,'')='' or ISNULL(c2.MemberDocFilename,'')=''
                                or ISNULL(c2.DataCollectDocFilename,'')='' or ISNULL(c2.ConservMeasFilename,'')='' or ISNULL(c2.SOCFilename,'')=''
                            )
                            or (c2.ToDoChecklit>2 and ISNULL(c2.ChecklistFilename,'')='')
                        )
                    )
                ) */
        
                union all
                select 8 mode, '7. 請至標管系統填寫「公共工程生態檢核自評表」' level, count(a.Seq) engCount
                from PrjXML a
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                where ISNULL(a.ActualBidAwardDate, '')<>''
                and a.TenderYear>106
                and b.EcologicalCheck='N'
                and ISNULL(pt.ExcludeControlCode & 128, 0 ) = 0
                union all
                select 9 mode, '8.品管人員即將到期，尚未回訓' level, count(a.Seq) engCount
                from PrjXML a
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                inner join EngMain c on(c.PrjXMLSeq=a.Seq and DATEDIFF(day, GETDATE(), c.SupervisorCommPerson4LicenseExpires) <= 90 )
                where a.TenderYear>106
                and ISNULL(pt.ExcludeControlCode & 512, 0 ) = 0
                union all
                select 10 mode, '9.職安人員即將到期，尚未回訓' level, count(a.Seq) engCount
                from PrjXML a
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                inner join EngMain c on(c.PrjXMLSeq=a.Seq and DATEDIFF(day, GETDATE(), c.SupervisorCommPerson3LicenseExpires) <= 90 )
                where a.TenderYear>106
                and ISNULL(pt.ExcludeControlCode & 1024, 0 ) = 0
                union all
                select 11 mode, '10.標案保固期限即將到期，請處理保固金發還作業' level, count(a.Seq) engCount
                from PrjXML a
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                inner join EngMain c on(c.PrjXMLSeq=a.Seq
                    and DATEDIFF(day, GETDATE(), c.WarrantyExpires) > 0
                    and DATEDIFF(day, GETDATE(), c.WarrantyExpires) <= 30
                )
                where a.TenderYear>106
                and ISNULL(pt.ExcludeControlCode & 2048, 0 ) = 0
                union all
                select 12 mode, '11.設計階段未上傳生態檢核相關文件，均會出現提醒，尚未上傳', count(a.Seq) engCount
                from EngMain a 
                inner join @tmp_PrjXML a1 ON(a1.Seq=a.PrjXMLSeq)
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a1.Seq  )
                inner join EcologicalChecklist ec on ec.EngMainSeq = a.Seq
                where ec.Stage = 1  and (
                    ec.SelfEvalFilename is null or
                    ec.PlanDesignRecordFilename is null or
                    ec.ConservMeasFilename is null
                )
                and ec.ToDoChecklit <= 2
                and ISNULL(pt.ExcludeControlCode & 4096, 0 ) = 0
                union all
                select 13 mode, '12.施工進度達10%，未上傳(施工階段)生態檢核相關文件，均會出現提醒，尚未上傳', count(a.Seq) engCount
                from EngMain a 
                inner join @tmp_PrjXML a1 ON(a1.Seq=a.PrjXMLSeq)
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a1.Seq  )
                inner join EcologicalChecklist ec on ec.EngMainSeq = a.Seq
                where ec.Stage = 2  
                and exists(
                    select ec2.ToDoChecklit from EcologicalChecklist ec2
                    where ec2.ToDoChecklit <=2 and ec2.Stage = 1 and ec2.EngMainSeq = a.Seq
                )
                and ISNULL((select PDAccuActualProgress from dbo.fPrjXMLProgress(a.PrjXMLSeq)), 0) >= 10
                and (
                    ec.SelfEvalFilename is null or
                    ec.PlanDesignRecordFilename is null or
                    ec.ConservMeasFilename is null
                )
                and ISNULL(pt.ExcludeControlCode & 8192, 0 ) = 0;
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            return db.GetDataTableWithClass<T>(cmd);
        }
        //事件清單
        public List<T> GetImportantEventList<T>(int mode)
        {
            int _mode = (int)Math.Pow(2, mode); 
            string prjSql = "";
            if(mode==1)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and (
                        b.BelongPrj is null
                        or b.BelongPrj=''
                        or b.BelongPrj not in (select ProjectName from wraControlPlanNo)
                    )
                    ";
            } else if (mode == 2)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where a.FundingSourceName is null
                    and a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    or (a.FundingSourceName is not null     and (
                        a.ExecUnitName like '%河川局' and not exists (
                            select value from string_split(a.FundingSourceName, ',')
                            where value Like  '%' +a.ExecUnitName +'%'      
                        )
                    ))
            
                    union all
            
                    select a.Seq
                    from PrjXML a
                    inner join Country2WRAMapping c on(
                        substring(a.ExecUnitName,1,3)=c.Country
			            or substring( replace(a.ExecUnitName, '台', '臺') ,1,3)=c.Country
                    )
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq  )
                    where a.FundingSourceName is not null
                    and a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and not exists(
                        select value from string_split(a.FundingSourceName, ',')
                        where value Like  '%' + c.RiverBureau + '%'
                    ) 
                    ";
            }
            else if (mode == 3)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and  (a.CoordX is null or a.CoordY is null) 
                    ";
            }
            else if (mode == 4)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where ISNULL(a.ScheStartDate,'')<>''
                    and a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and CONVERT(date, GetDate())>dbo.ChtDate2Date(a.ScheStartDate)
                    and (a.ActualStartDate is null or a.ActualStartDate='')
                    ";
            }
            else if (mode == 5)
            {
                prjSql += @"
                    select a.Seq from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL((select PDAccuActualProgress from dbo.fPrjXMLProgress(a.Seq)), 0) = 100
                    ";
            }
            else if (mode == 6)
            {
                prjSql += @"
                    select a.Seq from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL((select PDAccuActualProgress from dbo.fPrjXMLProgress(a.Seq)), 0) = 100
                    and b.Status='施工中'
                    ";
            }
            else if (mode == 7)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    inner join EngMain c1 on(c1.PrjXMLSeq=a.Seq)
                    left outer join EcologicalChecklist c2 on(c2.EngMainSeq=c1.Seq and c2.Stage=1)
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where ISNULL(a.ActualBidAwardDate,'')<>''
                    and a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and (
                        c2.Seq is null or(
                            c2.Seq is not null
                            AND(
                                c2.ToDoChecklit<=2 and (
                                    ISNULL(c2.SelfEvalFilename,'')='' or ISNULL(c2.PlanDesignRecordFilename,'')='' or ISNULL(c2.MemberDocFilename,'')=''
                                    or ISNULL(c2.DataCollectDocFilename,'')='' or ISNULL(c2.ConservMeasFilename,'')='' or ISNULL(c2.SOCFilename,'')=''
                                )
                                or (c2.ToDoChecklit>2 and ISNULL(c2.ChecklistFilename,'')='')
                            )
                        )
                    )
                    ";
            }
            else if (mode == 8)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where ISNULL(a.ActualBidAwardDate, '')<>''
                    and a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and b.EcologicalCheck='N'
                    ";
            }
            else if (mode == 9)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    inner join EngMain c on(c.PrjXMLSeq=a.Seq and DATEDIFF(day, GETDATE(), c.SupervisorCommPerson4LicenseExpires) <= 90 )
                    where a.TenderYear>106 and ISNULL(a.ActualBidAwardDate, '')<>''
                    ";
            }
            else if (mode == 10)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    inner join EngMain c on(c.PrjXMLSeq=a.Seq and DATEDIFF(day, GETDATE(), c.SupervisorCommPerson3LicenseExpires) <= 90 )
                    where a.TenderYear>106 and ISNULL(a.ActualBidAwardDate, '')<>''
                    ";
            }
            else if (mode == 11)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    inner join EngMain c on(c.PrjXMLSeq=a.Seq
                	    and DATEDIFF(day, GETDATE(), c.WarrantyExpires) > 0
                	    and DATEDIFF(day, GETDATE(), c.WarrantyExpires) <= 30
                    )
                    where a.TenderYear>106 and ISNULL(a.ActualBidAwardDate, '')<>''
                    ";
            }
            else if (mode == 12)
            {
                prjSql = @"
                    select a.PrjXMLSeq  --20230705
                    from EngMain a 
                    inner join EcologicalChecklist ec on ec.EngMainSeq = a.Seq
                    where ec.Stage = 1  and (
                        ec.SelfEvalFilename is null or
                        ec.PlanDesignRecordFilename is null or
                        ec.ConservMeasFilename is null
                    )
                    and ec.ToDoChecklit <= 2 --20230709

                ";
            }
            else if (mode == 13)
            {
                prjSql += @"
                    select a.PrjXMLSeq  --20230705
                    from EngMain a 
                    inner join EcologicalChecklist ec on ec.EngMainSeq = a.Seq
                    where ec.Stage = 2  
                    and exists(
                        select ec2.ToDoChecklit from EcologicalChecklist ec2
                        where ec2.ToDoChecklit <=2 and ec2.Stage = 1 and ec2.EngMainSeq = a.Seq
                    )
                    and ISNULL((select PDAccuActualProgress from dbo.fPrjXMLProgress(a.PrjXMLSeq)), 0) >= 10
                    and (
                        ec.SelfEvalFilename is null or
                        ec.PlanDesignRecordFilename is null or
                        ec.ConservMeasFilename is null
                    )

                ";
            }
            string prjXMLFilter = "";
            if(mode != 5 && mode != 6) prjXMLFilter = "inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)";// 20230709
            string sql = @"
                DECLARE @tmp_PrjXML table (Seq INT) --20230709

                INSERT INTO @tmp_PrjXML(Seq)
                select a.Seq from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230510
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                order by a.Seq

                select
                    a.Seq,
                    a.TenderNo,
                    a.TenderName,
                    a.ExecUnitName,
                    a.OutsourcingBudget,
                    ISNULL(b.DesignChangeContractAmount, a.BidAmount) BidAmount,
                    a.QualityControlFee,
                    a.ActualStartDate,
                    a.ScheCompletionDate,
                    Cast( case when pt.ExcludeControlCode & @Mode  > 0 then 1 else 0 end  as bit ) as exclude,
                    c.Seq EngSeq
                from PrjXML a
                " + prjXMLFilter + @"
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                left outer join EngMain c on(c.PrjXMLSeq=a.Seq )
                left join PrjXMLTag pt on (pt.PrjXMLSeq = a.Seq )
                where a.Seq in (
                " + prjSql + @"
                )";
            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Mode", _mode);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //經費等級工程清單
        //  水利署 & 所屬機關
        public List<T> GetEngFeeLevelList<T>(int feeLevel)
        {
            string sql = @"
                select
                    a.Seq,
                    a.TenderNo,
                    a.TenderName,
                    a.ExecUnitName,
                    a.OutsourcingBudget,
                    ISNULL(b.DesignChangeContractAmount, a.BidAmount) BidAmount,
                    a.QualityControlFee,
                    a.ActualStartDate,
                    a.ScheCompletionDate,
                    c.Seq EngSeq
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                left outer join EngMain c on(c.PrjXMLSeq=a.Seq )
                where a.Seq in(
                    select za.Seq from PrjXML za
                    inner join Unit zc on(zc.ParentSeq is null and zc.Name=a.ExecUnitName)
                    left outer join PrjXMLExt zb on(zb.PrjXMLSeq=za.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(za.Seq) d --20230510
                    where za.TenderYear>106 and ISNULL(zb.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                )
                and (
                    (@feeLevel=1 and ISNULL(b.DesignChangeContractAmount, a.BidAmount) < 10000)
                    or (@feeLevel=2 and ISNULL(b.DesignChangeContractAmount, a.BidAmount) >= 10000 and ISNULL(b.DesignChangeContractAmount, a.BidAmount) < 50000)
                    or (@feeLevel=3 and ISNULL(b.DesignChangeContractAmount, a.BidAmount) >= 50000 and ISNULL(b.DesignChangeContractAmount, a.BidAmount) < 200000)
                    or (@feeLevel=4 and ISNULL(b.DesignChangeContractAmount, a.BidAmount) >= 200000)
                )
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@feeLevel", feeLevel);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //  縣市政府 & 其它補助
        public List<T> GetEngFeeLevelOtherList<T>(int feeLevel)
        {
            string sql = @"
                select
                    a.Seq,
                    a.TenderNo,
                    a.TenderName,
                    a.OutsourcingBudget,
                    ISNULL(b.DesignChangeContractAmount, a.BidAmount) BidAmount,
                    a.QualityControlFee,
                    a.ActualStartDate,
                    a.ScheCompletionDate,
                    a.ExecUnitName,
                    c.Seq EngSeq
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                left outer join EngMain c on(c.PrjXMLSeq=a.Seq )
                where a.Seq in(
                    select a.Seq from PrjXML a
                    inner join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230510
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    union all
                    select a.Seq from PrjXML a
                    left outer join Unit c on(c.ParentSeq is null and c.Name=a.ExecUnitName)
                    left outer join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230510
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and c1.Country is null
                    and c.Name is null
                )
                and (
                    (@feeLevel=1 and ISNULL(b.DesignChangeContractAmount, a.BidAmount) < 10000)
                    or (@feeLevel=2 and ISNULL(b.DesignChangeContractAmount, a.BidAmount) >= 10000 and ISNULL(b.DesignChangeContractAmount, a.BidAmount) < 50000)
                    or (@feeLevel=3 and ISNULL(b.DesignChangeContractAmount, a.BidAmount) >= 50000 and ISNULL(b.DesignChangeContractAmount, a.BidAmount) < 200000)
                    or (@feeLevel=4 and ISNULL(b.DesignChangeContractAmount, a.BidAmount) >= 200000)
                )
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@feeLevel", feeLevel);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //經費等級工程件數統計
        //水利署 & 所屬機關
        public List<T> GetEngFeeLevelSta<T>()
        {
            //b.DesignChangeContractAmount = null, a.BidAmount = null 未納入統計
            string sql = @"
                select z.mode, z.cnt engCount
                from (
                    select 1 mode, count(a.Seq) cnt
                    from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where a.Seq in(
                        select za.Seq from PrjXML za
                        inner join Unit zc on(zc.ParentSeq is null and zc.Name=a.ExecUnitName)
                        left outer join PrjXMLExt zb on(zb.PrjXMLSeq=za.Seq )
                        CROSS APPLY dbo.fPrjXMLProgress(za.Seq) d --20230510
                        where za.TenderYear>106 and ISNULL(zb.ActualCompletionDate,'')=''
                        and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    )
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) < 10000

                    union all

                    select 2 mode, count(a.Seq) cnt
                    from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where a.Seq in(
                        select za.Seq from PrjXML za
                        inner join Unit zc on(zc.ParentSeq is null and zc.Name=a.ExecUnitName)
                        left outer join PrjXMLExt zb on(zb.PrjXMLSeq=za.Seq )
                        CROSS APPLY dbo.fPrjXMLProgress(za.Seq) d --20230510
                        where za.TenderYear>106 and ISNULL(zb.ActualCompletionDate,'')=''
                        and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    )
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) >= 10000
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) < 50000
        
                    union all
        
                    select 3 mode, count(a.Seq) cnt
                    from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where a.Seq in(
                        select za.Seq from PrjXML za
                        inner join Unit zc on(zc.ParentSeq is null and zc.Name=a.ExecUnitName)
                        left outer join PrjXMLExt zb on(zb.PrjXMLSeq=za.Seq )
                        CROSS APPLY dbo.fPrjXMLProgress(za.Seq) d --20230510
                        where za.TenderYear>106 and ISNULL(zb.ActualCompletionDate,'')=''
                        and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    )
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) >= 50000
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) < 200000
        
                    union all

                    select 4 mode, count(a.Seq) cnt
                    from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where a.Seq in(
                        select za.Seq from PrjXML za
                        inner join Unit zc on(zc.ParentSeq is null and zc.Name=a.ExecUnitName)
                        left outer join PrjXMLExt zb on(zb.PrjXMLSeq=za.Seq )
                        CROSS APPLY dbo.fPrjXMLProgress(za.Seq) d --20230510
                        where za.TenderYear>106 and ISNULL(zb.ActualCompletionDate,'')=''
                        and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    )
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) >= 200000
                ) z
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //縣市政府 & 其它補助
        public List<T> GetEngFeeLevelOtherSta<T>()
        {
            //b.DesignChangeContractAmount = null, a.BidAmount = null 未納入統計
            /*string sql = @"
                select z.mode, z.cnt engCount
                from (
                    select 1 mode, count(a.Seq) cnt
                    from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where a.Seq in(
                        select a.Seq from PrjXML a
                        inner join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                        left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                        CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230510
                        where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                        and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                        union all
                        select a.Seq from PrjXML a
                        left outer join Unit c on(c.ParentSeq is null and c.Name=a.ExecUnitName)
                        left outer join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                        left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                        CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230510
                        where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                        and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                        and c1.Country is null
                        and c.Name is null
                    )
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) < 10000

                    union all

                    select 2 mode, count(a.Seq) cnt
                    from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where a.Seq in(
                        select a.Seq from PrjXML a
                        inner join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                        left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                        CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230510
                        where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                        and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                        union all
                        select a.Seq from PrjXML a
                        left outer join Unit c on(c.ParentSeq is null and c.Name=a.ExecUnitName)
                        left outer join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                        left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                        CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230510
                        where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                        and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                        and c1.Country is null
                        and c.Name is null
                    )
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) >= 10000
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) < 50000
        
                    union all
        
                    select 3 mode, count(a.Seq) cnt
                    from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where a.Seq in(
                        select a.Seq from PrjXML a
                        inner join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                        left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                        CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230510
                        where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                        and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                        union all
                        select a.Seq from PrjXML a
                        left outer join Unit c on(c.ParentSeq is null and c.Name=a.ExecUnitName)
                        left outer join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                        left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                        CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230510
                        where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                        and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                        and c1.Country is null
                        and c.Name is null
                    )
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) >= 50000
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) < 200000
        
                    union all

                    select 4 mode, count(a.Seq) cnt
                    from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where a.Seq in(
                        select a.Seq from PrjXML a
                        inner join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                        left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                        CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230510
                        where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                        and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                        union all
                        select a.Seq from PrjXML a
                        left outer join Unit c on(c.ParentSeq is null and c.Name=a.ExecUnitName)
                        left outer join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                        left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                        CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230510
                        where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                        and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                        and c1.Country is null
                        and c.Name is null
                    )
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) >= 200000
                ) z
                ";*/
            //s20230510 性能調校
            string sql = @"
                DECLARE @tmp_PrjXML table (Seq INT)

                INSERT INTO @tmp_PrjXML(Seq)
                select z.Seq from (
	                select a.Seq from PrjXML a
                    inner join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230510
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    union all
                    select a.Seq from PrjXML a
                    left outer join Unit c on(c.ParentSeq is null and c.Name=a.ExecUnitName)
                    left outer join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230510
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and c1.Country is null
                    and c.Name is null
                ) z
                order by z.Seq

                select z.mode, z.cnt engCount
                from (
                    select 1 mode, count(a.Seq) cnt
                    from PrjXML a
                    inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where ISNULL(b.DesignChangeContractAmount, a.BidAmount) < 10000

                    union all

                    select 2 mode, count(a.Seq) cnt
                    from PrjXML a
                    inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where ISNULL(b.DesignChangeContractAmount, a.BidAmount) >= 10000
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) < 50000

                    union all

                    select 3 mode, count(a.Seq) cnt
                    from PrjXML a
                    inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where ISNULL(b.DesignChangeContractAmount, a.BidAmount) >= 50000
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) < 200000

                    union all

                    select 4 mode, count(a.Seq) cnt
                    from PrjXML a
                    inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where ISNULL(b.DesignChangeContractAmount, a.BidAmount) >= 200000
                ) z
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetEngFeeLevelSta0<T>()
        {
            //b.DesignChangeContractAmount = null, a.BidAmount = null 未納入統計
            string sql = @"
                select z.mode, z.cnt engCount
                from (
                    select 1 mode, count(a.Seq) cnt
                    from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) < 10000

                    union all

                    select 2 mode, count(a.Seq) cnt
                    from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) >= 10000
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) < 50000
        
                    union all
        
                    select 3 mode, count(a.Seq) cnt
                    from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) >= 50000
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) < 200000
        
                    union all

                    select 4 mode, count(a.Seq) cnt
                    from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(b.DesignChangeContractAmount, a.BidAmount) >= 200000
                ) z
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //各單位在建工程件數統計
        public List<T> GetEngCntForUnit<T>()
        {
            /*string sql = @"
	            select z.OrderNo, z.ExecUnitName, sum(z.constructionCount) constructionCount, sum(z.behindCount) behindCount
                from (
                    select c.OrderNo, a.ExecUnitName, 1 constructionCount, 0 behindCount
                    from PrjXML a
                    inner join Unit c on(c.ParentSeq is null and c.Name=a.ExecUnitName )
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230505
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
        
                    union all
        
                    select c.OrderNo, a.ExecUnitName, 0 constructionCount, 1 behindCount
                    from PrjXML a
                    inner join Unit c on(c.ParentSeq is null and c.Name=a.ExecUnitName)
                    left outer join PrjXMLExt p on(p.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230505
                    where a.TenderYear>106 and ISNULL(p.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and ISNULL(d.PDAccuScheProgress,0) > ISNULL(d.PDAccuActualProgress,0)
         
                    union all
         
                    select 1000 OrderNo, '縣市政府' ExecUnitName, 1 constructionCount, 0 behindCount
                    from PrjXML a
                    inner join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230505
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
        
                    union all
         
                    select 1000 OrderNo, '縣市政府' ExecUnitName, 0 constructionCount, 1 behindCount
                    from PrjXML a
                    inner join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                    left outer join PrjXMLExt p on(p.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230505
                    where a.TenderYear>106 and ISNULL(p.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and ISNULL(d.PDAccuScheProgress,0) > ISNULL(d.PDAccuActualProgress,0)
         
                    union all
         
                    select 2000 OrderNo, '其他補助' ExecUnitName, 1 constructionCount, 0 behindCount
                    --select 2000 OrderNo, a.ExecUnitName, 1 constructionCount, 0 behindCount
                    from PrjXML a
                    left outer join Unit c on(c.ParentSeq is null and c.Name=a.ExecUnitName)
                    left outer join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230505
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and c1.Country is null
                    and c.Name is null
        
                    union all
         
                    select 2000 OrderNo, '其他補助' ExecUnitName, 0 constructionCount, 1 behindCount
                    --select 2000 OrderNo, a.ExecUnitName, 1 constructionCount, 0 behindCount
                    from PrjXML a
                    left outer join Unit c on(c.ParentSeq is null and c.Name=a.ExecUnitName)
                    left outer join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                    left outer join PrjXMLExt p on(p.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230505
                    where a.TenderYear>106 and ISNULL(p.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and ISNULL(d.PDAccuScheProgress,0) > ISNULL(d.PDAccuActualProgress,0)
                    and c1.Country is null
                    and c.Name is null
        
	            ) z
                group by z.OrderNo, z.ExecUnitName
                order by z.OrderNo
                ";*/
            string sql = @"
                DECLARE @tmp_PrjXML table (Seq INT, behind bit)

                INSERT INTO @tmp_PrjXML(Seq, behind)
                select a.Seq, IIF(ISNULL(d.PDAccuScheProgress,0) > ISNULL(d.PDAccuActualProgress,0),1,0) behind from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230510
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                order by a.Seq


                select z.OrderNo, z.ExecUnitName, sum(z.constructionCount) constructionCount, sum(z.behindCount) behindCount
                from (
                    select c.OrderNo, a.ExecUnitName, 1 constructionCount, 0 behindCount
                    from PrjXML a
                    inner join @tmp_PrjXML a1 on(a1.Seq=a.Seq)
                    inner join Unit c on(c.ParentSeq is null and c.Name=a.ExecUnitName )
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
        
                    union all
        
                    select c.OrderNo, a.ExecUnitName, 0 constructionCount, 1 behindCount
                    from PrjXML a
                    inner join @tmp_PrjXML a1 on(a1.Seq=a.Seq and a1.behind=1)
                    inner join Unit c on(c.ParentSeq is null and c.Name=a.ExecUnitName)
                    left outer join PrjXMLExt p on(p.PrjXMLSeq=a.Seq )
         
                    union all
         
                    select 1000 OrderNo, '縣市政府' ExecUnitName, 1 constructionCount, 0 behindCount
                    from PrjXML a
                    inner join @tmp_PrjXML a1 on(a1.Seq=a.Seq)
                    inner join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
        
                    union all
         
                    select 1000 OrderNo, '縣市政府' ExecUnitName, 0 constructionCount, 1 behindCount
                    from PrjXML a
                    inner join @tmp_PrjXML a1 on(a1.Seq=a.Seq and a1.behind=1)
                    inner join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                    left outer join PrjXMLExt p on(p.PrjXMLSeq=a.Seq )
         
                    union all
         
                    select 2000 OrderNo, '其他補助' ExecUnitName, 1 constructionCount, 0 behindCount
                    from PrjXML a
                    inner join @tmp_PrjXML a1 on(a1.Seq=a.Seq)
                    left outer join Unit c on(c.ParentSeq is null and c.Name=a.ExecUnitName)
                    left outer join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    where c1.Country is null
                    and c.Name is null
        
                    union all
         
                    select 2000 OrderNo, '其他補助' ExecUnitName, 0 constructionCount, 1 behindCount
                    from PrjXML a
                    inner join @tmp_PrjXML a1 on(a1.Seq=a.Seq and a1.behind=1)
                    left outer join Unit c on(c.ParentSeq is null and c.Name=a.ExecUnitName)
                    left outer join Country2WRAMapping c1 on(c1.Country=substring(a.ExecUnitName,1,3))
                    left outer join PrjXMLExt p on(p.PrjXMLSeq=a.Seq )
                    where c1.Country is null
                    and c.Name is null
        
                ) z
                group by z.OrderNo, z.ExecUnitName
                order by z.OrderNo
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //施工中清單
        public List<T> GetConstructionEngList<T>(string unit)
        {
            string sql = @"
                select distinct
                    a.Seq
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230505
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                ";

            string sql2 = @"
                select distinct
                    a.Seq,
                    a.TenderNo,
                    a.TenderName,
                    a.OutsourcingBudget,
                    ISNULL(b.DesignChangeContractAmount, a.BidAmount) BidAmount,
                    a.QualityControlFee,
                    a.ActualStartDate,
                    a.ScheCompletionDate,
                    a.ExecUnitName,
                    c.Seq EngSeq
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                left outer join EngMain c on(c.PrjXMLSeq=a.Seq )
                where a.Seq in (" + sql+" ) ";
            if (unit == "縣市政府")
            {
                sql2 += @" and substring(a.ExecUnitName,1,3) in (select Country from Country2WRAMapping)
                        order by a.ExecUnitName";
            }
            else if (unit == "其他補助")
            {
                sql2 += @" and a.ExecUnitName not in (select name from unit where ParentSeq is null)
                          and substring(a.ExecUnitName,1,3) not in (select Country from Country2WRAMapping)
                        order by a.ExecUnitName";
            }
            else
            {
                sql2 += @" and a.ExecUnitName = @unit
                        order by a.TenderNo";
            }
            SqlCommand cmd = db.GetCommand(sql2);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@unit", unit);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //落後清單
        public List<T> GetBehindEngList<T>(string unit)
        {
            string sql = @"
                select distinct
                    a.Seq
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230505
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                and ISNULL(d.PDAccuScheProgress,0) > ISNULL(d.PDAccuActualProgress,0)
                ";
            string sql2 = @"
                select
                    a.Seq,
                    a.TenderNo,
                    a.TenderName,
                    a.OutsourcingBudget,
                    ISNULL(b.DesignChangeContractAmount, a.BidAmount) BidAmount,
                    a.QualityControlFee,
                    a.ActualStartDate,
                    a.ScheCompletionDate,
                    a.ExecUnitName,
                    c.Seq EngSeq
                from PrjXML a 
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                left outer join EngMain c on(c.PrjXMLSeq=a.Seq )               
                where a.Seq in(" + sql+")";

            if (unit == "縣市政府")
            {
                sql2 += @" and substring(a.ExecUnitName,1,3) in (select Country from Country2WRAMapping)
                        order by a.ExecUnitName";
            }
            else if (unit == "其他補助")
            {
                sql2 += @" and a.ExecUnitName not in (select name from unit where ParentSeq is null)
                          and substring(a.ExecUnitName,1,3) not in (select Country from Country2WRAMapping)
                        order by a.ExecUnitName";
            }
            else
            {
                sql2 += @" and a.ExecUnitName = @unit
                        order by a.TenderNo";
            }
            SqlCommand cmd = db.GetCommand(sql2);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@unit", unit);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //工程清單
        public int GetEngCreatedListCount(string state, string unitKeyword, string engKeyword)
        {
            string sql = @"
                SELECT
                    count(z.Seq) total
                from (
                    SELECT
                        a.Seq,
                        --a.TenderNo,
                        a.TenderName,
                        a.ExecUnitName,
                        --a.DesignUnitName,
                        --a.SupervisionUnitName,
                        NULLIF((select PDExecState from dbo.fPrjXMLProgress(a.Seq)), '') ExecState -- 執行狀態
                    FROM PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230505
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                ) z
                where (@ExecState='' or z.ExecState=@ExecState)
                and (@ExecUnitName='' or z.ExecUnitName like @ExecUnitName)
                and (@TenderName='' or z.TenderName like @TenderName)
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ExecState", state);
            cmd.Parameters.AddWithValue("@ExecUnitName", unitKeyword);
            cmd.Parameters.AddWithValue("@TenderName", engKeyword);

            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetEngCreatedList<T>(int pageRecordCount, int pageIndex, string state, string unitKeyword, string engKeyword)
        {
            string sql = @"
                select
                    z.Seq,
                    z.TenderNo,
                    z.TenderName,
                    z.ExecUnitName,
                    z.DesignUnitName,
                    z.SupervisionUnitName,
                    z.ExecState
                from (
                    SELECT
                        a.Seq,
                        a.TenderNo,
                        a.TenderName,
                        a.ExecUnitName,
                        a.DesignUnitName,
                        a.SupervisionUnitName,
                        NULLIF((select PDExecState from dbo.fPrjXMLProgress(a.Seq)), '') ExecState -- 執行狀態
                    FROM PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230505
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                ) z
                where (@ExecState='' or z.ExecState=@ExecState)
                and (@ExecUnitName='' or z.ExecUnitName like @ExecUnitName)
                and (@TenderName='' or z.TenderName like @TenderName)
                order by z.TenderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            cmd.Parameters.AddWithValue("@ExecState", state);
            cmd.Parameters.AddWithValue("@ExecUnitName", unitKeyword);
            cmd.Parameters.AddWithValue("@TenderName", engKeyword);
            //cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }
        
        //工程狀態清單
        public List<T> GetEngCreatedListState<T>()
        {
            string sql = @"
                select z.ExecState Text, z.ExecState Value
                from (
                    SELECT
                        DISTINCT ISNULL(
                            (select PDExecState from dbo.fPrjXMLProgress(a.Seq)), '') ExecState -- 執行狀態
                    FROM PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230505
                    where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                ) z
                ";
            SqlCommand cmd = db.GetCommand(sql);
            //cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}