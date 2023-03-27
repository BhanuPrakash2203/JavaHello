using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.BonSuccessoral;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories.BonSuccessoral
{
    public class TitreRapportModelFactory : ITitreRapportModelFactory
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ISectionModelMapper _sectionModelMapper;

        public TitreRapportModelFactory(IConfigurationRepository configurationRepository, 
            ISectionModelMapper sectionModelMapper)
        {
            _configurationRepository = configurationRepository;
            _sectionModelMapper = sectionModelMapper;
        }

        public TitreRapportModel Build(string sectionId, DonneesRapportIllustration donnees, IReportContext context)
        {
            var definitionSection = _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(sectionId, donnees.Produit);
            var model = new TitreRapportModel();
            _sectionModelMapper.MapperDefinition(model, definitionSection, donnees, context);
            model.Agents = donnees.Agents;
            model.Clients = donnees.Clients;
            model.DatePreparation = donnees.DatePreparation;
            return model;
        }
    }
}
