using EQC.Models;
using EQC.Services;

namespace EQC.ViewModel
{
    public class CarbonEmissionPayItemVModel : CarbonEmissionPayItemModel
    {//碳排量計算
        public string UnitFmt
        {
            get
            {
                string text = Unit;
                switch (RStatusCode)
                {
                    case CarbonEmissionPayItemService._C10NotMatchUnit:
                        text = string.Format("<font color=\"red\">{0}</font>", Unit);// 單位代碼錯誤";
                        break;
                }
                return text;
            }
        }
        public string RStatusCodeStr {
            get
            {
                string text = "";
                switch (RStatusCode)
                {
                    case CarbonEmissionPayItemService._NotLongEnoughEdit:
                    case CarbonEmissionPayItemService._NotLongEnough: text = "<font color=\"red\">不足10碼</font>";
                        break;
                    case CarbonEmissionPayItemService._FullMatch:
                    case CarbonEmissionPayItemService._Match:
                    case CarbonEmissionPayItemService._NonTypeMatch:
                    case CarbonEmissionPayItemService._FullMatchEdit:
                    case CarbonEmissionPayItemService._MatchEdit:
                    case CarbonEmissionPayItemService._NonTypeMatchEdit:
                        return "匹配";
                        break;

                    case CarbonEmissionPayItemService._MatchC10_0Edit:
                    case CarbonEmissionPayItemService._MatchC10_0:
                        return "匹配(末碼0)";
                        break;

                    case CarbonEmissionPayItemService._C10NotMatchUnitEdit:
                    case CarbonEmissionPayItemService._C10NotMatchUnit:
                        text = "<font color=\"red\">單位代碼錯誤</font>";// 第10碼不同";
                        break;

                    case CarbonEmissionPayItemService._C10NotMatch:
                    case CarbonEmissionPayItemService._C10NotMatchReason:
                        text = "<font color=\"red\">查無單位係數</font>";// 第10碼不同";
                        break;

                    case CarbonEmissionPayItemService._NotMatch:
                    case CarbonEmissionPayItemService._NotMatchReason: 
                        text = "<font color=\"red\">查無編碼</font>";
                        break;
                    //case CarbonEmissionPayItemService._C10NotMatchReason:
                    //case CarbonEmissionPayItemService._NotMatchReason: text = " ";break;

                }
                if(text != "" && !string.IsNullOrEmpty(RefItemCode))
                {
                    text += "<br>" + RefItemCode;
                }
                return text.Trim();
            }
        }
    }
}
