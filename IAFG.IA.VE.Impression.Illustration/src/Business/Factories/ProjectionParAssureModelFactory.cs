using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories
{
    public class ProjectionParAssureModelFactory : IProjectionParAssureModelFactory
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IIllustrationReportDataFormatter _formatter;
        private readonly ISectionModelMapper _sectionModelMapper;
        private readonly IDefinitionNoteManager _noteManager;
        private readonly IDefinitionTableauManager _tableauManager;

        public ProjectionParAssureModelFactory(IConfigurationRepository configurationRepository,
            IIllustrationReportDataFormatter formatter, ISectionModelMapper sectionModelMapper, 
            IDefinitionNoteManager noteManager, IDefinitionTableauManager tableauManager)
        {
            _configurationRepository = configurationRepository;
            _formatter = formatter;
            _sectionModelMapper = sectionModelMapper;
            _noteManager = noteManager;
            _tableauManager = tableauManager;
        }

        public SectionResultatParAssureModel Build(string sectionId, DonneesRapportIllustration donnees, IReportContext context)
        {
            return this.Build(sectionId, donnees, _configurationRepository, _formatter, _sectionModelMapper, _noteManager, _tableauManager, context);
        }

        public ChoixAnneesRapport DeterminerAnneesProjection(DonneesRapportIllustration donnees, TypeChoixAnneesRapport? choixAnnees)
        {
            return choixAnnees.HasValue ? new ChoixAnneesRapport {ChoixAnnees = choixAnnees.Value} : donnees.ChoixAnneesRapport;
        }

        public IList<ProtectionsGroupees> ObtenirProtectionsGroupees(DonneesRapportIllustration donnees, TypeTableau typeTableau)
        {
            var result = donnees.ProtectionsGroupees.Where(x => x.ProtectionsAssures?.Any() ?? false).ToList();
            switch (typeTableau)
            {
                case TypeTableau.AssureAdditionnel:
                    return result.Where(x => x.Identifier.Id != donnees.ObtenirIdentifiantGroupeAssurePrincipal()).ToList();
                case TypeTableau.AssurePrincipal:
                    return result.Where(x => x.Identifier.Id == donnees.ObtenirIdentifiantGroupeAssurePrincipal()).ToList();
                case TypeTableau.Assure:
                case TypeTableau.Contrat:
                case TypeTableau.TestSensibilite:
                    return result;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeTableau), typeTableau, null);
            }
        }
    }
}