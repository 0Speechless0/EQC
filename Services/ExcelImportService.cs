

using EQC.Common;
using EQC.EDMXModel;
using NPOI.Util;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace EQC.Services
{
    public  class ExcelImportService : BaseService
    {

        public int GetListCount(string tableName, string keyWord, string[] cols)
        {
            string sql = @"SELECT
                    count(*) total
                FROM " + tableName +" where ";
            foreach(string col in cols)
            {
                sql += col + " like @keyWord or ";
            }
            sql = sql.Remove(sql.Length - 3, 3);
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Add("@keyWord", "%" + keyWord + "%");

            return (int)db.ExecuteScalar(cmd);
        }


        public void update<T>(T m, Func<EQC_NEW_Entities, T> func ) where T : class
        {
            using(var context = new EQC_NEW_Entities() )
            {
                context.Entry(func.Invoke(context)).CurrentValues.SetValues(m);
                context.SaveChanges();
            }
        }

        public void delete<T>(Func<EQC_NEW_Entities, T> func) where T : class
        {
            using (var context = new EQC_NEW_Entities())
            {
                context.Entry(func.Invoke(context))
                    .State = EntityState.Deleted;
                context.SaveChanges();

            }
        }

        public void updateOrCreateFromExcel(ExcelProcesser excelProcesser, string tableName, string primaryCol)
        {
            int currentRow; int endRow;
            currentRow = excelProcesser.getStartRow();
            endRow = excelProcesser.getEndRowBySeq();
            while (currentRow < endRow)
            {
                string insertSql = excelProcesser.getInsertSql(tableName, currentRow);
                string updateSql = excelProcesser.getUpdateSql(tableName
                    , primaryCol, currentRow);
                int update = db.ExecuteNonQuery(db.GetCommand(updateSql));
                if (update == 0)
                {
                    SqlCommand cmd = db.GetCommand(insertSql);
                    db.ExecuteNonQuery(cmd);
                }
                currentRow++;
            }
        }
    }
}