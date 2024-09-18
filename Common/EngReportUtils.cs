using EQC.EDMXModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Common
{
    public static class EngReportUtils
    {
        public static string GetProposalReviwCopyId(int seq)
        {
            using (var context = new EQC_NEW_Entities())
            {

                var resultList = new List<EngReportList>();
                var currentSeqs = new List<int>() { seq };
                do
                {
                    var currentList = new List<EngReportList>();
                    currentSeqs.ForEach(s =>
                    {
                        currentList.AddRange(
                            context.EngReportList.Where(r => r.ProposalReviewEngReportSeq == s)
                            .ToList()
                        );
                    });
                    currentSeqs = currentList.Select(r => r.Seq).ToList();
                    resultList.AddRange(currentList);
                } while (currentSeqs.Count > 0);



                var str = resultList.Select(r => r.Seq).Aggregate("", (a, c) => a + "," + c);
                return str.Length > 0 ? str.Substring(1) : "";
            }
        }
    }
}