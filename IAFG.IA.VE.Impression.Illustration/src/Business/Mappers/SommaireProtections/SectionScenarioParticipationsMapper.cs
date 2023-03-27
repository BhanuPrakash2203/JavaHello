using System;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
{
    public class SectionScenarioParticipationsMapper : ReportMapperBase<SectionScenarioParticipationsModel, ScenarioParticipationsViewModel>, ISectionScenarioParticipationsMapper
    {
        public SectionScenarioParticipationsMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<SectionScenarioParticipationsModel, ScenarioParticipationsViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                    .ForMember(d => d.BaremesParticipations, m => m.MapFrom(s => FormatterBaremeParticipation(s.EcartBaremeParticipation, formatter, resourcesAccessor)));
            }

            private static string FormatterBaremeParticipation(double? ecartBaremeParticipation,
                IIllustrationReportDataFormatter formatter, IResourcesAccessorFactory resourcesAccessor)
            {
                if (!ecartBaremeParticipation.HasValue)
                {
                    return string.Empty;
                }

                var label =
                    $"{resourcesAccessor.GetResourcesAccessor().GetStringResourceById("BaremeAlternatif")}{formatter.AddColon()}";
                var value = string.Format(
                    resourcesAccessor.GetResourcesAccessor().GetStringResourceById("BaremeParticipationCourantMoins"),
                    formatter.FormatPercentage(Math.Abs(ecartBaremeParticipation.GetValueOrDefault())));
                return $"{label} {value}";
            }
        }
    }
}
