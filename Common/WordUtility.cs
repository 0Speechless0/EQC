using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using EQC.ProposalV2;
namespace EQC.Common
{
    public static class WordUtility
    {
        public static bool CombineDocx(string[] filesToMerge, string destFilepath)
        {
            bool result = false;
            int i = 0;
            foreach (string file in filesToMerge)
            {
                using (WordprocessingDocument wpd = WordprocessingDocument.Open(destFilepath, true))
                {
                    MainDocumentPart mdp = wpd.MainDocumentPart;
                    string altChunkId = "AltChunkId" + i;
                    AlternativeFormatImportPart chunk = mdp.AddAlternativeFormatImportPart(
                        AlternativeFormatImportPartType.WordprocessingML, altChunkId);
                    using (FileStream fileStream = System.IO.File.Open(file, FileMode.Open))
                    {
                        chunk.FeedData(fileStream);
                    }
                    AltChunk altChunk = new AltChunk();
                    altChunk.Id = altChunkId;
                    mdp.Document.Body.Append(altChunk);
                    //mdp.Document.Body.InsertAfter(altChunk, mdp.Document.Body.Elements<Paragraph>().Last());
                    if (i < (filesToMerge.Length - 1))
                        mdp.Document.Body.AppendChild(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
                    var lastP = mdp.Document.Body.Elements<Paragraph>().Last();
                    mdp.Document.Save();
                    wpd.Close();
                }
                i++;
            }
            result = true;
            return result;
        }
        public static void ReplaceParserTag(this OpenXmlElement elem, Dictionary<string, string> data, WordprocessingDocument docx)
        {
            var pool = new List<Run>();
            var matchText = string.Empty;
            var hiliteRuns = elem.Descendants<Run>() //找出鮮明提示
                .Where(o => o.RunProperties?.Elements<Highlight>().Any() ?? false).ToList();

            foreach (var run in hiliteRuns)
            {
                var t = run.InnerText;
                if (t.StartsWith("["))
                {
                    pool = new List<Run> { run };
                    matchText = t;
                }
                else
                {
                    matchText += t;
                    pool.Add(run);
                }
                if (t.EndsWith("]"))
                {
                    var m = Regex.Match(matchText, @"\[\$(?<n>\w+)\$\]");
                    if (m.Groups["n"].Value == "LocationMap" || m.Groups["n"].Value == "AerialPhotography"
                        || m.Groups["n"].Value == "ScenePhoto" || m.Groups["n"].Value == "BaseMap"
                        || m.Groups["n"].Value == "EngPlaneLayout" || m.Groups["n"].Value == "LongitudinalSection"
                        || m.Groups["n"].Value == "StandardSection"
                        )
                    {
                        if (m.Success && data.ContainsKey(m.Groups["n"].Value))
                        {
                            var firstRun = pool.First();
                            firstRun.RemoveAllChildren<Text>();
                            firstRun.RunProperties.RemoveAllChildren<Highlight>();
                            var newText = data[m.Groups["n"].Value];
                            var firstLine = true;
                            foreach (var line in Regex.Split(newText, @"\n"))
                            {
                                if (firstLine) firstLine = false;
                                else firstRun.Append(new Break());

                                if (!string.IsNullOrEmpty(line)) 
                                {
                                    firstRun.Append(EQC.Common.WordImg.GetPicInBody(docx, line));
                                }
                            }
                            pool.Skip(1).ToList().ForEach(o => o.Remove());
                        }
                    }
                    else if (m.Success && data.ContainsKey(m.Groups["n"].Value))
                    {
                        var firstRun = pool.First();
                        firstRun.RemoveAllChildren<Text>();
                        firstRun.RunProperties.RemoveAllChildren<Highlight>();
                        var newText = data[m.Groups["n"].Value];
                        var firstLine = true;
                        foreach (var line in Regex.Split(newText, @"\n"))
                        {
                            if (firstLine) firstLine = false;
                            else firstRun.Append(new Break());
                            firstRun.Append(new Text(line));
                        }
                        pool.Skip(1).ToList().ForEach(o => o.Remove());
                    }
                }

            }
        }


        public static void InsertImageToDocx(string templateFileName, string imageDir, Dictionary<string, string> fileNamesMap)
        {
            using(var doc = new OfficeDocumentV2(templateFileName, imageDir))
            {
                foreach(var pair in fileNamesMap)
                {
                    doc.InsertCharts(pair.Key, new List<Attachment> { new Attachment { Name = pair.Value, Description = "" } }, false, "[${0}$]");
               
                }

                doc.SaveAs(templateFileName);
            }
        }
        public static byte[] GenerateDocx(byte[] template, Dictionary<string, string> data)
        {
       
            using (var ms = new MemoryStream())
            {
                ms.Write(template, 0, template.Length);
                using (var docx = WordprocessingDocument.Open(ms, true))
                {
                    docx.MainDocumentPart.HeaderParts.ToList().ForEach(hdr =>
                    {
                        hdr.Header.ReplaceParserTag(data, docx);
                    });
                    docx.MainDocumentPart.FooterParts.ToList().ForEach(ftr =>
                    {
                        ftr.Footer.ReplaceParserTag(data, docx);
                    });
                    docx.MainDocumentPart.Document.Body.ReplaceParserTag(data, docx);
                    docx.Save();
                }
                return ms.ToArray();
            }
        }
    }
}