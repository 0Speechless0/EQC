using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class ConstCheckRecImproveFileService : BaseService
    {//抽驗缺失改善檔案上傳
        public List<ConstCheckRecImproveFileModel> GetItem(int seq)
        {
            string sql = @"select * from ConstCheckRecImproveFile where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<ConstCheckRecImproveFileModel>(cmd);
        }
        //上傳照片
        public bool Add(ConstCheckRecImproveFileModel m)
        {
            string sql = "";
            db.BeginTransaction();
            try
            {
                sql = @"
                insert into ConstCheckRecImproveFile(
                    ConstCheckRecImproveSeq,
                    ControllStSeq,
                    ItemGroup,
                    Memo,
                    OriginFileName,
                    UniqueFileName,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq,
                    OrderNo
                ) values (
                    @ConstCheckRecImproveSeq,    
                    @ControllStSeq,
                    @ItemGroup,
                    @Memo,
                    @OriginFileName,
                    @UniqueFileName,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq,
                    (select ISNULL(max(OrderNo),0)+1 from ConstCheckRecImproveFile where ConstCheckRecImproveSeq=@ConstCheckRecImproveSeq and ControllStSeq=@ControllStSeq)
                )";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ConstCheckRecImproveSeq", m.ConstCheckRecImproveSeq);
                cmd.Parameters.AddWithValue("@ControllStSeq", m.ControllStSeq);
                cmd.Parameters.AddWithValue("@ItemGroup", m.ItemGroup);
                cmd.Parameters.AddWithValue("@Memo", m.Memo); 
                cmd.Parameters.AddWithValue("@originFileName", m.OriginFileName);
                cmd.Parameters.AddWithValue("@uniqueFileName", m.UniqueFileName);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());

                db.ExecuteNonQuery(cmd);

                /*sql = @"update ConstCheckRecResult set
                    ResultItem=1
                    where ConstCheckRecImproveSeq=@ConstCheckRecImproveSeq and ControllStSeq=@ControllStSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ConstCheckRecImproveSeq", m.ConstCheckRecImproveSeq);
                cmd.Parameters.AddWithValue("@ControllStSeq", m.ControllStSeq);
                db.ExecuteNonQuery(cmd);*/

                sql = @"update ConstCheckRecImprove set
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                    where Seq=@ConstCheckRecImproveSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ConstCheckRecImproveSeq", m.ConstCheckRecImproveSeq);
                cmd.Parameters.AddWithValue("@ControllStSeq", m.ControllStSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();

                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("ConstCheckRecFileService.Add: " + e.Message);
                log.Info(sql);
                return false;
            }
        }
        
        //鑑驗照片清單
        public List<T> GetPhotos<T>(int constCheckRecSeq, int controllStSeq)
        {
            string sql = @"
                select * from (
                    select cast(0 as tinyint) ItemGroup, Seq, UniqueFileName, Memo, OrderNo from ConstCheckRecFile
                    where ConstCheckRecSeq=@ConstCheckRecSeq
                    and ControllStSeq=@ControllStSeq
                    union all
                    select a.ItemGroup, a.Seq, a.UniqueFileName, a.Memo, a.OrderNo from ConstCheckRecImproveFile a
                    inner join ConstCheckRecImprove b on(b.Seq=a.ConstCheckRecImproveSeq)
                    where b.ConstCheckRecSeq=@ConstCheckRecSeq
                    and a.ControllStSeq=@ControllStSeq
                ) z
                order by z.ItemGroup, z.OrderNo
                ";
            /*string sql = @"
                select
                    Seq,
                    UniqueFileName,
                    Memo
                from ConstCheckRecImproveFile
                where ConstCheckRecImproveSeq=@ConstCheckRecImproveSeq
                and ControllStSeq=@ControllStSeq
                order by OrderNo";*/
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@ConstCheckRecSeq", constCheckRecSeq);
            cmd.Parameters.AddWithValue("@ControllStSeq", controllStSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //刪除照片
        public bool Del(ConstCheckRecImproveFileModel m)
        {
            string sql = "";
            db.BeginTransaction();
            try
            {
                sql = @"delete ConstCheckRecImproveFile where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", m.Seq);

                db.ExecuteNonQuery(cmd);

                sql = @"update ConstCheckRecImprove set
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                    where Seq=@ConstCheckRecImproveSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ConstCheckRecImproveSeq", m.ConstCheckRecImproveSeq);
                cmd.Parameters.AddWithValue("@ControllStSeq", m.ControllStSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();

                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("ConstCheckRecFileService.Del: " + e.Message);
                log.Info(sql);
                return false;
            }
        }
    }
}