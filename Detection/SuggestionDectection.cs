using EQC.EDMXModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace EQC.Detection
{
    public static class SuggestionDectection
    {


        public static bool InsertAfterDetectSuggestion(int func, string keyWord, string name, int key)
        {
            if (name.Contains(keyWord))
            {

                using (var context = new EQC_NEW_Entities())
                {

                    try
                    {
                        var suggestionSeqs = context.Suggestion.Join(context.SuggestionClass.Where(r => r.ParentFunction == func),
                        r1 => r1.ClassSeq,
                        r2 => r2.Seq, (r1, r2) => r1.Seq);
                        foreach (var suggestionSeq in suggestionSeqs)
                        {
                            context.SuggestionHead.Add(new SuggestionHead
                            {
                                SuggestionSeq = suggestionSeq,
                                ParentClass = func,
                                ParentSeq = key,

                            });
                        }
                        context.SaveChanges();
                    }
                    catch(DbUpdateException e)
                    {
                       
                    }

                    

                }
                return true;
            }
            return false;
        }
        public static bool InsertAfterDetectSuggestion<T>(this T m, int func, string keyWord, Func<T, string> nameGetter, Func<T, int> keyGetter)
        {
            return InsertAfterDetectSuggestion(func, keyWord, nameGetter.Invoke(m), keyGetter.Invoke(m));
        }

        public static string InsertAfterDectectSuggestionFromTable<T>(this T m, Func<T, string> nameGatter)
        {
            return "";
        }
    }
}