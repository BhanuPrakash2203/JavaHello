using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.NotesIllustration;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.NotesIllustration;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    public class PageNotesIllustrationMapper : ReportMapperBase<SectionNotesIllustrationModel, PageNotesIllustrationViewModel>, IPageNotesIllustrationMapper
    {
        public PageNotesIllustrationMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<SectionNotesIllustrationModel, PageNotesIllustrationViewModel>().
                    ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection)).
                    ForMember(d => d.Description, m => m.MapFrom(s => s.Description)).
                    ForMember(d => d.Images, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperImages(s.Images))).
                    ForMember(d => d.Avis, m => m.MapFrom(s => s.Avis)).
                    ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes))).
                    ForMember(d => d.SousSections, m => m.MapFrom(s => s.SousSections));
                
                CreateMap<NotesIllustration, DetailNotesIllustrationViewModel>()
                    .ForMember(d => d.Texte, m => m.MapFrom(s => string.Empty))
                    .ForMember(d => d.Libelle, m => m.MapFrom(s => s.Titre))
                    .ForMember(d => d.ListeDescriptions, m => m.MapFrom(s => s.Textes))
                    .ForMember(d => d.Html, m => m.MapFrom(s => string.Empty));

            }
        }
    }
}