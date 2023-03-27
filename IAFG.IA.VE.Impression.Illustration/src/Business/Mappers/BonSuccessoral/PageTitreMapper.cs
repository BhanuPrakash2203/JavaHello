using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.BonSuccessoral
{
    public class PageTitreMapper : ReportMapperBase<TitreRapportModel, PageTitreViewModel>, IPageTitreMapper
    {
        public PageTitreMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<TitreRapportModel, PageTitreViewModel>()
                    .ForMember(d => d.TitreRapport, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.TitreConcept, m => m.MapFrom(s => s.Libelles.FirstOrDefault(x => x.Key == "Concept.Titre").Value))
                    .ForMember(d => d.LogoId, m => m.MapFrom(s => "IA_GroupeFinancier"))
                    .ForMember(d => d.PrepareePour, m => m.MapFrom(s => s.Clients.Where(c => c.EstContractant).Select(c => formatter.FormatFullName(c.Prenom, c.Nom, c.Initiale))))
                    .ForMember(d => d.DatePreparation, m => m.MapFrom(s => formatter.FormatLongDate(s.DatePreparation, true, false)))
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                    .ForMember(d => d.Images, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperImages(s.Images)))
                    .ForMember(d => d.Avis, m => m.MapFrom(s => s.Avis))
                    .ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)));
            }
        }
    }
}
