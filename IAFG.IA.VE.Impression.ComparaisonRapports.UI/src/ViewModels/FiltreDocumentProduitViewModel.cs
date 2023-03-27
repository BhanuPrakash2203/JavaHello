using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.ViewModels
{
    public class FiltreDocumentProduitViewModel : FiltreBaseViewModel, IFiltreDocument
    {
        public string Produit { get; set; }
        public ObservableCollection<FiltrePageViewModel> Filtres { get; set; }
        public IFiltreManager FiltreManager { get; set; }

        public FiltreDocumentProduitViewModel()
        {
            Filtres = new ObservableCollection<FiltrePageViewModel>();
        }

        public void SupprimerFiltre(FiltreBaseViewModel filtreBaseViewModel)
        {
            if (Filtres == null) return;
            var found = Filtres?.FirstOrDefault(x => x.Id == filtreBaseViewModel.Id);
            if (found != null)
            {
                found.Action = ActionFiltrePage.NonApplicable;
            }

            Filtres.ToList().ForEach(x => x.RemoveChild(filtreBaseViewModel.Id));
            Filtres.Where(x => !x.Filtres.Any() && x.Action == ActionFiltrePage.NonApplicable).ToList().ForEach(x => Filtres.Remove(x));
            NotifyChange(nameof(Filtres));
        }


        public void AddFiltrePageTitre(ActionFiltrePage actionFiltre, string titrePage)
        {
            GetOrCreateFiltrePage(Filtres, titrePage, actionFiltre);
        }

        public void AddFiltreTexte(ActionFiltre actionFiltre, string texte, string titrePage)
        {
            var filtrePage = GetOrCreateFiltrePage(Filtres, titrePage);
            filtrePage.AjouterFiltreTexte(actionFiltre, texte);
        }

        private FiltrePageViewModel GetOrCreateFiltrePage(
            ICollection<FiltrePageViewModel> filtres, string title, ActionFiltrePage? actionFiltre = null)
        {
            var filtre = Filtres.FirstOrDefault(x => x.TitrePage == title);
            if (filtre != null)
            {
                if (actionFiltre != null) filtre.Action = actionFiltre.GetValueOrDefault();
                return filtre;
            }

            filtre = new FiltrePageViewModel
            {
                TitrePage = title,
                Action = actionFiltre.GetValueOrDefault(),
                FiltreDocument = this
            };

            filtres.Add(filtre);
            return filtre;
        }
    }
}