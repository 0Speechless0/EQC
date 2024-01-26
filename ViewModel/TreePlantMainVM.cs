using EQC.EDMXModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    public class TreePlantMainVM 
    {
        EQC_NEW_Entities context;

        public TreePlantMain _row;

        public short? OrderNo { 
            get
            {
                var orderNo = context.Unit.Find(_row.ExecUnitSeq)?.OrderNo;
                if (orderNo != null)
                {
                    return orderNo;
                }
                return short.MaxValue;
            }
        
        }
        public TreePlantMainVM(TreePlantMain row,EQC_NEW_Entities _context)
        {
            _row = row;
             context = _context;
        }
        public string execUnitName
        {
            get
            {

                    return context.Unit.Find(_row.ExecUnitSeq)
                             ?.Name ?? "";

            }
        }

        public string EngTypeName
        {
            get
            {

                    return 
                        context.TreePlantEngType.Find(_row.TPEngTypeSeq)?.Name ?? "";
            }
        }

        public string TownCity
        {
            get
            {
                
                return
                    context.Town.Find(_row.EngTownSeq)?.TownName ?? ""
                    + context.City.Find(_row.CitySeq)?.CityName ?? ""
                    ;
            }
        }

        public string BidAwardDateStr
        {
            get
            {
                return $"{_row.ContractDate?.Year - 1911}年{_row.ContractDate?.Month}月";
            }
        }

        public string ScheduledPlantDateStr
        {
            get
            {
                return $"{_row.ScheduledPlantDate?.Year - 1911}年{_row.ScheduledPlantDate?.Month}月";
            }
        }
        public string ScheduledCompletionDateStr
        {
            get
            {
                return $"{_row.ScheduledCompletionDate?.Year - 1911}年{_row.ScheduledCompletionDate?.Month}月";
            }
        }
        public string TreePlantTypeName
        {
            get
            {
                return context.TreePlantType.Find(_row.TreePlantTypeSeq)?.Name ?? "";
            }
        }
        public string TPEngTypeName
        {
            get
            {
                return context.TreePlantEngType.Find(_row.TPEngTypeSeq)?.Name ?? "";
            }
        }
    }
}
