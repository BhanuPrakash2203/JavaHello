using System;
using System.Collections.Generic;
using System.Linq;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.Data
{
    public static class DocumentDataExtensions
    {
        public const string TRACE_PRODUCT_START = "[p::";
        public const string TRACE_DATE_IMPRIMEE_START = "[di::";
        public const string TRACE_DATE_PREPAREE_START = "[dp::";

        public static List<string> SplitAndRemoveEmptyEntries(this string text)
        {
            return text?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
        }

        public static void SetProductTrace(this DocumentData documentData, List<string> tags)
        {
            var tag = tags.FirstOrDefault(x => x.StartsWith(DocumentDataExtensions.TRACE_PRODUCT_START));
            if (tag == null) return;
            documentData.ProductTrace = tag.Replace(DocumentDataExtensions.TRACE_PRODUCT_START, "");
            tags.Remove(tag);
        }

        public static void SetTags(this DocumentData documentData, List<string> tags)
        {
            documentData.Tags.AddRange(
                tags.Select(x => x
                    .Replace(DocumentDataExtensions.TRACE_DATE_PREPAREE_START, "")
                    .Replace(DocumentDataExtensions.TRACE_DATE_IMPRIMEE_START, "")));
        }
    }
}