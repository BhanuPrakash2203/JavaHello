using System.Collections.Generic;
using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using ProjectionData = IAFG.IA.VI.Projection.Data;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.Illustration
{
    public interface IClientsMapper
    {
        List<Client> MapClients(IEnumerable<DonneesClient> clients,
            ProjectionData.Projection projection);
        
        void MapperIndividus(IEnumerable<ProjectionData.Contract.Individual> individuals,
            List<ProjectionData.Contract.Coverage.Joint> joints, List<Client> clients);
    }
}
