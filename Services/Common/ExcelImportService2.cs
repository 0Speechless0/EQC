

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
using System.Text;
using DbContext = EQC.EDMXModel.DbContext;
namespace EQC.Services.Common
{
    public  class ExcelImportService<T> : BaseService, IExcelImportService<T>

        where T : class
    {

        public ExcelImportService(string[,] _excelHeaderMap = null, Func<T, string, bool> _keyWordCompare = null)
        {
            excelHeaderMap = _excelHeaderMap;
            keyWordCompare = _keyWordCompare;
        }
        public string[,] excelHeaderMap { get; set; }
        public Func<T, string , bool> keyWordCompare { get; set; }

        public List<T> GetList(Func<EQC_NEW_Entities, List<T>> getFromEntity  ,int page, int per_page, string keyWord = null)
        {

            using (var context = new EQC_NEW_Entities(false) )
            {
                return getFromEntity.Invoke(context)
                    .Where(r => keyWord == null || keyWordCompare.Invoke(r, keyWord))
                    .ToList()
                    .getPagination(page, per_page);
            }
        }
        public int GetListCount(Func<EQC_NEW_Entities, List<T>> getFromEntity, string keyWord)
        {
            using (var context = new EQC_NEW_Entities(false))
            {
                return getFromEntity.Invoke(context)
                    .Where(r => keyWord == null || keyWordCompare.Invoke(r, keyWord))
                    .Count();
            }
        }


        public void update(T m, Func<EQC_NEW_Entities, T> func ) 
        {
            using(var context = new EQC_NEW_Entities() )
            {
                context.Entry(func.Invoke(context)).CurrentValues.SetValues(m);
                context.SaveChanges();
            }
        }
        public void add(T m)
        {
            using (var context = new EQC_NEW_Entities())
            {
                context.Entry(m).State = EntityState.Added;
                context.SaveChanges();
            }
        }
        public void delete(Func<EQC_NEW_Entities, T> func) 
        {
            using (var context = new EQC_NEW_Entities())
            {
                context.Entry(func.Invoke(context))
                    .State = EntityState.Deleted;
                context.SaveChanges();

            }
        }

        public void importPreAction(ExcelProcesser processer)
        {
            processer.declareCol("ListDate", new int[] { 1, 0 }, new string[] { "列表日期： ", "年", "月", "日" });
        }
        public void importExcel(ExcelProcesser processer, string tableName, string tablePrimary)
        {
            string sql = "";
            processer.createDbColumnMapToHeader(excelHeaderMap);
            int currentRow = processer.getStartRow();
            int lastRow = processer.getEndRow();
            

            while (currentRow < lastRow)
            {

                if (!processer.checkRowVaild(currentRow))
                {
                    currentRow++;
                    continue;
                }
                sql = processer.getInsertSql(tableName, currentRow);
                sql = processer.addDeclareColToInsertSql(sql);
                string updateSql = processer.getUpdateSql(tableName
                    , tablePrimary, currentRow);
                updateSql = processer.addDeclareColToUpdateSql(updateSql);


                try
                {
                    int update = db.ExecuteNonQuery(db.GetCommand(updateSql));
                    if (update == 0)
                    {
                        SqlCommand cmd = db.GetCommand(sql);
                        var a = db.ExecuteNonQuery(cmd);
                    }
                }
                catch(Exception e)
                {

                }

                currentRow++;
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