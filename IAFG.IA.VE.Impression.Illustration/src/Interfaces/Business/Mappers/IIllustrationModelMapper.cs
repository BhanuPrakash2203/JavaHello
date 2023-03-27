using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Models;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers
{
    public interface IIllustrationModelMapper
    {
        DonneesRapportIllustration Map(DonneesIllustration data);
    }
}