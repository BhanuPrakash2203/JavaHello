using System;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.Illustration;
using IAFG.IA.VI.Projection.Data;
using IAFG.IA.VI.Projection.Data.Contract;
using IAFG.IA.VI.Projection.Data.Contract.Traditional.Financial;
using IAFG.IA.VI.Projection.Data.Contract.Traditional.Financial.Loans;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Mappers.Illustration
{
    [TestClass]
    public class ProjectionMapperTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestMethod]
        public void MapAvancesSurPolice_WhenTraditionalFinancialIsNull_ThenReturnNull()
        {
            var projection = new Projection {Contract = new Contract()};
            var mapper = new ProjectionsMapper();
            var result = mapper.MapAvancesSurPolice(projection);

            using (new AssertionScope())
            {
                result.Should().BeNull();
            }
        }

        [TestMethod]
        public void MapAvancesSurPolice_WhenLoansIsNull_ThenReturnNull()
        {
            var projection = new Projection
            {
                Contract = new Contract
                {
                    TraditionalFinancial = new FinancialSection
                    {
                        Loans = null
                    }
                }
            }; 
            var mapper = new ProjectionsMapper();
            var result = mapper.MapAvancesSurPolice(projection);

            using (new AssertionScope())
            {
                result.Should().BeNull();
            }
        }

        [TestMethod]
        public void MapAvancesSurPolice_WhenBalanceIsZero_ThenReturnNull()
        {
            const double balance = 0.0D;

            var projection = new Projection
            {
                Contract = new Contract
                {
                    TraditionalFinancial = new FinancialSection
                    {
                        Loans = new Loans
                        {
                            Balance = balance
                        }
                    }
                }
            };
            var mapper = new ProjectionsMapper();
            var result = mapper.MapAvancesSurPolice(projection);

            using (new AssertionScope())
            {
                result.Should().BeNull();
            }
        }

        [TestMethod]
        public void MapAvancesSurPolice_WhenLoansIsNotNull_ThenReturnExpectedValues()
        {
            const double balance = 123.45;
            var dateLasUpdate = new DateTime(2022, 01, 02);

            var projection = new Projection
            {
                Contract = new Contract
                {
                    TraditionalFinancial = new FinancialSection{Loans = new Loans
                    {
                        LastUpdate = dateLasUpdate,
                        Balance = balance
                    }}
                }
            };
            var mapper = new ProjectionsMapper();
            var result = mapper.MapAvancesSurPolice(projection);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.DateDerniereMiseAJour.Should().Be(dateLasUpdate);
                result.Solde.Should().Be(balance);
            }
        }
    }
}
