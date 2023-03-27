using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IAFG.IA.VE.Impression.ComparaisonRapports.Data;
using IAFG.IA.VE.Impression.ComparaisonRapports.Traces;
using Newtonsoft.Json;
using Syncfusion.Drawing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;

namespace IAFG.IA.VE.Impression.ComparaisonRapports
{
    public static class SynchfusionComparator
    {
        private const string TRACE_MARK = "[T]";

        public static bool CompareFiles(string f1, string f2, string outputPath)
        {
            var fileStream1 = new FileStream(f1, FileMode.Open);
            using (var loadedDocument1 = new PdfLoadedDocument(fileStream1))
            {
                try
                {
                    var firstDocument = new DocumentData(f1, loadedDocument1);
                    var fileStream2 = new FileStream(f2, FileMode.Open);
                    using (var loadedDocument2 = new PdfLoadedDocument(fileStream2))
                    {
                        try
                        {
                            var secondDocument = new DocumentData(f2, loadedDocument2, firstDocument);
                            return CompareFiles(firstDocument, secondDocument, outputPath);
                        }
                        finally
                        {
                            loadedDocument2.Close(true);
                            fileStream2.Close();
                        }
                    }
                }
                finally
                {
                    loadedDocument1.Close(true);
                    fileStream1.Close();
                }
            }
        }

        private static bool CompareFiles(DocumentData firstDocument, DocumentData secondDocument, string outputPath)
        {
            TraiterPagesIdentiques(firstDocument, secondDocument);

            var traces = CreateTraces(firstDocument.FullPath, secondDocument.FullPath, secondDocument.ProductTrace);
            foreach (var currentPage in secondDocument.Pages.Where(x => x.IsNotMatched()))
            {
                var pageFirstDocument = FindPage(firstDocument, currentPage, secondDocument);
                if (pageFirstDocument == null)
                {
                    //Aucune page avec un minimum des correspondance n'existe dans le document.
                    continue;
                }

                currentPage.MatchedPage = pageFirstDocument;
                pageFirstDocument.MatchedPage = currentPage.MatchedPage;

                MatchedLines(pageFirstDocument.TextData, currentPage.TextData);
                MatchedLinesNotSameIndex(pageFirstDocument.TextData, currentPage.TextData);
                MatchedLinesMostLikely(pageFirstDocument.TextData, currentPage.TextData);

                var differences = GetDifferences(currentPage);
                MapDifferenceTraces(traces.Document2, currentPage, differences);

                var notMatched = pageFirstDocument.TextData.Where(x => x.Found == null && x.Matched == null).ToList();
                MapNotMatchedTraces(traces.Document1, pageFirstDocument, notMatched);
            }
            
            MapTracePagesAbsentes(traces.Document1, firstDocument);
            MapTracePagesAbsentes(traces.Document2, secondDocument);

            if (traces.HasNoTraces()) return true;
            SaveTraces(secondDocument.Filename, outputPath, traces);
            SaveDocument(firstDocument, outputPath, traces.Document1.FileName);
            SaveDocument(secondDocument, outputPath, traces.Document2.FileName);
            return false;
        }

        private static List<string> GetDifferences(PageData currentPage)
        {
            var differences = new List<string>();
            foreach (var item in currentPage.TextData.Where(x => x.Found == null))
            {
                differences.Add(item.Text);
                if (item.Position != null)
                {
                    currentPage.DocumentPage.Graphics.DrawEllipse(new PdfSolidBrush(new PdfColor(Color.Red)),
                        582, item.Position.Y, item.Position.Height, item.Position.Height);
                    currentPage.DocumentPage.Graphics.DrawString(TRACE_MARK, new PdfStandardFont(PdfFontFamily.Courier, 8),
                        new PdfSolidBrush(new PdfColor(Color.Red)), 590, item.Position.Y);
                    currentPage.DocumentPage.Graphics.DrawRectangle(PdfPens.Red, PdfBrushes.Transparent, item.Position.GetRectangleF());
                }
                
                //if (item.TextLine != null)
                //{
                //    if (item.Matched == null)
                //    {
                //        foreach (var word in item.TextLine.WordCollection)
                //        {
                //            currentPage.DocumentPage.Graphics.DrawRectangle(PdfPens.Red, PdfBrushes.Transparent, word.Bounds);
                //        }
                //    }
                //    else
                //    {
                //        var words = item.TextLine.WordCollection.Where(x => !item.Matched.Words.Contains(x.Text) && !string.IsNullOrWhiteSpace(x.Text));
                //        foreach (var word in words)
                //        {
                //            currentPage.DocumentPage.Graphics.DrawRectangle(PdfPens.Red, PdfBrushes.Transparent, word.Bounds);
                //        }
                //    }
                //}
            }

            return differences;
        }

