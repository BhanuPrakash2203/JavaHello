using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions.DescriptionsProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.DescriptionsProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories
{
    public class DescriptionsProtectionsModelFactory : IDescriptionsProtectionsModelFactory
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ISectionModelMapper _sectionModelMapper;

        public DescriptionsProtectionsModelFactory(IConfigurationRepository configurationRepository,
            ISectionModelMapper sectionModelMapper)
        {
            _configurationRepository = configurationRepository;
            _sectionModelMapper = sectionModelMapper;
        }

        public SectionDescriptionsProtectionsModel Build(string sectionId, DonneesRapportIllustration donnees,
            IReportContext context)
        {
            var definitionSection =
                _configurationRepository.ObtenirDefinitionSection<DefinitionSectionDescriptionsProtections>(
                    sectionId, donnees.Produit, FusionnerDefinitions);

            var descriptiveCodes = new List<string>();
            const string codeDescription = "CodeDescription";
            foreach (var protPdf in donnees.ProtectionsPDF)
            {
                if (protPdf.DescriptiveCodeInfos.ContainsKey(codeDescription))
                {
                    descriptiveCodes.AddRange(protPdf.DescriptiveCodeInfos[codeDescription]);
                }
            }

            var model = new SectionDescriptionsProtectionsModel();
            _sectionModelMapper.MapperDefinition(model, definitionSection, donnees, context);
            model.Details = CreerDetailDescriptions(definitionSection, donnees, descriptiveCodes);
            return model;
        }

        private static DefinitionSectionDescriptionsProtections FusionnerDefinitions(
            DefinitionSectionDescriptionsProtections definitionBase,
            DefinitionSectionDescriptionsProtections definition)
        {
            if (definitionBase == null)
            {
                return definition;
            }

            if (definitionBase.Notes != null)
            {
                if (definition.Notes == null) definition.Notes = new List<DefinitionNote>();
                definition.Notes.AddRange(definitionBase.Notes);
            }

            if (definitionBase.Avis != null)
            {
                if (definition.Avis == null) definition.Avis = new List<DefinitionAvis>();
                definition.Avis.AddRange(definitionBase.Avis);
            }

            if (definition.Descriptions == null) definition.Descriptions = new List<DefinitionDescriptions>();
            definition.Descriptions.AddRange(definitionBase.Descriptions);

            return definition;
        }

        internal static IList<DescriptionProtection> CreerDetailDescriptions(
            DefinitionSectionDescriptionsProtections definition, DonneesRapportIllustration donnees,
            IList<string> codes)
        {
            var result = new List<DescriptionProtection>();
            if (definition.Descriptions == null) return result;

            var textes = definition.Descriptions.OrderBy(t => t.SequenceId);
            result.AddRange(from texte in textes
                where ValiderCodeDescription(codes, texte, donnees.Produit) 
                select new DescriptionProtection
                {
                    Libelle = FormatterLibellee(texte, donnees),
                    Texte = FormatterTexte(texte, donnees),
                    SequenceId = texte.SequenceId,
                    SectionPrincipale = texte.SectionPrincipale,
                    SautPage = texte.SautPage,
                    Textes = CreerTextes(texte, donnees),
                    Tableau = CreerTableau(texte, donnees)
                });

            return result;
        }

        private static List<TexteItem> CreerTextes(DefinitionDescriptions definition,
            DonneesRapportIllustration donnees)
        {
            if (definition.Textes == null)
            {
                return new List<TexteItem>();
            }

            return donnees.Langue == Language.English
                ? definition.Textes
                    .Select(x => new TexteItem {Texte = x.TexteEn, SequenceId = x.SequenceId}).ToList()
                : definition.Textes
                    .Select(x => new TexteItem {Texte = x.Texte, SequenceId = x.SequenceId}).ToList();
        }

        private static List<TableauItem> CreerTableau(DefinitionDescriptions definition,
            DonneesRapportIllustration donnees)
        {
            if (definition.Tableau == null)
            {
                return new List<TableauItem>();
            }

            return donnees.Langue == Language.English
                ? definition.Tableau
                    .Select(x => new TableauItem {Categorie = x.CategorieEn, Texte = x.TexteEn}).ToList()
                : definition.Tableau
                    .Select(x => new TableauItem {Categorie = x.Categorie, Texte = x.Texte}).ToList();
        }


        private static string FormatterLibellee(DefinitionDescriptions definition,
            DonneesRapportIllustration donnees)
        {
            if (definition == null) return string.Empty;
            return (donnees.Langue == Language.English ? definition.LibelleEn : definition.Libelle) ?? string.Empty;
        }

        private static string FormatterTexte(DefinitionDescriptions definition,
            DonneesRapportIllustration donnees)
        {
            if (definition == null) return string.Empty;
            return (donnees.Langue == Language.English ? definition.TexteEn : definition.Texte) ?? string.Empty;
        }

        private static bool ValiderCodeDescription(ICollection<string> codes,
            DefinitionDescriptions definition, Produit produit)
        {
            if (definition.CodesRegle != null &&
                definition.CodesRegle.Any() &&
                !definition.CodesRegle.Any(x => x.All(codes.Contains)))
            {
                return false;
            }
                       
            return definition.RegleProduits.EstProduitValide(produit);
        }
    }
}