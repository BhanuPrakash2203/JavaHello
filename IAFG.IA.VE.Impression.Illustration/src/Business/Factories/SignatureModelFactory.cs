using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories
{
    public class SignatureModelFactory : ISignatureModelFactory
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ISectionModelMapper _sectionModelMapper;
        private readonly IDefinitionTexteManager _texteManager;

        public SignatureModelFactory(IConfigurationRepository configurationRepository, ISectionModelMapper sectionModelMapper, IDefinitionTexteManager texteManager)
        {
            _configurationRepository = configurationRepository;
            _sectionModelMapper = sectionModelMapper;
            _texteManager = texteManager;
        }

        public SectionSignatureModel Build(string sectionId, DonneesRapportIllustration donnees, IReportContext context)
        {
            var definitionSection = _configurationRepository.ObtenirDefinitionSection<DefinitionSectionSignature>(sectionId, donnees.Produit);
            var model = new SectionSignatureModel();
            _sectionModelMapper.MapperDefinition(model, definitionSection, donnees, context);
            model.Details = _texteManager.CreerDetailTextes(definitionSection.Textes, donnees);
            model.Signatures = MapperSignatures(definitionSection, donnees);
            model.NumeroContrat = donnees.NumeroContrat;
            model.EstNouveauContrat = donnees.Etat == Etat.NouvelleVente;
            return model;
        }

        public List<string> MapperSignatures(DefinitionSectionSignature definition, DonneesRapportIllustration donnees)
        {
            var result = new List<string>();
            var signatureContractant = donnees.ContractantEstCompagnie
                                           ? definition.SignatureContractantCompagnie
                                           : definition.SignatureContractant;

            result.Add(_texteManager.ObtenirTexte(signatureContractant, donnees));
            if (donnees.Clients.Count(c => c.EstContractant) > 1)
            {
                result.Add(_texteManager.ObtenirTexte(signatureContractant, donnees));
            }

            result.Add(_texteManager.ObtenirTexte(definition.SignatureRepresentant, donnees));
            return result;
        }
    }
}