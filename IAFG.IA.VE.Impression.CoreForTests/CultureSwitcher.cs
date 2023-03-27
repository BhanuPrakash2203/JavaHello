using System;
using System.Globalization;
using AutoFixture;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;
using NSubstitute;

namespace IAFG.IA.VE.Impression.CoreForTests
{
    public sealed class CultureSwitcher : IDisposable
    {
        private static readonly IFixture _auto = AutoFixtureFactory.Create();
        private readonly CultureAccessor _accessor;
        private readonly CultureInfo _previousCulture;

        private readonly IResourcesAccessorFactory _resourceAccessorFactory = Substitute.For<IResourcesAccessorFactory>();
        private readonly IResourcesAccessor _resourcesAccessor = _auto.Create<IResourcesAccessor>();
        private CultureSwitcher(string cultureCode)
        {
            _resourceAccessorFactory.GetResourcesAccessor().Returns(_resourcesAccessor);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureCode);
            _accessor = new CultureAccessor();
            _previousCulture = _accessor.GetCultureInfo();
            _accessor.SetCultureInfo(cultureCode, _resourceAccessorFactory);
        }

        public static CultureSwitcher SwitchTo(string cultureCode)
        {
            return new CultureSwitcher(cultureCode);
        }

        public static CultureSwitcher SwitchToEnglish()
        {
            return SwitchTo("en-CA");
        }

        public static CultureSwitcher SwitchToFrench()
        {
            return SwitchTo("fr-CA");
        }

        public void Dispose()
        {
            _accessor.SetCultureInfo(_previousCulture, _resourceAccessorFactory);

        }
    }
}