using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EQC.Models;
namespace EQC.Services
{
    public class CarbonFactorCompareService : BaseService
    {
        public const int _None = 300;               //300:無須匹配
        public const int _NotLongEnough = 200;      //200:不足10碼

        public const int _FullMatch = 101;          //101:全匹配
        public const int _Match = 100;              //100:匹配
        public const int _NonTypeMatch = 99;        //99:不分類匹配
        public const int _MatchC10_0 = 98;         //98:匹配 資料庫第10碼是0

        public const int _C10NotMatchReason = 55;   //52:第10碼不同(理由)
        public const int _NotMatchReason = 51;      //51:無匹配(理由)

        public const int _C10NotMatchUnit = 6;      //6:第10碼不同,(單位代碼1:M、2:M2、3:M3、4:式、5:T、6:只、7:個、8:組、9:KG)
        public const int _C10NotMatch = 5;          //5:第10碼不同
        public const int _NotMatch = 1;             //1:無匹配
        public const int _Init = 0;                 //0:未比對

        public CarbonFactorCompareService()
        {
          
        }
    }
}