using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.Common.Classes;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.Common.Constants;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.ViewModels
{
    public class DocumentViewModel : BaseItem
    {
        public ICommand OpenFilesCommand { get; }

        public DocumentViewModel()
        {
            DocumentTraces = new ObservableCollection<DocumentTraceViewModel>();
            OpenFilesCommand = new RelayCommand(OpenFilesCommandHandler, o => true);
        }

        public string FileName { get; set; }
        public string Produit => !string.IsNullOrWhiteSpace(Document1?.Produit) ? Document1.Produit : Document2?.Produit;

        public ObservableCollection<DocumentTraceViewModel> DocumentTraces { get; set; }
        public DocumentTraceViewModel Document1 { get; set; }
        public DocumentTraceViewModel Document2 { get; set; }

        private void OpenFilesCommandHandler(object obj)
        {
            var doc1NotExists = false;
            if (!string.IsNullOrWhiteSpace(Document1?.FullName))
            {
                doc1NotExists = !File.Exists(Document1.FullName);
                if (!doc1NotExists)
                {
                    Process.Start(new ProcessStartInfo(Document1.FullName));
                }
            }

            var doc2NotExists = false;
            if (!string.IsNullOrWhiteSpace(Document2?.FullName))
            {
                doc2NotExists = !File.Exists(Document2.FullName);
                if (!doc2NotExists)
                {
                    Process.Start(new ProcessStartInfo(Document2.FullName));
                }
            }

            if (doc1NotExists)
            {
                DocumentManager.Dialogs.ShowMessage(
                    Messages.GetMessageFileNotFound(Document1.FullName),
                    Messages.TITLE_ERROR,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            if (doc2NotExists)
            {
                DocumentManager.Dialogs.ShowMessage(
                    Messages.GetMessageFileNotFound(Document2.FullName),
                    Messages.TITLE_ERROR,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public override bool ValiderEstVisible()
        {
            return IsVisible && DocumentTraces.Any(x => x.ValiderEstVisible());
        }
    }
}