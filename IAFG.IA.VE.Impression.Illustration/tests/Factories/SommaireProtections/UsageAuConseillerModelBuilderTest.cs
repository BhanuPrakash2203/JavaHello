using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.ReportContext;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Factories.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Rules;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Factories.SommaireProtections
{
    [TestClass]
    public class UsageAuConseillerModelBuilderTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private readonly IIllustrationReportDataFormatter _formatter = Substitute.For<IIllustrationReportDataFormatter>();
        private readonly IDefinitionNoteManager _noteManager = Substitute.For<IDefinitionNoteManager>();
        private readonly IDefinitionTableauManager _tableauManager = Substitute.For<IDefinitionTableauManager>();
        private readonly IVecteurManager _vecteurManager = Substitute.For<IVecteurManager>();
        private readonly IProductRules _productRules = Substitute.For<IProductRules>();
        private readonly IReportContext _reportContext = new ReportContext();
        private ISectionModelMapper _sectionModelMapper;

        [TestInitialize]
        public void Initialize()
        {
            _sectionModelMapper = new SectionModelMapper(_formatter, _noteManager, _tableauManager, 
                new DefinitionTitreManager(_formatter), new DefinitionImageManager());

            _vecteurManager.ObtenirVecteurMontantNetAuRisque(Arg.Any<Projections>()).Returns(new[] { 110, 122.25, 77 });
            _vecteurManager.TrouverAnneeSelonIndex(Arg.Any<Projection>(), Arg.Any<int>()).Returns(99);

            _productRules.EstParmiFamilleAssuranceParticipants(Produit.AssuranceParticipant).Returns(true);
            _productRules.EstParmiFamilleAssuranceParticipants(Produit.AccesVie).Returns(false);
        }

        [TestMethod]
        public void BuildUsageAuConseillerModel_When_ThenBuildModelCorrectly()
        {
            var definition = Auto.Create<DefinitionSection>();
            var donnees = Auto.Create<DonneesRapportIllustration>();
            donnees.Produit = Produit.AssuranceParticipant;

            var builder = new UsageAuConseillerModelBuilder(_sectionModelMapper, _vecteurManager, _productRules);
            var model = builder.Build(definition, donnees, _reportContext);
            using (new AssertionScope())
            {
                model.MontantNetAuRisque.Should().NotBeNull();
                model.MontantNetAuRisque.Annee.Should().Be(99);
                model.MontantNetAuRisque.Montant.Should().Be(122.25);
            }
        }

        [TestMethod]
        public void BuildUsageAuConseillerModel_WhenDefinitionIsNull_ThenNull()
        {
            var donnees = Auto.Create<DonneesRapportIllustration>();
            donnees.Produit = Produit.AccesVie;

            var builder = new UsageAuConseillerModelBuilder(_sectionModelMapper, _vecteurManager, _productRules);
            var model = builder.Build(null, donnees, _reportContext);
            model.Should().BeNull();
        }

        [TestMethod]
        public void BuildUsageAuConseillerModel_WhenProduitIsAccesVie_ThenNull()
        {
            var definition = Auto.Create<DefinitionSection>();
            var donnees = Auto.Create<DonneesRapportIllustration>();
            donnees.Produit = Produit.AccesVie;

            var builder = new UsageAuConseillerModelBuilder(_sectionModelMapper, _vecteurManager, _productRules);
            var model = builder.Build(definition, donnees, _reportContext);
            model.Should().BeNull();
        }

        [TestMethod]
        public void BuildUsageAuConseillerModel_WhenVecteurIsEmpty_ThenNull()
        {
            var definition = Auto.Create<DefinitionSection>();
            var donnees = Auto.Create<DonneesRapportIllustration>();
            donnees.Produit = Produit.AssuranceParticipant;

            var vecteurManager = Substitute.For<IVecteurManager>();
            vecteurManager.ObtenirVecteurMontantNetAuRisque(Arg.Any<Projections>()).Returns(new double[] { });

            var builder = new UsageAuConseillerModelBuilder(_sectionModelMapper, vecteurManager, _productRules);
            var model = builder.Build(definition, donnees, _reportContext);
            model.Should().BeNull();
        }
    }
}
