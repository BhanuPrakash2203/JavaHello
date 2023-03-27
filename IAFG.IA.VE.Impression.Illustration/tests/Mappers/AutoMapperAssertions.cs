using AutoMapper;
using IAFG.IA.VE.Impression.Core.Types.Export;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Mappers
{
    public static class AutoMapperAssertions
    {
        public static void AssertConfigurationIsValid<T>() where T : Profile, new()
        {
            AssertConfigurationIsValid<T>(ReportAudienceTypes.FullClearance);
        }

        public static void AssertConfigurationIsValidForPrivacyContext<T>() where T : Profile, new()
        {
            AssertConfigurationIsValid<T>(ReportAudienceTypes.Privacy);
        }

        private static void AssertConfigurationIsValid<T>(ReportAudienceTypes audience) where T : Profile, new()
        {
            var autoMapperFactory = new AutoMapperFactory(
                Substitute.For<IIllustrationReportDataFormatter>(), 
                Substitute.For<IIllustrationResourcesAccessorFactory>(), 
                Substitute.For<IManagerFactory>());

            autoMapperFactory.InstanceFor(audience).ConfigurationProvider.AssertConfigurationIsValid<T>();
        }
    }
}