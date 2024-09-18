using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EngMaterialDeviceListService : BaseService
    {//材料設備送審清冊
        /*public List<T> GetList<T>(int engMainSeq)
        {
            string sql = @"
                SELECT
                    Seq,
                    MDName,
                    CreateTime,
                    ModifyTime
                FROM EngMaterialDeviceList
                where EngMainSeq=@EngMainSeq
                order by OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }*/

        public int GetListCount(int engMainSeq)
        {
            string sql = @"
                SELECT count(Seq) total FROM EngMaterialDeviceList where EngMainSeq=@EngMainSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
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
        public List<T> GetList<T>(int engMainSeq, int pageIndex, int perPage)
        {
            string sql = @"SELECT
                a.Seq,
                a.DataType,
                a.DataKeep,
                a.OrderNo,
                a.ExcelNo,
                a.ItemNo,
                a.MDName,
                a.FlowCharOriginFileName,
                a.IsAuditVendor,
                a.IsAuditCatalog,
                a.IsAuditReport,
                a.IsAuditSample,
                a.OtherAudit,
                a.CreateTime,
                a.ModifyTime,
                b.ContactQty,
                b.ContactUnit,
                b.IsSampleTest,
                b.IsFactoryInsp,
                (
                	SELECT count(z.Seq) total FROM EngMaterialDeviceControlSt z
                	where z.EngMaterialDeviceListSeq=a.Seq
                ) stdCount
                FROM EngMaterialDeviceList a
                left outer join EngMaterialDeviceSummary b on(b.EngMaterialDeviceListSeq=a.Seq)
				inner join
				(
					select Min(Seq) Seq  from EngMaterialDeviceSummary
					group by EngMaterialDeviceListSeq
				)
				c on( c.Seq=b.Seq)
                where EngMainSeq=@EngMainSeq
                order by a.DataKeep desc, a.OrderNo
                OFFSET @pageIndex ROWS
				FETCH FIRST @perPage ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@pageIndex", perPage * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@perPage", perPage);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //s20230624
        public List<T> GetListAll<T>(int engMainSeq)
        {
            string sql = @"SELECT
                Seq,
                OrderNo,
                EngMainSeq,
                ParentSeq,
                ItemNo,
                MDName,
                ExcelNo,
                FlowCharOriginFileName,
                FlowCharUniqueFileName,
                DataKeep,
                DataType,
                IsAuditVendor,
                IsAuditCatalog,
                IsAuditReport,
                IsAuditSample,
                OtherAudit
                FROM EngMaterialDeviceList
                where EngMainSeq=@EngMainSeq
                order by OrderNo
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public bool Update(EngMaterialDeviceListVModel m, bool check = true)
        {
            Null2Empty(m);
            string sql = @"";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql =  check ? @"
                update EngMaterialDeviceList set
                    OrderNo = @OrderNo,
                    ItemNo = @ItemNo,
                    MDName = @MDName,
                    DataKeep = @DataKeep,
                    IsAuditVendor = @IsAuditVendor,
                    IsAuditCatalog = @IsAuditCatalog,
                    IsAuditReport = @IsAuditReport,
                    IsAuditSample = @IsAuditSample,
                    OtherAudit = @OtherAudit,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                    from EngMaterialDeviceList　cc
                    left join EngMaterialDeviceTestSummary em on cc.Seq = em.EngMaterialDeviceListSeq

                where cc.Seq=@Seq and em.Seq is null" : @"

                update EngMaterialDeviceList set
                    OrderNo = @OrderNo,
                    ItemNo = @ItemNo,
                    MDName = @MDName,
                    DataKeep = @DataKeep,
                    IsAuditVendor = @IsAuditVendor,
                    IsAuditCatalog = @IsAuditCatalog,
                    IsAuditReport = @IsAuditReport,
                    IsAuditSample = @IsAuditSample,
                    OtherAudit = @OtherAudit,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq where Seq = @Seq";

                
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@OrderNo", m.OrderNo);
                cmd.Parameters.AddWithValue("@ItemNo", m.ItemNo);
                cmd.Parameters.AddWithValue("@MDName", m.MDName);
                cmd.Parameters.AddWithValue("@DataKeep", m.DataKeep);
                cmd.Parameters.AddWithValue("@IsAuditVendor", m.IsAuditVendor); //s20230327
                cmd.Parameters.AddWithValue("@IsAuditCatalog", m.IsAuditCatalog); //s20230327
                cmd.Parameters.AddWithValue("@IsAuditReport", m.IsAuditReport); //s20230327
                cmd.Parameters.AddWithValue("@IsAuditSample", m.IsAuditSample); //s20230327
                cmd.Parameters.AddWithValue("@OtherAudit", m.OtherAudit); //s20230327
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                if (db.ExecuteNonQuery(cmd) == 0)
                {
                    db.TransactionRollback(); 
                    return false;
                }

                sql = @"
                update EngMaterialDeviceSummary set
                    OrderNo = @OrderNo,
                    ItemNo = @ItemNo,
                    MDName = @MDName,
                    ContactQty = @ContactQty,
                    ContactUnit = @ContactUnit,
                    IsSampleTest = @IsSampleTest,
                    IsFactoryInsp = @IsFactoryInsp,
                    IsAuditVendor = @IsAuditVendor,
                    IsAuditCatalog = @IsAuditCatalog,
                    IsAuditReport = @IsAuditReport,
                    IsAuditSample = @IsAuditSample,
                    OtherAudit = @OtherAudit,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where EngMaterialDeviceListSeq=@EngMaterialDeviceListSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@OrderNo", m.OrderNo);
                cmd.Parameters.AddWithValue("@ItemNo", m.ItemNo);
                cmd.Parameters.AddWithValue("@MDName", m.MDName);
                cmd.Parameters.AddWithValue("@ContactQty", m.ContactQty);
                cmd.Parameters.AddWithValue("@ContactUnit", m.ContactUnit);
                cmd.Parameters.AddWithValue("@IsSampleTest", m.IsSampleTest);
                cmd.Parameters.AddWithValue("@IsFactoryInsp", m.IsFactoryInsp);
                cmd.Parameters.AddWithValue("@IsAuditVendor", m.IsAuditVendor); //s20230327
                cmd.Parameters.AddWithValue("@IsAuditCatalog", m.IsAuditCatalog); //s20230327
                cmd.Parameters.AddWithValue("@IsAuditReport", m.IsAuditReport); //s20230327
                cmd.Parameters.AddWithValue("@IsAuditSample", m.IsAuditSample); //s20230327
                cmd.Parameters.AddWithValue("@OtherAudit", m.OtherAudit); //s20230327
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", m.Seq);
                int result = db.ExecuteNonQuery(cmd);

                if(result==0)
                {
                    sql = @"
                    insert into EngMaterialDeviceSummary(
                        EngMaterialDeviceListSeq,
                        OrderNo,
                        ItemNo,
                        MDName,
                        ContactQty,
                        ContactUnit,
                        IsSampleTest,
                        IsAuditVendor,
                        IsAuditCatalog,
                        IsAuditReport,
                        IsAuditSample,
                        OtherAudit,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngMaterialDeviceListSeq,
                        @OrderNo,
                        @ItemNo,
                        @MDName,
                        @ContactQty,
                        @ContactUnit,
                        @IsSampleTest,
                        @IsAuditVendor,
                        @IsAuditCatalog,
                        @IsAuditReport,
                        @IsAuditSample,
                        @OtherAudit,
                        GETDATE(),
                        @ModifyUserSeq,
                        GETDATE(),
                        @ModifyUserSeq
                    )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@OrderNo", m.OrderNo);
                    cmd.Parameters.AddWithValue("@ItemNo", m.ItemNo);
                    cmd.Parameters.AddWithValue("@MDName", m.MDName);
                    cmd.Parameters.AddWithValue("@ContactQty", m.ContactQty);
                    cmd.Parameters.AddWithValue("@ContactUnit", m.ContactUnit);
                    cmd.Parameters.AddWithValue("@IsSampleTest", m.IsSampleTest);
                    cmd.Parameters.AddWithValue("@IsAuditVendor", m.IsAuditVendor); //s20230327
                    cmd.Parameters.AddWithValue("@IsAuditCatalog", m.IsAuditCatalog); //s20230327
                    cmd.Parameters.AddWithValue("@IsAuditReport", m.IsAuditReport); //s20230327
                    cmd.Parameters.AddWithValue("@IsAuditSample", m.IsAuditSample); //s20230327
                    cmd.Parameters.AddWithValue("@OtherAudit", m.OtherAudit); //s20230327
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", m.Seq);
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                db.Connection.Close();
                return true;
            } catch(Exception e)
            {
                db.TransactionRollback();
                db.Connection.Close();
                log.Info("EngMaterialDeviceListService.Update: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        public bool UpdateKeep(List<EngMaterialDeviceListVModel> items)
        {
            string sql = @"";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                //Null2Empty(m);
                sql = @"
                update EngMaterialDeviceList set
                    DataKeep = @DataKeep,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
                cmd = db.GetCommand(sql);

                int userSeq = getUserSeq();
                foreach (EngMaterialDeviceListVModel m in items)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@DataKeep", m.DataKeep);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                    cmd.Parameters.AddWithValue("@Seq", m.Seq);
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                db.Connection.Close();
                return true;
            } catch(Exception e)
            {
                db.TransactionRollback();
                db.Connection.Close();
                log.Info("EngMaterialDeviceListService.UpdateKeep: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        public List<T> GetItemBySeq<T>(int seq)
        {
            string sql = @"SELECT
                a.Seq,
                a.DataType,
                a.DataKeep,
                a.OrderNo,
                a.ExcelNo,
                a.ItemNo,
                a.MDName,
                a.FlowCharOriginFileName,
                a.CreateTime,
                a.CreateTime,
                a.ModifyTime,
                b.ContactQty,
                b.ContactUnit,
                b.IsSampleTest
                FROM EngMaterialDeviceList a
                left outer join EngMaterialDeviceSummary b on(b.EngMaterialDeviceListSeq=a.Seq)
                where a.Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        public List<EngMaterialDeviceListVModel> GetItemFileInfoBySeq(int seq)
        {
            string sql = @"SELECT
                Seq,
                FlowCharOriginFileName,
                FlowCharUniqueFileName
                FROM EngMaterialDeviceList
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<EngMaterialDeviceListVModel>(cmd);
        }

        public int UpdateUploadFile(int seq, string originFileName, string uniqueFileName)
        {
            string sql = @"
                update EngMaterialDeviceList set
                    FlowCharOriginFileName = @originFileName,
                    FlowCharUniqueFileName = @uniqueFileName,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@originFileName", originFileName);
            cmd.Parameters.AddWithValue("@uniqueFileName", uniqueFileName);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.ExecuteNonQuery(cmd);
        }

        public bool Add(EngMaterialDeviceListModel m)
        {
            Null2Empty(m);
            string sql = @"";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql = @"
                    insert into EngMaterialDeviceList (
                        DataType,
                        DataKeep,
                        EngMainSeq,
                        ExcelNo,
                        MDName,
                        FlowCharOriginFileName,
                        FlowCharUniqueFileName,
                        OrderNo,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @DataType,
                        @DataKeep,
                        @EngMainSeq,
                        @ExcelNo,
                        @MDName,
                        @FlowCharOriginFileName,
                        @FlowCharUniqueFileName,
                        (select max(OrderNo)+1 from EngMaterialDeviceList a where a.EngMainSeq=@EngMainSeq),
                        GETDATE(),
                        @ModifyUserSeq,
                        GETDATE(),
                        @ModifyUserSeq
                    )";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@DataType", m.DataType);
                cmd.Parameters.AddWithValue("@DataKeep", m.DataKeep);
                cmd.Parameters.AddWithValue("@EngMainSeq", m.EngMainSeq);
                cmd.Parameters.AddWithValue("@ExcelNo", m.ExcelNo);
                cmd.Parameters.AddWithValue("@MDName", m.MDName);
                cmd.Parameters.AddWithValue("@FlowCharOriginFileName", m.FlowCharOriginFileName);
                cmd.Parameters.AddWithValue("@FlowCharUniqueFileName", m.FlowCharUniqueFileName);
                //cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('EngMaterialDeviceList') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);
                m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                sql = @"
                    insert into EngMaterialDeviceSummary(
                        EngMaterialDeviceListSeq,
                        OrderNo,
                        ItemNo,
                        MDName,
                        ContactQty,
                        IsSampleTest,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngMaterialDeviceListSeq,
                        (select max(OrderNo) from EngMaterialDeviceList a where a.EngMainSeq=@EngMainSeq),
                        @ItemNo,
                        @MDName,
                        0,
                        1,
                        GETDATE(),
                        @ModifyUserSeq,
                        GETDATE(),
                        @ModifyUserSeq
                    )";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ItemNo", m.ItemNo);
                cmd.Parameters.AddWithValue("@MDName", m.MDName);
                cmd.Parameters.AddWithValue("@EngMainSeq", m.EngMainSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", m.Seq);
                db.ExecuteNonQuery(cmd);
                db.TransactionCommit();
                db.Connection.Close();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                db.Connection.Close();
                log.Info("EngMaterialDeviceListService.Add: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        public bool Delete(int seq)
        {
            string sql = @"";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql = @"delete EngMaterialDeviceSummary where EngMaterialDeviceListSeq=@EngMaterialDeviceListSeq";

                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", seq);

                db.ExecuteNonQuery(cmd);

                sql = @"delete EngMaterialDeviceList where Seq=@Seq";

                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", seq);

                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("EngMaterialDeviceListService.Delete: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        //材料設備送審管制總表 s20230624
        public List<T> GetEMDSummaryList<T>(int engMaterialDeviceListSeq)
        {
            string sql = @"
                SELECT 
                    b.ItemType
                    ,b.Seq
                    ,b.[EngMaterialDeviceListSeq]
                    ,b.[OrderNo]
                    ,b.[ItemNo]
                    ,b.[MDName]
                    ,b.[ContactQty]
                    ,b.[ContactUnit]
                    ,b.[IsSampleTest]
                    ,b.[SchAuditDate]
                    ,b.[RealAutitDate]
                    ,b.[IsFactoryInsp]
                    ,b.[FactoryInspDate]
                    ,b.[IsAuditVendor]
                    ,b.[VendorName]
                    ,b.[VendorTaxId]
                    ,b.[VendorAddr]
                    ,b.[IsAuditCatalog]
                    ,b.[IsAuditReport]
                    ,b.[IsAuditSample]
                    ,b.[OtherAudit]
                    ,b.[AuditDate]
                    ,b.[AuditResult]
                    ,b.[ArchiveNo]
                    ,b.ItemType
                    ,b.RefSeq
                from EngMaterialDeviceSummary b
                WHERE b.EngMaterialDeviceListSeq=@EngMaterialDeviceListSeq
                ORDER BY b.OrderNo
		        ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", engMaterialDeviceListSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}