using EQC.EDMXModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Services
{
    public class RecordService
    {
        Dictionary<short, Unit> unitDic;
        EQC_NEW_Entities dbContext;
        public RecordService(EQC_NEW_Entities _dbContext = null)
        {
            if (_dbContext == null)
                dbContext = new EQC_NEW_Entities();
            else dbContext = _dbContext;

            unitDic = dbContext.Unit.ToDictionary(r => r.Seq, r => r);
        }
        ~RecordService()
        {
            dbContext.Dispose();
        }

        public string GetLoginRecordUnitStr(Unit unit)
        {
            Stack<string> unitStack = new Stack<string>();
            string unitStr = "";
            do
            {
                unitStack.Push(unit.Name);
                unitDic.TryGetValue(unit.ParentSeq ?? 0, out Unit parentUnit);
                unit = parentUnit;
            } while (unit != null);

            string unitName = null;
            do
            {
                unitName = unitStack.Pop();
                unitStr += unitName + " ";


            } while (unitStack.Count() > 0);

            return unitStr; 
        }
    }
}