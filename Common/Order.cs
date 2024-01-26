using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Common
{

    class TrainComparer<T> : IComparer<T>
    {

        Dictionary<string, int> _orderMap;
        public  TrainComparer(Dictionary<string, int> orderMap )
        {
            _orderMap = orderMap;
        }

        public int Compare(T y, T x)
        {
            if (!_orderMap.ContainsKey(x.ToString())) return 1;
            if (!_orderMap.ContainsKey(y.ToString())) return -1;
            return _orderMap[y.ToString()] - _orderMap[x.ToString()];
        }


    }
    public static class Order
    {
        // Call CaseInsensitiveComparer.Compare with the parameters reversed.
        public static Dictionary<string, int> TrainOrderMap = new Dictionary<string, int>()
        {
            {"南港", 1},
            {"台北", 2},
            {"板橋", 3},
            {"桃園", 4},
            {"新竹", 5},
            {"苗栗", 6},
            {"台中", 7},
            {"彰化", 8},
            {"雲林", 9},
            {"嘉義", 10},
            {"台南", 11},
            {"左營", 12}
        };
        public static Dictionary<string, int> PositionOrderMap = new Dictionary<string, int>()
        {
            {"副總工程司", 1},
            {"組長", 2},
            {"副組長", 3},
            {"科長", 4},
        };
        public static Dictionary<string, int> CommitteeScoredOrder = new Dictionary<string, int>()
        {
            // 0 內聘委員  1 外聘委員  2 領隊
            {"0", 2},
            {"1", 2},
            {"2", 1},
        };
        public static List<T> SortListByMap<T>(this List<T> list, Func<T, string> keySelector, Dictionary<string, int> orderMap, bool reverse = false)
        {
            if (reverse)
            {
                return list.OrderByDescending(keySelector, new TrainComparer<string>(orderMap)).ToList();

            }
            return list.OrderBy(keySelector, new TrainComparer<string>(orderMap)).ToList();
        }

    }
}