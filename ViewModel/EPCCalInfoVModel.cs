using System;

namespace EQC.ViewModel
{
    public class EPCCalInfoVModel
    {//
        //0:未填寫 1:停工, 2:填寫, 
        public int Mode { get; set; }
        public DateTime ItemDate { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime ModifyTime { get; set; }

        public DateTime MiscConstructionCreateTime {get;set;}
        public DateTime MiscConstructionModifyTime {get;set;}
        
        public bool machineNumFormFilled { get; set; }
        public bool MachineLoading {
            get;set;
        }
        public string DateStr
        {
            get
            {
                return this.ItemDate.ToString("yyyy-MM-dd");
            }
        }
    }
}
