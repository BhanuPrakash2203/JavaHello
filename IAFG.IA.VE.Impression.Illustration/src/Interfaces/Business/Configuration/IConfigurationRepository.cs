using System;
using System.Collections.Generic;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration
{
    public interface IConfigurationRepository
    {
        ConfigurationRapport ObtenirConfigurationRapport(Produit produit, Etat etat);
        ConfigurationSection ObtenirConfigurationSection(string id, GroupeRapport? groupeRapport);
        ConfigurationSection[] ObtenirConfigurationSections(GroupeRapport groupeRapport);
        T ObtenirDefinitionSection<T>(string sectionId, Produit produit) where T : class, IDefinitionSection;
        T ObtenirDefinitionSection<T>(string sectionId, Produit produit, Func<T, T, T> fusionnerDefinitions) where T : class, IDefinitionSection;
        string ObtenirNomProduit(Produit produit);
        string ObtenirLibelleEnum<T>(string valeur);
        string ObtenirLibelleRessource(string nomRessource, string valeur);
        string ObtenirLibelleNoteEsperanceVie(NoteEsperanceVie valeur);
        string ObtenirLibelleProvince(string valeur);
        IList<TauxIndiceReferenceBoni> ObtenirTauxIndiceReferenceBoni();
        Language Language { get; set; }
    }
}