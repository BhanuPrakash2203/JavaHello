using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections.FluxMonetaire;
using IAFG.IA.VI.Projection.Data;
using IAFG.IA.VI.Projection.Data.Enums.CashFlow;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
{
    internal static class FluxMonetaireExtension
    {
        public static FluxMonetaire MapperFluxMonetaire(this FluxMonetaire fluxMonetaire, Projection projection, DateTime dateEmission)
        {
            fluxMonetaire.Transactions = new List<TransactionFluxMonetaire>();
            MapperDepots(fluxMonetaire, projection, dateEmission);
            MapperRetraits(fluxMonetaire, projection, dateEmission);
            return fluxMonetaire;
        }

        private static void MapperDepots(FluxMonetaire fluxMonetaire, Projection projection, DateTime dateEmission)
        {
            var depots = projection?.Transactions?.Deposits?.Where(x =>
                                                                       x.DepositType != DepositType.IrisCollateralLoanRefund &&
                                                                       x.DepositType != DepositType.IrisPolicyLoanRefund);

            if (depots == null) return;
            foreach (var item in depots)
            {
                var estDepotRetraitMaximal = false;
                var estDepotRetraitApresDechance = false;
                var montant = item.Amount?.Value ?? 0;
                if (item.DepositType == DepositType.Deposit && item.Amount != null && item.Amount.AmountType == AmountType.Maximum)
                {
                    estDepotRetraitMaximal = true;

                    if (projection.Transactions.TransactionValues != null && projection.Transactions.TransactionValues.Any(x => x.Key == item.TransactionIdentifier.Id))
                    {
                        var montantTransaction = projection.Transactions.TransactionValues.Single(x => x.Key == item.TransactionIdentifier.Id);
                        montant = montantTransaction.Value;
                    }
                    else
                    {
                        estDepotRetraitApresDechance = true;
                    }
                }

                fluxMonetaire.Transactions.Add(new TransactionFluxMonetaire
                {
                    TypeTransaction = TypeTransactionFluxMonetaire.Depot,
                    Type = $"DepositType.{item.DepositType.ToString()}",
                    EstDepotRetraitMaximal = estDepotRetraitMaximal,
                    EstDepotRetraitApresDecheance =  estDepotRetraitApresDechance,
                    Annee = item.StartDate.CalculerAnneeContratProjection(dateEmission),
                    Montant = montant,
                    TypeMontant = (TypeMontantFluxMonetaires) (item.Amount?.AmountType ?? AmountType.Unspecified)
                });
            }
        }

        private static void MapperRetraits(FluxMonetaire fluxMonetaire, Projection projection, DateTime dateEmission)
        {
            var retraits = projection?.Transactions?.Withdrawals?.Where(x =>
                                                                            x.WithdrawalType != WithdrawalType.IrisLoan &&
                                                                            x.WithdrawalType != WithdrawalType.IrisPolicyLoan);

            if (retraits == null) return;
            foreach (var item in retraits)
            {
                var estDepotRetraitMaximal = false;
                var estDepotRetraitApresDechance = false;
                var montant = item.Amount?.Value ?? 0;
                if (item.WithdrawalType == WithdrawalType.PartialWithdrawal && item.Amount != null && item.Amount.AmountType == AmountType.Maximum)
                {
                    estDepotRetraitMaximal = true;

                    if (projection.Transactions.TransactionValues != null && projection.Transactions.TransactionValues.Any(x => x.Key == item.TransactionIdentifier.Id))
                    {
                        var montantTransaction = projection.Transactions.TransactionValues.Single(x => x.Key == item.TransactionIdentifier.Id);
                        montant = montantTransaction.Value;
                    }
                    else
                    {
                        estDepotRetraitApresDechance = true;
                    }
                }

                fluxMonetaire.Transactions.Add(new TransactionFluxMonetaire
                {
                    TypeTransaction = TypeTransactionFluxMonetaire.Retrait,
                    Type = $"WithdrawalType.{item.WithdrawalType.ToString()}",
                    EstDepotRetraitMaximal = estDepotRetraitMaximal,
                    EstDepotRetraitApresDecheance = estDepotRetraitApresDechance,
                    Annee = item.StartDate.CalculerAnneeContratProjection(dateEmission),
                    Montant = montant,
                    TypeMontant = (TypeMontantFluxMonetaires) (item.Amount?.AmountType ?? AmountType.Unspecified)
                });
            }
        }
    }
}