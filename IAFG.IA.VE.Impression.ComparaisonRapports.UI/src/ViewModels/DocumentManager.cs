using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.Common.Classes;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.Common.Constants;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.Providers;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.Views;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.ViewModels
{
    public interface IDocumentManager
    {
        DialogsHandler Dialogs { get; }
        void AddFiltreTexte(ActionFiltre actionFiltre, TexteViewModel texteViewModel, PageViewModel pageViewModel);
        void AddFiltrePageTitre(ActionFiltrePage actionFiltre, PageViewModel pageViewModel);
        void ApplyFiltres();
    }

    public class DocumentManager : IDocumentManager, INotifyPropertyChanged
    {
        public DialogsHandler Dialogs { get; }
        public ControlHandler ControlHandler { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyChange([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand ResultsFolderBrowseCommand { get; }

        public DocumentManager(DialogsHandler dialogs)
        {
            Dialogs = dialogs;
            ResultsFolderBrowseCommand = new RelayCommand(ResultsFolderBrowseCommandHandler, o => true);
            Documents = new ObservableCollection<DocumentViewModel>();
            FiltreManager = new FiltreManager(this);
        }

        private void ResultsFolderBrowseCommandHandler(object obj)
        {
            var path = Dialogs.ShowFolderSelectDialog(obj, Folder, 
                Messages.SELECT_RESULTS_FOLDER_TITLE);
            
            IsProcessing = true;
            try
            {
                if (ExecuteAnalyse(path))
                {
                    ControlHandler?.ExpandAll(nameof(DocumentManagerView.ListFiltres), true);
                }
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private string _folder;
        public string Folder
        {
            get => _folder;
            set
            {
                _folder = value;
                NotifyChange();
            }
        }

        public int FileCount => Documents.Count();
        public int FileVisibleCount => Documents.Count(x => x.ValiderEstVisible());

        private bool _isProcessing;
        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                _isProcessing = value;
                NotifyChange();
                CallCursor(value);
            }
        }

        private static void CallCursor(bool value)
        {
            var myCursor = value ? Cursors.Wait : null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Mouse.OverrideCursor != myCursor)
                    Mouse.OverrideCursor = myCursor;
            });
        }

        private ObservableCollection<DocumentViewModel> _documents;
        public ObservableCollection<DocumentViewModel> Documents
        {
            get => _documents;
            set
            {
                _documents = value;
                NotifyChange();
                NotifyChange(nameof(FileCount));
                NotifyChange(nameof(FileVisibleCount));
            }
        }

        private FiltreManager _filtreManager;
        public FiltreManager FiltreManager
        {
            get => _filtreManager;
            set
            {
                _filtreManager = value;
                NotifyChange();
            }
        }

        private void NotifyDocumentsChange()
        {
            NotifyChange(nameof(FileVisibleCount));
        }

        public void AddFiltrePageTitre(ActionFiltrePage actionFiltre, PageViewModel pageViewModel)
        {
            pageViewModel.GetFiltreDocument().AddFiltrePageTitre(
                pageViewModel.GetDocumentTrace().Produit, 
                actionFiltre, 
                pageViewModel.Title);

            NotifyChange(nameof(FiltreManager));
            ApplyFiltres();
        }

        public void AddFiltreTexte(ActionFiltre actionFiltre, TexteViewModel texteViewModel, PageViewModel pageViewModel)
        {
            pageViewModel.GetFiltreDocument().AddFiltreTexte(
                pageViewModel.GetDocumentTrace().Produit, 
                actionFiltre, texteViewModel?.Valeur, 
                pageViewModel.Title);

            NotifyChange(nameof(FiltreManager));
            ApplyFiltres();
        }
        
        public void ApplyFiltres()
        {
            try
            {
                IsProcessing = true;
                ApplyFiltres(true);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        public bool ExecuteAnalyse(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            Folder = path;
            var provider = new DocumentProvider();
            Documents = provider.LoadDocuments(Folder, this);
            FiltreManager.Load(Folder);
            ApplyFiltres(false);
            return true;
        }

        private void ApplyFiltres(bool save)
        {
            if (save) FiltreManager.Save(Folder);
            var list = Documents.ToList();

            ApplyFiltres(list.GroupBy(x => x.Produit), FiltreManager);
            list.ForEach(x => x.DocumentTraces.ToList().ForEach(d => d.NotifyIsVisibilty()));
            list.ForEach(x => x.NotifyIsVisibilty());
            NotifyDocumentsChange();
        }

        private static void ApplyFiltres(IEnumerable<IGrouping<string, DocumentViewModel>> list, FiltreManager filtreManager)
        {
            foreach (var item in list)
            {
                ApplyFiltres(item.SelectMany(x => x.Document1.Pages),
                    filtreManager?.Document1?.Filtres?.Where(x => x.Produit == item.Key)
                        .SelectMany(x => x.Filtres).ToList());

                ApplyFiltres(item.SelectMany(x => x.Document2.Pages),
                    filtreManager?.Document2?.Filtres?.Where(x => x.Produit == item.Key)
                        .SelectMany(x => x.Filtres).ToList());
            }
        }

        private static void ApplyFiltres(IEnumerable<PageViewModel> pages, IReadOnlyCollection<FiltrePageViewModel> filtreViewModel)
        {
            foreach (var page in pages)
            {
                page.SetVisibility(true, false);
                page.Textes.ToList().ForEach(x => x.SetVisibility(true, false));
                var filtres = filtreViewModel?.Where(x => x.TitrePage == page.Title) ?? new FiltrePageViewModel[]{};
                foreach (var filtre in filtres.Where(x => x.TitrePage == page.Title))
                {
                    ApplyActionFiltrePage(filtre, page);
                }

                page.Textes.ToList().ForEach(x => x.NotifyIsVisibilty());
                page.NotifyIsVisibilty();
            }
        }

        private static void ApplyActionFiltrePage(FiltrePageViewModel filtre, PageViewModel page)
        {
            switch (filtre.Action)
            {
                case ActionFiltrePage.Egale:
                    ApplyActionFiltrePageTitre((x, y) => x == y, filtre.TitrePage, page);
                    break;
                case ActionFiltrePage.DebutePar:
                    ApplyActionFiltrePageTitre((x, y) => x.StartsWith(y), filtre.TitrePage, page);
                    break;
                case ActionFiltrePage.NonApplicable:
                    ApplyFiltreTextes(filtre.Filtres.ToList(), page);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filtre));
            }
        }

        private static void ApplyActionFiltrePageTitre(Func<string, string, bool> action, string titre, PageViewModel page)
        {
            if (action(page.Title, titre))
            {
                page.SetVisibility(false);
            }
        }

        private static void ApplyFiltreTextes(IReadOnlyCollection<FiltreTexteViewModel> filtres, PageViewModel page)
        {
            foreach (var texte in page.Textes)
            {
                if (filtres.Any(filtre => ApplyFiltreTexte(filtre, texte)))
                {
                    texte.SetVisibility(false);
                }
            }
        }

        private static bool ApplyFiltreTexte(FiltreTexteViewModel filtre, TexteViewModel texte)
        {
            switch (filtre.Action)
            {
                case ActionFiltre.NonApplicable:
                    return false;
                case ActionFiltre.Egale:
                    return filtre.Textes.Any(x => x.Valeur == texte.Valeur);
                case ActionFiltre.DebutePar:
                    return filtre.Textes.Any(x => texte.Valeur.StartsWith(x.Valeur));
                case ActionFiltre.EstLigneValeurNumerique:
                    return texte.EstLigneTableau;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filtre));
            }
        }
    }
}