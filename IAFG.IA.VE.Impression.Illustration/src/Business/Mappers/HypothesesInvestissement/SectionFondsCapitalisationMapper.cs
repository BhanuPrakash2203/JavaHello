using IAFG.IA.VE.Impression.Illustration.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.HypothesesInvestissement;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.HypothesesInvestissement
{
    public class SectionFondsCapitalisationMapper : ReportMapperBase<SectionFondsCapitalisationModel, FondsCapitalisationViewModel>, ISectionFondsCapitalisationMapper
    {
        public SectionFondsCapitalisationMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
        {
        }

        public class ReportProfile : ReportProfileBase
        {
            public ReportProfile() : this(new IllustrationReportDataFormatter(null, null, null, null, null, null, null, null, null, null, null, null, null, null), null, null)
            {

            }

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
                CreateMap<SectionFondsCapitalisationModel, FondsCapitalisationViewModel>().
                    ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection)).
                    ForMember(d => d.BoniFidelite, m => m.MapFrom(s =>  formatter.FormatterBoniFidelite(s.BoniFidelite, s.DebutBoniFidelite))).
                    ForMember(d => d.BoniInteret, m => m.MapFrom(s => formatter.FormatterBoniInteret(s.ChoixBoniInteret, s.BoniInteret, s.TauxBoni, s.DebutBoniInteret))).
                    ForMember(d => d.RendementMoyenCompte, m => m.MapFrom(s => s.RendementMoyenCompte.HasValue ? formatter.FormatPercentageWithoutSymbol(s.RendementMoyenCompte.Value) : string.Empty)).
                    ForMember(d => d.Avis, m => m.MapFrom(s => s.Avis)).
                    ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)));

                CreateMap<DetailCompte, DetailFondsCapitalisationViewModel>().
                    ForMember(d => d.Vehicule, m => m.MapFrom(s => s.Vehicule)).
                    ForMember(d => d.Description, m => m.MapFrom(s => s.Description)).
                    ForMember(d => d.OrdreTri, m => m.MapFrom(s => s.OrdreTri)).
                    ForMember(d => d.Taux, m => m.MapFrom(s => s.Taux.HasValue ? formatter.FormatPercentageWithoutSymbol(s.Taux.Value) : string.Empty)).
                    ForMember(d => d.Periode, m => m.MapFrom(s => s.Taux.HasValue ? formatter.FormatterPeriodeAnneeMois(s.AnneeDebut, s.MoisDebut, true) : string.Empty)).
                    ForMember(d => d.RepartitionInvestissement, m => m.MapFrom(s => s.RepartitionInvestissement.HasValue ?  formatter.FormatPercentageWithoutSymbol(s.RepartitionInvestissement.Value) : string.Empty)).
                    ForMember(d => d.RepartitionDeduction, m => m.MapFrom(s => s.RepartitionDeduction.HasValue ?  formatter.FormatPercentageWithoutSymbol(s.RepartitionDeduction.Value) : string.Empty)).
                    ForMember(d => d.RendementMoyen, m => m.MapFrom(s => s.RendementMoyen.HasValue ? formatter.FormatPercentageWithoutSymbol(s.RendementMoyen.Value) : string.Empty)).
                    ForMember(d => d.EstSoldeTotal, m => m.MapFrom(s => s.EstSoldeTotal)).
                    ForMember(d => d.Solde, m => m.MapFrom(s => formatter.FormatDecimal(s.Solde)));
            }
        }
    }
}