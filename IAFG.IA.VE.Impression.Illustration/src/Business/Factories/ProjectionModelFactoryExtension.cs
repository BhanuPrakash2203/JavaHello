using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories
{
    public static class ProjectionModelFactoryExtension
    {
        private const int NombreMaximalAnnee = 115;

        public static SectionResultatModel Build(this IProjectionModelFactory modelFactory, string sectionId,
            DonneesRapportIllustration donnees, IConfigurationRepository configurationRepository,
            ISectionModelMapper modelMapper) 
        {
            var definitionSection =
                configurationRepository.ObtenirDefinitionSection<DefinitionSectionResultat>(sectionId, donnees.Produit);

            var model = new SectionResultatModel
            {
                AgeReferenceFinProjection = donnees.Projections.AgeReferenceFinProjection,
                AnneeDebutProjection = donnees.Projections.AnneeDebutProjection,
                AnneeFinProjection = donnees.Projections.AnneeFinProjection
            };

            model.CalculerAnneesSelection(donnees,
                modelFactory.DeterminerAnneesProjection(donnees, definitionSection.Tableau.ChoixAnnees));
            modelMapper.MapperDefinition(model, definitionSection, donnees);
            return model;
        }

        public static SectionResultatParAssureModel Build(this IProjectionParAssureModelFactory modelFactory,
            string sectionId, DonneesRapportIllustration donnees, IConfigurationRepository configurationRepository,
            IIllustrationReportDataFormatter formatter, ISectionModelMapper modelMapper, IDefinitionNoteManager noteManager,
            IDefinitionTableauManager tableauManager, IReportContext context) 
        {
            var definitionSection =
                configurationRepository.ObtenirDefinitionSection<DefinitionSectionResultat>(sectionId, donnees.Produit);

            var models = new List<SectionResultatModel>();
            foreach (var protectionGroupee in modelFactory.ObtenirProtectionsGroupees(donnees,
                definitionSection.Tableau.TypeTableau))
            {
                var modelAssure = new SectionResultatModel
                {
                    DonneesIllustration = donnees,
                    AnneeDebutProjection = donnees.Projections.AnneeDebutProjection,
                    AnneeFinProjection = donnees.Projections.AnneeFinProjection,
                    AgeReferenceFinProjection = donnees.Projections.AgeReferenceFinProjection,
                    IndexFinProjection = donnees.Projections.IndexFinProjection,
                    Tableau = CreerTableau(definitionSection, donnees, formatter, noteManager, tableauManager, protectionGroupee)
                };

                modelAssure.CalculerAnneesSelection(donnees,
                    modelFactory.DeterminerAnneesProjection(donnees, definitionSection.Tableau.ChoixAnnees));
                models.Add(modelAssure);
            }

            var model = new SectionResultatParAssureModel { SectionResultatModels = models};
            modelMapper.MapperDefinition(model, definitionSection, donnees, context);
            return model;
        }

        public static void CalculerAnneesSelection(this SectionResultatModel resultatModel,
            DonneesRapportIllustration donnees, ChoixAnneesRapport choixAnneesRapport)
        {
            var choix = choixAnneesRapport ?? new ChoixAnneesRapport { ChoixAnnees = TypeChoixAnneesRapport.ToutesLesAnnees };
            var anneesAgesTranches5 = new List<int>();
            for (var i = 15; i <= NombreMaximalAnnee; i += 5)
            {
                anneesAgesTranches5.Add(i);
            }

            switch (choix.ChoixAnnees)
            {
                case TypeChoixAnneesRapport.Standard:
                    resultatModel.SelectionAgesResultats = new[] { 55, 60, 65, 70, 85, 100 };

                    var annees = new List<int>();
                    var index = 0;
                    foreach (var age in donnees.Projections.Projection.Ages)
                    {
                        if (resultatModel.SelectionAgesResultats.Contains((int)age))
                            annees.Add(donnees.Projections.Projection.AnneesContrat[index]);

                        index += 1;
                    }

                    resultatModel.SelectionAnneesResultats = Enumerable.Range(1, 20).Union(annees).ToArray();
                    break;

                case TypeChoixAnneesRapport.ToutesLesAnnees:
                    resultatModel.SelectionAnneesResultats = Enumerable.Range(donnees.Projections.AnneeDebutProjection, donnees.Projections.AnneeFinProjection).ToArray();
                    if (resultatModel.SelectionAnneesResultats == null || resultatModel.SelectionAnneesResultats.Length == 0)
                    {
                        resultatModel.SelectionAgesResultats = new int[0];
                        resultatModel.SelectionAnneesResultats = Enumerable.Range(1, NombreMaximalAnnee).ToArray();
                    }
                    break;

                case TypeChoixAnneesRapport.Annee1A20:
                    resultatModel.SelectionAgesResultats = new[] { 55, 60, 65, 70, 85, 100 };
                    resultatModel.SelectionAnneesResultats = Enumerable.Range(1, 20).ToArray();
                    break;

                case TypeChoixAnneesRapport.Annee1A10_15_20:
                    resultatModel.SelectionAgesResultats = new[] { 55, 60, 65, 70, 85, 100 };
                    resultatModel.SelectionAnneesResultats = Enumerable.Range(1, 10).Union(new[] { 15, 20 }).ToArray();
                    break;

                case TypeChoixAnneesRapport.Annee1A10Tranche5:
                    resultatModel.SelectionAgesResultats = new int[0];
                    resultatModel.SelectionAnneesResultats = Enumerable.Range(1, 10).Union(anneesAgesTranches5).ToArray();
                    break;

                case TypeChoixAnneesRapport.Annee1A10Quinquennaux:
                    resultatModel.SelectionAgesResultats = anneesAgesTranches5.ToArray();
                    resultatModel.SelectionAnneesResultats = Enumerable.Range(1, 10).ToArray();
                    break;

                case TypeChoixAnneesRapport.Selection:
                    resultatModel.SelectionAgesResultats = choix.Ages;
                    resultatModel.SelectionAnneesResultats = choix.Annees;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(choixAnneesRapport));
            }
        }

        private static TableauResultat CreerTableau(
            DefinitionSectionResultat definitionSection, 
            DonneesRapportIllustration donnees, 
            IIllustrationReportDataFormatter formatter,
            IDefinitionNoteManager noteManager,
            IDefinitionTableauManager tableauManager,
            ProtectionsGroupees protectionGroupees)
        {
            if (definitionSection == null) return null;

            var noms = FormatterNomAssures(protectionGroupees.ProtectionsAssures?.FirstOrDefault()?.Assures, formatter);
            var tableauResultat = tableauManager.CreerTableauPourGroupeAssures(definitionSection.Tableau, protectionGroupees.Identifier.Id, noms, donnees);

            if (definitionSection.Notes != null)
            {
                var donneesNote = new DonneesNote { IdentifiantGroupeAssure = protectionGroupees.Identifier.Id };
                tableauResultat.Notes = noteManager.CreerNotes(definitionSection.Notes, donnees, donneesNote);
            }

            return tableauResultat;
        }

        private static string[] FormatterNomAssures(IEnumerable<Assure> assures, IIllustrationReportDataFormatter formatter)
        {
            return assures?.Select(assure => formatter.FormatFullName(assure.Prenom, assure.Nom, assure.Initiale)).ToArray() ?? new string[] { };
        }
    }
}