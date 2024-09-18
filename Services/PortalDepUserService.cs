using EQC.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class PortalDepUserService : BaseService
    {//各局資訊儀表板
        //長官需要知道的事
        public List<T> GetImportantEventSta<T>(string execName = null)
        {
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
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
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
                    left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                    where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                    and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and ( a.FundingSourceName is null
                    or (a.FundingSourceName is not null     and (
                        a.ExecUnitName like '%河川局' and not exists (
                            select value from string_split(a.FundingSourceName, ',')
                            where value Like  '%' +a.ExecUnitName +'%'      
                        )
                    ) ))
                    and ISNULL(pt.ExcludeControlCode & 4, 0 ) = 0
            
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
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                    where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                    and a.FundingSourceName is not null
                    and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
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
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                and (
                    a.CoordX is null or a.CoordY is null
                )
                and ISNULL(pt.ExcludeControlCode & 8, 0 ) = 0
                union all
                select 4 mode, '4. 已屆預定開工日期，實際開工日期未填' level, count(a.Seq) engCount
                from PrjXML a
                inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                and (
                    ISNULL(a.ScheStartDate,'')<>''
                    and CONVERT(date, GetDate())>dbo.ChtDate2Date(a.ScheStartDate)
                    and (a.ActualStartDate is null or a.ActualStartDate='')
                )
                and ISNULL(pt.ExcludeControlCode & 16, 0 ) = 0       
                union all
                select 5 mode, '5. 執行進度100%，未填實際完工日期' level, count(a.Seq) engCount
                from PrjXML a
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                and ISNULL(b.ActualCompletionDate,'')=''
                and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                and (
                    ISNULL(b.ActualProgress,0)=100
                )
                and ISNULL(pt.ExcludeControlCode & 32, 0 ) = 0     
                union all
                select 6 mode, '6. 執行進度100%，狀態欄位卻填「施工中」' level, count(a.Seq) engCount
                from PrjXML a
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                and ISNULL(b.ActualCompletionDate,'')=''
                and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                and (
                    ISNULL(b.ActualProgress,0)=100
                    and b.Status='施工中'
                )
                and ISNULL(pt.ExcludeControlCode & 64, 0 ) = 0             
                /*union all
                select 7 mode, '7.  請至署內水利工程計畫透明網-生態檢核專區公開生態檢核(設計階段)資料' level, count(a.Seq) engCount
                from PrjXML a
                inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                inner join EngMain c1 on(c1.PrjXMLSeq=a.Seq)
                left outer join EcologicalChecklist c2 on(c2.EngMainSeq=c1.Seq and c2.Stage=1)
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                and (
                    ISNULL(a.ActualBidAwardDate,'')<>''
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
                )
                */
                union all
                select 8 mode, '7. 請至標管系統填寫「公共工程生態檢核自評表」' level, count(a.Seq) engCount
                from PrjXML a
                inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                and ISNULL(a.ActualBidAwardDate, '')<>''
                and b.EcologicalCheck='N'
                and ISNULL(pt.ExcludeControlCode & 256, 0 ) = 0     
                union all
                select 9 mode, '8.品管人員即將到期，尚未回訓' level, count(a.Seq) engCount
                from PrjXML a
                inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                inner join EngMain c on(c.PrjXMLSeq=a.Seq and DATEDIFF(day, GETDATE(), c.SupervisorCommPerson4LicenseExpires) <= 90 )
                where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                and ISNULL(a.ActualBidAwardDate, '')<>''
                and ISNULL(pt.ExcludeControlCode & 512, 0 ) = 0     
                union all
                select 10 mode, '9.職安人員即將到期，尚未回訓' level, count(a.Seq) engCount
                from PrjXML a
                inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
                inner join EngMain c on(c.PrjXMLSeq=a.Seq and DATEDIFF(day, GETDATE(), c.SupervisorCommPerson3LicenseExpires) <= 90 )
                where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                and ISNULL(a.ActualBidAwardDate, '')<>''
                and ISNULL(pt.ExcludeControlCode & 1024, 0 ) = 0     
                union all
                select 11 mode, '10.標案保固期限即將到期，請處理保固金發還作業' level, count(a.Seq) engCount
                from PrjXML a
                inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = a.Seq  )
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
                inner join PrjXML p on (a.PrjXMLSeq = p.Seq and p.TenderYear>106)
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = p.Seq  )
                inner join EcologicalChecklist ec on ec.EngMainSeq = a.Seq
                where ec.Stage = 1  and (
                    ec.SelfEvalFilename is null or
                    ec.PlanDesignRecordFilename is null or
                    ec.ConservMeasFilename is null
                )
                and ec.ToDoChecklit <= 2
                and p.ExecUnitName=@ExecUnitName
                and ISNULL(pt.ExcludeControlCode & 4096, 0 ) = 0     
                union all
                select 13 mode, '12.施工進度達10%，未上傳(施工階段)生態檢核相關文件，均會出現提醒，尚未上傳', count(a.Seq) engCount
                from EngMain a 
                inner join @tmp_PrjXML a1 ON(a1.Seq=a.PrjXMLSeq)
                inner join PrjXML p on (a.PrjXMLSeq = p.Seq and p.TenderYear>106)
                left join PrjXMLTag pt on  ( pt.PrjXMLSeq = p.Seq  )
                inner join EcologicalChecklist ec on ec.EngMainSeq = a.Seq
                CROSS APPLY dbo.fPrjXMLProgress(p.Seq) d --20230517
                where ec.Stage = 2  
                and d.PDAccuActualProgress >= 10 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                and (
                    ec.SelfEvalFilename is null or
                    ec.PlanDesignRecordFilename is null or
                    ec.ConservMeasFilename is null
                )
                and exists(
                    select ec2.ToDoChecklit from EcologicalChecklist ec2
                    where ec2.ToDoChecklit <=2 and ec2.Stage = 1 and ec2.EngMainSeq = a.Seq
                )
                and p.ExecUnitName=@ExecUnitName
                and ISNULL(pt.ExcludeControlCode & 8192, 0 ) = 0     
                ";
            string execUnitName = execName ?? Utils.GetUserUnitName();
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ExecUnitName", execUnitName);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //事件清單
        public List<T> GetImportantEventList<T>(int mode)
        {
            string prjSql = "";
            if (mode == 1)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                    where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                    and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and (
                        b.BelongPrj is null
                        or b.BelongPrj=''
                        or b.BelongPrj not in (select ProjectName from wraControlPlanNo)
                    )
                    ";
            }
            else if (mode == 2)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                    where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                    and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and ( a.FundingSourceName is null
                    or (a.FundingSourceName is not null     and (
                        a.ExecUnitName like '%河川局' and not exists (
                            select value from string_split(a.FundingSourceName, ',')
                            where value Like  '%' +a.ExecUnitName +'%'      
                        )
                    )))
            
                    union

                    select a.Seq from PrjXML a
                    inner join Country2WRAMapping c on(
                        substring(a.ExecUnitName,1,3)= c.Country 
                        or substring( replace(a.ExecUnitName, '台', '臺') ,1,3)=c.Country
                    )
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq  )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                    where a.TenderYear>106 and a.FundingSourceName is not null
                    and a.ExecUnitName=@ExecUnitName
                    and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and not exists(
                        select value from string_split(a.FundingSourceName, ',')
                        where value like '%'+c.RiverBureau+'%'
                    )  
  
                    ";
            }
            else if (mode == 3)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                    where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                    and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and (
                        a.CoordX is null or a.CoordY is null
                    )
                    ";
            }
            else if (mode == 4)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                    where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                        and ISNULL(b.ActualCompletionDate,'')=''
                        and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                        and (
                        ISNULL(a.ScheStartDate,'')<>''
                        and CONVERT(date, GetDate())>dbo.ChtDate2Date(a.ScheStartDate)
                        and (a.ActualStartDate is null or a.ActualStartDate='')
                    )
                    ";
            }
            else if (mode == 5)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                    where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                    and ISNULL(b.ActualCompletionDate,'')=''
                    and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and (
                        ISNULL(b.ActualProgress,0)=100
                    )
                    ";
            }
            else if (mode == 6)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                    where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                    and ISNULL(b.ActualCompletionDate,'')=''
                    and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and (
                        ISNULL(b.ActualProgress,0)=100
                        and b.Status='施工中'
                    )
                    ";
            }
            else if (mode == 7)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    inner join EngMain c1 on(c1.PrjXMLSeq=a.Seq)
                    left outer join EcologicalChecklist c2 on(c2.EngMainSeq=c1.Seq and c2.Stage=1)
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                    where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                    and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and (
                        ISNULL(a.ActualBidAwardDate,'')<>''
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
                    )
                    ";
            }
            else if (mode == 8)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    inner join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                    where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                    and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and ISNULL(a.ActualBidAwardDate, '')<>''
                    and b.EcologicalCheck='N'
                    ";
            }
            else if (mode == 9)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    inner join EngMain c on(c.PrjXMLSeq=a.Seq and DATEDIFF(day, GETDATE(), c.SupervisorCommPerson4LicenseExpires) <= 90 )
                    where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                    and ISNULL(a.ActualBidAwardDate, '')<>''
                    ";
            }
            else if (mode == 10)
            {
                prjSql = @"
                    select a.Seq from PrjXML a
                    inner join EngMain c on(c.PrjXMLSeq=a.Seq and DATEDIFF(day, GETDATE(), c.SupervisorCommPerson3LicenseExpires) <= 90 )
                    where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                    and ISNULL(a.ActualBidAwardDate, '')<>''
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
                    where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                    and ISNULL(a.ActualBidAwardDate, '')<>''
                    ";
            }
            else if(mode == 12)
            {
                prjSql = @"
                    select a.PrjXMLSeq  --20230705
                    from EngMain a 
                    inner join PrjXML p on (a.PrjXMLSeq = p.Seq and p.TenderYear>106)
                    inner join EcologicalChecklist ec on ec.EngMainSeq = a.Seq
                    where ec.Stage = 1 and (
                        ec.SelfEvalFilename is null or
                        ec.PlanDesignRecordFilename is null or
                        ec.ConservMeasFilename is null
                    )
                    and p.ExecUnitName=@ExecUnitName
                    and ec.ToDoChecklit <= 2

                ";
            }
            else if (mode == 13)
            {
                prjSql = @"
                    select a.PrjXMLSeq  --20230705
                    from EngMain a 
                    inner join PrjXML p on (a.PrjXMLSeq = p.Seq and p.TenderYear>106)
                    inner join EcologicalChecklist ec on ec.EngMainSeq = a.Seq
					CROSS APPLY dbo.fPrjXMLProgress(p.Seq) d --20230517
                    where ec.Stage = 2  
                    and d.PDAccuActualProgress >= 10 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and (
                        ec.SelfEvalFilename is null or
                        ec.PlanDesignRecordFilename is null or
                        ec.ConservMeasFilename is null
                    )

                    and exists(
                        select ec2.ToDoChecklit from EcologicalChecklist ec2
                        where ec2.ToDoChecklit <=2 and ec2.Stage = 1 and ec2.EngMainSeq = a.Seq
                    )
                    and p.ExecUnitName=@ExecUnitName

                ";
            }
            string prjXMLFilter = "";
            if (mode != 5 && mode != 6) prjXMLFilter = "inner join @tmp_PrjXML a1 ON(a1.Seq=a.Seq)";// 20230709
            string sql = @"
                DECLARE @tmp_PrjXML table (Seq INT) --20230709

                INSERT INTO @tmp_PrjXML(Seq)
                select a.Seq from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230510
                where a.TenderYear>106 and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                order by a.Seq;

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
                " + prjXMLFilter + @"
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                left outer join EngMain c on(c.PrjXMLSeq=a.Seq )
                left join PrjXMLTag pt on (pt.PrjXMLSeq = a.Seq )
                where a.Seq in (
                " + prjSql + @"
                ) and ISNULL(pt.ExcludeControlCode & @Mode, 0 ) = 0";
            string execUnitName =  Utils.GetUserUnitName();
            int _mode = (int)Math.Pow(2, mode);
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Mode", _mode);
            cmd.Parameters.AddWithValue("@ExecUnitName", execUnitName);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //工程清單 狀態
        public List<T> GetEngStateList<T>(int mode)
        {
            string stateSql = "";
            /*if(mode==1)
            {
                stateSql = @"
                    select a.Seq from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                    where a.ExecUnitName=@ExecUnitName
                    and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(a.ActualBidAwardDate,'')=''
                    ";
            } else*/ if (mode == 2)
            {
                stateSql = @"
                    select a.Seq from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                    where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                    and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and ISNULL(a.ActualBidAwardDate,'')<>''
                    and ISNULL(a.ActualStartDate,'')=''
                    ";
            } else if (mode == 3)
            {
                stateSql = @"
                    select a.Seq from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                    where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                    and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                    and ISNULL(a.ActualStartDate,'')<>''
                    ";
            } else if (mode == 4)
            {
                stateSql = @"
                    select a.Seq from PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                    where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                    and ISNULL(b.ActualCompletionDate,'')<>''
                    ";
            }
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
                    a.ScheCompletionDate
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                where a.Seq in (
                " + stateSql+ @"
                )
                ";
            if (mode == 1)
            {
                sql = @"select
                    a.Seq,
                    a1.EngNo TenderNo,
                    a1.EngName TenderName,
                    b1.Name ExecUnitName,
                    a.OutsourcingBudget,
                    ISNULL(b.DesignChangeContractAmount, a.BidAmount) BidAmount,
                    a.QualityControlFee,
                    a.ActualStartDate,
                    a.ScheCompletionDate
                from EngMain a1
                inner join Unit b1 on(b1.Seq = a1.ExecUnitSeq and b1.Name = @ExecUnitName)
                left outer join PrjXML a on(a.Seq=a1.PrjXMLSeq)
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                where ISNULL(a1.AwardDate,'')= ''
                ";
            }
            string execUnitName =   Utils.GetUserUnitName();
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ExecUnitName", execUnitName);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //工程狀態件數統計
        public List<T> GetEngStateSta<T>()
        {
            string sql = @"
                /* shioulo 20221216
                select 1 mode, '發包前' level, count(a.Seq) engCount
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                where a.ExecUnitName=@ExecUnitName
                and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(a.ActualBidAwardDate,'')=''*/
                select 1 mode, '發包前' level, count(a.Seq) engCount
                from EngMain a
                inner join Unit b on(b.Seq=a.ExecUnitSeq and b.Name=@ExecUnitName)
                where ISNULL(a.AwardDate,'')=''
        
                union all
                select 2 mode, '施工前' level, count(a.Seq) engCount
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                and ISNULL(a.ActualBidAwardDate,'')<>''
                and ISNULL(a.ActualStartDate,'')=''
        
                union all
                select 3 mode, '施工中' level, count(a.Seq) engCount
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq )
                CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                and ISNULL(b.ActualCompletionDate,'')=''
                and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                and ISNULL(a.ActualStartDate,'')<>''
        
                union all
                select 4 mode, '結案' level, count(a.Seq) engCount
                from PrjXML a
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                and ISNULL(b.ActualCompletionDate,'')<>''
                ";
            string execUnitName = Utils.GetUserUnitName();
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ExecUnitName", execUnitName);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //工程清單
        public int GetEngCreatedListCount(string state, string engKeyword)
        {
            string sql = @"
                SELECT
                    count(z.Seq) total
                from (
                    SELECT
                        a.Seq,
                        a.TenderName,
                        NULLIF(d.PDExecState, '') ExecState -- 執行狀態
                    FROM PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                    where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                    and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                ) z
                where (@ExecState='' or z.ExecState=@ExecState)
                and (@TenderName='' or z.TenderName like @TenderName)
                ";
            string execUnitName = Utils.GetUserUnitName();
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ExecUnitName", execUnitName);
            cmd.Parameters.AddWithValue("@ExecState", state);
            cmd.Parameters.AddWithValue("@TenderName", engKeyword);

            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }

        public List<T> GetEngCreatedList<T>(int pageRecordCount, int pageIndex, string state, string engKeyword)
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
                        NULLIF(d.PDExecState, '') ExecState -- 執行狀態
                    FROM PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                    where a.TenderYear>106 and a.ExecUnitName=@ExecUnitName
                    and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                ) z
                where (@ExecState='' or z.ExecState=@ExecState)
                and (@TenderName='' or z.TenderName like @TenderName)
                order by z.TenderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            string execUnitName = Utils.GetUserUnitName();
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@ExecUnitName", execUnitName);
            cmd.Parameters.AddWithValue("@ExecState", state);
            cmd.Parameters.AddWithValue("@TenderName", engKeyword);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
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
                        DISTINCT NULLIF(d.PDExecState, '') ExecState -- 執行狀態
                    FROM PrjXML a
                    left outer join PrjXMLExt b on(b.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230517
                    where a.ExecUnitName=@ExecUnitName
                    and ISNULL(b.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'
                ) z
                ";
            string execUnitName = Utils.GetUserUnitName();
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@ExecUnitName", execUnitName);
            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}