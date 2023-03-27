using System;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VI.AF.IPDFVie.Contrat.ENUMs;
using IAFG.IA.VI.AF.IPDFVie.Factory.Interfaces;
using IAFG.IA.VI.AF.IPDFVie.Illustration.ENUMs;
using IAFG.IA.VI.AF.IPDFVie.PDF;
using IAFG.IA.VI.AF.IPDFVie.PDF.Plan.ENUMs;
using IAFG.IA.VI.AF.IPDFVie.PDF.Product;
using IAFG.IA.VI.AF.IPDFVie.PDF.Product.ENUMs;
using IAFG.IA.VI.Projection.Data;
using IAFG.IA.VI.Projection.Data.Contract.Coverage;
using IAFG.IA.VI.Projection.PDFVie.Extensions;
using IPlanInfo = IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF.Types.IPlanInfo;
using PlanInfo = IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF.Types.PlanInfo;

namespace IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF
{
    public class RegleAffaireAccessor : IRegleAffaireAccessor
    {
        private readonly IFactory _pdfactory;

        public RegleAffaireAccessor(IFactory pdfactory)
        {
            _pdfactory = pdfactory;
        }

        public IPlanInfo ObtenirPlan(string codePlan)
        {
            var reglePlan = _pdfactory.GetIReglesPlan(codePlan);
            if (reglePlan == null)
            {
                throw new ArgumentOutOfRangeException(nameof(codePlan), codePlan,
                    $@"Ce code de plan {codePlan} n'est pas associé à un produit géré par le rapport.");
            }

            var planInfo = new PlanInfo
            {
                CodePlan = reglePlan.Plan,
                AgeMaturite = reglePlan.AgeMaturite,
                DescriptionAn = reglePlan.DescriptionAn,
                DescriptionFr = reglePlan.DescriptionFr,
                CodeGlossaire = reglePlan.CodeGlossaire                
            };

            DeterminerPrestationPlan(reglePlan, planInfo);
            return planInfo;
        }

        private static void DeterminerPrestationPlan(IReglesPlan plan, IPlanInfo planInfo)
        {
            switch (plan.DureeRevenuAppoint)
            {
                case DureeRevenuAppoint.A65Ans:
                    planInfo.TypePrestationPlan = TypePrestationPlan.PrestationMensuelle65Ans;
                    break;
                case DureeRevenuAppoint.Duree2Ans:
                    planInfo.TypePrestationPlan = TypePrestationPlan.PrestationMensuellePour2Ans;
                    break;
                case DureeRevenuAppoint.Duree5Ans:
                    planInfo.TypePrestationPlan = TypePrestationPlan.PrestationMensuellePour5Ans;
                    break;
                case DureeRevenuAppoint.NonApplicable:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (plan.TypeProtection.HasFlag(TypeProtection.Hospitalisation))
            {
                planInfo.TypePrestationPlan = TypePrestationPlan.PrestationJournaliere;
            }
        }
        
        public bool EstCompteInteretGarantie(string vehicule)
        {
            var regle = _pdfactory.GetReglesComptes(vehicule);
            if (regle == null) throw new ArgumentOutOfRangeException(nameof(vehicule), vehicule, $@"Le code de vehicule {vehicule} n'est pas valide dans PDFVie.");
            return regle.SelonTypeCompte.TypeCompte == TypeCompte.Garanti;
        }

        public bool EstComptePorteuille(string vehicule)
        {
            var regle = _pdfactory.GetReglesComptes(vehicule);
            if (regle == null) throw new ArgumentOutOfRangeException(nameof(vehicule), vehicule, $@"Le code de vehicule {vehicule} n'est pas valide dans PDFVie.");
            return regle.SelonTypeCompte.TypeCompte == TypeCompte.Courant;
        }

        public IGetPDFICoverageResponse GetPdfCoverage(Projection projectionData, Coverage coverage)
        {
            return _pdfactory.GetPdfICoverage(projectionData.GetProtection(coverage));
        }

        public IGetPDFICoverageResponse GetPdfCoverage(Projection projectionData, AdditionalBenefit additionalBenefit)
        {
            return _pdfactory.GetPdfICoverage(projectionData.GetProtection(additionalBenefit));
        }

        public InfoProtection DeterminerInfoProtection(Projection projectionData, Coverage coverage)
        {
            return DeterminerInfoProtection(GetPdfCoverage(projectionData, coverage));
        }

        public InfoProtection DeterminerInfoProtection(Projection projectionData, AdditionalBenefit additionalBenefit)
        {
            return DeterminerInfoProtection(GetPdfCoverage(projectionData, additionalBenefit));
        }

        private static InfoProtection DeterminerInfoProtection(IGetPDFICoverageResponse pdfCoverage)
        {
            if (pdfCoverage?.Coverage?.Plan == null)
            {
                return InfoProtection.Aucun;
            }

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

            return result;
        }
    }
}