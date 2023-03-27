using System.Linq;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Rules;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.BonSuccessoral.Sommaire;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories.BonSuccessoral
{
    public class SommaireBonSuccessoralModelFactory : ISommaireBonSuccessoralModelFactory
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ISectionModelMapper _sectionModelMapper;
        private readonly IProductRules _productRules;

        public SommaireBonSuccessoralModelFactory(
            IConfigurationRepository configurationRepository,
            ISectionModelMapper sectionModelMapper,
            IProductRules productRules)
        {
            _configurationRepository = configurationRepository;
            _sectionModelMapper = sectionModelMapper;
            _productRules = productRules;
        }

        public SommaireBonSuccessoralModel Build(string sectionId, DonneesRapportIllustration donnees, IReportContext context)
        {
            var definitionSection = _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(sectionId, donnees.Produit);
            var model = new SommaireBonSuccessoralModel();
            _sectionModelMapper.MapperDefinition(model, definitionSection, donnees, context);

            var bonSuccessoral = donnees.ConceptVente?.BonSuccessoral;
            if (bonSuccessoral == null) return model;

            model.Contrat = CreerSectionContrat(
                definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "Contrat"), 
                donnees, bonSuccessoral, context);

            model.HypothesesInvestissement = CreerSectionHypothesesInvestissement(
                definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "HypothesesInvestissement"), 
                donnees, bonSuccessoral, context);

            model.Imposition = CreerSectionImposition(
                definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "Imposition"), 
                donnees, bonSuccessoral, context);

            return model;
        }

        private ContratModel CreerSectionContrat(
            DefinitionSection definition, 
            DonneesRapportIllustration donnees, 
            Types.Models.BonSuccessoral.BonSuccessoral bonSuccessoral, 
            IReportContext context)
        {
            if (definition == null) return null;
            var model = new ContratModel();
            _sectionModelMapper.MapperDefinition(model, definition, donnees, context);
            model.DescriptionProtection = context.Language == Language.French ? bonSuccessoral.Plan.DescriptionFr : bonSuccessoral.Plan.DescriptionAn;
            model.MontantProtectionInitial = bonSuccessoral.MontantProtectionInitial;
            model.TauxInvestissement = bonSuccessoral.TauxInvestissement;
            
            if (_productRules.EstParmiFamilleAssuranceParticipants(donnees.Produit))
            {
                model.BaremeParticipation = context.Language == Language.French ? "Courant" : "Current";
            }

            return model;
        }

        private HypothesesInvestissementModel CreerSectionHypothesesInvestissement(
            DefinitionSection definition, 
            DonneesRapportIllustration donnees, 
            Types.Models.BonSuccessoral.BonSuccessoral bonSuccessoral, IReportContext context)
        {
            if (definition == null) return null;
            var model = new HypothesesInvestissementModel();
            _sectionModelMapper.MapperDefinition(model, definition, donnees, context);

            model.Interets = new InvestissementModel { 
                Repartition = bonSuccessoral.Hypotheses?.Interets?.Repartition ?? 0, 
                TauxRendement = bonSuccessoral.Hypotheses?.Interets?.TauxRendement ?? 0 
            };

            model.Dividendes = new InvestissementModel { 
                Repartition = bonSuccessoral.Hypotheses?.Dividendes?.Repartition ?? 0, 
                TauxRendement = bonSuccessoral.Hypotheses?.Dividendes?.TauxRendement ?? 0 
            };

            model.GainCapital = new InvestissementModel { 
                Repartition = bonSuccessoral.Hypotheses?.GainCapital?.Repartition ?? 0, 
                TauxRendement = bonSuccessoral.Hypotheses?.GainCapital?.TauxRendement ?? 0, 
                TauxRealisation = bonSuccessoral.Hypotheses?.GainCapital?.TauxRealisation ?? 0
            };

            return model;
        }

        private ImpositionModel CreerSectionImposition(
            DefinitionSection definition, 
            DonneesRapportIllustration donnees, 
            Types.Models.BonSuccessoral.BonSuccessoral bonSuccessoral, IReportContext context)
        {
            if (definition == null) return null;
            var model = new ImpositionModel();
            _sectionModelMapper.MapperDefinition(model, definition, donnees, context);
            model.EstCorporation = bonSuccessoral.Impositions.EstCorporation;
            model.TauxMarginal = bonSuccessoral.Impositions.TauxMarginal;
            model.TauxDividendes = bonSuccessoral.Impositions.TauxDividendes;
            model.TauxDividendesActionnaires = bonSuccessoral.Impositions.TauxDividendesActionnaires;
            model.TauxGainCapital = bonSuccessoral.Impositions.TauxGainCapital;
            return model;
        }


    }
}
