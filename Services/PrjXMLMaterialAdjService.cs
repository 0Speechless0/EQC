using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class PrjXMLMaterialAdjService : BaseService
    {//物料調整

        //新增
        public int NewItem(int prjXMLSeq)
        {
            string sql = @"insert into PrjXMLMaterialAdj(
                    PrjXMLSeq,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                )values(
                    @PrjXMLSeq,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@PrjXMLSeq", prjXMLSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("PrjXMLMaterialAdjService.NewItem: " + e.Message);
                //log.Info(sql);
                return 0;
            }
        }
        //清單
        public List<T> GetItemByPrjXMLSeq<T>(int prjXMLSeq)
        {
            string sql = @"
                select
                    Seq,
                    --PrjXMLSeq,
                    IsChangeContract,
                    IsAppendAgreement,
                    IsForYear97,
                    ForMade97Month,
                    IsAppleBeforeClose,
                    PriceAdjust,
                    BudgetAvail
                from PrjXMLMaterialAdj
                where PrjXMLSeq=@PrjXMLSeq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@PrjXMLSeq", prjXMLSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }        
        //更新
        public int Update(PrjXMLMaterialAdjModel m)
        {
            Null2Empty(m);
            string sql = @"
                update PrjXMLMaterialAdj set
                    IsChangeContract =@IsChangeContract,
                    IsAppendAgreement =@IsAppendAgreement,
                    IsForYear97 =@IsForYear97,
                    ForMade97Month =@ForMade97Month,
                    IsAppleBeforeClose =@IsAppleBeforeClose,
                    PriceAdjust =@PriceAdjust,
                    BudgetAvail =@BudgetAvail,
                    ModifyTime=GetDate(),
                    ModifyUserSeq=@ModifyUserSeq
                where Seq=@Seq
                ";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@IsChangeContract", this.NulltoDBNull(m.IsChangeContract));
                cmd.Parameters.AddWithValue("@IsAppendAgreement", this.NulltoDBNull(m.IsAppendAgreement));
                cmd.Parameters.AddWithValue("@IsForYear97", this.NulltoDBNull(m.IsForYear97));
                cmd.Parameters.AddWithValue("@ForMade97Month", m.ForMade97Month);
                cmd.Parameters.AddWithValue("@IsAppleBeforeClose", this.NulltoDBNull(m.IsAppleBeforeClose));
                cmd.Parameters.AddWithValue("@PriceAdjust", this.NulltoDBNull(m.PriceAdjust));
                cmd.Parameters.AddWithValue("@BudgetAvail", this.NulltoDBNull(m.BudgetAvail));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("PrjXMLMaterialAdjService.Update: " + e.Message);
                //log.Info(sql);
                return 0;
            }
        }
    }
}