using System.Collections.Generic;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.NotesIllustration;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories
{
    public class NotesIllustrationModelFactory : INotesIllustrationModelFactory
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ISectionModelMapper _sectionModelMapper;
        private readonly IDefinitionSectionManager _sectionManager;
        private readonly IDefinitionTexteManager _texteManager;
        private readonly IDefinitionTitreManager _titreManager;

        public NotesIllustrationModelFactory(IConfigurationRepository configurationRepository,
            ISectionModelMapper sectionModelMapper, 
            IDefinitionSectionManager sectionManager, IDefinitionTitreManager titreManager, 
            IDefinitionTexteManager texteManager)
        {
            _configurationRepository = configurationRepository;
            _sectionModelMapper = sectionModelMapper;
            _sectionManager = sectionManager;
            _titreManager = titreManager;
            _texteManager = texteManager;
        }

        public SectionNotesIllustrationModel Build(string sectionId, DonneesRapportIllustration donnees,
            IReportContext context)
        {
            var definitionSection =
                _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(sectionId,
                    donnees.Produit, _sectionManager.Merge);

            var model = new SectionNotesIllustrationModel();
            _sectionModelMapper.MapperDefinition(model, definitionSection, donnees, context);
            model.SousSections = CreerDetailNotesIllustration(definitionSection, donnees);
            return model;
        }

        public IList<NotesIllustration> CreerDetailNotesIllustration(
            DefinitionSection definition, DonneesRapportIllustration donnes)
        {
            var result = new List<NotesIllustration>();

            foreach (var sousSectionNotes in definition.ListSections)
            {
                var sousSection = new NotesIllustration
                {
                    Titre = _titreManager.ObtenirTitre(sousSectionNotes.Titres, donnes),
                    Textes = _texteManager.CreerDetailTextes(sousSectionNotes.Textes, donnes)
                };
                result.Add(sousSection);
            }

            return result;
        }
    }
}