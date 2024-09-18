

using EQC.Common;
using EQC.EDMXModel;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using DbContext = EQC.EDMXModel.DbContext;

namespace EQC.Services
{
    public  class ExcelImportService : BaseService
    {

        public ExcelImportService(string[,]  _excelHeaderMap = null)
        {
            excelHeaderMap = _excelHeaderMap;
        }
        public string[,] excelHeaderMap { get; set; }
        public List<T> GetList<T>(Func<EQC_NEW_Entities, List<T>> getFromEntity  ,int page, int per_page, Func<T, bool> keyWordcomapre )
        {

            using(var context = new EQC_NEW_Entities() )
            {
                return getFromEntity.Invoke(context)
                    .Where(r => keyWordcomapre.Invoke(r))
                    .ToList()
                    .getPagination(page, per_page);
            }
        }
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

        public List<object> getExcelImportFields()
        {
            int rowNum = excelHeaderMap.GetLength(0);
            List<object> list = new List<object>();
            for (int i = 0; i < rowNum; i++)
            {
                list.Add(new { fieldName = excelHeaderMap[i, 0], fieldShowName = excelHeaderMap[i, 1] });
            }

            return list;
        }
    }
}