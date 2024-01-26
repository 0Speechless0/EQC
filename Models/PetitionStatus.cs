using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{
    public class PetitionStatus
    {
        public int StatusCode { get; set; } //(int, not null)
        public string StatusName { get; set; } //(nvarchar(50), null)
    }
}