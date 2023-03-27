using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using IAFG.IA.VE.Impression.ComparaisonRapports.Traces;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.ViewModels;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.Providers
{
    public class DocumentProvider
    {
        public ObservableCollection<DocumentViewModel> LoadDocuments(string folder, DocumentManager manager)
        {
            var result = new ObservableCollection<DocumentViewModel>();
            var filenames = GetFiles(folder);
            foreach (var filename in filenames)
            {
                var traces = GetTraces(filename, folder);
                var doc = result.AddDocument(
                    new DocumentViewModel
                    {
                        FileName = Path.GetFileNameWithoutExtension(traces.Document1?.Document ?? traces.Document2.Document)
                    }, 
                    manager);

                if (traces.Document1 != null)
                {
                    doc.Document1 = LoadDocumentTrace(manager, doc, traces.Document1, 1, manager.FiltreManager.Document1);
                }

                if (traces.Document2 != null)
                {
                    doc.Document2 = LoadDocumentTrace(manager, doc, traces.Document2, 2, manager.FiltreManager.Document2);
                }
            }

            return result;
        }

        private static DocumentTraceViewModel LoadDocumentTrace(DocumentManager manager, 
            DocumentViewModel docViewModel, 
            DocumentTrace doc, 
            int sequence,
            FiltreDocumentViewModel filtreDocument) 
        {
            var docTrace = docViewModel.DocumentTraces.AddDocument(
                new DocumentTraceViewModel
                {
                    FileName = Path.GetFileNameWithoutExtension(doc.FileName),
                    FullName = Path.Combine(manager.Folder, doc.FileName),
                    Produit = doc.ProductTrace,
                    FiltreDocument = filtreDocument,
                    Sequence = sequence
                }, 
                manager);
           
            var pages = new List<PageViewModel>();
            if (doc.Pages != null)
            {
                foreach (var page in doc.Pages)
                {
                    var p = page.CreatePage(docTrace, false);
                    page.Texts?.ForEach(x => p.CreateTexte(x));
                    page.Unmatched?.ForEach(x => p.CreateTexte(x));
                    pages.Add(p);
                }
            }

            if (doc.Unmatched != null)
            {
                pages.AddRange(doc.Unmatched.Select(page => page.CreatePage(docTrace, true)));
            }

            pages.OrderBy(x => x.PageIndex).ToList()
                .ForEach(p => docTrace.Pages.Add(p));

            return docTrace;
        }

        public static string[] GetFiles(string folder)
        {
            return string.IsNullOrWhiteSpace(folder) 
                ? new string[0] 
                : Directory.GetFiles(folder, "*.json", SearchOption.TopDirectoryOnly);
        }

        public static Traces.Traces GetTraces(string filename, string folder)
        {
            return TracesExtensions.ReadTraces(filename, folder);
        }
    }
}