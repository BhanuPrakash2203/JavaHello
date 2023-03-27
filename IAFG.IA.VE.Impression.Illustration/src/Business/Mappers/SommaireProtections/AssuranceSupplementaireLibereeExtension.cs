using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections.ASL;
using IAFG.IA.VI.Projection.Data;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
{
    internal static class AssuranceSupplementaireLibereeExtension
    {
        public static AssuranceSupplementaireLiberee MapperAsl(this AssuranceSupplementaireLiberee asl, Projection projection, DateTime dateEmission)
        {
            var paidUpAdditionalOption = projection.Contract.PaidUpAdditionalOption;
            if (paidUpAdditionalOption == null) return null;

            asl.OptionVersementBoni = (TypeOptionVersementBoni) paidUpAdditionalOption.PurchaseOption;
            asl.CapitalAssureMaximal = paidUpAdditionalOption.MaximalFaceAmount;
            asl.MontantAllocationInitial = paidUpAdditionalOption.AllocationAmount;
            asl.TauxAnnees = new List<TauxAnnee>();
            asl.Allocations = new List<Allocation>();

            var equiBuild = projection?.Parameters?.Assumptions?.ReferenceIndex?.FirstOrDefault(x => x.Vehicle?.ToUpper() == "IBO970");
            if (equiBuild?.InterestRates != null)
            {
                foreach (var item in equiBuild.InterestRates)
                {
                    asl.TauxAnnees.Add(new TauxAnnee
                    {
                        Annee = item.StartDate.ConvertirDateProjection(dateEmission)?.Year ?? 0,
                        Taux = item.Value
                    });
                }
            }

            if (projection?.Transactions?.PaidUpAdditionalOptionChanges == null)
            {
                return asl;
            }

            foreach (var changementOptionAsl in projection.Transactions.PaidUpAdditionalOptionChanges)
            {
                asl.Allocations.Add(new Allocation
                                    {
                                        Annee = changementOptionAsl.StartDate.CalculerAnneeContratProjection(dateEmission),
                                        Montant = paidUpAdditionalOption.AllocationAmount
                                    });
            }

            return asl;
        }
    }
}