        private static void SaveDocument(DocumentData document, string path, string filename)
        {
            var fullName = Path.Combine(path, filename);
            using (var fs = new FileStream(fullName, FileMode.Create))
            {
                document.LoadedDocument.Save(fs);
            }
        }

        private static void MapNotMatchedTraces(DocumentTrace trace, PageData page, List<TextData> unmatched)
        {
            if (unmatched.Count <= 0) return;
            
            foreach (var item in unmatched)
            {
                if (item.Position != null)
                {
                    page.DocumentPage.Graphics.DrawEllipse(new PdfSolidBrush(new PdfColor(Color.Red)),
                        582, item.Position.Y, item.Position.Height, item.Position.Height);
                    page.DocumentPage.Graphics.DrawString(TRACE_MARK, new PdfStandardFont(PdfFontFamily.Courier, 8),
                        new PdfSolidBrush(new PdfColor(Color.Red)), 590, item.Position.Y);
                    page.DocumentPage.Graphics.DrawRectangle(PdfPens.Red, PdfBrushes.Transparent, item.Position.GetRectangleF());
                }

                //foreach (var word in item.TextLine.WordCollection)
                //{
                //    page.DocumentPage.Graphics.DrawRectangle(PdfPens.Red, PdfBrushes.Transparent, 
                //        new RectangleF(item.Position.X, item.Position.Y, item.Position.Width, item.Position.Height));
                //}
            }

            trace.Pages.Add(new PageTrace
            {
                PageIndex = page.Index,
                Pagination = page.Pagination,
                Title = page.TitrePage,
                Unmatched = unmatched.Select(x => x.Text).ToList()
            });
        }

        private static void MapDifferenceTraces(DocumentTrace trace, PageData page, List<string> differences)
        {
            if (differences.Count > 0)
            {
                trace.Pages.Add(new PageTrace
                {
                    PageIndex = page.Index,
                    Pagination = page.Pagination,
                    Title = page.TitrePage,
                    Texts = differences
                });
            }
        }

        private static void SaveTraces(string fileName, string path, Traces.Traces traces)
        {
            var fullName =
                Path.Combine(path, $"{Path.GetFileNameWithoutExtension(fileName)}_TRACE.json");

            var data = JsonConvert.SerializeObject(
                traces,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    Converters = new JsonConverter[] {new Newtonsoft.Json.Converters.StringEnumConverter()}
                });

            File.WriteAllText(fullName, data, Encoding.GetEncoding("UTF-8"));
        }

        private static void MapTracePagesAbsentes(DocumentTrace trace, DocumentData document)
        {
            var notFoundPages = document.Pages.Where(x => x.IsNotMatched()).ToList();
            if (!notFoundPages.Any()) return;

            notFoundPages.ForEach(x =>
            {
                x.DocumentPage.Graphics.DrawEllipse(new PdfSolidBrush(new PdfColor(Color.Red)), 560, 50, 40, 40);
                x.DocumentPage.Graphics.DrawString(TRACE_MARK, new PdfStandardFont(PdfFontFamily.Courier, 8),
                    new PdfSolidBrush(new PdfColor(Color.Red)), 590, 50);
            });

            trace.Unmatched = notFoundPages.Select(x => new PageTrace
            {
                PageIndex = x.Index,
                Pagination = x.Pagination,
                Title = x.TitrePage
            }).ToList();
        }

