using Microsoft.Office.Interop.Word;
using System.IO;
using Microsoft.Office.Core;

namespace EQC.Common
{
    public static class OfficeExtension
    {

        /// <summary>
        ///  從擴充的word來源路徑轉換為pdf，並回傳該pdf之路徑
        /// </summary>
        /// <param name="path"> word 來源路徑</param>
        /// <param name="exportDir"> 轉換後pdf存放資料夾路徑，此資料夾與原本word同層</param>
        /// <returns></returns>
        ///
        static Application _app;

        private static Document initDocument(string path)
        {
            _app = new Application();
            _app.DisplayAlerts = WdAlertLevel.wdAlertsNone;
            _app.Visible = false;

            return _app.Documents.Open(path, MsoTriState.msoFalse, MsoTriState.msoTrue, MsoTriState.msoFalse);
        }
        public static string CreateODT(this string path, string exportDir = "")
        {
            Document doc = initDocument(path);
            var odtFileName = Path.ChangeExtension(path, ".odt");
            var odtPath = Path.Combine(exportDir ?? "", odtFileName);

            try
            {

                doc.SaveAs2(odtPath, WdSaveFormat.wdFormatOpenDocumentText);
            }
            catch
            {
                odtPath = null;
            }
            finally
            {
                doc.Saved = true;
                doc.Close();
                _app.Quit();
            }
            return odtPath;
        }
         public static string CreatePDF(this string path, string exportDir ="")
        {
            Document doc = initDocument(path); 


            var pdfFileName = Path.ChangeExtension(path, ".pdf");
            var pdfPath = Path.Combine(exportDir ?? "", pdfFileName);
            try
            {
                doc.SaveAs2(pdfPath, WdSaveFormat.wdFormatPDF);
            }
            catch
            {
                pdfPath = null;
            }
            finally
            {
                doc.Saved = true;
                doc.Close();
                _app.Quit();
            }
            return pdfPath;
        }
    }
}