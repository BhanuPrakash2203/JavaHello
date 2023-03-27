using System;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
{
    public class SectionDetailParticipationsMapper : ReportMapperBase<SectionDetailParticipationsModel, DetailParticipationsViewModel>, ISectionDetailParticipationsMapper
    {
        public SectionDetailParticipationsMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<SectionDetailParticipationsModel, DetailParticipationsViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.OptionParticipation, m => m.MapFrom(s => FormatterOptionParticipation(s.OptionParticipation, formatter, resourcesAccessor)))
                    .ForMember(d => d.SoldeParticipationsEnDepot, m => m.MapFrom(s => FormatterSoldeParticipationsEnDepot(s.SoldeParticipationsEnDepot, formatter, resourcesAccessor)));
            }

            private static string FormatterOptionParticipation(TypeOptionParticipation optionParticipation,
                IIllustrationReportDataFormatter formatter, IResourcesAccessorFactory resourcesAccessor)
            {
                var label =
                    $"{resourcesAccessor.GetResourcesAccessor().GetStringResourceById("OptionVersementParticipations")}{formatter.AddColon()}";
                var value = formatter.FormatterEnum<TypeOptionParticipation>(optionParticipation.ToString());
                return $"{label} {value}";
            }

            private static string FormatterSoldeParticipationsEnDepot(double? soldeParticipationsEnDepot,
                IIllustrationReportDataFormatter formatter, IResourcesAccessorFactory resourcesAccessor)
            {
                if (!soldeParticipationsEnDepot.HasValue || soldeParticipationsEnDepot.GetValueOrDefault() == 0)
                {
                    return string.Empty;
                }

                var label = $"{resourcesAccessor.GetResourcesAccessor().GetStringResourceById("InfoSoldeParticipationsEnDepot")}";
                var value = formatter.FormatCurrency(Math.Abs(soldeParticipationsEnDepot.GetValueOrDefault()));
                return string.Format(label, value);
            }

        }
    }
}