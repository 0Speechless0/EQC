using EQC.Common;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class PrjXMLService : BaseService
    {//標案主檔(從XML匯入)
        //新增工程 20230327
        public bool AddEng(PrjXMLModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into PrjXMLTmp(
                    TenderYear,
                    TenderNo,
                    TenderName,
                    ExecUnitName,
                    ExecUnitCd,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                )values(
                    @TenderYear,
                    @TenderNo,
                    @TenderName,
                    @ExecUnitName,
                    ISNULL((select top 1 ExecUnitCd from PrjXML where ExecUnitName=@ExecUnitName order by TenderYear desc),'未定義'),
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            try { 
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@TenderYear", m.TenderYear);
                cmd.Parameters.AddWithValue("@TenderNo", m.TenderNo);
                cmd.Parameters.AddWithValue("@TenderName", m.TenderName);
                cmd.Parameters.AddWithValue("@ExecUnitName", m.ExecUnitName);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                return db.ExecuteNonQuery(cmd)>0;
            }
            catch (Exception e)
            {
                log.Info("PrjXMLService.AddEng: " + e.Message);
                //log.Info(sql);
                return false;
            }
}
        //標案年分清單
        public List<SelectIntOptionModel> GetTenderYearList()
        {
            /*string sql = @"
                SELECT DISTINCT
                    cast(a.EngYear as integer) EngYear
                FROM EngMain a
                inner join SupervisionProjectList d on(d.EngMainSeq=a.Seq)
                where a.PrjXMLSeq is not null"//where a.CreateUserSeq=@CreateUserSeq
                + Utils.getAuthoritySql("a.")
                + @" order by EngYear DESC";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngYearVModel>(cmd);*/

            string sql = @"
                SELECT DISTINCT
                    cast(a.TenderYear as varchar(3)) Text,
                    cast(a.TenderYear as integer) Value
                FROM PrjXML a
                where 1=1"
                + Utils.getAuthoritySqlForTender1("a.")
                + "order by Value DESC";
            SqlCommand cmd = db.GetCommand(sql);
            //cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<SelectIntOptionModel>(cmd);
        }
        //標案執行機關清單
        public List<SelectOptionModel> GetTenderExecUnitList(string year)
        {
            /*string sql = @"
                SELECT DISTINCT
                    b.OrderNo,
                    a.ExecUnitSeq UnitSeq,
                    b.Name UnitName
                FROM EngMain a
                inner join SupervisionProjectList d on(d.EngMainSeq=a.Seq)            
                inner join Unit b on(b.Seq=a.ExecUnitSeq and b.parentSeq is null)
                where a.EngYear=@EngYear
                and a.PrjXMLSeq is not null"
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" order by b.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngYear", engYear);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngExecUnitsVModel>(cmd);*/

            string sql = @"
                SELECT DISTINCT
                    a.ExecUnitName Text,
                    a.ExecUnitName Value
                FROM PrjXML a
                where a.TenderYear=@TenderYear"
                + Utils.getAuthoritySqlForTender1("a.")
                + "order by Text";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@TenderYear", year);
            //cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<SelectOptionModel>(cmd);
        }

        //工程標案數量
        public int GetTenderListCount(string year, string execUnitName, string keyWord, ESQEngFilterVModel filterItem)
        {
            string sql = @"";
            sql = @"
                SELECT count(a.Seq) total FROM PrjXML a
                --left outer join EngMain b on(b.PrjXMLSeq=a.Seq)
                left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                where a.TenderYear=@TenderYear
                and a.ExecUnitName=@ExecUnitName
                and (@TenderName='' or a.TenderName like @TenderName)";
            if (filterItem.ActualStartDate)
                sql += @" and a.ActualStartDate !='' ";
            if (filterItem.W1)
                sql += @" and ISNULL(e.W1,0) > 0 ";
            if (filterItem.W2)
                sql += @" and ISNULL(e.W2,0) > 0 ";
            if (filterItem.W3)
                sql += @" and ISNULL(e.W3,0) > 0 ";
            if (filterItem.W4)
                sql += @" and ISNULL(e.W4,0) > 0 ";
            if (filterItem.W5)
                sql += @" and ISNULL(e.W5,0) > 0 ";
            if (filterItem.W6)
                sql += @" and ISNULL(e.W6,0) > 0 ";
            if (filterItem.W7)
                sql += @" and ISNULL(e.W7,0) > 0 ";
            if (filterItem.W8)
                sql += @" and ISNULL(e.W8,0) > 0 ";
            if (filterItem.W9)
                sql += @" and ISNULL(e.W9,0) > 0 ";
            if (filterItem.W10)
                sql += @" and ISNULL(e.W10,0) > 0 ";
            if (filterItem.W11)
                sql += @" and ISNULL(e.W11,0) > 0 ";
            if (filterItem.W12)
                sql += @" and ISNULL(e.W12,0) > 0 ";
            if (filterItem.W13)
                sql += @" and ISNULL(e.W13,0) > 0 ";
            if (filterItem.W14)
                sql += @" and ISNULL(e.W14,0) > 0 ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@TenderYear", year);
            cmd.Parameters.AddWithValue("@ExecUnitName", execUnitName);
            cmd.Parameters.AddWithValue("@TenderName", keyWord);

            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetTenderList<T>(string year, string execUnitName, string keyWord, int pageRecordCount, int pageIndex, ESQEngFilterVModel filterItem)
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
                    e.W14
                FROM PrjXML a
                left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                left outer join EngMain b on(b.PrjXMLSeq=a.Seq)
                where a.TenderYear=@TenderYear
                and a.ExecUnitName=@ExecUnitName
                and (@TenderName='' or a.TenderName like @TenderName)";
                if (filterItem.ActualStartDate)
                    sql += @" and a.ActualStartDate !='' ";
                if (filterItem.W1)
                    sql += @" and e.W1 > 0 ";
                if (filterItem.W2)
                    sql += @" and e.W2 > 0 ";
                if (filterItem.W3)
                    sql += @" and e.W3 > 0 ";
                if (filterItem.W4)
                    sql += @" and e.W4 > 0 ";
                if (filterItem.W5)
                    sql += @" and e.W5 > 0 ";
                if (filterItem.W6)
                    sql += @" and e.W6 > 0 ";
                if (filterItem.W7)
                    sql += @" and e.W7 > 0 ";
                if (filterItem.W8)
                    sql += @" and e.W8 > 0 ";
                if (filterItem.W9)
                    sql += @" and e.W9 > 0 ";
                if (filterItem.W10)
                    sql += @" and e.W10 > 0 ";
                if (filterItem.W11)
                    sql += @" and e.W11 > 0 ";
                if (filterItem.W12)
                    sql += @" and e.W12 > 0 ";
                if (filterItem.W13)
                    sql += @" and e.W13 > 0 ";
                if (filterItem.W14)
                    sql += @" and e.W14 > 0 ";
                sql += @"order by (e.W1+e.W2+e.W3+e.W4+e.W5+e.W6+e.W7+e.W8+e.W9+e.W10+e.W11+e.W12+e.W13+e.W14) DESC, a.TenderName Desc
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@TenderYear", year);
            cmd.Parameters.AddWithValue("@ExecUnitName", execUnitName);
            cmd.Parameters.AddWithValue("@TenderName", keyWord);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            //cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetPrjXMLPlaneWeaknessList<T>(string year, string execUnitName)
        {
            string sql = @"";
            sql = @"
                SELECT
                    a.Seq,
                    --a.TenderNo EngNo,
                    a.TenderName EngName,
                    e.Seq EngMainSeq, 
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
                    e.W14
                FROM PrjXML a
                left outer join EngMain b on(b.PrjXMLSeq=a.Seq)
                left outer join viewPrjXMLPlaneWeakness e on(e.PrjXMLSeq=a.Seq)
                where a.TenderYear=@TenderYear
                and a.ExecUnitName=@ExecUnitName
                order by (e.W1+e.W2+e.W3+e.W4+e.W5+e.W6+e.W7+e.W8+e.W9+e.W10+e.W11+e.W12+e.W13+e.W14) DESC, a.TenderName Desc
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@TenderYear", year);
            cmd.Parameters.AddWithValue("@ExecUnitName", execUnitName);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //由工程資料選取標案
        public List<T> GetTenderListByEng<T>(int engMainSeq)
        {
            string sql = @"
          select * from ( SELECT 
					1 code,
                    b.Seq,
                    b.TenderNo,
                    b.TenderName,  
                    c.PrjXMLSeq,
					a.Seq EngSeq
                FROM EngMain a
                inner join PrjXML b ON(
                    b.TenderYear=a.EngYear
                    and b.ExecUnitName=(select c.Name from unit c where c.Seq=a.ExecUnitSeq )
                )
				left outer join EngMain c on(c.PrjXMLSeq=b.Seq)
				union all
                SELECT 
					0 code,
                    b2.Seq,
                    b2.TenderNo,
                    b2.TenderName,  
                    c.PrjXMLSeq,
					a.Seq EngSeq
                FROM EngMain a
                inner join PrjXMLTmp b2 ON(
                    b2.TenderYear=a.EngYear
                    and b2.ExecUnitName=(select c.Name from unit c where c.Seq=a.ExecUnitSeq )
                )
				left outer join EngMain c on(c.PrjXMLSeq= -b2.Seq) 
				) z
				where z.EngSeq = @Seq 
				order by z.TenderName";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}