using System.Linq;
using System.Collections.Generic;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories
{
    public class ApercuProtectionsModelFactory : IApercuProtectionsModelFactory
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IIllustrationReportDataFormatter _formatter;
        private readonly IDefinitionNoteManager _noteManager;
        private readonly IDefinitionTableauManager _tableauManager;
        private readonly IDefinitionTitreManager _titreManager;

        public ApercuProtectionsModelFactory(IConfigurationRepository configurationRepository, 
            IIllustrationReportDataFormatter formatter, 
            IDefinitionNoteManager noteManager,
            IDefinitionTitreManager definitionTitreManager,
            IDefinitionTableauManager tableauManager)
        {
            _configurationRepository = configurationRepository;
            _formatter = formatter;
            _noteManager = noteManager;
            _titreManager = definitionTitreManager;
            _tableauManager = tableauManager;
        }

        public SectionApercuProtectionsModel Build(string sectionId, DonneesRapportIllustration donnees, IReportContext context)
        {
            var definitionSection = _configurationRepository.ObtenirDefinitionSection<DefinitionSectionResultats>(sectionId, donnees.Produit);

            var model = new SectionApercuProtectionsModel
            {
                TitreSection = _formatter.FormatterTitre(definitionSection.Titres.FirstOrDefault(), donnees),
                SectionResultats = BuildResultats(donnees, definitionSection)
            };

            return model;
        }

        private SectionResultatModel[] BuildResultats(DonneesRapportIllustration donnees, DefinitionSectionResultats definitionSection)
        {
            var result = new List<SectionResultatModel>();
            foreach (var protectionGroupee in donnees.ProtectionsGroupees)
            {
                if (!protectionGroupee.ProtectionsAssures.Any(p => p.EstProtectionContractant) && protectionGroupee.ProtectionsAssures.Count > 0)
                {
                    var section = protectionGroupee.ProtectionsAssures.First().Assures.Count == 1 ? definitionSection.SectionIndividuelle : definitionSection.SectionConjointe;
                    var tableau = CreerTableauDynamique(section, donnees, _formatter, _noteManager, _tableauManager, protectionGroupee);
                    var noms = FormatterNoms(protectionGroupee.ProtectionsAssures.FirstOrDefault());
                    tableau.TitreTableau = string.Format(tableau.TitreTableau, noms);
                    var model = new SectionResultatModel
                    {
                        Tableau = tableau,
                        DonneesIllustration = donnees,
                        AnneeDebutProjection = donnees.Projections.AnneeDebutProjection,
                        AnneeFinProjection = donnees.Projections.AnneeFinProjection,
                        AgeReferenceFinProjection = donnees.Projections.AgeReferenceFinProjection,
                        IndexFinProjection = donnees.Projections.IndexFinProjection
                    };

                    model.CalculerAnneesSelection(donnees, donnees.ChoixAnneesRapport);
                    result.Add(model);
                }
            }

            return result.ToArray();
        }

        private string FormatterNoms(Protection protection)
        {
            if (protection?.Assures == null)
            {
                return string.Empty;
            }

            var noms = string.Empty;
            foreach (var assure in protection.Assures)
            {
                if (!string.IsNullOrEmpty(noms))
                {
                    noms += " - ";
                }
                noms += _formatter.FormatFullName(assure.Prenom, assure.Nom, assure.Initiale);
            }

            return noms;
        }

        private TableauResultat CreerTableauDynamique(DefinitionSectionResultat definitionSection,
            DonneesRapportIllustration donnes,
            IIllustrationReportDataFormatter formatter,
            IDefinitionNoteManager noteManager,
            IDefinitionTableauManager tableauManager,
            ProtectionsGroupees protectionsGroupes)
        {
            if (definitionSection == null) return null;

            var tabResult = new TableauResultat
            {
                InclureLigneDecheance = false,
                Avis = new List<string>(),
                Notes = new List<DetailNote>(),
                TitreTableau = _titreManager.ObtenirTitre(definitionSection.Tableau.Titres, donnes),
                TypeTableau = definitionSection.Tableau.TypeTableau,
                IdentifiantGroupeAssure = protectionsGroupes.Identifier.Id,
                GroupeColonnes = tableauManager.CreerGroupeColonnes(definitionSection.Tableau.GroupeColonnes, donnes, protectionsGroupes.Identifier.Id)
            };

            if (definitionSection.Avis != null)
            {
                tabResult.Avis = (from DefinitionAvis avi in definitionSection.Avis orderby avi.SequenceId select formatter.FormatterAvis(avi, donnes)).ToList();
            }

            if (definitionSection.Notes != null)
            {
                tabResult.Notes = noteManager.CreerNotes(definitionSection.Notes, donnes, new DonneesNote());
            }

            return tabResult;
        }
    }
}