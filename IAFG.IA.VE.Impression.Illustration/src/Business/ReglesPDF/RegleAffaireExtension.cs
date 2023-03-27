using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VI.AF.IPDFVie.Factory.Interfaces;
using IAFG.IA.VI.AF.IPDFVie.PDF.Plan.ENUMs;

namespace IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF
{
    internal static class RegleAffaireExtension
    {
        internal static InfoProtection DeterminerInfoProtection(this IGetPDFICoverageResponse pdfCoverage)
        {
            if (pdfCoverage?.Coverage?.Plan == null) return InfoProtection.Aucun;
            var result = InfoProtection.Aucun;

            if (pdfCoverage.Coverage.Plan.TypeProtectionInfoAdd.HasFlag(TypeProtectionInfoAdd.AssTempoRenouvEtRembPrimeDeces) ||
                pdfCoverage.Coverage.Plan.TypeProtectionInfoAdd.HasFlag(TypeProtectionInfoAdd.AssuranceTemporaireRenouvelable))
            {
                result |= InfoProtection.Renouvellable;
            }

            if (pdfCoverage.Coverage.Plan.TypeProtectionInfoAdd.HasFlag(TypeProtectionInfoAdd.RevenuAppointAccident))
            {
                result |= InfoProtection.RevenuAppointAccident;
            }

            if (pdfCoverage.Coverage.Plan.TypeProtectionInfoAdd.HasFlag(TypeProtectionInfoAdd.RevenuAppointMaladie))
            {
                result |= InfoProtection.RevenuAppointMaladie;
            }

            if (pdfCoverage.Coverage.Plan.TypeRetourPrime.HasFlag(TypeRemboursementPrime.RemboursementPrimeDecesMG) ||
                pdfCoverage.Coverage.Plan.TypeRetourPrime.HasFlag(TypeRemboursementPrime.RemboursementFlexiblePrimeMG))
            {
                result |= InfoProtection.MaladieGraveAvecRemboursementPrime;
            }

            if (pdfCoverage.Coverage.Plan.TypeProtectionInfoAdd.HasFlag(TypeProtectionInfoAdd.AssuranceTemporaire) && 
                !pdfCoverage.Coverage.Plan.TypeProtectionInfoAdd.HasFlag(TypeProtectionInfoAdd.T100))
            {
                result |= InfoProtection.Temporaire;
            }

            return result;
        }
    }
}