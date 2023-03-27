using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    public class AutoMapperFactoryConfiguration : IAutoMapperFactoryConfiguration
    {
        public AutoMapperFactoryConfiguration(params Assembly[] additionnalAssembliesToScanForProfiles)
        {
            ProfilesAssembliesToScan = new List<Assembly>(new[] {GetType().Assembly}.Concat(additionnalAssembliesToScanForProfiles));
            AdditionalProfileTypes = new List<Type>();
        }

        public List<Type> AdditionalProfileTypes { get; private set; }
        public List<Assembly> ProfilesAssembliesToScan { get; private set; }
    }
}