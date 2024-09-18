using EQC.EDMXModel;
using EQC.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EQC.Controllers.InterFaceForFrontEnd
{
    interface IExcelImport<T> where T : class
    {
        ExcelImportService<T> iService { get; set; }
        Func<EQC_NEW_Entities, List<T>> ListGetter { get; set; }

        string tableName { get; set; }
        JsonResult GetList(int page, int per_page, string keyWord = null);
        JsonResult excelUpload();

        JsonResult getDemandFields();
 

        JsonResult getLastUpdateTime();

        JsonResult Add(T m);
        JsonResult Update(T m);

        JsonResult Delete(object id);

    }
}
