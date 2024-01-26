using EQC.Models;
using System.Collections.Generic;

namespace EQC.ViewModel
{
    public class PriceIndexListVModel : PriceIndexKindModel
    {//物價指數
        public List<PriceIndexItemModel> items = new List<PriceIndexItemModel>();
    }
}
