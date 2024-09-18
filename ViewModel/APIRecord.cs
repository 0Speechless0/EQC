using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.EDMXModel
{
    public partial class APIRecord
    {
        public string MenuName { get; set; }


        public Menu Menu {get;set;}
        public int ExecSec { 
            get { 
                if(EndingTime != null && CreateTime != null)
                {
                    return (EndingTime.Value - CreateTime.Value).Milliseconds;
                }
                return 0;
            } 
        } 
        public string UserName { 
            
            
            get {
                return
                    UserMain != null ?
                    UserMain.DisplayName + ":" + UserMain.UserNo : "" ;
                    
            } 
        }

    }
}