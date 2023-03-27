using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories
{
    public class GlossaireModelFactory: IGlossaireModelFactory
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IIllustrationReportDataFormatter _formatter;
        private readonly ISectionModelMapper _sectionModelMapper;

        public GlossaireModelFactory(
            IConfigurationRepository configurationRepository, 
            IIllustrationReportDataFormatter formatter, ISectionModelMapper sectionModelMapper)
        {
            _configurationRepository = configurationRepository;
            _formatter = formatter;
            _sectionModelMapper = sectionModelMapper;
        }

        public SectionGlossaireModel Build(string sectionId, DonneesRapportIllustration donnees, IReportContext context)
        {
            var definitionSection = _configurationRepository.ObtenirDefinitionSection<DefinitionSectionGlossaire>(sectionId, donnees.Produit);
            var planInfos = donnees.ObtenirPlanInfos().Where(x => !string.IsNullOrEmpty(x.CodeGlossaire)).ToArray();
            var charSeparators = new[] { ',', ';' };
            var codes = new List<string>();
            foreach (var item in planInfos)
            {
                codes.AddRange(item.CodeGlossaire.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries));
            }

            var model = new SectionGlossaireModel();
            _sectionModelMapper.MapperDefinition(model, definitionSection, donnees, context);
            model.Details = CreerDetailGlossaire(definitionSection, donnees, codes.GroupBy(x => x.Trim().ToUpper()).Select(g => g.First()).ToArray());
            return model;
        }

        internal List<DetailGlossaire> CreerDetailGlossaire(DefinitionSectionGlossaire definition, DonneesRapportIllustration donnees, string[] codes)
        {
            var result = new List<DetailGlossaire>();
            if (definition.GlossaireTextes == null) return result;

            if (definition.Codes != null && definition.Codes.Any())
            {
                foreach (var itemCode in definition.Codes
                    .Where(c => codes.Contains(c.Code.Trim().ToUpper()))
                    .OrderBy(c => c.SequenceId).ToArray())
                {
                    var textes =
                        definition.GlossaireTextes
                        .Where(
                            t => t.Codes == null || 
                            !t.Codes.Any() || 
                            t.Codes.Select(x => x.Trim().ToUpper()).Contains(itemCode.Code.Trim().ToUpper()))
                        .OrderBy(t => t.SequenceId)
                        .ToArray();

                    result.AddRange(from item in textes.OrderBy(t => t.SequenceId).ToArray()
                                    where EstVisible(item.Regles, donnees)
                                    select new DetailGlossaire
                                    {
                                        CodeGlossaire = itemCode.Code.Trim().ToUpper(),
                                        CodeSequenceId = itemCode.SequenceId,
                                        Titre = _formatter.FormatterTitre(itemCode.Titres, donnees),
                                        Libelle = _formatter.FormatterLibellee(item, donnees),
                                        Texte = _formatter.FormatterTexte(item, donnees),
                                        SequenceId = item.SequenceId
                                    });
                }
            }
            else
            {
                var textes = definition.GlossaireTextes
                    .Where(t => t.Codes == null || !t.Codes.Any() || t.Codes.Select(x => x.Trim().ToUpper()).Intersect(codes).Any())
                    .OrderBy(t => t.SequenceId).ToArray();
                
                result.AddRange(from item in textes
                                where EstVisible(item.Regles, donnees)
                                select new DetailGlossaire
                                {
                                    Libelle = _formatter.FormatterLibellee(item, donnees),
                                    Texte = _formatter.FormatterTexte(item, donnees),
                                    SequenceId = item.SequenceId
                                });
            }

            return result;
        }

        internal bool EstVisible(List<RegleGlossaire[]> regles, DonneesRapportIllustration donnees)
        {
            return regles == null || regles.Any(r => EstVisible(r, donnees));
        }

        internal bool EstVisible(IEnumerable<RegleGlossaire> regles, DonneesRapportIllustration donnees)
        {
            if (regles == null) return true;

            var result = true;
            foreach (var item in regles)
            {
                switch (item)
                {
                    case RegleGlossaire.Aucune:
                        break;
                    case RegleGlossaire.CompteTerme:
                        result = result && donnees.PresenceCompteTerme;
                        break;
                    case RegleGlossaire.EstCapitalPlusFonds:
                        result = result && (donnees.Protections.PrestationDeces == OptionPrestationDeces.CapitalPlusFonds);
                        break;
                    case RegleGlossaire.EstCapitalPlusFondsOptionVMax:
                        result = result && (donnees.Protections.PrestationDeces == OptionPrestationDeces.CapitalPlusFondsValMax);
                        break;
                    case RegleGlossaire.EstCapitalPlusFondsPlusCBR:
                        result = result && (donnees.Protections.PrestationDeces == OptionPrestationDeces.CapitalPlusFondsPlusRemboursementCBR);
                        break;
                    case RegleGlossaire.EstCapitalPlusRetourPrimes:
                        result = result && (donnees.Protections.PrestationDeces == OptionPrestationDeces.CapitalPlusRemboursementPrimeGarantie);
                        break;
                    case RegleGlossaire.EstCapitalSeul:
                        result = result && (donnees.Protections.PrestationDeces == OptionPrestationDeces.Capital);
                        break;
                    case RegleGlossaire.EstValeurMaximisee:
                        result = result && (donnees.Protections.PrestationDeces == OptionPrestationDeces.ValeurMaximisee);
                        break;
                    case RegleGlossaire.OACAEstActif:
                        result = result && (donnees.Protections.StatutOacaActif ?? false);
                        break;
                    case RegleGlossaire.BoniInteretGaranti:
                        result = result && (donnees.Boni.ChoixBoniInteret == ChoixBoniInteret.Garanti);
                        break;
                    case RegleGlossaire.BoniInteretRendement:
                        result = result && (donnees.Boni.ChoixBoniInteret == ChoixBoniInteret.Rendement);
                        break;
                    case RegleGlossaire.BoniInteretVariable:
                        result = result && (donnees.Boni.ChoixBoniInteret == ChoixBoniInteret.Variable);
                        break;
                    case RegleGlossaire.BoniFideliteInvestissement:
                        result = result && (donnees.Boni.BoniFidelite == BoniFidelite.Regle7);
                        break;
                    case RegleGlossaire.EstDeductionCNAP:
                        result = result && (donnees.ConceptVente?.PretEnCollateral?.Data?.Taxation?.Borrower?.NetCostPureInsuranceDeduction ?? false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return result;
        }
    }
}