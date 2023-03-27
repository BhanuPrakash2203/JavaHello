using System.Globalization;
using System.Threading;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;

namespace IAFG.IA.VE.Impression.Core.ResourcesAccessor
{
    public class CultureAccessor : ICultureAccessor
    {
        public CultureInfo GetCultureInfo()
        {
            return Thread.CurrentThread.CurrentUICulture;
        }

        public void SetCultureInfo(string newCultureInfo, IResourcesAccessorFactory resourcesAccessor)
        {
            SetCultureInfo(new CultureInfo(newCultureInfo), resourcesAccessor);
        }

        public void SetCultureInfo(CultureInfo newCultureInfo, IResourcesAccessorFactory resourcesAccessor)
        {
            Thread.CurrentThread.CurrentUICulture = newCultureInfo;
            Thread.CurrentThread.CurrentCulture = newCultureInfo;

            resourcesAccessor?.GetResourcesAccessor().ResetCulture();
        }
    }

    public static class CultureHelper
    {
        public static readonly CultureInfo FrenchCulture = CultureInfo.GetCultureInfo("fr-CA");
        public static readonly CultureInfo DefaultCulture = CultureInfo.GetCultureInfo("en-CA");
    }
}
