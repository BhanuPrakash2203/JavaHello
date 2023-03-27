using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.ReportContext;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Export;
using IAFG.IA.VE.Impression.Core.Types.Reports.MasterReports;
using IAFG.IA.VE.Impression.Illustration.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.RelevantBuilder;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Services;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;

namespace IAFG.IA.VE.Impression.Illustration.Services
{
    public class IllustrationsExportService : IIllustrationsExportService
    {
        // ReSharper disable once InconsistentNaming
        private const string EN_CA_CULTURE = "en-CA";
        // ReSharper disable once InconsistentNaming
        private const string FR_CA_CULTURE = "fr-CA";
        private readonly ICultureAccessor _accessorCulture;
        private readonly IIllustrationMasterReportBuilder _reportBuilder;
        private readonly IIllustrationModelMapper _modelMapper;
        private readonly IPageRelevancyAnalyzer _pageRelevancyAnalyzer;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IIllustrationResourcesAccessorFactory _resources;

        public IllustrationsExportService(ICultureAccessor accessorCulture,
                                          IIllustrationModelMapper modelMapper,
                                          IIllustrationMasterReportBuilder reportBuilder,
                                          IPageRelevancyAnalyzer pageRelevancyAnalyzer, 
                                          IConfigurationRepository configurationRepository,
                                          IIllustrationResourcesAccessorFactory resources)
        {
            _accessorCulture = accessorCulture;
            _reportBuilder = reportBuilder;
            _pageRelevancyAnalyzer = pageRelevancyAnalyzer;
            _configurationRepository = configurationRepository;
            _modelMapper = modelMapper;
            _resources = resources;
            _resources.Contexte = ResourcesContexte.Illustration;
        }

        public PdfDocument ExportToPDF(DonneesIllustration illustration) => ExportToPDF(illustration,
            new ExportOptions {Audience = ReportAudienceTypes.FullClearance});

        public PdfDocument ExportToPDF(DonneesIllustration illustration, ExportOptions options)
        {
            _resources.Contexte = ResourcesContexte.Illustration;
            var reportContext = CreateReportContext(illustration, options.Audience);
            SetCulture(reportContext);

            var donnees = _modelMapper.Map(illustration);
            var reports = _reportBuilder.Build(donnees, reportContext);
            var pdf = CreatePdf(reports.FirstOrDefault(), reports.Skip(1).ToArray());
            foreach (var report in reports) 
            {
                report.DisposeReport();
            }
            return pdf;
        }

        public IList<PageRapport> ObtenirInformationPagesRapport(DonneesIllustration illustration) =>
            ObtenirInformationPagesRapport(illustration,
                new ExportOptions {Audience = ReportAudienceTypes.FullClearance});

        public IList<PageRapport> ObtenirInformationPagesRapport(DonneesIllustration illustration,
            ExportOptions options)
        {
            _resources.Contexte = ResourcesContexte.Illustration;
            var reportContext = CreateReportContext(illustration, options.Audience);
            SetCulture(reportContext);

            var model = _modelMapper.Map(illustration);
            var pages = _pageRelevancyAnalyzer.GetRelevantPages(model);
            return CreerListePages(model.Produit, pages);
        }

        public IList<PageRapport> ObtenirInformationPagesRapport(DonneesRapport donneesRapport)
        {
            var configuration =
                _configurationRepository.ObtenirConfigurationRapport(donneesRapport.Produit, donneesRapport.Etat);
            return CreerListePages(donneesRapport.Produit, configuration.Pages);
        }

        private IList<PageRapport> CreerListePages(Produit produit, IEnumerable<PageRapportInfo> pages)
        {
            return (from page in pages
                let section = _configurationRepository.ObtenirConfigurationSection(page.SectionId, null)
                where section != null
                select new PageRapport
                {
                    Id = page.SectionId,
                    Titre = section.ObtenirTitre(produit),
                    EstDefaut = page.EstDefaut,
                    EstPageTitre = section.BuilderName == "Principal.PageTitre",
                    OrdreAffichage = page.OrdreAffichage,
                    ExigeCalculRendement = section.ExigeCalculRendement,
                    EstAccap = page.EstAccap,
                    GroupeRapport = section.GroupeRapport,
                    SectionAffichage = section.SectionAffichage,
                    Regles = section.Regles
                }).OrderBy(p => p.OrdreAffichage).ToList();
        }

        private static ReportContext CreateReportContext(DonneesIllustration illustration,
            ReportAudienceTypes reportAudience)
        {
            return new ReportContext
            {
                ReportAudience = reportAudience,
                Language = illustration.ParametreRapport.Language,
                Mode = ApplicationMode.Agent
            };
        }

        private void SetCulture(IReportContext reportContext)
        {
            _accessorCulture.SetCultureInfo(
                reportContext.Language == Language.French
                    ? new CultureInfo(FR_CA_CULTURE, false)
                    : new CultureInfo(EN_CA_CULTURE, false),
                _resources);

            _configurationRepository.Language = reportContext.Language;
        }

        private static ApplicationPdfDocument CreatePdf(IMasterReport masterReport,
            IMasterReport[] reportsToAppend)
        {
            byte[] result = null;
            if (masterReport != null)
            {
                result = masterReport.ToPdf(IsoPdfVersion.Unknown, reportsToAppend);
            }

            return new ApplicationPdfDocument
            {
                Content = result,
                PdfVersion = IsoPdfVersion.Unknown
            };
        }
        
    }
}