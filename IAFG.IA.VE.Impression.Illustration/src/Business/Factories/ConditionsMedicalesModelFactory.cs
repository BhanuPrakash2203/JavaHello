using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions.ConditionsMedicales;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.ConditionsMedicales;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories
{
    public class ConditionsMedicalesModelFactory : IConditionsMedicalesModelFactory
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ISectionModelMapper _sectionModelMapper;

        private const string CodeConditionMedicale = "CodeConditionMedicale";
        private const string CodeDescription = "CodeDescription";
        private const string CodeModuleEnfantMaladieGrave = "ModuleEnfantMaladieGrave1";

        private const string CodeMaladieGrave25Permanente = "MaladiesGraves25Permanente";
        private const string CodeMaladieGrave25Temporaire = "MaladiesGraves25Temporaire";
        private const string CodeMaladieGraveDuoVieSante = "DuoVieSante";

        private const string CodeMaladieGrave4Permanente = "MaladiesGraves4Permanente";
        private const string CodeMaladieGrave4Temporaire = "MaladiesGraves4Temporaire";

        public ConditionsMedicalesModelFactory(IConfigurationRepository configurationRepository,
            ISectionModelMapper sectionModelMapper)
        {
            _configurationRepository = configurationRepository;
            _sectionModelMapper = sectionModelMapper;
        }

        public SectionConditionsMedicalesModel Build(string sectionId, DonneesRapportIllustration donnees,
            IReportContext context)
        {
            var definitionSection =
                _configurationRepository.ObtenirDefinitionSection<DefinitionSectionConditionsMedicales>(sectionId,
                    donnees.Produit);

            var model = new SectionConditionsMedicalesModel
            {
                Sections = new List<ConditionsMedicalesSection>()
            };
            _sectionModelMapper.MapperDefinition(model, definitionSection, donnees, context);

            if (definitionSection.Sections == null)
            {
                return model;
            }

            foreach (var section in definitionSection.Sections.Where(x => IsVisible(x, donnees)))
            {
                AddSection(model, section.SectionId, donnees, context);
            }

            return model;
        }

        private void AddSection(SectionConditionsMedicalesModel model,
            string sectionId, DonneesRapportIllustration donnees, IReportContext context)
        {
            var definition =
                _configurationRepository
                    .ObtenirDefinitionSection<DefinitionConditionsMedicalesSousSection>(sectionId,
                        donnees.Produit);

            var section = new ConditionsMedicalesSection
            {
                Details = CreerDetailConditionsMedicales(definition, donnees)
            };

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
            model.Sections.Add(section);
        }

        internal static List<ConditionMedicale> CreerDetailConditionsMedicales(
            DefinitionConditionsMedicalesSousSection definitionConditionsMedicalesSous, DonneesRapportIllustration donnees)
        {
            var result = new List<ConditionMedicale>();
            if (definitionConditionsMedicalesSous.ConditionMedicaleTextes == null)
            {
                return result;
            }

            var descriptiveCodes = new List<string>();
            foreach (var protPdf in donnees.ProtectionsPDF)
            {
                if (protPdf.DescriptiveCodeInfos.ContainsKey(CodeConditionMedicale))
                {
                    descriptiveCodes.AddRange(protPdf.DescriptiveCodeInfos[CodeConditionMedicale]);
                }

                if (protPdf.DescriptiveCodeInfos.ContainsKey(CodeDescription) &&
                    protPdf.DescriptiveCodeInfos.Any(desc => desc.Value.Contains(CodeModuleEnfantMaladieGrave)))
                {
                    descriptiveCodes.Add(CodeModuleEnfantMaladieGrave);
                }
            }

            var textes = definitionConditionsMedicalesSous.ConditionMedicaleTextes.OrderBy(t => t.SequenceId);
            result.AddRange(from texteDescriptions in textes
                where ValiderCodeDescription(descriptiveCodes, texteDescriptions)
                select new ConditionMedicale
                {
                    Libelle = FormatterLibellee(texteDescriptions, donnees),
                    Texte = FormatterTexte(texteDescriptions, donnees),
                    SequenceId = texteDescriptions.SequenceId,
                    Textes = CreerTextes(texteDescriptions, donnees),
                    Tableau = CreerTableau(texteDescriptions, donnees),
                    EstTitreSousSection = definitionConditionsMedicalesSous.EstTitreSousSection
                });

            return result;
        }

        private static List<TableauItem> CreerTableau(DefinitionConditionMedicale definition,
            DonneesRapportIllustration donnees)
        {
            if (definition.Tableau == null)
            {
                return new List<TableauItem>();
            }

            var tableau = definition.Tableau
                .Where(d => ValiderRegleDescriptionItem(d, donnees))
                .OrderBy(p => p.Categorie);

            return donnees.Langue == Language.English
                ? tableau.Select(x => new TableauItem { Categorie = x.CategorieEn, Texte = x.TexteEn }).ToList()
                : tableau.Select(x => new TableauItem { Categorie = x.Categorie, Texte = x.Texte }).ToList();
        }

        private static List<TexteItem> CreerTextes(DefinitionConditionMedicale definition,
            DonneesRapportIllustration donnees)
        {
            if (definition.Textes == null)
            {
                return new List<TexteItem>();
            }

            return donnees.Langue == Language.English
                ? definition.Textes
                    .Select(x => new TexteItem { Texte = x.TexteEn, SequenceId = x.SequenceId }).ToList()
                : definition.Textes
                    .Select(x => new TexteItem { Texte = x.Texte, SequenceId = x.SequenceId }).ToList();
        }

        private static bool IsVisible(DefinitionConditionsMedicalesSousSection definitionConditionsMedicalesSous,
            DonneesRapportIllustration donnees)
        {
            var regles = definitionConditionsMedicalesSous.Regles;
            if (regles == null) return true;

            var result = true;
            foreach (var item in regles)
            {
                switch (item)
                {
                    case RegleConditionsMedicales.Aucune:
                        break;

                    case RegleConditionsMedicales.MaladieGrave25:
                        result = donnees.ProtectionsPDF.Any(p => p.DescriptiveCodeInfos.Any(desc =>
                                                                     desc.Key.Equals(CodeConditionMedicale) &&
                                                                     (desc.Value.Contains(
                                                                          CodeMaladieGrave25Permanente) ||
                                                                      desc.Value.Contains(
                                                                          CodeMaladieGrave25Temporaire) ||
                                                                      desc.Value.Contains(
                                                                          CodeMaladieGraveDuoVieSante))) ||
                                                                 p.DescriptiveCodeInfos.Any(desc =>
                                                                     desc.Value.Contains("ModuleEnfantMaladieGrave1")));
                        break;

                    case RegleConditionsMedicales.MaladieGrave4:
                        var contient25Maladies = donnees.ProtectionsPDF.Any(p => p.DescriptiveCodeInfos.Any(desc =>
                                                                                      desc.Key.Equals(
                                                                                          CodeConditionMedicale) &&
                                                                                      (desc.Value.Contains(
                                                                                           CodeMaladieGrave25Permanente) ||
                                                                                       desc.Value.Contains(
                                                                                           CodeMaladieGrave25Temporaire) ||
                                                                                       desc.Value.Contains(
                                                                                           CodeMaladieGraveDuoVieSante)
                                                                                      )) || p.DescriptiveCodeInfos.Any(
                                                                                      desc => desc.Value.Contains(
                                                                                          "ModuleEnfantMaladieGrave1")));
                        result = !contient25Maladies && donnees.ProtectionsPDF.Any(p =>
                                     p.DescriptiveCodeInfos.Any(desc =>
                                         desc.Key.Equals(CodeConditionMedicale) &&
                                         (desc.Value.Contains(CodeMaladieGrave4Permanente) ||
                                          desc.Value.Contains(CodeMaladieGrave4Temporaire))));
                        break;

                    case RegleConditionsMedicales.MaladieGraveJuvenile:
                        var listeCodePlan = donnees.ProtectionsPDF
                            .Where(p => p.Specification != null && p.Specification.IsCriticalIllness)
                            .Select(p => p.CodePlan).ToList();
                        var trouve = donnees.Protections.ProtectionsAssures.Any(pa =>
                            pa.Assures.Any(a => a.CalculerAgeAssure(donnees.DatePreparation) < 25) &&
                            listeCodePlan.Contains(pa.Plan.CodePlan));

                        if (!trouve)
                        {
                            trouve = donnees.ProtectionsPDF.Any(p =>
                                p.DescriptiveCodeInfos.Any(desc => desc.Value.Contains("ModuleEnfantMaladieGrave1")));
                            if (!trouve)
                            {
                                result = false;
                            }
                        }
                        break;

                    case RegleConditionsMedicales.MaladieGravePreventionPlus:
                        // Prestation Prévention +est exclu pour Transition enfants et Maladie grave enfants. 
                        // Si on a une base Assurance Traditionnelle et un avenant Maladie grave enfants, il n'y aura pas la section Prestation Prévention +
                        result = donnees.ProtectionsPDF.Any(p =>
                            p.Specification.IsCriticalIllness && !p.Specification.IsCriticalIllnessChildModule);

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (!result) break;
            }

            return result;
        }

        private static bool ValiderCodeDescription(List<string> codes,
            DefinitionConditionMedicale conditionMedicale)
        {
            return conditionMedicale.CodesRegle == null ||
                   !conditionMedicale.CodesRegle.Any() ||
                   conditionMedicale.CodesRegle.Any(codes.Contains);
        }

        private static bool ValiderRegleDescriptionItem(DefinitionTableauItem definition,
            DonneesRapportIllustration donnees)
        {
            if (definition?.Regles == null)
            {
                return true;
            }

            var result = true;
            foreach (var item in definition.Regles)
            {
                switch (item)
                {
                    case RegleTableauItem.Aucune:
                        break;

                    case RegleTableauItem.SansAssureMoins25AnsSansAvenant:
                        var listeCodePlan = donnees.ProtectionsPDF.Where(p => p.Specification != null &&
                                                                              p.Specification
                                                                                  .IsCriticalIllnessChildModule)
                            .Select(p => p.CodePlan).ToList();
                        result = (donnees.Protections.ProtectionsAssures.Any(pa =>
                                     pa.Assures.Any(a => a.CalculerAgeAssure(donnees.DatePreparation) < 25) &&
                                     listeCodePlan.Contains(pa.Plan.CodePlan))) ||
                                 (donnees.ProtectionsPDF.Any(protPdf =>
                                     protPdf.DescriptiveCodeInfos.ContainsKey(CodeDescription) &&
                                     protPdf.DescriptiveCodeInfos.Any(
                                         desc => desc.Value.Contains(CodeModuleEnfantMaladieGrave))));

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (!result) break;
            }

            return result;
        }

        private static string FormatterLibellee(DefinitionConditionMedicale definition,
            DonneesRapportIllustration donnees)
        {
            if (definition == null) return string.Empty;
            return (donnees.Langue == Language.English ? definition.LibelleEn : definition.Libelle) ?? string.Empty;
        }

        private static string FormatterTexte(DefinitionConditionMedicale definition,
            DonneesRapportIllustration donnees)
        {
            if (definition == null) return string.Empty;
            return (donnees.Langue == Language.English ? definition.TexteEn : definition.Texte) ?? string.Empty ;
        }
    }
}