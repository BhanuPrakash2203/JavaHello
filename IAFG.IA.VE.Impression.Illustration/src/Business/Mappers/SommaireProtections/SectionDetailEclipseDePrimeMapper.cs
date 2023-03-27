using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using System.Collections.Generic;
using System.Linq;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
{
    public class SectionDetailEclipseDePrimeMapper : ReportMapperBase<SectionDetailEclipseDePrimeModel, DetailEclipseDePrimeViewModel>, ISectionDetailEclipseDePrimeMapper
    {
        public SectionDetailEclipseDePrimeMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<SectionDetailEclipseDePrimeModel, DetailEclipseDePrimeViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                    .ForMember(d => d.DescriptionAnneeActivation, m => m.MapFrom(s => $"{resourcesAccessor.GetResourcesAccessor().GetStringResourceById("EclipseDePrimeActivation")}{formatter.AddColon()}"))
                    .ForMember(d => d.Baremes, m => m.MapFrom(s => FormatterBaremes(s, formatter, resourcesAccessor)))
                    .ForMember(d => d.Notes, m=>m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)));
            }
        }

        private static IList<string[]> FormatterBaremes(SectionDetailEclipseDePrimeModel source, IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor)
        {
            var results = new List<string[]>();

            if (source.Baremes == null)
            {
                return results;
            }

            results.AddRange(source
                .Baremes
                .Select(b => new List<string>
                {
                    FormatterDiminutionBareme(b, resourcesAccessor),
                    formatter.FormatterPeriodeAnnees(b.Annee, null)
                }.ToArray()));

            return results;
        }

        private static string FormatterDiminutionBareme(Bareme bareme, IIllustrationResourcesAccessorFactory resourcesAccessor)
        {
            var resourceId = bareme.Diminution.HasValue ? "BaremeAlternatif" : "BaremeCourant";
            return resourcesAccessor.GetResourcesAccessor().GetStringResourceById(resourceId);
        }
    }
}
