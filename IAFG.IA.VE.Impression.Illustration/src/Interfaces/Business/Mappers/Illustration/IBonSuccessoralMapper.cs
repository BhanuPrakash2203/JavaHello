using ProjectionData = IAFG.IA.VI.Projection.Data;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.Illustration
{
    public interface IBonSuccessoralMapper
    {
        Types.Models.BonSuccessoral.BonSuccessoral Map(ProjectionData.Projections projections, Types.Models.HypothesesInvestissement.Hypotheses HypothesesInvestissement);
    }
}
