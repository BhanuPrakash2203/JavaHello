using System;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
{
    public class SectionSurprimeMapper : ReportMapperBase<SectionSurprimesModel, SurprimesViewModel>, ISectionSurprimeMapper
    {
        public SectionSurprimeMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<SectionSurprimesModel, SurprimesViewModel>().
                    ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection)).
                ForMember(d => d.FrequenceFacturation, m => m.MapFrom(s => s.FrequenceFacturation));

                CreateMap<DetailProtection, ProtectionSurprimeViewModel>().
                    ForMember(d => d.NomsAssures, m => m.MapFrom(s => s.Noms.JoinStringLines())).
                    ForMember(d => d.TauxTotal, m => m.MapFrom(s => formatter.FormatCurrency(s.SurprimeTotal))).
                    ForMember(d => d.NomProtection, m => m.MapFrom(s => $"{s.Description} - {formatter.FormatCurrency(s.MontantCapitalAssureActuel)}"));

                CreateMap<DetailSurprime, SurprimeDetailViewModel>().
                    ForMember(d => d.NomConjoint, m => m.MapFrom(s => s.Description)).
                    ForMember(d => d.Description, m => m.MapFrom(s => FormatterDescription(formatter, resourcesAccessor, s.TauxPourcentage, s.TauxMontant, s.DateLiberation, s.EstTypeTemporaire)));
            }

            private static string FormatterDescription(IIllustrationReportDataFormatter formatter,
                                                       IIllustrationResourcesAccessorFactory resourcesAccessor,
                                                       double? tauxPourcentage,
                                                       double? tauxMontant, 
                                                       DateTime? dateLiberation,
                                                       bool estTemporaire)
            {
                var typeSurprime = resourcesAccessor.GetResourcesAccessor().GetStringResourceById(estTemporaire ? "SurprimeTemporaire" : "SurprimePermanente");
                return string.Format($"{typeSurprime} ", FormatterTaux(formatter, resourcesAccessor, tauxPourcentage, tauxMontant)) + " - " + FormatterSurprime(dateLiberation,formatter);
            }

            private static string FormatterTaux(IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor, double? tauxPourcentage, double? tauxMontant)
            {
                var formattedPercentPerm = "";
                if (tauxPourcentage.HasValue) formattedPercentPerm = $"+ { formatter.FormatPercentage(tauxPourcentage.Value)}";

                var formattedMontant = "";
                if (tauxMontant.HasValue) formattedMontant = $"{formatter.FormatCurrency(tauxMontant)}{resourcesAccessor.GetResourcesAccessor().GetStringResourceById("SurprimeParMille")}";
                var separateur = !string.IsNullOrWhiteSpace(formattedPercentPerm) && !string.IsNullOrWhiteSpace(formattedMontant) ? "  " : "";
                return $"{formattedPercentPerm}{separateur}{formattedMontant}";
            }

            private static string FormatterSurprime(DateTime? date, IIllustrationReportDataFormatter formatter)
            {
                return formatter.FormatterDuree(TypeDuree.DateTerminaison, formatter.FormatLongDate(date));
            }
        }
    }
}
