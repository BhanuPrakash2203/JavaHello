using System;
using FluentAssertions;
using IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VI.AF.IPDFVie.Factory.Interfaces;
using IAFG.IA.VI.AF.IPDFVie.PDF;
using IAFG.IA.VI.AF.IPDFVie.PDF.Plan.ENUMs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace IAFG.IA.VE.Impression.Illustration.Test.ReglesPDF
{
    [TestClass]
    public class RegleAffaireAccessorTest
    {      
        [TestMethod]
        public void ObtenirPlan_PlanNonGere_ThenArgumentOutOfRangeException()
        {
            var pdfFactory = Substitute.For<IFactory>();
            pdfFactory.GetIReglesPlan(Arg.Any<string>()).ReturnsNull();

            var regleAccessor = new RegleAffaireAccessor(pdfFactory);
            Action action = () => regleAccessor.ObtenirPlan("Test");
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [DataRow(DureeRevenuAppoint.A65Ans, TypePrestationPlan.PrestationMensuelle65Ans)]
        [DataRow(DureeRevenuAppoint.Duree2Ans, TypePrestationPlan.PrestationMensuellePour2Ans)]
        [DataRow(DureeRevenuAppoint.Duree5Ans, TypePrestationPlan.PrestationMensuellePour5Ans)]
        [DataRow(DureeRevenuAppoint.NonApplicable, TypePrestationPlan.NonApplicable)]
        [DataTestMethod]
        public void ObtenirPlan_DeterminerPrestationPlan_ThenGoodResultn(DureeRevenuAppoint dureeRevenuAppoint, TypePrestationPlan typePrestationPlan)
        {
            var pdfFactory = Substitute.For<IFactory>();
            var reglesPlan = Substitute.For<IReglesPlan>();
            reglesPlan.DureeRevenuAppoint.Returns(dureeRevenuAppoint);
            pdfFactory.GetIReglesPlan(Arg.Any<string>()).Returns(reglesPlan);

            var regleAccessor = new RegleAffaireAccessor(pdfFactory);
            regleAccessor.ObtenirPlan("Test").TypePrestationPlan.Should().Be(typePrestationPlan);
        }

        [TestMethod]
        public void ObtenirPlan_DureeRevenuAppointNonGerer_ThenArgumentOutOfRangeException()
        {
            var pdfFactory = Substitute.For<IFactory>();
            var reglesPlan = Substitute.For<IReglesPlan>();
            reglesPlan.DureeRevenuAppoint.Returns((DureeRevenuAppoint) (-8888));
            pdfFactory.GetIReglesPlan(Arg.Any<string>()).Returns(reglesPlan);

            var regleAccessor = new RegleAffaireAccessor(pdfFactory);
            Action action = () => regleAccessor.ObtenirPlan("Test");
            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
