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
    public class SectionAvancesSurPoliceMapper : ReportMapperBase<SectionAvancesSurPoliceModel, AvancesSurPoliceViewModel>, ISectionAvancesSurPoliceMapper
    {
        public SectionAvancesSurPoliceMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<SectionAvancesSurPoliceModel, AvancesSurPoliceViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.SoldeEnDateDu, m => m.MapFrom(s => FormatterDateDerniereMiseAJour(s.DateDerniereMiseAJour, s.Solde, formatter, resourcesAccessor)));
            }

            private static string FormatterDateDerniereMiseAJour(DateTime dateDerniereMiseAJour, 
                                                                 double solde,
                                                                 IIllustrationReportDataFormatter formatter, 
                                                                 IResourcesAccessorFactory resourcesAccessor)
            {
                var label =
                    $"{resourcesAccessor.GetResourcesAccessor().GetStringResourceById("SoldeAvancesSurPoliceEnDateDu")}";
                var valueDate = formatter.FormatLongDate(dateDerniereMiseAJour);
                var valueSolde = formatter.FormatCurrency(solde);
                return string.Format(label, valueDate, valueSolde);
            }
        }
    }
}
