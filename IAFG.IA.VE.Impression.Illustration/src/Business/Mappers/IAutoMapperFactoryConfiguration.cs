using System;
using System.Collections.Generic;
using System.Reflection;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    public interface IAutoMapperFactoryConfiguration
    {
        List<Type> AdditionalProfileTypes { get; }
        List<Assembly> ProfilesAssembliesToScan { get; }
    }
}