using EQC.Common;
using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class SupervisePhaseService : BaseService
    {//督導期別

        //日程表 s20230519
        public List<SuperviseScheduleFormModel> GetScheduleForm(int superviseEngSeq)
        {
            string sql = @"
                SELECT
                    Seq,
                    OrderNo,
                    StartTime,
                    EndTime,
                    ActivityTime,
                    Summary
                FROM SuperviseScheduleForm
                where SuperviseEngSeq=@SuperviseEngSeq
                order by OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);

            return db.GetDataTableWithClass<SuperviseScheduleFormModel>(cmd);
        }
        //日程表初始化 s20230519
        public bool InitScheduleForm(int superviseEngSeq)
        {
            string sql = @"
                insert into SuperviseScheduleForm (
                    SuperviseEngSeq, OrderNo, ActivityTime, Summary, StartTime, EndTime
                ) values (
                    @SuperviseEngSeq, @OrderNo, @ActivityTime, @Summary, '', ''
                )
                ";
            db.BeginTransaction();
            try { 
                
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);
                cmd.Parameters.AddWithValue("@OrderNo", 1);
                cmd.Parameters.AddWithValue("@ActivityTime", 45);
                cmd.Parameters.AddWithValue("@Summary", "領隊及單位主管致詞(5分鐘)\n主辦單位簡報(10分鐘)\n監造單位簡報(10分鐘)\n施工單位簡報(10分鐘)\n簡報答詢(10分鐘)");
                db.ExecuteNonQuery(cmd);

                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);
                cmd.Parameters.AddWithValue("@OrderNo", 2);
                cmd.Parameters.AddWithValue("@ActivityTime", 90);
                cmd.Parameters.AddWithValue("@Summary", "現勘(含來回車程約60分鐘)");
                db.ExecuteNonQuery(cmd);

                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);
                cmd.Parameters.AddWithValue("@OrderNo", 3);
                cmd.Parameters.AddWithValue("@ActivityTime", 30);
                cmd.Parameters.AddWithValue("@Summary", "中午用餐");
                db.ExecuteNonQuery(cmd);

                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);
                cmd.Parameters.AddWithValue("@OrderNo", 4);
                cmd.Parameters.AddWithValue("@ActivityTime", 60);
                cmd.Parameters.AddWithValue("@Summary", "文件審閱");
                db.ExecuteNonQuery(cmd);

                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);
                cmd.Parameters.AddWithValue("@OrderNo", 5);
                cmd.Parameters.AddWithValue("@ActivityTime", 70);
                cmd.Parameters.AddWithValue("@Summary", "綜合檢討及扣點會議");
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return true;
            }
            catch (Exception ex)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.initScheduleForm:" + ex.Message);
                return false;
            }
        }

        internal int SyncSuperviseEngWeakness(int prjSeq, int pharseSeq)
        {
            string sql = @"
                update SuperviseEng
                SET 
	                W1 = vs.W1,
                     W2 = vs.W2,
                     W3 = vs.W3,
                     W4 = vs.W4,
                     W5 = vs.W5,
                     W6 = vs.W6,
                     W7 = vs.W7,
                     W8 = vs.W8,
                     W9 = vs.W9,
                     W10 = vs.W10,
                     W11 = vs.W11,
                     W12 = vs.W12,
                     W13 = vs.W13,
                     W14 = vs.W14

                from SuperviseEng s
                inner join viewPrjXMLPlaneWeakness vs on vs.PrjXMLSeq = s.PrjXMLSeq 
                where s.PrjXMLSeq  = @PrjXMLSeq and
                    s.SupervisePhaseSeq = @SupervisePhaseSeq 


            ";
            var cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@PrjXMLSeq", prjSeq);
            cmd.Parameters.AddWithValue("@SupervisePhaseSeq", pharseSeq);
            return db.ExecuteNonQuery(cmd);
        }

        internal string GetWeaknessDescription(int prjSeq)
        {
            string sql = @"
                select
                    (
                        IIF(e.W1=1,'1重大(或重點防汛)，','')
                        +IIF(e.W2=1,'2進度落後，','')
                        +IIF(e.W3=1,'3決標比偏低，','')
                        +IIF(e.W4=1,'4施工廠商近年查核成績不佳，','')
                        +IIF(e.W5=1,'5曾發生重大職安事件之標案，','')
                        +IIF(e.W6=1,'6履約計分偏低標案，','')
                        +IIF(e.W7=1,'7近三年曾遭停權之施工廠商，','')
                        +IIF(e.W8=1,'8施工廠商近期承攬能量偏高，','')
                        +IIF(e.W9=1,'9施工廠商跨區域承攬，','')
                        +IIF(e.W10=1,'10施工量能偏低，','')
                        +IIF(e.W11=1,'11委外監造之工程，','')
                        +IIF(e.W12=1,'12成績不佳的委外監造廠商，','')
                        +IIF(e.W13=1,'13高敏感區域工程，','')
                        +IIF(e.W14=1,'14全民督工','')
                    )
                from PrjXML a
                left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                where a.Seq=@PrjXMLSeq

            ";
            var cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@PrjXMLSeq", prjSeq);
            return db.ExecuteScalar(cmd).ToString();
        }

        //水利署列管計畫
        public List<SelectOptionModel> GetControlPlanNoList()
        {
            string sql = @"SELECT
                    ProjectName Text,
                    ProjectName Value
                FROM wraControlPlanNo
                order by ProjectName
                ";
            SqlCommand cmd = db.GetCommand(sql);

            return db.GetDataTableWithClass<SelectOptionModel>(cmd);
        }
        //
        public List<T> QCPlaneWeaknessList<T>(int mode)
        {
            string sql = @"
                SELECT
                    a.TenderName,
                    a.ExecUnitName,
                    a.TownName,
                    a.ActualStartDate,
                    a.ScheCompletionDate,
                    exta.ActualProgress,
                    e.W1,
                    e.W2,
                    e.W3,
                    e.W4,
                    e.W5,
                    e.W6,
                    e.W7,
                    e.W8,
                    e.W9,
                    e.W10,
                    e.W11,
                    e.W12,
                    e.W13,
                    e.W14,
                    exta.AuditDate, --受查核
                    a.BidAmount,
                    ( -- 受督導資訊
                        SELECT STUFF(
                        (SELECT ',' + z.RecDate
                        FROM (
                            select DISTINCT z1.RecDate from (
                                select cast(datepart(year, zc.ModifyTime)-1911 as varchar(4))+FORMAT(CONVERT(Date, zc.ModifyTime), '/M/d') RecDate from SuperviseEng za
                                inner join SuperviseFill zb on(zb.SuperviseEngSeq=za.Seq)
                                inner join SuperviseFillInsideCommittee zc on(zc.SuperviseFillSeq=zb.Seq)
                                where za.SupervisePhaseSeq=a1.SupervisePhaseSeq
                                and za.PrjXMLSeq=a1.PrjXMLSeq

                                union all

                                select cast(datepart(year, zc.ModifyTime)-1911 as varchar(4))+FORMAT(CONVERT(Date, zc.ModifyTime), '/M/d') RecDate from SuperviseEng za
                                inner join SuperviseFill zb on(zb.SuperviseEngSeq=za.Seq)
                                inner join SuperviseFillOutCommittee zc on(zc.SuperviseFillSeq=zb.Seq)
                                where za.SupervisePhaseSeq=a1.SupervisePhaseSeq
                                and za.PrjXMLSeq=a1.PrjXMLSeq
                            ) z1
                        ) z
                        order by z.RecDate
                        FOR XML PATH('')) ,1,1,'')
                    ) AS RecDate,
                    ( -- 鄰近砂石場名稱
                        SELECT STUFF(
                        (SELECT ',' + z.vendorname
                        FROM (
                            select DISTINCT z1.vendorname from (
                                select za.vendorname
                                from Gravelfieldcoord za
                                WHERE e.W13=1
                                and a.CoordX is not null and a.CoordX is not null
                                and ((a.CoordX - za.TW97_X)*(a.CoordX - za.TW97_X)+(a.CoordY - za.TW97_Y)*(a.CoordY - za.TW97_Y)) < 250000
                            ) z1
                        ) z
                        order by z.vendorname
                        FOR XML PATH('')) ,1,1,'')
                    ) AS GravelField,
                    ( -- 鄰近標案名稱
                        SELECT STUFF(
                        (SELECT ',' + z.TenderName
                        FROM (
                            select DISTINCT z1.TenderName from (
                                select za.TenderName
                                from PrjXML za
                                WHERE za.TenderName like '%疏濬%'
                                and za.Seq <> a.Seq
                                and za.CoordX is not null and za.CoordX is not null
                                and a.CoordX is not null and a.CoordX is not null
                                and ((a.CoordX - za.CoordX)*(a.CoordX - za.CoordX)+(a.CoordY - za.CoordY)*(a.CoordY - za.CoordY)) < 250000
                            ) z1
                        ) z
                        order by z.TenderName
                        FOR XML PATH('')) ,1,1,'')
                    ) AS NearTender
                ";
            if (mode == 1)
            {//所屬機關
                sql += @"
                    FROM PrjXML a
                    left outer join PrjXMLExt exta on(exta.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    left outer join EngMain b on(b.PrjXMLSeq=a.Seq)
                    left outer join SuperviseEng a1 on(a1.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230706
                    where a.TenderYear>106 --ISNULL(exta.ActualProgress,0)<100
                    and ISNULL(exta.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'  --20230706
                    "
                    + Utils.getAuthoritySqlForTender1("a.")
                    + @"
                    and exists (select Name from Unit where ParentSeq is null and Name=a.ExecUnitName) ";
            }
            else if (mode == 2)
            {//縣市政府
                sql += @"
                    FROM PrjXML a
                    inner join Country2WRAMapping aa on(aa.Country=substring(a.ExecUnitName,1,3) " + getAuthoritySqlForTender("a.", "aa.RiverBureau") + @")
                    left outer join PrjXMLExt exta on(exta.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    left outer join EngMain b on(b.PrjXMLSeq=a.Seq)
                    left outer join SuperviseEng a1 on(a1.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230706
                    where a.TenderYear>106 --ISNULL(exta.ActualProgress,0)<100
                    and ISNULL(exta.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'  --20230706
                    ";
            }
            else if (mode == 3)
            {//其他補助
                sql += @"
                    FROM PrjXML a
                    left outer join PrjXMLExt exta on(exta.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    left outer join EngMain b on(b.PrjXMLSeq=a.Seq)
                    left outer join SuperviseEng a1 on(a1.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230706
                    where a.TenderYear>106 --ISNULL(exta.ActualProgress,0)<100
                    and ISNULL(exta.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'  --20230706
                    "
                    + Utils.getAuthoritySqlForTender1("a.")
                    + @"
                    and not exists (select Name from Unit where ParentSeq is null and Name=a.ExecUnitName)
                    and not exists (select Country from Country2WRAMapping where Country=substring(a.ExecUnitName,1,3) ) ";
            }
            /*sql += @"
                order by a.TenderYear desc, (e.W1+e.W2+e.W3+e.W4+e.W5+e.W6+e.W7+e.W8+e.W9+e.W10+e.W11+e.W12+e.W13+e.W14) DESC, a.TenderName Desc
                ";*/
            sql += @"
                order by a.ExecUnitName, a.TenderYear desc, a.TenderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            //cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            //cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            //cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<SuperviseEngSchedule2VModel> GetSupervisePhaseScheduleList(int supervisePhaseSeq)
        {
            string sql = @"
                SELECT
                    c.PhaseCode,
                    a.OrganizerName,
                    a.ExecUnitName, 
                    b.BelongPrj,
                    a.TenderName,
                    a.Location,
                    a1.SuperviseDate,
                    a1.SuperviseEndDate, --s20230316
                    ISNULL(b.DesignChangeContractAmount, a.BidAmount) BidAmount,
                    a.ScheCompletionDate,
                    d.PDAccuActualProgress ActualProgress,
                    (d.PDAccuActualProgress-d.PDAccuScheProgress) DiffProgress,
                    (
                    	select za.DisplayName from UserMain za
                        where za.Seq = a1.LeaderSeq
                    ) LeaderName,
                    (
                        SELECT STUFF(
                        (SELECT ',' + z.cName
                        FROM (
                            select zb.ECName cName, za.OrderNo from OutCommittee za
                            inner join ExpertCommittee zb on(zb.Seq=za.ExpertCommitteeSeq)
                            where za.SuperviseEngSeq=a1.Seq
                        ) z
                        order by z.OrderNo
                        FOR XML PATH('')) ,1,1,'')
                    ) AS OutCommittee,
                    (
                        SELECT STUFF(
                        (SELECT ',' + z.cName
                        FROM (
                            select zb.DisplayName cName, za.OrderNo from InsideCommittee za
                            inner join UserMain zb on(zb.Seq=za.UserMainSeq)
                            where za.SuperviseEngSeq=a1.Seq
                        ) z
                        order by z.OrderNo
                        FOR XML PATH('')) ,1,1,'')
                    ) AS InsideCommittee,
                    (
    	                select top 1 zb.DisplayName cName from Officer za
                        inner join UserMain zb on(zb.Seq=za.UserMainSeq)
                        where za.SuperviseEngSeq=a1.Seq
                        order by za.OrderNo
                    ) AS OfficerName,
                    a1.Memo
                FROM SupervisePhase c
                inner join SuperviseEng a1 on(a1.SupervisePhaseSeq=c.Seq)
                inner join PrjXML a on(a.Seq=a1.PrjXMLSeq)
                left outer join PrjXMLExt b on(b.PrjXMLSeq=a1.PrjXMLSeq)
                left join ProgressData d on (
	                d.Seq = ( select top 1 Seq from ProgressData where PrjXMLSeq=a1.PrjXMLSeq order by (PDYear*100+PDMonth) desc )
                )
                where c.Seq=@SupervisePhaseSeq
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SupervisePhaseSeq", supervisePhaseSeq);

            return db.GetDataTableWithClass<SuperviseEngSchedule2VModel>(cmd);
        }
        internal List<SuperviseEngTHSRVModel> GetThsrList()
        {
            string sql = $@"
                select * from 
                    (
                        select Max(z.CarNo) CarNo, 
                        Max(z.Memo) Memo, 
                        Max(z.DepartureTime) DepartureTime ,
                        Max(z.StartStationName) StartStationName,
                        Max(z.EndingStationName) EndingStationName,
                        Max(z.Direction) Direction
                    from(
                    select 
                    (
                        case a.Direction when '1' then '北上 車次:' else '南下 車次:' END
                        +a.CarNo+' '+a.StartStationName+'('+a.DepartureTime+')-'+a.EndingStationName+'('+a.ArrivalTime+')'
                    ) as CarNo,
                    a.Memo,
					a.DepartureTime,
                    a.StartStationName,
                    a.EndingStationName,
                    a.Direction
                    from THSR a
                    where Memo >= '{DateTime.Now.ToString("yyyy-MM-dd")}'

                    ) z

                group by z.CarNo) z2
				order by z2.DepartureTime
                ";
            SqlCommand cmd = db.GetCommand(sql);
            //cmd.Parameters.AddWithValue("@carNo", "%" + filter.carNo + "%");
            //cmd.Parameters.AddWithValue("@start", "%" + filter.start + "%");
            //cmd.Parameters.AddWithValue("@end", "%" + filter.end + "%");
            //cmd.Parameters.AddWithValue("@direction", filter.direction);
            return db.GetDataTableWithClass<SuperviseEngTHSRVModel>(cmd);
        }
        //職稱清單
        public List<T> GetPositions<T>()
        {//s20230327 包含 工程事務組 下屬單位
            string sql = @"
                select DISTINCT z.PositionSeq Value, z.PositionName Text
       
                from (
                  select a.UnitSeq, C.Name UnitName, a.PositionSeq, d.Name PositionName,
                      b.Seq, b.DisplayName, b.Email, b.Tel, b.Mobile
                  from UserUnitPosition a
                  inner join UserMain b on(b.Seq=a.UserMainSeq  and b.IsDelete = 0 and b.IsEnabled = 1)
                  inner join Position d on( d.Seq=a.PositionSeq and d.Name in('副總工程司','組長','副組長','簡任正工程司','科長','代理科長') )
                  inner join Unit c on( c.Seq=a.UnitSeq)
                  left outer join Unit c1 on( c1.Seq=c.ParentSeq )
                  where (c.Name in('總工程司室','工程事務組') or c1.Name in('工程事務組'))
                ) z
				";
            SqlCommand cmd = db.GetCommand(sql);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //職稱人員清單
        public List<T> GetPositionsUser<T>(int positionSeq)
        {//s20230327 包含 工程事務組 下屬單位
            string sql = @"
                select DISTINCT z.Seq Value, z.UserName Text
                from (
                  select a.UnitSeq, C.Name UnitName, a.PositionSeq, d.Name PositionName,
                      b.Seq, b.DisplayName UserName, b.Email, b.Tel, b.Mobile
                  from UserUnitPosition a
                  inner join UserMain b on(b.Seq=a.UserMainSeq and b.IsDelete = 0 and b.IsEnabled = 1)
                  inner join Position d on( d.Seq=a.PositionSeq and d.Name in('副總工程司','組長','副組長','簡任正工程司','科長','代理科長') )
                  inner join Unit c on( c.Seq=a.UnitSeq)
                  left outer join Unit c1 on( c1.Seq=c.ParentSeq )
                  where (c.Name in('總工程司室','工程事務組') or c1.Name in('工程事務組'))
                ) z
                where z.PositionSeq=@PositionSeq
                order by z.UserName
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@PositionSeq", positionSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //幹事 清單與查詢
        public List<T> GetOfficerCommitte<T>(int superviseEngSeq, string keyword)
        {
            string sql = @"
                select Cast(0 as int) mode,
                    a.OrderNo,
                    a.Seq,
                    ISNULL(ic.Seq, 0) IsInside,
	                b.DisplayName CName,
                    b.Email,
                    b.Tel,
                    b.Mobile
                from Officer a
                inner join UserMain b on(b.Seq=a.UserMainSeq)
                left join InsideCommittee ic on (ic.UserMainSeq = a.UserMainSeq and ic.SuperviseEngSeq = @SuperviseEngSeq)
                where a.SuperviseEngSeq=@SuperviseEngSeq
				";
            if (!String.IsNullOrEmpty(keyword))
            {//s20230413 包含 工程事務組 下屬單位
                sql = @"select Cast(1 as int) mode,
                    0 OrderNo,
                    a.Seq,
                    ic.Seq IsInside,
	                a.DisplayName CName,
                    a.Email,
                    a.Tel,
                    a.Mobile
                from UserMain a
                inner join UserUnitPosition b on(b.UserMainSeq=a.Seq)
                inner join Unit c on(c.Seq=b.UnitSeq)
                left outer join Unit c1 on( c1.Seq=c.ParentSeq )
                left join InsideCommittee ic on (ic.UserMainSeq = a.Seq and ic.SuperviseEngSeq = @SuperviseEngSeq)
                where a.IsEnabled = 1 and a.IsDelete = 0
                and (c.Name in('工程事務組') or c1.Name in('工程事務組'))
                and a.DisplayName like @keyword
                and a.Seq not in (
	                select UserMainSeq from Officer where SuperviseEngSeq=@SuperviseEngSeq
                )

                union all

                " + sql;
                keyword = "%" + keyword + "%";
            }

            sql = @"select z.*
                    from (" + sql +
                    @") z
                    order by z.mode DESC, z.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);
            cmd.Parameters.AddWithValue("@keyword", keyword);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public int AddOfficerCommitte(int superviseEngSeq, int committeSeq)
        {
            string sql = @"
                insert into Officer(
	                SuperviseEngSeq,
                    UserMainSeq,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                )values(
	                @SuperviseEngSeq,
                    @UserMainSeq,
                    (select count(Seq)+1 from Officer where SuperviseEngSeq=@SuperviseEngSeq),
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )
				";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);
                cmd.Parameters.AddWithValue("@UserMainSeq", committeSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.AddOfficerCommitte: " + e.Message);
                log.Info(sql);
                return 0;
            }
        }
        public int DelOfficerCommitte(int seq)
        {
            string sql = @"delete from Officer where Seq=@Seq";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);

                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.DelOfficerCommitte: " + e.Message);
                log.Info(sql);
                return 0;
            }
        }

        //內聘委員 清單與查詢
        public List<T> GetInsideCommitte<T>(int superviseEngSeq, string keyword)
        {
            string sql = @"
                select Cast(0 as int) mode,
                    a.OrderNo,
                    a.Seq,
	                b.DisplayName CName,
                    b.Email,
                    b.Tel,
                    b.Mobile
                from InsideCommittee a
                inner join UserMain b on(b.Seq=a.UserMainSeq)
              
                where 
                a.SuperviseEngSeq=@SuperviseEngSeq
				";

            /*string getUnitSql = "select ParentSeq, Name, Seq from Unit";
            //db.GetDataTableWithClass<Unit>(db.GetCommand(getUnitSql));
            List<Unit> unitList = db.GetDataTable(db.GetCommand(getUnitSql)).Rows.Cast<DataRow>()

            .Select(row => new Unit()
            {
                ParentSeq = Convert.ToInt32(row.Field<Int16?>("ParentSeq") ),
                Seq = row.Field<Int16>("Seq"),
                Name = row.Field<string>("Name"),
                Code = ""
            }).ToList<Unit>();*/

            if (!String.IsNullOrEmpty(keyword))
            {
                sql = @"select Cast(1 as int) mode,
                    0 OrderNo,
                    a.Seq,
	                a.DisplayName CName,
                    a.Email,
                    a.Tel,
                    a.Mobile
                from UserMain a
                inner join UserUnitPosition b on(b.UserMainSeq=a.Seq)
                inner join Unit c on( c.Seq=b.UnitSeq)
                left outer join Unit c1 on(c1.Seq=c.ParentSeq)
                left outer join Unit c2 on(c2.Seq=c1.ParentSeq)
                where a.IsEnabled = 1 and a.IsDelete = 0
                and a.DisplayName like @keyword
                and a.Seq not in (
	                select UserMainSeq from InsideCommittee where SuperviseEngSeq=@SuperviseEngSeq
                )

                union all

                " + sql;
                keyword = "%" + keyword + "%";
            }

            sql = @"select z.mode, z.OrderNo, z.Seq, z.CName, z.Email, z.Tel, z.Mobile
                    from (" + sql +
                    @") z
                    order by z.mode DESC, z.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);
            cmd.Parameters.AddWithValue("@keyword", keyword);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public int AddInsideCommitte(int superviseEngSeq, int committeSeq)
        {
            string sql = @"
                insert into InsideCommittee(
	                SuperviseEngSeq,
                    UserMainSeq,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                )values(
	                @SuperviseEngSeq,
                    @UserMainSeq,
                    (select count(Seq)+1 from InsideCommittee where SuperviseEngSeq=@SuperviseEngSeq),
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )
				";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);
                cmd.Parameters.AddWithValue("@UserMainSeq", committeSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.AddInsideCommitte: " + e.Message);
                log.Info(sql);
                return 0;
            }
        }
        public int DelInsideCommitte(int seq)
        {
            string sql = @"
                delete from SuperviseFillInsideCommittee where InsideCommitteeSeq = @Seq;
                delete from InsideCommittee where Seq=@Seq";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);

                db.ExecuteNonQuery(cmd);
                return 1;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.DelInsideCommitte: " + e.Message);
                log.Info(sql);
                return 0;
            }
        }

        //外聘委員 清單與查詢
        public List<T> GetOutCommitte<T>(int superviseEngSeq, string keyword)
        {
            string sql = @"
                select Cast(0 as int) mode,
                    a.OrderNo,
                    a.Seq,
	                b.ECName CName,
                    b.ECEmail Email,
                    b.ECTel Tel,
                    b.ECMobile Mobile,
                    SUBSTRING(b.ECId,1,2) Id
                from OutCommittee a
                inner join ExpertCommittee b on(b.Seq=a.ExpertCommitteeSeq)
                where a.SuperviseEngSeq=@SuperviseEngSeq
				";
            if(!String.IsNullOrEmpty(keyword))
            {
                sql = @"select Cast(1 as int) mode,
                    a.OrderNo,
                    a.Seq,
	                a.ECName CName,
                    a.ECEmail Email,
                    a.ECTel Tel,
                    a.ECMobile Mobile,
                    SUBSTRING(a.ECId,1,2) Id
                from ExpertCommittee a
                where a.ECName like @keyword
                and a.Seq not in (
	                select ExpertCommitteeSeq from OutCommittee where SuperviseEngSeq=@SuperviseEngSeq
                )

                union all

                " + sql;
                keyword = "%" + keyword + "%";
            }

            sql = @"select z.mode, z.OrderNo, z.Seq, z.CName, z.Email, z.Tel, z.Mobile, z.Id
                    from (" + sql +
                    @") z
                    order by z.mode DESC, z.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);
            cmd.Parameters.AddWithValue("@keyword", keyword);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public int AddOutCommitte(int superviseEngSeq, int committeSeq)
        {
            string sql = @"
                insert into OutCommittee(
	                SuperviseEngSeq,
                    ExpertCommitteeSeq,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                )values(
	                @SuperviseEngSeq,
                    @ExpertCommitteeSeq,
                    (select count(Seq)+1 from OutCommittee where SuperviseEngSeq=@SuperviseEngSeq),
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )
				";
            try { 
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);
                cmd.Parameters.AddWithValue("@ExpertCommitteeSeq", committeSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.AddOutCommitte: " + e.Message);
                log.Info(sql);
                return 0;
            }
}
        public int DelOutCommitte(int seq)
        {
            string sql = @"
                delete from SuperviseFillOutCommittee where OutCommitteeSeq = @Seq;
                delete from OutCommittee where Seq=@Seq;";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);

                db.ExecuteNonQuery(cmd);
                return 1;
            }
            catch (Exception    e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.DelOutCommitte: " + e.Message);
                log.Info(sql);
                return 0;
            }
        }
        //外聘委員建立帳號
        public int checkAccount(int itemType, string taxId, string userName, string email, string tel, string mobile)
        {
            Int16 unitSeq = -1;
            int accountSeq = -1;
            Int16 unitParentSeq = 0;
            byte roleSeq = 0;
            if (itemType == UnitService.type_OutCommitteeUnit)
            {
                unitParentSeq = ConfigManager.OutCommitteeUnit_UnitParentSeq;
                unitSeq = unitParentSeq;
                roleSeq = ConfigManager.Committee_RoleSeq;
            }

            if (unitParentSeq == 0 || roleSeq == 0) return -1;//系統資料錯誤

            string sql;
            SqlCommand cmd;
            DataTable dt;

            sql = @"SELECT Seq FROM UserMain where UserNo=@taxId";
            cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@taxId", taxId);
            dt = db.GetDataTable(cmd);
            if (dt.Rows.Count > 0) accountSeq = Convert.ToInt16(dt.Rows[0]["Seq"].ToString());

            if (accountSeq == -1)
            {

                db.BeginTransaction();
                try
                {
                    if (accountSeq == -1)
                    {
                        //新增人員
                        sql = @"
                        INSERT INTO [UserMain]
                               ([UserNo]
                               ,[PassWord]
                               ,[DisplayName]
                               ,[Email]
                               ,[Tel]
                               ,[Mobile]
                               ,[IsEnabled]
                               ,[CreateTime]
                               ,[CreateUserSeq]
                               ,[ModifyTime]
                               ,[ModifyUserSeq])
                         VALUES
                               (@UserNo
                               ,@PassWord
                               ,@DisplayName
                               ,@Email
                               ,@Tel
                               ,@Mobile
                               ,1
                               ,GETDATE()
                               ,@ModifyUserSeq
                               ,GETDATE()
                               ,@ModifyUserSeq)";
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.AddWithValue("@UserNo", taxId);
                        cmd.Parameters.AddWithValue("@PassWord", taxId.Substring(6, 4));
                        cmd.Parameters.AddWithValue("@DisplayName", userName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Tel", this.NulltoDBNull(tel));
                        cmd.Parameters.AddWithValue("@Mobile", this.NulltoDBNull(mobile));
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                        db.ExecuteNonQuery(cmd);

                        sql = @"SELECT IDENT_CURRENT('UserMain') AS NewSeq";
                        cmd = db.GetCommand(sql);
                        dt = db.GetDataTable(cmd);
                        accountSeq = Convert.ToInt16(dt.Rows[0]["NewSeq"].ToString());

                        //使用者、單位、職務關聯
                        sql = @"
                        INSERT INTO [UserUnitPosition]
                               ([UnitSeq]
                               ,[UserMainSeq]
                               ,[IsEnabled]
                               ,[CreateTime]
                               ,[CreateUserSeq]
                               ,[ModifyTime]
                               ,[ModifyUserSeq])
                         VALUES
                               (@UnitSeq
                               ,@UserMainSeq
                               ,1
                               ,GETDATE()
                               ,@ModifyUserSeq
                               ,GETDATE()
                               ,@ModifyUserSeq)";
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.AddWithValue("@UnitSeq", unitSeq);
                        cmd.Parameters.AddWithValue("@UserMainSeq", accountSeq);
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                        db.ExecuteNonQuery(cmd);

                        sql = @"SELECT IDENT_CURRENT('UserUnitPosition') AS NewSeq";
                        cmd = db.GetCommand(sql);
                        dt = db.GetDataTable(cmd);
                        int userUnitPositionSeq = Convert.ToInt16(dt.Rows[0]["NewSeq"].ToString());

                        sql = @"
                        INSERT INTO [UserRole] (
                               UserUnitPositionSeq,
                               RoleSeq
                        )VALUES(
                               @UserUnitPositionSeq,
                               @RoleSeq
                        )";
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.AddWithValue("@UserUnitPositionSeq", userUnitPositionSeq);
                        cmd.Parameters.AddWithValue("@RoleSeq", roleSeq);
                        db.ExecuteNonQuery(cmd);
                    }

                    db.TransactionCommit();
                }
                catch (Exception ex)
                {
                    db.TransactionRollback();
                    log.Info("SupervisePhaseService.checkAccount");
                    log.Info(ex.Message);
                    log.Info(sql);
                    return -999;
                }
            }
            return 0;
        }
        //取得工程 前置作業
        public List<T> GetEngForPrework<T>(int seq)
        {
            string sql = @"SELECT
                    b.Seq,
                    b.PrjXMLSeq,
                    b.SuperviseDate,
                    b.SuperviseEndDate, --s20230316
                    b.SuperviseMode,
                    b.LeaderSeq,
                    b.Memo,
                    a.TenderNo EngNo,
                    a.TenderName EngName,
                    c.PositionSeq,
                    ISNULL(a.ManualBelongPrj, d.BelongPrj) BelongPrj
                FROM SuperviseEng b
                inner join PrjXML a on(a.Seq=b.PrjXMLSeq)
                left outer join UserUnitPosition c on(c.UserMainSeq=b.LeaderSeq)
                left outer join PrjXMLExt d on(d.PrjXMLSeq=a.Seq)
                where b.Seq=@Seq"
                + Utils.getAuthoritySql("a.");
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public int SaveEngForPrework(SuperviseEngPreWordVModel m)
        {
            Null2Empty(m);
            string sql = @"
                update SuperviseEng set
                    SuperviseDate = @SuperviseDate,
                    SuperviseEndDate = @SuperviseEndDate,  --s20230316
                    SuperviseMode = @SuperviseMode,
                    LeaderSeq = @LeaderSeq,
                    Memo = @Memo,
                    ModifyTime=GetDate(),
                    ModifyUser=@ModifyUserSeq
                where Seq=@Seq
				";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@SuperviseDate", this.NulltoDBNull(m.SuperviseDate));
                cmd.Parameters.AddWithValue("@SuperviseEndDate", this.NulltoDBNull(m.SuperviseEndDate));
                cmd.Parameters.AddWithValue("@SuperviseMode", m.SuperviseMode);
                cmd.Parameters.AddWithValue("@LeaderSeq", this.NulltoDBNull(m.LeaderSeq));
                cmd.Parameters.AddWithValue("@Memo", m.Memo);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                sql = @"
                update PrjXML set
                    ManualBelongPrj = @ManualBelongPrj,
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                where Seq=( select PrjXMLSeq from SuperviseEng where Seq=@Seq )
                and Exists (select Seq from wraControlPlanNo where ProjectName=@ManualBelongPrj)
				";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ManualBelongPrj", m.BelongPrj);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 1;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.SaveEngForPrework: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }

        //更新期別
        public int SaveChangePhaseForPrework(SuperviseEngPreWordVModel m, int PhaseSeq)
        {
            Null2Empty(m);
            string sql = @"
                update SuperviseEng set
                    SupervisePhaseSeq = @SupervisePhaseSeq,
                    ModifyTime=GetDate(),
                    ModifyUser=@ModifyUserSeq
                where Seq=@Seq
				";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@SupervisePhaseSeq", PhaseSeq);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
                return 1;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.SaveChangePhaseForPrework: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
            //取得工程 行程安排
            public List<T> GetEngForShcedule<T>(int seq)
        {
            string sql = @"SELECT
                    b.Seq,
                    b.PrjXMLSeq,
                    (select DisplayName from UserMain where Seq = b.LeaderSeq) LeaderName,
                    (
                        SELECT STUFF(
                        (SELECT ',' + z.cName
                        FROM (
                            select zb.ECName cName, za.OrderNo from OutCommittee za
                            inner join ExpertCommittee zb on(zb.Seq=za.ExpertCommitteeSeq)
                            where za.SuperviseEngSeq=b.Seq
                        ) z
                        order by z.OrderNo
                        FOR XML PATH('')) ,1,1,'')
                    ) AS OutCommittee,
                    (
                        SELECT STUFF(
                        (SELECT ',' + z.cName
                        FROM (
                            select zb.DisplayName cName, za.OrderNo from InsideCommittee za
                            inner join UserMain zb on(zb.Seq=za.UserMainSeq)
                            where za.SuperviseEngSeq=b.Seq
                        ) z
                        order by z.OrderNo
                        FOR XML PATH('')) ,1,1,'')
                    ) AS InsideCommittee,
                    (
                        SELECT STUFF(
                        (SELECT ',' + z.cName
                        FROM (
                            select zb.DisplayName cName, za.OrderNo from Officer za
                            inner join UserMain zb on(zb.Seq=za.UserMainSeq)
                            where za.SuperviseEngSeq=b.Seq
                        ) z
                        order by z.OrderNo
                        FOR XML PATH('')) ,1,1,'')
                    ) OfficerName,
                    (
    	                select top 1 zb.Tel from Officer za
                        inner join UserMain zb on(zb.Seq=za.UserMainSeq)
                        where za.SuperviseEngSeq=b.Seq
                        order by za.OrderNo
                    ) OfficerTel,
                    (
    	                select top 1 zb.Mobile from Officer za
                        inner join UserMain zb on(zb.Seq=za.UserMainSeq)
                        where za.SuperviseEngSeq=b.Seq
                        order by za.OrderNo
                    ) OfficerMobile,
                    b.BriefingPlace,
                    b.BriefingAddr,
                    b.IsVehicleDisp,
                    b.AdminContact,
                    b.AdminTel,
                    b.AdminMobile,
                    b.RiverBureauContact,
                    b.RiverBureauTel,
                    b.RiverBureauMobile,
                    b.LocalGovContact,
                    b.LocalGovTel,
                    b.LocalGovMobile,
                    b.ToBriefingDrive,
                    b.SuperviseStartTime,
                    b.SuperviseOrder,
                    a.TenderNo EngNo,
                    a.TenderName EngName,
                    a.ExecUnitName,
                    a.OrganizerName,
                    a.ContactName,
                    a.ContactPhone
                FROM SuperviseEng b
                inner join PrjXML a on(a.Seq=b.PrjXMLSeq)
                --left outer join PrjXML c on(c.Seq=a.PrjXMLSeq)
                where b.Seq=@Seq"
                + Utils.getAuthoritySql("a.");
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public int SaveEngForShcedule(SuperviseEngModel m, List<SuperviseScheduleFormModel> scheduleForm)
        {
            Null2Empty(m);
            string sql = @"
                update SuperviseEng set
                    BriefingPlace = @BriefingPlace,
                    BriefingAddr = @BriefingAddr,
                    IsVehicleDisp = @IsVehicleDisp,
                    AdminContact = @AdminContact,
                    AdminTel = @AdminTel,
                    AdminMobile = @AdminMobile,
                    RiverBureauContact = @RiverBureauContact,
                    RiverBureauTel = @RiverBureauTel,
                    RiverBureauMobile = @RiverBureauMobile,
                    LocalGovContact = @LocalGovContact,
                    LocalGovTel = @LocalGovTel,
                    LocalGovMobile = @LocalGovMobile,
                    ToBriefingDrive = @ToBriefingDrive,
                    SuperviseStartTime = @SuperviseStartTime,
                    SuperviseOrder = @SuperviseOrder,
                    ModifyTime=GetDate(),
                    ModifyUser=@ModifyUserSeq
                where Seq=@Seq
				";
            db.BeginTransaction();
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@BriefingPlace", m.BriefingPlace);
                cmd.Parameters.AddWithValue("@BriefingAddr", m.BriefingAddr);
                cmd.Parameters.AddWithValue("@IsVehicleDisp", m.IsVehicleDisp);
                cmd.Parameters.AddWithValue("@AdminContact", m.AdminContact);
                cmd.Parameters.AddWithValue("@AdminTel", m.AdminTel);
                cmd.Parameters.AddWithValue("@AdminMobile", m.AdminMobile);
                cmd.Parameters.AddWithValue("@RiverBureauContact", m.RiverBureauContact);
                cmd.Parameters.AddWithValue("@RiverBureauTel", m.RiverBureauTel);
                cmd.Parameters.AddWithValue("@RiverBureauMobile", m.RiverBureauMobile);
                cmd.Parameters.AddWithValue("@LocalGovContact", m.LocalGovContact);
                cmd.Parameters.AddWithValue("@LocalGovTel", m.LocalGovTel);
                cmd.Parameters.AddWithValue("@LocalGovMobile", m.LocalGovMobile);
                cmd.Parameters.AddWithValue("@ToBriefingDrive", m.ToBriefingDrive);
                cmd.Parameters.AddWithValue("@SuperviseStartTime", this.NulltoDBNull(m.SuperviseStartTime));
                cmd.Parameters.AddWithValue("@SuperviseOrder", this.NulltoDBNull(m.SuperviseOrder));
                
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                sql = @"
                    update SuperviseScheduleForm set
                        OrderNo = @OrderNo,
                        Summary = @Summary,
                        StartTime = @StartTime,
                        EndTime = @EndTime,
                        ActivityTime = @ActivityTime
                    where Seq=@Seq
                    ";
                int orderNo = 1;
                foreach(SuperviseScheduleFormModel item in scheduleForm)
                {//s20230519
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@OrderNo", orderNo);
                    cmd.Parameters.AddWithValue("@Summary", item.Summary);
                    cmd.Parameters.AddWithValue("@StartTime", item.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", item.EndTime);
                    cmd.Parameters.AddWithValue("@ActivityTime", item.ActivityTime);//s20230524
                    cmd.Parameters.AddWithValue("@Seq", item.Seq);
                    db.ExecuteNonQuery(cmd);
                    orderNo++;
                }
                db.TransactionCommit();
                return 1;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.SaveEngForShcedule: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
        //高鐵
        public List<SuperviseEngTHSRVModel> GetEngTHSR(int superviseEngSeq)
        {
            string sql = @"
                select
                    a.Seq,
                    a.SuperviseEngSeq,
	                a.CarNo,
                    a.Memo
                from SuperviseEngTHSR a
                where a.SuperviseEngSeq=@SuperviseEngSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);

            return db.GetDataTableWithClass<SuperviseEngTHSRVModel>(cmd);
        }
        public int AddEngTHSR(SuperviseEngTHSRVModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into SuperviseEngTHSR(
	                SuperviseEngSeq,
                    CarNo,
                    Memo,
                    CreateTime,
                    CreateUser
                )values(
	                @SuperviseEngSeq,
                    @CarNo,
                    @Memo,
                    GetDate(),
                    @ModifyUserSeq
                )
				";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@SuperviseEngSeq", m.SuperviseEngSeq);
                cmd.Parameters.AddWithValue("@CarNo", m.CarNo);
                cmd.Parameters.AddWithValue("@Memo", m.Memo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.AddEngTHSR: " + e.Message);
                log.Info(sql);
                return 0;
            }
        }
        public int DelEngTHSR(int seq)
        {
            string sql = @"delete from SuperviseEngTHSR where Seq=@Seq";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);

                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.DelEngTHSR: " + e.Message);
                log.Info(sql);
                return 0;
            }
        }
        public List<SuperviseEngTHSRVModel> GetTHSR()
        {
            string sql = @"
                select z.CarNo, z.Memo from(
                    select 
                    (
                        case a.Direction when '1' then '北上 車次:' else '南下 車次:' END
                        +a.CarNo+' '+a.StartStationName+'('+a.DepartureTime+')-'+a.EndingStationName+'('+a.ArrivalTime+')'
                    ) as CarNo,
                    a.Memo
                    from THSR a
                ) z
                order by z.CarNo";
            SqlCommand cmd = db.GetCommand(sql);

            return db.GetDataTableWithClass<SuperviseEngTHSRVModel>(cmd);
        }
        //年度督導期別
        public List<SupervisePhaseModel> GetPhaseOptions(string chsYear)
        {
            string sql = @"
                SELECT Seq, PhaseCode FROM SupervisePhase
                where PhaseCode like @Year ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Year", String.Format("{0}%", chsYear));
            return db.GetDataTableWithClass<SupervisePhaseModel>(cmd);
        }
        //查詢督導期別
        public List<SupervisePhaseModel> GetPhaseCode(string phaseCode)
        {
            string sql = @"
                SELECT Seq, PhaseCode FROM SupervisePhase
                where PhaseCode Like @PhaseCode + '%' ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@PhaseCode", phaseCode);
            return db.GetDataTableWithClass<SupervisePhaseModel>(cmd);
        }
        //新增督導期別
        public int AddPhaseCode(string phaseCode)
        {
            string sql = @"
                insert into SupervisePhase (
                    PhaseCode,
                    CreateTime,
                    CreateUser,
                    ModifyTime,
                    ModifyUser
                )values(
                    @PhaseCode,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )";

            db.BeginTransaction();
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@PhaseCode", phaseCode);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('SupervisePhase') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);

                db.TransactionCommit();
                return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.AddPhaseCode: " + e.Message);
                log.Info(sql);
                return 0;
            }
        }
        //刪除期別
        public int DelPhase(int seq)
        {
            string sql ="";
            db.BeginTransaction();
            try
            {
                sql = @"delete from SuperviseEng where SupervisePhaseSeq=@SupervisePhaseSeq";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@SupervisePhaseSeq", seq);
                db.ExecuteNonQuery(cmd);

                sql = @"delete from SupervisePhase where Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.DelPhase: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
        //工程加入督導期別
        public int AddEng(int phaseSeq, int prjXMLSeq)
        {
            string sql = "";
            db.BeginTransaction();
            try
            {
                sql = @"
                    update SupervisePhase set
                        ModifyTime=GetDate(),
                        ModifyUser=@ModifyUserSeq
                    where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", phaseSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                sql = @"
                insert into SuperviseEng (
                    SupervisePhaseSeq,
                    PrjXMLSeq,
                    W1,
                    W2,
                    W3,
                    W4,
                    W5,
                    W6,
                    W7,
                    W8,
                    W9,
                    W10,
                    W11,
                    W12,
                    W13,
                    W14,
                    SuperviseStartTime,
                    Memo,
                    CreateTime,
                    CreateUser,
                    ModifyTime,
                    ModifyUser
                )
                select
                    @SupervisePhaseSeq,
                    a.Seq,
                    ISNULL(e.W1,0) W1,
                    ISNULL(e.W2,0) W2,
                    ISNULL(e.W3,0) W3,
                    ISNULL(e.W4,0) W4,
                    ISNULL(e.W5,0) W5,
                    ISNULL(e.W6,0) W6,
                    ISNULL(e.W7,0) W7,
                    ISNULL(e.W8,0) W8,
                    ISNULL(e.W9,0) W9,
                    ISNULL(e.W10,0) W10,
                    ISNULL(e.W11,0) W11,
                    ISNULL(e.W12,0) W12,
                    ISNULL(e.W13,0) W13,
                    ISNULL(e.W14,0) W14,
                    '10:00:00',
                    (
                        IIF(e.W1=1,'1重大(或重點防汛)，','')
                        +IIF(e.W2=1,'2進度落後，','')
                        +IIF(e.W3=1,'3決標比偏低，','')
                        +IIF(e.W4=1,'4施工廠商近年查核成績不佳，','')
                        +IIF(e.W5=1,'5曾發生重大職安事件之標案，','')
                        +IIF(e.W6=1,'6履約計分偏低標案，','')
                        +IIF(e.W7=1,'7近三年曾遭停權之施工廠商，','')
                        +IIF(e.W8=1,'8施工廠商近期承攬能量偏高，','')
                        +IIF(e.W9=1,'9施工廠商跨區域承攬，','')
                        +IIF(e.W10=1,'10施工量能偏低，','')
                        +IIF(e.W11=1,'11委外監造之工程，','')
                        +IIF(e.W12=1,'12成績不佳的委外監造廠商，','')
                        +IIF(e.W13=1,'13高敏感區域工程，','')
                        +IIF(e.W14=1,'14全民督工','')
                    ),
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                from PrjXML a
                left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                where a.Seq=@PrjXMLSeq
                ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@SupervisePhaseSeq", phaseSeq);
                cmd.Parameters.AddWithValue("@PrjXMLSeq", prjXMLSeq); 
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                int cnt = db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return cnt;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.AddEng: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
        //督導期別移除工程
        public int DelEng(int seq)
        {
            string sql = "";

            db.BeginTransaction();
            try
            {
                sql = @"update SupervisePhase set
                        ModifyTime=GetDate(),
                        ModifyUser=@ModifyUserSeq
                        where Seq=(
                            select a.SupervisePhaseSeq from SuperviseEng a where a.Seq=@Seq
                        )";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                sql = @"delete from SuperviseEng where Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.DelEng: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }   
        //工程標案數量 shioulo 20230210
        public int GetTenderListCount(int mode, string fName, string fUnit)
        {
            string sql = @"";
            sql = @"
                SELECT count(a.Seq) total
                ";
                /* FROM PrjXML a
                left outer join PrjXMLExt exta on(exta.PrjXMLSeq=a.Seq)
                --left outer join EngMain b on(b.PrjXMLSeq=a.Seq)
                --left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                where ISNULL(exta.ActualProgress,0)<100
                "
                + Utils.getAuthoritySqlForTender1("a.");*/
            if (mode == 1)
            {
                sql += @"
                    FROM PrjXML a
                    left outer join PrjXMLExt exta on(exta.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    left outer join EngMain b on(b.PrjXMLSeq=a.Seq)
                    left outer join SuperviseEng a1 on(a1.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230706
                    where a.TenderYear>106 --ISNULL(exta.ActualProgress,0)<100
                    and ISNULL(exta.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'  --20230808
                    "
                    + Utils.getAuthoritySqlForTender1("a.")
                    + @"
                    and exists (select Name from Unit where ParentSeq is null and Name=a.ExecUnitName) ";
            }
            else if (mode == 2)
            {
                sql += @"
                    FROM PrjXML a
                    inner join Country2WRAMapping aa on(aa.Country=substring(a.ExecUnitName,1,3) "
                    + getAuthoritySqlForTender("a.", "aa.RiverBureau") +
                    @")
                    left outer join PrjXMLExt exta on(exta.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    left outer join EngMain b on(b.PrjXMLSeq=a.Seq)
                    left outer join SuperviseEng a1 on(a1.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230706
                    where a.TenderYear>106 --ISNULL(exta.ActualProgress,0)<100
                    and ISNULL(exta.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成' --20230808";
            }
            else if (mode == 3)
            {
                sql += @"
                    FROM PrjXML a
                    left outer join PrjXMLExt exta on(exta.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    left outer join EngMain b on(b.PrjXMLSeq=a.Seq)
                    left outer join SuperviseEng a1 on(a1.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230706
                    where a.TenderYear>106 --ISNULL(exta.ActualProgress,0)<100
                    and ISNULL(exta.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成' --20230808
                    "
                    + Utils.getAuthoritySqlForTender1("a.")
                    + @"
                    and not exists (select Name from Unit where ParentSeq is null and Name=a.ExecUnitName)
                    and not exists (select Country from Country2WRAMapping where Country=substring(a.ExecUnitName,1,3) ) ";
            }
            sql += @"
                    and (@fName='' or a.TenderName like @fName)
                    and (@fUnit='' or a.ExecUnitName=@fUnit)
                ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@fName", String.IsNullOrEmpty(fName) ? "" : "%" + fName + "%");
            cmd.Parameters.AddWithValue("@fUnit", String.IsNullOrEmpty(fUnit) ? "" : fUnit);

            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetTenderList<T>(int mode, int pageRecordCount, int pageIndex, string fName, string fUnit/*, ESQEngFilterVModel filterItem*/)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.TenderNo EngNo,
                    a.TenderName EngName,
                    a.ExecUnitName ExecUnit,
                    b.Seq EngMainSeq,
                    e.W1,
                    e.W2,
                    e.W3,
                    e.W4,
                    e.W5,
                    e.W6,
                    e.W7,
                    e.W8,
                    e.W9,
                    e.W10,
                    e.W11,
                    e.W12,
                    e.W13,
                    e.W14,
                    (
                        SELECT STUFF(
                        (SELECT ',' + z.RecDate
                        FROM (
                            select DISTINCT z1.RecDate from (
                                select cast(datepart(year, zc.ModifyTime)-1911 as varchar(4))+FORMAT(CONVERT(Date, zc.ModifyTime), '/M/d') RecDate from SuperviseEng za
                                inner join SuperviseFill zb on(zb.SuperviseEngSeq=za.Seq)
                                inner join SuperviseFillInsideCommittee zc on(zc.SuperviseFillSeq=zb.Seq)
                                where za.SupervisePhaseSeq=a1.SupervisePhaseSeq
                                and za.PrjXMLSeq=a1.PrjXMLSeq

                                union all

                                select cast(datepart(year, zc.ModifyTime)-1911 as varchar(4))+FORMAT(CONVERT(Date, zc.ModifyTime), '/M/d') RecDate from SuperviseEng za
                                inner join SuperviseFill zb on(zb.SuperviseEngSeq=za.Seq)
                                inner join SuperviseFillOutCommittee zc on(zc.SuperviseFillSeq=zb.Seq)
                                where za.SupervisePhaseSeq=a1.SupervisePhaseSeq
                                and za.PrjXMLSeq=a1.PrjXMLSeq
                            ) z1
                        ) z
                        order by z.RecDate
                        FOR XML PATH('')) ,1,1,'')
                    ) AS RecDate
                ";
            /*FROM PrjXML a
            left outer join PrjXMLExt exta on(exta.PrjXMLSeq=a.Seq)
            left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
            left outer join EngMain b on(b.PrjXMLSeq=a.Seq)
            left outer join SuperviseEng a1 on(a1.PrjXMLSeq=a.Seq)
            where ISNULL(exta.ActualProgress,0)<100
            ";*/

            if (mode == 1)
            {//所屬機關
                sql += @"
                    FROM PrjXML a
                    left outer join PrjXMLExt exta on(exta.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    left outer join EngMain b on(b.PrjXMLSeq=a.Seq)
                    left outer join SuperviseEng a1 on(a1.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230706
                    where a.TenderYear>106 --ISNULL(exta.ActualProgress,0)<100
                    and ISNULL(exta.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成'  --20230706
                    "
                    + Utils.getAuthoritySqlForTender1("a.")
                    + @"
                    and exists (select Name from Unit where ParentSeq is null and Name=a.ExecUnitName) ";
            } else if (mode == 2)
            {//縣市政府
                sql += @"
                    FROM PrjXML a
                    inner join Country2WRAMapping aa on(aa.Country=substring(a.ExecUnitName,1,3) "
                    + getAuthoritySqlForTender("a.", "aa.RiverBureau") +
                    @")
                    left outer join PrjXMLExt exta on(exta.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    left outer join EngMain b on(b.PrjXMLSeq=a.Seq)
                    left outer join SuperviseEng a1 on(a1.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230706
                    where a.TenderYear>106 --ISNULL(exta.ActualProgress,0)<100
                    and ISNULL(exta.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成' --20230706
                    ";
                    
            }
            else if (mode == 3)
            {//其他補助
                sql += @"
                    FROM PrjXML a
                    left outer join PrjXMLExt exta on(exta.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    left outer join EngMain b on(b.PrjXMLSeq=a.Seq)
                    left outer join SuperviseEng a1 on(a1.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230706
                    where a.TenderYear>106 --ISNULL(exta.ActualProgress,0)<100
                    and ISNULL(exta.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成' --20230706
                    "
                    + Utils.getAuthoritySqlForTender1("a.")
                    + @"
                    and not exists (select Name from Unit where ParentSeq is null and Name=a.ExecUnitName)
                    and not exists (select Country from Country2WRAMapping where Country=substring(a.ExecUnitName,1,3) ) ";
            }
            /*sql += @"
                order by a.TenderYear desc, (e.W1+e.W2+e.W3+e.W4+e.W5+e.W6+e.W7+e.W8+e.W9+e.W10+e.W11+e.W12+e.W13+e.W14) DESC, a.TenderName Desc
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";*/

            sql += @"
                and (@fName='' or a.TenderName like @fName)
                and (@fUnit='' or a.ExecUnitName=@fUnit)
                order by (e.W1+e.W2+e.W3+e.W4+e.W5+e.W6+e.W7+e.W8+e.W9+e.W10+e.W11+e.W12+e.W13+e.W14) DESC
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@fName", String.IsNullOrEmpty(fName) ? "" : "%"+ fName + "%");
            cmd.Parameters.AddWithValue("@fUnit", String.IsNullOrEmpty(fUnit) ? "" : fUnit);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //單位清單 s20230310
        public List<SelectOptionModel> GetTenderUnitList(int mode)
        {
            string sql = @"
                SELECT DISTINCT a.ExecUnitName Text, a.ExecUnitName Value
                ";
            if (mode == 1)
            {//所屬機關
                sql += @"
                    FROM PrjXML a
                    left outer join PrjXMLExt exta on(exta.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    left outer join EngMain b on(b.PrjXMLSeq=a.Seq)
                    left outer join SuperviseEng a1 on(a1.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230706
                    where a.TenderYear>106 --ISNULL(exta.ActualProgress,0)<100
                    and ISNULL(exta.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成' --20230706
                    "
                    + Utils.getAuthoritySqlForTender1("a.")
                    + @"
                    and exists (select Name from Unit where ParentSeq is null and Name=a.ExecUnitName) ";
            }
            else if (mode == 2)
            {//縣市政府
                sql += @"
                    FROM PrjXML a
                    inner join Country2WRAMapping aa on(aa.Country=substring(a.ExecUnitName,1,3) "
                    + getAuthoritySqlForTender("a.", "aa.RiverBureau") +
                    @")
                    left outer join PrjXMLExt exta on(exta.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    left outer join EngMain b on(b.PrjXMLSeq=a.Seq)
                    left outer join SuperviseEng a1 on(a1.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230706
                    where a.TenderYear>106 --ISNULL(exta.ActualProgress,0)<100
                    and ISNULL(exta.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成' --20230706
                    ";
            }
            else if (mode == 3)
            {//其他補助
                sql += @"
                    FROM PrjXML a
                    left outer join PrjXMLExt exta on(exta.PrjXMLSeq=a.Seq)
                    left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                    left outer join EngMain b on(b.PrjXMLSeq=a.Seq)
                    left outer join SuperviseEng a1 on(a1.PrjXMLSeq=a.Seq)
                    CROSS APPLY dbo.fPrjXMLProgress(a.Seq) d --20230706
                    where a.TenderYear>106 --ISNULL(exta.ActualProgress,0)<100
                    and ISNULL(exta.ActualCompletionDate,'')=''
                    and ISNULL(d.PDAccuActualProgress,0)<100 and d.PDExecState<>'已結案' and d.PDExecState<>'驗收完成' --20230706
                    "
                    + Utils.getAuthoritySqlForTender1("a.")
                    + @"
                    and not exists (select Name from Unit where ParentSeq is null and Name=a.ExecUnitName)
                    and not exists (select Country from Country2WRAMapping where Country=substring(a.ExecUnitName,1,3) ) ";
            }
            sql += @"
                order by a.ExecUnitName
                ";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<SelectOptionModel>(cmd);
        }
        private string getAuthoritySqlForTender(string alias, string fieldName)
        {
            string sql = " and 1=0";
            System.Web.SessionState.HttpSessionState _session = System.Web.HttpContext.Current.Session;
            if (_session["UserInfo"] == null) return sql;

            UserInfo userInfo = (UserInfo)_session["UserInfo"];
            if (userInfo.UnitSeq1 == null && userInfo.UnitSeq2 == null) return sql;

            if (userInfo.IsAdmin) return " ";//系統管理者

            //
            List<Role> roles = userInfo.Role;
            if (roles.Count == 0) return sql;

            int roleSeq = roles[0].Seq;

            if (roleSeq == ConfigManager.DepartmentAdmin_RoleSeq) //署管理者
            {
                sql = " ";
            }
            else if (roleSeq == ConfigManager.DepartmentUser_RoleSeq) //署使用者
            {
                if (userInfo.UnitName1 == "水利署")
                {
                    sql = " ";
                }
                else
                {
                    sql = String.Format(@" and (
                        {0}ExecUnitName like '{1}' or {0}PlanOrganizerName like '{1}' or {2} like '{1}' 
                    ) ", alias, "%" + userInfo.UnitName1, fieldName);
                }
            }
            return sql;
        }
        //期別工程清單
        public List<T> GetPhaseEngList<T>(int supervisePhaseSeq)
        {
            string sql = @"SELECT
                    a1.Seq,
                    a1.PrjXMLSeq,
                    a1.W1,
                    a1.W2,
                    a1.W3,
                    a1.W4,
                    a1.W5,
                    a1.W6,
                    a1.W7,
                    a1.W8,
                    a1.W9,
                    a1.W10,
                    a1.W11,
                    a1.W12,
                    a1.W13,
                    a1.W14,
                    a.TenderNo EngNo,
                    a.TenderName EngName,
                    a.ExecUnitName ExecUnit,
                    (
                        SELECT STUFF(
                        (SELECT ',' + z.RecDate
                        FROM (
                            select DISTINCT z1.RecDate from (
                                select cast(datepart(year, zc.ModifyTime)-1911 as varchar(4))+FORMAT(CONVERT(Date, zc.ModifyTime), '/M/d') RecDate from SuperviseEng za
                                inner join SuperviseFill zb on(zb.SuperviseEngSeq=za.Seq)
                                inner join SuperviseFillInsideCommittee zc on(zc.SuperviseFillSeq=zb.Seq)
                                where za.SupervisePhaseSeq=a1.SupervisePhaseSeq
                                and za.PrjXMLSeq=a1.PrjXMLSeq

                                union all

                                select cast(datepart(year, zc.ModifyTime)-1911 as varchar(4))+FORMAT(CONVERT(Date, zc.ModifyTime), '/M/d') RecDate from SuperviseEng za
                                inner join SuperviseFill zb on(zb.SuperviseEngSeq=za.Seq)
                                inner join SuperviseFillOutCommittee zc on(zc.SuperviseFillSeq=zb.Seq)
                                where za.SupervisePhaseSeq=a1.SupervisePhaseSeq
                                and za.PrjXMLSeq=a1.PrjXMLSeq
                            ) z1
                        ) z
                        order by z.RecDate
                        FOR XML PATH('')) ,1,1,'')
                    ) AS RecDate,
                    a2.PhaseCode 
                FROM SuperviseEng a1
                left join SupervisePhase a2 on a1.SupervisePhaseSeq = a2.Seq
                inner join PrjXML a on(a.Seq=a1.PrjXMLSeq)
                where a1.SupervisePhaseSeq=@SupervisePhaseSeq"
                + Utils.getAuthoritySqlForTender1("a.")
                + @"
                order by (a1.W1 + a1.W2 + a1.W3 + a1.W4 + a1.W5 + a1.W6 + a1.W7 + a1.W8 + a1.W9 + a1.W10 + a1.W11 + a1.W12 + a1.W13 + a1.W14) DESC, EngNo Desc
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SupervisePhaseSeq", supervisePhaseSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public int GetPhaseEngList1Count(int supervisePhaseSeq)
        {
            string sql = @"SELECT
                    count(a1.Seq) total
                FROM SuperviseEng a1
                inner join PrjXML a on(a.Seq=a1.PrjXMLSeq)
                left outer join viewPrjXMLPlaneWeakness b on(b.PrjXMLSeq=a.Seq)
                where a1.SupervisePhaseSeq=@SupervisePhaseSeq "
                ;
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SupervisePhaseSeq", supervisePhaseSeq);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());

            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetPhaseEngList1<T>(int supervisePhaseSeq, int pageRecordCount, int pageIndex)
        {
            string sql = @"SELECT
                    a1.Seq,
                    a1.PrjXMLSeq,
                    a.TenderNo EngNo,
                    a.TenderName EngName,
                    a.SupervisionUnitName,
                    a.DesignUnitName,
                    a.ExecUnitName ExecUnit,
                    NULLIF(
                        (select top 1 b.PDExecState from ProgressData b
                        where PrjXMLSeq=a.Seq
                        order by b.PDYear DESC, b.PDMonth DESC), '') ExecState, -- 執行進度
                    cast(IIF(
                        (IIF(a1.W1 <> ISNULL(b.w1,0),1,0)+
                        IIF(a1.W2 <> ISNULL(b.w2,0),1,0)+
                        IIF(a1.W3 <> ISNULL(b.w3,0),1,0)+
                        IIF(a1.W4 <> ISNULL(b.w4,0),1,0)+
                        IIF(a1.W5 <> ISNULL(b.w5,0),1,0)+
                        IIF(a1.W6 <> ISNULL(b.w6,0),1,0)+
                        IIF(a1.W7 <> ISNULL(b.w7,0),1,0)+
                        IIF(a1.W8 <> ISNULL(b.w8,0),1,0)+
                        IIF(a1.W9 <> ISNULL(b.w9,0),1,0)+
                        IIF(a1.W10 <> ISNULL(b.w10,0),1,0)+
                        IIF(a1.W11 <> ISNULL(b.w11,0),1,0)+
                        IIF(a1.W12 <> ISNULL(b.w12,0),1,0)+
                        IIF(a1.W13 <> ISNULL(b.w13,0),1,0)+
                        IIF(a1.W14 <> ISNULL(b.w14,0),1,0)
                    )>0, 1, 0) as bit) Updated
                FROM SuperviseEng a1
                inner join PrjXML a on(a.Seq=a1.PrjXMLSeq)
                left outer join viewPrjXMLPlaneWeakness b on(b.PrjXMLSeq=a.Seq)
                where a1.SupervisePhaseSeq=@SupervisePhaseSeq
                order by a.TenderNo Desc
				OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SupervisePhaseSeq", supervisePhaseSeq);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());

            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}