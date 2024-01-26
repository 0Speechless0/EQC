using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class ECSchProgressPayItemService : BaseService
    {//工程變更 - 預定進度
        public List<T> GetDateList<T>(int engMainSeq)
        {
            string sql = @"
                    select Version, SPDate from (
                        select DISTINCT b.Seq Version, a.SPDate
                        from EC_SchEngProgressHeader b
                        inner join EC_SchEngProgressPayItem c on(c.EC_SchEngProgressHeaderSeq=b.Seq)
                        inner join EC_SchProgressPayItem a on(a.EC_SchEngProgressPayItemSeq=c.Seq)
                        where b.EngMainSeq=@EngMainSeq
                    ) z                            
                    Order by z.SPDate 
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}