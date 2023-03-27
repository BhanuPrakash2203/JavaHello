using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.RelevantBuilder;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ConceptVentes;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test
{
    [TestClass]
    public class PageRelevancyAnalyzerTest
    {
        private static readonly IFixture Fixture = AutoFixtureFactory.Create();

        [TestMethod]
        public void GIVEN_IsValid_AvecReglesNull_THEN_RetourneVrai()
        {
            var config = new ConfigurationSection { Regles = null };

            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);
            analyzer.IsValid("1", new DonneesRapportIllustration()).Should().BeTrue();
        }

        [TestMethod]
        public void GIVEN_IsValid_AvecReglesVide_THEN_RetourneVrai()
        {
            var config = new ConfigurationSection { Regles = new List<RegleSection[]>() };

            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);
            analyzer.IsValid("1", new DonneesRapportIllustration()).Should().BeTrue();
        }

        [TestMethod]
        public void GIVEN_IsValid_AvecRegleAucune_THEN_RetourneVrai()
        {
            var config = new ConfigurationSection { Regles = new List<RegleSection[]> { new[] { RegleSection.Aucune } } };

            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);
            analyzer.IsValid("1", new DonneesRapportIllustration()).Should().BeTrue();
        }

        [TestMethod]
        public void GIVEN_IsValid_AvecRegleContractantEstCompagnie_THEN_RetourneSelonValeurDonnees()
        {
            var config = new ConfigurationSection { Regles = new List<RegleSection[]> { new[] { RegleSection.ContractantEstCompagnie } } };

            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);

            using (new AssertionScope())
            {
                analyzer.IsValid("1", new DonneesRapportIllustration { ContractantEstCompagnie = true }).Should().BeTrue();
                analyzer.IsValid("1", new DonneesRapportIllustration { ContractantEstCompagnie = false }).Should().BeFalse();
            }
        }

        [TestMethod]
        public void GIVEN_IsValid_AvecRegleContractantEstIndividu_THEN_RetourneSelonValeurDonnees()
        {
            var config = new ConfigurationSection { Regles = new List<RegleSection[]> { new[] { RegleSection.ContractantEstIndividu } } };

            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);

            using (new AssertionScope())
            {
                analyzer.IsValid("1", new DonneesRapportIllustration { ContractantEstCompagnie = true }).Should().BeFalse();
                analyzer.IsValid("1", new DonneesRapportIllustration { ContractantEstCompagnie = false }).Should().BeTrue();
            }
        }

        [TestMethod]
        public void GIVEN_IsValid_AvecReglePasBannierePPI_THEN_RetourneSelonValeurDonnees()
        {
            var config = new ConfigurationSection { Regles = new List<RegleSection[]> { new[] { RegleSection.PasBannierePPIProduitCapitalValeur } } };

            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);

            using (new AssertionScope())
            {
                analyzer.IsValid("1", new DonneesRapportIllustration { Banniere = Banniere.PPI, Produit = Produit.Genesis }).Should().BeTrue();
                analyzer.IsValid("1", new DonneesRapportIllustration { Banniere = Banniere.IA, Produit = Produit.Genesis }).Should().BeTrue();
                analyzer.IsValid("1", new DonneesRapportIllustration { Banniere = Banniere.IA, Produit = Produit.CapitalValeur }).Should().BeTrue();
                analyzer.IsValid("1", new DonneesRapportIllustration { Banniere = Banniere.PPI, Produit = Produit.CapitalValeur }).Should().BeFalse();
            }
        }

        [TestMethod]
        public void GIVEN_IsValid_AvecReglePasBannierePPI_ET_ContractantEstCompagnie_THEN_RetourneSelonValeurDonnees()
        {
            var config = new ConfigurationSection { Regles = new List<RegleSection[]> { new[] { RegleSection.PasBannierePPIProduitCapitalValeur, RegleSection.ContractantEstCompagnie } } };

            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);

            using (new AssertionScope())
            {
                analyzer.IsValid("1", new DonneesRapportIllustration { Banniere = Banniere.IA, Produit = Produit.Genesis, ContractantEstCompagnie = true }).Should().BeTrue();
                analyzer.IsValid("1", new DonneesRapportIllustration { Banniere = Banniere.IA, Produit = Produit.Genesis, ContractantEstCompagnie = false }).Should().BeFalse();
                analyzer.IsValid("1", new DonneesRapportIllustration { Banniere = Banniere.PPI, Produit = Produit.CapitalValeur, ContractantEstCompagnie = true }).Should().BeFalse();
            }
        }

        [TestMethod]
        public void GIVEN_IsValid_AvecReglePasBannierePPI_OU_ContractantEstCompagnie_THEN_RetourneSelonValeurDonnees()
        {
            var config = new ConfigurationSection
            {
                Regles = new List<RegleSection[]>
                {
                    new[] { RegleSection.PasBannierePPIProduitCapitalValeur },
                    new[] { RegleSection.ContractantEstCompagnie }
                }
            };

            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);

            using (new AssertionScope())
            {
                analyzer.IsValid("1", new DonneesRapportIllustration { Banniere = Banniere.IA, Produit = Produit.Genesis, ContractantEstCompagnie = true }).Should().BeTrue();
                analyzer.IsValid("1", new DonneesRapportIllustration { Banniere = Banniere.IA, Produit = Produit.Genesis, ContractantEstCompagnie = false }).Should().BeTrue();
                analyzer.IsValid("1", new DonneesRapportIllustration { Banniere = Banniere.PPI, Produit = Produit.CapitalValeur, ContractantEstCompagnie = true }).Should().BeTrue();
                analyzer.IsValid("1", new DonneesRapportIllustration { Banniere = Banniere.PPI, Produit = Produit.CapitalValeur, ContractantEstCompagnie = false }).Should().BeFalse();
            }
        }

        [TestMethod]
        public void GIVEN_IsValid_AvecRegleConceptProprietePartagee_THEN_RetourneSelonValeurDonnees()
        {
            var config = new ConfigurationSection { Regles = new List<RegleSection[]> { new[] { RegleSection.ConceptProprietePartagee } } };

            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);

            using (new AssertionScope())
            {
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = null }).Should().BeFalse();
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = new ConceptVente() }).Should().BeFalse();
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = new ConceptVente { ProprietePartagee = true } }).Should().BeTrue();
            }
        }

        [TestMethod]
        public void GIVEN_IsValid_AvecRegleConceptAvancePret_THEN_RetourneSelonValeurDonnees()
        {
            var config = new ConfigurationSection { Regles = new List<RegleSection[]> { new[] { RegleSection.ConceptAvancePret } } };

            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);

            using (new AssertionScope())
            {
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = null }).Should().BeFalse();
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = new ConceptVente() }).Should().BeFalse();
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = new ConceptVente { AvancePret = new AvancePret() } }).Should().BeTrue();
            }
        }

        [TestMethod]
        public void GIVEN_IsValid_AvecRegleConceptPretEnCollateral_THEN_RetourneSelonValeurDonnees()
        {
            var config = new ConfigurationSection { Regles = new List<RegleSection[]> { new[] { RegleSection.ConceptAvancePret } } };

            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);

            using (new AssertionScope())
            {
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = null }).Should().BeFalse();
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = new ConceptVente() }).Should().BeFalse();
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = new ConceptVente { AvancePret = new AvancePret() } }).Should().BeTrue();
            }
        }

        [TestMethod]
        public void GIVEN_IsValid_AvecRegleConceptPretEnCollateralEmprunteurTiercePartie_THEN_RetourneSelonValeurDonnees()
        {
            var config = new ConfigurationSection { Regles = new List<RegleSection[]> { new[] { RegleSection.ConceptPretEnCollateralEmprunteurTiercePartie } } };

            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);

            var conceptTrue = Fixture.Create<ConceptVente>();
            conceptTrue.PretEnCollateral.Data.Taxation.Borrower.BorrowerType = IA.VI.Projection.Data.Enums.Concept.BorrowerType.ThirdParty;

            var conceptFalse = Fixture.Create<ConceptVente>();
            conceptFalse.PretEnCollateral.Data.Taxation.Borrower.BorrowerType = IA.VI.Projection.Data.Enums.Concept.BorrowerType.Applicant;

            using (new AssertionScope())
            {
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = null }).Should().BeFalse();
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = new ConceptVente() }).Should().BeFalse();
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = conceptFalse }).Should().BeFalse();
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = conceptTrue }).Should().BeTrue();
            }
        }

        [TestMethod]
        public void GIVEN_IsValid_AvecRegleConceptPretEnCollateralEmprunteurAutreQueTiercePartie_THEN_RetourneSelonValeurDonnees()
        {
            var config = new ConfigurationSection { Regles = new List<RegleSection[]> { new[] { RegleSection.ConceptPretEnCollateralEmprunteurAutreQueTiercePartie } } };

            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);

            var conceptFalse = Fixture.Create<ConceptVente>();
            conceptFalse.PretEnCollateral.Data.Taxation.Borrower.BorrowerType = IA.VI.Projection.Data.Enums.Concept.BorrowerType.ThirdParty;

            var conceptTrue = Fixture.Create<ConceptVente>();
            conceptTrue.PretEnCollateral.Data.Taxation.Borrower.BorrowerType = IA.VI.Projection.Data.Enums.Concept.BorrowerType.Applicant;

            using (new AssertionScope())
            {
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = null }).Should().BeFalse("ConceptVente = null");
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = new ConceptVente() }).Should().BeFalse("new ConceptVente()");
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = conceptFalse }).Should().BeFalse("conceptFalse");
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = conceptTrue }).Should().BeTrue("conceptTrue");
            }
        }

        [TestMethod]
        public void GIVEN_IsValid_AvecRegleIsSignaturePapierFalse_THEN_RetourneSelonValeurDonnees()
        {
            var config = new ConfigurationSection { Regles = new List<RegleSection[]> { new[] { RegleSection.IsSignaturePapier } } };
            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);

            using (new AssertionScope())
            {
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = null }).Should().BeFalse();
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = new ConceptVente() }).Should().BeFalse();
                analyzer.IsValid("1", new DonneesRapportIllustration { ConceptVente = new ConceptVente { AvancePret = new AvancePret() } }).Should().BeFalse();
            }
        }

        [TestMethod]
        public void GIVEN_IsValid_AvecRegleIsSignaturePapierTrue_THEN_RetourneSelonValeurDonnees()
        {
            var config = new ConfigurationSection { Regles = new List<RegleSection[]> { new[] { RegleSection.IsSignaturePapier } } };
            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);

            analyzer.IsValid("1", new DonneesRapportIllustration { Produit = Produit.Genesis, IsSignaturePapier = true }).Should().BeTrue();
        }

        [TestMethod]
        public void GIVEN_IsValidAvecReglePrimesRenouvellementPresente_THENRetourneSelonValeurDonnees()
        {
            var config = new ConfigurationSection { Regles = new List<RegleSection[]> { new[] { RegleSection.PrimesRenouvellementPresente } } };

            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);

            using (new AssertionScope())
            {
                analyzer.IsValid("1", new DonneesRapportIllustration { ProtectionsPDF = InitialiserProtectionPdfs(true) }).Should().BeTrue();
                analyzer.IsValid("1", new DonneesRapportIllustration { ProtectionsPDF = InitialiserProtectionPdfs(false) }).Should().BeFalse();
            }
        }

        [TestMethod]
        public void GIVEN_IsValidAvecRegleValeurRachatPresente_THENRetourneSelonValeurDonnees()
        {
            var config = new ConfigurationSection { Regles = new List<RegleSection[]> { new[] { RegleSection.ValeurRachatPresente } } };

            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);
            using (new AssertionScope())
            {
                analyzer.IsValid("1", new DonneesRapportIllustration { ProtectionsPDF = InitialiserCashValueAvailable(true) }).Should().BeTrue();
                analyzer.IsValid("1", new DonneesRapportIllustration { ProtectionsPDF = InitialiserCashValueAvailable(false) }).Should().BeFalse();
            }
        }

        [TestMethod]
        public void GIVEN_IsValidAvecRegleProtectionMaladieGravePresente_THENRetourneSelonValeurDonnees()
        {
            var config = new ConfigurationSection { Regles = new List<RegleSection[]> { new[] { RegleSection.ProtectionMaladieGravePresente } } };

            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);
            using (new AssertionScope())
            {
                analyzer.IsValid("1", new DonneesRapportIllustration { ProtectionsPDF = InitialiserIsCriticalIllness(true) }).Should().BeTrue();
                analyzer.IsValid("1", new DonneesRapportIllustration { ProtectionsPDF = InitialiserIsCriticalIllness(false) }).Should().BeFalse();
            }
        }

        [TestMethod]
        public void GIVEN_IsValidAvecRegleProfilInvestisseurElectroniqueCompletPresente_THENRetourneSelonValeurDonnees()
        {
            var config = new ConfigurationSection { Regles = new List<RegleSection[]> { new[] { RegleSection.ProfilInvestisseurElectroniqueComplet } } };

            var configurationRepository = Substitute.For<IConfigurationRepository>();
            configurationRepository.ObtenirConfigurationSection(Arg.Any<string>(), null).Returns(config);

            var analyzer = new PageRelevancyAnalyzer(configurationRepository);
            using (new AssertionScope())
            {
                analyzer.IsValid("1", new DonneesRapportIllustration { ProfilInvestisseurElectroniqueComplet = true }).Should().BeTrue();
                analyzer.IsValid("1", new DonneesRapportIllustration { ProfilInvestisseurElectroniqueComplet = false }).Should().BeFalse();
            }
        }

        private static List<ProtectionPdf> InitialiserProtectionPdfs(bool isRenewable)
        {
            return new List<ProtectionPdf>
            {
                new ProtectionPdf {Specification = new SpecificationProtection() {IsRenewable = isRenewable}}
            };
        }

        private static List<ProtectionPdf> InitialiserIsCriticalIllness(bool isCriticalIllness)
        {
            return new List<ProtectionPdf>
            {
                new ProtectionPdf {Specification = new SpecificationProtection() {IsCriticalIllness = isCriticalIllness}}
            };
        }

        private static List<ProtectionPdf> InitialiserCashValueAvailable(bool isCashValueAvailable)
        {
            return new List<ProtectionPdf>
            {
                new ProtectionPdf {Specification = new SpecificationProtection() {IsCashValueAvailable = isCashValueAvailable}}
            };
        }
    }
}
