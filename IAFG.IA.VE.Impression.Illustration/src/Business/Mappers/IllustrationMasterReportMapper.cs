using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    public class IllustrationMasterReportMapper : ReportMapperBase<DonneesRapportIllustration, IllustrationMasterReportViewModel>, IIllustrationMasterReportMapper
    {
        public IllustrationMasterReportMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
        {
        }

        public class ReportProfile : ReportProfileBase
        {

            public ReportProfile(
                IIllustrationReportDataFormatter formatter, 
                IIllustrationResourcesAccessorFactory resourcesAccessor, 
                IManagerFactory managerFactory) : base(formatter, resourcesAccessor, managerFactory)
            {
            }

            protected override void ConfigureMapping(
                IIllustrationReportDataFormatter formatter, 
                IIllustrationResourcesAccessorFactory resourcesAccessor, 
                IManagerFactory managerFactory)
            {
                CreateMap<DonneesRapportIllustration, IllustrationMasterReportViewModel>().
                    ForMember(d => d.TitreRapport, m => m.MapFrom(s => s.TitreRapport)).
                    ForMember(d => d.ProduitTrace, m => m.MapFrom(s => s.Produit)).
                    ForMember(d => d.Banniere, m => m.MapFrom(s => s.Banniere)).
                    ForMember(d => d.InclurePageTitre, m => m.MapFrom(s => s.InclurePageTitre)).
                    ForMember(d => d.LogoId, m => m.MapFrom(s => DeterminerLogoBanniere(s.Banniere))).
                    ForMember(d => d.DateMiseAJour, m => m.MapFrom(s => s.Etat == Etat.EnVigueur && s.DateMiseAJour.HasValue ? formatter.FormatLongDate(s.DateMiseAJour.Value): string.Empty)).
                    ForMember(d => d.PrepareePour, m => m.MapFrom(s => s.Clients.Where(c => c.EstContractant).Select(c => formatter.FormatFullName(c.Prenom, c.Nom, c.Initiale)))).
                    ForMember(d => d.DatePreparation, m => m.MapFrom(s => formatter.FormatLongDate(s.DatePreparation, true, false))).
                    ForMember(d => d.DateImprimee, m => m.MapFrom(s => formatter.FormatCurrentLongDateTime())).
                    ForMember(d => d.NotePiedDePage, m => m.MapFrom(s => s.SectionsAccapManquantes ? resourcesAccessor.GetResourcesAccessor().GetStringResourceById("NotePiedPage2") : resourcesAccessor.GetResourcesAccessor().GetStringResourceById("NotePiedPage1"))).
                    ForMember(d => d.VersionProduit, m => m.MapFrom(s => s.VersionProduit)).
                    ForMember(d => d.VersionEVO, m => m.MapFrom(s => s.VersionEVO)).
                    ForMember(d => d.VersionProduitFormattee, m => m.MapFrom(s => s.VersionProduitFormattee)).
                    ForMember(d => d.NumeroContrat , m => m.MapFrom(s => s.NumeroContrat)).
                    ForMember(d => d.EstNouveauContrat, m => m.MapFrom(s => s.Etat == Etat.NouvelleVente));

                CreateMap<Agent, AgentViewModel>().
                    ForMember(d => d.Agence, m => m.MapFrom(s => s.Agence)).
                    ForMember(d => d.NomComplet, m => m.MapFrom(s => formatter.FormatFullName(s.Prenom, s.Nom, s.Initiale, s.Genre, string.Empty))).
                    ForMember(d => d.NomCompletAvecTitre, m => m.MapFrom(s => formatter.FormatFullName(s.Prenom, s.Nom, s.Initiale, s.Genre, s.Titre))).
                    ForMember(d => d.Courriel, m => m.MapFrom(s => s.Courriel)).
                    ForMember(d => d.Telecopieur, m => m.MapFrom(s => formatter.FormatPhoneNumber(s.Telecopieur))).
                    ForMember(d => d.TelephoneBureau, m => m.MapFrom(s => formatter.FormatPhoneNumber(s.TelephoneBureau))).
                    ForMember(d => d.TelephonePrincipal, m => m.MapFrom(s => formatter.FormatPhoneNumber(s.TelephonePrincipal)));
            }
            
            private static string DeterminerLogoBanniere(Banniere banniere)
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (banniere)
                {
                    case Banniere.IA:
                        return "IA_GroupeFinancier";
                    default:
                        return "IA_GroupeFinancier";
                }
            }
        }
    }
}