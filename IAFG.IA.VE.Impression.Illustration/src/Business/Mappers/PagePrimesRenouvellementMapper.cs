using System;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.PrimesRenouvellement;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.PrimesRenouvellement;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    public class PagePrimesRenouvellementMapper : ReportMapperBase<PagePrimesRenouvellementModel, PagePrimesRenouvellementViewModel>, IPagePrimesRenouvellementMapper
    {
        public PagePrimesRenouvellementMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<PagePrimesRenouvellementModel, PagePrimesRenouvellementViewModel>().
                    ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection)).
                    ForMember(d => d.Avis, m => m.MapFrom(s => s.Avis)).
                    ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes))).
                    ForMember(d => d.Images, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperImages(s.Images))).
                    ForMember(d => d.SectionPrimesRenouvellementViewModels, m => m.MapFrom(s => s.SectionPrimesRenouvellementModels));

                CreateMap<SectionPrimeRenouvellementModel, SectionPrimeRenouvellementViewModel>().
                    ForMember(d => d.DetailsPrimeRenouvellement, m => m.MapFrom(s => s.DetailsPrimeRenouvellement));

                CreateMap<DetailsPrimeRenouvellementModel, DetailsPrimeRenouvellementViewModel>().
                    ForMember(d => d.Id, m => m.MapFrom(s => s.Id)).
                    ForMember(d => d.Assures, m => m.MapFrom(s => formatter.FormatterNomsAssures(s.Assures))).
                    ForMember(d => d.Description, m => m.MapFrom(s => s.Description)).
                    ForMember(d => d.CapitalAssure, m => m.MapFrom(s => formatter.FormatCurrency(s.CapitalAssure))).
                    ForMember(d => d.FrequenceFacturation, m => m.MapFrom(s => s.FrequenceFacturation)).
                    ForMember(d => d.EstProtectionContractant, m => m.MapFrom(s => s.EstProtectionContractant)).
                    ForMember(d => d.EstProtectionBase, m => m.MapFrom(s => s.EstProtectionBase)).
                    ForMember(d => d.EstProtectionBase, m => m.MapFrom(s => s.EstProtectionBase)).
                    ForMember(d => d.EstProtectionConjointe, m => m.MapFrom(s => s.EstProtectionConjointe));

                CreateMap<PeriodePrimeModel, PeriodePrimeViewModel>().
                    ForMember(d => d.Periode, m => m.MapFrom(s => FormatterPeriodes(s.AnneeDebut, s.AnneeFin, formatter))).
                    ForMember(d => d.PrimeGarantie, m => m.MapFrom(s => Math.Abs(s.PrimeGarantie) > 0 ? formatter.FormatCurrency(s.PrimeGarantie) : string.Empty));
            }

            private static string FormatterPeriodes(int anneeDebut, int? anneeFin, IIllustrationReportDataFormatter formatter)
            {
                var periode = formatter.FormatterPeriode(anneeDebut, anneeFin).PremiereLettreEnMajuscule();
                return periode;
            }
        }
    }
}