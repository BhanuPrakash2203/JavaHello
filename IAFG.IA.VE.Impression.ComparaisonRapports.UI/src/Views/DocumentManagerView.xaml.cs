using System.Windows;
using System.Windows.Controls;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.Common.Classes;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.ViewModels;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.Views
{
    /// <summary>
    /// Interaction logic for DocumentManagerView.xaml
    /// </summary>
    public partial class DocumentManagerView : UserControl
    {
        public DocumentManagerView()
        {
            InitializeComponent();
            var viewModel = new DocumentManager(new DialogsHandler())
            {
                Dialogs =
                {
                    ShowMessage = MessageBoxShow, 
                    ShowFolderSelectDialog = FolderSelectDialogShow
                },
                ControlHandler = new ControlHandler { ExpandAll = ExpandAll }
            };

            DataContext = viewModel;
        }

        private void ExpandAll(string name, bool value)
        {
            if (FindName(name) is ItemsControl control)
            {
                ExpandAll(control, value);
            }
        }

        private static void ExpandAll(ItemsControl items, bool isExpanded)
        {
            foreach (var obj in items.Items)
            {
                var childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                if (childControl is TreeViewItem item)
                {
                    item.IsExpanded = isExpanded;
                }

                if (childControl != null)
                {
                    ExpandAll(childControl, isExpanded);
                }
            }
        }

        private MessageBoxResult MessageBoxShow(string messageBoxText,
            string caption,
            MessageBoxButton button,
            MessageBoxImage icone)
        {
            return Dispatcher?.CheckAccess() != false ?
                MessageBox.Show(messageBoxText, caption, button, icone) :
                Dispatcher.Invoke(() => MessageBox.Show(messageBoxText, caption, button, icone));
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
