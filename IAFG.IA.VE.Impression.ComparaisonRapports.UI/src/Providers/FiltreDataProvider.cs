using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.ViewModels;
using Newtonsoft.Json;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.Providers
{
    public class FiltreDataProvider
    {
        private const string FILTRES_FILTERS = "__Filtres.filters";

        private class DataDefinition
        {
            public List<DocumentDto> Documents1 { get; set; }
            public List<DocumentDto> Documents2 { get; set; }
        }

        private class DocumentDto
        {
            public string Produit { get; set; }
            public List<PageDto> Pages { get; set; }
        }

        private class PageDto
        {
            public string TitrePage { get; set; }
            public ActionFiltrePage Action { get; set; }
            public List<FiltreDto> Filtres { get; set; }
        }

        private class FiltreDto
        {
            public ActionFiltre Action { get; set; }
            public List<string> Textes { get; set; }
        }

        public void Load(string path, FiltreDocumentViewModel document1, FiltreDocumentViewModel document2)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            var filePath = Path.Combine(path, FILTRES_FILTERS);
            if (!File.Exists(filePath))
            {
                return;
            }

            var data = File.ReadAllText(filePath, Encoding.GetEncoding("UTF-8"));
            var definition = GetDataDefinition();
            Map(definition?.Documents1, document1);
            Map(definition?.Documents2, document2);

            DataDefinition GetDataDefinition()
            {
                try
                {
                    return JsonConvert.DeserializeObject<DataDefinition>(data);
                }
                catch (Exception)
                {
                    // on ne veut que l'application plante si probleme de DeserializeObject.
                    return null;
                }
            }
        }

        public void Save(string path, FiltreDocumentViewModel document1, FiltreDocumentViewModel document2)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            var definition = new DataDefinition
            {
                Documents1 = Map(document1),
                Documents2 = Map(document2)
            };

            var data = JsonConvert.SerializeObject(
                definition, 
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    Converters = new JsonConverter[] { new Newtonsoft.Json.Converters.StringEnumConverter() }
                });

            File.WriteAllText(Path.Combine(path, FILTRES_FILTERS), data, Encoding.GetEncoding("UTF-8"));
        }

        private static List<DocumentDto> Map(FiltreDocumentViewModel documentViewModel)
        {
            var result = new List<DocumentDto>();

            foreach (var docProduit in documentViewModel.Filtres)
            {
                result.Add(new DocumentDto
                {
                    Produit = docProduit.Produit,
                    Pages = Map(docProduit.Filtres)
                });
            }

            return result; 
        }

        private static List<PageDto> Map(IEnumerable<FiltrePageViewModel> pages)
        {
            return pages?.OrderBy(x => x.TitrePage)
                .Select(page => new PageDto
                {
                    TitrePage = page.TitrePage, 
                    Action = page.Action, 
                    Filtres = Map(page.Filtres)
                }).ToList();
        }

        private static List<FiltreDto> Map(IEnumerable<FiltreTexteViewModel> filtres)
        {
            return filtres?.Select(filtre => new FiltreDto
            {
                Action = filtre.Action,
                Textes = filtre.Textes?.Select(t => t.Valeur).ToList()
            }).ToList();
        }

        private static void Map(List<DocumentDto> documents, FiltreDocumentViewModel filtreDocument)
        {
            if (documents == null)
            {
                return;
            }

            foreach (var doc in documents)
            {
                filtreDocument.Filtres.Add(new FiltreDocumentProduitViewModel
                {
                    Produit = doc.Produit,
                    FiltreDocument = filtreDocument,
                    Filtres = MapPages(doc.Pages, filtreDocument)
                });
            }
        }

        private static ObservableCollection<FiltrePageViewModel> MapPages(List<PageDto> pages, FiltreDocumentViewModel filtreDocument)
        {
            var result = new ObservableCollection<FiltrePageViewModel>();

            if (pages == null)
            {
                return result;
            }

            foreach (var page in pages)
            {
                result.Add(new FiltrePageViewModel
                {
                    TitrePage = page.TitrePage,
                    Action = page.Action,
                    FiltreDocument = filtreDocument,
                    Filtres = MapFiltres(page.Filtres, filtreDocument)
                });
            }

            return result;
        }

        private static ObservableCollection<FiltreTexteViewModel> MapFiltres(IReadOnlyCollection<FiltreDto> filtres, IFiltreDocument filtreDocument)
        {
            var result = new ObservableCollection<FiltreTexteViewModel>();
            if (filtres == null) return result;
            foreach (var filtre in filtres)
            {
                result.Add(new FiltreTexteViewModel
                {
                    Action = filtre.Action,
                    FiltreDocument = filtreDocument,
                    Textes = MapTextes(filtre.Textes, filtreDocument)
                });
            }
            return result;
        }

        private static ObservableCollection<TexteValeurViewModel> MapTextes(IReadOnlyCollection<string> textes, IFiltreDocument filtreDocument)
        {
            var result = new ObservableCollection<TexteValeurViewModel>();
            if (textes == null) return result;
            foreach (var texte in textes)
            {
                result.Add(new TexteValeurViewModel
                {
                    Valeur = texte,
                    FiltreDocument = filtreDocument
                });
            }

            return result;
        }
    }
}