using System;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class PCCESSMainService : BaseService
    {
        //是否已有 工程號
        public int GetCountByContractNo(string contractNo)
        {
            string sql = @"SELECT Seq FROM PCCESSMain where contractNo=@contractNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@contractNo", contractNo);
            DataTable dt = db.GetDataTable(cmd);
            int result = dt.Rows.Count;
            if(result == 0)
            {
                sql = @"SELECT Seq FROM EngMain where EngNo=@contractNo";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@contractNo", contractNo);
                dt = db.GetDataTable(cmd);
                result = dt.Rows.Count;
            }
            return result;
        }

        public int GetPCCESSMainSeqByContractNo(string contractNo, ref int engMainSeq)
        {
            string sql = @"
                SELECT a.Seq, b.Seq as engMainSeq FROM PCCESSMain a
                inner join EngMain b on(b.EngNo=a.contractNo)
                where a.contractNo=@contractNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@contractNo", contractNo);
            DataTable dt = db.GetDataTable(cmd);
            if (dt.Rows.Count == 1)
            {
                engMainSeq = Convert.ToInt32(dt.Rows[0]["engMainSeq"].ToString());
                return Convert.ToInt32(dt.Rows[0]["Seq"].ToString());
            } else
            {
                return -1;
            }
        }
    }
}