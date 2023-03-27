using System.Globalization;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Helpers
{
    public class CultureAccessorFrench : ICultureAccessor
    {
        private readonly CultureInfo _cultureInfo;

        public CultureAccessorFrench()
        {
            _cultureInfo = new CultureInfo("fr-CA", false);
        }

        public CultureInfo GetCultureInfo()
        {
            return _cultureInfo;
        }

        public void SetCultureInfo(string newCultureInfo, IResourcesAccessorFactory resourcesAccessor) { }
        public void SetCultureInfo(CultureInfo newCultureInfo, IResourcesAccessorFactory resourcesAccessor) { }
    }
}