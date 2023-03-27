using System;
using System.Windows;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.Common.Classes
{
    public class DialogsHandler
    {
        public Func<string, string, MessageBoxButton, MessageBoxImage, MessageBoxResult> ShowMessage { get; set; }
        public Func<object, string, string, string> ShowFolderSelectDialog { get; set; }
    }
}
