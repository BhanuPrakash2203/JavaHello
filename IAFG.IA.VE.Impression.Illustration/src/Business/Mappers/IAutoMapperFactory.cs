using AutoMapper;
using IAFG.IA.VE.Impression.Core.Types.Export;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    public interface IAutoMapperFactory
    {
        IMapper InstanceFor(ReportAudienceTypes audience);
    }
}