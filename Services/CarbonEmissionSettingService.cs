using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class CarbonEmissionSettingService : BaseService
    {//碳排量設定
        //s20230502
        public List<T> GetUnitList<T>()
        {
            string sql = @"
                SELECT
                    a.Seq Value,
                    a.Name Text
                from Unit a
                where a.Seq=1
                or (a.Seq>=23 and a.Seq<=36)
                order by a.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetListAll<T>()
        {
            string sql = @"
                SELECT
                    -1 Seq,
	                z.OrderNo,
                    z.UnitName,
                    z.EngUnitSeq,
                    z.CarbonDemandQuantity,
                    z.ApprovedCarbonQuantity,
                    z.CarbonDesignQuantity,
                    z.CarbonConstructionQuantity
                from (
                    select 
                        b.OrderNo,
                        b.Name UnitName,
                        a.ExecUnitSeq EngUnitSeq,
                        sum(a.CarbonDemandQuantity) CarbonDemandQuantity,
                        sum(a.ApprovedCarbonQuantity) ApprovedCarbonQuantity,
                        sum(a.CarbonDesignQuantity) CarbonDesignQuantity,
                        sum(a.CarbonConstructionQuantity) CarbonConstructionQuantity
                    from  EngMain a
                    inner join Unit b on(b.Seq=a.ExecUnitSeq)
                    where (a.ExecUnitSeq>=23 and a.ExecUnitSeq<=33)
	                group by b.OrderNo,a.ExecUnitSeq,b.Name
    
                    union all

                    select
                        b.OrderNo,
                        b.Name UnitName,
                        a.EngUnitSeq,
                        sum(a.CarbonDemandQuantity) CarbonDemandQuantity,
                        sum(a.ApprovedCarbonQuantity) ApprovedCarbonQuantity,
                        ( --s20230531
        	                select sum(CarbonDesignQuantity) from EngMain where EngMain.ExecUnitSeq=a.EngUnitSeq
                        ) CarbonDesignQuantity,
                        ( --s20230531
        	                select sum(CarbonConstructionQuantity) from EngMain where EngMain.ExecUnitSeq=a.EngUnitSeq
                        ) CarbonConstructionQuantity
                    from CarbonEmissionSetting a
                    inner join Unit b on(b.Seq=a.EngUnitSeq)
                    group by b.OrderNo,a.EngUnitSeq,b.Name
                ) z
                order by z.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetList<T>(int year)
        {
            string sql = @"
                SELECT
                    z.Seq,
                    z.OrderNo,
                    z.UnitName,
                    z.EngUnitSeq,
                    z.CarbonDemandQuantity,
                    z.ApprovedCarbonQuantity,
                    z.CarbonDesignQuantity,
                    z.CarbonConstructionQuantity
                from (
                    select 
                        -1 Seq,
                        b.OrderNo,
                        b.Name UnitName,
                        a.ExecUnitSeq EngUnitSeq,
                        sum(a.CarbonDemandQuantity) CarbonDemandQuantity,
                        sum(a.ApprovedCarbonQuantity) ApprovedCarbonQuantity,
                        sum(a.CarbonDesignQuantity) CarbonDesignQuantity,
                        sum(a.CarbonConstructionQuantity) CarbonConstructionQuantity
                    from EngMain a
                    inner join Unit b on(b.Seq=a.ExecUnitSeq)
                    where a.EngYear=@EngYear
                    and (a.ExecUnitSeq>=23 and a.ExecUnitSeq<=33)
                    group by b.OrderNo,a.ExecUnitSeq,b.Name
    
                    union all

                    select
                        a.Seq,
                        b.OrderNo,
                        b.Name UnitName,
                        a.EngUnitSeq,
                        a.CarbonDemandQuantity,
                        a.ApprovedCarbonQuantity,
                        ( --s20230502
        	                select sum(CarbonDesignQuantity) from EngMain where EngMain.ExecUnitSeq=a.EngUnitSeq and EngMain.EngYear=@EngYear
                        ) CarbonDesignQuantity,
                        ( --s20230502
        	                select sum(CarbonConstructionQuantity) from EngMain where EngMain.ExecUnitSeq=a.EngUnitSeq and EngMain.EngYear=@EngYear
                        ) CarbonConstructionQuantity
                    from CarbonEmissionSetting a
                    inner join Unit b on(b.Seq=a.EngUnitSeq)
                    where a.EngYear=@EngYear
                ) z
                order by z.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngYear", year);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //新增
        public int AddRecord(CarbonEmissionSettingModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into CarbonEmissionSetting (
                    EngYear,
                    EngUnitSeq,
                    CarbonDemandQuantity,
                    ApprovedCarbonQuantity,
                    ModifyTime,
                    ModifyUserSeq
                )values(
                    @EngYear,
                    @EngUnitSeq,
                    @CarbonDemandQuantity,
                    @ApprovedCarbonQuantity,
                    GetDate(),
                    @ModifyUserSeq
                )";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@EngYear", m.EngYear);
                cmd.Parameters.AddWithValue("@EngUnitSeq", m.EngUnitSeq);
                cmd.Parameters.AddWithValue("@CarbonDemandQuantity", this.NulltoDBNull(m.CarbonDemandQuantity));
                cmd.Parameters.AddWithValue("@ApprovedCarbonQuantity", this.NulltoDBNull(m.ApprovedCarbonQuantity));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("CarbonEmissionSettingService.AddRecord: " + e.Message);
                return -1;
            }
        }
        //更新
        public int UpdateRecord(CarbonEmissionSettingModel m)
        {
            Null2Empty(m);
            string sql = @"
                update CarbonEmissionSetting set 
                    CarbonDemandQuantity = @CarbonDemandQuantity,
                    ApprovedCarbonQuantity = @ApprovedCarbonQuantity,
                    ModifyTime = GetDate(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@CarbonDemandQuantity", this.NulltoDBNull(m.CarbonDemandQuantity));
                cmd.Parameters.AddWithValue("@ApprovedCarbonQuantity", this.NulltoDBNull(m.ApprovedCarbonQuantity));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("CarbonEmissionSettingService.UpdateRecord: " + e.Message);
                return -1;
            }
        }
        //刪除
        public int DelRecord(int seq)
        {
            string sql;
            try
            {
                sql = @"delete from CarbonEmissionSetting where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("CarbonEmissionSettingService.DelRecord: " + e.Message);
                return -1;
            }
        }
        //儲存匯入資料
        public bool UpdateImport(List<CarbonEmissionSettingImportModel> items)
        {
            SqlCommand cmd;
            string sql = @"
                update EngMain set
                    CarbonDemandQuantity=@CarbonDemandQuantity,
                    ApprovedCarbonQuantity=@ApprovedCarbonQuantity,
                    OfficialApprovedCarbonQuantity=1,
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                where EngNo=@EngNo
                ";
            
            try
            {
                foreach (CarbonEmissionSettingImportModel m in items)
                {
                    Null2Empty(m);
                    if(updateItem(m)==0)
                    {
                        insertItem(m);
                    }
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EngNo", m.EngNo);
                    cmd.Parameters.AddWithValue("@CarbonDemandQuantity", m.CarbonDemandQuantity);
                    cmd.Parameters.AddWithValue("@ApprovedCarbonQuantity", m.ApprovedCarbonQuantity);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                return true;
            }
            catch (Exception e)
            {
                log.Info("CarbonEmissionSettingService.UpdateImport: " + e.Message);
                return false;
            }
        }

        private int updateItem(CarbonEmissionSettingImportModel m)
        {
            string sql = @"
                update CarbonEmissionSettingImport set
                    EngYear=@EngYear,
                    EngUnitSeq=@EngUnitSeq,
                    CarbonDemandQuantity=@CarbonDemandQuantity,
                    ApprovedCarbonQuantity=@ApprovedCarbonQuantity,
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                where EngNo=@EngNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngNo", m.EngNo);
            cmd.Parameters.AddWithValue("@EngYear", m.EngYear);
            cmd.Parameters.AddWithValue("@EngUnitSeq", m.EngUnitSeq);
            cmd.Parameters.AddWithValue("@CarbonDemandQuantity", m.CarbonDemandQuantity);
            cmd.Parameters.AddWithValue("@ApprovedCarbonQuantity", m.ApprovedCarbonQuantity);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
            return db.ExecuteNonQuery(cmd);
        }
        private int insertItem(CarbonEmissionSettingImportModel m)
        {
            string sql = @"
                insert into CarbonEmissionSettingImport (
                    EngYear,
                    EngUnitSeq,
                    EngNo,
                    CarbonDemandQuantity,
                    ApprovedCarbonQuantity,
                    ModifyTime,
                    ModifyUserSeq
                )values(
                    @EngYear,
                    @EngUnitSeq,
                    @EngNo,
                    @CarbonDemandQuantity,
                    @ApprovedCarbonQuantity,
                    GetDate(),
                    @ModifyUserSeq
                )
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngYear", m.EngYear);
            cmd.Parameters.AddWithValue("@EngUnitSeq", m.EngUnitSeq);
            cmd.Parameters.AddWithValue("@EngNo", m.EngNo);
            cmd.Parameters.AddWithValue("@CarbonDemandQuantity", m.CarbonDemandQuantity);
            cmd.Parameters.AddWithValue("@ApprovedCarbonQuantity", m.ApprovedCarbonQuantity);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
            return db.ExecuteNonQuery(cmd);
        }
    }
}