using EQC.Common;
using EQC.EDMXModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQC.Services.Common
{
    public interface IExcelImportService<T> where T : class
    {
        string[,] excelHeaderMap { get; set; }
        DateTime getLastUpdateTime(string tableName);


        Func<T, string, bool> keyWordCompare { get; set; }
        List<object> getExcelImportFields();

        List<T> GetList(Func<EQC_NEW_Entities, List<T>> getFromEntity, int page, int per_page, string keyWord = null);
        void update(T m, Func<EQC_NEW_Entities, T> func);
        void add(T m);
        void importPreAction(ExcelProcesser processer);
        void importExcel(ExcelProcesser excelProcesser, string tableName, string tablePrimary);
        void delete(Func<EQC_NEW_Entities, T> func) ;
        int GetListCount(Func<EQC_NEW_Entities, List<T>> getFromEntity, string keyWord);
        void updateOrCreateFromExcel(ExcelProcesser excelProcesser, string tableName, string primaryCol);
    }
}
