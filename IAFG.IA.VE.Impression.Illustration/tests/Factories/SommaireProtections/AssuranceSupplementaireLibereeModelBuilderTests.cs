using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.ReportContext;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Factories.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections.ASL;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
namespace IAFG.IA.VE.Impression.Illustration.Tests.Factories.SommaireProtections
{
    [TestClass]

    public class AssuranceSupplementaireLibereeModelBuilderTests
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private readonly IReportContext _reportContext = new ReportContext();
        private readonly ISectionModelMapper _sectionModelMapper = Substitute.For<ISectionModelMapper>();

        [TestInitialize]
        public void Initialize()
        {
            _sectionModelMapper.MapperDefinition(Arg.Any<SectionASLModel>(), Arg.Any<DefinitionSection>(),
                Arg.Any<DonneesRapportIllustration>(), Arg.Any<IReportContext>()).Returns(new SectionASLModel());
        }

        [TestMethod]
        public void Build_When_Definition_Is_Null_Then_Returns_Null()
        {
            //Arrange
            var donnees = Auto.Create<DonneesRapportIllustration>();
            donnees.Produit = Produit.AssuranceParticipant;

            var builder = new AssuranceSupplementaireLibereeModelBuilder(_sectionModelMapper);

            //Act
            var model = builder.Build(null, donnees, _reportContext);

            //Assert
            model.Should().BeNull();
        }

        [TestMethod]
        public void Build_When_AssuranceSupplementaireLiberee_Is_Null_Then_Returns_Null()
        {
            //Arrange
            var definition = Auto.Create<DefinitionSection>();
            var donnees = Auto.Create<DonneesRapportIllustration>();
            donnees.Produit = Produit.AssuranceParticipant;
            donnees.AssuranceSupplementaireLiberee = null;

            var builder = new AssuranceSupplementaireLibereeModelBuilder(_sectionModelMapper);

            //Act
            var model = builder.Build(definition, donnees, _reportContext);

            //Assert
            model.Should().BeNull();
        }

        [TestMethod]
        public void Build_When_Produit_Other_Than_CapitalValeur_And_CapitalValeur3_Then_Returns_Null()
        {
            //Arrange
            var definition = Auto.Create<DefinitionSection>();
            var donnees = Auto.Create<DonneesRapportIllustration>();
            donnees.Produit = Produit.AssuranceParticipant;

            var builder = new AssuranceSupplementaireLibereeModelBuilder(_sectionModelMapper);

            //Act
            var model = builder.Build(definition, donnees, _reportContext);

            //Assert
            model.Should().BeNull();
        }

        [TestMethod]
        public void Build_When_Produit_CapitalValeur_Without_Taux_And_Without_Allocations_Then_Returns_ExpectedValues()
        {
            //Arrange
            const double capitalAssureMaximal = 12345.67;
            const double montantAllocationInitial = 987.65;
            const int anneeDebutProjection = 10;

            var definition = Auto.Create<DefinitionSection>();
            var donnees = Auto.Create<DonneesRapportIllustration>();
            donnees.Produit = Produit.CapitalValeur;
            donnees.Projections.AnneeDebutProjection = anneeDebutProjection;

            donnees.AssuranceSupplementaireLiberee = new AssuranceSupplementaireLiberee()
            {
                CapitalAssureMaximal = capitalAssureMaximal,
                MontantAllocationInitial = montantAllocationInitial,
                OptionVersementBoni = TypeOptionVersementBoni.Aucun,
                Allocations = null,
                TauxAnnees = null
            };

            var builder = new AssuranceSupplementaireLibereeModelBuilder(_sectionModelMapper);

            //Act
            var model = builder.Build(definition, donnees, _reportContext);

            //Assert
            using (new AssertionScope())
            {
                model.OptionVersementBoni.Should().Be(TypeOptionVersementBoni.Aucun);
                model.CapitalAssureMaximal.Should().Be(capitalAssureMaximal);
                model.AucunAchat.Should().BeFalse();
                model.AucunMaximum.Should().BeFalse();
                model.Taux.Should().BeEmpty();
                model.Allocations.Should().HaveCount(1);
                model.Allocations[0].AnneeDebut.Should().Be(anneeDebutProjection);
                model.Allocations[0].Montant.Should().Be(montantAllocationInitial);

            }
        }

        [TestMethod]
        public void Build_When_Produit_CapitalValeur3_With_One_Taux_And_One_Allocation_Then_Returns_ExpectedValues()
        {
            //Arrange
            const double capitalAssureMaximal = 0.008;
            const double montantAllocationInitial = 987.65;
            const double taux = 0.005;
            const double montantAllocation = 123.45;

            const int anneeDebutProjection = 10;
            const int anneeTaux = 11;
            const int anneeAllocation = 12;

            var definition = Auto.Create<DefinitionSection>();
            var donnees = Auto.Create<DonneesRapportIllustration>();
            donnees.Produit = Produit.CapitalValeur;
            donnees.Projections.AnneeDebutProjection = anneeDebutProjection;

            donnees.AssuranceSupplementaireLiberee = new AssuranceSupplementaireLiberee()
            {
                CapitalAssureMaximal = capitalAssureMaximal,
                MontantAllocationInitial = montantAllocationInitial,
                OptionVersementBoni = TypeOptionVersementBoni.Aucun,
                Allocations = new List<Allocation>{new Allocation(){Annee = anneeAllocation, Montant = montantAllocation } },
                TauxAnnees = new List<TauxAnnee>{new TauxAnnee(){Annee = anneeTaux, Taux = taux } }
            };

            var builder = new AssuranceSupplementaireLibereeModelBuilder(_sectionModelMapper);

            //Act
            var model = builder.Build(definition, donnees, _reportContext);

            //Assert
            using (new AssertionScope())
            {
                model.OptionVersementBoni.Should().Be(TypeOptionVersementBoni.Aucun);
                model.CapitalAssureMaximal.Should().Be(capitalAssureMaximal);
                model.AucunAchat.Should().BeTrue();
                model.AucunMaximum.Should().BeFalse();
                model.Taux.Should().HaveCount(1);
                model.Taux[0].AnneeDebut.Should().Be(anneeTaux);
                model.Taux[0].Taux.Should().Be(taux);
                model.Allocations.Should().HaveCount(2);
                model.Allocations[0].AnneeDebut.Should().Be(anneeDebutProjection);
                model.Allocations[0].Montant.Should().Be(montantAllocationInitial);
                model.Allocations[1].AnneeDebut.Should().Be(anneeAllocation);
                model.Allocations[1].Montant.Should().Be(montantAllocation);

            }
        }
    }
}
