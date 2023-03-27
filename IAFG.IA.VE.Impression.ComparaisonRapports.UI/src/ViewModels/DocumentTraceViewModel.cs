using System;
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
    public class DocumentTraceViewModel : BaseItem
    {
        public ICommand OpenFileCommand { get; }

        public DocumentTraceViewModel()
        {
            Pages = new ObservableCollection<PageViewModel>();
            OpenFileCommand = new RelayCommand(OpenFileCommandHandler, o => true);
        }

        public string FullName { get; set; }
        public string FileName { get; set; }
        public string Produit { get; set; }
        public int Sequence { get; set; }
        public FiltreDocumentViewModel FiltreDocument { get; set; }
        public ObservableCollection<PageViewModel> Pages { get; set; }

        private void OpenFileCommandHandler(object obj)
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                return;
            }

            if (File.Exists(FullName))
            {
                try
                {
                    const string search = "search=\"[T]\"";
                    Process.Start(new ProcessStartInfo("AcroRdd32.exe", $"/A \"{search}\" \"{FullName}\""));
                }
                catch (Exception)
                {
                    Process.Start(new ProcessStartInfo(FullName));
                }
            }
            else
            {
                DocumentManager.Dialogs.ShowMessage(
                    Messages.GetMessageFileNotFound(FullName),
                    Messages.TITLE_ERROR,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public override bool IsVisibleHandler => true;

        public override bool ValiderEstVisible()
        {
            return IsVisible && Pages.Any(x => x.ValiderEstVisible());
        }
    }
}