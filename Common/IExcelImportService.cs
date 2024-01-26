using EQC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQC.Common
{
    public interface IExcelImportService<T>
    {
        string[,] excelHeaderMap { get; set; }
        DateTime getLastUpdateTime(string tableName);
        List<T> GetList(int page, int per_page, string keyWord);
        Object GetCount();

        void importExcel(ExcelProcesser processer);
        List<object> getExcelImportFields();

    }
}
