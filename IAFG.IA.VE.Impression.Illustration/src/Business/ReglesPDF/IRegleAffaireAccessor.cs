using System;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VI.AF.IPDFVie.Factory.Interfaces;
using IAFG.IA.VI.Projection.Data;
using IAFG.IA.VI.Projection.Data.Contract.Coverage;
using IPlanInfo = IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF.Types.IPlanInfo;

namespace IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF
{
    public interface IRegleAffaireAccessor
    {
        IPlanInfo ObtenirPlan(string codePlan);
        bool EstCompteInteretGarantie(string vehicule);
        bool EstComptePorteuille(string vehicule);
        IGetPDFICoverageResponse GetPdfCoverage(Projection projectionData, Coverage coverage);
        IGetPDFICoverageResponse GetPdfCoverage(Projection projectionData, AdditionalBenefit additionalBenefit);
        InfoProtection DeterminerInfoProtection(Projection projectionData, Coverage coverage);
        InfoProtection DeterminerInfoProtection(Projection projectionData, AdditionalBenefit additionalBenefit);
    }
}