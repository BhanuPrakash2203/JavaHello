using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.Glossaires;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    public class PageGlossaireMapper : ReportMapperBase<SectionGlossaireModel, PageGlossaireViewModel>, IPageGlossaireMapper
    {
        public PageGlossaireMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
        {
        }

        public class ReportProfile : ReportProfileBase
        {
            public ReportProfile(IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor, IManagerFactory managerFactory) : base(formatter, resourcesAccessor, managerFactory)
            {
            }

            protected override void ConfigureMapping(IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor, IManagerFactory managerFactory)
            {
                CreateMap<SectionGlossaireModel, PageGlossaireViewModel>().
                    ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection)).
                    ForMember(d => d.Description, m => m.MapFrom(s => s.Description)).
                    ForMember(d => d.Avis, m => m.MapFrom(s => s.Avis)).
                    ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes))).
                    ForMember(d => d.Images, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperImages(s.Images))).
                    ForMember(d => d.Details, m => m.MapFrom(s => s.Details));

                CreateMap<DetailGlossaire, DetailGlossaireViewModel>().
                    ForMember(d => d.CodeGlossaire, m => m.MapFrom(s => s.CodeGlossaire)).
                    ForMember(d => d.CodeSequenceId, m => m.MapFrom(s => s.CodeSequenceId)).
                    ForMember(d => d.Titre , m => m.MapFrom(s => s.Titre)).
                    ForMember(d => d.SequenceId, m => m.MapFrom(s => s.SequenceId)).
                    ForMember(d => d.Libelle, m => m.MapFrom(s => s.Libelle)).
                    ForMember(d => d.Texte, m => m.MapFrom(s => !s.Texte.StartsWith("<html>", true, null) ? s.Texte : string.Empty)).
                    ForMember(d => d.Html, m => m.MapFrom(s => s.Texte.StartsWith("<html>", true, null) ? s.Texte.Replace("<html>", @"<html><font face=""Calibri"" size=""2pt"">").Replace("</html>", "</font></html>") : string.Empty));
            }
        }
    }
}