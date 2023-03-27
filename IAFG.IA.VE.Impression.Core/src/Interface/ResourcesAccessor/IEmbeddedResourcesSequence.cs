using System.Collections.Generic;

namespace IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor
{
    public interface IEmbeddedResourcesSequence
    {
        IEnumerable<IEmbeddedResourcesReader> GetReaders();
    }
}