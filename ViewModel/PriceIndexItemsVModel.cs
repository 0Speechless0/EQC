
using EQC.Models;
using System;

namespace EQC.ViewModel
{
    public class PriceIndexItemsVModel : PriceIndexItemModel
    {//物價指數
        public string PIDateStr
        {
            get
            {
                return String.Format("{0}/{1}", PIDate.Year - 1911, PIDate.Month);
            }
        }
        
    }
}
