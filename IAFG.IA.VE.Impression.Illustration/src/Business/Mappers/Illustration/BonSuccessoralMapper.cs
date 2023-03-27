using System;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.Illustration;
using IAFG.IA.VE.Impression.Illustration.Types.Models.BonSuccessoral;
using ProjectionData = IAFG.IA.VI.Projection.Data;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.Illustration
{
    public class BonSuccessoralMapper : IBonSuccessoralMapper
    {
        private readonly IProjectionManager _projectionManager;
        private readonly IRegleAffaireAccessor _regleAffaireAccessor;

        public BonSuccessoralMapper(
            IProjectionManager projectionManager, 
            IRegleAffaireAccessor regleAffaireAccessor)
        {
            _projectionManager = projectionManager;
            _regleAffaireAccessor = regleAffaireAccessor;
        }

        public Types.Models.BonSuccessoral.BonSuccessoral Map(
            ProjectionData.Projections projections, 
            Types.Models.HypothesesInvestissement.Hypotheses HypothesesInvestissement)
        {
            var projectionBonSuccessoral = _projectionManager.GetEstateBondProjection(projections);
            if (projectionBonSuccessoral == null) return null;

            var projection = _projectionManager.GetDefaultProjection(projections);
            if (projection == null) return null;
            var protectionBase = _projectionManager.GetMainCoverage(projection);
            if (protectionBase == null) return null;
                        
            var result = new Types.Models.BonSuccessoral.BonSuccessoral
            {
                Plan = MapperPlan(protectionBase.PlanCode),
                MontantProtectionInitial = protectionBase.FaceAmount.Actual,               
                TauxInvestissement = HypothesesInvestissement.FondsCapitalisation?.RendementMoyenCompte,
                Hypotheses = MapperHypotheses(projectionBonSuccessoral), 
                Impositions = MapperImposition(projection)               
            };

            return result;
        }

        private Plan MapperPlan(string planCode)
        {
            var planInfo = _regleAffaireAccessor.ObtenirPlan(planCode);
            return new Plan { CodePlan = planCode, DescriptionFr = planInfo.DescriptionFr, DescriptionAn = planInfo.DescriptionAn };
        }

        private Hypotheses MapperHypotheses(ProjectionData.Projection projection)
        {
            var hypothese = projection?.Concept?.EstateBond?.Assumptions?.List?.FirstOrDefault();
            if (hypothese == null) return null;

            var result = new Hypotheses();
            if (hypothese.InterestInvestment != null)
            {
                result.Interets = new Hypothese 
                { 
                    Repartition = hypothese.InterestInvestment.Allocation, 
                    TauxRendement = hypothese.InterestInvestment.RateOfReturn 
                };
            }

            if (hypothese.DividendInvestment != null)
            {
                result.Dividendes = new Hypothese 
                { 
                    Repartition = hypothese.DividendInvestment.Allocation, 
                    TauxRendement = hypothese.DividendInvestment.RateOfReturn 
                };
            }

            if (hypothese.CapitalGainInvestment != null)
            {
                result.GainCapital = new Hypothese 
                { 
                    Repartition = hypothese.CapitalGainInvestment.Allocation, 
                    TauxRendement = hypothese.CapitalGainInvestment.RateOfReturn, 
                    TauxRealisation = hypothese.CapitalGainRealizationRate 
                };
            }

            return result;
        }

        private Impositions MapperImposition(ProjectionData.Projection projection)
        {
            if (_projectionManager.ContractantEstCompagnie(projection)) 
            {
                var impotCorporation = new Impositions
                {
                    EstCorporation = true,
                    TauxMarginal = projection.Parameters?.Taxation?.Corporate?.MarginalRates?.FirstOrDefault().Value,
                    TauxDividendes = projection.Parameters?.Taxation?.Corporate?.DividendRates?.FirstOrDefault().Value,
                    TauxDividendesActionnaires = projection.Parameters?.Taxation?.Personal?.DividendRates?.FirstOrDefault().Value,
                    TauxGainCapital = projection.Parameters?.Taxation?.Corporate?.CapitalGainsRates?.FirstOrDefault().Value
                };
                
                return impotCorporation;
            }

            var impotIndividu = new Impositions
            {
                TauxMarginal = projection.Parameters?.Taxation?.Personal?.MarginalRates?.FirstOrDefault().Value,
                TauxDividendes = projection.Parameters?.Taxation?.Personal?.DividendRates?.FirstOrDefault().Value,
                TauxGainCapital = projection.Parameters?.Taxation?.Personal?.CapitalGainsRates?.FirstOrDefault().Value
            };

            return impotIndividu;
        }
    }
}
