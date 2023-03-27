using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories
{
    public class SectionModelFactory : ISectionModelFactory
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ISectionModelMapper _sectionModelMapper;

        public SectionModelFactory(
            IConfigurationRepository configurationRepository,
            ISectionModelMapper sectionModelMapper)
        {
            _configurationRepository = configurationRepository;
            _sectionModelMapper = sectionModelMapper;
        }

        public SectionModel Build(string sectionId, DonneesRapportIllustration donnees, IReportContext context)
        {
            var definitionSection = _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(sectionId, donnees.Produit);
            var model = new SectionModel();
            _sectionModelMapper.MapperDefinition(model, definitionSection, donnees, context);
            return model;
        }
    }
}