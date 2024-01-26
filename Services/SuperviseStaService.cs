using EQC.Common;
using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class SuperviseStaService : BaseService
    {//督導統計
        //統計
        public List<T> GetStaList<T>(int docNo, DateTime sDate, DateTime eDate)
        {
            string sql = @"
                select top 50 z1.MissingNo, z1.Content, z1.MissingCnt, z.Total, cast(z1.MissingCnt*100/z.Total as decimal(18,2)) MissingRate
                from (
                    select cast(count(d1.Seq) as decimal(18,2)) Total from SuperviseEng d1
                    inner join SuperviseFill d2 on(d2.SuperviseEngSeq=d1.Seq and d2.MissingNo like @docNo)
                    where d1.SuperviseDate >=@sDate
                    and d1.SuperviseDate <=@eDate
                ) z
                inner join (
                    select b.MissingNo, c.Content, cast(COUNT(b.MissingNo) as decimal(18,2)) MissingCnt
                    from SuperviseEng a
                    inner join SuperviseFill b on(b.SuperviseEngSeq=a.Seq and b.MissingNo like @docNo)
                    left outer join QualityDeductionPoints c ON(c.MissingNo=b.MissingNo)
                    where a.SuperviseDate >=@sDate
                    and a.SuperviseDate <=@eDate
                    group by b.MissingNo, c.Content 
                ) z1 ON(1=1)
                order by z1.MissingNo
				";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@docNo", (docNo==1) ? "4%" :"5%");
            cmd.Parameters.AddWithValue("@sDate", sDate);
            cmd.Parameters.AddWithValue("@eDate", eDate);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //外聘委員 清單與查詢
        public List<T> GetOutCommitte<T>(int superviseEngSeq)
        {
            string sql = @"
                select a.OrderNo,
                    a.Seq,
	                b.ECName,
                    b.ECId,
                    b.ECAddr1,
                    b.ECBankNo,
                    b.ECMemo
                from OutCommittee a
                inner join ExpertCommittee b on(b.Seq=a.ExpertCommitteeSeq)
                where a.SuperviseEngSeq=@SuperviseEngSeq
                order by a.OrderNo
				";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);

            return db.GetDataTableWithClass<T>(cmd);
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
                    b.SuperviseDate,
                    b.SuperviseEndDate, --s20230316
                    b.BriefingPlace,
                    b.BriefingAddr,
                    b.IsVehicleDisp,
                    b.IsTHSR,
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
                    b.SuperviseMode,
                    a.TenderNo EngNo,
                    a.TenderName EngName,
                    a2.BelongPrj,
                    b1.PhaseCode,
                    a.Location EngPlace,
                    a.ExecUnitName
                FROM SuperviseEng b
                inner join SupervisePhase b1 on(b1.Seq=b.SupervisePhaseSeq)
                inner join PrjXML a on(a.Seq=b.PrjXMLSeq)
                left outer join PrjXMLExt a2 on(a2.PrjXMLSeq=a.Seq)
                where b.Seq=@Seq"
                + Utils.getAuthoritySql("a.");
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //查詢督導期別
        public List<SupervisePhaseModel> GetPhaseCode(string phaseCode)
        {
            string sql = @"
                SELECT Seq, PhaseCode FROM SupervisePhase
                where PhaseCode=@PhaseCode ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@PhaseCode", phaseCode);
            return db.GetDataTableWithClass<SupervisePhaseModel>(cmd);
        }
        //期別工程清單
        public int GetPhaseEngList1Count(int supervisePhaseSeq)
        {
            string sql = @"SELECT
                    count(a1.Seq) total
                FROM SuperviseEng a1
                inner join PrjXML a on(a.Seq=a1.PrjXMLSeq)
                --inner join Unit b on(b.Seq=a.ExecUnitSeq)
                where a1.SupervisePhaseSeq=@SupervisePhaseSeq"
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
                    a.TenderNo EngName,
                    a.SupervisionUnitName,
                    a.DesignUnitName,
                    a.ExecUnitName ExecUnit,
                    NULLIF(
                        (select top 1 b.PDExecState from ProgressData b
                        where PrjXMLSeq=a.Seq
                        order by b.PDYear DESC, b.PDMonth DESC), '') ExecState -- 執行進度
                FROM SuperviseEng a1
                inner join PrjXML a on(a.Seq=a1.PrjXMLSeq)
                -- inner join Unit b on(b.Seq=a.ExecUnitSeq)
                where a1.SupervisePhaseSeq=@SupervisePhaseSeq
                order by a.EngNo Desc
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