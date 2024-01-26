using System.Collections.Generic;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class ConstCheckRecResultService : BaseService
    {//抽驗紀錄填報紀錄
        //前次施工抽查清單 20230302
        public List<T> GetConstCheckRecResultHistory<T>(int constCheckRecSeq)
        {
            string sql = @"
                SELECT
                    a.Seq constCheckRecSeq,
                    c.CCManageItem1 CheckItem1,
                    c.CCManageItem2 CheckItem2,
                    c.CCCheckStand1 Stand1,
                    c.CCCheckStand2 Stand2,
                    '' Stand3,
                    '' Stand4,
                    '' Stand5,
                    c.CCCheckFields CheckFields,
                    b.Seq,
                    b.ConstCheckRecSeq,
                    b.ControllStSeq,
                    b.CCRRealCheckCond,
                    b.CCRCheckResult,
                    b.CCRIsNCR,
                    b.ResultItem
                FROM ConstCheckRec a
                inner join ConstCheckRecResult b ON(b.ConstCheckRecSeq=a.Seq)
                inner join ConstCheckControlSt c on(c.Seq=b.ControllStSeq)
                where a.Seq=(
                    SELECT top 1 a.Seq
                    FROM ConstCheckRec a
                    inner join ConstCheckRec a1 on(a1.Seq=@Seq and a1.EngConstructionSeq=a.EngConstructionSeq
    	                and a1.CCRCheckType1=a.CCRCheckType1 and a1.ItemSeq=a.ItemSeq and a1.CCRCheckFlow=a.CCRCheckFlow)
                    where a.Seq <> @Seq
                    order by a.CCRCheckDate desc
                )
                and a.CCRCheckDate <=(
	                SELECT CCRCheckDate FROM ConstCheckRec where Seq = @Seq
                )
                order by c.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", constCheckRecSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //施工抽查清單
        public List<T> GetConstCheckRecResult<T>(int constCheckRecSeq)
        {
            string sql = @"
                SELECT
                    a.Seq constCheckRecSeq,
                    a.FormConfirm, 
                    c.CCManageItem1 CheckItem1,
                    c.CCManageItem2 CheckItem2,
                    c.CCCheckStand1 Stand1,
                    c.CCCheckStand2 Stand2,
                    '' Stand3,
                    '' Stand4,
                    '' Stand5,
                    c.CCCheckFields CheckFields,
                    c.CCType itemType, --s20231016
                    b.Seq,
                    b.ConstCheckRecSeq,
                    b.ControllStSeq,
                    b.CCRRealCheckCond,
                    b.CCRCheckResult,
                    b.CCRIsNCR,
                    b.RecResultRemark,
                    b.ResultItem
                FROM ConstCheckRec a
                inner join ConstCheckRecResult b ON(b.ConstCheckRecSeq=a.Seq)
                inner join ConstCheckControlSt c on(c.Seq=b.ControllStSeq)
                where a.Seq=@Seq
                order by c.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", constCheckRecSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //前次設備運轉測試紀錄 20230907
        public List<T> GetEquOperRecResultHistory<T>(int constCheckRecSeq)
        {
            string sql = @"
                SELECT
                    a.Seq constCheckRecSeq,
                    a.FormConfirm, 
                    c.EPCheckItem1 CheckItem1,
                    c.EPCheckItem2 CheckItem2,
                    c.EPStand1 Stand1,
                    c.EPStand2 Stand2,
                    c.EPStand3 Stand3,
                    c.EPStand4 Stand4,
                    c.EPStand5 Stand5,
                    c.EPCheckFields CheckFields,
                    b.Seq,
                    b.ConstCheckRecSeq,
                    b.ControllStSeq,
                    b.CCRRealCheckCond,
                    b.CCRCheckResult,
                    b.CCRIsNCR,
                    b.ResultItem
                FROM ConstCheckRec a
                inner join ConstCheckRecResult b ON(b.ConstCheckRecSeq=a.Seq)
                inner join EquOperControlSt c on(c.Seq=b.ControllStSeq)
                where a.Seq=(
                    SELECT top 1 a.Seq
                    FROM ConstCheckRec a
                    inner join ConstCheckRec a1 on(a1.Seq=@Seq and a1.EngConstructionSeq=a.EngConstructionSeq
    	                and a1.CCRCheckType1=a.CCRCheckType1 and a1.ItemSeq=a.ItemSeq and a1.CCRCheckFlow=a.CCRCheckFlow)
                    where a.Seq <> @Seq
                    order by a.CCRCheckDate desc
                )
                and a.CCRCheckDate <=(
	                SELECT CCRCheckDate FROM ConstCheckRec where Seq = @Seq
                )
                order by c.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", constCheckRecSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //設備運轉測試清單
        public List<T> GetEquOperRecResult<T>(int constCheckRecSeq)
        {
            string sql = @"
                SELECT
                    a.Seq constCheckRecSeq,
                    a.FormConfirm, 
                    c.EPCheckItem1 CheckItem1,
                    c.EPCheckItem2 CheckItem2,
                    c.EPStand1 Stand1,
                    c.EPStand2 Stand2,
                    c.EPStand3 Stand3,
                    c.EPStand4 Stand4,
                    c.EPStand5 Stand5,
                    c.EPCheckFields CheckFields,
                    c.EPType itemType, --s20231016
                    b.Seq,
                    b.ConstCheckRecSeq,
                    b.ControllStSeq,
                    b.CCRRealCheckCond,
                    b.CCRCheckResult,
                    b.RecResultRemark,
                    b.CCRIsNCR,
                    b.ResultItem
                FROM ConstCheckRec a
                inner join ConstCheckRecResult b ON(b.ConstCheckRecSeq=a.Seq)
                inner join EquOperControlSt c on(c.Seq=b.ControllStSeq)
                where a.Seq=@Seq
                order by c.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", constCheckRecSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //前次職業安全衛生紀錄 s20230907
        public List<T> GetOccuSafeHealthRecResultHistory<T>(int constCheckRecSeq)
        {
            string sql = @"
                SELECT
                    a.Seq constCheckRecSeq,
                    a.FormConfirm, 
                    c.OSCheckItem1 CheckItem1,
                    c.OSCheckItem2 CheckItem2,
                    c.OSStand1 Stand1,
                    c.OSStand2 Stand2,
                    c.OSStand3 Stand3,
                    c.OSStand4 Stand4,
                    c.OSStand5 Stand5,
                    c.OSCheckFields CheckFields,
                    b.Seq,
                    b.ConstCheckRecSeq,
                    b.ControllStSeq,
                    b.CCRRealCheckCond,
                    b.CCRCheckResult,
                    b.CCRIsNCR,
                    b.ResultItem
                FROM ConstCheckRec a
                inner join ConstCheckRecResult b ON(b.ConstCheckRecSeq=a.Seq)
                inner join OccuSafeHealthControlSt c on(c.Seq=b.ControllStSeq)
                where a.Seq=(
                    SELECT top 1 a.Seq
                    FROM ConstCheckRec a
                    inner join ConstCheckRec a1 on(a1.Seq=@Seq and a1.EngConstructionSeq=a.EngConstructionSeq
    	                and a1.CCRCheckType1=a.CCRCheckType1 and a1.ItemSeq=a.ItemSeq and a1.CCRCheckFlow=a.CCRCheckFlow)
                    where a.Seq <> @Seq
                    order by a.CCRCheckDate desc
                )
                and a.CCRCheckDate <=(
	                SELECT CCRCheckDate FROM ConstCheckRec where Seq = @Seq
                )
                order by c.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", constCheckRecSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //職業安全衛生清單
        public List<T> GetOccuSafeHealthRecResult<T>(int constCheckRecSeq)
        {
            string sql = @"
                SELECT
                    a.Seq constCheckRecSeq,
                    a.FormConfirm, 
                    c.OSCheckItem1 CheckItem1,
                    c.OSCheckItem2 CheckItem2,
                    c.OSStand1 Stand1,
                    c.OSStand2 Stand2,
                    c.OSStand3 Stand3,
                    c.OSStand4 Stand4,
                    c.OSStand5 Stand5,
                    c.OSCheckFields CheckFields,
                    c.OSType itemType, --s20231016
                    b.Seq,
                    b.ConstCheckRecSeq,
                    b.ControllStSeq,
                    b.CCRRealCheckCond,
                    b.CCRCheckResult,
                    b.RecResultRemark,
                    b.CCRIsNCR,
                    b.ResultItem
                FROM ConstCheckRec a
                inner join ConstCheckRecResult b ON(b.ConstCheckRecSeq=a.Seq)
                inner join OccuSafeHealthControlSt c on(c.Seq=b.ControllStSeq)
                where a.Seq=@Seq
                order by c.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", constCheckRecSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //前次環境保育紀錄 s20230907
        public List<T> GetEnvirConsRecResultHistory<T>(int constCheckRecSeq)
        {
            string sql = @"
                SELECT
                    a.Seq constCheckRecSeq,
                    a.FormConfirm, 
                    c.ECCCheckItem1 CheckItem1,
                    c.ECCCheckItem2 CheckItem2,
                    c.ECCStand1 Stand1,
                    c.ECCStand2 Stand2,
                    c.ECCStand3 Stand3,
                    c.ECCStand4 Stand4,
                    c.ECCStand5 Stand5,
                    c.ECCCheckFields CheckFields,
                    b.Seq,
                    b.ConstCheckRecSeq,
                    b.ControllStSeq,
                    b.CCRRealCheckCond,
                    b.CCRCheckResult,
                    b.CCRIsNCR,
                    b.ResultItem
                FROM ConstCheckRec a
                inner join ConstCheckRecResult b ON(b.ConstCheckRecSeq=a.Seq)
                inner join EnvirConsControlSt c on(c.Seq=b.ControllStSeq)
                where a.Seq=(
                    SELECT top 1 a.Seq
                    FROM ConstCheckRec a
                    inner join ConstCheckRec a1 on(a1.Seq=@Seq and a1.EngConstructionSeq=a.EngConstructionSeq
    	                and a1.CCRCheckType1=a.CCRCheckType1 and a1.ItemSeq=a.ItemSeq and a1.CCRCheckFlow=a.CCRCheckFlow)
                    where a.Seq <> @Seq
                    order by a.CCRCheckDate desc
                )
                and a.CCRCheckDate <=(
	                SELECT CCRCheckDate FROM ConstCheckRec where Seq = @Seq
                )
                order by c.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", constCheckRecSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //環境保育清單
        public List<T> GetEnvirConsRecResult<T>(int constCheckRecSeq)
        {
            string sql = @"
                SELECT
                    a.Seq constCheckRecSeq,
                    a.FormConfirm, 
                    c.ECCCheckItem1 CheckItem1,
                    c.ECCCheckItem2 CheckItem2,
                    c.ECCStand1 Stand1,
                    c.ECCStand2 Stand2,
                    c.ECCStand3 Stand3,
                    c.ECCStand4 Stand4,
                    c.ECCStand5 Stand5,
                    c.ECCCheckFields CheckFields,
                    c.ECCType itemType, --s20231016
                    b.Seq,
                    b.ConstCheckRecSeq,
                    b.ControllStSeq,
                    b.CCRRealCheckCond,
                    b.CCRCheckResult,
                    b.RecResultRemark,
                    b.CCRIsNCR,
                    b.ResultItem
                FROM ConstCheckRec a
                inner join ConstCheckRecResult b ON(b.ConstCheckRecSeq=a.Seq)
                inner join EnvirConsControlSt c on(c.Seq=b.ControllStSeq)
                where a.Seq=@Seq
                order by c.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", constCheckRecSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}