using System.Collections.ObjectModel;
using IAFG.IA.VE.Impression.ComparaisonRapports.Traces;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.ViewModels
{
    public static class ViewModelExtensions
    {
        private static void SetParent(this BaseItem item, BaseItem parent)
        {
            item.Parent = parent;
            item.DocumentManager = parent.DocumentManager;
        }

        public static TexteViewModel CreateTexte(
            this PageViewModel page, 
            string texte)
        {
            var texteViewModel = new TexteViewModel
            {
                Valeur = texte
            };

            texteViewModel.SetParent(page);
            page.Textes.Add(texteViewModel);
            return texteViewModel;
        }

        public static PageViewModel CreatePage(
            this PageTrace page, 
            DocumentTraceViewModel parent, 
            bool estPageAbsente)
        {
            var pageViewModel = new PageViewModel
            {
                Title = page.Title,
                PageIndex = page.PageIndex,
                Pagination = page.Pagination,
                EstPageAbsente = estPageAbsente
            };

            pageViewModel.SetParent(parent);
            return pageViewModel;
        }

        public static DocumentViewModel AddDocument(
            this ObservableCollection<DocumentViewModel> docs, 
            DocumentViewModel documentViewModel, 
            DocumentManager manager)
        {
            documentViewModel.DocumentManager = manager;
            docs.Add(documentViewModel);
            return documentViewModel;
        }

        public static DocumentTraceViewModel AddDocument(
            this ObservableCollection<DocumentTraceViewModel> docs, 
            DocumentTraceViewModel viewModel, 
            DocumentManager manager)
        {
            viewModel.DocumentManager = manager;
            docs.Add(viewModel);
            return viewModel;
        }
    }
}