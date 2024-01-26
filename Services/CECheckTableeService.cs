using EQC.Common;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class CECheckTableService : BaseService
    {//碳排放量檢核表
        public List<T> GetList<T>(int engMainSeq)
        {
            string sql = @"select b.*
                from CarbonEmissionHeader a
                inner join CECheckTable b on(b.CarbonEmissionHeaderSeq=a.Seq)
                where a.EngMainSeq=@EngMainSeq
            ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public int AddRecord(CECheckTableModel m)
        {
            Null2Empty(m);
            string sql = @"
            insert into CECheckTable (
                CarbonEmissionHeaderSeq,
                F1101, F1102, F1103, F1104, F1105, F1106, F1106TreeJson,
                F1107, F1107Desc, F1107TreeJson, F1107TreeTotal,
                F1108, F1108Area, F1109, F1109Length, F1110, F1110Desc,
                F1201, F1201Mix, F1201Other, F1202, F1203, F1204, F1205, F1206, F1206Desc,
                F1301, F1302, F1303, F1304, F1305, F1306, F1306Mode, F1307, F1307Mode,
                F1308, F1309, F1309Mode, F1310, F1310Mode, F1311, F1312, F1312Desc,
                F1401, F1402, F1403, F1404, F1404Desc,
                F2101, F2102, F2103, F2103Desc,
                F2201, F2202, F2203, F2204, F2204Desc,
                F2301, F2302, F2303, F2303Desc,
                F3101, F3102, F3103, F3104, F3104Desc,
                F3201, F3202, F3203, F3204, F3204Desc,
                F3301, F3302, F3302Desc,
                F3401, F3402, F3403, F3403Desc,
                Signature, Remark,
                CreateTime,
                CreateUserSeq,
                ModifyTime,
                ModifyUserSeq
            )values(
                @CarbonEmissionHeaderSeq,
                @F1101, @F1102, @F1103, @F1104, @F1105, @F1106, @F1106TreeJson,
                @F1107, @F1107Desc, @F1107TreeJson, @F1107TreeTotal,
                @F1108, @F1108Area, @F1109, @F1109Length, @F1110, @F1110Desc,
                @F1201, @F1201Mix, @F1201Other, @F1202, @F1203, @F1204, @F1205, @F1206, @F1206Desc,
                @F1301, @F1302, @F1303, @F1304, @F1305, @F1306, @F1306Mode, @F1307, @F1307Mode,
                @F1308, @F1309, @F1309Mode, @F1310, @F1310Mode, @F1311, @F1312, @F1312Desc,
                @F1401, @F1402, @F1403, @F1404, @F1404Desc,
                @F2101, @F2102, @F2103, @F2103Desc,
                @F2201, @F2202, @F2203, @F2204, @F2204Desc,
                @F2301, @F2302, @F2303, @F2303Desc,
                @F3101, @F3102, @F3103, @F3104, @F3104Desc,
                @F3201, @F3202, @F3203, @F3204, @F3204Desc,
                @F3301, @F3302, @F3302Desc,
                @F3401, @F3402, @F3403, @F3403Desc,
                @Signature, @Remark,
                GetDate(),
                @ModifyUserSeq,
                GetDate(),
                @ModifyUserSeq
            )";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@CarbonEmissionHeaderSeq", m.CarbonEmissionHeaderSeq);
                cmd.Parameters.AddWithValue("@F1101", this.NulltoDBNull(m.F1101));
                cmd.Parameters.AddWithValue("@F1102", this.NulltoDBNull(m.F1102));
                cmd.Parameters.AddWithValue("@F1103", this.NulltoDBNull(m.F1103));
                cmd.Parameters.AddWithValue("@F1104", this.NulltoDBNull(m.F1104));
                cmd.Parameters.AddWithValue("@F1105", this.NulltoDBNull(m.F1105));
                cmd.Parameters.AddWithValue("@F1106", this.NulltoDBNull(m.F1106));
                cmd.Parameters.AddWithValue("@F1106TreeJson", m.F1106TreeJson);
                cmd.Parameters.AddWithValue("@F1107", this.NulltoDBNull(m.F1107));
                cmd.Parameters.AddWithValue("@F1107Desc", m.F1107Desc);
                cmd.Parameters.AddWithValue("@F1107TreeJson", m.F1107TreeJson);//s20231006
                cmd.Parameters.AddWithValue("@F1107TreeTotal", m.F1107TreeTotal);//s20231006
                cmd.Parameters.AddWithValue("@F1108", this.NulltoDBNull(m.F1108));
                cmd.Parameters.AddWithValue("@F1108Area", this.NulltoDBNull(m.F1108Area));
                cmd.Parameters.AddWithValue("@F1109", this.NulltoDBNull(m.F1109));
                cmd.Parameters.AddWithValue("@F1109Length", this.NulltoDBNull(m.F1109Length));
                cmd.Parameters.AddWithValue("@F1110", this.NulltoDBNull(m.F1110));
                cmd.Parameters.AddWithValue("@F1110Desc", m.F1110Desc);
                cmd.Parameters.AddWithValue("@F1201", this.NulltoDBNull(m.F1201));
                cmd.Parameters.AddWithValue("@F1201Mix", this.NulltoDBNull(m.F1201Mix));
                cmd.Parameters.AddWithValue("@F1201Other", m.F1201Other);
                cmd.Parameters.AddWithValue("@F1202", this.NulltoDBNull(m.F1202));
                cmd.Parameters.AddWithValue("@F1203", this.NulltoDBNull(m.F1203));
                cmd.Parameters.AddWithValue("@F1204", this.NulltoDBNull(m.F1204));
                cmd.Parameters.AddWithValue("@F1205", this.NulltoDBNull(m.F1205));
                cmd.Parameters.AddWithValue("@F1206", this.NulltoDBNull(m.F1206));
                cmd.Parameters.AddWithValue("@F1206Desc", m.F1206Desc);
                cmd.Parameters.AddWithValue("@F1301", this.NulltoDBNull(m.F1301));
                cmd.Parameters.AddWithValue("@F1302", this.NulltoDBNull(m.F1302));
                cmd.Parameters.AddWithValue("@F1303", this.NulltoDBNull(m.F1303));
                cmd.Parameters.AddWithValue("@F1304", this.NulltoDBNull(m.F1304));
                cmd.Parameters.AddWithValue("@F1305", this.NulltoDBNull(m.F1305));
                cmd.Parameters.AddWithValue("@F1306", this.NulltoDBNull(m.F1306));
                cmd.Parameters.AddWithValue("@F1306Mode", this.NulltoDBNull(m.F1306Mode));
                cmd.Parameters.AddWithValue("@F1307", this.NulltoDBNull(m.F1307));
                cmd.Parameters.AddWithValue("@F1307Mode", this.NulltoDBNull(m.F1307Mode));
                cmd.Parameters.AddWithValue("@F1308", this.NulltoDBNull(m.F1308));
                cmd.Parameters.AddWithValue("@F1309", this.NulltoDBNull(m.F1309));
                cmd.Parameters.AddWithValue("@F1309Mode", this.NulltoDBNull(m.F1309Mode));
                cmd.Parameters.AddWithValue("@F1310", this.NulltoDBNull(m.F1310));
                cmd.Parameters.AddWithValue("@F1310Mode", this.NulltoDBNull(m.F1310Mode));
                cmd.Parameters.AddWithValue("@F1311", this.NulltoDBNull(m.F1311));
                cmd.Parameters.AddWithValue("@F1312", this.NulltoDBNull(m.F1312));
                cmd.Parameters.AddWithValue("@F1312Desc", m.F1312Desc);
                cmd.Parameters.AddWithValue("@F1401", this.NulltoDBNull(m.F1401));
                cmd.Parameters.AddWithValue("@F1402", this.NulltoDBNull(m.F1402));
                cmd.Parameters.AddWithValue("@F1403", this.NulltoDBNull(m.F1403));
                cmd.Parameters.AddWithValue("@F1404", this.NulltoDBNull(m.F1404));
                cmd.Parameters.AddWithValue("@F1404Desc", m.F1404Desc);
                cmd.Parameters.AddWithValue("@F2101", this.NulltoDBNull(m.F2101));
                cmd.Parameters.AddWithValue("@F2102", this.NulltoDBNull(m.F2102));
                cmd.Parameters.AddWithValue("@F2103", this.NulltoDBNull(m.F2103));
                cmd.Parameters.AddWithValue("@F2103Desc", m.F2103Desc);
                cmd.Parameters.AddWithValue("@F2201", this.NulltoDBNull(m.F2201));
                cmd.Parameters.AddWithValue("@F2202", this.NulltoDBNull(m.F2202));
                cmd.Parameters.AddWithValue("@F2203", this.NulltoDBNull(m.F2203));
                cmd.Parameters.AddWithValue("@F2204", this.NulltoDBNull(m.F2204));
                cmd.Parameters.AddWithValue("@F2204Desc", m.F2204Desc);
                cmd.Parameters.AddWithValue("@F2301", this.NulltoDBNull(m.F2301));
                cmd.Parameters.AddWithValue("@F2302", this.NulltoDBNull(m.F2302));
                cmd.Parameters.AddWithValue("@F2303", this.NulltoDBNull(m.F2303));
                cmd.Parameters.AddWithValue("@F2303Desc", m.F2303Desc);
                cmd.Parameters.AddWithValue("@F3101", this.NulltoDBNull(m.F3101));
                cmd.Parameters.AddWithValue("@F3102", this.NulltoDBNull(m.F3102));
                cmd.Parameters.AddWithValue("@F3103", this.NulltoDBNull(m.F3103));
                cmd.Parameters.AddWithValue("@F3104", this.NulltoDBNull(m.F3104));
                cmd.Parameters.AddWithValue("@F3104Desc", m.F3104Desc);
                cmd.Parameters.AddWithValue("@F3201", this.NulltoDBNull(m.F3201));
                cmd.Parameters.AddWithValue("@F3202", this.NulltoDBNull(m.F3202));
                cmd.Parameters.AddWithValue("@F3203", this.NulltoDBNull(m.F3203));
                cmd.Parameters.AddWithValue("@F3204", this.NulltoDBNull(m.F3204));
                cmd.Parameters.AddWithValue("@F3204Desc", m.F3204Desc);
                cmd.Parameters.AddWithValue("@F3301", this.NulltoDBNull(m.F3301));
                cmd.Parameters.AddWithValue("@F3302", this.NulltoDBNull(m.F3302));
                cmd.Parameters.AddWithValue("@F3302Desc", m.F3302Desc);
                cmd.Parameters.AddWithValue("@F3401", this.NulltoDBNull(m.F3401));
                cmd.Parameters.AddWithValue("@F3402", this.NulltoDBNull(m.F3402));
                cmd.Parameters.AddWithValue("@F3403", this.NulltoDBNull(m.F3403));
                cmd.Parameters.AddWithValue("@F3403Desc", m.F3403Desc);
                cmd.Parameters.AddWithValue("@Signature", m.Signature);
                cmd.Parameters.AddWithValue("@Remark", m.Remark);

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('CECheckTable') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);

                m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                return 0;
            }
            catch (Exception e)
            {
                log.Info("CECheckTableService.AddRecord: " + e.Message);
                return -1;
            }
        }
        //更新
        public int UpdateRecord(CECheckTableModel m)
        {
            Null2Empty(m);
            string sql = @"
            update CECheckTable set
                F1101=@F1101, F1102=@F1102, F1103=@F1103, F1104=@F1104, F1105=@F1105, F1106=@F1106, F1106TreeJson=@F1106TreeJson,
                F1107=@F1107, F1107Desc=@F1107Desc, F1107TreeJson=@F1107TreeJson, F1107TreeTotal=@F1107TreeTotal,
                F1108=@F1108, F1108Area=@F1108Area, F1109=@F1109, F1109Length=@F1109Length, F1110=@F1110, F1110Desc=@F1110Desc,
                F1201=@F1201, F1201Mix=@F1201Mix, F1201Other=@F1201Other, F1202=@F1202, F1203=@F1203, F1204=@F1204, F1205=@F1205, F1206=@F1206, F1206Desc=@F1206Desc,
                F1301=@F1301, F1302=@F1302, F1303=@F1303, F1304=@F1304, F1305=@F1305, F1306=@F1306, F1306Mode=@F1306Mode, F1307=@F1307, F1307Mode=@F1307Mode,
                F1308=@F1308, F1309=@F1309, F1309Mode=@F1309Mode, F1310=@F1310, F1310Mode=@F1310Mode, F1311=@F1311, F1312=@F1312, F1312Desc=@F1312Desc,
                F1401=@F1401, F1402=@F1402, F1403=@F1403, F1404=@F1404, F1404Desc=@F1404Desc,
                F2101=@F2101, F2102=@F2102, F2103=@F2103, F2103Desc=@F2103Desc,
                F2201=@F2201, F2202=@F2202, F2203=@F2203, F2204=@F2204, F2204Desc=@F2204Desc,
                F2301=@F2301, F2302=@F2302, F2303=@F2303, F2303Desc=@F2303Desc,
                F3101=@F3101, F3102=@F3102, F3103=@F3103, F3104=@F3104, F3104Desc=@F3104Desc,
                F3201=@F3201, F3202=@F3202, F3203=@F3203, F3204=@F3204, F3204Desc=@F3204Desc,
                F3301=@F3301, F3302=@F3302, F3302Desc=@F3302Desc,
                F3401=@F3401, F3402=@F3402, F3403=@F3403, F3403Desc=@F3403Desc,
                Signature=@Signature, Remark=@Remark,
                ModifyTime = GetDate(),
                ModifyUserSeq = @ModifyUserSeq
            where Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@F1101", this.NulltoDBNull(m.F1101));
                cmd.Parameters.AddWithValue("@F1102", this.NulltoDBNull(m.F1102));
                cmd.Parameters.AddWithValue("@F1103", this.NulltoDBNull(m.F1103));
                cmd.Parameters.AddWithValue("@F1104", this.NulltoDBNull(m.F1104));
                cmd.Parameters.AddWithValue("@F1105", this.NulltoDBNull(m.F1105));
                cmd.Parameters.AddWithValue("@F1106", this.NulltoDBNull(m.F1106));
                cmd.Parameters.AddWithValue("@F1106TreeJson", m.F1106TreeJson);
                cmd.Parameters.AddWithValue("@F1107", this.NulltoDBNull(m.F1107));
                cmd.Parameters.AddWithValue("@F1107Desc", m.F1107Desc);
                cmd.Parameters.AddWithValue("@F1107TreeJson", m.F1107TreeJson);//s20231006
                cmd.Parameters.AddWithValue("@F1107TreeTotal", m.F1107TreeTotal);//s20231006
                cmd.Parameters.AddWithValue("@F1108", this.NulltoDBNull(m.F1108));
                cmd.Parameters.AddWithValue("@F1108Area", this.NulltoDBNull(m.F1108Area));
                cmd.Parameters.AddWithValue("@F1109", this.NulltoDBNull(m.F1109));
                cmd.Parameters.AddWithValue("@F1109Length", this.NulltoDBNull(m.F1109Length));
                cmd.Parameters.AddWithValue("@F1110", this.NulltoDBNull(m.F1110));
                cmd.Parameters.AddWithValue("@F1110Desc", m.F1110Desc);
                cmd.Parameters.AddWithValue("@F1201", this.NulltoDBNull(m.F1201));
                cmd.Parameters.AddWithValue("@F1201Mix", this.NulltoDBNull(m.F1201Mix));
                cmd.Parameters.AddWithValue("@F1201Other", m.F1201Other);
                cmd.Parameters.AddWithValue("@F1202", this.NulltoDBNull(m.F1202));
                cmd.Parameters.AddWithValue("@F1203", this.NulltoDBNull(m.F1203));
                cmd.Parameters.AddWithValue("@F1204", this.NulltoDBNull(m.F1204));
                cmd.Parameters.AddWithValue("@F1205", this.NulltoDBNull(m.F1205));
                cmd.Parameters.AddWithValue("@F1206", this.NulltoDBNull(m.F1206));
                cmd.Parameters.AddWithValue("@F1206Desc", m.F1206Desc);
                cmd.Parameters.AddWithValue("@F1301", this.NulltoDBNull(m.F1301));
                cmd.Parameters.AddWithValue("@F1302", this.NulltoDBNull(m.F1302));
                cmd.Parameters.AddWithValue("@F1303", this.NulltoDBNull(m.F1303));
                cmd.Parameters.AddWithValue("@F1304", this.NulltoDBNull(m.F1304));
                cmd.Parameters.AddWithValue("@F1305", this.NulltoDBNull(m.F1305));
                cmd.Parameters.AddWithValue("@F1306", this.NulltoDBNull(m.F1306));
                cmd.Parameters.AddWithValue("@F1306Mode", this.NulltoDBNull(m.F1306Mode));
                cmd.Parameters.AddWithValue("@F1307", this.NulltoDBNull(m.F1307));
                cmd.Parameters.AddWithValue("@F1307Mode", this.NulltoDBNull(m.F1307Mode));
                cmd.Parameters.AddWithValue("@F1308", this.NulltoDBNull(m.F1308));
                cmd.Parameters.AddWithValue("@F1309", this.NulltoDBNull(m.F1309));
                cmd.Parameters.AddWithValue("@F1309Mode", this.NulltoDBNull(m.F1309Mode));
                cmd.Parameters.AddWithValue("@F1310", this.NulltoDBNull(m.F1310));
                cmd.Parameters.AddWithValue("@F1310Mode", this.NulltoDBNull(m.F1310Mode));
                cmd.Parameters.AddWithValue("@F1311", this.NulltoDBNull(m.F1311));
                cmd.Parameters.AddWithValue("@F1312", this.NulltoDBNull(m.F1312));
                cmd.Parameters.AddWithValue("@F1312Desc", m.F1312Desc);
                cmd.Parameters.AddWithValue("@F1401", this.NulltoDBNull(m.F1401));
                cmd.Parameters.AddWithValue("@F1402", this.NulltoDBNull(m.F1402));
                cmd.Parameters.AddWithValue("@F1403", this.NulltoDBNull(m.F1403));
                cmd.Parameters.AddWithValue("@F1404", this.NulltoDBNull(m.F1404));
                cmd.Parameters.AddWithValue("@F1404Desc", m.F1404Desc);
                cmd.Parameters.AddWithValue("@F2101", this.NulltoDBNull(m.F2101));
                cmd.Parameters.AddWithValue("@F2102", this.NulltoDBNull(m.F2102));
                cmd.Parameters.AddWithValue("@F2103", this.NulltoDBNull(m.F2103));
                cmd.Parameters.AddWithValue("@F2103Desc", m.F2103Desc);
                cmd.Parameters.AddWithValue("@F2201", this.NulltoDBNull(m.F2201));
                cmd.Parameters.AddWithValue("@F2202", this.NulltoDBNull(m.F2202));
                cmd.Parameters.AddWithValue("@F2203", this.NulltoDBNull(m.F2203));
                cmd.Parameters.AddWithValue("@F2204", this.NulltoDBNull(m.F2204));
                cmd.Parameters.AddWithValue("@F2204Desc", m.F2204Desc);
                cmd.Parameters.AddWithValue("@F2301", this.NulltoDBNull(m.F2301));
                cmd.Parameters.AddWithValue("@F2302", this.NulltoDBNull(m.F2302));
                cmd.Parameters.AddWithValue("@F2303", this.NulltoDBNull(m.F2303));
                cmd.Parameters.AddWithValue("@F2303Desc", m.F2303Desc);
                cmd.Parameters.AddWithValue("@F3101", this.NulltoDBNull(m.F3101));
                cmd.Parameters.AddWithValue("@F3102", this.NulltoDBNull(m.F3102));
                cmd.Parameters.AddWithValue("@F3103", this.NulltoDBNull(m.F3103));
                cmd.Parameters.AddWithValue("@F3104", this.NulltoDBNull(m.F3104));
                cmd.Parameters.AddWithValue("@F3104Desc", m.F3104Desc);
                cmd.Parameters.AddWithValue("@F3201", this.NulltoDBNull(m.F3201));
                cmd.Parameters.AddWithValue("@F3202", this.NulltoDBNull(m.F3202));
                cmd.Parameters.AddWithValue("@F3203", this.NulltoDBNull(m.F3203));
                cmd.Parameters.AddWithValue("@F3204", this.NulltoDBNull(m.F3204));
                cmd.Parameters.AddWithValue("@F3204Desc", m.F3204Desc);
                cmd.Parameters.AddWithValue("@F3301", this.NulltoDBNull(m.F3301));
                cmd.Parameters.AddWithValue("@F3302", this.NulltoDBNull(m.F3302));
                cmd.Parameters.AddWithValue("@F3302Desc", m.F3302Desc);
                cmd.Parameters.AddWithValue("@F3401", this.NulltoDBNull(m.F3401));
                cmd.Parameters.AddWithValue("@F3402", this.NulltoDBNull(m.F3402));
                cmd.Parameters.AddWithValue("@F3403", this.NulltoDBNull(m.F3403));
                cmd.Parameters.AddWithValue("@F3403Desc", m.F3403Desc);
                cmd.Parameters.AddWithValue("@Signature", m.Signature);
                cmd.Parameters.AddWithValue("@Remark", m.Remark); //s20231006

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("CECheckTableService.UpdateRecord: " + e.Message);
                return -1;
            }
        }
        //==========================


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