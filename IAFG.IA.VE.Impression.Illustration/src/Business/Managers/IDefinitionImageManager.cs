using System.Collections.Generic;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public interface IDefinitionImageManager
    {
        Dictionary<string, ImageModel> Mapper(Dictionary<string,
            List<DefinitionImageSelonProduit>> images, DonneesRapportIllustration donnees);
    }
}