using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.ModificationsDemandees;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Mappers.ModificationsDemandees
{
    [TestClass]
    public class TransactionModelExtensionTests
    {
        private IIllustrationReportDataFormatter _formatter;
        private IIllustrationResourcesAccessorFactory _resourcesAccessor;

        [TestInitialize]
        public void Initialize()
        {
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _formatter.FormatCurrencyWithoutDecimal(50000.00).Returns("50 000$");
            _formatter.FormatCurrency(50000.00).Returns("50 000.00$");
            _formatter.FormatCurrency(2500.00).Returns("2500.00$");
            _formatter.FormatterEnum<TypeOptionVersementBoni>(TypeOptionVersementBoni.AvecBoniEtFonds.ToString()).Returns("AvecBoniEtFonds");
            _formatter.FormatterEnum<OptionPrestationDeces>(OptionPrestationDeces.CapitalPlusFonds.ToString()).Returns("Capital Plus Fonds");
            _resourcesAccessor = Substitute.For<IIllustrationResourcesAccessorFactory>();
            _resourcesAccessor.GetResourcesAccessor().GetStringResourceById("CapitalAssureMaximalASL").Returns("Capital Assure Maximal ASL :");
            _resourcesAccessor.GetResourcesAccessor().GetStringResourceById("AucunAchatASL").Returns("Aucun Achat ASL");
            _resourcesAccessor.GetResourcesAccessor().GetStringResourceById("AucunMaximum").Returns("Aucun Maximum");
        }

        [TestMethod]
        public void MapperTransactions_WithNull()
        {
            ((List<TransactionModel>) null).MapperTransactions(_resourcesAccessor, _formatter).Should().NotBeNull();
        }

        [TestMethod]
        public void MapperTransactions_WithDesactivationOptimisationAutomatiqueCapitalAssure_THEN_TransactionMapped()
        {
            var transactions = new List<TransactionModel>
            {
                new TransactionDesactivationOptimisationAutomatiqueCapitalAssureModel
                {
                    Annee = 5,
                    Descpription = "une description"
                }
            };

            var result = transactions.MapperTransactions(_resourcesAccessor, _formatter);
            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.First().DescriptionModification.Should().Be("une description");
                result.First().Sequence.Should().Be(1);
            }
        }

        [TestMethod]
        public void MapperTransactions_WitChangementOptionAssuranceSupplementaire_THEN_TransactionMapped()
        {
            var transactions = new List<TransactionModel>
            {
                new TransactionChangementOptionAssuranceSupplementaireLibereeModel
                {
                    Annee = 5,
                    Descpription = "une description",
                    DescpriptionOptionAchat = "DescpriptionOptionAchat : ",
                    DescpriptionMontantAllocation = "DescpriptionMontantAllocation : ",
                    OptionVersementBoni = TypeOptionVersementBoni.AvecBoniEtFonds,
                    CapitalAssurePlafond = 50000,
                    MontantAllocation = 2500
                }
            };

            var result = transactions.MapperTransactions(_resourcesAccessor, _formatter);
            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.First().DescriptionModification.Should().Be("une description");
                result.First().Details.Should().HaveCount(3);
                result.First().Sequence.Should().Be(1);
            }
        }

        [TestMethod]
        public void MapperTransactions_WitChangementChangementPrestationDeces_THEN_TransactionMapped()
        {
            var transactions = new List<TransactionModel>
            {
                new TransactionChangementPrestationDecesModel
                {
                    Annee = 5,
                    Descpription = "une description",
                    DescpriptionOption = "DescpriptionOption : {0}",
                    OptionPrestationDeces = OptionPrestationDeces.CapitalPlusFonds
                }
            };

            var result = transactions.MapperTransactions(_resourcesAccessor, _formatter);
            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.First().DescriptionModification.Should().Be("une description");
                result.First().Details.Should().HaveCount(1);
                result.First().Details.First().Should().Be("DescpriptionOption : Capital Plus Fonds");
                result.First().Sequence.Should().Be(1);
            }
        }
    }
}
