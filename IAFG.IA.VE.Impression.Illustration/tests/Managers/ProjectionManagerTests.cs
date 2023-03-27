using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VI.AF.IPDFVie.Factory.Interfaces;
using IAFG.IA.VI.Projection.Data;
using IAFG.IA.VI.Projection.Data.Characteristics;
using IAFG.IA.VI.Projection.Data.Contract;
using IAFG.IA.VI.Projection.Data.Contract.Coverage;
using IAFG.IA.VI.Projection.Data.Enums;
using IAFG.IA.VI.Projection.Data.Illustration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Managers
{
    [TestClass]
    public class ProjectionManagerTests
    {

        [TestMethod]
        public void WHEN_GetDefaultProjection_WithNull()
        {
            var manager = new ProjectionManager(null);
            using (new AssertionScope())
            {
                manager.GetDefaultProjection(null).Should().BeNull();
                manager.GetDefaultProjection(new Projections()).Should().BeNull();
            }
        }

        [TestMethod]
        public void WHEN_GetMainCoverage_WithNull()
        {
            var manager = new ProjectionManager(null);
            using (new AssertionScope())
            {
                manager.GetMainCoverage(null).Should().BeNull();
                manager.GetMainCoverage(new Projection()).Should().BeNull();
            }
        }

        [TestMethod]
        public void WHEN_GetMainCoverage_ReturnsMain()
        {
            var manager = new ProjectionManager(null);
            var mainCoverage = new Coverage
            {
                IsMain = true,
                PlanCode = "ABC123",
                InsuranceType = new VI.Projection.Data.Enums.Coverage.InsuranceType(),
                Identifier = new UniqueIdentifier { Id = "658" },
                Dates = new Dates
                {
                    Issuance = new DateTime(),
                    Maturity = new DateTime(),
                },
                Insured = new VI.Projection.Data.Contract.Coverage.Insured()
                {
                    ExtraPremiums = new List<ExtraPremium>(),
                    Age = new Age { Issuance = 16 }
                }
            };

            var listeCoverage = new List<Coverage>() { new Coverage(),  mainCoverage };
            var listeInsured = new List<VI.Projection.Data.Contract.Insured> 
            { 
                new VI.Projection.Data.Contract.Insured() { 
                    IsMain = true, 
                    Coverages = listeCoverage, 
                    Identifier = new UniqueIdentifier() { Id = "658" } 
                } 
            };

            var projection = new Projection() 
            {
                Contract = new Contract
                {
                    Insured = listeInsured
                }
            };

            manager.GetMainCoverage(projection).Should().Be(mainCoverage);
        }

        [TestMethod]
        public void WHEN_GetPdfCoverage_ReturnValid()
        {
            var result = Substitute.For<IGetPDFICoverageResponse>();
            var regleAffaireAccessor = Substitute.For<IRegleAffaireAccessor>();
            regleAffaireAccessor.GetPdfCoverage(Arg.Any<Projection>(), Arg.Any<Coverage>()).Returns(result);

            var manager = new ProjectionManager(regleAffaireAccessor);
            manager.GetPdfCoverage(new Projection(), new Coverage()).Should().Be(result);
        }

        [TestMethod]
        public void WHEN_DeterminerPresenceCompteTerme_ManageNulls()
        {
            var manager = new ProjectionManager(null);
            using (new AssertionScope())
            {
                manager.DeterminerPresenceCompteTerme(new Projection()).Should().BeFalse();
                manager.DeterminerPresenceCompteTerme(new Projection { Contract = new Contract() }).Should().BeFalse();
                manager.DeterminerPresenceCompteTerme(
                    new Projection { Contract = new Contract { FinancialSection = new VI.Projection.Data.Contract.Financial.FinancialSection() } }).Should().BeFalse();
            }
        }

        [TestMethod]
        public void WHEN_DeterminerPresenceCompteTerme_ManageEmptyList()
        {
            var regleAffaireAccessor = Substitute.For<IRegleAffaireAccessor>();
            regleAffaireAccessor.EstCompteInteretGarantie(Arg.Any<string>()).Returns(true);

            var projection = new Projection
            {
                Contract = new Contract
                {
                    FinancialSection = new VI.Projection.Data.Contract.Financial.FinancialSection
                    {
                        Funds = new List<VI.Projection.Data.Contract.Financial.Funds.Fund>()
                    }
                }
            };

            var manager = new ProjectionManager(regleAffaireAccessor);
            manager.DeterminerPresenceCompteTerme(projection).Should().BeFalse();
        }

        [TestMethod]
        public void WHEN_DeterminerPresencePrimeRenouvellable_ManageFalse()
        {
            var manager = new ProjectionManager(null);
            var illustration = new VI.Projection.Data.Illustration.Illustration
            {
                Columns = new List<Data<double[]>>(),
                ColumnDescriptions = new List<ColumnDescription> 
                { 
                    new ColumnDescription { Id = 10, Attributes = new List<string> { "Type:GuaranteedRenewal" } } 
                }
            };

            var projection = new Projection()
            {
                Illustration = illustration
            };

            using (new AssertionScope())
            {
                manager.DeterminerPresencePrimeRenouvellable(new Projection()).Should().BeFalse();
                manager.DeterminerPresencePrimeRenouvellable(new Projection { Illustration = new VI.Projection.Data.Illustration.Illustration()}).Should().BeFalse();
                manager.DeterminerPresencePrimeRenouvellable(projection).Should().BeFalse();
            }
        }

        [TestMethod]
        public void WHEN_DeterminerPresencePrimeRenouvellable_ManageTrue()
        {
            var manager = new ProjectionManager(null);
            var illustration = new VI.Projection.Data.Illustration.Illustration
            {
                Columns = new List<Data<double[]>> { new Data<double[]> { Id = 10, Value= new double[] { 100 } } },
                ColumnDescriptions = new List<ColumnDescription>
                {
                    new ColumnDescription { Id = 10, Attributes = new List<string> { "Type:GuaranteedRenewal" } }
                }
            };

            var projection = new Projection()
            {
                Illustration = illustration
            };

            manager.DeterminerPresencePrimeRenouvellable(projection).Should().BeTrue();
        }

        [TestMethod]
        public void WHEN_DeterminerTabagismePreferentiel_ManageNulls()
        {
            var manager = new ProjectionManager(null);
            using (new AssertionScope())
            {
                manager.DeterminerTabagismePreferentiel(null).Should().BeFalse();
                manager.DeterminerTabagismePreferentiel(new Projection()).Should().BeFalse();
                manager.DeterminerTabagismePreferentiel(new Projection { Contract = new Contract() }).Should().BeFalse();
                manager.DeterminerTabagismePreferentiel(
                    new Projection { Contract = new Contract { Insured = new List<VI.Projection.Data.Contract.Insured>()} }).Should().BeFalse();
            }
        }

        [TestMethod]
        public void WHEN_DeterminerTabagismePreferentiel_ManageTrue()
        {
            var manager = new ProjectionManager(null);
            var mainCoverage = new Coverage
            {
                IsMain = true,
                Status = VI.Projection.Data.Enums.Coverage.CoverageStatus.New,
                PlanCode = "ABC123",
                InsuranceType = new VI.Projection.Data.Enums.Coverage.InsuranceType(),
                Identifier = new UniqueIdentifier { Id = "658" },
                Dates = new Dates
                {
                    Issuance = new DateTime(),
                    Maturity = new DateTime(),
                },
                Insured = new VI.Projection.Data.Contract.Coverage.Insured()
                {
                    ExtraPremiums = new List<ExtraPremium>(),
                    Age = new Age { Issuance = 16 },
                    SmokerType = SmokerType.NonSmokerPreferred
                }
            };

            var listeCoverage = new List<Coverage>() { new Coverage(), mainCoverage };
            var listeInsured = new List<VI.Projection.Data.Contract.Insured>
            {
                new VI.Projection.Data.Contract.Insured() {
                    IsMain = true,
                    Coverages = listeCoverage,
                    Identifier = new UniqueIdentifier() { Id = "658" },
                }
            };

            var projection = new Projection()
            {
                Contract = new Contract
                {
                    Insured = listeInsured
                }
            };

            manager.DeterminerTabagismePreferentiel(projection).Should().BeTrue();
        }

        [TestMethod]
        public void WHEN_ContratEstEnDecheance_WithNull_THEN_ReturnsFALSE()
        {
            var manager = new ProjectionManager(null);
            using (new AssertionScope())
            {
                manager.ContratEstEnDecheance(null).Should().BeFalse();
                manager.ContratEstEnDecheance(new Projection()).Should().BeFalse();
                manager.ContratEstEnDecheance(new Projection
                {
                    Values = new List<KeyValuePair<Characteristic, double>>()
                }).Should().BeFalse();
            }
        }

        [TestMethod]
        public void WHEN_EtatContrat_WithMainCoverageNull_ThenException()
        {
            var manager = new ProjectionManager(null);
            var listeCoverage = new List<Coverage>() { new Coverage() };
            var listeInsured = new List<VI.Projection.Data.Contract.Insured>
            {
                new VI.Projection.Data.Contract.Insured() {
                    IsMain = true,
                    Coverages = listeCoverage,
                    Identifier = new UniqueIdentifier() { Id = "658" }
                }
            };

            var projection = new Projection()
            {
                Contract = new Contract
                {
                    Insured = listeInsured
                }
            };

            Action action = () => manager.EtatContrat(projection);
            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void WHEN_EtatContrat_WithContract_THEN_ReturnsEtat()
        {
            var manager = new ProjectionManager(null);
            var mainCoverage = new Coverage
            {
                IsMain = true,
                Status = VI.Projection.Data.Enums.Coverage.CoverageStatus.New,
                PlanCode = "ABC123",
                InsuranceType = new VI.Projection.Data.Enums.Coverage.InsuranceType(),
                Identifier = new UniqueIdentifier { Id = "658" },
                Dates = new Dates
                {
                    Issuance = new DateTime(),
                    Maturity = new DateTime(),
                },
                Insured = new VI.Projection.Data.Contract.Coverage.Insured()
                {
                    ExtraPremiums = new List<ExtraPremium>(),
                    Age = new Age { Issuance = 16 }
                }
            };

            var listeCoverage = new List<Coverage>() { new Coverage(), mainCoverage };
            var listeInsured = new List<VI.Projection.Data.Contract.Insured>
            {
                new VI.Projection.Data.Contract.Insured() {
                    IsMain = true,
                    Coverages = listeCoverage,
                    Identifier = new UniqueIdentifier() { Id = "658" }
                }
            };

            var projection = new Projection()
            {
                Contract = new Contract
                {
                    Insured = listeInsured
                }
            };

            using (new AssertionScope())
            {
                manager.EtatContrat(projection).Should().Be(Etat.NouvelleVente);
                mainCoverage.Status = VI.Projection.Data.Enums.Coverage.CoverageStatus.Modified;
                manager.EtatContrat(projection).Should().Be(Etat.EnVigueur);
            }
        }

        [TestMethod]
        public void WHEN_ProvinceEtat_WithNull_THEN_ReturnsInconnue()
        {
            var manager = new ProjectionManager(null);
            using (new AssertionScope())
            {
                manager.ProvinceEtat(null).Should().Be(ProvinceEtat.Inconnu);
                manager.ProvinceEtat(new Projection()).Should().Be(ProvinceEtat.Inconnu);
            }
        }

        [TestMethod]
        public void WHEN_ProvinceEtat_WithContractQuebec_THEN_ReturnsQuebec()
        {
            var projection = new Projection
            {
                Contract = new Contract
                {
                    ProvinceState = ProvinceState.Quebec
                }
            };

            var manager = new ProjectionManager(null);
            manager.ProvinceEtat(projection).Should().Be(ProvinceEtat.Quebec);
        }

        [TestMethod]
        public void WHEN_ProvinceEtat_WithContractAlaska_THEN_ReturnsAlaska()
        {
            var projection = new Projection
            {
                Contract = new Contract
                {
                    ProvinceState = ProvinceState.Alaska
                }
            };

            var manager = new ProjectionManager(null);
            manager.ProvinceEtat(projection).Should().Be(ProvinceEtat.Alaska);
        }

        [TestMethod]
        public void WHEN_Banniere_WithNull_THEN_ReturnsDefaut()
        {
            var manager = new ProjectionManager(null);
            using (new AssertionScope())
            {
                manager.Banniere(null).Should().Be(Banniere.Defaut);
                manager.Banniere(new Projection()).Should().Be(Banniere.Defaut);
            }
        }

        [TestMethod]
        public void WHEN_Banniere_WithContractIA_THEN_ReturnsIA()
        {
            var projection = new Projection
            {
                Contract = new Contract
                {
                    Banner = Banner.IA
                }
            };

            var manager = new ProjectionManager(null);
            manager.Banniere(projection).Should().Be(Banniere.IA);
        }

        [TestMethod]
        public void WHEN_Banniere_WithContractIAP_THEN_ReturnsIAP()
        {
            var projection = new Projection
            {
                Contract = new Contract
                {
                    Banner = Banner.IAP
                }
            };

            var manager = new ProjectionManager(null);
            manager.Banniere(projection).Should().Be(Banniere.IAP);
        }
    }
}
