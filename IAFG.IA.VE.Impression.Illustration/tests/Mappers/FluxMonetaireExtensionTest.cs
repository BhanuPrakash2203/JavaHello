using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections.FluxMonetaire;
using IAFG.IA.VI.Projection.Data;
using IAFG.IA.VI.Projection.Data.Enums;
using IAFG.IA.VI.Projection.Data.Enums.CashFlow;
using IAFG.IA.VI.Projection.Data.Transactions;
using IAFG.IA.VI.Projection.Data.Transactions.CashFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Illustration.Test.Mappers
{
    [TestClass]
    public class FluxMonetaireExtensionTest
    {
        [TestMethod]
        public void GIVEN_MapperFluxMonetaire_WITH_EmptyProjection_THEN_ReturnsTransactionList()
        {
            var fluxMonetaire = new FluxMonetaire();
            fluxMonetaire.MapperFluxMonetaire(new Projection(), DateTime.Now);
            fluxMonetaire.Transactions.Should().NotBeNull();
        }

        [TestMethod]
        public void GIVEN_MapperFluxMonetaire_WITH_Depots_THEN_ReturnsTransactionList()
        {
            var transaction = new Transactions();
            var projection = new Projection
            {
                Transactions = transaction
            };

            transaction.Deposits = new List<Deposit>
                                   {
                                       new Deposit {DepositType = DepositType.IrisCollateralLoanRefund},
                                       new Deposit
                                       {
                                           DepositType = DepositType.Deposit,
                                           Amount = new Amount {AmountType = AmountType.Customized, Value = 100.01},
                                           StartDate = new GenericDate {DateType = DateType.Calender, CalenderDate = DateTime.Now}
                                       },
                                       new Deposit
                                       {
                                           DepositType = DepositType.Deposit,
                                           Amount = new Amount {AmountType = AmountType.Customized, Value = 26.03},
                                           StartDate = new GenericDate {DateType = DateType.Calender,CalenderDate = DateTime.Now.AddYears(3)}
                                       },
                                       new Deposit
                                       {
                                           DepositType = DepositType.StandardPolicyLoanRefund,
                                           Amount = new Amount {AmountType = AmountType.Maximum},
                                           StartDate = new GenericDate {DateType = DateType.Calender,CalenderDate = DateTime.Now.AddYears(1)}
                                       }
                                   };


            var fluxMonetaire = new FluxMonetaire();
            fluxMonetaire.MapperFluxMonetaire(projection, DateTime.Now);

            using (new AssertionScope())
            {
                fluxMonetaire.Transactions.Should().HaveCount(3);
                fluxMonetaire.Transactions.Where(x => x.TypeTransaction ==  TypeTransactionFluxMonetaire.Depot).Should().HaveCount(3);
                var result = fluxMonetaire.Transactions.OrderBy(x => x.Montant).ToList();
                var r1 = result.First();
                r1.Montant.Should().Be(0);
                r1.TypeMontant.Should().Be(TypeMontantFluxMonetaires.Maximum);
                r1.Type.Should().EndWith(DepositType.StandardPolicyLoanRefund.ToString());
                r1.Annee.Should().Be(2);
                var r2 = result.Skip(1).First();
                r2.Montant.Should().Be(26.03);
                r2.TypeMontant.Should().Be(TypeMontantFluxMonetaires.Personnalise);
                r2.Type.Should().EndWith(DepositType.Deposit.ToString());
                r2.Annee.Should().Be(4);
                var r3 = result.Last();
                r3.Montant.Should().Be(100.01);
                r3.TypeMontant.Should().Be(TypeMontantFluxMonetaires.Personnalise);
                r3.Type.Should().EndWith(DepositType.Deposit.ToString());
                r3.Annee.Should().Be(1);
            }
        }

        [TestMethod]
        public void GIVEN_MapperFluxMonetaire_WITH_Retraits_THEN_ReturnsTransactionList()
        {
            var transaction = new Transactions();
            var projection = new Projection
            {
                Transactions = transaction
            };

            transaction.Withdrawals = new List<Withdrawal>
                                      {
                                          new Withdrawal {WithdrawalType = WithdrawalType.IrisLoan},
                                          new Withdrawal
                                          {
                                              WithdrawalType = WithdrawalType.PartialWithdrawal,
                                              Amount = new Amount {AmountType = AmountType.Maximum},
                                              StartDate = new GenericDate {DateType = DateType.Calender, CalenderDate = DateTime.Now.AddYears(1)}
                                          },
                                          new Withdrawal {WithdrawalType = WithdrawalType.IrisPolicyLoan}
                                      };


            var fluxMonetaire = new FluxMonetaire();
            fluxMonetaire.MapperFluxMonetaire(projection, DateTime.Now);

            using (new AssertionScope())
            {
                fluxMonetaire.Transactions.Should().HaveCount(1);
                fluxMonetaire.Transactions.Where(x => x.TypeTransaction == TypeTransactionFluxMonetaire.Retrait).Should().HaveCount(1);
                var result = fluxMonetaire.Transactions.OrderBy(x => x.Montant).ToList();
                var r1 = result.First();
                r1.Montant.Should().Be(0);
                r1.TypeMontant.Should().Be(TypeMontantFluxMonetaires.Maximum);
                r1.Type.Should().EndWith(WithdrawalType.PartialWithdrawal.ToString());
                r1.Annee.Should().Be(2);
            }
        }

        [TestMethod]
        public void GIVEN_MapperFluxMonetaire_WITH_DepotsRetraits_THEN_ReturnsTransactionList()
        {
            var transaction = new Transactions();
            var projection = new Projection
            {
                Transactions = transaction
            };

            transaction.Deposits = new List<Deposit>
                                   {
                                       new Deposit {DepositType = DepositType.IrisCollateralLoanRefund},
                                       new Deposit
                                       {
                                           DepositType = DepositType.Deposit,
                                           Amount = new Amount {AmountType = AmountType.Customized, Value = 100.01},
                                           StartDate = new GenericDate {DateType = DateType.Calender, CalenderDate = DateTime.Now}
                                       },
                                       new Deposit
                                       {
                                           DepositType = DepositType.Deposit,
                                           Amount = new Amount {AmountType = AmountType.Customized, Value = 26.03},
                                           StartDate = new GenericDate {DateType = DateType.Calender,CalenderDate = DateTime.Now.AddYears(3)}
                                       },
                                       new Deposit
                                       {
                                           DepositType = DepositType.StandardPolicyLoanRefund,
                                           Amount = new Amount {AmountType = AmountType.Maximum},
                                           StartDate = new GenericDate {DateType = DateType.Calender,CalenderDate = DateTime.Now.AddYears(1)}
                                       }
                                   };

            transaction.Withdrawals = new List<Withdrawal>
                                      {
                                          new Withdrawal {WithdrawalType = WithdrawalType.IrisLoan},
                                          new Withdrawal
                                          {
                                              WithdrawalType = WithdrawalType.PartialWithdrawal,
                                              Amount = new Amount {AmountType = AmountType.Maximum},
                                              StartDate = new GenericDate {DateType = DateType.Calender, CalenderDate = DateTime.Now.AddYears(1)}
                                          },
                                          new Withdrawal {WithdrawalType = WithdrawalType.IrisPolicyLoan}
                                      };


            var fluxMonetaire = new FluxMonetaire();
            fluxMonetaire.MapperFluxMonetaire(projection, DateTime.Now);

            using (new AssertionScope())
            {
                fluxMonetaire.Transactions.Should().HaveCount(4);
                fluxMonetaire.Transactions.Where(x => x.TypeTransaction == TypeTransactionFluxMonetaire.Depot).Should().HaveCount(3);
                fluxMonetaire.Transactions.Where(x => x.TypeTransaction == TypeTransactionFluxMonetaire.Retrait).Should().HaveCount(1);
                var result = fluxMonetaire.Transactions.OrderBy(x => x.TypeTransaction).ThenBy(x => x.Montant).ToList();
                var d1 = result.First();
                d1.Montant.Should().Be(0);
                d1.TypeMontant.Should().Be(TypeMontantFluxMonetaires.Maximum);
                d1.Type.Should().EndWith(DepositType.StandardPolicyLoanRefund.ToString());
                d1.Annee.Should().Be(2);
                var d2 = result.Skip(1).First();
                d2.Montant.Should().Be(26.03);
                d2.TypeMontant.Should().Be(TypeMontantFluxMonetaires.Personnalise);
                d2.Type.Should().EndWith(DepositType.Deposit.ToString());
                d2.Annee.Should().Be(4);
                var d3 = result.Skip(2).First();
                d3.Montant.Should().Be(100.01);
                d3.TypeMontant.Should().Be(TypeMontantFluxMonetaires.Personnalise);
                d3.Type.Should().EndWith(DepositType.Deposit.ToString());
                d3.Annee.Should().Be(1);
                var r1 = result.Last();
                r1.Montant.Should().Be(0);
                r1.TypeMontant.Should().Be(TypeMontantFluxMonetaires.Maximum);
                r1.Type.Should().EndWith(WithdrawalType.PartialWithdrawal.ToString());
                r1.Annee.Should().Be(2);
            }
        }
    }
}
