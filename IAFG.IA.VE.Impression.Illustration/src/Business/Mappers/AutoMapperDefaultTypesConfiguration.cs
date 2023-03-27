using AutoMapper.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    internal static class AutoMapperDefaultTypesConfiguration
    {
        public static void Configure(MapperConfigurationExpression cfg, IIllustrationReportDataFormatter illustrationReportDataFormatter)
        {
            //cfg.CreateMap<DateTime, string>().ConstructUsing(reportDataFormatter.Format);
            //cfg.CreateMap<IAdvisor, string>().ConstructUsing(reportDataFormatter.Format);
            //cfg.CreateMap<IIndividualName, string>().ConstructUsing(reportDataFormatter.Format);
            //cfg.CreateMap<Sexe, string>().ConstructUsing(reportDataFormatter.Format);
            //cfg.CreateMap<bool, string>().ConstructUsing(reportDataFormatter.Format);
            //cfg.CreateMap<IAdresse, string>().ConstructUsing(reportDataFormatter.Format);
        }
    }
}