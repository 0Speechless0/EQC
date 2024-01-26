using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel.Common
{
    /// <summary> 下拉選單 </summary>
    public class SelectVM
    {
        /// <summary> 項目文字 </summary>
        public string Text { get; set; }

        /// <summary> 項目值 </summary>
        public string Value { get; set; }

        /// <summary> 是否選擇 </summary>
        public bool IsSelected { get; set; }
    }
}