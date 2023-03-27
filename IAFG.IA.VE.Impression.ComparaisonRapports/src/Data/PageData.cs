using System;
using System.Collections.Generic;
using System.Linq;
using Syncfusion.Pdf;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.Data
{
    public class PageData
    {
        public PageData(int index, PdfPageBase documentPage, DocumentData documentData) 
        {
            DocumentPage = documentPage;
            var extractedTexts = documentPage.ExtractText(true)?.SplitAndRemoveEmptyEntries() ?? new List<string>();
            var tags = GetTags(extractedTexts);
            documentData.SetProductTrace(tags);
            documentData.SetTags(tags);
            extractedTexts = CleanupTexts(extractedTexts, documentData.Tags);
            Pagination = ManagePageNumber(extractedTexts);
            Texts = extractedTexts;

            documentPage.ExtractText(out var collection);
            var textLines = new List<TextLine>(collection.TextLine.Select(x => x));
            TextData = MatchTextLines(textLines, extractedTexts)
                .Where(t => !string.IsNullOrWhiteSpace(t.Texte))
                .Select((x, i) => new TextData(i, x.TextLines, x.Texte)).ToList();
            
            Words = TextData.SelectMany(x => x.Words.Select(w => w)).ToList();
            TitrePage = extractedTexts.FirstOrDefault() ?? "Titre inconnue";
            Index = index;
        }

        private static List<string> CleanupTexts(List<string> texts, IReadOnlyCollection<string> tags)
        {
            if (tags == null || !tags.Any())
            {
                return texts;
            }

            return texts.Select(item => 
                    tags.Aggregate(item, (current, tag) => current.Replace(tag, "")))
                .Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        }

        private static List<string> GetTags(ICollection<string> texts)
        {
            var result = new List<string>();
            var found = texts.FirstOrDefault(x => x.StartsWith(DocumentDataExtensions.TRACE_PRODUCT_START));
            if (found == null) return result;
            result.AddRange(found.Split(new[] { ']' }, StringSplitOptions.RemoveEmptyEntries));
            texts.Remove(found);
            return result;
        }

        private static string ManagePageNumber(ICollection<string> texts)
        {
            var pageNumber = texts?.FirstOrDefault(x => x.StartsWith("Page "));
            if (pageNumber == null) return "Pagination inconnue";
            texts.Remove(pageNumber);
            return pageNumber;
        }

        private static string GetText(IReadOnlyCollection<TextWord> collection)
        {
            if (collection == null || !collection.Any()) return string.Empty;
            if (collection.Count == 1) return collection.First().Text;

            var result = new List<string> { collection.First().Text };
            foreach (var item in collection.Skip(1))
            {
                if (result.Last() != item.Text) result.Add(item.Text);
            }

            return string.Join("", result.ToArray());
        }

        private class DataTextLines
        {
            public string Texte { get; set; }
            public List<TextLine> TextLines { get; set; }
        }

        private static IEnumerable<DataTextLines> MatchTextLines(IEnumerable<TextLine> textLines, IEnumerable<string> texts)
        {
            var copyTextLines = new List<TextLine>(textLines);
            var result = new List<DataTextLines>();
            foreach (var text in texts)
            {
                var data = new DataTextLines { Texte = text, TextLines = new List<TextLine>() };
                var matchFound = copyTextLines.FirstOrDefault(x => x.Text == text);
                if (matchFound != null)
                {
                    data.TextLines.Add(matchFound);
                    copyTextLines.Remove(matchFound);
                }
                else
                {
                    TryMatchTextLines(text, data, copyTextLines);
                }

                result.Add(data);
            }

            return result;
        }

        private static void TryMatchTextLines(string text, DataTextLines data, ICollection<TextLine> textLines)
        {
            var matchFound = textLines.FirstOrDefault(x => text.StartsWith(GetText(x.WordCollection)));
            if (matchFound == null) return;
            
            data.TextLines.Add(matchFound);
            textLines.Remove(matchFound);
            if (!textLines.Any()) return;

            var cumulText = GetText(matchFound.WordCollection);
            while (cumulText != text && textLines.Any())
            {
                var textLine = textLines.First();
                var words = GetText(textLine.WordCollection);
                if (text.StartsWith(cumulText + words))
                {
                    cumulText += words;
                    data.TextLines.Add(textLine);
                    textLines.Remove(textLine);
                }
                else
                {
                    break;
                }
            }
        }

        public List<TextData> TextData { get; }
        public List<string> Words { get; }
        public List<string> Texts { get; }
        public string Pagination { get; }
        public string TitrePage { get; }
        public int Index { get; }
        public PageData MatchedPage { get; set; }
        public PdfPageBase DocumentPage { get; }

        public bool IsMatched()
        {
            return MatchedPage != null;
        }

        public bool IsNotMatched()
        {
            return !IsMatched();
        }
    }
}