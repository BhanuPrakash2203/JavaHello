using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Illustration.Test.Configuration
{
    [TestClass]
    public class ConfigurationSectionTests
    {
        [TestMethod]
        public void ObtenirTitreTest()
        {
            using (new AssertionScope())
            {
                new ConfigurationSection().ObtenirTitre(Produit.Traditionnel, Language.English).Should().BeEmpty();

                new ConfigurationSection
                {
                    Titres = new Dictionary<Produit, Dictionary<Language, string>>
                    {
                        {Produit.Genesis, new Dictionary<Language, string> {{Language.English, "A Title Genesis"}}}
                    }
                }.ObtenirTitre(Produit.Traditionnel, Language.English).Should().BeEmpty();

                new ConfigurationSection
                {
                    Titre = new Dictionary<Language, string> { {Language.English, "A Title" } },
                    Titres = new Dictionary<Produit, Dictionary<Language, string>>
                    {
                        {Produit.Genesis, new Dictionary<Language, string> {{Language.English, "A Title Genesis" } }}
                    }
                }.ObtenirTitre(Produit.Traditionnel, Language.English).Should().Be("A Title");

                new ConfigurationSection
                {
                    Titre = new Dictionary<Language, string> { { Language.English, "A Title" } },
                    Titres = new Dictionary<Produit, Dictionary<Language, string>>
                    {
                        {Produit.Traditionnel, new Dictionary<Language, string> {{Language.English, "A Title Genesis" } }}
                    }
                }.ObtenirTitre(Produit.Traditionnel, Language.French).Should().Be("A Title");

                new ConfigurationSection
                {
                    Titres = new Dictionary<Produit, Dictionary<Language, string>>
                    {
                        {
                            Produit.Traditionnel,
                            new Dictionary<Language, string> {{Language.English, "A Title Traditionnel"}}
                        }
                    }
                }.ObtenirTitre(Produit.Traditionnel, Language.English).Should().Be("A Title Traditionnel");
            }
        }

        [TestMethod]
        public void ObtenirTitreSansLangueTest()
        {
            using (new AssertionScope())
            {
                new ConfigurationSection().ObtenirTitre(Produit.Traditionnel).Should().BeNull();

                new ConfigurationSection
                {
                    Titres = new Dictionary<Produit, Dictionary<Language, string>>
                    {
                        {Produit.Genesis, new Dictionary<Language, string> {{Language.English, "A Title Genesis"}}}
                    }
                }.ObtenirTitre(Produit.Traditionnel).Should().BeNull();

                new ConfigurationSection
                {
                    Titre = new Dictionary<Language, string> { { Language.English, "A Title" } },
                    Titres = new Dictionary<Produit, Dictionary<Language, string>>
                    {
                        {Produit.Genesis, new Dictionary<Language, string> {{Language.English, "A Title Genesis" } }}
                    }
                }.ObtenirTitre(Produit.Traditionnel).Should().BeEquivalentTo(new Dictionary<Language, string> { { Language.English, "A Title" } });

                new ConfigurationSection
                {
                    Titres = new Dictionary<Produit, Dictionary<Language, string>>
                    {
                        {
                            Produit.Traditionnel,
                            new Dictionary<Language, string> {{Language.English, "A Title Traditionnel"}}
                        }
                    }
                }.ObtenirTitre(Produit.Traditionnel).Should().BeEquivalentTo(new Dictionary<Language, string> { { Language.English, "A Title Traditionnel" } });
            }
        }
    }
}
