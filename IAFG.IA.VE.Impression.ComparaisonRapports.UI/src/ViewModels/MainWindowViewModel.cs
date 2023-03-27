using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.Common.Classes;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.Common.Constants;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public DialogsHandler Dialogs { get; }

        private string _folder1;
        public string Folder1
        {
            get => _folder1;
            set
            {
                _folder1 = value;
                NotifyChange();
            }
        }

        private string _folder2;
        public string Folder2
        {
            get => _folder2;
            set
            {
                _folder2 = value;
                NotifyChange();
            }
        }

        private string _outputFolder;
        public string OutputFolder
        {
            get => _outputFolder;
            set
            {
                _outputFolder = value;
                NotifyChange();
            }
        }

        private int _currentFile;
        public int CurrentFile
        {
            get => _currentFile;
            set
            {
                _currentFile = value;
                NotifyChange();
            }
        }

        private int _totalFiles;
        public int TotalFiles
        {
            get => _totalFiles;
            set
            {
                _totalFiles = value;
                NotifyChange();
            }
        }

        private string _resutls;
        public string Results
        {
            get => _resutls;
            set
            {
                _resutls = value;
                NotifyChange();
            }
        }

        private bool _isProcessing;
        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                _isProcessing = value;
                NotifyChange();
            }
        }

        public ICommand PdfFolderBrowseCommand1 { get; }
        public ICommand PdfFolderBrowseCommand2 { get; }
        public ICommand OutputResultsFolderBrowseCommand { get; }
        public ICommand CompareCommand { get; }

        public MainWindowViewModel(DialogsHandler dialogs)
        {
            Dialogs = dialogs;
            PdfFolderBrowseCommand1 = new RelayCommand(PdfFolderBrowseCommand1Handler, o => true);
            PdfFolderBrowseCommand2 = new RelayCommand(PdfFolderBrowseCommand2Handler, o => true);
            OutputResultsFolderBrowseCommand = new RelayCommand(OutputResultsFolderBrowseCommandHandler, o => true);
            CompareCommand = new RelayCommand(CompareCommandHandler, CanExecuteCompareCommand);
        }

        private void PdfFolderBrowseCommand1Handler(object obj)
        {
            var path = Dialogs.ShowFolderSelectDialog(obj, Folder1, Messages.SELECT_REPORTS_FOLDER_TITLE);
            if (!string.IsNullOrEmpty(path)) Folder1 = path;
        }

        private void PdfFolderBrowseCommand2Handler(object obj)
        {
            var path = Dialogs.ShowFolderSelectDialog(obj, Folder2, Messages.SELECT_REPORTS_FOLDER_TITLE);
            if (!string.IsNullOrEmpty(path)) Folder2 = path;
        }
        private void OutputResultsFolderBrowseCommandHandler(object obj)
        {
            var path = Dialogs.ShowFolderSelectDialog(obj, OutputFolder, Messages.SELECT_OUTPUT_FOLDER_TITLE);
            if (!string.IsNullOrEmpty(path)) OutputFolder = path;
        }

        private void CompareCommandHandler(object obj)
        {
            if (!FolderExists(Folder1) || !FolderExists(Folder2)) return;
            if (!Directory.Exists(OutputFolder)) Directory.CreateDirectory(OutputFolder);

            CurrentFile = 0;
            TotalFiles = PdfComparator.GetTotalFilesCount(Folder1);
            Results = string.Empty;
            IsProcessing = true;

            var progress = new Progress<int>(v => { CurrentFile += v; });
            Task.Run(() => DoWork(progress));
        }

        private void DoWork(IProgress<int> progress)
        {
            try
            {
                var result = new PdfComparator().Compare(Folder1, Folder2, OutputFolder, progress);
                var resultText = new StringBuilder();
                resultText.AppendLine(Messages.MSG_COMPARISON_PROCESS_FINISHED);
                resultText.AppendLine($"Nombre total de fichiers : {result.FilesCount}");
                resultText.AppendLine($"Nombre de fichiers identiques : {result.FilesIdentical}");
                resultText.AppendLine($"Nombre de fichiers non-trouvés : {result.FilesNotFound}");
                resultText.AppendLine($"Nombre de fichiers avec des différences : {result.FilesWithDifferences}");
                resultText.AppendLine($"Nombre de fichiers avec des erreurs : {result.FilesWithError}");
                Results = resultText.ToString();
                IsProcessing = false;
                Dialogs.ShowMessage(Results, Messages.TITLE_DONE, 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private bool CanExecuteCompareCommand(object obj)
        {
            return !(string.IsNullOrEmpty(Folder1) || string.IsNullOrEmpty(Folder2) || string.IsNullOrEmpty(OutputFolder));
        }

        private bool FolderExists(string folder)
        {
            if (Directory.Exists(folder))
            {
                return true;
            }

            Dialogs.ShowMessage(Messages.GetMessageFolderNotFound(folder),
                Messages.TITLE_ERROR,
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyChange([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
