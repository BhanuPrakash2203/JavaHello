using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.Illustration;
using IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees.TransactionContrat;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees.TransactionContrat.Participations;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees.TransactionProtection;
using IAFG.IA.VI.Projection.Data.Enums.Traditional;
using IAFG.IA.VI.Projection.Data.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using ProjectionData = IAFG.IA.VI.Projection.Data;
using ProjectionContract = IAFG.IA.VI.Projection.Data.Contract;
using ProjectionCoverage = IAFG.IA.VI.Projection.Data.Contract.Coverage;
using ProjectionEnums = IAFG.IA.VI.Projection.Data.Enums;
using ProjectionTransaction = IAFG.IA.VI.Projection.Data.Transactions;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Mappers.Illustration
{
    [TestClass]
    public class ModificationsMapperTests
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private IRegleAffaireAccessor _regleAffaireAccessor;

        [TestInitialize]
        public void TestInitialize()
        {
            _regleAffaireAccessor = Substitute.For<IRegleAffaireAccessor>();
        }

        [TestMethod]
        public void MapperModificationsDemandees_WITH_ProjectionNull_THEN_NotNull()
        {
            var clients = Auto.Create<List<Client>>();
            var mapper = new ModificationsMapper(_regleAffaireAccessor);

            var result = mapper.MapModificationsDemandees(null, clients, DateTime.Now.Date);
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Protections.Should().NotBeNull();
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WITH_EmptyTransactions_THEN_AllTransactionsMapped()
        {
            var clients = Auto.Create<List<Client>>();
            var coverages = Auto.Create<List<ProjectionCoverage.Coverage>>();
            var lastProtection = coverages.Last();
            var lastClient = clients.Last();

            lastProtection.Insured.Joints = null;
            lastProtection.Insured.InsuredIndividual = new ProjectionCoverage.InsuredIndividual
            {
                Individual = new ProjectionData.UniqueIdentifier().CreateIdentifier(lastClient.ReferenceExterneId)
            };

            var projection = new ProjectionData.Projection
            {
                Contract = new ProjectionContract.Contract
                {
                    Insured = new List<ProjectionContract.Insured>
                    {
                        new ProjectionContract.Insured
                        {
                            Coverages = coverages
                        }
                    }
                },
                Transactions = new ProjectionTransaction.Transactions
                {
                    Terminations = new List<ProjectionTransaction.Coverage.Termination>
                    {
                        new ProjectionTransaction.Coverage.Termination
                        {
                            TransactionIdentifier = new ProjectionData.UniqueIdentifier().CreateIdentifier("T1"),
                            CoverageIdentifier =
                                new ProjectionData.UniqueIdentifier().CreateIdentifier(
                                    lastProtection.Identifier.Id),
                            StartDate = new ProjectionData.GenericDate
                            {
                                DateType = ProjectionEnums.DateType.Calender,
                                CalenderDate = DateTime.Now.Date.AddYears(5)
                            }
                        }
                    },
                    FaceAmountChanges = new List<ProjectionTransaction.Coverage.FaceAmountChange>(),
                    TobaccoUsageChanges = new List<ProjectionTransaction.Coverage.TobaccoUsageChange>(),
                    Levelings = new List<ProjectionTransaction.Coverage.Leveling>()
                }
            };

            var mapper = new ModificationsMapper(_regleAffaireAccessor);
            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date);
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Protections.Should().NotBeNull();
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WITH_Termination_THEN_TransactionMapped()
        {
            var clients = Auto.Create<List<Client>>();
            var coverages = Auto.Create<List<ProjectionCoverage.Coverage>>();
            var lastProtection = coverages.Last();
            var lastClient = clients.Last();

            lastProtection.Insured.Joints = null;
            lastProtection.Insured.InsuredIndividual = new ProjectionCoverage.InsuredIndividual
            {
                Individual = new ProjectionData.UniqueIdentifier().CreateIdentifier(lastClient.ReferenceExterneId)
            };

            var transaction = new ProjectionTransaction.Coverage.Termination
            {
                TransactionIdentifier = new ProjectionData.UniqueIdentifier().CreateIdentifier("T1"),
                CoverageIdentifier =
                    new ProjectionData.UniqueIdentifier().CreateIdentifier(
                        lastProtection.Identifier.Id),
                StartDate = new ProjectionData.GenericDate
                {
                    DateType = ProjectionEnums.DateType.YearContract,
                    Year = 5
                }
            };

            var projection = new ProjectionData.Projection
            {
                Contract = new ProjectionContract.Contract
                {
                    Insured = new List<ProjectionContract.Insured>
                    {
                        new ProjectionContract.Insured
                        {
                            Coverages = coverages
                        }
                    }
                },
                Transactions = new ProjectionTransaction.Transactions
                {
                    Terminations = new List<ProjectionTransaction.Coverage.Termination>
                    {
                        transaction
                    }
                }
            };

            var mapper = new ModificationsMapper(_regleAffaireAccessor);
            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Protections?.Values.FirstOrDefault();
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.Protection.Coverage.Should().Be(lastProtection);
                var transactionResult = result?.Transactions?.FirstOrDefault() as Terminaison;
                transactionResult.Should().NotBeNull();
                transactionResult?.Annee.Should().Be(5);
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WITH_ReductionCapital_THEN_TransactionMapped()
        {
            var clients = Auto.Create<List<Client>>();
            var coverages = Auto.Create<List<ProjectionCoverage.Coverage>>();
            var lastProtection = coverages.Last();
            var lastClient = clients.Last();

            lastProtection.Insured.Joints = null;
            lastProtection.Insured.InsuredIndividual = new ProjectionCoverage.InsuredIndividual
            {
                Individual = new ProjectionData.UniqueIdentifier().CreateIdentifier(lastClient.ReferenceExterneId)
            };

            var transaction = new ProjectionTransaction.Coverage.FaceAmountChange
            {
                TransactionIdentifier = new ProjectionData.UniqueIdentifier().CreateIdentifier("T1"),
                CoverageIdentifier =
                    new ProjectionData.UniqueIdentifier().CreateIdentifier(
                        lastProtection.Identifier.Id),
                StartDate = new ProjectionData.GenericDate
                {
                    DateType = ProjectionEnums.DateType.YearContract,
                    Year = 5
                },
                Amount = 888.88,
                FaceAmountChangeType = ProjectionEnums.Coverage.FaceAmountChangeType.Reduction
            };

            var projection = new ProjectionData.Projection
            {
                Contract = new ProjectionContract.Contract
                {
                    Insured = new List<ProjectionContract.Insured>
                    {
                        new ProjectionContract.Insured
                        {
                            Coverages = coverages
                        }
                    }
                },
                Transactions = new ProjectionTransaction.Transactions
                {
                    FaceAmountChanges = new List<ProjectionTransaction.Coverage.FaceAmountChange>
                    {
                        transaction
                    }
                }
            };

            var mapper = new ModificationsMapper(_regleAffaireAccessor);
            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Protections?.Values.FirstOrDefault();
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.Protection.Coverage.Should().Be(lastProtection);
                var transactionResult = result?.Transactions?.FirstOrDefault() as ReductionCapital;
                transactionResult.Should().NotBeNull();
                transactionResult?.Annee.Should().Be(5);
                transactionResult?.Montant.Should().Be(888.88);
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WITH_ChangementUsageTabac_THEN_TransactionMapped()
        {
            var clients = Auto.Create<List<Client>>();
            var coverages = Auto.Create<List<ProjectionCoverage.Coverage>>();
            var lastProtection = coverages.Last();
            var lastClient = clients.Last();

            lastProtection.Insured.Joints = null;
            lastProtection.Insured.InsuredIndividual = new ProjectionCoverage.InsuredIndividual
            {
                Individual = new ProjectionData.UniqueIdentifier().CreateIdentifier(lastClient.ReferenceExterneId)
            };

            var transaction = new ProjectionTransaction.Coverage.TobaccoUsageChange
            {
                TransactionIdentifier = new ProjectionData.UniqueIdentifier().CreateIdentifier("T1"),
                CoverageIdentifier =
                    new ProjectionData.UniqueIdentifier().CreateIdentifier(
                        lastProtection.Identifier.Id),
                StartDate = new ProjectionData.GenericDate
                {
                    DateType = ProjectionEnums.DateType.YearContract,
                    Year = 5
                },
                SmokerType = ProjectionEnums.SmokerType.SmokerPreferred,
                IndividualIdentifier = new ProjectionData.UniqueIdentifier().CreateIdentifier(lastClient.ReferenceExterneId)
            };

            var projection = new ProjectionData.Projection
            {
                Contract = new ProjectionContract.Contract
                {
                    Insured = new List<ProjectionContract.Insured>
                    {
                        new ProjectionContract.Insured
                        {
                            Coverages = coverages
                        }
                    }
                },
                Transactions = new ProjectionTransaction.Transactions
                {
                    TobaccoUsageChanges = new List<ProjectionTransaction.Coverage.TobaccoUsageChange>
                    {
                        transaction
                    }
                }
            };

            var mapper = new ModificationsMapper(_regleAffaireAccessor);
            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Protections?.Values.FirstOrDefault();
            using (new AssertionScope())
            {
                result?.Protection.Should().NotBeNull();
                result?.Protection.ReferenceExterneId.Should().Be(lastProtection.Identifier.Id);
                result?.Protection.Coverage.Should().Be(lastProtection);
                result?.Protection.ReferenceExterneId.Should().Be(lastProtection.Identifier.Id);
                var transactionResult = result?.Transactions?.FirstOrDefault() as ChangementUsageTabac;
                transactionResult.Should().NotBeNull();
                transactionResult?.Annee.Should().Be(5);
                transactionResult?.StatutTabagisme.Should().Be(StatutTabagisme.FumeurPrivilege);
                transactionResult?.Initiale.Should().Be(lastClient.Initiale);
                transactionResult?.Prenom.Should().Be(lastClient.Prenom);
                transactionResult?.Nom.Should().Be(lastClient.Nom);
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WITH_Nivellement_THEN_TransactionMapped()
        {
            var clients = Auto.Create<List<Client>>();
            var coverages = Auto.Create<List<ProjectionCoverage.Coverage>>();
            var lastProtection = coverages.Last();
            var lastClient = clients.Last();

            lastProtection.Insured.Joints = null;
            lastProtection.Insured.InsuredIndividual = new ProjectionCoverage.InsuredIndividual
            {
                Individual = new ProjectionData.UniqueIdentifier().CreateIdentifier(lastClient.ReferenceExterneId)
            };

            var transaction = new ProjectionTransaction.Coverage.Leveling()
            {
                TransactionIdentifier = new ProjectionData.UniqueIdentifier().CreateIdentifier("T1"),
                CoverageIdentifier =
                    new ProjectionData.UniqueIdentifier().CreateIdentifier(
                        lastProtection.Identifier.Id),
                StartDate = new ProjectionData.GenericDate
                {
                    DateType = ProjectionEnums.DateType.YearContract,
                    Year = 5
                },
                Age = 52
            };

            var projection = new ProjectionData.Projection
            {
                Contract = new ProjectionContract.Contract
                {
                    Insured = new List<ProjectionContract.Insured>
                    {
                        new ProjectionContract.Insured
                        {
                            Coverages = coverages
                        }
                    }
                },
                Transactions = new ProjectionTransaction.Transactions
                {
                    Levelings = new List<ProjectionTransaction.Coverage.Leveling>
                    {
                        transaction
                    }
                }
            };

            var mapper = new ModificationsMapper(_regleAffaireAccessor);
            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Protections?.Values.FirstOrDefault();
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.Protection.Coverage.Should().Be(lastProtection);
                var transactionResult = result?.Transactions?.FirstOrDefault() as Nivellement;
                transactionResult.Should().NotBeNull();
                transactionResult?.Annee.Should().Be(5);
                transactionResult?.Age.Should().Be(52);
                transactionResult?.AgeSurprime.Should().BeNull();
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WITH_AjoutOption_THEN_TransactionMapped()
        {
            var clients = Auto.Create<List<Client>>();
            var coverages = Auto.Create<List<ProjectionCoverage.Coverage>>();
            var lastProtection = coverages.Last();
            var lastClient = clients.Last();

            lastProtection.Insured.Joints = null;
            lastProtection.Insured.InsuredIndividual = new ProjectionCoverage.InsuredIndividual
            {
                Individual = new ProjectionData.UniqueIdentifier().CreateIdentifier(lastClient.ReferenceExterneId)
            };

            var option = Auto.Create<ProjectionCoverage.Coverage>();
            option.Insured.Joints = null;
            option.Insured.InsuredIndividual = new ProjectionCoverage.InsuredIndividual
            {
                Individual = new ProjectionData.UniqueIdentifier().CreateIdentifier(lastClient.ReferenceExterneId)
            };
            lastProtection.Coverages = new List<ProjectionCoverage.Coverage>
            {
                option
            };

            var transaction = new ProjectionTransaction.Coverage.ExtendedCoverageAddition
            {
                TransactionIdentifier = new ProjectionData.UniqueIdentifier().CreateIdentifier("T1"),
                CoverageIdentifier =
                    new ProjectionData.UniqueIdentifier().CreateIdentifier(
                        lastProtection.Identifier.Id),
                StartDate = new ProjectionData.GenericDate
                {
                    DateType = ProjectionEnums.DateType.YearContract,
                    Year = 5
                },
                ExtendedCoverageIdentifier = new ProjectionData.UniqueIdentifier().CreateIdentifier(option.Identifier.Id),
                PlanCode = "abc123",
                VersionDate =
                    new ProjectionData.GenericDate
                    {
                        CalenderDate = DateTime.Today.AddYears(8),
                        DateType = ProjectionEnums.DateType.Calender
                    }
            };

            var projection = new ProjectionData.Projection
            {
                Contract = new ProjectionContract.Contract
                {
                    Insured = new List<ProjectionContract.Insured>
                    {
                        new ProjectionContract.Insured
                        {
                            Coverages = coverages
                        }
                    }
                },
                Transactions = new ProjectionTransaction.Transactions
                {
                    ExtendedCoverageAdditions = new List<ProjectionTransaction.Coverage.ExtendedCoverageAddition>
                    {
                        transaction
                    }
                }
            };

            var mapper = new ModificationsMapper(_regleAffaireAccessor);

            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Protections?.Values.FirstOrDefault();
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.Protection.Coverage.Should().Be(lastProtection);
                var transactionResult = result?.Transactions?.FirstOrDefault() as AjoutOption;
                transactionResult.Should().NotBeNull();
                transactionResult?.Annee.Should().Be(5);
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WITH_NivellementAndTermination_SameCoverage_THEN_TransactionsMapped()
        {
            var clients = Auto.Create<List<Client>>();
            var coverages = Auto.Create<List<ProjectionCoverage.Coverage>>();
            var lastProtection = coverages.Last();
            var lastClient = clients.Last();

            lastProtection.Insured.Joints = null;
            lastProtection.Insured.InsuredIndividual = new ProjectionCoverage.InsuredIndividual
            {
                Individual = new ProjectionData.UniqueIdentifier().CreateIdentifier(lastClient.ReferenceExterneId)
            };

            var transactionLeveling = new ProjectionTransaction.Coverage.Leveling
            {
                TransactionIdentifier = new ProjectionData.UniqueIdentifier().CreateIdentifier("T1"),
                CoverageIdentifier =
                    new ProjectionData.UniqueIdentifier().CreateIdentifier(
                        lastProtection.Identifier.Id),
                StartDate = new ProjectionData.GenericDate
                {
                    DateType = ProjectionEnums.DateType.YearContract,
                    Year = 5
                },
                Age = 52
            };

            var transactionTermination = new ProjectionTransaction.Coverage.Termination
            {
                TransactionIdentifier = new ProjectionData.UniqueIdentifier().CreateIdentifier("T1"),
                CoverageIdentifier =
                    new ProjectionData.UniqueIdentifier().CreateIdentifier(
                        lastProtection.Identifier.Id),
                StartDate = new ProjectionData.GenericDate
                {
                    DateType = ProjectionEnums.DateType.YearContract,
                    Year = 10
                }
            };

            var projection = new ProjectionData.Projection
            {
                Contract = new ProjectionContract.Contract
                {
                    Insured = new List<ProjectionContract.Insured>
                    {
                        new ProjectionContract.Insured
                        {
                            Coverages = coverages
                        }
                    }
                },
                Transactions = new ProjectionTransaction.Transactions
                {
                    Levelings = new List<ProjectionTransaction.Coverage.Leveling>
                    {
                        transactionLeveling
                    },
                    Terminations = new List<ProjectionTransaction.Coverage.Termination>
                    {
                        transactionTermination
                    }
                }
            };

            var mapper = new ModificationsMapper(_regleAffaireAccessor);

            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Protections?.Values.FirstOrDefault();
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.Protection.Coverage.Should().Be(lastProtection);
                var nivellementResult = result?.Transactions?.FirstOrDefault(x => x is Nivellement) as Nivellement;
                nivellementResult.Should().NotBeNull();
                nivellementResult?.Annee.Should().Be(5);
                nivellementResult?.Age.Should().Be(52);
                nivellementResult?.AgeSurprime.Should().BeNull();
                var terminaisonResult = result?.Transactions?.FirstOrDefault(x => x is Terminaison) as Terminaison;
                terminaisonResult.Should().NotBeNull();
                terminaisonResult?.Annee.Should().Be(10);
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WITH_TransformationIndividuelConjoint_THEN_TransactionMapped()
        {
            var clients = Auto.Create<List<Client>>();
            var coverages = Auto.Create<List<ProjectionCoverage.Coverage>>();
            var lastProtection = coverages.Last();
            var lastClient = clients.Last();

            lastProtection.Insured.Joints = null;
            lastProtection.Insured.InsuredIndividual = new ProjectionCoverage.InsuredIndividual
            {
                Individual = new ProjectionData.UniqueIdentifier().CreateIdentifier(lastClient.ReferenceExterneId)
            };

            var option = Auto.Create<ProjectionCoverage.Coverage>();
            option.Insured.Joints = null;
            option.Insured.InsuredIndividual = new ProjectionCoverage.InsuredIndividual
            {
                Individual = new ProjectionData.UniqueIdentifier().CreateIdentifier(lastClient.ReferenceExterneId)
            };
            lastProtection.Coverages = new List<ProjectionCoverage.Coverage>
            {
                option
            };

            var transaction = new ProjectionTransaction.Coverage.InsuranceTypeChange()
            {
                TransactionIdentifier = new ProjectionData.UniqueIdentifier().CreateIdentifier("T1"),
                CoverageIdentifier =
                    new ProjectionData.UniqueIdentifier().CreateIdentifier(
                        lastProtection.Identifier.Id),
                StartDate = new ProjectionData.GenericDate
                {
                    DateType = ProjectionEnums.DateType.YearContract,
                    Year = 5
                }
            };

            var projection = new ProjectionData.Projection
            {
                Contract = new ProjectionContract.Contract
                {
                    Insured = new List<ProjectionContract.Insured>
                    {
                        new ProjectionContract.Insured
                        {
                            Coverages = coverages
                        }
                    }
                },
                Transactions = new ProjectionTransaction.Transactions
                {
                    InsuranceTypeChanges = new List<ProjectionTransaction.Coverage.InsuranceTypeChange>
                    {
                        transaction
                    }
                }
            };

            var mapper = new ModificationsMapper(_regleAffaireAccessor);

            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Protections?.Values.FirstOrDefault();
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.Protection.Coverage.Should().Be(lastProtection);
                var transactionResult = result?.Transactions?.FirstOrDefault() as TransformationConjointDernierDeces;
                transactionResult.Should().NotBeNull();
                transactionResult?.Annee.Should().Be(5);
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WITH_DesactivationOaca_THEN_TransactionMapped()
        {
            var transaction = new ProjectionTransaction.Contract.DisableAutomaticFaceAmountOptimization
            {
                TransactionIdentifier = new ProjectionData.UniqueIdentifier().CreateIdentifier("T1"),
                StartDate = new ProjectionData.GenericDate
                {
                    DateType = ProjectionEnums.DateType.YearContract,
                    Year = 5
                }
            };

            var projection = new ProjectionData.Projection
            {
                Contract = new ProjectionContract.Contract(),
                Transactions = new ProjectionTransaction.Transactions
                {
                    DisableAutomaticFaceAmountOptimization = transaction
                }
            };

            var clients = Auto.Create<List<Client>>();
            var mapper = new ModificationsMapper(_regleAffaireAccessor);

            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Contrat;
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var transactionResult = result?.Transactions?.FirstOrDefault() as DesactivationOptimisationCapitalAssure;
                transactionResult.Should().NotBeNull();
                transactionResult?.Annee.Should().Be(5);
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WITH_ChangementOptionOaca_THEN_TransactionMapped()
        {
            var transaction = new ProjectionTransaction.Contract.PaidUpAdditionalOptionChange
            {
                TransactionIdentifier = new ProjectionData.UniqueIdentifier().CreateIdentifier("T1"),
                StartDate = new ProjectionData.GenericDate
                {
                    DateType = ProjectionEnums.DateType.YearContract,
                    Year = 5
                },
                PaidUpAdditionalOption = new ProjectionContract.PaidUpAdditionalOption
                {
                    AllocationAmount = 2500,
                    MaximalFaceAmount = 50000,
                    PurchaseOption = ProjectionEnums.PaidUpAdditionalPurchaseOption.WithBonus
                }
            };

            var projection = new ProjectionData.Projection
            {
                Contract = new ProjectionContract.Contract(),
                Transactions = new ProjectionTransaction.Transactions
                {
                    PaidUpAdditionalOptionChanges = new List<ProjectionTransaction.Contract.PaidUpAdditionalOptionChange> { transaction }
                }
            };

            var clients = Auto.Create<List<Client>>();
            var mapper = new ModificationsMapper(_regleAffaireAccessor);

            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Contrat;
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var transactionResult = result?.Transactions?.FirstOrDefault() as ChangementOptionAssuranceSupplementaireLiberee;
                transactionResult.Should().NotBeNull();
                transactionResult?.Annee.Should().Be(5);
                transactionResult?.MontantAllocation.Should().Be(2500);
                transactionResult?.CapitalAssurePlafond.Should().Be(50000);
                transactionResult?.OptionVersementBoni.Should().Be(TypeOptionVersementBoni.AvecBoni);
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WITH_ChangementOptionParticipation_THEN_TransactionMapped()
        {
            var transaction = new ProjectionTransaction.Contract.Participating.OptionChange
            {
                TransactionIdentifier = new ProjectionData.UniqueIdentifier().CreateIdentifier("T1"),
                StartDate = new ProjectionData.GenericDate
                {
                    DateType = ProjectionEnums.DateType.YearContract,
                    Year = 5
                },
                Option = DividendOption.Cash
            };

            var projection = new ProjectionData.Projection
            {
                Contract = new ProjectionContract.Contract(),
                Transactions = new ProjectionTransaction.Transactions
                {
                    Participating = new ProjectionTransaction.Contract.Participating.Participating
                    {
                        OptionChanges = new List<ProjectionTransaction.Contract.Participating.OptionChange> { transaction }
                    }
                }
            };

            var clients = Auto.Create<List<Client>>();
            var mapper = new ModificationsMapper(_regleAffaireAccessor);

            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Contrat;
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var transactionResult = result.Transactions?.FirstOrDefault() as ChangementOptionParticipation;
                transactionResult.Should().NotBeNull();
                transactionResult?.Annee.Should().Be(5);
                transactionResult?.Option.Should().Be(DividendOption.Cash);
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WITH_ChangementPrestationDeces_THEN_TransactionMapped()
        {
            var transaction = new ProjectionTransaction.Contract.DeathBenefitOptionChange
            {
                TransactionIdentifier = new ProjectionData.UniqueIdentifier().CreateIdentifier("T1"),
                StartDate = new ProjectionData.GenericDate
                {
                    DateType = ProjectionEnums.DateType.YearContract,
                    Year = 5
                },
                DeathBenefitOption = ProjectionEnums.Coverage.DeathBenefitOption.FaceAmountPlusFund
            };

            var projection = new ProjectionData.Projection
            {
                Contract = new ProjectionContract.Contract(),
                Transactions = new ProjectionTransaction.Transactions
                {
                    DeathBenefitOptionChanges = new List<ProjectionTransaction.Contract.DeathBenefitOptionChange> { transaction }
                }
            };

            var clients = Auto.Create<List<Client>>();
            var mapper = new ModificationsMapper(_regleAffaireAccessor);

            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Contrat;
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var transactionResult = result?.Transactions?.FirstOrDefault() as ChangementPrestationDeces;
                transactionResult.Should().NotBeNull();
                transactionResult?.Annee.Should().Be(5);
                transactionResult?.OptionPrestationDeces.Should().Be(OptionPrestationDeces.CapitalPlusFonds);
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WITH_AjoutProtection_THEN_TransactionMapped()
        {
            var clients = Auto.Create<List<Client>>();
            var coverages = Auto.Create<List<ProjectionCoverage.Coverage>>();
            var lastProtection = coverages.Last();
            var lastClient = clients.Last();

            lastProtection.Insured.Joints = null;
            lastProtection.Insured.InsuredIndividual = new ProjectionCoverage.InsuredIndividual
            {
                Individual = new ProjectionData.UniqueIdentifier().CreateIdentifier(lastClient.ReferenceExterneId)
            };

            var transaction = new ProjectionTransaction.Contract.CoverageAddition
            {
                TransactionIdentifier = new ProjectionData.UniqueIdentifier().CreateIdentifier("T1"),
                StartDate = new ProjectionData.GenericDate
                {
                    DateType = ProjectionEnums.DateType.YearContract,
                    Year = 5
                },
                PlanCode = lastProtection.PlanCode,
                CoverageIdentifier = lastProtection.Identifier.CreateIdentifier(),
                FaceAmount = lastProtection.FaceAmount.Actual,
                InsuranceType = ProjectionEnums.Coverage.InsuranceType.Individual,
                MortalityType = ProjectionEnums.Coverage.MortalityType.Level,
                Insured = new ProjectionTransaction.Insured.Insured
                {
                    Individuals = new List<ProjectionTransaction.Insured.InsuredIndividual>
                    {
                        new ProjectionTransaction.Insured.InsuredIndividual
                        {
                            SmokerType = ProjectionEnums.SmokerType.NonSmokerElite,
                            IndividualIdentifier =
                                new ProjectionData.UniqueIdentifier().CreateIdentifier(lastClient.ReferenceExterneId)
                        }
                    }
                }
            };

            var projection = new ProjectionData.Projection
            {
                Contract = new ProjectionContract.Contract
                {
                    Insured = new List<ProjectionContract.Insured>
                    {
                        new ProjectionContract.Insured
                        {
                            Coverages = coverages
                        }
                    }
                },
                Transactions = new ProjectionTransaction.Transactions
                {
                    CoverageAdditions = new List<ProjectionTransaction.Contract.CoverageAddition> { transaction }
                }
            };

            var mapper = new ModificationsMapper(_regleAffaireAccessor);
            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Protections?.Values.FirstOrDefault();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var transactionResult = result?.Transactions?.FirstOrDefault() as AjoutProtection;
                transactionResult.Should().NotBeNull();
                transactionResult?.Annee.Should().Be(5);
                transactionResult?.CapitalAssureActuel.Should().Be(lastProtection.FaceAmount.Actual);
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WhenAjoutProtectionAvecAssureAvecSurprimeUnspecified_ThenSurprimeBienMappee()
        {
            var termeSurprime = Auto.Create<int>();
            var surprime = new ProjectionCoverage.ExtraPremium
            {
                ExtraPremiumType = ProjectionEnums.Coverage.ExtraPremiumType.Unspecified,
                Term = termeSurprime
            };
           
            var clients = Auto.Create<List<Client>>();
            var projection = CreerProjectionAjoutProtectionAvecSurprime(clients, surprime);
            var mapper = new ModificationsMapper(_regleAffaireAccessor);

            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Protections.LastOrDefault().Value;
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var surprimeIndividu = result.Protection.Assures.Individus.First().Surprimes.First();
                surprimeIndividu.Should().NotBeNull();
                surprimeIndividu.EstTypeTemporaire.Should().BeFalse();
                surprimeIndividu.EstV999.Should().BeFalse();
                surprimeIndividu.Terme.Should().Be(termeSurprime);
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WhenAjoutProtectionAvecAssureAvecSurprimePermanentFlatExtraEtMontantPlusGrandQue0_ThenSurprimeBienMappee()
        {
            const double montantSurprime = 100;
            var termeSurprime = Auto.Create<int>();
            var surprime = new ProjectionCoverage.ExtraPremium
            {
                ExtraPremiumType = ProjectionEnums.Coverage.ExtraPremiumType.PermanentFlatExtra,
                Term = termeSurprime,
                Amount = montantSurprime
            };

            var clients = Auto.Create<List<Client>>();
            var projection = CreerProjectionAjoutProtectionAvecSurprime(clients, surprime);
            var mapper = new ModificationsMapper(_regleAffaireAccessor);

            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Protections.LastOrDefault().Value;
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var surprimeIndividu = result.Protection.Assures.Individus.First().Surprimes.First();
                surprimeIndividu.Should().NotBeNull();
                surprimeIndividu.EstTypeTemporaire.Should().BeFalse();
                surprimeIndividu.EstV999.Should().BeFalse();
                surprimeIndividu.Terme.Should().Be(termeSurprime);
                surprimeIndividu.TauxMontant.Should().Be(montantSurprime);
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WhenAjoutProtectionAvecAssureAvecSurprimePermanentFlatExtraEtMontantEst0_ThenSurprimeBienMappee()
        {
            var termeSurprime = Auto.Create<int>();
            var surprime = new ProjectionCoverage.ExtraPremium
            {
                ExtraPremiumType = ProjectionEnums.Coverage.ExtraPremiumType.PermanentFlatExtra,
                Term = termeSurprime,
                Amount = 0
            };

            var clients = Auto.Create<List<Client>>();
            var projection = CreerProjectionAjoutProtectionAvecSurprime(clients, surprime);
            var mapper = new ModificationsMapper(_regleAffaireAccessor);

            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Protections.LastOrDefault().Value;
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var surprimeIndividu = result.Protection.Assures.Individus.First().Surprimes.First();
                surprimeIndividu.Should().NotBeNull();
                surprimeIndividu.EstTypeTemporaire.Should().BeFalse();
                surprimeIndividu.EstV999.Should().BeFalse();
                surprimeIndividu.Terme.Should().Be(termeSurprime);
                surprimeIndividu.TauxMontant.Should().BeNull();
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WhenAjoutProtectionAvecAssureAvecSurprimePermanentRateEtPourcentagePlusPetitQue999_ThenSurprimeBienMappee()
        {
            const int pourcentage = 998;
            const double expectedPourcentage = 9.98;
            var termeSurprime = Auto.Create<int>();
            var surprime = new ProjectionCoverage.ExtraPremium
            {
                ExtraPremiumType = ProjectionEnums.Coverage.ExtraPremiumType.PermanentRate,
                Term = termeSurprime,
                Percentage = pourcentage
            };

            var clients = Auto.Create<List<Client>>();
            var projection = CreerProjectionAjoutProtectionAvecSurprime(clients, surprime);
            var mapper = new ModificationsMapper(_regleAffaireAccessor);

            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Protections.LastOrDefault().Value;
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var surprimeIndividu = result.Protection.Assures.Individus.First().Surprimes.First();
                surprimeIndividu.Should().NotBeNull();
                surprimeIndividu.EstTypeTemporaire.Should().BeFalse();
                surprimeIndividu.EstV999.Should().BeFalse();
                surprimeIndividu.Terme.Should().Be(termeSurprime);
                surprimeIndividu.TauxPourcentage.Should().Be(expectedPourcentage);
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WhenAjoutProtectionAvecAssureAvecSurprimePermanentRateEtPourcentageEst999_ThenSurprimeBienMappee()
        {
            var termeSurprime = Auto.Create<int>();
            var surprime = new ProjectionCoverage.ExtraPremium
            {
                ExtraPremiumType = ProjectionEnums.Coverage.ExtraPremiumType.PermanentRate,
                Term = termeSurprime,
                Percentage = 999
            };

            var clients = Auto.Create<List<Client>>();
            var projection = CreerProjectionAjoutProtectionAvecSurprime(clients, surprime);
            var mapper = new ModificationsMapper(_regleAffaireAccessor);

            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Protections.LastOrDefault().Value;
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var surprimeIndividu = result.Protection.Assures.Individus.First().Surprimes.First();
                surprimeIndividu.Should().NotBeNull();
                surprimeIndividu.EstTypeTemporaire.Should().BeFalse();
                surprimeIndividu.EstV999.Should().BeTrue();
                surprimeIndividu.Terme.Should().Be(termeSurprime);
                surprimeIndividu.TauxPourcentage.Should().BeNull();
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WhenAjoutProtectionAvecAssureAvecSurprimeTemporaryFlatExtraEtMontantPlusGrandQue0_ThenSurprimeBienMappee()
        {
            const double montantSurprime = 100;
            var termeSurprime = Auto.Create<int>();
            var surprime = new ProjectionCoverage.ExtraPremium
            {
                ExtraPremiumType = ProjectionEnums.Coverage.ExtraPremiumType.TemporaryFlatExtra,
                Term = termeSurprime,
                Amount = montantSurprime
            };

            var clients = Auto.Create<List<Client>>();
            var projection = CreerProjectionAjoutProtectionAvecSurprime(clients, surprime);
            var mapper = new ModificationsMapper(_regleAffaireAccessor);

            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Protections.LastOrDefault().Value;
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var surprimeIndividu = result.Protection.Assures.Individus.First().Surprimes.First();
                surprimeIndividu.Should().NotBeNull();
                surprimeIndividu.EstTypeTemporaire.Should().BeTrue();
                surprimeIndividu.EstV999.Should().BeFalse();
                surprimeIndividu.Terme.Should().Be(termeSurprime);
                surprimeIndividu.TauxMontant.Should().Be(montantSurprime);
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WhenAjoutProtectionAvecAssureAvecSurprimeTemporaryFlatExtraEtMontantEst0_ThenSurprimeBienMappee()
        {
            var termeSurprime = Auto.Create<int>();
            var surprime = new ProjectionCoverage.ExtraPremium
            {
                ExtraPremiumType = ProjectionEnums.Coverage.ExtraPremiumType.TemporaryFlatExtra,
                Term = termeSurprime,
                Amount = 0
            };

            var clients = Auto.Create<List<Client>>();
            var projection = CreerProjectionAjoutProtectionAvecSurprime(clients, surprime);
            var mapper = new ModificationsMapper(_regleAffaireAccessor);

            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Protections.LastOrDefault().Value;
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var surprimeIndividu = result.Protection.Assures.Individus.First().Surprimes.First();
                surprimeIndividu.Should().NotBeNull();
                surprimeIndividu.EstTypeTemporaire.Should().BeTrue();
                surprimeIndividu.EstV999.Should().BeFalse();
                surprimeIndividu.Terme.Should().Be(termeSurprime);
                surprimeIndividu.TauxMontant.Should().BeNull();
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WhenAjoutProtectionAvecAssureAvecSurprimePermanentTableEtPourcentagePlusPetitQue999_ThenSurprimeBienMappee()
        {
            const int pourcentage = 998;
            const double expectedPourcentage = 9.98;
            var termeSurprime = Auto.Create<int>();
            var surprime = new ProjectionCoverage.ExtraPremium
            {
                ExtraPremiumType = ProjectionEnums.Coverage.ExtraPremiumType.PermanentTable,
                Term = termeSurprime,
                Percentage = pourcentage
            };

            var clients = Auto.Create<List<Client>>();
            var projection = CreerProjectionAjoutProtectionAvecSurprime(clients, surprime);
            var mapper = new ModificationsMapper(_regleAffaireAccessor);

            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Protections.LastOrDefault().Value;
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var surprimeIndividu = result.Protection.Assures.Individus.First().Surprimes.First();
                surprimeIndividu.Should().NotBeNull();
                surprimeIndividu.EstTypeTemporaire.Should().BeFalse();
                surprimeIndividu.EstV999.Should().BeFalse();
                surprimeIndividu.Terme.Should().Be(termeSurprime);
                surprimeIndividu.TauxPourcentage.Should().Be(expectedPourcentage);
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WhenAjoutProtectionAvecAssureAvecSurprimePermanentTableEtPourcentageEst999_ThenSurprimeBienMappee()
        {
            var termeSurprime = Auto.Create<int>();
            var surprime = new ProjectionCoverage.ExtraPremium
            {
                ExtraPremiumType = ProjectionEnums.Coverage.ExtraPremiumType.PermanentTable,
                Term = termeSurprime,
                Percentage = 999
            };

            var clients = Auto.Create<List<Client>>();
            var projection = CreerProjectionAjoutProtectionAvecSurprime(clients, surprime);
            var mapper = new ModificationsMapper(_regleAffaireAccessor);

            var result = mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date).Protections.LastOrDefault().Value;
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var surprimeIndividu = result.Protection.Assures.Individus.First().Surprimes.First();
                surprimeIndividu.Should().NotBeNull();
                surprimeIndividu.EstTypeTemporaire.Should().BeFalse();
                surprimeIndividu.EstV999.Should().BeTrue();
                surprimeIndividu.Terme.Should().Be(termeSurprime);
                surprimeIndividu.TauxPourcentage.Should().BeNull();
            }
        }

        [TestMethod]
        public void MapperModificationsDemandees_WhenAjoutProtectionAvecAssureAvecSurprimeInconnue_ThenArgumentOutOfRangeException()
        {
            var termeSurprime = Auto.Create<int>();
            var surprime = new ProjectionCoverage.ExtraPremium
            {
                ExtraPremiumType = (ProjectionEnums.Coverage.ExtraPremiumType)(-1),
                Term = termeSurprime,
                Percentage = 999
            };

            var clients = Auto.Create<List<Client>>();
            var projection = CreerProjectionAjoutProtectionAvecSurprime(clients, surprime);
            var mapper = new ModificationsMapper(_regleAffaireAccessor);
            var action = new Action(() => mapper.MapModificationsDemandees(projection, clients, DateTime.Now.Date));
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        private ProjectionData.Projection CreerProjectionAjoutProtectionAvecSurprime(List<Client> clients, ProjectionCoverage.ExtraPremium surprime)
        {
            var coverages = Auto.Create<List<ProjectionCoverage.Coverage>>();
            var lastProtection = coverages.Last();
            var lastClient = clients.Last();

            lastProtection.Insured.ExtraPremiums = new List<ProjectionCoverage.ExtraPremium> { surprime };

            lastProtection.Insured.Joints = null;
            lastProtection.Insured.InsuredIndividual = new ProjectionCoverage.InsuredIndividual
            {
                Individual = new ProjectionData.UniqueIdentifier().CreateIdentifier(lastClient.ReferenceExterneId)
            };

            var transaction = new ProjectionTransaction.Contract.CoverageAddition
            {
                TransactionIdentifier = new ProjectionData.UniqueIdentifier().CreateIdentifier("T1"),
                Insured = new ProjectionTransaction.Insured.Insured
                {
                    Individuals = new List<ProjectionTransaction.Insured.InsuredIndividual>
                    {
                        new ProjectionTransaction.Insured.InsuredIndividual
                        {
                            IndividualIdentifier =
                                new ProjectionData.UniqueIdentifier().CreateIdentifier(lastClient.ReferenceExterneId)
                        }
                    }
                },
                CoverageIdentifier = lastProtection.Identifier.CreateIdentifier(),
            };

            return new ProjectionData.Projection
            {
                Contract = new ProjectionContract.Contract
                {
                    Insured = new List<ProjectionContract.Insured>
                    {
                        new ProjectionContract.Insured
                        {
                            Coverages = coverages
                        }
                    }
                },
                Transactions = new ProjectionTransaction.Transactions
                {
                    CoverageAdditions = new List<ProjectionTransaction.Contract.CoverageAddition> { transaction }
                }
            };
        }
    }
}
