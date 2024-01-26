using Newtonsoft.Json;
using System.Collections.Generic;

namespace EQC.ViewModel
{
    public class EngPriceAdjWorkItemGroupVModel
    {//工程物價調整款.WorkItem
        public EngPriceAdjWorkItemGroupVModel(List<EngPriceAdjWorkItemVModel> wItems)
        {
            init(wItems, true, false);
        }
        public EngPriceAdjWorkItemGroupVModel(List<EngPriceAdjWorkItemVModel> wItems, bool expand, bool clearData)
        {
            init(wItems, expand, clearData);
        }
        private void init(List<EngPriceAdjWorkItemVModel> wItems, bool expand, bool clearData) { 
            EngPriceAdjWorkItemVModel temp;
            foreach (EngPriceAdjWorkItemVModel m in wItems)
            {
                if (clearData)
                {
                    m.PriceIndex = 0;
                    m.PriceAdjustment = 0;
                    m.AdjKind = 0;
                }
                if (m.Kind == 101)
                {
                    M03310.Add(m);
                    if (expand)
                    {
                        M03.Add(Clone(201, m));
                        Mxxx.Add(Clone(999, m));
                    }
                }
                else if (m.Kind == 102)
                {
                    M03210.Add(m);
                    if (expand)
                    {
                        M03.Add(Clone(201, m));
                        Mxxx.Add(Clone(999, m));
                    }
                }
                else if (m.Kind == 103)
                {
                    M02742.Add(m);
                    if (expand)
                    {
                        M027_0296.Add(Clone(204, m));
                        Mxxx.Add(Clone(999, m));
                    }
                }
                else if (m.Kind == 201)
                {
                    M03.Add(m);
                    if (expand) Mxxx.Add(Clone(999, m));
                }
                else if (m.Kind == 202)
                {
                    M05.Add(m);
                    if (expand) Mxxx.Add(Clone(999, m));
                }
                else if (m.Kind == 203)
                {
                    M02319.Add(m);
                    if (expand) Mxxx.Add(Clone(999, m));
                }
                else if (m.Kind == 204)
                {
                    M027_0296.Add(m);
                    if (expand) Mxxx.Add(Clone(999, m));
                }
                else if (m.Kind == 999)
                    Mxxx.Add(m);
            }
            //
            M03310.Sort((x, y) => (x.PayItem + x.ItemCode).CompareTo(y.PayItem + y.ItemCode));
            M03210.Sort((x, y) => (x.PayItem + x.ItemCode).CompareTo(y.PayItem + y.ItemCode));
            M02742.Sort((x, y) => (x.PayItem + x.ItemCode).CompareTo(y.PayItem + y.ItemCode));
            M03.Sort((x, y) => (x.PayItem + x.ItemCode).CompareTo(y.PayItem + y.ItemCode));
            M05.Sort((x, y) => (x.PayItem + x.ItemCode).CompareTo(y.PayItem + y.ItemCode));
            M02319.Sort((x, y) => (x.PayItem + x.ItemCode).CompareTo(y.PayItem + y.ItemCode));
            M027_0296.Sort((x, y) => (x.PayItem + x.ItemCode).CompareTo(y.PayItem + y.ItemCode));
            Mxxx.Sort((x, y) => (x.PayItem+x.ItemCode).CompareTo(y.PayItem + y.ItemCode));
        }
        public List<EngPriceAdjWorkItemVModel> M03310 = new List<EngPriceAdjWorkItemVModel>();
        public List<EngPriceAdjWorkItemVModel> M03210 = new List<EngPriceAdjWorkItemVModel>();
        public List<EngPriceAdjWorkItemVModel> M02742 = new List<EngPriceAdjWorkItemVModel>();
        public List<EngPriceAdjWorkItemVModel> M03 = new List<EngPriceAdjWorkItemVModel>();
        public List<EngPriceAdjWorkItemVModel> M05 = new List<EngPriceAdjWorkItemVModel>();
        public List<EngPriceAdjWorkItemVModel> M02319 = new List<EngPriceAdjWorkItemVModel>();
        public List<EngPriceAdjWorkItemVModel> M027_0296 = new List<EngPriceAdjWorkItemVModel>();
        public List<EngPriceAdjWorkItemVModel> Mxxx = new List<EngPriceAdjWorkItemVModel>();

        private EngPriceAdjWorkItemVModel Clone(int kind, EngPriceAdjWorkItemVModel m)
        {
            EngPriceAdjWorkItemVModel nm = JsonConvert.DeserializeObject<EngPriceAdjWorkItemVModel>(JsonConvert.SerializeObject(m));
            nm.Kind = kind;
            return nm;
        }
    }
}
