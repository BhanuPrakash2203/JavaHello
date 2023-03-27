using System;
using IAFG.IA.VE.Impression.Illustration.Types.Models.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;
using ProjectionData = IAFG.IA.VI.Projection.Data;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.Illustration
{
    public interface IHypothesesMapper
    {
        Hypotheses MapHypothesesInvestissement(ProjectionData.Projection projection, DateTime dateEmission, Projections projections);
    };
}