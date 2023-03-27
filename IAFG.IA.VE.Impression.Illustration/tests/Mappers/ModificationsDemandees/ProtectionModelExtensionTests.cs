using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.ModificationsDemandees;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Mappers.ModificationsDemandees
{
    [TestClass]
    public class ProtectionModelExtensionTests
    {
        private IIllustrationReportDataFormatter _formatter;
        private IIllustrationResourcesAccessorFactory _resourcesAccessor;

        [TestInitialize]
        public void Initialize()
        {
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _formatter.FormatFullName("Daisy", "Duck", "W.").Returns("Daisy W. Duck");
            _formatter.FormatFullName("Yvan", "DuStock", null).Returns("Yvan DuStock");
            _formatter.FormatNames(Arg.Any<IEnumerable<string>>()).Returns(x => x.Arg<IEnumerable<string>>().FirstOrDefault());
            _formatter.FormatCurrencyWithoutDecimal(50000.00).Returns("50 000$");
            _formatter.FormatCurrencyWithoutDecimal(25000.11).Returns("25 000$");
            _formatter.FormatAge(55).Returns("55 ans");
            _formatter.FormatAge(56).Returns("56 ans");
            _formatter.FormatUsageTabac(StatutTabagisme.FumeurElite).Returns("Statut: " + StatutTabagisme.FumeurElite);
            _formatter.FormatPercentage(225).Returns("225%");

            _resourcesAccessor = Substitute.For<IIllustrationResourcesAccessorFactory>();
        }
        
        [TestMethod]
        public void MapperTransactions_WithNull()
        {
            ((List<ProtectionModel>)null).MapperProtections(_resourcesAccessor, _formatter).Should().NotBeNull();
        }

        [TestMethod]
        public void MapperTransactions_WithData()
        {
            var protections = new List<ProtectionModel>
            {
                new ProtectionModel
                {
                    Id = "p1",
                    Capital = 50000,
                    Libelle = "un libelle",
                    Assures = new List<AssureModel> {new AssureModel {Nom = "Duck", Prenom = "Daisy", Initiale = "W."}}
                }
            };

            var result = protections.MapperProtections(_resourcesAccessor, _formatter);
            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.First().Id.Should().Be("p1");
                result.First().DescriptionProtection.Should().Be("un libelle");
                result.First().Montant.Should().Be("50 000$");
                result.First().NomAssures.Should().Be("Daisy W. Duck");
                result.First().Modifications.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void MapperTransactions_WithTerminaison()
        {
            var protections = new List<ProtectionModel>
            {
                new ProtectionModel
                {
                    Id = "p1",
                    Libelle = "un libelle",
                    Assures = new List<AssureModel>(),
                    Transactions = new List<TransactionModel>
                    {
                        new TransactionTerminaisonModel {Annee = 56, Descpription = "Une terminaison a l'année {0}"}
                    }
                }
            };

            var result = protections.MapperProtections(_resourcesAccessor, _formatter);
            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.First().Modifications.Should().HaveCount(1);
                var resultTransaction = result.First().Modifications.First();
                resultTransaction.DescriptionModification.Should().Be("Une terminaison a l'année 56");
                resultTransaction.Sequence.Should().Be(1);
            }
        }

        [TestMethod]
        public void MapperTransactions_WITH_Terminaison_AND_ReductionCapitalModel_THEN_MappedAndOrdered()
        {
            var protections = new List<ProtectionModel>
            {
                new ProtectionModel
                {
                    Id = "p1",
                    Libelle = "un libelle",
                    Assures = new List<AssureModel>(),
                    Transactions = new List<TransactionModel>
                    {
                        new TransactionTerminaisonModel {Annee = 56, Descpription = "Une terminaison a l'année {0}"},
                        new TransactionReductionCapitalModel
                        {
                            Annee = 8,
                            Descpription = "Une réduction a l'année {0}",
                            Montant = 25000.11,
                            DescpriptionMontant = "Montant de {0}"
                        }
                    }
                }
            };

            var result = protections.MapperProtections(_resourcesAccessor, _formatter);
            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.First().Modifications.Should().HaveCount(2);
                var resultTransaction1 = result.First().Modifications.First();
                resultTransaction1.DescriptionModification.Should().Be("Une réduction a l'année 8");
                resultTransaction1.ModificationDetails.First().Should().Be("Montant de 25 000$");
                resultTransaction1.Sequence.Should().Be(1);
                var resultTransaction2 = result.First().Modifications.Last();
                resultTransaction2.DescriptionModification.Should().Be("Une terminaison a l'année 56");
                resultTransaction2.Sequence.Should().Be(2);
            }
        }

        [TestMethod]
        public void MapperTransactions_WithNivellement()
        {
            var protections = new List<ProtectionModel>
            {
                new ProtectionModel
                {
                    Id = "p1",
                    Libelle = "un libelle",
                    Assures = new List<AssureModel>(),
                    Transactions = new List<TransactionModel>
                    {
                        new TransactionNivellementModel
                        {
                            Annee = 12,
                            Descpription = "Un nivellement a l'année {0}",
                            Age = 55,
                            DescpriptionAge = "age de {0}",
                            AgeSurprime = 56,
                            DescpriptionAgeSurprime = "age surprime de {0}"
                        }
                    }
                }
            };

            var result = protections.MapperProtections(_resourcesAccessor, _formatter);
            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.First().Modifications.Should().HaveCount(1);
                var resultTransaction = result.First().Modifications.First();
                resultTransaction.DescriptionModification.Should().Be("Un nivellement a l'année 12");
                resultTransaction.ModificationDetails.First().Should().Be("age de 55 ans");
                resultTransaction.ModificationDetails.Last().Should().Be("age surprime de 56 ans");
                resultTransaction.Sequence.Should().Be(1);
            }
        }

        [TestMethod]
        public void MapperTransactions_WithChangementUsageTabacModel()
        {
            var protections = new List<ProtectionModel>
            {
                new ProtectionModel
                {
                    Id = "p1",
                    Libelle = "un libelle",
                    Assures = new List<AssureModel>(),
                    Transactions = new List<TransactionModel>
                    {
                        new TransactionChangementUsageTabacModel
                        {
                            Annee = 12,
                            Descpription = "Un changement de tabac a l'année {0}",
                            StatutTabagisme = StatutTabagisme.FumeurElite,
                            Prenom = "Yvan",
                            Nom = "DuStock"
                        }
                    }
                }
            };

            var result = protections.MapperProtections(_resourcesAccessor, _formatter);
            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.First().Modifications.Should().HaveCount(1);
                var resultTransaction = result.First().Modifications.First();
                resultTransaction.DescriptionModification.Should().Be("Un changement de tabac a l'année 12");
                resultTransaction.ModificationDetails.First().Should().Be("Yvan DuStock");
                resultTransaction.ModificationDetails.Last().Should().Be("Statut: " + StatutTabagisme.FumeurElite);
                resultTransaction.Sequence.Should().Be(1);
            }
        }

        [TestMethod]
        public void MapperTransactions_WithAjoutOption()
        {
            var protections = new List<ProtectionModel>
            {
                new ProtectionModel
                {
                    Id = "p1",
                    Libelle = "un libelle",
                    Assures = new List<AssureModel>(),
                    Transactions = new List<TransactionModel>
                    {
                        new TransactionAjoutOptionModel
                        {
                            Annee = 12,
                            Descpription = "Un ajout d'option a l'année {0}",
                            DescpriptionMontantPrime = "Montant de {0}",
                            DescpriptionOption = "une option"
                        }
                    }
                }
            };

            var result = protections.MapperProtections(_resourcesAccessor, _formatter);
            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.First().Modifications.Should().HaveCount(1);
                var resultTransaction = result.First().Modifications.First();
                resultTransaction.DescriptionModification.Should().Be("Un ajout d'option a l'année 12");
                resultTransaction.Sequence.Should().Be(1);
            }
        }

        [TestMethod]
        public void MapperTransactions_WithAjoutProtection()
        {
            var protections = new List<ProtectionModel>
            {
                new ProtectionModel
                {
                    Id = "p1",
                    Libelle = "un libelle",
                    Assures = new List<AssureModel>(),
                    Transactions = new List<TransactionModel>
                    {
                        new TransactionAjoutProtectionModel
                        {
                            Annee = 12,
                            Descpription = "Un ajout de protection a l'année {0}",
                            DescpriptionMontantPrime = "Montant de {0}",
                            DescpriptionProtection = "une protection"
                        }
                    }
                }
            };

            var result = protections.MapperProtections(_resourcesAccessor, _formatter);
            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.First().Modifications.Should().HaveCount(1);
                var resultTransaction = result.First().Modifications.First();
                resultTransaction.DescriptionModification.Should().Be("Un ajout de protection a l'année 12");
                resultTransaction.Sequence.Should().Be(1);
            }
        }

        [TestMethod]
        public void MapperTransactions_WithTransformationConjointDernierDeces()
        {
            var protections = new List<ProtectionModel>
            {
                new ProtectionModel
                {
                    Id = "p1",
                    Libelle = "un libelle",
                    Assures = new List<AssureModel>(),
                    Transactions = new List<TransactionModel>
                    {
                        new TransactionTransformationConjointDernierDecesModel
                        {
                            Annee = 12,
                            Descpription = "Une transfo en conjoint dernier décès a l'année {0}"
                        },
                        new TransactionTransformationConjointDernierDecesModel
                        {
                            Annee = 15,
                            Descpription = "Une transfo en conjoint dernier décès a l'année {0}",
                            Surprimes = new List<SurprimeModel>
                            {
                                new SurprimeModel {Descpription = "une Surprime"},
                                new SurprimeModel {Descpription = "Autre surprime", TauxPourcentage = 225}
                            }
                        }
                    }
                }
            };

            var result = protections.MapperProtections(_resourcesAccessor, _formatter);
            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.First().Modifications.Should().HaveCount(2);
                var resultTransaction1 = result.First().Modifications.First();
                resultTransaction1.DescriptionModification.Should().Be("Une transfo en conjoint dernier décès a l'année 12");
                resultTransaction1.Sequence.Should().Be(1);
                var resultTransaction2 = result.First().Modifications.Last();
                resultTransaction2.DescriptionModification.Should().Be("Une transfo en conjoint dernier décès a l'année 15");
                resultTransaction2.Sequence.Should().Be(2);
                resultTransaction2.Details.Should().HaveCount(2);
                resultTransaction2.Details.First().Should().Be("une Surprime");
                resultTransaction2.Details.Last().Should().Be("Autre surprime +225%");
            }
        }
    }
}
