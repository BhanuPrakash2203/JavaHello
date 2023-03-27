using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
{
    public class SectionFluxMonetaireMapper : ReportMapperBase<SectionFluxMonetaireModel, FluxMonetaireViewModel>, ISectionFluxMonetaireMapper
    {
        public SectionFluxMonetaireMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<SectionFluxMonetaireModel, FluxMonetaireViewModel>().
                    ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection));

                CreateMap<DetailFluxMonetaire, FluxMonetaireDetailViewModel>().
                    ForMember(d => d.TypeTransaction, m => m.MapFrom(s => s.TypeTransaction)).
                    ForMember(d => d.Description, m => m.MapFrom(s => s.FormatterDescription(formatter))).
                    ForMember(d => d.AnneeDebut, m => m.MapFrom(s => s.AnneeDebut)).
                    ForMember(d => d.Periode, m => m.MapFrom(s => s.FormatterPeriode(formatter))).
                    ForMember(d => d.Montant, m => m.MapFrom(s => s.FormatterMontant(formatter, resourcesAccessor)));
            }
        }
    }
}