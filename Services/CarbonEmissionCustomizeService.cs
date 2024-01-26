using EQC.Common;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class CarbonEmissionCustomizeService : BaseService
    {//自定義 碳排係數維護

        /*/批次處裡
        public void ImportData(List<CarbonEmissionCustomizeModel> items, ref int iCnt, ref int uCnt, ref string errCnt)
        {
            SqlCommand cmd;
            string sql;
            foreach (CarbonEmissionCustomizeModel m in items)
            {
                Null2Empty(m);
                sql = @"SELECT Seq FROM CarbonEmissionCustomize
                        where NameSpec=@NameSpec";
                cmd = db.GetCommand(sql);
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
        }*/
        //新增
        public int AddRecord(CarbonEmissionCustomizeModel m)
        {
            Null2Empty(m);
            string sql = @"
            insert into CarbonEmissionCustomize (
                CreateUnit,
                ItemCode,
                NameSpec,
                KgCo2e,
                Unit,
                Memo,
                CreateTime,
                CreateUserSeq,
                ModifyTime,
                ModifyUserSeq
            )values(
                @CreateUnit,
                @ItemCode,
                @NameSpec,
                @KgCo2e,
                @Unit,
                @Memo,
                GetDate(),
                @ModifyUserSeq,
                GetDate(),
                @ModifyUserSeq
            )";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@CreateUnit", new SessionManager().GetUser().UnitName1);
                cmd.Parameters.AddWithValue("@ItemCode", m.ItemCode);
                cmd.Parameters.AddWithValue("@NameSpec", m.NameSpec);
                cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(m.KgCo2e));
                cmd.Parameters.AddWithValue("@Unit", m.Unit);
                cmd.Parameters.AddWithValue("@Memo", m.Memo);

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('CarbonEmissionCustomize') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);

                m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                return 0;
            }
            catch (Exception e)
            {
                log.Info("CarbonEmissionCustomizeService.AddRecord: " + e.Message);
                return -1;
            }
        }
        //更新
        public int UpdateRecord(CarbonEmissionCustomizeModel m)
        {
            Null2Empty(m);
            string sql = @"
            update CarbonEmissionCustomize set 
                IsDel=0,
                ItemCode = @ItemCode,
                NameSpec = @NameSpec,
                KgCo2e = @KgCo2e,
                Unit = @Unit,
                Memo = @Memo,
                ModifyTime = GetDate(),
                ModifyUserSeq = @ModifyUserSeq
            where Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ItemCode", m.ItemCode);
                cmd.Parameters.AddWithValue("@NameSpec", m.NameSpec);
                cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(m.KgCo2e));
                cmd.Parameters.AddWithValue("@Unit", m.Unit);
                cmd.Parameters.AddWithValue("@Memo", m.Memo);

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("CarbonEmissionCustomizeService.UpdateRecord: " + e.Message);
                return -1;
            }
        }
        //清單
        public int GetListCount(string keyWord)
        {
            string sql = @"SELECT
                    count(a.Seq) total
                FROM CarbonEmissionCustomize a 
                where a.IsDel=0
                and (1=@IsAdmin or CreateUnit=@CreateUnit)
                and a.NameSpec Like @keyWord";

            UserInfo userInfo = new SessionManager().GetUser();
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@IsAdmin", (userInfo.IsAdmin || userInfo.IsEQCAdmin) ? 1 : 0);
            cmd.Parameters.AddWithValue("@CreateUnit", userInfo.UnitName1);
            cmd.Parameters.AddWithValue("@keyWord", "%" + keyWord + "%");
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetList<T>(int pageRecordCount, int pageIndex, string keyWord)
        {
            string sql = @"SELECT
                    a.Seq,
                    a.CreateUnit,
                    a.ItemCode,
                    a.NameSpec,
                    a.KgCo2e,
                    a.Unit,
                    a.Memo
                FROM CarbonEmissionCustomize a
				where a.IsDel=0
                and (1=@IsAdmin or CreateUnit=@CreateUnit)
                and a.NameSpec Like @keyWord
                order by a.NameSpec
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";

            UserInfo userInfo = new SessionManager().GetUser();
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            cmd.Parameters.AddWithValue("@IsAdmin", (userInfo.IsAdmin || userInfo.IsEQCAdmin) ? 1 : 0);
            cmd.Parameters.AddWithValue("@CreateUnit", userInfo.UnitName1);
            cmd.Parameters.AddWithValue("@keyWord", "%" + keyWord + "%");

            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<DateTimeVModel> GetLastDateTime()
        {
            string sql = @"SELECT max(ModifyTime) itemDT FROM CarbonEmissionCustomize";
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
                //sql = @"delete from CarbonEmissionCustomize where Seq=@Seq";
                sql = @"
                update CarbonEmissionCustomize set
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
                log.Info("CarbonEmissionCustomizeService.DelRecord: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
    }
}