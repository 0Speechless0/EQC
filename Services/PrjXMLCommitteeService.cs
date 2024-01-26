using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class PrjXMLCommitteeService : BaseService
    {//決標委員資料
        public List<T> GetCommittee<T>(int prjXMLSeq)
        {
            string sql = @"SELECT
                    Seq,
                    PrjXMLSeq,
                    CName,
                    Kind,
                    Profession,
                    Experience,
                    IsPresence
                FROM PrjXMLCommittee
                where PrjXMLSeq=@PrjXMLSeq
                order by Kind, CName";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@PrjXMLSeq", prjXMLSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //更新
        public int Update(PrjXMLCommitteeModel m)
        {
            Null2Empty(m);
            string sql = @"
                update PrjXMLCommittee set
                    CName = @CName,
                    Kind = @Kind,
                    Profession = @Profession,
                    Experience = @Experience,
                    IsPresence = @IsPresence,
                    ModifyTime = GETDATE(),
                    ModifyUser = @ModifyUserSeq
                where Seq=@Seq
                ";
            try { 
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CName", m.CName);
                cmd.Parameters.AddWithValue("@Kind", m.Kind);
                cmd.Parameters.AddWithValue("@Profession", m.Profession);
                cmd.Parameters.AddWithValue("@Experience", m.Experience);
                cmd.Parameters.AddWithValue("@IsPresence", this.NulltoDBNull(m.IsPresence));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@Seq", m.Seq);

                return db.ExecuteNonQuery(cmd);
            } catch(Exception e) {
                log.Info("PrjXMLCommitteeService.Update: " + e.Message);
                log.Info(sql);
                return -1;
            }
}
        public int Add(PrjXMLCommitteeModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into PrjXMLCommittee (
                    PrjXMLSeq,
                    CName,
                    Kind,
                    Profession,
                    Experience,
                    IsPresence,
                    CreateTime,
                    CreateUser,
                    ModifyTime,
                    ModifyUser
                ) values (
                    @PrjXMLSeq,
                    @CName,
                    @Kind,
                    @Profession,
                    @Experience,
                    @IsPresence,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", m.PrjXMLSeq);
                cmd.Parameters.AddWithValue("@CName", m.CName);
                cmd.Parameters.AddWithValue("@Kind", m.Kind);
                cmd.Parameters.AddWithValue("@Profession", m.Profession);
                cmd.Parameters.AddWithValue("@Experience", m.Experience);
                cmd.Parameters.AddWithValue("@IsPresence", this.NulltoDBNull(m.IsPresence));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                int result = db.ExecuteNonQuery(cmd);
                if (result == 0) return 0;

                cmd.Parameters.Clear();

                string sql1 = @"SELECT IDENT_CURRENT('PrjXMLCommittee') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);
                return Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
            } catch(Exception e) {
                log.Info("PrjXMLCommitteeService.Add: " + e.Message);
                log.Info(sql);
                return 0;
            }
        }

        public int Del(int seq)
        {
            string sql = @"delete PrjXMLCommittee where Seq=@Seq";

            SqlCommand cmd = db.GetCommand(sql);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }
        //批次處裡
        public int ImportData(List<PrjXMLCommitteeModel> items, int prjXMLSeq, string actualBiddingMethod, string bidAwardMethod, ref int iCnt, ref int uCnt, ref string errCnt)
        {
            SqlCommand cmd;
            string sql;
            if (!String.IsNullOrEmpty(actualBiddingMethod))
            {
                try
                {
                    sql = @"update PrjXML set ActualBiddingMethod = @ActualBiddingMethod where Seq=@Seq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@ActualBiddingMethod", actualBiddingMethod);
                    cmd.Parameters.AddWithValue("@Seq", prjXMLSeq);
                    db.ExecuteNonQuery(cmd);
                }
                catch (Exception e)
                {
                    log.Info("PrjXMLCommitteeService.ImportData(Update ActualBiddingMethod): " + e.Message);
                    //log.Info(sql);
                    return -1;
                }
            }
            if (!String.IsNullOrEmpty(actualBiddingMethod))
            {
                sql = @"update PrjXML set BidAwardMethod = @BidAwardMethod where Seq=@Seq";
                try
                {
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@BidAwardMethod", bidAwardMethod);
                    cmd.Parameters.AddWithValue("@Seq", prjXMLSeq);
                    db.ExecuteNonQuery(cmd);
                }
                catch (Exception e)
                {
                    log.Info("PrjXMLCommitteeService.ImportData(Update BidAwardMethod): " + e.Message);
                    //log.Info(sql);
                    return -2;
                }
            }

            int inx = 1;
            foreach(PrjXMLCommitteeModel m in items)
            {
                if (m.Seq == -1)
                {
                    if (Add(m) == 0)
                        errCnt += m.CName + ",";
                    else
                        iCnt++;
                } else
                {
                    if (Update(m) == -1)
                        errCnt += m.CName + ",";
                    else
                        uCnt++;
                }
                inx++;
            }
            return 1;
        }
    }
}