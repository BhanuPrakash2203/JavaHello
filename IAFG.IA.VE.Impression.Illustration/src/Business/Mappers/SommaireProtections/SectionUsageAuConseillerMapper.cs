using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using MaximumOdsPermisViewModel = IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections.MaximumOdsPermisViewModel;
using MontantNetAuRisqueViewModel = IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections.MontantNetAuRisqueViewModel;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
{
    public class SectionUsageAuConseillerMapper: ReportMapperBase<SectionUsageAuConseillerModel, UsageAuConseillerViewModel>, ISectionUsageAuConseillerMapper
    {
        public SectionUsageAuConseillerMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<SectionUsageAuConseillerModel, UsageAuConseillerViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)))
                    .ForMember(d => d.MontantNetAuRisqueViewModel, m => m.MapFrom(s => s.MontantNetAuRisque))
                    .ForMember(d => d.MaximumOdsPermisViewModel, m => m.MapFrom(s => s.MaximumOdsPermis));

                CreateMap<MontantNetAuRisqueModel, MontantNetAuRisqueViewModel>()
                    .ForMember(d => d.Annee, m => m.ResolveUsing(s => s.Annee))
                    .ForMember(d => d.Montant, m => m.ResolveUsing(s => formatter.FormatCurrency(s.Montant)));

                CreateMap<MaximumOdsPermisModel, MaximumOdsPermisViewModel>()
                    .ForMember(d => d.Montant, m => m.ResolveUsing(s => formatter.FormatCurrency(s.Montant)));

            }
        }
    }
}
