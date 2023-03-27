using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Parameters;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;

namespace IAFG.IA.VE.Impression.Illustration.Business.Configuration
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly IPilotageRapportIllustrations _pilotageRapportIllustrations;

        public Language Language { get; set; }

        public ConfigurationRepository(IPilotageRapportIllustrations pilotageRapportIllustrations)
        {
            _pilotageRapportIllustrations = pilotageRapportIllustrations;
        }

        public ConfigurationRapport ObtenirConfigurationRapport(Produit produit, Etat etat)
        {
            var config = _pilotageRapportIllustrations.ConfigurationRapports.FirstOrDefault(configuration => configuration.Produit == produit && configuration.Etat == etat);
            if (config == null) throw new ArgumentException(nameof(produit), $@"Aucune configuration de rapport n'est implémentée pour ce produit: {produit}.");
            return config;
        }

        public ConfigurationSection ObtenirConfigurationSection(string id, GroupeRapport? groupeRapport)
        {
            return groupeRapport.HasValue 
                ? _pilotageRapportIllustrations?.ConfigurationSections?.FirstOrDefault(s => s.SectionId == id && s.GroupeRapport == groupeRapport.GetValueOrDefault())
                : _pilotageRapportIllustrations?.ConfigurationSections?.FirstOrDefault(s => s.SectionId == id);
        }

        public ConfigurationSection[] ObtenirConfigurationSections(GroupeRapport groupeRapport)
        {
            return _pilotageRapportIllustrations?.ConfigurationSections?.Where(x => x.GroupeRapport == groupeRapport).ToArray() ?? new ConfigurationSection[] { };
        }

        public T ObtenirDefinitionSection<T>(string sectionId, Produit produit) where T : class, IDefinitionSection
        {
            var definition = _pilotageRapportIllustrations.ObtenirDefinitionSection<T>(sectionId, produit);
            if (definition == null)
            {
                throw new ArgumentException(nameof(sectionId), $@"Aucune configuration de section n'est implémentée: {sectionId}.");
            }

            return definition;
        }

        public T ObtenirDefinitionSection<T>(string sectionId, Produit produit, Func<T, T, T> fusionnerDefinitions) where T : class, IDefinitionSection
        {
            var definition = _pilotageRapportIllustrations.ObtenirDefinitionSection(sectionId, produit, fusionnerDefinitions);
            if (definition == null)
            {
                throw new ArgumentException(nameof(sectionId), $@"Aucune configuration de section n'est implémentée: {sectionId}.");
            }

            return definition;
        }

        public string ObtenirNomProduit(Produit produit)
        {
            var p = _pilotageRapportIllustrations?.Ressources?.Produits?.FirstOrDefault(x => x.Key == produit).Value;
            return p?.Libelles == null ? produit.ToString() : (Language == Language.French ? p.Libelles.Libelle : p.Libelles.LibelleEn);
        }

        public string ObtenirLibelleEnum<T>(string valeur)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new ArgumentException($@"Ce type {nameof(T)} n'est pas une énumération!", nameof(T));
            var enums = _pilotageRapportIllustrations?.Ressources?.Enums?.FirstOrDefault(e => e.Key == type.Name).Value;
            return enums == null || !enums.ContainsKey(valeur) ? valeur : (Language == Language.French ? enums[valeur].Libelle : enums[valeur].LibelleEn);
        }

        public string ObtenirLibelleRessource(string nomRessource, string valeur)
        {
            var ressource = _pilotageRapportIllustrations?.Ressources?.Enums?.FirstOrDefault(e => e.Key == nomRessource).Value;
            if (ressource == null) return valeur;
            var valeurRessource = ressource.FirstOrDefault(v => v.Key == valeur).Value;
            return valeurRessource == null ? valeur : (Language == Language.French ? valeurRessource.Libelle : valeurRessource.LibelleEn);
        }

        public string ObtenirLibelleNoteEsperanceVie(NoteEsperanceVie valeur)
        {
            var note = _pilotageRapportIllustrations?.Ressources?.NotesEsperanceVie?.FirstOrDefault(n => n.Key == valeur).Value;
            return note == null ? string.Empty : Language == Language.French ? note.Libelle : note.LibelleEn;
        }

        public string ObtenirLibelleProvince(string valeur)
        {
            return ObtenirLibelleRessource("Province", valeur);
        }

        public IList<TauxIndiceReferenceBoni> ObtenirTauxIndiceReferenceBoni()
        {
            if (_pilotageRapportIllustrations?.Ressources?.TauxIndiceReferenceBonis == null) return new List<TauxIndiceReferenceBoni>();
            var result = new List<TauxIndiceReferenceBoni>();
            result.AddRange(_pilotageRapportIllustrations.Ressources.TauxIndiceReferenceBonis);
            return result;
        }
    }
}