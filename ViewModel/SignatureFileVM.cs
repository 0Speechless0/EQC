using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    /// <summary> 簽名檔 ViewModel </summary>
    public class SignatureFileVM
    {
        /// <summary> 檔案名稱(顯示) </summary>
        public int UserMainSeq { get; set; }

        /// <summary> 檔案名稱(顯示) </summary>
        public string DisplayFileName { get; set; }

        /// <summary> 檔案名稱(實體) </summary>
        public string FileName { get; set; }

        /// <summary> 檔案路徑(相對路徑) </summary>
        public string FilePath { get; set; }
    }
}