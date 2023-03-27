using IAFG.IA.VE.Impression.Illustration.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.HypothesesInvestissement;
using DetailTauxAVMCompte = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.HypothesesInvestissement.DetailTauxAVMCompte;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.HypothesesInvestissement
{
    public class SectionAjustementValeurMarchandeMapper : ReportMapperBase<SectionAjustementValeurMarchandeModel, AjustementValeurMarchandeViewModel>, ISectionAjustementValeurMarchandeMapper
    {
        public SectionAjustementValeurMarchandeMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<SectionAjustementValeurMarchandeModel, AjustementValeurMarchandeViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Description, m => m.MapFrom(s => s.Description));
                    
                CreateMap<DetailTexte, LigneTexte>().
                    ForMember(d => d.Texte, m => m.MapFrom(s => s.Texte.Replace("<html>", @"<html><font face=""Calibri"" size=""2pt"">").Replace("</html>", "</font></html>"))).
                    ForMember(d => d.SautDeLigneApres, m => m.MapFrom(s => s.SautDeLigneApres)).
                    ForMember(d => d.SautDeLigneAvant, m => m.MapFrom(s => s.SautDeLigneAvant)).
                    ForMember(d => d.SequenceId, m => m.MapFrom(s => s.SequenceId));

                CreateMap<DetailTauxAVMCompte, DetailTauxAVMCompteViewModel>().
                    ForMember(d => d.Vehicule, m => m.MapFrom(s => s.Vehicule)).
                    ForMember(d => d.Description, m => m.MapFrom(s => s.Description)).
                    ForMember(d => d.Taux, m => m.MapFrom(s => s.Taux.HasValue ? formatter.FormatPercentageWithoutSymbol(s.Taux.Value) : string.Empty)).
                    ForMember(d => d.Periode, m => m.MapFrom(s => s.Taux.HasValue ? formatter.FormatterPeriodeAnneeMois(s.AnneeDebut, s.MoisDebut, true) : string.Empty));
            }
        }
    }
}
