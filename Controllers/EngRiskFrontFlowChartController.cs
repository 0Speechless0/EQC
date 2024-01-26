using EQC.Common;
using EQC.Models;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EngRiskFrontFlowChartController : Controller
    {
        EngRiskFrontFlowChartService service;

        public EngRiskFrontFlowChartController()
        {
            service = new EngRiskFrontFlowChartService();
        }

        private string GetFlowChartPath()
        {
            return Utils.GetTemplateFolder();
        }
    }
}