using System.Collections.Generic;
using FluentAssertions;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Extensions
{
    [TestClass]
    public class DonneesRapportExtensionTests
    {
        [TestMethod]
        public void HasPreferentialStatus_WhenNullProtection_ThenFalse()
        {
            var donnees = new DonneesRapportIllustration();

            donnees.HasPreferentialStatus().Should().BeFalse();
        }

        [TestMethod]
        public void HasPreferentialStatus_WhenNullProtectionsAssures_ThenFalse()
        {
            var donnees = new DonneesRapportIllustration
            {
                Protections = new Protections()
            };

            donnees.HasPreferentialStatus().Should().BeFalse();
        }

        [TestMethod]
        public void HasPreferentialStatus_WhenEmptyProtectionsAssures_ThenFalse()
        {
            var donnees = new DonneesRapportIllustration
            {
                Protections = new Protections
                {
                    ProtectionsAssures = new List<Types.Models.SommaireProtections.Protection>()
                }
            };

            donnees.HasPreferentialStatus().Should().BeFalse();
        }

        [DataRow(StatutTabagisme.Fumeur)]
        [DataRow(StatutTabagisme.NonApplicable)]
        [DataRow(StatutTabagisme.NonDefini)]
        [DataRow(StatutTabagisme.NonFumeur)]
        [DataTestMethod]
        public void HasPreferentialStatus_WhenIndividuelleAndNotPreferentiel_ThenFalse(StatutTabagisme statutTabagisme)
        {
            var donnees = new DonneesRapportIllustration
            {
                Protections = new Protections
                {
                    ProtectionsAssures = new List<Types.Models.SommaireProtections.Protection>
                    {
                        new Protection
                        {
                            TypeAssurance = TypeAssurance.Individuelle,
                            StatutTabagisme = statutTabagisme
                        }
                    }
                }
            };

            donnees.HasPreferentialStatus().Should().BeFalse();
        }

        [DataRow(StatutTabagisme.FumeurElite)]
        [DataRow(StatutTabagisme.FumeurPrivilege)]
        [DataRow(StatutTabagisme.NonFumeurElite)]
        [DataRow(StatutTabagisme.NonFumeurPrivilege)]
        [DataTestMethod]
        public void HasPreferentialStatus_WhenIndividuelleAndPreferentiel_ThenTrue(StatutTabagisme statutTabagisme)
        {
            var donnees = new DonneesRapportIllustration
            {
                Protections = new Protections
                {
                    ProtectionsAssures = new List<Types.Models.SommaireProtections.Protection>
                    {
                        new Protection
                        {
                            TypeAssurance = TypeAssurance.Individuelle,
                            StatutTabagisme = statutTabagisme
                        }
                    }
                }
            };

            donnees.HasPreferentialStatus().Should().BeTrue();
        }

        [DataRow(StatutTabagisme.Fumeur)]
        [DataRow(StatutTabagisme.NonApplicable)]
        [DataRow(StatutTabagisme.NonDefini)]
        [DataRow(StatutTabagisme.NonFumeur)]
        [DataTestMethod]
        public void HasPreferentialStatus_WhenNotIndividuelleAndNotPreferentiel_ThenFalse(StatutTabagisme statutTabagisme)
        {
            var donnees = new DonneesRapportIllustration
            {
                Protections = new Protections
                {
                    ProtectionsAssures = new List<Types.Models.SommaireProtections.Protection>
                    {
                        new Protection
                        {
                            TypeAssurance = TypeAssurance.Conjoint,
                            StatutTabagisme = StatutTabagisme.NonFumeur,
                            Assures = new[]
                            {
                                new Assure { StatutTabagisme = StatutTabagisme.NonFumeur },
                                new Assure { StatutTabagisme = statutTabagisme }
                            }
                        }
                    }
                }
            };

            donnees.HasPreferentialStatus().Should().BeFalse();
        }

        [DataRow(StatutTabagisme.FumeurElite)]
        [DataRow(StatutTabagisme.FumeurPrivilege)]
        [DataRow(StatutTabagisme.NonFumeurElite)]
        [DataRow(StatutTabagisme.NonFumeurPrivilege)]
        [DataTestMethod]
        public void HasPreferentialStatus_WhenNotIndividuelleAndPreferentiel_ThenTrue(StatutTabagisme statutTabagisme)
        {
            var donnees = new DonneesRapportIllustration
            {
                Protections = new Protections
                {
                    ProtectionsAssures = new List<Types.Models.SommaireProtections.Protection>
                    {
                        new Protection
                        {
                            TypeAssurance = TypeAssurance.Conjoint,
                            StatutTabagisme = StatutTabagisme.NonFumeur,
                            Assures = new[]
                            {
                                new Assure { StatutTabagisme = StatutTabagisme.NonFumeur },
                                new Assure { StatutTabagisme = statutTabagisme }
                            }
                        }
                    }
                }
            };

            donnees.HasPreferentialStatus().Should().BeTrue();
        }

        [TestMethod]
        public void HasMVAAccount_WhenFondsVehiculesHasAccountType_Average5Years_AND_SubType_LisseAccountIris_Rule6_ShouldBeTrue()
        {
            var donnees = new DonneesRapportIllustration()
            {
                Vehicules = new List<Vehicule>()
                {
                    new Vehicule() {Vehicle = "SRA081"}
                },
                FondsInvestissement = new List<Fonds>()
                {
                    new Fonds() {AccountType = "Average5Years", SubType = "LisseAccountIris_Rule6", Vehicule = "SRA081"}
                }
            };

            donnees.IsMarketValueAdjustmentApplied().Should().BeTrue();
        }

        [TestMethod]
        public void HasMVAAccount_WhenFondsVehiculesHasAccountType_Average5Years_AND_SubType_SRIAAccount_ShouldBeTrue()
        {
            var donnees = new DonneesRapportIllustration()
            {
                Vehicules = new List<Vehicule>()
                {
                    new Vehicule() {Vehicle = "SRD080"}
                },
                FondsInvestissement = new List<Fonds>()
                {
                    new Fonds() {AccountType = "Average5Years", SubType = "SRIAAccount", Vehicule = "SRD080"}
                }
            };

            donnees.IsMarketValueAdjustmentApplied().Should().BeTrue();
        }

        [TestMethod]
        public void HasMVAAccount_WhenFondsVehiculesHasAccountType_Average5Years_AND_SubType_LisseAccount_Rule5_ShouldBeTrue()
        {
            var donnees = new DonneesRapportIllustration()
            {
                Vehicules = new List<Vehicule>()
                {
                    new Vehicule() {Vehicle = "CPV970"}
                },
                FondsInvestissement = new List<Fonds>()
                {
                    new Fonds() {AccountType = "Average5Years", SubType = "LisseAccount_Rule5", Vehicule = "CPV970"}
                }
            };

            donnees.IsMarketValueAdjustmentApplied().Should().BeTrue();
        }

        [TestMethod]
        public void HasMVAAccount_WhenFondsVehiculesHasNotAccountType_AND_SubType_ShouldBeFalse()
        {
            var donnees = new DonneesRapportIllustration()
            {
                Vehicules = new List<Vehicule>()
                {
                    new Vehicule() {Vehicle = "CPV970"}
                },
                FondsInvestissement = new List<Fonds>()
                {
                    new Fonds() {AccountType = "AccountType", SubType = "SubType", Vehicule = "CPV970"}
                }
            };

            donnees.IsMarketValueAdjustmentApplied().Should().BeFalse();
        }
    }
}
