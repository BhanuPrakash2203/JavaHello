using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.ViewModels
{
    public class FiltreTexteViewModel : FiltreBaseViewModel
    {
        public FiltreTexteViewModel()
        {
            Textes = new ObservableCollection<TexteValeurViewModel>();
        }

        private ActionFiltre _action;
        public ActionFiltre Action
        {
            get => _action;
            set
            {
                _action = value;
                NotifyChange();
            }
        }

        public bool HasAction => Action != ActionFiltre.NonApplicable;
        public bool IsActionFiltreLigneTableau => Action== ActionFiltre.EstLigneValeurNumerique;
        public string ActionFormatter => GetTexteAction(Action);
        public ObservableCollection<TexteValeurViewModel> Textes { get; set; }

        public void RemoveChild(string id)
        {
            var found = Textes?.FirstOrDefault(x => x.Id == id);
            if (found != null) Textes.Remove(found);
        }

        private static string GetTexteAction(ActionFiltre action)
        {
            switch (action)
            {
                case ActionFiltre.NonApplicable:
                    return string.Empty;
                case ActionFiltre.Egale:
                    return "Ne pas considérer le texte : ";
                case ActionFiltre.DebutePar:
                    return "Ne pas considérer le texte débutant par : ";
                case ActionFiltre.EstLigneValeurNumerique:
                    return "Ne pas considérer les lignes de tableau (valeurs numériques)";
                default:
                    throw new ArgumentOutOfRangeException(nameof(action));
            }
        }
    }
}