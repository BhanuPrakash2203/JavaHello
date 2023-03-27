using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories
{
    public class TestSensibiliteModelFactory : ITestSensibiliteModelFactory, IProjectionModelFactory
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ISectionModelMapper _sectionModelMapper;

        public TestSensibiliteModelFactory(IConfigurationRepository configurationRepository, ISectionModelMapper sectionModelMapper)
        {
            _configurationRepository = configurationRepository;
            _sectionModelMapper = sectionModelMapper;
        }

        public SectionResultatModel Build(string sectionId, DonneesRapportIllustration donnees, IReportContext context)
        {
            var model = this.Build(sectionId, donnees, _configurationRepository, _sectionModelMapper);
            return model;
        }

        public ChoixAnneesRapport DeterminerAnneesProjection(DonneesRapportIllustration donnees, TypeChoixAnneesRapport? choixAnnees)
        {
            return new ChoixAnneesRapport
            {
                ChoixAnnees = TypeChoixAnneesRapport.Selection,
                Annees = new[] { 5, 10, 20 },
                Ages = new[] { 65, 85, donnees.Projections.AgeReferenceFinProjection }
            };
        }
    }
}