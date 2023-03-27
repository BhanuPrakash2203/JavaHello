using System.Collections.Generic;
using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;
using ProjectionData = IAFG.IA.VI.Projection.Data;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.Illustration
{
    public interface IProjectionsMapper
    {
        Projections Map(ProjectionData.Projections projections, 
            ParametreRapport parametreRapport, ConfigurationRapport configurationRapport);

        Facturation MapFacturation(ProjectionData.Projection projection);
    
        AvancesSurPolice MapAvancesSurPolice(ProjectionData.Projection projection);
    }
}
