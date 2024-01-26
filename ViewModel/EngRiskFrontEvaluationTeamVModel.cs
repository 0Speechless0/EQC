using EQC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{
    public class EngRiskFrontEvaluationTeamVModel: EngRiskFrontEvaluationTeamModel
    {
        public string OrganizerUnitName { get; set; }
        public string UnitName { get; set; }
        public string PrincipalName { get; set; }
    }
}