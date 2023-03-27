using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.Providers;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.ViewModels
{
    public interface IFiltreManager
    {
        void ApplyFiltres();
        void Save(string path);
        void Load(string path);
    }

    public class FiltreManager : IFiltreManager, INotifyPropertyChanged
    {
        public IDocumentManager DocumentManager { get; }

        public FiltreManager(IDocumentManager documentManager)
        {
            DocumentManager = documentManager;
            Document1 = new FiltreDocumentViewModel { Titre = "Document - 1", FiltreManager = this};
            Document2 = new FiltreDocumentViewModel { Titre = "Document - 2", FiltreManager = this };
            Documents = new ObservableCollection<FiltreDocumentViewModel> { Document1, Document2 };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyChange([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<FiltreDocumentViewModel> Documents { get; set; }
        public FiltreDocumentViewModel Document1 { get; set; }
        public FiltreDocumentViewModel Document2 { get; set; }

        public void ApplyFiltres()
        {
            DocumentManager.ApplyFiltres();
        }

        public void Save(string path)
        {
            new FiltreDataProvider().Save(path, Document1, Document2);
        }

        public void Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            Documents.ToList().ForEach(x => x.Filtres.Clear());
            var provider = new FiltreDataProvider();
            provider.Load(path, Document1, Document2);

            NotifyChange(nameof(Document1));
            NotifyChange(nameof(Document2));
            NotifyChange(nameof(Documents));
        }
    }
}