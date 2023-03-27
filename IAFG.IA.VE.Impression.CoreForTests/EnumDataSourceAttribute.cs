using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.CoreForTests
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class EnumDataSourceAttribute : Attribute, ITestDataSource
    {
        private readonly Type _enumDataSource;
        private readonly object[] _exclusions;

        public EnumDataSourceAttribute(Type enumDataSource, params object[] exclusions)
        {
            _enumDataSource = enumDataSource ??
                              throw new ArgumentNullException(nameof(enumDataSource), "L'enum à itérer n'est pas spécifié.");

            _exclusions = exclusions;
        }

        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            if (_enumDataSource.BaseType != typeof(Enum))
                throw new InvalidCastException("Le type demandé n'est pas un enum.");

            foreach (var value in Enum.GetValues(_enumDataSource))
                if (!_exclusions.Contains(value))
                    yield return new[] { value };
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            return data == null ? null : $"{methodInfo.Name} ({string.Join(", ", data)})";
        }
    }
}
