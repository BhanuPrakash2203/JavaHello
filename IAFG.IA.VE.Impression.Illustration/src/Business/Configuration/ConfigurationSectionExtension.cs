using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Business.Configuration
{
    public static class ConfigurationSectionExtension
    {
        public static string ObtenirFichierDefinition(this ConfigurationSection configuration, Produit produit)
        {
            if (configuration.FichierDefinitions != null && configuration.FichierDefinitions.ContainsKey(produit))
            {
                return configuration.FichierDefinitions[produit];
            }

            return configuration.FichierDefinition;
        }

        public static string ObtenirTitre(this ConfigurationSection configuration, Produit produit, Language language)
        {
            var result = string.Empty;
            if (configuration.Titre != null)
            {
                result = configuration.Titre.ContainsKey(language) ? configuration.Titre[language] : configuration.Titre.FirstOrDefault().Value;
            }

            if (configuration.Titres == null) return result;
            if (!configuration.Titres.ContainsKey(produit)) return result;
            return configuration.Titres[produit].ContainsKey(language) ? configuration.Titres[produit][language] : result;
        }

        public static Dictionary<Language, string> ObtenirTitre(this ConfigurationSection configuration, Produit produit)
        {
            if (configuration.Titres != null && configuration.Titres.ContainsKey(produit)) return configuration.Titres[produit];
            return configuration.Titre;
        }
    }
}