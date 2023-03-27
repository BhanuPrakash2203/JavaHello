using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.ViewModels
{
    public class FiltrePageViewModel : FiltreBaseViewModel
    {
        public FiltrePageViewModel()
        {
            Filtres = new ObservableCollection<FiltreTexteViewModel>();
        }

        private ActionFiltrePage _action;
        public ActionFiltrePage Action
        {
            get => _action;
            set
            {
                _action = value;
                NotifyChange();
                NotifyChange(nameof(HasAction));
                NotifyChange(nameof(ActionFormatter));
            }
        }

        public bool HasAction => Action != ActionFiltrePage.NonApplicable;
        public string ActionFormatter => GetTexteAction(Action);
        public string TitrePage { get; set; }
        public ObservableCollection<FiltreTexteViewModel> Filtres { get; set; }

        public void AjouterFiltreTexte(ActionFiltre actionFiltre, string texte)
        {
            var filtreTexte = GetOrCreateFiltreTexte(Filtres, actionFiltre);
            if (!string.IsNullOrWhiteSpace(texte))
            {
                filtreTexte.Textes.Add(
                    new TexteValeurViewModel
                    {
                        Valeur = texte, 
                        FiltreDocument = this.FiltreDocument
                    });
            }

            NotifyChange(nameof(Filtres));
        }

        private FiltreTexteViewModel GetOrCreateFiltreTexte(ICollection<FiltreTexteViewModel> filtres, ActionFiltre actionFiltre)
        {
            var filtre = filtres.FirstOrDefault(x => x.Action == actionFiltre);
            if (filtre != null)
            {
                return filtre;
            }

            filtre = new FiltreTexteViewModel
            {
                Action = actionFiltre, 
                FiltreDocument = this.FiltreDocument
            };
            
            filtres.Add(filtre);
            return filtre;
        }

        public void RemoveChild(string id)
        {
            if (Filtres == null) return;
            var found = Filtres?.FirstOrDefault(x => x.Id == id);
            if (found != null) Filtres.Remove(found);
            Filtres.ToList().ForEach(x => x.RemoveChild(id));
            Filtres.Where(x => !x.Textes.Any() && !x.IsActionFiltreLigneTableau).ToList().ForEach(x => Filtres.Remove(x));
        }

        private static string GetTexteAction(ActionFiltrePage action)
        {
            switch (action)
            {
                case ActionFiltrePage.NonApplicable:
                    return string.Empty;
                case ActionFiltrePage.Egale:
                    return "Ne pas considérer les pages avec ce titre";
                case ActionFiltrePage.DebutePar:
                    return "Ne pas considérer les pages qui débute par ce titre";
                default:
                    throw new ArgumentOutOfRangeException(nameof(action));
            }
        }

    }
}