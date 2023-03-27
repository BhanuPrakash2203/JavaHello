using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.ModificationsDemandees;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.ModificationsDemandees
{
    public class SectionContratMapper : ReportMapperBase<SectionContratModel, ContratViewModel>, ISectionContratMapper
    {
        public SectionContratMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<SectionContratModel, ContratViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Avis, m => m.MapFrom(s => s.Avis))
                    .ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)))
                    .ForMember(d => d.Modifications, m => m.MapFrom(s => s.Transactions.MapperTransactions(resourcesAccessor, formatter)));
            }
        }
    }
}