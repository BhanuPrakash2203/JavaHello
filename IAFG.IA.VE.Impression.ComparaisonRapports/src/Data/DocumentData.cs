using System.Collections.Generic;
using System.IO;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.Data
{
    public class DocumentData
    {
        public DocumentData(string fileName, PdfLoadedDocument document, DocumentData first)
        {
            LoadedDocument = document;
            Filename = Path.GetFileName(fileName);
            FullPath = fileName;
            Tags = new List<string>(first.Tags);

            Pages = new List<PageData>();
            for (var i = 0; i < document.Pages.Count; i++)
            {
                Pages.Add(GetPageData(i, document.Pages[i], this));
            }

            ProductTrace = first.ProductTrace;
        }

        public DocumentData(string fileName, PdfLoadedDocument document)
        {
            LoadedDocument = document;
            Filename = Path.GetFileName(fileName);
            FullPath = fileName;
            Tags = new List<string>();

            Pages = new List<PageData>();
            for (var i = 0; i < document.Pages.Count; i++)
            {
                Pages.Add(GetPageData(i, document.Pages[i], this));
            }
        }

        private static PageData GetPageData(int index, PdfPageBase documentPage, DocumentData documentData)
        {
            return new PageData(index, documentPage, documentData);
        }

        public List<PageData> Pages { get; }
        public string Filename { get; }
        public string FullPath { get; }
        public string ProductTrace { get; set; }
        public List<string> Tags { get; }
        public PdfLoadedDocument LoadedDocument { get; }
    }
}