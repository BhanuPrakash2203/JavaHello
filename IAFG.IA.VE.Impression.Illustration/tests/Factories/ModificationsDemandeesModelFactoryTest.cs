using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees.TransactionContrat;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees.TransactionContrat.Participations;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees.TransactionProtection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Factories
{
    [TestClass]
    public class ModificationsDemandeesModelFactoryTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private IConfigurationRepository _configurationRepository;
        private IIllustrationReportDataFormatter _formatter;
        private IIllustrationResourcesAccessorFactory _resourcesAccessor;
        private readonly IDefinitionNoteManager _noteManager = Substitute.For<IDefinitionNoteManager>();
        private readonly IDefinitionTableauManager _tableauManager = Substitute.For<IDefinitionTableauManager>();
        private IDefinitionTitreManager _titreManager;
        private IDefinitionImageManager _imageManager;
        private PrivateObject _factory;

        [TestInitialize]
        public void Initialize()
        {
            _configurationRepository = Substitute.For<IConfigurationRepository>();
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _resourcesAccessor = Substitute.For<IIllustrationResourcesAccessorFactory>();

            _factory = new PrivateObject(typeof(ModificationsDemandeesModelFactory),
                _configurationRepository,
                _resourcesAccessor,
                Substitute.For<ISectionModelMapper>());

            _titreManager = new DefinitionTitreManager(_formatter);
            _imageManager = new DefinitionImageManager();
        }

        [TestMethod]
        public void GIVEN_ModificationsDemandeesModelFactory_WHEN_Build_Then_ReturnSectionModel()
        {
            var donnees = Auto.Create<DonneesRapportIllustration>();
            var definition = new DefinitionSection
            {
                SectionId = "ModificationsDemandees",
                Titres = Auto.Create<List<DefinitionTitreDescriptionSelonProduit>>()
            };

            _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(Arg.Any<string>(), Arg.Any<Produit>()).Returns(definition);
            _formatter.FormatterTitre(definition.Titres.FirstOrDefault(), donnees).Returns(definition.Titres.First().Titre);

            var factory = new ModificationsDemandeesModelFactory(_configurationRepository, _resourcesAccessor, 
                new SectionModelMapper(_formatter, _noteManager, _tableauManager, _titreManager, _imageManager));

            var model = factory.Build(definition.SectionId, donnees, Auto.Create<IReportContext>());

            model.TitreSection.Should().Be(definition.Titres.First().Titre);
        }

        [TestMethod]
        public void GIVEN_ModificationsDemandeesModelFactory_WHEN_Build_Then_ReturnSectionContratAndProtectionsModel()
        {
            var titreSection = Auto.Create<List<DefinitionTitreDescriptionSelonProduit>>();
            var titreSectionContrat = Auto.Create<List<DefinitionTitreDescriptionSelonProduit>>();
            var titreSectionProtections = Auto.Create<List<DefinitionTitreDescriptionSelonProduit>>();

            var donnees = Auto.Create<DonneesRapportIllustration>();
            var definition = new DefinitionSection
            {
                SectionId = "ModificationsDemandees",
                Titres = titreSection,
                ListSections = new List<DefinitionSection>
                {
                    new DefinitionSection {Titres = titreSectionContrat, SectionId = "Contrat"},
                    new DefinitionSection {Titres = titreSectionProtections, SectionId = "Protections"}
                }
            };

            _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(Arg.Any<string>(), Arg.Any<Produit>()).Returns(definition);
            _formatter.FormatterTitre(titreSection.FirstOrDefault(), donnees).Returns(titreSection.First().Titre);
            _formatter.FormatterTitre(titreSectionContrat.FirstOrDefault(), donnees).Returns(titreSectionContrat.First().Titre);
            _formatter.FormatterTitre(titreSectionProtections.FirstOrDefault(), donnees).Returns(titreSectionProtections.First().Titre);
            
            var reportContext = Auto.Create<IReportContext>();
            reportContext.Language = Language.French;
            var factory = new ModificationsDemandeesModelFactory(_configurationRepository, _resourcesAccessor, 
                new SectionModelMapper(_formatter, _noteManager, _tableauManager, _titreManager, _imageManager));

            var model = factory.Build(definition.SectionId, donnees, reportContext);

            using (new AssertionScope())
            {
                model.TitreSection.Should().Be(titreSection.First().Titre);
                model.SectionContratModel.TitreSection.Should().Be(titreSectionContrat.First().Titre);
                model.SectionProtectionsModel.TitreSection.Should().Be(titreSectionProtections.First().Titre);

                var index = 0;
                foreach (var protection in model.SectionProtectionsModel.Protections)
                {
                    var source = donnees.ModificationsDemandees.Protections.Values.Skip(index).First().Protection;
                    protection.Libelle.Should().Be(source.Plan.DescriptionFr);
                    protection.Capital.Should().Be(source.Plan.SansVolumeAssurance ? null : source.CapitalAssureActuel);
                    protection.Assures.Should().HaveSameCount(source.Assures.Individus);
                    protection.Assures.First().Nom.Should().Be(source.Assures.Individus.First().Nom);
                    protection.Assures.First().Prenom.Should().Be(source.Assures.Individus.First().Prenom);
                    protection.Assures.First().Initiale.Should().Be(source.Assures.Individus.First().Initiale);
                    protection.Transactions.Should().BeEmpty();
                    index += 1;
                }
            }
        }

        [TestMethod]
        public void GIVEN_ModificationsDemandeesModelFactory_WHEN_Build_Then_ReturnSectionContratWithTransactionModel()
        {
            var titreSection = Auto.Create<List<DefinitionTitreDescriptionSelonProduit>>();
            var titreSectionContrat = Auto.Create<List<DefinitionTitreDescriptionSelonProduit>>();

            var transactionDesactivationOptimisationCapitalAssure = Auto.Create<DesactivationOptimisationCapitalAssure>();
            var transactionChangementPrestationDeces = Auto.Create<ChangementPrestationDeces>();
            var transactions = new List<Transaction>
            {
                transactionDesactivationOptimisationCapitalAssure,
                transactionChangementPrestationDeces
            };

            var donnees = Auto.Create<DonneesRapportIllustration>();
            donnees.ModificationsDemandees.Contrat = new ModificationsContrat
            {
                Transactions = transactions
            };

            var definition = new DefinitionSection
            {
                SectionId = "ModificationsDemandees",
                Titres = titreSection,
                ListSections = new List<DefinitionSection>
                {
                    new DefinitionSection {Titres = titreSectionContrat, SectionId = "Contrat"}
                }
            };

            _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(Arg.Any<string>(), Arg.Any<Produit>()).Returns(definition);
            _formatter.FormatterTitre(titreSection.FirstOrDefault(), donnees).Returns(titreSection.First().Titre);
            _formatter.FormatterTitre(titreSectionContrat.FirstOrDefault(), donnees).Returns(titreSectionContrat.First().Titre);

            var reportContext = Auto.Create<IReportContext>();
            reportContext.Language = Language.English;
            var factory = new ModificationsDemandeesModelFactory(_configurationRepository, _resourcesAccessor, 
                new SectionModelMapper(_formatter, _noteManager, _tableauManager, _titreManager, _imageManager));

            var model = factory.Build(definition.SectionId, donnees, reportContext);

            using (new AssertionScope())
            {
                model.TitreSection.Should().Be(titreSection.First().Titre);
                model.SectionContratModel.TitreSection.Should().Be(titreSectionContrat.First().Titre);
                model.SectionContratModel.Transactions.Should().HaveSameCount(transactions);

                var t1 = model.SectionContratModel.Transactions.FirstOrDefault(x => x is TransactionChangementPrestationDecesModel) as TransactionChangementPrestationDecesModel;
                t1.Should().NotBeNull();
                t1?.Annee.Should().Be(transactionChangementPrestationDeces.Annee);

                var t2 = model.SectionContratModel.Transactions.FirstOrDefault(x => x is TransactionDesactivationOptimisationAutomatiqueCapitalAssureModel) as TransactionDesactivationOptimisationAutomatiqueCapitalAssureModel;
                t2.Should().NotBeNull();
                t2?.Annee.Should().Be(transactionDesactivationOptimisationCapitalAssure.Annee);
            }
        }

        [TestMethod]
        public void GIVEN_ModificationsDemandeesModelFactory_WHEN_Build_Then_ReturnSectionProtectionsWithTransactionModel()
        {
            var titreSection = Auto.Create<List<DefinitionTitreDescriptionSelonProduit>>();
            var titreSectionProtections = Auto.Create<List<DefinitionTitreDescriptionSelonProduit>>();

            var transactionTerminaison = Auto.Create<Terminaison>();
            var transactionChangementUsageTabac = Auto.Create<ChangementUsageTabac>();
            var transactionReductionCapital = Auto.Create<ReductionCapital>();
            var transactionNivellement = Auto.Create<Nivellement>();
            var transactionAjoutProtection = Auto.Create<AjoutProtection>();
            var transactions = new List<Transaction>
            {
                transactionTerminaison,
                transactionChangementUsageTabac,
                transactionReductionCapital,
                transactionNivellement,
                transactionAjoutProtection
            };

            var donnees = Auto.Create<DonneesRapportIllustration>();
            var dictModification = new Dictionary<string, ModificationsProtection>
            {
                {
                    "p1", new ModificationsProtection
                    {
                        Protection = Auto.Create<Protection>(),
                        Transactions = transactions
                    }
                }
            };

            donnees.ModificationsDemandees.Protections = dictModification;

            var definition = new DefinitionSection
            {
                SectionId = "ModificationsDemandees",
                Titres = titreSection,
                ListSections = new List<DefinitionSection>
                {
                    new DefinitionSection {Titres = titreSectionProtections, SectionId = "Protections"}
                }
            };

            _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(Arg.Any<string>(), Arg.Any<Produit>()).Returns(definition);
            _formatter.FormatterTitre(titreSection.FirstOrDefault(), donnees).Returns(titreSection.First().Titre);
            _formatter.FormatterTitre(titreSectionProtections.FirstOrDefault(), donnees).Returns(titreSectionProtections.First().Titre);

            var reportContext = Auto.Create<IReportContext>();
            reportContext.Language = Language.English;
            var factory = new ModificationsDemandeesModelFactory(_configurationRepository, _resourcesAccessor, 
                new SectionModelMapper(_formatter, _noteManager, _tableauManager, _titreManager, _imageManager));

            var model = factory.Build(definition.SectionId, donnees, reportContext);

            using (new AssertionScope())
            {
                model.TitreSection.Should().Be(titreSection.First().Titre);
                model.SectionProtectionsModel.TitreSection.Should().Be(titreSectionProtections.First().Titre);

                var index = 0;
                foreach (var protection in model.SectionProtectionsModel.Protections)
                {
                    var source = donnees.ModificationsDemandees.Protections.Values.Skip(index).First().Protection;
                    protection.Libelle.Should().Be(source.Plan.DescriptionAn);
                    protection.Capital.Should().Be(source.Plan.SansVolumeAssurance ? null : source.CapitalAssureActuel);
                    protection.Assures.Should().HaveSameCount(source.Assures.Individus);
                    protection.Assures.Last().Nom.Should().Be(source.Assures.Individus.Last().Nom);
                    protection.Assures.Last().Prenom.Should().Be(source.Assures.Individus.Last().Prenom);
                    protection.Assures.Last().Initiale.Should().Be(source.Assures.Individus.Last().Initiale);
                    protection.Transactions.Should().HaveSameCount(transactions);

                    var t1 = protection.Transactions.FirstOrDefault(x => x is TransactionTerminaisonModel) as TransactionTerminaisonModel;
                    t1.Should().NotBeNull();
                    t1?.Annee.Should().Be(transactionTerminaison.Annee);

                    var t2 = protection.Transactions.FirstOrDefault(x => x is TransactionChangementUsageTabacModel) as TransactionChangementUsageTabacModel;
                    t2.Should().NotBeNull();
                    t2?.Annee.Should().Be(transactionChangementUsageTabac.Annee);
                    t2?.Initiale.Should().Be(transactionChangementUsageTabac.Initiale);
                    t2?.Nom.Should().Be(transactionChangementUsageTabac.Nom);
                    t2?.Prenom.Should().Be(transactionChangementUsageTabac.Prenom);
                    t2?.StatutTabagisme.Should().Be(transactionChangementUsageTabac.StatutTabagisme);

                    var t3 = protection.Transactions.FirstOrDefault(x => x is TransactionReductionCapitalModel) as TransactionReductionCapitalModel;
                    t3.Should().NotBeNull();
                    t3?.Annee.Should().Be(transactionReductionCapital.Annee);
                    t3?.Montant.Should().Be(transactionReductionCapital.Montant);

                    var t4 = protection.Transactions.FirstOrDefault(x => x is TransactionNivellementModel) as TransactionNivellementModel;
                    t4.Should().NotBeNull();
                    t4?.Annee.Should().Be(transactionNivellement.Annee);
                    t4?.Age.Should().Be(transactionNivellement.Age);
                    t4?.AgeSurprime.Should().Be(transactionNivellement.AgeSurprime);
                    index += 1;

                    var t5 = protection.Transactions.FirstOrDefault(x => x is TransactionAjoutProtectionModel) as TransactionAjoutProtectionModel;
                    t5.Should().NotBeNull();
                    t5?.Annee.Should().Be(transactionAjoutProtection.Annee);
                    t5?.MontantPrime.Should().Be(transactionAjoutProtection.MontantPrime);
                    index += 1;
                }
            }
        }

        [TestMethod]
        public void MapperTransactionAjoutOption_WhenMontantPrimeIsNull_Fr_ThenDescriptionMontantPrimeIsEmpty()
        {
            var results = new List<TransactionModel>();
            var transactions = new List<AjoutOption>()
            {
                new AjoutOption()
                {
                    Annee = 3,
                    MontantPrime = null,
                    Plan = new Plan()
                    {
                        DescriptionFr = "Description fr.",
                        DescriptionAn = "Description En"
                    }
                }
            };

            _factory.SetField("_languageIsFrench", BindingFlags.NonPublic | BindingFlags.Instance, true);
            _factory.Invoke("MapperTransactionAjoutOption", results, transactions);

            using (new AssertionScope())
            {
                results.Should().NotBeNullOrEmpty();
                results.Should().HaveCount(1);
                var transactionAjoutOption = results.First() as TransactionAjoutOptionModel;
                transactionAjoutOption?.DescpriptionMontantPrime.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void MapperTransactionNivellements_En_ThenResultISNotEmpty()
        {
            var results = new List<TransactionModel>();
            var transactions = new List<Nivellement>()
            {
                new Nivellement()
                {
                    Annee = 10,
                    Age = 45,
                    AgeSurprime = 47
                }
            };

            _factory.SetField("_languageIsFrench", BindingFlags.NonPublic | BindingFlags.Instance, false);
            _factory.Invoke("MapperTransactionNivellements", results, transactions);

            using (new AssertionScope())
            {
                results.Should().NotBeNullOrEmpty();
                results.Should().HaveCount(1);
                var transactionNivellement = results.First() as TransactionNivellementModel;
                transactionNivellement?.Age.Should().Be(45);
                transactionNivellement?.AgeSurprime.Should().Be(47);
                transactionNivellement?.Annee.Should().Be(10);
                transactionNivellement?.DescpriptionAge.Should().Be("Leveling age: {0}");
            }
        }

        [TestMethod]
        public void MapperTransactionReductionCapital_Fr_ThenResultIsNotEmpty()
        {
            var results = new List<TransactionModel>();
            var transactions = new List<ReductionCapital>()
            {
                new ReductionCapital()
                {
                    Annee = 10,
                    Montant = 50000
                }
            };

            _factory.SetField("_languageIsFrench", BindingFlags.NonPublic | BindingFlags.Instance, true);
            _factory.Invoke("MapperTransactionReductionCapital", results, transactions);

            using (new AssertionScope())
            {
                results.Should().NotBeNullOrEmpty();
                results.Should().HaveCount(1);
                var transactionReductionCapital = results.First() as TransactionReductionCapitalModel;
                transactionReductionCapital?.Montant.Should().Be(50000);
                transactionReductionCapital?.DescpriptionMontant.Should().Be("Nouveau capital assuré : {0}");
            }
        }

        [TestMethod]
        public void MapperTransactionChangementOptionParticipation_Fr_ThenResultIsNotEmpty()
        {
            var results = new List<TransactionModel>();
            var transactions = new List<ChangementOptionParticipation>
            {
                new ChangementOptionParticipation
                {
                    Annee = 10,
                    Option = TypeOptionParticipation.Comptant
                }
            };

            _factory.SetField("_languageIsFrench", BindingFlags.NonPublic | BindingFlags.Instance, true);
            _factory.Invoke("MapperTransactionChangementOptionParticipation", results, transactions);

            using (new AssertionScope())
            {
                results.Should().NotBeNullOrEmpty();
                results.Should().HaveCount(1);
                var transactionModel = results.First() as TransactionChangementOptionParticipantModel;
                transactionModel?.Annee.Should().Be(10);
                transactionModel?.Option.Should().Be(TypeOptionParticipation.Comptant);
                transactionModel?.Descpription.Should().Be("Changement de l'option d'affectation des participations à l'année {0}");
            }
        }

        [TestMethod]
        public void MapperTransactionChangementOptionParticipation_En_ThenResultIsNotEmpty()
        {
            var results = new List<TransactionModel>();
            var transactions = new List<ChangementOptionParticipation>
            {
                new ChangementOptionParticipation
                {
                    Annee = 10,
                    Option = TypeOptionParticipation.Comptant
                }
            };

            _factory.SetField("_languageIsFrench", BindingFlags.NonPublic | BindingFlags.Instance, false);
            _factory.Invoke("MapperTransactionChangementOptionParticipation", results, transactions);

            using (new AssertionScope())
            {
                results.Should().NotBeNullOrEmpty();
                results.Should().HaveCount(1);
                var transactionModel = results.First() as TransactionChangementOptionParticipantModel;
                transactionModel?.Annee.Should().Be(10);
                transactionModel?.Option.Should().Be(TypeOptionParticipation.Comptant);
                transactionModel?.Descpription.Should().Be("Change of the dividend option in year {0}");
            }
        }
    }
}
