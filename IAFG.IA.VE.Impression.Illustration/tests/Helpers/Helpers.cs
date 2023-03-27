using IAFG.IA.VE.Impression.Core.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.Types;
using IAFG.IA.VE.Impression.Illustration.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Pilotage;
using IAFG.IA.VE.Impression.Illustration.Resources;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using Unity;
using Unity.Lifetime;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Helpers
{
    internal static class Helpers
    {
        internal static ICultureAccessor CreateCultureAccessor(bool french = false)
        {
            if (french)
            {
                return new CultureAccessorFrench();
            }

            return new CultureAccessorEnglish();
        }
        
        public static IllustrationReportDataFormatter CreateIllustrationReportDataFormatter(bool french, out IUnityContainer container)
        {
            var cultureAcessor = CreateCultureAccessor(french);
            var dateBuilder = new DateBuilder(cultureAcessor);

            container = new UnityContainer();
            container.RegisterInstance(cultureAcessor);
            container.RegisterType<IImpressionResourcesAccessor, ImpressionResourcesAccessor>(new ContainerControlledLifetimeManager());
            container.RegisterType<IIllustrationResourcesAccessorFactory, ResourcesAccessorFactory>(new ContainerControlledLifetimeManager());

            var systemInformation = new SystemInformation();
            var dateFormatter = new DateFormatter(cultureAcessor, dateBuilder);
            var longDateFormatter = new LongDateFormatter(cultureAcessor, dateBuilder);
            var decimalFormatter = new DecimalFormatter(cultureAcessor, dateBuilder);
            var noDecimalFormatter = new NoDecimalFormatter(cultureAcessor, dateBuilder);
            var currencyFormatter = new CurrencyFormatter(cultureAcessor, dateBuilder);
            var currencyWithoutDecimalFormatter = new CurrencyWithoutDecimalFormatter(cultureAcessor, dateBuilder);
            var percentageFormatter = new PercentageFormatter(cultureAcessor, dateBuilder);
            var percentageWithoutSymbolFormatter = new PercentageWithoutSymbolFormatter(cultureAcessor, dateBuilder);
            var configurationRepository = new ConfigurationRepository(new PilotageRapportIllustrationsIncorpores());
            var vectorManager = new VecteurManager();

            return new IllustrationReportDataFormatter(
                systemInformation,
                dateFormatter,
                longDateFormatter,
                decimalFormatter,
                noDecimalFormatter,
                currencyFormatter,
                currencyWithoutDecimalFormatter,
                percentageFormatter,
                percentageWithoutSymbolFormatter,
                cultureAcessor,
                container.Resolve<IIllustrationResourcesAccessorFactory>(),
                configurationRepository,
                dateBuilder,
                vectorManager
            );
        }

        public static IllustrationReportDataFormatter CreateIllustrationReportDataFormatter(bool french = false)
        {
            return CreateIllustrationReportDataFormatter(french, out _);
        }
    }
}
