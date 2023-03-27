using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    public class PageSignatureMapper : ReportMapperBase<SectionSignatureModel, PageSignatureViewModel>, IPageSignatureMapper
    {
        public PageSignatureMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<SectionSignatureModel, PageSignatureViewModel>().
                    ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection)).
                    ForMember(d => d.Description, m => m.MapFrom(s => s.Description)).
                    ForMember(d => d.Images, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperImages(s.Images))).
                    ForMember(d => d.Avis, m => m.MapFrom(s => s.Avis)).
                    ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes))).
                    ForMember(d => d.Signatures, m => m.MapFrom(s => s.Signatures)).
                    ForMember(d => d.EstNouveauContrat, m => m.MapFrom(s => s.EstNouveauContrat)).
                    ForMember(d => d.NumeroContrat, m => m.MapFrom(s => s.NumeroContrat));

                CreateMap<DetailTexte, LigneTexte>().
                    ForMember(d => d.Texte, m => m.MapFrom(s => s.Texte.Replace("<html>", @"<html><font face=""Calibri"" size=""2pt"">").Replace("</html>", "</font></html>"))).
                    ForMember(d => d.SautDeLigneApres, m => m.MapFrom(s => s.SautDeLigneApres)).
                    ForMember(d => d.SautDeLigneAvant, m => m.MapFrom(s => s.SautDeLigneAvant)).
                    ForMember(d => d.SequenceId, m => m.MapFrom(s => s.SequenceId));
            }
        }
    }
}