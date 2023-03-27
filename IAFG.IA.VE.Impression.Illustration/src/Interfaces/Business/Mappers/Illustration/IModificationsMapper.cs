using System;
using System.Collections.Generic;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using ProjectionData = IAFG.IA.VI.Projection.Data;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.Illustration
{
    public interface IModificationsMapper
    {
        Types.Models.ModificationsDemandees.ModificationsDemandees MapModificationsDemandees(
            ProjectionData.Projection projection, List<Client> clients, DateTime dateEmission);
    }
}