﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace EQC.ViewModel
{
    public class ECEngPriceAdjWorkItemGroupVModel
    {//工程變更-工程物價調整款.WorkItem
        public ECEngPriceAdjWorkItemGroupVModel(List<ECEngPriceAdjWorkItemVModel> wItems)
        {
            init(wItems, true, false);
        }
        public ECEngPriceAdjWorkItemGroupVModel(List<ECEngPriceAdjWorkItemVModel> wItems, bool expand, bool clearData)
        {
            init(wItems, expand, clearData);
        }
        private void init(List<ECEngPriceAdjWorkItemVModel> wItems, bool expand, bool clearData) {
            ECEngPriceAdjWorkItemVModel temp;
            foreach (ECEngPriceAdjWorkItemVModel m in wItems)
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
        public List<ECEngPriceAdjWorkItemVModel> M03310 = new List<ECEngPriceAdjWorkItemVModel>();
        public List<ECEngPriceAdjWorkItemVModel> M03210 = new List<ECEngPriceAdjWorkItemVModel>();
        public List<ECEngPriceAdjWorkItemVModel> M02742 = new List<ECEngPriceAdjWorkItemVModel>();
        public List<ECEngPriceAdjWorkItemVModel> M03 = new List<ECEngPriceAdjWorkItemVModel>();
        public List<ECEngPriceAdjWorkItemVModel> M05 = new List<ECEngPriceAdjWorkItemVModel>();
        public List<ECEngPriceAdjWorkItemVModel> M02319 = new List<ECEngPriceAdjWorkItemVModel>();
        public List<ECEngPriceAdjWorkItemVModel> M027_0296 = new List<ECEngPriceAdjWorkItemVModel>();
        public List<ECEngPriceAdjWorkItemVModel> Mxxx = new List<ECEngPriceAdjWorkItemVModel>();

        private ECEngPriceAdjWorkItemVModel Clone(int kind, ECEngPriceAdjWorkItemVModel m)
        {
            ECEngPriceAdjWorkItemVModel nm = JsonConvert.DeserializeObject<ECEngPriceAdjWorkItemVModel>(JsonConvert.SerializeObject(m));
            nm.Kind = kind;
            return nm;
        }
    }
}