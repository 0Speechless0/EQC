using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class ConstCheckRecFileService : BaseService
    {//抽驗紀錄上傳檔案
        public List<ConstCheckRecFileModel> GetItem(int seq)
        {
            string sql = @"select * from ConstCheckRecFile where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<ConstCheckRecFileModel>(cmd);
        }
        //上傳照片
        public bool Add(ConstCheckRecFileModel m)
        {
            string sql = "";
            db.BeginTransaction();
            try
            {
                sql = @"
                insert into ConstCheckRecFile(
                    ConstCheckRecSeq,
                    ControllStSeq,
                    Memo,
                    IsSign,
                    OriginFileName,
                    UniqueFileName,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq,
                    OrderNo,
                    RESTful
                ) values (
                    @ConstCheckRecSeq,    
                    @ControllStSeq,
                    @Memo,
                    @IsSign,
                    @OriginFileName,
                    @UniqueFileName,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq,
                    (select ISNULL(max(OrderNo),0)+1 from ConstCheckRecFile where ConstCheckRecSeq=@ConstCheckRecSeq and ControllStSeq=@ControllStSeq),
                    @restful
                )";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ConstCheckRecSeq", m.ConstCheckRecSeq);
                cmd.Parameters.AddWithValue("@ControllStSeq", m.ControllStSeq);
                cmd.Parameters.AddWithValue("@Memo", m.Memo); 
                cmd.Parameters.AddWithValue("@IsSign", NulltoDBNull(m.IsSign));
                cmd.Parameters.AddWithValue("@originFileName", m.OriginFileName);
                cmd.Parameters.AddWithValue("@uniqueFileName", m.UniqueFileName);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@restful", m.RESTful);
                db.ExecuteNonQuery(cmd);

                /*sql = @"update ConstCheckRecResult set
                    ResultItem=1
                    where ConstCheckRecSeq=@ConstCheckRecSeq and ControllStSeq=@ControllStSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ConstCheckRecSeq", m.ConstCheckRecSeq);
                cmd.Parameters.AddWithValue("@ControllStSeq", m.ControllStSeq);
                db.ExecuteNonQuery(cmd);*/

                sql = @"update ConstCheckRec set
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                    where Seq=@ConstCheckRecSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ConstCheckRecSeq", m.ConstCheckRecSeq);
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
                select
                    Seq,
                    UniqueFileName,
                    Memo,
                    RESTful
                from ConstCheckRecFile
                where ConstCheckRecSeq=@ConstCheckRecSeq
                and ControllStSeq=@ControllStSeq
                order by OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@ConstCheckRecSeq", constCheckRecSeq);
            cmd.Parameters.AddWithValue("@ControllStSeq", controllStSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //刪除照片
        public bool Del(ConstCheckRecFileModel m)
        {
            string sql = "";
            db.BeginTransaction();
            try
            {
                sql = @"delete ConstCheckRecFile where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", m.Seq);

                db.ExecuteNonQuery(cmd);

                sql = @"update ConstCheckRec set
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                    where Seq=@ConstCheckRecSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ConstCheckRecSeq", m.ConstCheckRecSeq);
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

        //更新有無蜂窩欄位
        public bool update(ConstCheckRecFileModel m)
        {
            string sql = "";
            db.BeginTransaction();
            try
            {
                sql = @"update ConstCheckRecFile set 
                    RESTful = @RESTful,
                    Memo = @Memo 
                    where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@RESTful", m.RESTful);
                cmd.Parameters.AddWithValue("@Memo", m.Memo);

                db.ExecuteNonQuery(cmd);

                sql = @"update ConstCheckRec set
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                    where Seq=@ConstCheckRecSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ConstCheckRecSeq", m.ConstCheckRecSeq);
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
        public bool Add1(ConstCheckRecFileModel m)
        {
            string sql = "";
            db.BeginTransaction();
            try
            {
                sql = @"
                insert into ConstCheckRecFile(
                    ConstCheckRecSeq,
                    ControllStSeq,
                    Memo,
                    IsSign,
                    OriginFileName,
                    UniqueFileName,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq,
                    OrderNo
                ) values (
                    @ConstCheckRecSeq,    
                    @ControllStSeq,
                    @Memo,
                    @IsSign,
                    @OriginFileName,
                    @UniqueFileName,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq,
                    (select ISNULL(max(OrderNo),0)+1 from ConstCheckRecFile where ConstCheckRecSeq=@ConstCheckRecSeq and ControllStSeq=@ControllStSeq)
                )";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ConstCheckRecSeq", m.ConstCheckRecSeq);
                cmd.Parameters.AddWithValue("@ControllStSeq", m.ControllStSeq);
                cmd.Parameters.AddWithValue("@Memo", m.Memo);
                cmd.Parameters.AddWithValue("@IsSign", NulltoDBNull(m.IsSign));
                cmd.Parameters.AddWithValue("@originFileName", m.OriginFileName);
                cmd.Parameters.AddWithValue("@uniqueFileName", m.UniqueFileName);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                /*sql = @"update ConstCheckRecResult set
                    ResultItem=1
                    where ConstCheckRecSeq=@ConstCheckRecSeq and ControllStSeq=@ControllStSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ConstCheckRecSeq", m.ConstCheckRecSeq);
                cmd.Parameters.AddWithValue("@ControllStSeq", m.ControllStSeq);
                db.ExecuteNonQuery(cmd);*/

                sql = @"update ConstCheckRec set
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                    where Seq=@ConstCheckRecSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ConstCheckRecSeq", m.ConstCheckRecSeq);
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
    }
}