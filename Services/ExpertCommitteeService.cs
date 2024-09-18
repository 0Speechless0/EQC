using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class ExpertCommitteeService : BaseService
    {//專家委員
        public List<T> GetList<T>()
        {
            string sql = @"SELECT
                    Seq,
                    ECName,
                    ECKind,
                    ECPosition,
                    ECEmail,
                    ECTel,
                    ECMobile,
                    ECMainSkill
                FROM ExpertCommittee where IsDeleted=0
                order by ECKind, ECName";
            SqlCommand cmd = db.GetCommand(sql);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public List<T> GetCommittee<T>(int seq)
        {
            string sql = @"SELECT
                    Seq,
                    ECName,
                    ECBirthday,
                    ECId,
                    ECKind,
                    ECPosition,
                    ECUnit,
                    ECEmail,
                    ECTel,
                    ECMobile,
                    ECFax,
                    ECAddr1,
                    ECAddr2,
                    ECMainSkill,
                    ECSecSkill,
                    ECBankNo,
                    ECDiet,
                    ECNeed,
                    ECMemo
                FROM ExpertCommittee
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public int Update(ExpertCommitteeModel m)
        {
            Null2Empty(m);
            string sql = @"
                update ExpertCommittee set
                    ECName = @ECName,
                    ECBirthday = @ECBirthday,
                    ECId = @ECId,
                    ECKind = @ECKind,
                    ECPosition = @ECPosition,
                    ECUnit = @ECUnit,
                    ECEmail = @ECEmail,
                    ECTel = @ECTel,
                    ECMobile = @ECMobile,
                    ECFax = @ECFax,
                    ECAddr1 = @ECAddr1,
                    ECAddr2 = @ECAddr2,
                    ECMainSkill = @ECMainSkill,
                    ECSecSkill = @ECSecSkill,
                    ECBankNo = @ECBankNo,
                    ECDiet = @ECDiet,
                    ECNeed = @ECNeed,
                    ECMemo = @ECMemo,
                    ModifyTime = GETDATE(),
                    ModifyUser = @ModifyUserSeq
                where Seq=@Seq";
            try { 
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ECName", m.ECName);
                cmd.Parameters.AddWithValue("@ECBirthday", this.NulltoDBNull(m.ECBirthday));
                cmd.Parameters.AddWithValue("@ECId", m.ECId);
                cmd.Parameters.AddWithValue("@ECKind", m.ECKind);
                cmd.Parameters.AddWithValue("@ECPosition", m.ECPosition);
                cmd.Parameters.AddWithValue("@ECUnit", m.ECUnit);
                cmd.Parameters.AddWithValue("@ECEmail", m.ECEmail);
                cmd.Parameters.AddWithValue("@ECTel", m.ECTel);
                cmd.Parameters.AddWithValue("@ECMobile", m.ECMobile);
                cmd.Parameters.AddWithValue("@ECFax", m.ECFax);
                cmd.Parameters.AddWithValue("@ECAddr1", m.ECAddr1);
                cmd.Parameters.AddWithValue("@ECAddr2", m.ECAddr2);
                cmd.Parameters.AddWithValue("@ECMainSkill", m.ECMainSkill);
                cmd.Parameters.AddWithValue("@ECSecSkill", m.ECSecSkill);
                cmd.Parameters.AddWithValue("@ECBankNo", m.ECBankNo);
                cmd.Parameters.AddWithValue("@ECDiet", this.NulltoDBNull(m.ECDiet));
                cmd.Parameters.AddWithValue("@ECNeed", m.ECNeed);
                cmd.Parameters.AddWithValue("@ECMemo", m.ECMemo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@Seq", m.Seq);

                return db.ExecuteNonQuery(cmd);
            } catch(Exception e) {
                log.Info("ExpertCommitteeService.Update: " + e.Message);
                log.Info(sql);
                return -1;
            }
}
        public int Add(ExpertCommitteeModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into ExpertCommittee (
                    ECName,
                    ECBirthday,
                    ECId,
                    ECKind,
                    ECPosition,
                    ECUnit,
                    ECEmail,
                    ECTel,
                    ECMobile,
                    ECFax,
                    ECAddr1,
                    ECAddr2,
                    ECMainSkill,
                    ECSecSkill,
                    ECBankNo,
                    ECDiet,
                    ECNeed,
                    ECMemo,
                    OrderNo,
                    IsDeleted,
                    CreateTime,
                    CreateUser,
                    ModifyTime,
                    ModifyUser
                ) values (
                    @ECName,
                    @ECBirthday,
                    @ECId,
                    @ECKind,
                    @ECPosition,
                    @ECUnit,
                    @ECEmail,
                    @ECTel,
                    @ECMobile,
                    @ECFax,
                    @ECAddr1,
                    @ECAddr2,
                    @ECMainSkill,
                    @ECSecSkill,
                    @ECBankNo,
                    @ECDiet,
                    @ECNeed,
                    @ECMemo,
                    0,
                    0,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ECName", m.ECName);
                cmd.Parameters.AddWithValue("@ECBirthday", this.NulltoDBNull(m.ECBirthday));
                cmd.Parameters.AddWithValue("@ECId", m.ECId);
                cmd.Parameters.AddWithValue("@ECKind", m.ECKind);
                cmd.Parameters.AddWithValue("@ECPosition", m.ECPosition);
                cmd.Parameters.AddWithValue("@ECUnit", m.ECUnit);
                cmd.Parameters.AddWithValue("@ECEmail", m.ECEmail);
                cmd.Parameters.AddWithValue("@ECTel", m.ECTel);
                cmd.Parameters.AddWithValue("@ECMobile", m.ECMobile);
                cmd.Parameters.AddWithValue("@ECFax", m.ECFax);
                cmd.Parameters.AddWithValue("@ECAddr1", m.ECAddr1);
                cmd.Parameters.AddWithValue("@ECAddr2", m.ECAddr2);
                cmd.Parameters.AddWithValue("@ECMainSkill", m.ECMainSkill);
                cmd.Parameters.AddWithValue("@ECSecSkill", m.ECSecSkill);
                cmd.Parameters.AddWithValue("@ECBankNo", m.ECBankNo);
                cmd.Parameters.AddWithValue("@ECDiet", this.NulltoDBNull(m.ECDiet));
                cmd.Parameters.AddWithValue("@ECNeed", m.ECNeed);
                cmd.Parameters.AddWithValue("@ECMemo", m.ECMemo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                int result = db.ExecuteNonQuery(cmd);
                if (result == 0) return 0;

                cmd.Parameters.Clear();

                string sql1 = @"SELECT IDENT_CURRENT('ExpertCommittee') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);
                return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
            } catch(Exception e) {
                log.Info("ExpertCommitteeService.Add: " + e.Message);
                log.Info(sql);
                return 0;
            }
        }

        public int? Del(int seq)
        {
            string sql = @"delete e from ExpertCommittee e

            left join OutCommittee o on o.ExpertCommitteeSeq = e.Seq
            where e.Seq=@Seq and o.Seq is null;
            select s.PrjXMLSeq  from  OutCommittee o 
            inner join SuperviseEng s on o.SuperviseEngSeq = s.Seq
             where o.ExpertCommitteeSeq=@Seq;
            ";

            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return (int?)db.ExecuteScalar(cmd);
        }
        //批次處裡
        public void ImportData(List<ExpertCommitteeModel> items, ref int iCnt, ref int uCnt, ref string errCnt)
        {
            SqlCommand cmd;
            string sql;
            int inx = 1;
            foreach(ExpertCommitteeModel m in items)
            {
                if (String.IsNullOrEmpty(m.ECId))
                {
                    if (Add(m) == 0)
                        errCnt += inx.ToString() + ",";
                    else
                        iCnt++;
                } else
                {
                    sql = @"SELECT Seq FROM ExpertCommittee where ECId=@ECId";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.AddWithValue("@ECId", m.ECId);
                    DataTable dt = db.GetDataTable(cmd);
                    if (dt.Rows.Count == 1) {
                        m.Seq = Convert.ToInt32(dt.Rows[0]["Seq"].ToString());
                        if (Update(m) == -1)
                            errCnt += inx.ToString() + ",";
                        else
                            uCnt++;
                    } else
                    {
                        if (Add(m) == 0)
                            errCnt += inx.ToString() + ",";
                        else
                            iCnt++;
                    }
                }
                inx++;
            }
        }
    }
}