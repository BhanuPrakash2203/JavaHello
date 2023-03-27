using System;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using ProjectionData = IAFG.IA.VI.Projection.Data;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.Illustration
{
    public interface IConceptVenteMapper
    {
        Types.Models.ConceptVentes.ConceptVente Map(ProjectionData.Projections projections, DonneesRapportIllustration model, DateTime dateEmission);
    }
}
