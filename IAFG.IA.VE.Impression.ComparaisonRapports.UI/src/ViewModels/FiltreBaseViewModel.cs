using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.Common.Classes;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.ViewModels
{
    public class FiltreBaseViewModel : INotifyPropertyChanged
    {
        public string Id { get; }
        public IFiltreDocument FiltreDocument { get; set; }

        public ICommand SupprimerCommand { get; }

        private void SupprimerCommandCommandHandler(object obj)
        {
            FiltreDocument.SupprimerFiltre(this);
        }

        public FiltreBaseViewModel()
        {
            Id = Guid.NewGuid().ToString();
            SupprimerCommand = new RelayCommand(SupprimerCommandCommandHandler, o => true);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyChange([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}