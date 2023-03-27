using FluentAssertions;
using IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VI.AF.IPDFVie.Factory.Interfaces;
using IAFG.IA.VI.AF.IPDFVie.PDF.Plan.ENUMs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.ReglesPDF
{
    [TestClass]
    public class RegleAffaireExtensionTest
    {
        private readonly IGetPDFICoverageResponse _getPDFICoverageResponse = Substitute.For<IGetPDFICoverageResponse>();

        [TestMethod]
        public void DeterminerInfoProtection_WhenNull_ThenHasFlagAucun()
        {
            IGetPDFICoverageResponse response = null;
            var infoProtection = response.DeterminerInfoProtection();
            infoProtection.HasFlag(InfoProtection.Aucun).Should().BeTrue();
        }

        [TestMethod]
        public void DeterminerInfoProtection_WhenAssTempoRenouvEtRembPrimeDeces_ThenHasFlagRenouvellable()
        {
            _getPDFICoverageResponse.Coverage.Plan.TypeProtectionInfoAdd.Returns(TypeProtectionInfoAdd.AssTempoRenouvEtRembPrimeDeces);

            var infoProtection = _getPDFICoverageResponse.DeterminerInfoProtection();

            infoProtection.HasFlag(InfoProtection.Renouvellable).Should().BeTrue();
        }

        [TestMethod]
        public void DeterminerInfoProtection_WhenAssuranceTemporaireRenouvelable_ThenHasFlagRenouvellable()
        {
            _getPDFICoverageResponse.Coverage.Plan.TypeProtectionInfoAdd.Returns(TypeProtectionInfoAdd.AssuranceTemporaireRenouvelable);

            var infoProtection = _getPDFICoverageResponse.DeterminerInfoProtection();

            infoProtection.HasFlag(InfoProtection.Renouvellable).Should().BeTrue();
        }

        [TestMethod]
        public void DeterminerInfoProtection_WhenRevenuAppointAccident_ThenHasFlagRevenuAppointAccident()
        {
            _getPDFICoverageResponse.Coverage.Plan.TypeProtectionInfoAdd.Returns(TypeProtectionInfoAdd.RevenuAppointAccident);

            var infoProtection = _getPDFICoverageResponse.DeterminerInfoProtection();

            infoProtection.HasFlag(InfoProtection.RevenuAppointAccident).Should().BeTrue();
        }

        [TestMethod]
        public void DeterminerInfoProtection_WhenRevenuAppointMaladie_ThenHasFlagRevenuAppointMaladie()
        {
            _getPDFICoverageResponse.Coverage.Plan.TypeProtectionInfoAdd.Returns(TypeProtectionInfoAdd.RevenuAppointMaladie);

            var infoProtection = _getPDFICoverageResponse.DeterminerInfoProtection();

            infoProtection.HasFlag(InfoProtection.RevenuAppointMaladie).Should().BeTrue();
        }

        [TestMethod]
        public void DeterminerInfoProtection_WhenAssuranceTemporaire_ThenHasFlagTemporaire()
        {
            _getPDFICoverageResponse.Coverage.Plan.TypeProtectionInfoAdd.Returns(TypeProtectionInfoAdd.AssuranceTemporaire);

            var infoProtection = _getPDFICoverageResponse.DeterminerInfoProtection();

            infoProtection.HasFlag(InfoProtection.Temporaire).Should().BeTrue();
        }

        [TestMethod]
        public void DeterminerInfoProtection_WhenAssuranceTemporaire_ThenNotHasFlagTemporaire()
        {
            _getPDFICoverageResponse.Coverage.Plan.TypeProtectionInfoAdd.Returns(TypeProtectionInfoAdd.AssuranceTemporaire | TypeProtectionInfoAdd.T100);

            var infoProtection = _getPDFICoverageResponse.DeterminerInfoProtection();

            infoProtection.HasFlag(InfoProtection.Temporaire).Should().BeFalse();
        }

        [TestMethod]
        public void DeterminerInfoProtection_WhenAvecBoniAndIris_ThenHasFlagAucun()
        {
            _getPDFICoverageResponse.Coverage.Plan.TypeProtectionInfoAdd.Returns(TypeProtectionInfoAdd.AvecBoni| TypeProtectionInfoAdd.IRIS);

            var infoProtection = _getPDFICoverageResponse.DeterminerInfoProtection();

            infoProtection.HasFlag(InfoProtection.Aucun).Should().BeTrue();
        }

        [TestMethod]
        public void DeterminerInfoProtection_WhenRemboursementPrimeDecesMG_ThenHasFlagMaladieGraveAvecRemboursementPrime()
        {
            _getPDFICoverageResponse.Coverage.Plan.TypeRetourPrime.Returns(TypeRemboursementPrime.RemboursementPrimeDecesMG);

            var infoProtection = _getPDFICoverageResponse.DeterminerInfoProtection();

            infoProtection.HasFlag(InfoProtection.MaladieGraveAvecRemboursementPrime).Should().BeTrue();
        }

        [TestMethod]
        public void DeterminerInfoProtection_WhenRemboursementFlexiblePrimeMG_ThenHasFlagMaladieGraveAvecRemboursementPrime()
        {
            _getPDFICoverageResponse.Coverage.Plan.TypeRetourPrime.Returns(TypeRemboursementPrime.RemboursementFlexiblePrimeMG);

            var infoProtection = _getPDFICoverageResponse.DeterminerInfoProtection();

            infoProtection.HasFlag(InfoProtection.MaladieGraveAvecRemboursementPrime).Should().BeTrue();
        }

        [TestMethod]
        public void DeterminerInfoProtection_WhenPrestationDecesMajoreT100Ultra_ThenHasFlagAucun()
        {
            _getPDFICoverageResponse.Coverage.Plan.TypeRetourPrime.Returns(TypeRemboursementPrime.PrestationDecesMajoreT100Ultra);

            var infoProtection = _getPDFICoverageResponse.DeterminerInfoProtection();

            infoProtection.HasFlag(InfoProtection.MaladieGraveAvecRemboursementPrime).Should().BeFalse();
            infoProtection.HasFlag(InfoProtection.Aucun).Should().BeTrue();
        }
    }
}
