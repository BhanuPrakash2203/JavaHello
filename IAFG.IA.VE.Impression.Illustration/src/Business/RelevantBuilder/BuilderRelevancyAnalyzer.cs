using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.RelevantBuilder;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;

namespace IAFG.IA.VE.Impression.Illustration.Business.RelevantBuilder
{
    public class BuilderRelevancyAnalyzer : IBuilderRelevancyAnalyzer
    {
        private readonly IGeneriqueBuilders _generiqueBuilders;
        private readonly IPrincipalBuilders _principalBuilders;
        private readonly IBonSuccessoralBuilders _bonSucessoralBuilders;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly Dictionary<string, Func<ConfigurationSection, DonneesRapportIllustration, IReportContext, IRelevantBuilder>> _builders;

        public BuilderRelevancyAnalyzer(
            IGeneriqueBuilders generiqueBuilders,
            IPrincipalBuilders principalBuilders,
            IBonSuccessoralBuilders bonSucessoralBuilders,
            IConfigurationRepository configurationRepository)
        {
            _generiqueBuilders = generiqueBuilders;
            _principalBuilders = principalBuilders;
            _bonSucessoralBuilders = bonSucessoralBuilders;
            _configurationRepository = configurationRepository;
            _builders = CreerBuilderFunctionList();
        }

        private Dictionary<string, Func<ConfigurationSection, DonneesRapportIllustration, IReportContext, IRelevantBuilder>> CreerBuilderFunctionList()
        {
            var list = new Dictionary<string, Func<ConfigurationSection, DonneesRapportIllustration, IReportContext, IRelevantBuilder>>
            {
                { "Generique.Glossaire", _generiqueBuilders.GetBuilderGlossaire },
                { "Generique.Projection", _generiqueBuilders.GetBuilderProjection },
                { "Generique.ProjectionParGroupeAssure", _generiqueBuilders.GetBuilderProjectionParGroupeAssure },
                { "Generique.Section", _generiqueBuilders.GetBuilderSection },
                { "Principal.ApercuProtections", _principalBuilders.GetBuilderApercuProtections },
                { "Principal.ConceptVente", _principalBuilders.GetBuilderConceptVente },
                { "Principal.ConditionsMedicales", _principalBuilders.GetBuilderConditionsMedicales },
                { "Principal.DescriptionsProtections", _principalBuilders.GetBuilderDescriptionsProtections },
                { "Principal.PageTitre", null },
                { "Principal.ParticularitesAssurance", _principalBuilders.GetBuilderDescriptionsProtections },
                { "Principal.HypothesesInvestissement", _principalBuilders.GetBuilderHypothesesInvestissement },
                { "Principal.Introduction", _principalBuilders.GetBuilderIntroduction },
                { "Principal.ModificationsDemandees", _principalBuilders.GetBuilderModificationsDemandees },
                { "Principal.NotesIllustration", _principalBuilders.GetBuilderNotesIllustration },
                { "Principal.PrimesDeRenouvellement", _principalBuilders.GetBuilderPrimesDeRenouvellement },
                { "Principal.Protections", _principalBuilders.GetBuilderProtections },
                { "Principal.Signature", _principalBuilders.GetBuilderSignature },
                { "Principal.SommaireProtections", _principalBuilders.GetBuilderSommaireProtections },
                { "Principal.TestDeSensibilite", _principalBuilders.GetBuilderTestDeSensibilite },
                { "BonSuccessoral.PageTitre", _bonSucessoralBuilders.GetBuilderPageTitre },
                { "BonSuccessoral.SommaireBonSuccessoral", _bonSucessoralBuilders.GetBuilderSommaireBonSuccessoral },
                { "BonSuccessoral.PageGraphique", _bonSucessoralBuilders.GetBuilderGraphique }
            };

            return list;
        }

        public IList<IRelevantBuilder> GetRelevantBuilders(DonneesRapportIllustration donnees, IReportContext context)
        {
            return GetRelevantBuildersBasedOnUserSelection(donnees, context);
        }

        private List<IRelevantBuilder> GetRelevantBuildersBasedOnUserSelection(DonneesRapportIllustration donnees, 
            IReportContext context)
        {
            var builders = new List<IRelevantBuilder>();
            if (donnees.PagesRapportSelectionnees != null)
            {
                builders.AddRange(donnees.PagesRapportSelectionnees
                    .Select(page => GetBuilder(page.SectionId, donnees, context))
                    .Where(builder => builder != null));
            }

            return builders;
        }

        private IRelevantBuilder GetBuilder(string sectionId, DonneesRapportIllustration donnees, 
            IReportContext context)
        {
            var section = _configurationRepository.ObtenirConfigurationSection(sectionId, donnees.GroupeRapport);
            return GetBuilderBasedOnBuilderName(section, donnees, context);
        }

        private IRelevantBuilder GetBuilderBasedOnBuilderName(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context)
        {
            if (section == null || string.IsNullOrEmpty(section.BuilderName))
            {
                return null;
            }

            if (!_builders.ContainsKey(section.BuilderName))
            {
                throw new ArgumentOutOfRangeException(nameof(section), $@"Le constructeur de section <{section.BuilderName}> n'est pas géré.");
            }

            return _builders[section.BuilderName]?.Invoke(section, donnees, context);
        }
    }
}