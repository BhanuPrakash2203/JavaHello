using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    public class SectionModelMapper : ISectionModelMapper
    {
        private readonly IIllustrationReportDataFormatter _formatter;
        private readonly IDefinitionNoteManager _noteManager;
        private readonly IDefinitionTableauManager _tableauManager;
        private readonly IDefinitionTitreManager _titreManager;
        private readonly IDefinitionImageManager _imageManager;

        public SectionModelMapper(
            IIllustrationReportDataFormatter formatter, 
            IDefinitionNoteManager noteManager, 
            IDefinitionTableauManager tableauManager, 
            IDefinitionTitreManager titreManager, 
            IDefinitionImageManager imageManager)
        {
            _formatter = formatter;
            _noteManager = noteManager;
            _tableauManager = tableauManager;
            _titreManager = titreManager;
            _imageManager = imageManager;
        }

        public SectionResultatModel MapperDefinition(SectionResultatModel model, DefinitionSectionResultat definition,
            DonneesRapportIllustration donnees)
        {
            model.TitreSection = _titreManager.ObtenirTitre(definition.Titres, donnees);
            model.Description = _titreManager.ObtenirDescription(definition.Titres, donnees);
            model.Avis = _noteManager.CreerAvis(definition.Avis, donnees);
            model.Notes = _noteManager.CreerNotes(definition.Notes, donnees, new DonneesNote());
            model.Tableau = _tableauManager.CreerTableau(definition.Tableau, donnees);
            model.DonneesIllustration = donnees;
            return model;
        }

        public ISectionModel MapperDefinition(ISectionModel model, 
            IDefinitionSection definition, DonneesRapportIllustration donnees, IReportContext context)
        {
            model.TitreSection = _titreManager.ObtenirTitre(definition.Titres, donnees);
            model.Description = _titreManager.ObtenirDescription(definition.Titres, donnees);
            model.Avis = _noteManager.CreerAvis(definition.Avis, donnees);
            model.Notes = _noteManager.CreerNotes(definition.Notes, donnees, new DonneesNote());
            model.Libelles = _formatter.FormatterLibellees(definition.Libelles, context);
            model.Images = _imageManager.Mapper(definition.Images, donnees);
            return model;
        }
    }
}