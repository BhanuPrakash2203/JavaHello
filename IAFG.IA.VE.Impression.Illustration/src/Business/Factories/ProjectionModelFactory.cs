using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories
{
    public class ProjectionModelFactory : IProjectionModelFactory
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ISectionModelMapper _sectionModelMapper;

        public ProjectionModelFactory(IConfigurationRepository configurationRepository, ISectionModelMapper sectionModelMapper)
        {
            _configurationRepository = configurationRepository;
            _sectionModelMapper = sectionModelMapper;
        }

        public SectionResultatModel Build(string sectionId, DonneesRapportIllustration donnees, Core.Interface.ReportContext.IReportContext context)
        {
            return this.Build(sectionId, donnees, _configurationRepository, _sectionModelMapper);
        }

        public ChoixAnneesRapport DeterminerAnneesProjection(DonneesRapportIllustration donnees, TypeChoixAnneesRapport? choixAnnees)
        {
            return choixAnnees.HasValue ? new ChoixAnneesRapport { ChoixAnnees = choixAnnees.Value } : donnees.ChoixAnneesRapport;
        }
    }
}