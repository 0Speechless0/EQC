using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading;
using System.Web;

namespace EQC.EDMXModel
{
    public partial class EQC_NEW_Entities : DbContext
    {
        public EQC_NEW_Entities(bool lazyLoading) : base("name=EQC_NEW_Entities")
        {
            this.Configuration.LazyLoadingEnabled = lazyLoading;
        }


       
    }
}