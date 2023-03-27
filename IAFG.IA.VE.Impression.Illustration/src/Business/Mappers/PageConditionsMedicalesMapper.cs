using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.ConditionsMedicales;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.ConditionsMedicales;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    public class PageConditionsMedicalesMapper :
        ReportMapperBase<SectionConditionsMedicalesModel, PageConditionsMedicalesViewModel>,
        IPageConditionsMedicalesMapper
    {
        public PageConditionsMedicalesMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
        {
        }

        public class ReportProfile : ReportProfileBase
        {
            public ReportProfile()
                : this(new IllustrationReportDataFormatter(null, null, null, null, null, null, null, null, null, null, null, null, null, null), null, null)
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
                CreateMap<SectionConditionsMedicalesModel, PageConditionsMedicalesViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                    .ForMember(d => d.Avis, m => m.MapFrom(s => s.Avis))
                    .ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)))
                    .ForMember(d => d.Images, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperImages(s.Images)))
                    .ForMember(d => d.Sections, m => m.MapFrom(s => s.Sections));

                CreateMap<ConditionsMedicalesSection, ConditionsMedicalesSectionViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                    .ForMember(d => d.Avis, m => m.MapFrom(s => s.Avis))
                    .ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)))
                    .ForMember(d => d.Details, m => m.MapFrom(s => s.Details));

                CreateMap<ConditionMedicale, ConditionMedicaleViewModel>()
                    .ForMember(d => d.Titre, m => m.MapFrom(s => s.Titre))
                    .ForMember(d => d.SequenceId, m => m.MapFrom(s => s.SequenceId))
                    .ForMember(d => d.Libelle, m => m.MapFrom(s => s.Libelle))
                    .ForMember(d => d.Texte,
                        m => m.MapFrom(s => !s.Texte.StartsWith("<html>", true, null) ? s.Texte : string.Empty))
                    .ForMember(d => d.Html,
                        m => m.MapFrom(s =>
                            s.Texte.StartsWith("<html>", true, null)
                                ? s.Texte.Replace("<html>", @"<html><font face=""Calibri"" size=""2pt"">")
                                    .Replace("</html>", "</font></html>")
                                : string.Empty))
                    .ForMember(d => d.Textes, m => m.MapFrom(s => s.Textes))
                    .ForMember(d => d.Tableau, m => m.MapFrom(s => s.Tableau));

                CreateMap<TexteItem, TexteItemViewModel>()
                    .ForMember(d => d.Texte,
                        m => m.MapFrom(s =>
                            s.Texte.Replace("<html>", @"<html><font face=""Calibri"" size=""2pt"">")
                                .Replace("</html>", "</font></html>")));

                CreateMap<TableauItem, TableauItemViewModel>()
                    .ForMember(d => d.Texte,
                        m => m.MapFrom(s =>
                            s.Texte.Replace("<html>", @"<html><font face=""Calibri"" size=""2pt"">")
                                .Replace("</html>", "</font></html>")));
            }
        }
    }
}