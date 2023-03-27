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
    internal static class TransactionModelExtension
    {
        internal static List<ModificationViewModel> MapperTransactions(this List<TransactionModel> transactions,
          IIllustrationResourcesAccessorFactory resourcesAccessor, IIllustrationReportDataFormatter formatter)
        {
            var result = new List<ModificationViewModel>();
            if (transactions == null) return result;

            var sequence = 0;
            foreach (var transaction in transactions.OrderBy(x => x.Annee))
            {
                sequence += 1;
                if (transaction is TransactionChangementOptionAssuranceSupplementaireLibereeModel model)
                {
                    result.Add(MapperChangementOptionAssuranceSupplementaireLiberee(sequence, 
                        model, resourcesAccessor, formatter));               
                }
                else if (transaction is TransactionChangementPrestationDecesModel decesModel)
                {
                    result.Add(MapperChangementPrestationDeces(sequence,
                        decesModel, formatter));
                }
                else if (transaction is TransactionChangementOptionParticipantModel optionParticipantModel)
                {
                    result.Add(MapperChangementOptionParticipant(sequence,
                        optionParticipantModel, formatter));
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

        private static ModificationViewModel MapperChangementPrestationDeces(int sequence,
            TransactionChangementPrestationDecesModel transaction, IIllustrationReportDataFormatter formatter)
        {
            var details = new List<string>
            {
                string.Format(transaction.DescpriptionOption,
                    formatter.FormatterEnum<OptionPrestationDeces>(transaction.OptionPrestationDeces.ToString()))
            };

            return new ModificationViewModel
            {
                Sequence = sequence,
                DescriptionModification = string.Format(transaction.Descpription, transaction.Annee),
                Details = details
            };
        }

        private static ModificationViewModel MapperChangementOptionAssuranceSupplementaireLiberee(int sequence,
            TransactionChangementOptionAssuranceSupplementaireLibereeModel transaction, 
            IResourcesAccessorFactory resourcesAccessor, IIllustrationReportDataFormatter formatter)
        {
            var capitalAssureMaximal = formatter.FormatCurrency(transaction.CapitalAssurePlafond.GetValueOrDefault());

            if (transaction.AucunAchat)
            {
                capitalAssureMaximal = resourcesAccessor.GetResourcesAccessor().GetStringResourceById("AucunAchatASL");
            }

            if (transaction.AucunMaximum)
            {
                capitalAssureMaximal = resourcesAccessor.GetResourcesAccessor().GetStringResourceById("AucunMaximum");
            }

            var details = new List<string>
            {
                string.Format(transaction.DescpriptionOptionAchat,
                    formatter.FormatterEnum<TypeOptionVersementBoni>(transaction.OptionVersementBoni.ToString())),
                resourcesAccessor.GetResourcesAccessor().GetStringResourceById("CapitalAssureMaximalASL") + @" " +
                capitalAssureMaximal
            };

            if (transaction.MontantAllocation.HasValue)
            {
                details.Add(string.Format(transaction.DescpriptionMontantAllocation,
                    formatter.FormatCurrency(transaction.MontantAllocation.GetValueOrDefault())));
            }

            return new ModificationViewModel
            {
                Sequence = sequence,
                DescriptionModification = string.Format(transaction.Descpription, transaction.Annee),
                Details = details
            };
        }

        private static ModificationViewModel MapperChangementOptionParticipant(int sequence, 
            TransactionChangementOptionParticipantModel transaction, 
            IIllustrationReportDataFormatter formatter)
        {
            var details = new List<string>
            {
                string.Format(transaction.DescpriptionOption,
                    formatter.FormatterEnum<TypeOptionParticipation>(transaction.Option.ToString()))
            };

            return new ModificationViewModel
            {
                Sequence = sequence,
                DescriptionModification = string.Format(transaction.Descpription, transaction.Annee),
                Details = details
            };
        }
    }
}