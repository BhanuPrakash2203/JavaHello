using System.Windows;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.Common.Classes;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.ViewModels;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainWindowViewModel(new DialogsHandler())
            {
                Dialogs =
                {
                    ShowMessage = MessageBoxShow, 
                    ShowFolderSelectDialog = FolderSelectDialogShow
                },
            };

            DataContext = viewModel;
        }

        private MessageBoxResult MessageBoxShow( 
            string messageBoxText,
            string caption,
            MessageBoxButton button,
            MessageBoxImage icone)
        {
            return Dispatcher?.CheckAccess() != false ?
                MessageBox.Show(this, messageBoxText, caption, button, icone) :
                Dispatcher.Invoke(() => MessageBox.Show(this, messageBoxText, caption, button, icone));
        }

        private static string FolderSelectDialogShow(object parameter, string initialDirectory, string title)
        {
            if (!(parameter is Window owner))
            {
                owner = Application.Current.MainWindow;
            }

            var folderSelectDialog = new FolderSelectDialog
            {
                Description = title,
                InitialDirectory = initialDirectory
            };

            return folderSelectDialog.ShowDialog(owner) ?
                folderSelectDialog.SelectedPath :
                string.Empty;
        }
    }
}
