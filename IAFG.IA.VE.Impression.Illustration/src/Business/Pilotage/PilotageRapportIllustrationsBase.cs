using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Parameters;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using Newtonsoft.Json;

namespace IAFG.IA.VE.Impression.Illustration.Business.Pilotage
{
    public abstract class PilotageRapportIllustrationsBase : IPilotageRapportIllustrations
    {
        private string _path;

        private readonly Dictionary<Produit, List<IDefinitionSection>> _definitionSections =
            new Dictionary<Produit, List<IDefinitionSection>>();

        public List<ConfigurationRapport> ConfigurationRapports { get; internal set; }
        public List<ConfigurationSection> ConfigurationSections { get; internal set; }
        public Ressources Ressources { get; internal set; }

        public T ObtenirDefinitionSection<T>(string sectionId, Produit produit) where T : class, IDefinitionSection
        {
            return ObtenirDefinitionSection<T>(sectionId, produit, null);
        }

        public T ObtenirDefinitionSection<T>(string sectionId, Produit produit, Func<T, T, T> fusionnerDefinitions)
            where T : class, IDefinitionSection
        {
            if (!_definitionSections.ContainsKey(produit))
            {
                _definitionSections.Add(produit, new List<IDefinitionSection>());
            }

            var definitionSection = _definitionSections[produit].FirstOrDefault(x => x.SectionId == sectionId);
            if (definitionSection != null)
            {
                return (T)definitionSection;
            }

            var configurationSection = ConfigurationSections.First(s => s.SectionId == sectionId);
            var contenu = LireFichier(_path, configurationSection.ObtenirFichierDefinition(produit));
            var definition = JsonConvert.DeserializeObject<T>(contenu, new Newtonsoft.Json.Converters.StringEnumConverter());

            if (!string.IsNullOrWhiteSpace(configurationSection.FichierDefinitionBase) && fusionnerDefinitions != null)
            {
                var contenuBase = LireFichier(_path, configurationSection.FichierDefinitionBase);
                var definitionBase = JsonConvert.DeserializeObject<T>(contenuBase, new Newtonsoft.Json.Converters.StringEnumConverter());
                definition = fusionnerDefinitions(definitionBase, definition);
            }

            definition.SectionId = sectionId;
            _definitionSections[produit].Add(definition);
            return definition;
        }

        public void Initialize(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            _path = path;

            var definition = new {Version = "", Fichiers = new List<string>()};
            var configuration =
                JsonConvert.DeserializeAnonymousType(LireFichier(_path, "ConfigurationRapports.json"), definition);
            ConfigurationRapports = new List<ConfigurationRapport>();
            foreach (var filename in configuration.Fichiers)
            {
                ConfigurationRapports.AddRange(DeserializeConfigurationRapport(filename));
            }

            if (!string.IsNullOrEmpty(configuration.Version))
            {
                foreach (var config in ConfigurationRapports.Where(x => string.IsNullOrEmpty(x.Version)))
                {
                    config.Version = configuration.Version;
                }
            }

            ConfigurationSections = LireConfigurationSections();
            Ressources = LireRessources();
        }

        private Ressources LireRessources()
        {
            return JsonConvert.DeserializeObject<Ressources>(LireFichier(_path, "Ressources.json"),
                            new Newtonsoft.Json.Converters.StringEnumConverter());
        }

        private List<ConfigurationSection> LireConfigurationSections()
        {
            var liste = JsonConvert.DeserializeObject<List<ConfigurationSection>>(
                LireFichier(_path, "Principal\\ConfigurationSections.json"), 
                new Newtonsoft.Json.Converters.StringEnumConverter());
            
            var rapportCommission = JsonConvert.DeserializeObject<List<ConfigurationSection>>(
                LireFichier(_path, "RapportCommission\\ConfigurationSections.json"), 
                new Newtonsoft.Json.Converters.StringEnumConverter());

            var bonSuccessoral = JsonConvert.DeserializeObject<List<ConfigurationSection>>(
                LireFichier(_path, "BonSuccessoral\\ConfigurationSections.json"), 
                new Newtonsoft.Json.Converters.StringEnumConverter());

            liste.AddRange(rapportCommission);
            liste.AddRange(bonSuccessoral);

            return liste;
        }

        private IEnumerable<ConfigurationRapport> DeserializeConfigurationRapport(string item)
        {
            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Converters = new JsonConverter[] { new Newtonsoft.Json.Converters.StringEnumConverter() }
            };

            return JsonConvert.DeserializeObject<List<ConfigurationRapport>>(LireFichier(_path, item), settings);
        }

        protected abstract string LireFichier(string path, string filename);
    }
}