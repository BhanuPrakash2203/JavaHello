using IAFG.IA.VE.Impression.Core.ResourcesAccessor;

namespace IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor
{
    public interface IResourcesAccessorFactory
    {
        ResourcesContexte Contexte { get; set; }
        IResourcesAccessor GetResourcesAccessor();
        IResourcesAccessor GetResourcesAccessor(ResourcesContexte contexte);
    }
}