using System;
using System.Linq;
using System.Windows.Input;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.Common.Classes;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.ViewModels
{
    public class TexteViewModel : BaseItem
    {
        public ICommand AjouterFiltreTextTextCommand { get; }
        public ICommand AjouterFiltreTextDebuteParCommand { get; }

        public TexteViewModel()
        {
            AjouterFiltreTextTextCommand = new RelayCommand(AjouterFiltreTextCommandHandler, o => true);
            AjouterFiltreTextDebuteParCommand = new RelayCommand(AjouterFiltreTextDebuteParCommandHandler, o => true);
        }

        private void AjouterFiltreTextCommandHandler(object obj)
        {
            DocumentManager.AddFiltreTexte(ActionFiltre.Egale,this, Parent as PageViewModel);
        }

        private void AjouterFiltreTextDebuteParCommandHandler(object obj)
        {
            DocumentManager.AddFiltreTexte(ActionFiltre.DebutePar,this, Parent as PageViewModel);
        }

        public bool EstLigneTableau { get; set; }

        public bool ValiderEstLigneValeurNumerique(string valeur)
        {
            var w = new string(valeur.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());

            var v = w
                .Replace("-", "")
                .Replace("$", "")
                .Replace("%", "")
                .Replace(".", "")
                .Replace(",", "")
                .Trim();

            return v.All(char.IsDigit);
        }

        private string _valeur;
        public string Valeur
        {
            get => _valeur;
            set
            {
                _valeur = value;
                EstLigneTableau = ValiderEstLigneValeurNumerique(value);
            }
        }
    }
}