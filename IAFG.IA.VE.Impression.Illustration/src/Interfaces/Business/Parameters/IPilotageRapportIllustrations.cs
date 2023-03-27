using System;
using System.Collections.Generic;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Parameters
{
    public interface IPilotageRapportIllustrations
    {
        List<ConfigurationRapport> ConfigurationRapports { get; }
        List<ConfigurationSection> ConfigurationSections { get;}
        Ressources Ressources { get; }
        T ObtenirDefinitionSection<T>(string sectionId, Produit produit) where T : class, IDefinitionSection;
        T ObtenirDefinitionSection<T>(string sectionId, Produit produit, Func<T, T, T> fusionnerDefinitions) where T : class, IDefinitionSection;
        void Initialize(string path);
    }
}