using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using IAFG.IA.VE.Impression.ComparaisonRapports.UI.Common.Classes;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.ViewModels
{
    public class PageViewModel : BaseItem
    {
        public ICommand AjouterFiltreTitrePageCommand { get; }
        public ICommand AjouterFiltreTitrePageDebuteParCommand { get; }
        public ICommand AjouterFiltreTextValeurNumeriqueCommand { get; }

        public PageViewModel()
        {
            AjouterFiltreTitrePageCommand = new RelayCommand(AjouterFiltreTitrePageCommandHandler, o => true);
            AjouterFiltreTitrePageDebuteParCommand = new RelayCommand(AjouterFiltreTitrePageDebuteParCommandHandler, o => true);
            AjouterFiltreTextValeurNumeriqueCommand = new RelayCommand(AjouterFiltreTextValeurNumeriqueCommandHandler, o => true);
            Textes = new ObservableCollection<TexteViewModel>();
        }

        private void AjouterFiltreTitrePageCommandHandler(object obj)
        {
            DocumentManager.AddFiltrePageTitre(ActionFiltrePage.Egale, this);
        }

        private void AjouterFiltreTitrePageDebuteParCommandHandler(object obj)
        {
            DocumentManager.AddFiltrePageTitre(ActionFiltrePage.DebutePar, this);
        }

        private void AjouterFiltreTextValeurNumeriqueCommandHandler(object obj)
        {
            DocumentManager.AddFiltreTexte(ActionFiltre.EstLigneValeurNumerique, null, this);
        }

        public override bool ValiderEstVisible()
        {
            return IsVisible && (EstPageAbsente || Textes.Any(x => x.ValiderEstVisible()));
        }

        public FiltreDocumentViewModel GetFiltreDocument()
        {
            return GetDocumentTrace()?.FiltreDocument;
        }

        public DocumentTraceViewModel GetDocumentTrace()
        {
            return Parent as DocumentTraceViewModel;
        }

        private int GetSequenceReference()
        {
            return GetDocumentTrace().Sequence == 1 ? 2 : 1;
        }

        public string Title { get; set; }
        public int PageIndex { get; set; }
        public string Pagination { get; set; }
        public string FormattedTitle => $"{Pagination} - Index: {PageIndex} - {Title}";
        public string FormattedTexte => EstPageAbsente ? $"  >> Page absente du Document {GetSequenceReference()}" : $"  Textes qui diffèrent du Document {GetSequenceReference()} :";
        public bool EstPageAbsente { get; set; }
        public bool EstPasPageAbsente => !EstPageAbsente;
        public ObservableCollection<TexteViewModel> Textes { get; set; }
    }
}