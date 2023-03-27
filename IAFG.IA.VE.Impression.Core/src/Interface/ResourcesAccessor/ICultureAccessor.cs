using System.Globalization;

namespace IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor
{
    public interface ICultureAccessor
    {
        CultureInfo GetCultureInfo();
        void SetCultureInfo(string newCultureInfo, IResourcesAccessorFactory resourcesAccessor);
        void SetCultureInfo(CultureInfo newCultureInfo, IResourcesAccessorFactory resourcesAccessor);
    }
}