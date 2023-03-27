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
    public class SectionASLMapper : ReportMapperBase<SectionASLModel, ASLViewModel>, ISectionASLMapper
    {
        public SectionASLMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<SectionASLModel, ASLViewModel>().
                    ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection)).
                    ForMember(d => d.OptionVersementBoni, m => m.MapFrom(s => formatter.FormatterEnum<TypeOptionVersementBoni>(s.OptionVersementBoni.ToString()))).
                    ForMember(d => d.CapitalAssuraMaximal, m => m.MapFrom(s => FormatterCapitalAssure(s.AucunAchat, s.AucunMaximum, s.CapitalAssureMaximal, formatter, resourcesAccessor))).
                    ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)));

                CreateMap<DetailTaux, TauxASLViewModel>().
                    ForMember(d => d.AnneeDebut, m => m.MapFrom(s => s.AnneeDebut)).
                    ForMember(d => d.Periode, m => m.MapFrom(s =>  formatter.FormatterPeriodeAnneeMois(s.AnneeDebut, null))).
                    ForMember(d => d.Taux, m => m.MapFrom(s => formatter.FormatPercentageWithoutSymbol(s.Taux)));

                CreateMap<DetailAllocationASL, AllocationASLViewModel>().
                    ForMember(d => d.AnneeDebut, m => m.MapFrom(s => s.AnneeDebut)).
                    ForMember(d => d.Periode, m => m.MapFrom(s => formatter.FormatterPeriodeAnneeMois(s.AnneeDebut, null))).
                    ForMember(d => d.Montant, m => m.MapFrom(s => formatter.FormatDecimal(s.Montant)));
            }

            private static string FormatterCapitalAssure(bool aucunAchat, bool aucunMaximum, double capitalAssureMaximal, IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor)
            {
                if (aucunAchat) return resourcesAccessor.GetResourcesAccessor().GetStringResourceById("AucunAchatASL");
                return aucunMaximum ? resourcesAccessor.GetResourcesAccessor().GetStringResourceById("AucunMaximum") : formatter.FormatCurrency(capitalAssureMaximal);
            }
        }
    }
}