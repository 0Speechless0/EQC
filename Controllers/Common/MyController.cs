﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    public class MyController  :Controller
    {
        JsonSerializerSettings settings;
        IsoDateTimeConverter isoConvert = new IsoDateTimeConverter { 
            DateTimeFormat = "yyyy-MM-dd"
        };

        protected void setAPIReturnSetting(JsonSerializerSettings _settings)
        {
            settings = _settings;
        }
        protected void ResponseJson(object data, string DateTimeFormat = "yyyy-MM-dd")
        {
            List<JsonConverter> converters = new List<JsonConverter>();
            isoConvert.DateTimeFormat = DateTimeFormat;
            converters.Add(isoConvert);
            settings = settings ?? new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            settings.Converters = converters;
            var str = JsonConvert.SerializeObject(data, settings);
            Response.AddHeader("Content-Type", "application/json");
            Response.Write(str);
        }
        protected void DownloadFile(MemoryStream memoryStream, string fileName)
        {
            Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}");
            Response.BinaryWrite(memoryStream.ToArray());
        }

    }
}