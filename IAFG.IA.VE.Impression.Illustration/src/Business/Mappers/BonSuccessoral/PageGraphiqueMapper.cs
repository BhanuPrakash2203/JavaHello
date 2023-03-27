using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.BonSuccessoral
{
    public class PageGraphiqueMapper : ReportMapperBase<PageGraphiqueModel, PageGraphiqueViewModel>, IPageGraphiqueMapper
    {
        public PageGraphiqueMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<PageGraphiqueModel, PageGraphiqueViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                    .ForMember(d => d.Images, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperImages(s.Images)))
                    .ForMember(d => d.TitreGraphique, m => m.MapFrom(s => s.TitreGraphique))
                    .ForMember(d => d.Annees, m => m.MapFrom(s => s.Annees))
                    .ForMember(d => d.Ages, m => m.MapFrom(s => s.Ages))
                    .ForMember(d => d.LibelleAnnees, m => m.MapFrom(s => s.LibelleAnnees))
                    .ForMember(d => d.LibelleAge, m => m.MapFrom(s => s.LibelleAge))
                    .ForMember(d => d.LibelleValeur, m => m.MapFrom(s => s.LibelleValeur))
                    .ForMember(d => d.Valeurs, m => m.MapFrom(s => s.Valeurs))
                    .ForMember(d => d.Legendes, m => m.MapFrom(s => s.Legendes))
                    .ForMember(d => d.Avis, m => m.MapFrom(s => s.Avis))
                    .ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)));
            }
        }
    }
}
