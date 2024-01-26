using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class CarbonEmissionMachineService : BaseService
    {//機具 碳排係數維護

        //機台清單
        public List<SelectOptionModel> GetMachineKindList()
        {
            string sql = @"
                SELECT
                    DISTINCT Kind Text, Kind Value
                FROM CarbonEmissionMachine
                order by Kind";
            SqlCommand cmd = db.GetCommand(sql);


            return db.GetDataTableWithClass<SelectOptionModel>(cmd);
        }
        //該機台規格清單
        public List<CarbonEmissionMachineModel> GetMachineSpecList(string kind)
        {
            /*string sql = @"
                SELECT
                    DISTINCT NameSpec Text, NameSpec Value
                FROM CarbonEmissionMachine
                where Kind=@Kind
                order by NameSpec";*/
            //s20230502
            string sql = @"
                SELECT
                    NameSpec, KgCo2e
                FROM CarbonEmissionMachine
                where Kind=@Kind
                order by NameSpec";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Kind", kind);

            return db.GetDataTableWithClass<CarbonEmissionMachineModel>(cmd);
        }

        //批次處裡
        public void ImportData(List<CarbonEmissionMachineModel> items, ref int iCnt, ref int uCnt, ref string errCnt)
        {
            SqlCommand cmd;
            string sql;
            foreach (CarbonEmissionMachineModel m in items)
            {
                Null2Empty(m);
                sql = @"SELECT Seq FROM CarbonEmissionMachine
                        where Kind=@Kind
                        and NameSpec=@NameSpec";
                cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Kind", m.Kind);
                cmd.Parameters.AddWithValue("@NameSpec", m.NameSpec);
                DataTable dt = db.GetDataTable(cmd);
                if (dt.Rows.Count == 1)
                {
                    m.Seq = Convert.ToInt32(dt.Rows[0]["Seq"].ToString());
                    if (UpdateRecord(m) == -1)
                        errCnt += String.Format("{0},", m.itemNo);
                    else
                        uCnt++;
                }
                else
                {
                    if (AddRecord(m) == -1)
                    {
                        errCnt += String.Format("{0},", m.itemNo);
                    }
                    else
                        iCnt++;
                }
            }
        }
        //新增
        public int AddRecord(CarbonEmissionMachineModel m)
        {
            Null2Empty(m);
            string sql = @"
            insert into CarbonEmissionMachine (
                Kind,
                NameSpec,
                KgCo2e,
                Unit,
                Memo,
                ConsumptionRate,
                ConsumptionRateUnit,
                FuelKind,
                FuelKgCo2e,
                FuelUnit,
                CreateTime,
                CreateUserSeq,
                ModifyTime,
                ModifyUserSeq
            )values(
                @Kind,
                @NameSpec,
                @KgCo2e,
                @Unit,
                @Memo,
                @ConsumptionRate,
                @ConsumptionRateUnit,
                @FuelKind,
                @FuelKgCo2e,
                @FuelUnit,
                GetDate(),
                @ModifyUserSeq,
                GetDate(),
                @ModifyUserSeq
            )";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Kind", m.Kind);
                cmd.Parameters.AddWithValue("@NameSpec", m.NameSpec);
                cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(m.KgCo2e));
                cmd.Parameters.AddWithValue("@Unit", m.Unit);
                cmd.Parameters.AddWithValue("@Memo", m.Memo);
                cmd.Parameters.AddWithValue("@ConsumptionRate", this.NulltoDBNull(m.ConsumptionRate));
                cmd.Parameters.AddWithValue("@ConsumptionRateUnit", m.ConsumptionRateUnit);
                cmd.Parameters.AddWithValue("@FuelKind", m.FuelKind);
                cmd.Parameters.AddWithValue("@FuelKgCo2e", this.NulltoDBNull(m.FuelKgCo2e));
                cmd.Parameters.AddWithValue("@FuelUnit", m.FuelUnit);
                

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('CarbonEmissionMachine') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);

                m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                return 0;
            }
            catch (Exception e)
            {
                log.Info("CarbonEmissionMachineService.AddRecord: " + e.Message);
                return -1;
            }
        }
        //更新
        public int UpdateRecord(CarbonEmissionMachineModel m)
        {
            Null2Empty(m);
            string sql = @"
            update CarbonEmissionMachine set 
                IsDel=0,
                Kind = @Kind,
                NameSpec = @NameSpec,
                KgCo2e = @KgCo2e,
                Unit = @Unit,
                Memo = @Memo,
                ConsumptionRate = @ConsumptionRate,
                ConsumptionRateUnit = @ConsumptionRateUnit,
                FuelKind = @FuelKind,
                FuelKgCo2e = @FuelKgCo2e,
                FuelUnit = @FuelUnit,
                ModifyTime = GetDate(),
                ModifyUserSeq = @ModifyUserSeq
            where Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@Kind", m.Kind);
                cmd.Parameters.AddWithValue("@NameSpec", m.NameSpec);
                cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(m.KgCo2e));
                cmd.Parameters.AddWithValue("@Unit", m.Unit);
                cmd.Parameters.AddWithValue("@Memo", m.Memo);
                cmd.Parameters.AddWithValue("@ConsumptionRate", this.NulltoDBNull(m.ConsumptionRate));
                cmd.Parameters.AddWithValue("@ConsumptionRateUnit", m.ConsumptionRateUnit);
                cmd.Parameters.AddWithValue("@FuelKind", m.FuelKind);
                cmd.Parameters.AddWithValue("@FuelKgCo2e", this.NulltoDBNull(m.FuelKgCo2e));
                cmd.Parameters.AddWithValue("@FuelUnit", m.FuelUnit);

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("CarbonEmissionMachineService.UpdateRecord: " + e.Message);
                return -1;
            }
        }
        //清單
        public int GetListCount(string keyWord)
        {
            string sql = @"SELECT
                    count(a.Seq) total
                FROM CarbonEmissionMachine a 
                where a.IsDel=0 and a.NameSpec Like @keyWord";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@keyWord", "%" + keyWord + "%");
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetList<T>(int pageRecordCount, int pageIndex, string keyWord)
        {
            string sql = @"SELECT
                    a.Seq,
                    a.Kind,
                    a.NameSpec,
                    a.KgCo2e,
                    a.Unit,
                    a.Memo,
                    a.ConsumptionRate,
                    a.ConsumptionRateUnit,
                    a.FuelKind,
                    a.FuelKgCo2e,
                    a.FuelUnit
                FROM CarbonEmissionMachine a
				where a.IsDel=0 and a.NameSpec Like @keyWord
                order by a.Kind,a.NameSpec
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            cmd.Parameters.AddWithValue("@keyWord", "%" + keyWord + "%");

            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<DateTimeVModel> GetLastDateTime()
        {
            string sql = @"SELECT max(ModifyTime) itemDT FROM CarbonEmissionMachine";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<DateTimeVModel>(cmd);
        }

        //刪除
        public int DelRecord(int seq)
        {
            string sql = "";
            db.BeginTransaction();
            try
            {
                //sql = @"delete from CarbonEmissionMachine where Seq=@Seq";
                sql = @"
                update CarbonEmissionMachine set
                    IsDel=1,
                    ModifyTime = GetDate(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("CarbonEmissionMachineService.DelRecord: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
    }
}