using System.Collections.Generic;
using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections.Participations;
using ProjectionData = IAFG.IA.VI.Projection.Data;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.Illustration
{
    public interface IProtectionsMapper
    {
        Participations MapParticipations(ProjectionData.Projection projection, ParametreRapport parametreRapport);
        Protections MapperProtections(DonneesIllustration data, ProjectionData.Projection projection, IList<Client> clients);       
        IList<ProtectionsGroupees> MapperProtectionsGroupees(DonneesIllustration data, ProjectionData.Projection projection, IList<Client> clients);
    }
}
