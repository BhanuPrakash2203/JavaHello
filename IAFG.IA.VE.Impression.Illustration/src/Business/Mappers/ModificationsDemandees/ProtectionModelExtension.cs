using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.ModificationsDemandees;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.ModificationsDemandees
{
    internal static class ProtectionModelExtension
    {
        internal static List<ProtectionViewModel> MapperProtections(this List<ProtectionModel> protections,
            IIllustrationResourcesAccessorFactory resourcesAccessor, IIllustrationReportDataFormatter formatter)
        {
            if (protections == null) return new List<ProtectionViewModel>();
            return protections.Select(protection => new ProtectionViewModel
                {
                    Id = protection.Id,
                    NomAssures =
                        formatter.FormatNames(protection.Assures.Select(x =>
                            formatter.FormatFullName(x.Prenom, x.Nom, x.Initiale))),
                    DescriptionProtection = protection.Libelle,
                    Montant = protection.Capital.HasValue ? formatter.FormatCurrencyWithoutDecimal(protection.Capital.GetValueOrDefault()) : string.Empty,
                    Modifications = MapperModifications(protection, formatter, resourcesAccessor)
                })
                .ToList();
        }

        private static IList<ModificationViewModel> MapperModifications(ProtectionModel protection,
            IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor)
        {
            var result = new List<ModificationViewModel>();
            if (protection.Transactions == null) return result;
            var sequence = 0;
            foreach (var transaction in protection.Transactions.OrderBy(x => x.Annee))
            {
                sequence += 1;
                if (transaction is TransactionNivellementModel)
                {
                    result.Add(MapperNivellement(formatter, sequence, transaction as TransactionNivellementModel));
                }
                else if (transaction is TransactionChangementUsageTabacModel)
                {
                    result.Add(MapperChangementUsageTabac(formatter, sequence, transaction as TransactionChangementUsageTabacModel));
                }
                else if (transaction is TransactionReductionCapitalModel)
                {
                    result.Add(MapperReductionCapital(formatter, sequence, transaction as TransactionReductionCapitalModel));
                }
                else if (transaction is TransactionAjoutOptionModel)
                {
                    result.Add(MapperAjoutOption(formatter, sequence, transaction, transaction as TransactionAjoutOptionModel));
                }
                else if (transaction is TransactionTransformationConjointDernierDecesModel)
                {
                    result.Add(MapperTransformationConjointDernierDeces(formatter, resourcesAccessor, sequence, transaction as TransactionTransformationConjointDernierDecesModel));
                }
                else if (transaction is TransactionAjoutProtectionModel)
                {
                    result.Add(MapperAjoutProtection(formatter, sequence, transaction as TransactionAjoutProtectionModel));
                }
                else
                {
                    result.Add(new ModificationViewModel
                    {
                        Sequence = sequence,
                        DescriptionModification = string.Format(transaction.Descpription, transaction.Annee)
                    });
                }
            }

            return result;
        }

        private static ModificationViewModel MapperAjoutProtection(IIllustrationReportDataFormatter formatter, int sequence, TransactionAjoutProtectionModel ajoutProtection)
        {
            return new ModificationViewModel
            {
                Sequence = sequence,
                DescriptionModification = string.Format(ajoutProtection.Descpription, ajoutProtection.Annee),
                ModificationDetails = new List<string>
                {
                    ajoutProtection.DescpriptionProtection                   
                },
                Details = new List<string> { string.Format(ajoutProtection.DescpriptionMontantPrime,
                    formatter.FormatCurrency(ajoutProtection.MontantPrime))}
            };
        }

        private static ModificationViewModel MapperTransformationConjointDernierDeces(
            IIllustrationReportDataFormatter formatter, IResourcesAccessorFactory resourcesAccessor,
            int sequence, TransactionTransformationConjointDernierDecesModel transformation)
        {
            return new ModificationViewModel
            {
                Sequence = sequence,
                DescriptionModification = string.Format(transformation.Descpription, transformation.Annee),
                Details = MapperDetailSurprimes(formatter, resourcesAccessor, transformation.Surprimes)
            };
        }

        private static IList<string> MapperDetailSurprimes(IIllustrationReportDataFormatter formatter,
            IResourcesAccessorFactory resourcesAccessor, List<SurprimeModel> surprimes)
        {
            var result = new List<string>();
            if (surprimes == null) return result;
            var ressourceAccessor = resourcesAccessor.GetResourcesAccessor();
            foreach (var item in surprimes)
            {
                var descriptionPourcentage = item.TauxPourcentage.HasValue
                    ? $"+{formatter.FormatPercentage((int) item.TauxPourcentage)}"
                    : string.Empty;
                var descriptionMontant = item.TauxMontant.HasValue
                    ? $"{formatter.FormatCurrency(item.TauxMontant)}{ressourceAccessor.GetStringResourceById("SurprimeParMille")}"
                    : string.Empty;
                var descriptionTerme = item.Terme.HasValue
                    ? formatter.FormatterDuree(TypeDuree.PendantNombreAnnees, item.Terme.Value)
                    : string.Empty;

                result.Add(
                    $"{item.Descpription} {descriptionPourcentage}{descriptionMontant} {descriptionTerme}".Trim());
            }

            return result;
        }

        private static ModificationViewModel MapperReductionCapital(IIllustrationReportDataFormatter formatter,
            int sequence, TransactionReductionCapitalModel reductionCapital)
        {
            return new ModificationViewModel
            {
                Sequence = sequence,
                DescriptionModification = string.Format(reductionCapital.Descpription, reductionCapital.Annee),
                ModificationDetails = new List<string>
                {
                    string.Format(reductionCapital.DescpriptionMontant,
                        formatter.FormatCurrencyWithoutDecimal(reductionCapital.Montant))
                }
            };
        }

        private static ModificationViewModel MapperAjoutOption(IIllustrationReportDataFormatter formatter,
            int sequence, TransactionModel transaction, TransactionAjoutOptionModel ajoutOption)
        {
            return new ModificationViewModel
            {
                Sequence = sequence,
                DescriptionModification = string.Format(transaction.Descpription, transaction.Annee),
                ModificationDetails = new List<string>
                {
                    ajoutOption.DescpriptionOption,
                    string.Format(ajoutOption.DescpriptionMontantPrime,
                        formatter.FormatCurrency(ajoutOption.MontantPrime))
                }
            };
        }

        private static ModificationViewModel MapperChangementUsageTabac(IIllustrationReportDataFormatter formatter,
            int sequence, TransactionChangementUsageTabacModel changementUsageTabac)
        {
            return new ModificationViewModel
            {
                Sequence = sequence,
                DescriptionModification = string.Format(changementUsageTabac.Descpription, changementUsageTabac.Annee),
                ModificationDetails = new List<string>
                {
                    formatter.FormatFullName(changementUsageTabac.Prenom, changementUsageTabac.Nom,
                        changementUsageTabac.Initiale),
                    formatter.FormatUsageTabac(changementUsageTabac.StatutTabagisme)
                }
            };
        }

        private static ModificationViewModel MapperNivellement(IIllustrationReportDataFormatter formatter, int sequence,
            TransactionNivellementModel nivellement)
        {
            return new ModificationViewModel
            {
                Sequence = sequence,
                DescriptionModification = string.Format(nivellement.Descpription, nivellement.Annee),
                ModificationDetails = new List<string>
                {
                    nivellement.Age.HasValue
                        ? string.Format(nivellement.DescpriptionAge,
                            formatter.FormatAge(nivellement.Age.GetValueOrDefault()))
                        : string.Empty,
                    nivellement.AgeSurprime.HasValue
                        ? string.Format(nivellement.DescpriptionAgeSurprime,
                            formatter.FormatAge(nivellement.AgeSurprime.GetValueOrDefault()))
                        : string.Empty
                }.Where(x => !string.IsNullOrEmpty(x)).ToList()
            };
        }
    }
}