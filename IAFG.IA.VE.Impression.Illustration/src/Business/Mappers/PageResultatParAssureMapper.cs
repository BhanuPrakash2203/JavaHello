using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    public class PageResultatParAssureMapper : ReportMapperBase<SectionResultatParAssureModel, PageResultatViewModel>, IPageResultatParAssureMapper
    {
        public PageResultatParAssureMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<SectionResultatParAssureModel, PageResultatViewModel>().
                    ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection)).
                    ForMember(d => d.Description, m => m.MapFrom(s => s.Description)).
                    ForMember(d => d.Avis, m => m.MapFrom(s => s.Avis)).
                    ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes))).
                    ForMember(d => d.Tableaux, m => m.MapFrom(s => managerFactory.GetTableauResultatManager().MapperTableaux(s)));
            }
        }
    }
}