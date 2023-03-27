using IAFG.IA.VE.Impression.ComparaisonRapports.UI.Common.Constants;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.Views;
using System;
using System.Windows;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            MainWindow = new MainWindow();
            MainWindow.Show();
            MainWindow.Activate();
        }
      
        private void ApplicationDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(Messages.GetMessageException(e.Exception.Message),
                Messages.TITLE_EXCEPTION,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
