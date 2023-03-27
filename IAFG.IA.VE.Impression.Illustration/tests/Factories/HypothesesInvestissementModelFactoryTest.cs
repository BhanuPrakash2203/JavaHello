using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Illustration.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.HypothesesInvestissement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Fonds = IAFG.IA.VE.Impression.Illustration.Types.Models.HypothesesInvestissement.Fonds;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Factories
{
    [TestClass]
    public class HypothesesInvestissementModelFactoryTest
    {

        private IRegleAffaireAccessor _regleAffaireAccessor;
        private IConfigurationRepository _configurationRepository;
        private IIllustrationReportDataFormatter _formatter;
        private ISectionModelMapper _sectionModelMapper;
        private IDefinitionTexteManager _texteManager;

        private PrivateObject _hypothesesInvestissementModelFactory;

        [TestInitialize]
        public void Initialize()
        {
            _regleAffaireAccessor = Substitute.For<IRegleAffaireAccessor>();
            _configurationRepository = Substitute.For<IConfigurationRepository>();
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _sectionModelMapper = Substitute.For<ISectionModelMapper>();
            _texteManager = Substitute.For<IDefinitionTexteManager>();

            _hypothesesInvestissementModelFactory = new PrivateObject(
                typeof(HypothesesInvestissementModelFactory), _regleAffaireAccessor, _configurationRepository, _formatter, _sectionModelMapper, _texteManager);
        }

        [TestMethod]
        public void MapperSoldeFonds_WhenComptesIsEmpty_Valid()
        {
            var result = _hypothesesInvestissementModelFactory.Invoke(
                "MapperSoldeFonds",
                "description",
                1964.74,
                Enumerable.Empty<DetailCompte>(),
                DateTime.Today,
                Core.Types.Enums.Language.French) as List<DetailCompte>;

            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.First().Solde.Should().Be(1964.74);
                result.First().EstSoldeTotal.Should().BeTrue();
                result.First().OrdreTri.Should().Be(99);
            }
        }

        [TestMethod]
        public void MapperSoldeFonds_Valid()
        {
            var detailComptes = new List<DetailCompte>()
            {
                new DetailCompte()
                {
                    Description = "description detail compte",
                    EstSoldeTotal = false,
                    OrdreTri = 1
                }
            };

            var result = _hypothesesInvestissementModelFactory.Invoke(
                "MapperSoldeFonds",
                "description",
                1964.74,
                detailComptes,
                DateTime.Today,
                Core.Types.Enums.Language.French) as List<DetailCompte>;

            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.First().Solde.Should().Be(1964.74);
                result.First().EstSoldeTotal.Should().BeTrue();
                result.First().OrdreTri.Should().Be(99);
            }
        }

        [TestMethod]
        public void MapperComptesFonds_WhenFondsIsEmpty_ReturnsListWithOneItem()
        {
            var result = _hypothesesInvestissementModelFactory
                .Invoke("MapperComptesFonds", "description", 
                    Enumerable.Empty<Fonds>(), null,
                    DateTime.Today, 
                    null,
                    Core.Types.Enums.Language.French) as IList<DetailCompte>;

            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.First().Description.Should().Be("description");
            }
            
        }

        [TestMethod]
        public void MapperComptesFonds_MoisDebutSuperieurAUn_Valid()
        {
            var fonds = new List<Fonds>()
            {
                new Fonds
                {
                    AnneeDebut = 5, MoisDebut = 2, Vehicule = "M5A080"
                }
            };

            var donnees = new DonneesRapportIllustration
            {
                Vehicules = new List<Vehicule>
                {
                    new Vehicule { Vehicle = "M5A080", DescriptionFr = "DescriptionFr_M5A080" }
                }
            };

            var result = _hypothesesInvestissementModelFactory
                .Invoke("MapperComptesFonds", "description", 
                    fonds, null, 
                    DateTime.Today,
                    donnees,
                    Core.Types.Enums.Language.French) as List<DetailCompte>;

            using (new AssertionScope())
            {
                result.Should().HaveCount(2);
                var firstOne = result.First();
                firstOne.AnneeDebut.Should().Be(5);
                firstOne.MoisDebut.Should().Be(2);
                firstOne.Description.Should().Be("DescriptionFr_M5A080");
            }
        }

        [TestMethod]
        public void MapperComptesFonds_MoisDebutEgalAUn_Valid()
        {
            var fonds = new List<Fonds>
            {
                new Fonds { AnneeDebut = 5, MoisDebut = 1, Vehicule = "M5A080" }
            };

            var donnees = new DonneesRapportIllustration
            {
                Vehicules = new List<Vehicule>
                {
                    new Vehicule { Vehicle = "M5A080", DescriptionFr = "DescriptionFr_M5A080" }
                }
            };

            var result = _hypothesesInvestissementModelFactory
                .Invoke("MapperComptesFonds", "description", 
                    fonds, null, 
                    DateTime.Today, donnees,
                    Core.Types.Enums.Language.French) as IList<DetailCompte>;

            using (new AssertionScope())
            {
                result.Should().HaveCount(2);
                var firstOne = result.First();
                firstOne.AnneeDebut.Should().Be(5);
                firstOne.MoisDebut.Should().BeNull();
                firstOne.Description.Should().Be("DescriptionFr_M5A080");
            }
        }
    }
}
