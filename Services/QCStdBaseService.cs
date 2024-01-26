using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class QCStdBaseService: BaseService
    {
        //703 職業安全衛生抽查標準範本 s20230415
        public int ImportChapter703StdList(int occuSafeHealthListSeq, List<QCStdModel> items)
        {
            string sql;
            db.BeginTransaction();
            try
            {
                sql = @"
                delete from OccuSafeHealthControlTp where OccuSafeHealthListSeq=@Seq
                ";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", occuSafeHealthListSeq);
                db.ExecuteNonQuery(cmd);

                sql = @"
                insert into OccuSafeHealthControlTp (
                    OccuSafeHealthListSeq,
                    OSCheckItem1,
                    OSStand1,
                    OSCheckTiming,
                    OSCheckMethod,
                    OSCheckFeq,
                    OSIncomp,
                    OSManageRec,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @OccuSafeHealthListSeq,
                    @OSCheckItem1,
                    @OSStand1,
                    @OSCheckTiming,
                    @OSCheckMethod,
                    @OSCheckFeq,
                    @OSIncomp,
                    @OSManageRec,
                    @OrderNo,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
                int orderNo = 1;
                foreach (QCStdModel m in items)
                {
                    this.Null2Empty(m);

                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@OccuSafeHealthListSeq", occuSafeHealthListSeq);
                    cmd.Parameters.AddWithValue("@OSCheckItem1", m.ManageItem);
                    cmd.Parameters.AddWithValue("@OSStand1", m.Stand);
                    cmd.Parameters.AddWithValue("@OSCheckTiming", m.CheckTiming);
                    cmd.Parameters.AddWithValue("@OSCheckMethod", m.CheckMethod);
                    cmd.Parameters.AddWithValue("@OSCheckFeq", m.CheckFeq);
                    cmd.Parameters.AddWithValue("@OSIncomp", m.Incomp);
                    cmd.Parameters.AddWithValue("@OSManageRec", m.ManageRec);
                    cmd.Parameters.AddWithValue("@OrderNo", orderNo++);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                    int result = db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("OccuSafeHealthControlStService.ImportChapter703StdList: " + e.Message);
                return -1;
            }
        }
        public List<T> GetChapter703StdList<T>(int emdSeq)
        {
            string sql = @"
                SELECT
                    e2.OSCheckItem1 ManageItem,
                    e2.OSStand1 Stand,
                    e2.OSCheckTiming CheckTiming,
                    e2.OSCheckMethod CheckMethod,
                    e2.OSCheckFeq CheckFeq,
                    e2.OSIncomp Incomp,
                    e2.OSManageRec ManageRec
                FROM OccuSafeHealthControlTp e2
                inner join OccuSafeHealthListTp e1 on e1.Seq = e2.OccuSafeHealthListSeq 
                where OccuSafeHealthListSeq=@OccuSafeHealthListSeq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@OccuSafeHealthListSeq", emdSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        
        //702 環境保育抽查標準範本 s20230415
        public int ImportChapter702StdList(int envirConsListSeq, List<QCStdModel> items)
        {
            string sql;
            db.BeginTransaction();
            try
            {
                sql = @"
                delete from EnvirConsControlTp where EnvirConsListSeq=@Seq
                ";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", envirConsListSeq);
                db.ExecuteNonQuery(cmd);

                sql = @"
                insert into EnvirConsControlTp (
                    EnvirConsListSeq,
                    ECCFlow1,
                    ECCCheckItem1,
                    ECCStand1,
                    ECCCheckTiming,
                    ECCCheckMethod,
                    ECCCheckFeq,
                    ECCIncomp,
                    ECCManageRec,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @EnvirConsListSeq,
                    @ECCFlow1,
                    @ECCCheckItem1,
                    @ECCStand1,
                    @ECCCheckTiming,
                    @ECCCheckMethod,
                    @ECCCheckFeq,
                    @ECCIncomp,
                    @ECCManageRec,
                    @OrderNo,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
                int orderNo = 1;
                foreach (QCStdModel m in items)
                {
                    this.Null2Empty(m);

                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EnvirConsListSeq", envirConsListSeq);
                    cmd.Parameters.AddWithValue("@ECCFlow1", m.FlowType);
                    cmd.Parameters.AddWithValue("@ECCCheckItem1", m.ManageItem);
                    cmd.Parameters.AddWithValue("@ECCStand1", m.Stand);
                    cmd.Parameters.AddWithValue("@ECCCheckTiming", m.CheckTiming);
                    cmd.Parameters.AddWithValue("@ECCCheckMethod", m.CheckMethod);
                    cmd.Parameters.AddWithValue("@ECCCheckFeq", m.CheckFeq);
                    cmd.Parameters.AddWithValue("@ECCIncomp", m.Incomp);
                    cmd.Parameters.AddWithValue("@ECCManageRec", m.ManageRec);
                    cmd.Parameters.AddWithValue("@OrderNo", orderNo++);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                    int result = db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("EnvirConsControlStService.ImportChapter702StdList: " + e.Message);
                return -1;
            }
        }
        public List<T> GetChapter702StdList<T>(int emdSeq)
        {
            string sql = @"
                SELECT
                    e2.ECCFlow1 FlowType,
                    e2.ECCCheckItem1 ManageItem,
                    e2.ECCStand1 Stand,
                    e2.ECCCheckTiming CheckTiming,
                    e2.ECCCheckMethod CheckMethod,
                    e2.ECCCheckFeq CheckFeq,
                    e2.ECCIncomp Incomp,
                    e2.ECCManageRec ManageRec
                FROM EnvirConsControlTp e2
                inner join EnvirConsList e1 on e1.Seq = e2.EnvirConsListSeq
                where EnvirConsListSeq=@EnvirConsListSeq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EnvirConsListSeq", emdSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        
        //701 施工抽查標準範本 s20230415
        public int ImportChapter701StdList(int constCheckListTpSeq, List<QCStdModel> items)
        {
            string sql;
            db.BeginTransaction();
            try
            {
                sql = @"
                delete from ConstCheckControlTp where ConstCheckListTpSeq=@Seq
                ";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", constCheckListTpSeq);
                db.ExecuteNonQuery(cmd);

                sql = @"
                insert into ConstCheckControlTp (
                    ConstCheckListTpSeq,
                    CCFlow1,
                    CCFlow2,
                    CCManageItem1,
                    CCCheckStand1,
                    CCCheckTiming,
                    CCCheckMethod,
                    CCCheckFeq,
                    CCIncomp,
                    CCManageRec,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @ConstCheckListTpSeq,
                    @CCFlow1,
                    @CCFlow2,
                    @CCManageItem1,
                    @CCCheckStand1,
                    @CCCheckTiming,
                    @CCCheckMethod,
                    @CCCheckFeq,
                    @CCIncomp,
                    @CCManageRec,
                    @OrderNo,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
                int orderNo = 1;
                foreach (QCStdModel m in items)
                {
                    this.Null2Empty(m);

                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@ConstCheckListTpSeq", constCheckListTpSeq);
                    cmd.Parameters.AddWithValue("@CCFlow1", m.FlowType);
                    cmd.Parameters.AddWithValue("@CCFlow2", m.Flow);
                    cmd.Parameters.AddWithValue("@CCManageItem1", m.ManageItem);
                    cmd.Parameters.AddWithValue("@CCCheckStand1", m.Stand);
                    cmd.Parameters.AddWithValue("@CCCheckTiming", m.CheckTiming);
                    cmd.Parameters.AddWithValue("@CCCheckMethod", m.CheckMethod);
                    cmd.Parameters.AddWithValue("@CCCheckFeq", m.CheckFeq);
                    cmd.Parameters.AddWithValue("@CCIncomp", m.Incomp);
                    cmd.Parameters.AddWithValue("@CCManageRec", m.ManageRec);
                    cmd.Parameters.AddWithValue("@OrderNo", orderNo++);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                    int result = db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("QCStdBaseService.ImportChapter701StdList: " + e.Message);
                return -1;
            }
        }
        public List<T> GetChapter701StdList<T>(int emdSeq)
        {
            string sql = @"
                SELECT
                    e2.CCFlow1 FlowType,
                    e2.CCFlow2 Flow,
                    e2.CCManageItem1 ManageItem,
                    e2.CCCheckStand1 Stand,
                    e2.CCCheckTiming CheckTiming,
                    e2.CCCheckMethod CheckMethod,
                    e2.CCCheckFeq CheckFeq,
                    e2.CCIncomp Incomp,
                    e2.CCManageRec ManageRec
                FROM ConstCheckControlTp e2
                inner join ConstCheckListTp e1 on e2.ConstCheckListTpSeq  = e1.Seq
                where e2.ConstCheckListTpSeq=@ConstCheckListTpSeq
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ConstCheckListTpSeq", emdSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        
        //6 設備運轉抽查標準範本 s20230415
        public int ImportChapter6StdList(int equOperTestTpSeq, List<QCStdModel> items)
        {
            string sql;
            db.BeginTransaction();
            try
            {
                sql = @"
                delete from EquOperControlTp where EquOperTestTpSeq=@Seq
                ";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", equOperTestTpSeq);
                db.ExecuteNonQuery(cmd);

                sql = @"
                insert into EquOperControlTp (
                    EquOperTestTpSeq,
                    EPCheckItem1,
                    EPStand1,
                    EPCheckTiming,
                    EPCheckMethod,
                    EPCheckFeq,
                    EPIncomp,
                    EPManageRec,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @EquOperTestTpSeq,
                    @EPCheckItem1,
                    @EPStand1,
                    @EPCheckTiming,
                    @EPCheckMethod,
                    @EPCheckFeq,
                    @EPIncomp,
                    @EPManageRec,
                    @OrderNo,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
                int orderNo = 1;
                foreach (QCStdModel m in items)
                {
                    this.Null2Empty(m);

                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EquOperTestTpSeq", equOperTestTpSeq);
                    cmd.Parameters.AddWithValue("@EPCheckItem1", m.ManageItem);
                    cmd.Parameters.AddWithValue("@EPStand1", m.Stand);
                    cmd.Parameters.AddWithValue("@EPCheckTiming", m.CheckTiming);
                    cmd.Parameters.AddWithValue("@EPCheckMethod", m.CheckMethod);
                    cmd.Parameters.AddWithValue("@EPCheckFeq", m.CheckFeq);
                    cmd.Parameters.AddWithValue("@EPIncomp", m.Incomp);
                    cmd.Parameters.AddWithValue("@EPManageRec", m.ManageRec);
                    cmd.Parameters.AddWithValue("@OrderNo", orderNo++);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                    int result = db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("QCStdBaseService.ImportChapter6StdList: " + e.Message);
                return -1;
            }
        }
        public List<T> GetChapter6StdList<T>(int emdSeq)
        {
            string sql = @"
                SELECT
                    e2.EPCheckItem1 ManageItem,
                    e2.EPStand1 Stand,
                    e2.EPCheckTiming CheckTiming,
                    e2.EPCheckMethod CheckMethod,
                    e2.EPCheckFeq CheckFeq,
                    e2.EPIncomp Incomp,
                    e2.EPManageRec ManageRec
                FROM EquOperControlTp e2
                inner join EquOperTestListTp e1 on e1.Seq = e2.EquOperTestTpSeq
                where e2.EquOperTestTpSeq=@EquOperTestTpSeq
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EquOperTestTpSeq", emdSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        
        //5 材料設備抽查管理標準範本 s20230415
        public int ImportChapter5StdList(int engMaterialDeviceListTpSeq, List<QCStdModel> items)
        {
            string sql;
            db.BeginTransaction();
            try
            {
                sql = @"
                delete from EngMaterialDeviceControlTp where EngMaterialDeviceListTpSeq=@Seq
                ";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", engMaterialDeviceListTpSeq);
                db.ExecuteNonQuery(cmd);

                sql = @"
                insert into EngMaterialDeviceControlTp (
                    EngMaterialDeviceListTpSeq,
                    MDTestItem,
                    MDTestStand1,
                    MDTestTime,
                    MDTestMethod,
                    MDTestFeq,
                    MDIncomp,
                    MDManageRec,
                    MDMemo,
                    OrderNo,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @EngMaterialDeviceListTpSeq,
                    @MDTestItem,
                    @MDTestStand1,
                    @MDTestTime,
                    @MDTestMethod,
                    @MDTestFeq,
                    @MDIncomp,
                    @MDManageRec,
                    @MDMemo,
                    @OrderNo,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
                int orderNo = 1;
                foreach (QCStdModel m in items)
                {
                    this.Null2Empty(m);

                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EngMaterialDeviceListTpSeq", engMaterialDeviceListTpSeq);
                    cmd.Parameters.AddWithValue("@MDTestItem", m.ManageItem);
                    cmd.Parameters.AddWithValue("@MDTestStand1", m.Stand);
                    cmd.Parameters.AddWithValue("@MDTestTime", m.CheckTiming);
                    cmd.Parameters.AddWithValue("@MDTestMethod", m.CheckMethod);
                    cmd.Parameters.AddWithValue("@MDTestFeq", m.CheckFeq);
                    cmd.Parameters.AddWithValue("@MDIncomp", m.Incomp);
                    cmd.Parameters.AddWithValue("@MDManageRec", m.ManageRec);
                    cmd.Parameters.AddWithValue("@MDMemo", m.Memo);
                    cmd.Parameters.AddWithValue("@OrderNo", orderNo++);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                    int result = db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("QCStdBaseService.ImportChapter5StdList: " + e.Message);
                return -1;
            }
        }
        public List<T> GetChapter5StdList<T>(int emdSeq)
        {
            string sql = @"
                SELECT
                    ec.MDTestItem ManageItem,
                    ec.MDTestStand1 Stand,
                    ec.MDTestTime CheckTiming,
                    ec.MDTestMethod CheckMethod,
                    ec.MDTestFeq CheckFeq,
                    ec.MDIncomp Incomp,
                    ec.MDManageRec ManageRec,
                    ec.MDMemo Memo
                FROM EngMaterialDeviceControlTp ec
                inner join EngMaterialDeviceListTp el on ec.EngMaterialDeviceListTpSeq = el.Seq
                where EngMaterialDeviceListTpSeq=@EngMaterialDeviceListTpSeq
                order by ec.OrderNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMaterialDeviceListTpSeq", emdSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //第五章 材料設備清冊範本
        public List<T> GetEngMaterialDeviceListTpList<T>()
        {
            string sql = @"SELECT Seq, MDName FROM EngMaterialDeviceListTp order by OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<T>(cmd);
        }

        public int GetEngMaterialDeviceControlTpCount(string emdSeq, string code="")
        {
            string sql = @"
                SELECT count(el.Seq) total FROM EngMaterialDeviceControlTp ec
                inner join  EngMaterialDeviceListTp el on el.Seq = ec.EngMaterialDeviceListTpSeq
                where 
                ec.EngMaterialDeviceListTpSeq=@EngMaterialDeviceListTpSeq
                or(@EngMaterialDeviceListTpSeq = 0 and el.ExcelNo = @Code)";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMaterialDeviceListTpSeq", emdSeq);
            cmd.Parameters.AddWithValue("@Code", code);
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
        //材料設備抽查管理標準範本
        public List<T> GetEngMaterialDeviceControlTpByEngMaterialDeviceListTpSeq<T>(string emdSeq, int pageIndex, int perPage, string code= "")
        {
            string sql = @"
                SELECT
                    ec.Seq,
                    ec.EngMaterialDeviceListTpSeq,
                    ec.MDTestItem,
                    ec.MDTestStand1,
                    ec.MDTestStand2,
                    ec.MDTestTime,
                    ec.MDTestMethod,
                    ec.MDTestFeq,
                    ec.MDIncomp,
                    ec.MDManageRec,
                    ec.MDMemo,
                    ec.OrderNo
                FROM EngMaterialDeviceControlTp ec
                inner join EngMaterialDeviceListTp el on ec.EngMaterialDeviceListTpSeq = el.Seq
                where EngMaterialDeviceListTpSeq=@EngMaterialDeviceListTpSeq
                or(@EngMaterialDeviceListTpSeq = 0  and el.ExcelNo = @Code )
                order by OrderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMaterialDeviceListTpSeq", emdSeq);
            cmd.Parameters.AddWithValue("@Code", code);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //第六章 設備運轉測試清單範本
        public List<T> GetEquOperTestListTpList<T>()
        {
            string sql = @"SELECT Seq,ItemName FROM EquOperTestListTp order by OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<T>(cmd);
        }

        public int GetEquOperControlTpCount(string emdSeq, string code = "")
        {
            string sql = @"
                SELECT count(e2.Seq) total FROM EquOperControlTp e2
                inner join EquOperTestListTp e1 on e1.Seq = e2.EquOperTestTpSeq
                where e2.EquOperTestTpSeq=@EquOperTestTpSeq or( @EquOperTestTpSeq = 0 and @Code = e1.ExcelNo )";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EquOperTestTpSeq", emdSeq);
            cmd.Parameters.AddWithValue("@Code", code);
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
        //設備運轉抽查標準範本
        public List<T> GetEquOperControlTpByEquOperTestTpSeq<T>(string emdSeq, int pageIndex, int perPage, string code="")
        {
            string sql = @"
                SELECT
                    e2.Seq,
                    e2.EquOperTestTpSeq,
                    e2.EPCheckItem1,
                    e2.EPCheckItem2,
                    e2.EPStand1,
                    e2.EPStand2,
                    e2.EPStand3,
                    e2.EPStand4,
                    e2.EPStand5,
                    e2.EPCheckTiming,
                    e2.EPCheckMethod,
                    e2.EPCheckFeq,
                    e2.EPIncomp,
                    e2.EPManageRec,
                    e2.EPType,
                    e2.EPMemo,
                    e2.EPCheckFields,
                    e2.EPManageFields,
                    e2.OrderNo 
                FROM EquOperControlTp e2
                inner join EquOperTestListTp e1 on e1.Seq = e2.EquOperTestTpSeq
                where e2.EquOperTestTpSeq=@EquOperTestTpSeq or( @EquOperTestTpSeq = 0 and @Code = e1.ExcelNo )
                order by OrderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EquOperTestTpSeq", emdSeq);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            cmd.Parameters.AddWithValue("@Code", code);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //701 施工抽查清單範本
        public List<T> GetConstCheckListTpList<T>()
        {
            string sql = @"SELECT Seq,ItemName  FROM ConstCheckListTp order by OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<T>(cmd);
        }

        public int GetConstCheckControlTpCount(string emdSeq, string code="")
        {
            string sql = @"
                SELECT count(e2.Seq) total FROM ConstCheckControlTp e2
                inner join ConstCheckListTp e1 on e1.Seq = e2.ConstCheckListTpSeq
                where e2.ConstCheckListTpSeq=@ConstCheckListTpSeq or( @ConstCheckListTpSeq = 0 and @Code = e1.ExcelNo)";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@ConstCheckListTpSeq", emdSeq);
            cmd.Parameters.AddWithValue("@Code", code);
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
        //701 施工抽查標準範本
        public List<T> GetConstCheckControlTpByConstCheckListTpSeq<T>(string emdSeq, int pageIndex, int perPage, string code= "")
        {
            string sql = @"
                SELECT
                    e2.Seq,
                    e2.ConstCheckListTpSeq,
                    e2.CCFlow1,
                    e2.CCFlow2,
                    e2.CCManageItem1,
                    e2.CCManageItem2,
                    e2.CCCheckStand1,
                    e2.CCCheckStand2,
                    e2.CCCheckTiming,
                    e2.CCCheckMethod,
                    e2.CCCheckFeq,
                    e2.CCIncomp,
                    e2.CCManageRec,
                    e2.CCType,
                    e2.CCMemo,
                    e2.CCCheckFields,
                    e2.CCManageFields,
                    e2.OrderNo 
                FROM ConstCheckControlTp e2
                inner join ConstCheckListTp e1 on e2.ConstCheckListTpSeq  = e1.Seq
                where e2.ConstCheckListTpSeq=@ConstCheckListTpSeq or( @ConstCheckListTpSeq = 0 and @Code = e1.ExcelNo)
                order by OrderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ConstCheckListTpSeq", emdSeq);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            cmd.Parameters.AddWithValue("@Code", code);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //702 環境保育清單範本
        public List<T> GetEnvirConsListTpList<T>()
        {
            string sql = @"SELECT Seq,ItemName FROM EnvirConsListTp order by OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public int GetEnvirConsControlTpCount(string emdSeq, string code = "")
        {
            string sql = @"
                SELECT count(e2.Seq) total FROM EnvirConsControlTp e2
                inner join EnvirConsListTp e1 on e1.Seq = e2.EnvirConsListSeq --20230829
                where EnvirConsListSeq=@EnvirConsListSeq or(@EnvirConsListSeq = 0 and @Code =e1.ExcelNo)";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EnvirConsListSeq", emdSeq);
            cmd.Parameters.AddWithValue("@Code", code);
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
        //702 環境保育抽查標準範本
        public List<T> GetEnvirConsControlTpByEnvirConsListSeq<T>(string emdSeq, int pageIndex, int perPage, string code="")
        {
            string sql = @"
                SELECT
                    e2.Seq,
                    e2.EnvirConsListSeq,
                    e2.ECCFlow1,
                    e2.ECCCheckItem1,
                    e2.ECCCheckItem2,
                    e2.ECCStand1,
                    e2.ECCStand2,
                    e2.ECCStand3,
                    e2.ECCStand4,
                    e2.ECCStand5,
                    e2.ECCCheckTiming,
                    e2.ECCCheckMethod,
                    e2.ECCCheckFeq,
                    e2.ECCIncomp,
                    e2.ECCManageRec,
                    e2.ECCType,
                    e2.ECCMemo,
                    e2.ECCCheckFields,
                    e2.ECCManageFields,
                    e2.OrderNo 
                FROM EnvirConsControlTp e2
                inner join EnvirConsListTp e1 on e1.Seq = e2.EnvirConsListSeq --20230829
                where EnvirConsListSeq=@EnvirConsListSeq or(@EnvirConsListSeq = 0 and @Code =e1.ExcelNo)
                order by OrderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EnvirConsListSeq", emdSeq);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            cmd.Parameters.AddWithValue("@Code", code);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //703 職業安全衛生清單範本
        public List<T> GetOccuSafeHealthListTpList<T>()
        {
            string sql = @"SELECT Seq,ItemName FROM OccuSafeHealthListTp order by OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<T>(cmd);
        }

        public int GetOccuSafeHealthControlTpCount(string emdSeq, string code ="")
        {
            string sql = @"
                SELECT count(e2.Seq) total FROM OccuSafeHealthControlTp e2
                inner join OccuSafeHealthListTp e1 on e1.Seq = e2.OccuSafeHealthListSeq 
                where OccuSafeHealthListSeq=@OccuSafeHealthListSeq or( @OccuSafeHealthListSeq  = 0 and @Code = e1.ExcelNo)";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@OccuSafeHealthListSeq", emdSeq);
            cmd.Parameters.AddWithValue("@Code", code);
            return (int)db.ExecuteScalar(cmd);

        }
        //703 職業安全衛生抽查標準範本
        public List<T> GetOccuSafeHealthControlTpByOccuSafeHealthListSeq<T>(string emdSeq, int pageIndex, int perPage, string code ="")
        {
            string sql = @"
                SELECT
                    e2.Seq,
                    e2.OccuSafeHealthListSeq,
                    e2.OSCheckItem1,
                    e2.OSCheckItem2,
                    e2.OSStand1,
                    e2.OSStand2,
                    e2.OSStand3,
                    e2.OSStand4,
                    e2.OSStand5,
                    e2.OSCheckTiming,
                    e2.OSCheckMethod,
                    e2.OSCheckFeq,
                    e2.OSIncomp,
                    e2.OSManageRec,
                    e2.OSType,
                    e2.OSMemo,
                    e2.OSCheckFields,
                    e2.OSManageFields,
                    e2.OrderNo 
                FROM OccuSafeHealthControlTp e2
                inner join OccuSafeHealthListTp e1 on e1.Seq = e2.OccuSafeHealthListSeq 
                where OccuSafeHealthListSeq=@OccuSafeHealthListSeq or( @OccuSafeHealthListSeq  = 0 and @Code = e1.ExcelNo)
            order by OrderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@OccuSafeHealthListSeq", emdSeq);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);
            cmd.Parameters.AddWithValue("@Code", code);
            return db.GetDataTableWithClass<T>(cmd);
        }


       
        /*public List<PersonModel> getList(int page, int per_page, string sort_by)
        {
			string sql = @"
				SELECT Seq
					,ParentSeq
					,Code
					,Name
					,OrderNo
					,IsEnabled
					,IsSubUnit
					,IsRegTable
					,CreateTime
					,CreateUser
					,ModifyTime
					,ModifyUser
				FROM Unit
				ORDER BY CASE @Sort_by
						WHEN 'Seq'
							THEN Unit.Seq
						END OFFSET @Page ROWS

				FETCH FIRST @Per_page ROWS ONLY";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@Sort_by", sort_by);
			cmd.Parameters.AddWithValue("@Page", page * per_page);
			cmd.Parameters.AddWithValue("@Per_page", per_page);
			return db.GetDataTableWithClass<PersonModel>(cmd);
		}

		public int AddUnit(FormCollection collection)
        {
			string sql = @"
				INSERT INTO Unit (
					ParentSeq
					,Code
					,[Name]
					,OrderNo
					,IsEnabled
					,IsSubUnit
					,IsRegTable
					,CreateTime
					,CreateUser
					,ModifyTime
					,ModifyUser
					)
				VALUES (
					@ParentSeq
					,@Code
					,@Name
					,@OrderNo
					,@IsEnabled
					,@IsSubUnit
					,@IsRegTable
					,GETDATE()
					,@CreateUser
					,GETDATE()
					,@ModifyUser
					)";
			SqlCommand cmd = db.GetCommand(sql);
			if (Convert.ToInt32(collection["_parentSeq"]) == 0)
			{
				cmd.Parameters.AddWithValue("@ParentSeq", DBNull.Value);
			}
			else
            {
				cmd.Parameters.AddWithValue("@ParentSeq", collection["_parentSeq"]);
            }
			cmd.Parameters.AddWithValue("@Code", collection["_code"]);
			cmd.Parameters.AddWithValue("@Name", collection["_name"]);
			cmd.Parameters.AddWithValue("@OrderNo", collection["_orderNo"]);
			cmd.Parameters.AddWithValue("@IsEnabled", collection["_isEnabled"]);
			cmd.Parameters.AddWithValue("@IsSubUnit", collection["_isSubUnit"]);
			cmd.Parameters.AddWithValue("@IsRegTable", collection["_isRegTable"]);
			cmd.Parameters.AddWithValue("@CreateUser", new SessionManager().GetUser().Seq);
			cmd.Parameters.AddWithValue("@ModifyUser", new SessionManager().GetUser().Seq);
			return db.ExecuteNonQuery(cmd);
		}

		public Object GetCount()
        {
			string sql = @"
				SELECT COUNT(*)
				FROM Unit
				";
			SqlCommand cmd = db.GetCommand(sql);
			return db.ExecuteScalar(cmd);
        }

		public int Update(VUnit item)
        {
			string sql = @"
				UPDATE Unit
				SET ParentSeq = @ParentSeq
					,Code = @Code
					,Name = @Name
					,OrderNo = @OrderNo
					,IsEnabled = @IsEnabled
					,IsSubUnit = @IsSubUnit
					,IsRegTable = @IsRegTable
					,ModifyTime = GETDATE()
					,ModifyUser = @ModifyUser
				WHERE Seq = @Seq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("Seq", item.Seq);
			if (Convert.ToInt32(item.ParentSeq) == 0)
			{
				cmd.Parameters.AddWithValue("@ParentSeq", DBNull.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@ParentSeq", item.ParentSeq);
			}
			cmd.Parameters.AddWithValue("@Code", item.Code);
			cmd.Parameters.AddWithValue("@Name", item.Name);
			cmd.Parameters.AddWithValue("@OrderNo", item.OrderNo);
			cmd.Parameters.AddWithValue("@IsEnabled", item.IsEnabled);
			cmd.Parameters.AddWithValue("@IsSubUnit", item.IsSubUnit);
			cmd.Parameters.AddWithValue("@IsRegTable", item.IsRegTable);
			cmd.Parameters.AddWithValue("@ModifyUser", new SessionManager().GetUser().Seq);
			return db.ExecuteNonQuery(cmd);
		}

		public int Delete(VUnit item)
        {
			string sql = @"
				DELETE Unit WHERE Seq = @Seq";
			SqlCommand cmd = db.GetCommand(sql);
			cmd.Parameters.AddWithValue("@Seq", item.Seq);
			return db.ExecuteNonQuery(cmd);
        }

		public int GetMaxOrderNo()
        {
			string sql = @"
				SELECT MAX(OrderNo) AS MaxOrderNo
				FROM [Unit]";
			SqlCommand cmd = db.GetCommand(sql);
			DataTable dt = db.GetDataTable(cmd);
			if (dt.Rows.Count == 0) return 0;

			if (dt.Rows[0]["MaxOrderNo"] == DBNull.Value)
            {
				return 0;
            }
			else
            {
				return int.Parse(dt.Rows[0]["MaxOrderNo"].ToString());
            }
		}

		public List<VUnit> GetEnabledUnit()
        {
			string sql = @"
				SELECT Seq
					,ParentSeq
					,Code
					,[Name]
					,OrderNo
					,IsEnabled
					,IsSubUnit
					,IsRegTable
					,CreateTime
					,CreateUser
					,ModifyTime
					,ModifyUser
				FROM Unit
				WHERE IsEnabled = 1
				ORDER BY OrderNo";
			SqlCommand cmd = db.GetCommand(sql);
			return db.GetDataTableWithClass<VUnit>(cmd);
        }*/
    }
}