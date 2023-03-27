using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public class DefinitionTitreManager : IDefinitionTitreManager
    {
        private readonly IIllustrationReportDataFormatter _formatter;

        public DefinitionTitreManager(IIllustrationReportDataFormatter formatter)
        {
            _formatter = formatter;
        }

        public string ObtenirDescription(List<DefinitionTitreDescriptionSelonProduit> definitions, DonneesRapportIllustration donnees)
        {
            if (definitions == null || !definitions.Any())
            {
                return string.Empty;
            }

            var titres =
                (definitions.FirstOrDefault(t => t.Produit == donnees.Produit) ??
                 definitions.FirstOrDefault(t => t.Produit == Produit.NonDefini)) ?? definitions.First();
            return _formatter.FormatterDescription(titres, donnees);
        }

        public string ObtenirTitre(List<DefinitionTitreDescriptionSelonProduit> definitions, DonneesRapportIllustration donnees, string[] parametres = null)
        {
            if (definitions == null || !definitions.Any())
            {
                return string.Empty;
            }

            var titres =
                (definitions.FirstOrDefault(t =>
                     t.Produit == donnees.Produit && EstValide(t.TitreRegles, donnees)) ??
                 definitions.FirstOrDefault(t =>
                     t.Produit == Produit.NonDefini && EstValide(t.TitreRegles, donnees))) ??
                definitions.First();

            return _formatter.FormatterTitre(titres, donnees, parametres);
        }

        private static bool EstValide(List<RegleTitre[]> regles, DonneesRapportIllustration donnees)
        {
            return regles == null || !regles.Any() || regles.Any(r => EstValide(r, donnees));
        }

        private static bool EstValide(IEnumerable<RegleTitre> regles, DonneesRapportIllustration donnees)
        {
            if (regles == null)
            {
                return true;
            }

            var result = true;
            foreach (var item in regles)
            {
                switch (item)
                {
                    case RegleTitre.Aucune:
                        break;
                    case RegleTitre.ContractantUnique:
                        result = (donnees.Clients?.Count(x => x.EstContractant)).GetValueOrDefault() <= 1;
                        break;
                    case RegleTitre.ContractantMultiple:
                        result = (donnees.Clients?.Count(x => x.EstContractant)).GetValueOrDefault() > 1;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(regles));
                }

                if (!result)
                {
                    return false;
                }
            }

            return true;
        }
    }
}