        private static Traces.Traces CreateTraces(string docName1, string docName2, string productTrace)
        {
            return new Traces.Traces
            {
                Document1 = new DocumentTrace
                {
                    Document = docName1,
                    ProductTrace = productTrace,
                    FileName = $"{Path.GetFileNameWithoutExtension(docName1)}_1.pdf",
                    Pages = new List<PageTrace>(), 
                    Unmatched = new List<PageTrace>()
                },
                Document2 = new DocumentTrace
                {
                    Document = docName2,
                    ProductTrace = productTrace,
                    FileName = $"{Path.GetFileNameWithoutExtension(docName2)}_2.pdf",
                    Pages = new List<PageTrace>(), 
                    Unmatched = new List<PageTrace>()
                }
            };
        }

        private static PageData FindPage(DocumentData firstDocument, PageData currentPage, DocumentData secondDocument)
        {
            var pages = firstDocument.Pages
                .Where(x => x.IsNotMatched())
                .Select(x => new { Page = x, ExceptionList = currentPage.Words.Except(x.Words).ToList() })
                .OrderBy(x => x.ExceptionList.Count);

            foreach (var canditate in pages)
            {
                //Verifier que la page ne pourrait pas faire mieux ailleurs...
                var bestMatch = secondDocument.Pages
                    .Where(x => x.IsNotMatched())
                    .Select(x => new { Page = x, ExceptionList = canditate.Page.Words.Except(x.Words).ToList() })
                    .OrderBy(x => x.ExceptionList.Count)
                    .FirstOrDefault();

                if (bestMatch != null && bestMatch.Page == currentPage) return canditate.Page;
            }

            return null;
        }

        private static void MatchedLinesMostLikely(IReadOnlyCollection<TextData> origin, IEnumerable<TextData> textLines)
        {
            foreach (var item in textLines.Where(x => x.Found == null))
            {
                var matched = origin.Where(x => x.Found == null)
                    .Select(x => new Tuple<TextData, List<string>, List<string>>(x,
                        item.Words.Intersect(x.Words).Where(s => !string.IsNullOrWhiteSpace(s)).ToList(), item.Words.Except(x.Words).ToList()))
                    .OrderByDescending(x => x.Item2.Count)
                    .ThenBy(x => x.Item3.Count)
                    .FirstOrDefault(x => x.Item2.Count > 0);

                if (matched == null || matched.Item2.Count >= item.Words.Count) continue;
                item.Matched = matched.Item1;
                matched.Item1.Matched = item;
            }
        }

        private static void MatchedLinesNotSameIndex(IReadOnlyCollection<TextData> origin, IEnumerable<TextData> textLines)
        {
            foreach (var item in textLines.Where(x => x.Found == null))
            {
                var found = origin.FirstOrDefault(x => x.Text == item.Text && x.Found == null);
                if (found == null) continue;
                item.Found = found;
                found.Found = item;
            }
        }

        private static void MatchedLines(IList<TextData> origin, IEnumerable<TextData> textLines)
        {
            foreach (var item in textLines)
            {
                var found = origin.FirstOrDefault(x => x.Index == item.Index && x.Text == item.Text);
                if (found == null) continue;
                item.Found = found;
                found.Found = item;
            }
        }

        private static void TraiterPagesIdentiques(DocumentData firstDocument, DocumentData secondDocument)
        {
            foreach (var page in secondDocument.Pages)
            {
                var identicalPageFound = firstDocument.Pages.FirstOrDefault(p => p.IsNotMatched() && p.Words.SequenceEqual(page.Words));
                if (identicalPageFound == null) continue;
                //Une page identique a ete trouvee dans le document.
                identicalPageFound.MatchedPage = page;
                page.MatchedPage = identicalPageFound;
            }
        }
    }
}
