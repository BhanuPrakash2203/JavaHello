using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Reports;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.RelevantBuilder;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.MasterReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class IllustrationMasterReportBuilder : IIllustrationMasterReportBuilder
    {
        private readonly IIllustrationMasterReportMapper _mapper;
        private readonly IReportFactory _reportFactory;
        private readonly IBuilderRelevancyAnalyzer _relevancyAnalyzer;
        private readonly IPageRelevancyAnalyzer _pageRelevancyAnalyzer;
        private readonly IConfigurationRepository _configurationRepository;

        public IllustrationMasterReportBuilder(IIllustrationMasterReportMapper mapper,
                                               IReportFactory reportFactory,
                                               IBuilderRelevancyAnalyzer relevancyAnalyzer,
                                               IPageRelevancyAnalyzer pageRelevancyAnalyzer,
                                               IConfigurationRepository configurationRepository)
        {
            _mapper = mapper;
            _reportFactory = reportFactory;
            _relevancyAnalyzer = relevancyAnalyzer;
            _pageRelevancyAnalyzer = pageRelevancyAnalyzer;
            _configurationRepository = configurationRepository;
        }

        public IIllustrationMasterReport[] Build(DonneesRapportIllustration donnees, IReportContext reportContext)
        {
            var result = new List<IIllustrationMasterReport>();
            var configuration = _configurationRepository.ObtenirConfigurationRapport(donnees.Produit, donnees.Etat);

            foreach (GroupeRapport groupeRapport in System.Enum.GetValues(typeof(GroupeRapport))) 
            {
                donnees.GroupeRapport = groupeRapport;
                donnees.PagesRapport = _pageRelevancyAnalyzer.GetRelevantPages(donnees).ToList();
                if (donnees.PagesSelectionnees != null && donnees.PagesSelectionnees.Any())
                {
                    donnees.PagesRapportSelectionnees = donnees.PagesRapport.Where(page => donnees.PagesSelectionnees.Contains(page.SectionId)).ToList();
                    donnees.SectionsAccapManquantes = configuration.Etat != Etat.NouvelleVente && donnees.PagesRapport.Any(page => page.EstAccap && !donnees.PagesSelectionnees.Contains(page.SectionId));
                }
                else
                {
                    donnees.PagesRapportSelectionnees = donnees.PagesRapport.ToList();
                }

                donnees.InclurePageTitre = donnees.PagesRapportSelectionnees.Any(page => EstPageTitre(page.SectionId));
                               
                var rapport = BuildReport(donnees, reportContext);
                if (rapport != null) result.Add(rapport);
            }

            return result.ToArray();
        }

        private IIllustrationMasterReport BuildReport(DonneesRapportIllustration donnees, IReportContext reportContext)
        {
            var configuration = _configurationRepository.ObtenirConfigurationRapport(donnees.Produit, donnees.Etat);
           
            if (configuration != null)
            {
                donnees.TitreRapport = configuration.Titre[reportContext.Language];
            }

            IIllustrationMasterReport report = null;
            var relevantBuilders = _relevancyAnalyzer.GetRelevantBuilders(donnees, reportContext);
            var informationsParProduit = _configurationRepository.ObtenirDefinitionSection<PageTitre>("PageTitre", donnees.Produit).InformationsParProduit;
            var ressource = informationsParProduit.SingleOrDefault(p => p.Produit == donnees.Produit.ToString()) ?? informationsParProduit.Single(p => string.IsNullOrWhiteSpace(p.Produit));

            if (relevantBuilders.Count > 0 || donnees.InclurePageTitre)
            {
                report = _reportFactory.Create<IIllustrationMasterReport>();
                ReportBuilderAssembler.Assemble(report,
                    new IllustrationMasterReportViewModel { NomRessourceImageProduit = ressource.NomRessourceImage },
                    new BuildParameters<DonneesRapportIllustration>(donnees) { ReportContext = reportContext },
                    _mapper, vm => BuildSubParts(relevantBuilders, report));
            }

            return report;
        }

        private bool EstPageTitre(string sectionId)
        {
            return _configurationRepository.ObtenirConfigurationSection(sectionId, GroupeRapport.Principal)?.BuilderName == "Principal.PageTitre";
        }

        private void BuildSubParts(IList<IRelevantBuilder> relevantBuilders, IReport report)
        {
            foreach (IRelevantBuilder relevantBuilder in relevantBuilders)
            {
                relevantBuilder.Build(report);
            }
        }
    }
}