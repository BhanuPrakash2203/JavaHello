using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
{
    public class SectionPrimesMapper : ReportMapperBase<SectionPrimesModel, ProtectionPrimesViewModel>, ISectionPrimesMapper
    {
        public SectionPrimesMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<SectionPrimesModel, ProtectionPrimesViewModel>().
                    ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection)).
                    ForMember(d =>d.TitreColonnePrimesVersees, m =>m.MapFrom(s => resourcesAccessor.GetResourcesAccessor().GetStringResourceById(s.TitreColonnePrimesVersees))).
                    ForMember(d => d.FrequenceFacturation, m => m.MapFrom(s => s.FrequenceFacturation)).
                    ForMember(d => d.Avis, m => m.MapFrom(s => s.Avis)).
                    ForMember(d => d.Notes, m => m.ResolveUsing(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)));

                CreateMap<DetailPrime, PrimeViewModel>().
                    ForMember(d => d.Description, m => m.ResolveUsing(s => s.FormatterDescription(formatter))).
                    ForMember(d => d.MontantAvecTaxe, m => m.ResolveUsing(s => s.FormatterMontantAvecTaxe(formatter))).
                    ForMember(d => d.Montant, m => m.MapFrom(s => formatter.FormatCurrency(s.Montant)));

                CreateMap<DetailPrimeVersee, PrimeVerseeViewModel>().
                    ForMember(d => d.Description, m => m.ResolveUsing(s => s.FormatterDescription(formatter, resourcesAccessor))).
                    ForMember(d => d.Periode, m => m.MapFrom(s => s.FormatterPeriode(formatter))).
                    ForMember(d => d.Montant, m => m.ResolveUsing(s => s.FormatterMontant(formatter, resourcesAccessor))).
                    ForMember(d => d.FrequenceFacturation, m => m.MapFrom(s => s.FormatterFrequenceFacturation(resourcesAccessor)));
            }
        }
    }

    public static class DetailPrimeMapperExtension
    {
        public static string FormatterDescription(this DetailPrime source, IIllustrationReportDataFormatter formatter)
        {
            return formatter.FormatterEnum<TypeDetailPrime>(source.TypeDetailPrime.ToString());
        }

        public static string FormatterMontantAvecTaxe(this DetailPrime source, IIllustrationReportDataFormatter formatter)
        {
            return formatter.FormatCurrency(source.MontantAvecTaxe);
        }
    }
}
