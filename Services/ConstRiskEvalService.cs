using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class ConstRiskEvalService : BaseService
    {//設計階段施工風險評估
        public List<T> GetItem<T>(int seq)
        {
            string sql = @"
                select
                    Seq,
                    EngMainSeq,
                    ApprovedDate,
                    Descr,
                    FileName
                from ConstRiskEval
                where Seq=@Seq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //新增
        public int Add(ConstRiskEvalModel m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"
                    insert into ConstRiskEval (
                        EngMainSeq,
                        ApprovedDate,
                        Descr,
                        FileName,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values(
                        @EngMainSeq,
                        @ApprovedDate,
                        @Descr,
                        @FileName,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", m.EngMainSeq);
                cmd.Parameters.AddWithValue("@ApprovedDate", m.ApprovedDate);
                cmd.Parameters.AddWithValue("@Descr", m.Descr);
                cmd.Parameters.AddWithValue("@FileName", m.FileName);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("ConstRiskEvalService.Add: " + e.Message);
                //log.Info(sql);
                return -1;
            }
        }
        //更新
        public int Update(ConstRiskEvalModel m)
        {
            Null2Empty(m);
            try
            {
                string sql;
                if (String.IsNullOrEmpty(m.FileName))
                {
                    sql = @"update ConstRiskEval set
                        ApprovedDate=@ApprovedDate,
                        Descr=@Descr,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                }
                else
                {
                    sql = @"update ConstRiskEval set
                        ApprovedDate=@ApprovedDate,
                        Descr=@Descr,
                        FileName=@FileName,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                }

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ApprovedDate", m.ApprovedDate);
                cmd.Parameters.AddWithValue("@Descr", m.Descr);
                cmd.Parameters.AddWithValue("@FileName", m.FileName);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("ConstRiskEvalService.Update: " + e.Message);
                //log.Info(sql);
                return -1;
            }
        }
        //刪除
        public int Del(int seq)
        {
            string sql = @"delete from ConstRiskEval where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }
        //清單
        public List<T> GetList<T>(int engMainSeq)
        {
            string sql = @"
                select
                    Seq,
                    EngMainSeq,
                    ApprovedDate,
                    Descr,
                    dbo.DelGuidStr(FileName) FileName
                from ConstRiskEval
                where EngMainSeq=@EngMainSeq
                Order by ApprovedDate
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}