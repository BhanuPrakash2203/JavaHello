using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.ViewModels
{
    public interface IFiltreDocument
    {
        void SupprimerFiltre(FiltreBaseViewModel filtreBaseViewModel);
        IFiltreManager FiltreManager { get; set; }
    }

    public class FiltreDocumentViewModel : FiltreBaseViewModel, IFiltreDocument
    {
        public FiltreDocumentViewModel()
        {
            Filtres = new ObservableCollection<FiltreDocumentProduitViewModel>();
        }

        public IFiltreManager FiltreManager { get; set; }
        public ObservableCollection<FiltreDocumentProduitViewModel> Filtres { get; set; }
        public string Titre { get; set; }

        public void AddFiltrePageTitre(string produit, ActionFiltrePage actionFiltre, string titrePage)
        {
            GetOrCreateFiltrePageProduit(Filtres, produit).AddFiltrePageTitre(actionFiltre, titrePage);
        }

        public void AddFiltreTexte(string produit, ActionFiltre actionFiltre, string texte, string titrePage)
        {
            GetOrCreateFiltrePageProduit(Filtres, produit).AddFiltreTexte(actionFiltre, texte, titrePage);
        }

        public void SupprimerFiltre(FiltreBaseViewModel filtreBaseViewModel)
        {
            foreach (var docViewModel in Filtres)
            {
                docViewModel.SupprimerFiltre(filtreBaseViewModel);
            }
            NotifyChange(nameof(Filtres));
            FiltreManager.ApplyFiltres();
        }

        private FiltreDocumentProduitViewModel GetOrCreateFiltrePageProduit(
            ICollection<FiltreDocumentProduitViewModel> filtres, string produit)
        {
            var filtre = Filtres.FirstOrDefault(x => x.Produit == produit);
            if (filtre != null)
            {
                return filtre;
            }

            filtre = new FiltreDocumentProduitViewModel
            { 
                Produit = produit,
                FiltreDocument = this
            };
            
            filtres.Add(filtre);
            return filtre;
        }
    }
}