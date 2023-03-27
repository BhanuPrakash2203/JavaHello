using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.HypothesesInvestissement;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.HypothesesInvestissement
{
    public class SectionPretsMapper : ReportMapperBase<SectionPretsModel, PretsViewModel>, ISectionPretsMapper
    {
        public SectionPretsMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<SectionPretsModel, PretsViewModel>().
                    ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection)).
                    ForMember(d => d.Avis, m => m.MapFrom(s => s.Avis)).
                    ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)));

                CreateMap<DetailPret, DetailPretViewModel>().
                    ForMember(d => d.Pret, m => m.MapFrom(s => formatter.FormatterEnum<TypePret>(s.Pret.ToString()))).
                    ForMember(d => d.Solde, m => m.MapFrom(s => formatter.FormatDecimal(s.Solde)));
            }
        }
    }
}