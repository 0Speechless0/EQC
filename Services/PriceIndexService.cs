using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class PriceIndexService : BaseService
    {//物價指數維護
        public List<T> GetKindItem<T>(int id, string mCode)
        {
            string sql = @"SELECT
                    a.Id,
                    a.MCode,
                    a.PS
                FROM PriceIndexKind a
                where Id=@Id
                and MCode=@MCode
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@MCode", mCode);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public List<T> GetKindList<T>()
        {
            string sql = @"SELECT
                    a.Id,
                    a.MCode,
                    a.PS
                FROM PriceIndexKind a
                order by a.Id
                ";
            SqlCommand cmd = db.GetCommand(sql);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //清單
        public List<T> GetList<T>(int priceIndexKindId)
        {
            string sql = @"SELECT
                    a.Seq,
                    a.PriceIndexKindId,
                    a.PIDate,
                    a.PriceIndex
                FROM PriceIndexItems a
				where a.PriceIndexKindId=@PriceIndexKindId
                order by a.PIDate DESC
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@PriceIndexKindId", priceIndexKindId);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //匯入
        public void ImportData(List<PriceIndexListVModel> items)
        {
            SqlCommand cmd;
            string sql;
            foreach (PriceIndexListVModel kind in items)
            {
                db.BeginTransaction();
                try
                {
                    sql = @"delete FROM PriceIndexItems where PriceIndexKindId=@PriceIndexKindId";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.AddWithValue("@PriceIndexKindId", kind.Id);
                    db.ExecuteNonQuery(cmd);
                    foreach (PriceIndexItemModel m in kind.items)
                    {
                        Null2Empty(m);
                        sql = @"insert into PriceIndexItems (
                            PriceIndexKindId,
                            PIDate,
                            PriceIndex,
                            ModifyTime,
                            ModifyUserSeq
                        ) values (
                            @PriceIndexKindId,
                            @PIDate,
                            @PriceIndex,
                            GetDate(),
                            @ModifyUserSeq
                        )
                        ";
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.AddWithValue("@PriceIndexKindId", kind.Id);
                        cmd.Parameters.AddWithValue("@PIDate", m.PIDate);
                        cmd.Parameters.AddWithValue("@PriceIndex", m.PriceIndex);
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                        db.ExecuteNonQuery(cmd);
                    }

                    db.TransactionCommit();
                    //return true;
                }
                catch (Exception e)
                {
                    db.TransactionRollback();
                    log.Info("PriceIndexService.ImportData: " + e.Message);
                    //return false;
                }
            }
        }

        //=============================================================
        //新增
        public int AddRecord(CarbonEmissionFactorModel m)
        {
            Null2Empty(m);
            string sql = @"
            insert into CarbonEmissionFactor (
                Code,
                Item,
                KgCo2e,
                Unit,
                Kind,
                IsEnabled,
                SubCode,
                Memo,
                KeyCode1,
                KeyCode2,
                KeyCode3,
                CreateTime,
                CreateUserSeq,
                ModifyTime,
                ModifyUserSeq
            )values(
                @Code,
                @Item,
                @KgCo2e,
                @Unit,
                @Kind,
                @IsEnabled,
                @SubCode,
                @Memo,
                @KeyCode1,
                @KeyCode2,
                @KeyCode3,
                GetDate(),
                @ModifyUserSeq,
                GetDate(),
                @ModifyUserSeq
            )";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Code", m.Code);
                cmd.Parameters.AddWithValue("@Item", m.Item);
                cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(m.KgCo2e));
                cmd.Parameters.AddWithValue("@Unit", m.Unit);
                cmd.Parameters.AddWithValue("@Kind", m.Kind);
                cmd.Parameters.AddWithValue("@IsEnabled", m.IsEnabled);
                cmd.Parameters.AddWithValue("@SubCode", m.SubCode);
                cmd.Parameters.AddWithValue("@Memo", m.Memo);
                cmd.Parameters.AddWithValue("@KeyCode1", m.KeyCode1);
                cmd.Parameters.AddWithValue("@KeyCode2", m.KeyCode2);
                cmd.Parameters.AddWithValue("@KeyCode3", m.KeyCode3);

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('CarbonEmissionFactor') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);

                m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                return 0;
            }
            catch (Exception e)
            {
                log.Info("CarbonEmissionFactorService.AddRecords: " + e.Message);
                log.Info(m.Code);
                return -1;
            }
        }
        //更新
        public int UpdateRecord(CarbonEmissionFactorModel m)
        {
            Null2Empty(m);
            string sql = @"
            update CarbonEmissionFactor set 
                Code = @Code,
                Item = @Item,
                KgCo2e = @KgCo2e,
                Unit = @Unit,
                Kind = @Kind,
                IsEnabled = @IsEnabled,
                SubCode = @SubCode,
                Memo = @Memo,
                KeyCode1 = @KeyCode1,
                KeyCode2 = @KeyCode2,
                KeyCode3 = @KeyCode3,
                ModifyTime = GetDate(),
                ModifyUserSeq = @ModifyUserSeq
            where Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@Code", m.Code);
                cmd.Parameters.AddWithValue("@Item", m.Item);
                cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(m.KgCo2e));
                cmd.Parameters.AddWithValue("@Unit", m.Unit);
                cmd.Parameters.AddWithValue("@Kind", m.Kind);
                cmd.Parameters.AddWithValue("@IsEnabled", m.IsEnabled);
                cmd.Parameters.AddWithValue("@SubCode", m.SubCode);
                cmd.Parameters.AddWithValue("@Memo", m.Memo);
                cmd.Parameters.AddWithValue("@KeyCode1", m.KeyCode1);
                cmd.Parameters.AddWithValue("@KeyCode2", m.KeyCode2);
                cmd.Parameters.AddWithValue("@KeyCode3", m.KeyCode3);

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("CarbonEmissionFactorService.UpdateRecords: " + e.Message);
                log.Info(m.Code);
                return -1;
            }
        }
        //刪除
        public int DelRecord(int seq)
        {
            string sql = "";
            db.BeginTransaction();
            try
            {
                sql = @"delete from CarbonEmissionFactor where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("CarbonEmissionFactorService.DelDelRecords: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
       
    }
}