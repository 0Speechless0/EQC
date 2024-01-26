using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EcologicalChecklistService : BaseService
    {//生態檢核
        public const int _DesignStage = 1; //設計階段
        public const int _ConstructionStage = 2; //施工階段

        public List<T> GetItem<T>(int seq, int stage = 1)
        {
            string sql = @"
                select
                    Seq,
                    EngMainSeq,
                    Stage,
                    ToDoChecklit,
                    ChecklistFilename,
                    SelfEvalFilename,
                    PlanDesignRecordFilename,
                    MemberDocFilename,
                    DataCollectDocFilename,
                    ConservMeasFilename,
                    SOCFilename,
                    LivePhoto,
                    EngDiagram,
                    Other,
                    Other2,
                    EngCreatureNameList
                from EcologicalChecklist
                where Seq=@Seq
                and Stage = @Stage
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            cmd.Parameters.AddWithValue("@Stage", stage);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetItemByEngMain<T>(int seq, int stage = 1)
        {
            string sql = @"
                select
                    Seq,
                    EngMainSeq,
                    Stage,
                    ToDoChecklit,
                    ChecklistFilename,
                    SelfEvalFilename,
                    PlanDesignRecordFilename,
                    MemberDocFilename,
                    DataCollectDocFilename,
                    ConservMeasFilename,
                    SOCFilename,
                    LivePhoto,
                    EngDiagram,
                    Other,
                    Other2
                from EcologicalChecklist
                where EngMainSeq=@EngMainSeq
                    and Stage = @Stage
             ";


            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", seq);
            cmd.Parameters.AddWithValue("@Stage", stage);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //新增
        public int Add(int engMainSeq, int stage)
        {
            try
            {
                string sql = @"
                    insert into EcologicalChecklist (
                        EngMainSeq,
                        Stage,
                        ToDoChecklit,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values(
                        @EngMainSeq,
                        @Stage,
                        1,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                cmd.Parameters.AddWithValue("@Stage", stage);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("EcologicalChecklistService.Add: " + e.Message);
                //log.Info(sql);
                return -1;
            }
        }
        //設計階段
        //更新
        public int UpdateDesign(EcologicalChecklistModel m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"update EcologicalChecklist set
                        ToDoChecklit=@ToDoChecklit,
                        ChecklistFilename=@ChecklistFilename,
                        SelfEvalFilename=@SelfEvalFilename,
                        PlanDesignRecordFilename=@PlanDesignRecordFilename,
                        MemberDocFilename=@MemberDocFilename,
                        DataCollectDocFilename=@DataCollectDocFilename,
                        ConservMeasFilename=@ConservMeasFilename,
                        SOCFilename=@SOCFilename,
                        EngCreatureNameList=@EngCreatureNameList,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ToDoChecklit", m.ToDoChecklit);
                cmd.Parameters.AddWithValue("@ChecklistFilename", m.ChecklistFilename);
                cmd.Parameters.AddWithValue("@SelfEvalFilename", m.SelfEvalFilename);
                cmd.Parameters.AddWithValue("@PlanDesignRecordFilename", m.PlanDesignRecordFilename);
                cmd.Parameters.AddWithValue("@MemberDocFilename", m.MemberDocFilename);
                cmd.Parameters.AddWithValue("@DataCollectDocFilename", m.DataCollectDocFilename);
                cmd.Parameters.AddWithValue("@ConservMeasFilename", m.ConservMeasFilename);
                cmd.Parameters.AddWithValue("@EngCreatureNameList", m.EngCreatureNameList);
                cmd.Parameters.AddWithValue("@SOCFilename", m.SOCFilename);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("EcologicalChecklistService.Update: " + e.Message);
                //log.Info(sql);
                return -1;
            }
        }
        //清單 
        public List<T> GetListDesign<T>(int engMainSeq)
        {
            string sql = @"
                select
                    Seq,
                    EngMainSeq,
                    Stage,
                    ToDoChecklit,
                    dbo.DelGuidStr(ChecklistFilename) ChecklistFilename,
                    dbo.DelGuidStr(SelfEvalFilename) SelfEvalFilename,
                    dbo.DelGuidStr(PlanDesignRecordFilename) PlanDesignRecordFilename,
                    dbo.DelGuidStr(MemberDocFilename) MemberDocFilename,
                    dbo.DelGuidStr(DataCollectDocFilename) DataCollectDocFilename,
                    dbo.DelGuidStr(ConservMeasFilename) ConservMeasFilename,
                    dbo.DelGuidStr(SOCFilename) SOCFilename,
                    dbo.DelGuidStr(EngCreatureNameList) EngCreatureNameList 
                from EcologicalChecklist
                where EngMainSeq=@EngMainSeq
                and Stage=@Stage
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@Stage", _DesignStage);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //施工階段
        //清單
        public List<T> GetListConstruction<T>(int engMainSeq)
        {
            string sql = @"
                select
                    Seq,
                    EngMainSeq,
                    Stage,
                    ToDoChecklit,
                    dbo.DelGuidStr(ChecklistFilename) ChecklistFilename,
                    dbo.DelGuidStr(SelfEvalFilename) SelfEvalFilename,
                    dbo.DelGuidStr(PlanDesignRecordFilename) PlanDesignRecordFilename,
                    dbo.DelGuidStr(DataCollectDocFilename) DataCollectDocFilename,
                    dbo.DelGuidStr(ConservMeasFilename) ConservMeasFilename,
                    dbo.DelGuidStr(SOCFilename) SOCFilename,
                    dbo.DelGuidStr(LivePhoto) LivePhoto,
                    dbo.DelGuidStr(EngDiagram) EngDiagram,
                    dbo.DelGuidStr(Other) Other,
                    dbo.DelGuidStr(Other2) Other2
                from EcologicalChecklist
                where EngMainSeq=@EngMainSeq
                and Stage=@Stage
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@Stage", _ConstructionStage);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //更新
        public int UpdateConstruction(EcologicalChecklist2Model m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"update EcologicalChecklist set
                        ChecklistFilename=@ChecklistFilename,
                        SelfEvalFilename=@SelfEvalFilename,
                        PlanDesignRecordFilename=@PlanDesignRecordFilename,
                        DataCollectDocFilename=@DataCollectDocFilename,
                        ConservMeasFilename=@ConservMeasFilename,
                        SOCFilename=@SOCFilename,
                        LivePhoto=@LivePhoto,
                        EngDiagram=@EngDiagram,
                        Other=@Other,
                        Other2 = @Other2,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ChecklistFilename", m.ChecklistFilename);
                cmd.Parameters.AddWithValue("@SelfEvalFilename", m.SelfEvalFilename);
                cmd.Parameters.AddWithValue("@PlanDesignRecordFilename", m.PlanDesignRecordFilename);
                cmd.Parameters.AddWithValue("@DataCollectDocFilename", m.DataCollectDocFilename);
                cmd.Parameters.AddWithValue("@ConservMeasFilename", m.ConservMeasFilename);
                cmd.Parameters.AddWithValue("@SOCFilename", m.SOCFilename);
                cmd.Parameters.AddWithValue("@LivePhoto", m.LivePhoto);
                cmd.Parameters.AddWithValue("@EngDiagram", m.EngDiagram);
                cmd.Parameters.AddWithValue("@Other", m.Other);
                cmd.Parameters.AddWithValue("@Other2", m.Other2);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("EcologicalChecklistService.Update: " + e.Message);
                //log.Info(sql);
                return -1;
            }
        }
    }
}