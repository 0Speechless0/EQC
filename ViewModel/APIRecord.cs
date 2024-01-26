using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.EDMXModel
{
    public partial class APIRecord
    {
        public string MenuName { get; set; }

        public string UserName { 
            
            
            get {
                return
                    UserMain  != null ?
                    UserMain.DisplayName + ":" + UserMain.UserNo : "端點使用者";
                    
            } 
        }

    }
}