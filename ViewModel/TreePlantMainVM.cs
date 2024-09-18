using EQC.Common;
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

        public TreePlantMain _row = new TreePlantMain();

        public DateTime? ContractDateResult
        {
            get
            {
                if(_row.EngCreatType == 1)
                {
                    return Utils.StringChs2DateToDateTime(
                        context.EngMain.Find(_row.EngSeq)?.PrjXML.ActualBidAwardDate) ;
                }
                if(_row.EngCreatType == 2)
                {
                    return Utils.StringChs2DateToDateTime(
                        context.PrjXML.Find(_row.EngSeq)?.ActualBidAwardDate);
                }
                return _row.ContractDate;
                
            }
        }
        public string ContractorNameResult {
            get
            {
                if(_row.EngCreatType == 1)
                {
                    return context.EngMain.Find(_row.EngSeq)?.BuildContractorName ?? "";
                }
                if(_row.EngCreatType == 2)
                {
                    return context.PrjXML.Find(_row.EngSeq)?.ContractorName1 ?? "";
                }

                return _row.Contractor;
            }
            set
            {
                _row.Contractor = value;
            }
        }

        public string ContractorUniformNumberResult
        {
            get
            {
                if (_row.EngCreatType == 1)
                {
                    return context.EngMain.Find(_row.EngSeq)?.BuildContractorTaxId ?? "";
                }

                return _row.ContractorUniformNumber;
            }
            set
            {
                _row.ContractorUniformNumber = value;
            }
        }
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
        public TreePlantMainVM SetTreeMainSourceEntity(TreePlantMain row,EQC_NEW_Entities _context)
        {
            _row = row;
             context = _context;
            return this;
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
                    (context.Town.Find(_row.EngTownSeq)?.TownName ?? "" )
                    + ( context.City.Find(_row.CitySeq)?.CityName ?? "" )
                    ;
            }
        }

        public string BidAwardDateStr
        {
            get
            {
                return $"{ContractDateResult?.Year - 1911}年{ContractDateResult?.Month}月";
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
        public string ActualCompletionDateStr
        {
            get
            {
                return $"{_row.ActualCompletionPlantDate?.Year - 1911}年{_row.ActualCompletionPlantDate?.Month}月";
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
