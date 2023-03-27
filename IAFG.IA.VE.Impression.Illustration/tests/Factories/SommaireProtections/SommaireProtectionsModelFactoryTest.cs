using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Factories.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Rules;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections.Participations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Factories.SommaireProtections
{
    [TestClass]
    public class SommaireProtectionsModelFactoryTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private IConfigurationRepository _configurationRepository;
        private IIllustrationReportDataFormatter _formatter;
        private SommaireProtectionsModelFactory _modelFactory;
        private DefinitionSection _definition;
        private DonneesRapportIllustration _donneesRapportIllustration;
        private readonly IDefinitionNoteManager _noteManager = Substitute.For<IDefinitionNoteManager>();
        private readonly IDefinitionTableauManager _tableauManager = Substitute.For<IDefinitionTableauManager>();
        private IVecteurManager _vecteurManager;
        private IProductRules _productRules;
        private IDefinitionTitreManager _titreManager;
        private IDefinitionImageManager _imageManager;

        [TestInitialize]
        public void Initialize()
        {
            _configurationRepository = Substitute.For<IConfigurationRepository>();

            _vecteurManager = Substitute.For<IVecteurManager>();
            _productRules = Substitute.For<IProductRules>();
            _definition = Auto.Create<DefinitionSection>();
            _donneesRapportIllustration = Auto.Create<DonneesRapportIllustration>();
            _definition = Auto.Create<DefinitionSection>();
            _definition.ListSections.Add(new DefinitionSection { SectionId = "Identifications" });
            _definition.ListSections.Add(new DefinitionSection { SectionId = "Protections" });
            _definition.ListSections.Add(new DefinitionSection { SectionId = "Primes" });
            _definition.ListSections.Add(new DefinitionSection { SectionId = "ASL" });
            _definition.ListSections.Add(new DefinitionSection { SectionId = "FluxMonetaire" });
            _definition.ListSections.Add(new DefinitionSection { SectionId = "Surprimes" });
            _definition.ListSections.Add(new DefinitionSection { SectionId = "DetailParticipations" });
            _definition.ListSections.Add(new DefinitionSection { SectionId = "AvancesSurPolice" });

            _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(Arg.Any<string>(), Arg.Any<Produit>()).Returns(_definition);
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _formatter.FormatterTitre(Arg.Any<DefinitionTitreDescriptionSelonProduit>(), _donneesRapportIllustration).Returns(_definition.Titres.First().Titre);

            _titreManager = new DefinitionTitreManager(_formatter);
            _imageManager = new DefinitionImageManager();

            var sectionModelMapper =
                new SectionModelMapper(_formatter, _noteManager, _tableauManager, _titreManager, _imageManager);

            _modelFactory = new SommaireProtectionsModelFactory(
                _configurationRepository, 
                _formatter, 
                sectionModelMapper,
                _productRules,
                new UsageAuConseillerModelBuilder(sectionModelMapper, _vecteurManager, _productRules),
                new AssuranceSupplementaireLibereeModelBuilder(sectionModelMapper));
        }

        [TestMethod]
        public void MapperIdentifications_When_ThenMapSectionsCorrectly()
        {
            _donneesRapportIllustration.NumeroContrat = "test";
            _donneesRapportIllustration.ProvinceEtat = ProvinceEtat.Alabama;
            _donneesRapportIllustration.ContractantEstCompagnie = true;
            _donneesRapportIllustration.TauxMarginal = 56;
            _donneesRapportIllustration.Clients = new List<Client>()
            {
                new Client
                {
                    DateNaissance = DateTime.Now,
                    Nom = "test",
                    Prenom = "test",
                    Initiale = "TT",
                    Sexe = Sexe.Femme,
                    EstContractant = true,
                    SequenceIndividu = 1
                }
            };

            var model = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());
            using (new AssertionScope())
            {
                model.TitreSection.Should().Be(_definition.Titres.First().Titre);
                model.SectionIdendification.Should().NotBeNull();
            }
        }

        [TestMethod]
        public void MapperDetailParticipations_WhenSoldeParticipationsEnDepotNotNull_ThenMapSectionsCorrectly()
        {
            const double solde = 200.25D;

            _donneesRapportIllustration.NumeroContrat = "test";
            _donneesRapportIllustration.Participations = new Participations
            {
                OptionParticipation = TypeOptionParticipation.Depot,
                SoldeParticipationsEnDepot = solde,
                ReductionBaremeParticipation = -1.5
            };
            
            var model = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());
            using (new AssertionScope())
            {
                model.TitreSection.Should().Be(_definition.Titres.First().Titre);
                model.SectionDetailParticipations.Should().NotBeNull();
                model.SectionDetailParticipations.OptionParticipation.Should().Be(TypeOptionParticipation.Depot);
                model.SectionDetailParticipations.SoldeParticipationsEnDepot.Should().Be(solde);
            }
        }

        [TestMethod]
        public void MapperDetailParticipations_WhenSoldeParticipationsEnDepotNull_ThenMapSectionsCorrectly()
        {
            _donneesRapportIllustration.NumeroContrat = "test";
            _donneesRapportIllustration.Participations = new Participations
            {
                OptionParticipation = TypeOptionParticipation.Depot,
                ReductionBaremeParticipation = -1.5
            };

            var model = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());
            using (new AssertionScope())
            {
                model.TitreSection.Should().Be(_definition.Titres.First().Titre);
                model.SectionDetailParticipations.Should().NotBeNull();
                model.SectionDetailParticipations.OptionParticipation.Should().Be(TypeOptionParticipation.Depot);
                model.SectionDetailParticipations.SoldeParticipationsEnDepot.Should().BeNull();
            }
        }
        [TestMethod]
        public void MapperDetailParticipations_NonSpecifie_When_ThenMapSectionsNull()
        {
            _donneesRapportIllustration.NumeroContrat = "test";
            _donneesRapportIllustration.Participations = new Participations
            {
                OptionParticipation = TypeOptionParticipation.NonSpecifie
            };

            var model = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());
            using (new AssertionScope())
            {
                model.TitreSection.Should().Be(_definition.Titres.First().Titre);
                model.SectionDetailParticipations.Should().BeNull();
            }
        }

        [TestMethod]
        public void MapperDetailParticipations_Null_When_ThenMapSectionsNull()
        {
            _donneesRapportIllustration.NumeroContrat = "test";
            _donneesRapportIllustration.Participations = null;

            var model = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());
            using (new AssertionScope())
            {
                model.TitreSection.Should().Be(_definition.Titres.First().Titre);
                model.SectionDetailParticipations.Should().BeNull();
            }
        }

        [TestMethod]
        public void MapperValeurMaximisee_When_ThenMapSectionsCorrectly()
        {
            _donneesRapportIllustration.NumeroContrat = "test";
            _donneesRapportIllustration.Projections = new Projections { AnneeDebutProjection = 40, Projection = new Projection() };
            _donneesRapportIllustration.Protections.ValeurMaximisee = new Types.Models.SommaireProtections.ValeurMaximisee
            {
                CapitalAssurePlafond = 100.33,
                CapitalAssurePlancher = 200.77,
                DureeDebutMinimisation = 33
            };

            var model = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());
            using (new AssertionScope())
            {
                model.TitreSection.Should().Be(_definition.Titres.First().Titre);
                model.SectionProtections.ValeurMaximisee.Should().NotBeNull();
                model.SectionProtections.ValeurMaximisee.CapitalAssurePlancher.Should().Be(200.77);
                model.SectionProtections.ValeurMaximisee.CapitalAssurePlafond.Should().Be(100.33);
                model.SectionProtections.ValeurMaximisee.DureeDebutMinimisation.Should().BeNull();
            }

            //Cas avec annee debut projection inferieure;
            _donneesRapportIllustration.Projections = new Projections { AnneeDebutProjection = 25, Projection = new Projection() };
            model = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());
            using (new AssertionScope())
            {
                model.SectionProtections.ValeurMaximisee.Should().NotBeNull();
                model.SectionProtections.ValeurMaximisee.CapitalAssurePlancher.Should().Be(200.77);
                model.SectionProtections.ValeurMaximisee.CapitalAssurePlafond.Should().BeNull();
                model.SectionProtections.ValeurMaximisee.DureeDebutMinimisation.Should().Be(33);
            }
        }

        [TestMethod]
        public void MapperValeurMaximisee_Null_When_ThenMapSectionsCorrectly()
        {
            _donneesRapportIllustration.NumeroContrat = "test";
            _donneesRapportIllustration.Protections.ValeurMaximisee = null;

            var model = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());
            using (new AssertionScope())
            {
                model.TitreSection.Should().Be(_definition.Titres.First().Titre);
                model.SectionProtections.ValeurMaximisee.Should().BeNull();
            }
        }

        [TestMethod]
        public void MapperAvancesSurPolice_WhenDonneesAvancesSurPoliceNulles_ThenMapSectionNull()
        {
            _donneesRapportIllustration.NumeroContrat = "test";
            _donneesRapportIllustration.AvancesSurPolice = null;

            var model = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());
            using (new AssertionScope())
            {
                model.TitreSection.Should().Be(_definition.Titres.First().Titre);
                model.SectionAvancesSurPolice.Should().BeNull();
            }
        }

        [TestMethod]
        public void MapperAvancesSurPolice_WhenDonneesAvancesSurPolicePresentes_ThenMapSectionCorrectly()
        {
            var dateDerniereMiseAJour = new DateTime(2021, 08, 01);
            const double solde = 123.45D;

            _donneesRapportIllustration.NumeroContrat = "test";
            _donneesRapportIllustration.AvancesSurPolice = new AvancesSurPolice
            {
                DateDerniereMiseAJour = dateDerniereMiseAJour,
                Solde = solde
            };

            var model = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());
            using (new AssertionScope())
            {
                model.TitreSection.Should().Be(_definition.Titres.First().Titre);
                model.SectionAvancesSurPolice.Should().NotBeNull();
                model.SectionAvancesSurPolice.DateDerniereMiseAJour.Should().BeSameDateAs(dateDerniereMiseAJour);
                model.SectionAvancesSurPolice.Solde.Should().Be(solde);
            }
        }
    }
}
