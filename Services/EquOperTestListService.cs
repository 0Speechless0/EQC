using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EquOperTestListService : BaseService
    {

        public List<T> GetList<T>(string engNo)
        {
            string sql = @"
                SELECT
                    es.*
                FROM EquOperTestList e
                inner join EngMain b on(b.Seq=e.EngMainSeq and b.EngNo=@EngNo)
                inner join EquOperControlSt es on es.EquOperTestStSeq = e.Seq
                --where DataType=1
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngNo", engNo);

            return db.GetDataTableWithClass<T>(cmd);
        }


        //設備運轉清單
        //僅使用者自訂項目 s20230302
        public List<T> GetListByEngNo<T>(string engNo)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.ItemName
                FROM EquOperTestList a
                inner join EngMain b on(b.Seq=a.EngMainSeq and b.EngNo=@EngNo)
                --where DataType=1
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngNo", engNo);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //複製流程圖,檢驗標準 s20230302
        public bool CopyFlowAndStItem(int srcSeq, int tarSeq)
        {
            string sql = @"";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql = @"
                    delete FlowChartTpDiagram where FlowChartTpSeq=@tarSeq and type like 'Chapter6%';

                    delete EquOperControlSt where EquOperTestStSeq=@tarSeq;

                    insert into FlowChartTpDiagram (FlowChartTpSeq, [Type], DiagramJson)
                    SELECT top 1 @tarSeq FlowChartTpSeq, 'Chapter6Addition', z.DiagramJson
                    from (
                        Select DiagramJson from FlowChartTpDiagram 
                        where FlowChartTpSeq=@srcSeq
                        and type like 'Chapter6%'

                        union all

                        Select DiagramJson from FlowChartTpDiagram 
                        where FlowChartTpSeq in (
                            select b.Seq from ConstCheckList a 
                            inner join ConstCheckListTp b on (
                                select top 1 value from string_split(a.FlowCharUniqueFileName,'.') ) = (select top 1 value from string_split(b.FlowCharUniqueFileName,'.')
                            ) 
                            where a.Seq = @srcSeq
                            and a.FlowCharUniqueFileName != '' 
                        )
                        and type like 'Chapter6%'
                    ) z;
                    /*Select top 1 @tarSeq FlowChartTpSeq, [Type], DiagramJson
                    from FlowChartTpDiagram 
                    where FlowChartTpSeq=@srcSeq
                    and type like 'Chapter6%';*/

                    insert into EquOperControlSt(
	                    EquOperTestStSeq,
                        DataType,
                        DataKeep,
                        EPCheckItem1,
                        EPCheckItem2,
                        EPStand1,
                        EPStand2,
                        EPStand3,
                        EPStand4,
                        EPStand5,
                        EPCheckTiming,
                        EPCheckMethod,
                        EPCheckFeq,
                        EPIncomp,
                        EPManageRec,
                        EPType,
                        EPMemo,
                        EPCheckFields,
                        EPManageFields,
                        OrderNo,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    )
                    SELECT
	                    @tarSeq,
                        b.DataType,
                        b.DataKeep,
                        b.EPCheckItem1,
                        b.EPCheckItem2,
                        b.EPStand1,
                        b.EPStand2,
                        b.EPStand3,
                        b.EPStand4,
                        b.EPStand5,
                        b.EPCheckTiming,
                        b.EPCheckMethod,
                        b.EPCheckFeq,
                        b.EPIncomp,
                        b.EPManageRec,
                        b.EPType,
                        b.EPMemo,
                        b.EPCheckFields,
                        b.EPManageFields,
                        b.OrderNo,
                        GETDATE(),
                        @ModifyUserSeq,
                        GETDATE(),
                        @ModifyUserSeq
                    FROM EquOperTestList a
                    inner join EquOperControlSt b on(b.EquOperTestStSeq=a.Seq)
                    where a.Seq=@srcSeq
                    order by b.OrderNo
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@srcSeq", srcSeq);
                cmd.Parameters.AddWithValue("@tarSeq", tarSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("EquOperTestListService.CopyFlowAndStItem: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
        public int GetListCount(int engMainSeq)
        {
            string sql = @"
                SELECT count(Seq) total FROM EquOperTestList where EngMainSeq=@EngMainSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            DataTable dt = db.GetDataTable(cmd);
            if (dt.Rows.Count == 1)
            {
                return Convert.ToInt32(dt.Rows[0]["total"].ToString());
            }
            else
            {
                return 0;
            }
        }
        public List<T> GetList<T>(int engMainSeq, int pageIndex, int perPage)
        {
            string sql = @"SELECT
                Seq,
                DataType,
                DataKeep,
                EPKind,
                OrderNo,
                ExcelNo,
                ItemName,
                ( --s20230302
	                IIF(len(ISNULL(FlowCharOriginFileName,''))=0,
                    (select top 1 [Type] from FlowChartTpDiagram where FlowChartTpSeq=EquOperTestList.Seq and [type] like 'Chapter6%'),
                    FlowCharOriginFileName)
                ) FlowCharOriginFileName,
                CreateTime,
                ModifyTime,
                (
                	SELECT count(z.Seq) total FROM EquOperControlSt z
                	where z.EquOperTestStSeq=EquOperTestList.Seq
                ) stdCount
                FROM EquOperTestList
                where EngMainSeq=@EngMainSeq
                order by DataKeep desc, OrderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //s20230629
        public List<T> GetListAll<T>(int engMainSeq)
        {
            string sql = @"SELECT
                Seq,
                EngMainSeq,
                DataType,
                DataKeep,
                EPKind,
                OrderNo,
                ExcelNo,
                ItemName,
                FlowCharOriginFileName,
                FlowCharUniqueFileName
                FROM EquOperTestList
                where EngMainSeq=@EngMainSeq
                order by OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public int Update(int seq, int orderNo, int epKind, string itemName, bool dataKeep)
        {
            //Null2Empty(m);
            string sql = @"
                update EquOperTestList set
                    OrderNo = @OrderNo,
                    EPKind = @EPKind,
                    ItemName = @ItemName,
                    DataKeep = @DataKeep,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@OrderNo", orderNo);
            cmd.Parameters.AddWithValue("@EPKind", epKind);
            cmd.Parameters.AddWithValue("@ItemName", this.NulltoEmpty(itemName));
            cmd.Parameters.AddWithValue("@DataKeep", dataKeep);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }

        public bool UpdateKeep(List<EquOperTestListVModel> items)
        {
            string sql = @"";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                //Null2Empty(m);
                sql = @"
                update EquOperTestList set
                    DataKeep = @DataKeep,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
                cmd = db.GetCommand(sql);

                int userSeq = getUserSeq();
                foreach (EquOperTestListVModel m in items)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@DataKeep", m.DataKeep);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                    cmd.Parameters.AddWithValue("@Seq", m.Seq);
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return true;
            } catch(Exception e)
            {
                db.TransactionRollback();
                log.Info("EquOperTestListService.UpdateKeep: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        public List<T> GetItemBySeq<T>(int seq)
        {
            string sql = @"SELECT
                Seq,
                DataType,
                DataKeep,
                EPKind,
                OrderNo,
                ExcelNo,
                ItemName,
                FlowCharOriginFileName,
                CreateTime,
                CreateTime,
                ModifyTime
                FROM EquOperTestList
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public List<EquOperTestListVModel> GetItemFileInfoBySeq(int seq)
        {
            string sql = @"SELECT
                Seq,
                FlowCharOriginFileName,
                FlowCharUniqueFileName
                FROM EquOperTestList
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<EquOperTestListVModel>(cmd);
        }

        public int UpdateUploadFile(int seq, string originFileName, string uniqueFileName)
        {
            string sql = @"
                update EquOperTestList set
                    FlowCharOriginFileName = @originFileName,
                    FlowCharUniqueFileName = @uniqueFileName,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@originFileName", originFileName);
            cmd.Parameters.AddWithValue("@uniqueFileName", uniqueFileName);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }

        public int Add(EquOperTestListModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into EquOperTestList (
                    DataType,
                    DataKeep,
                    EngMainSeq,
                    EPKind,
                    ExcelNo,
                    ItemName,
                    FlowCharOriginFileName,
                    FlowCharUniqueFileName,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @DataType,
                    @DataKeep,
                    @EngMainSeq,
                    @EPKind,
                    @ExcelNo,
                    @ItemName,
                    @FlowCharOriginFileName,
                    @FlowCharUniqueFileName,
                    (select max(OrderNo)+1 from EquOperTestList a where a.EngMainSeq=@EngMainSeq),
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@DataType", m.DataType);
            cmd.Parameters.AddWithValue("@DataKeep", m.DataKeep);
            cmd.Parameters.AddWithValue("@EngMainSeq", m.EngMainSeq);
            cmd.Parameters.AddWithValue("@EPKind", m.EPKind);
            cmd.Parameters.AddWithValue("@ExcelNo", m.ExcelNo);
            cmd.Parameters.AddWithValue("@ItemName", m.ItemName);
            cmd.Parameters.AddWithValue("@FlowCharOriginFileName", m.FlowCharOriginFileName);
            cmd.Parameters.AddWithValue("@FlowCharUniqueFileName", m.FlowCharUniqueFileName);
            //cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

            int result = db.ExecuteNonQuery(cmd);
            if (result == 0) return 0;

            cmd.Parameters.Clear();

            string sql1 = @"SELECT IDENT_CURRENT('EquOperTestList') AS NewSeq";
            cmd = db.GetCommand(sql1);
            DataTable dt = db.GetDataTable(cmd);
            m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
            return m.Seq;
        }

        public int Delete(int seq)
        {
            string sql = @"delete EquOperTestList where Seq=@Seq";

            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }
        /*public int Update(EquOperTestListVModel m)
        {
            Null2Empty(m);
            string sql = @"
                update EquOperTestList set
                    ExcelNo = @ExcelNo,
                    ItemName = @ItemName,
                    FlowCharOriginFileName = @FlowCharOriginFileName,
                    FlowCharUniqueFileName = @FlowCharUniqueFileName,
                    OrderNo = @OrderNo,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ExcelNo", m.ExcelNo);
            cmd.Parameters.AddWithValue("@ItemName", m.ItemName);
            cmd.Parameters.AddWithValue("@FlowCharOriginFileName", m.FlowCharOriginFileName);
            cmd.Parameters.AddWithValue("@FlowCharUniqueFileName", m.FlowCharUniqueFileName);
            cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@Seq", m.Seq);

            return db.ExecuteNonQuery(cmd);
        }
        
        */
    }
}