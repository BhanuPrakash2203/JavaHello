using System.Linq;
using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Factories
{
    [TestClass]
    public class PrimesRenouvellementModelFactoryTest
    {
        private readonly IFixture _auto = AutoFixtureFactory.Create();
        private readonly IConfigurationRepository _configurationRepository = Substitute.For<IConfigurationRepository>();
        private readonly IIllustrationReportDataFormatter _formatter = Substitute.For<IIllustrationReportDataFormatter>();
        private readonly IDefinitionNoteManager _noteManager = Substitute.For<IDefinitionNoteManager>();
        private readonly IDefinitionTableauManager _tableauManager = Substitute.For<IDefinitionTableauManager>();

        [TestMethod]
        public void PrimesRenouvellementModelFactory_WHEN_Build_Then_ReturnPagePrimesRenouvellementModel()
        {
            var definition = _auto.Create<DefinitionSection>();
            var donnees = _auto.Create<DonneesRapportIllustration>();

            _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(Arg.Any<string>(), Arg.Any<Produit>()).Returns(definition);
            _formatter.FormatterTitre(definition.Titres.First(), donnees).Returns(definition.Titres.First().Titre);
            var factory = new PrimesRenouvellementModelFactory(_configurationRepository, _formatter, 
                new SectionModelMapper(_formatter, _noteManager, _tableauManager, new DefinitionTitreManager(_formatter), new DefinitionImageManager()));

            var model = factory.Build("1", donnees, _auto.Create<IReportContext>());

            model.TitreSection.Should().Be(definition.Titres.First().Titre);
            model.SectionPrimesRenouvellementModels.Should().NotBeNull();
        }
    }
}