using System;
using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using ENUMs_ProjectionData = IAFG.IA.VI.Projection.Data.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Tests
{
    [TestClass]
    public class IllustrationsEnumTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();

        private IResourcesAccessor _resourcesAccessor;
        
        [TestInitialize]
        public void Setup()
        {
            _resourcesAccessor = Substitute.For<IResourcesAccessor>();
        }

        [DataRow(ENUMs_ProjectionData.Product.Genesis, Produit.Genesis)]
        [DataRow(ENUMs_ProjectionData.Product.Trend, Produit.Tendance)]
        [DataRow(ENUMs_ProjectionData.Product.Equibuild, Produit.CapitalValeur)]
        [DataRow(ENUMs_ProjectionData.Product.Equibuild3, Produit.CapitalValeur3)]
        [DataRow(ENUMs_ProjectionData.Product.Meridia, Produit.Meridia)]
        [DataRow(ENUMs_ProjectionData.Product.TraditionalInsurance, Produit.Traditionnel)]
        [DataRow(ENUMs_ProjectionData.Product.Topaz, Produit.Topaz)]
        [DataRow(ENUMs_ProjectionData.Product.ParticipatingLifeInsurance, Produit.AssuranceParticipant)]
        [DataRow(ENUMs_ProjectionData.Product.Transition, Produit.Transition)]
        [DataRow(ENUMs_ProjectionData.Product.MDLifePlan, Produit.RegimeVieMD)]
        [DataRow(ENUMs_ProjectionData.Product.AccessLife, Produit.AccesVie)]
        [DataRow(ENUMs_ProjectionData.Product.Transition, Produit.Transition)]
        [DataTestMethod]
        public void DeterminerProduit_ThenGoodResult(ENUMs_ProjectionData.Product product, Produit expectedResult)
        {
            ((Produit)product).Should().Be(expectedResult);
        }

        [TestMethod]
        public void ValiderCorrespondanceDesEnums()
        {
            //Les enums suivants doivent avoir les mêmes items définis. 
            EnumAssert.ContientToutesLesValeurs<Banniere, ENUMs_ProjectionData.Banner>();
            EnumAssert.ContientToutesLesValeurs<Produit, ENUMs_ProjectionData.Product>();
            EnumAssert.ContientToutesLesValeurs<StatutTabagisme, ENUMs_ProjectionData.SmokerType>();
            EnumAssert.ContientToutesLesValeurs<Sexe, ENUMs_ProjectionData.Sex> ();
            EnumAssert.ContientToutesLesValeurs<TypeFrequenceFacturation, ENUMs_ProjectionData.Billing.Frequency> ();
            EnumAssert.ContientToutesLesValeurs<TypeAssurance, ENUMs_ProjectionData.Coverage.InsuranceType> ();
            EnumAssert.ContientToutesLesValeurs<ChoixBoniInteret, ENUMs_ProjectionData.Financial.BonusType>();
            EnumAssert.ContientToutesLesValeurs<TypeCout, ENUMs_ProjectionData.Coverage.MortalityType> ();
            EnumAssert.ContientToutesLesValeurs<TypePret, ENUMs_ProjectionData.Financial.LoanType> ();
            EnumAssert.ContientToutesLesValeurs<TypeOptionVersementBoni, ENUMs_ProjectionData.PaidUpAdditionalPurchaseOption> ();
            EnumAssert.ContientToutesLesValeurs<TypeMontantFluxMonetaires, ENUMs_ProjectionData.CashFlow.AmountType> ();
            EnumAssert.ContientToutesLesValeurs<OptionPrestationDeces, ENUMs_ProjectionData.Coverage.DeathBenefitOption>();
        }

        [TestMethod]
        public void GetDescription_WhenTypeCoutNivele_ThenReturnRessourceNiveles()
        {
            var expectedDescription = Auto.Create<string>();
            _resourcesAccessor.GetStringResourceById("_coûts_niveles").Returns(expectedDescription);

            TypeCout.Nivele.GetDescription(_resourcesAccessor).Should().Be(expectedDescription);
            TypeCout.NiveleEpargne.GetDescription(_resourcesAccessor).Should().Be(expectedDescription);
        }

        [TestMethod]
        public void GetDescription_WhenTypeCoutTra_ThenReturnRessourceTra()
        {
             var expectedDescription = Auto.Create<string>();
            _resourcesAccessor.GetStringResourceById("_coûts_tra").Returns(expectedDescription);

            TypeCout.TRA.GetDescription(_resourcesAccessor).Should().Be(expectedDescription);
        }

        [TestMethod]
        public void GetDescription_WhenTypeCoutNonApplicable_ThenReturnEmptyString()
        {
            var expectedDescription = string.Empty;

            TypeCout.NonApplicable.GetDescription(_resourcesAccessor).Should().Be(expectedDescription);
        }

        [TestMethod]
        public void ConvertirSex_WhenSexFemale_ShouldReturnFemme()
        {
            ENUMs_ProjectionData.Sex.Female.ConvertirSex()
                .Should()
                .Be(Sexe.Femme);
        }

        [TestMethod]
        public void ConvertirSex_WhenSexMale_ShouldReturnHomme()
        {
            ENUMs_ProjectionData.Sex.Male.ConvertirSex()
                .Should()
                .Be(Sexe.Homme);
        }

        [TestMethod]
        public void ConvertirSex_WhenSexUnspecified_ShouldReturnNonDefini()
        {
            ENUMs_ProjectionData.Sex.Unspecified.ConvertirSex()
                .Should()
                .Be(Sexe.NonDefini);
        }

        [TestMethod]
        public void ConvertirSex_WhenSexUnknown_ShouldReturnInconnu()
        {
            ENUMs_ProjectionData.Sex.Unknown.ConvertirSex()
                .Should()
                .Be(Sexe.Inconnu);
        }

        [TestMethod]
        public void ConvertirFrequence_WhenAnnual_ShouldReturnAnnuelle()
        {
            ENUMs_ProjectionData.Billing.Frequency.Annual.ConvertirFrequence()
                .Should()
                .Be(TypeFrequenceFacturation.Annuelle);
        }

        [TestMethod]
        public void ConvertirFrequence_WhenMonthly_ShouldReturnMensuelle()
        {
            ENUMs_ProjectionData.Billing.Frequency.Monthly.ConvertirFrequence()
                .Should()
                .Be(TypeFrequenceFacturation.Mensuelle);
        }

        [TestMethod]
        public void ConvertirFrequence_WhenNone_ShouldReturnAucunMode()
        {
            ENUMs_ProjectionData.Billing.Frequency.None.ConvertirFrequence()
                .Should()
                .Be(TypeFrequenceFacturation.AucunMode);
        }

        [TestMethod]
        public void ConvertirFrequence_WhenOther_ShouldReturnAutre()
        {
            ENUMs_ProjectionData.Billing.Frequency.Other.ConvertirFrequence()
                .Should()
                .Be(TypeFrequenceFacturation.Autre);
        }

        [TestMethod]
        public void ConvertirTypePret_WhenUnspecified_ShouldReturnNonDefini()
        {
            ENUMs_ProjectionData.Financial.LoanType.Unspecified.ConvertirTypePret()
                .Should()
                .Be(TypePret.NonDefini);
        }

        [TestMethod]
        public void ConvertirTypePret_WhenAutomaticPremiumLoan_ShouldReturnAvanceAutomatique()
        {
            ENUMs_ProjectionData.Financial.LoanType.AutomaticPremiumLoan.ConvertirTypePret()
                .Should()
                .Be(TypePret.AvanceAutomatique);
        }

        [TestMethod]
        public void ConvertirTypePret_WhenInvestmentLoan_ShouldReturnIris()
        {
            ENUMs_ProjectionData.Financial.LoanType.InvestmentLoan.ConvertirTypePret()
                .Should()
                .Be(TypePret.Iris);
        }

        [TestMethod]
        public void ConvertirTypePret_WhenInvestmentPolicyLoan_ShouldReturnEpargnePlus()
        {
            ENUMs_ProjectionData.Financial.LoanType.InvestmentPolicyLoan.ConvertirTypePret()
                .Should()
                .Be(TypePret.EpargnePlus);
        }

        [TestMethod]
        public void ConvertirTypePret_WhenRetirementStrategyBankLoan_ShouldReturnRetirementStrategyBankLoan()
        {
            ENUMs_ProjectionData.Financial.LoanType.RetirementStrategyBankLoan.ConvertirTypePret()
                .Should()
                .Be(TypePret.RetirementStrategyBankLoan);
        }

        [TestMethod]
        public void ConvertirTypePret_WhenPolicyLoan_ShouldReturnAvance()
        {
            ENUMs_ProjectionData.Financial.LoanType.PolicyLoan.ConvertirTypePret()
                .Should()
                .Be(TypePret.Avance);
        }
        
        private static class EnumAssert
        {
            public static void ContientToutesLesValeurs<TEnumRapport, TMoteur>()
            {
                var rapportValues = Enum.GetValues(typeof (TEnumRapport));
                var moteurValues = Enum.GetValues(typeof (TMoteur));

                Assert.AreEqual(rapportValues.Length,
                                moteurValues.Length,
                                $"Echec de la comparaison, le nombre d'élément est différent entre {typeof (TMoteur).FullName} et {typeof (TEnumRapport).FullName}.");

                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var valeurEnum in moteurValues)
                {
                    Assert.IsTrue(Enum.IsDefined(typeof (TEnumRapport), (int) valeurEnum),
                                  $"Echec de la comparaison pour {typeof (TMoteur).FullName}, l'item {valeurEnum} n'est pas présent dans {typeof (TEnumRapport).FullName}.");
                }

                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var valeurEnum in rapportValues)
                {
                    Assert.IsTrue(Enum.IsDefined(typeof (TMoteur), (int) valeurEnum),
                                  $"Echec de la comparaison pour {typeof (TEnumRapport).FullName}, l'item {valeurEnum} n'est pas présent dans {typeof (TMoteur).FullName}.");
                }
            }
        }
    }
}