using System;
using System.Linq;
using System.Reflection;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.Common.Classes
{
    public class Reflector
    {
        private readonly Assembly _asmb;

        public Reflector(string an)
        {
            _asmb = null;

            var asemb = Assembly.GetExecutingAssembly().GetReferencedAssemblies().First(a => a.FullName.StartsWith(an));

            _asmb = Assembly.Load(asemb);
        }

        public Type GetType(string typeName)
        {
            var type = _asmb.GetType(typeName);

            return type;
        }

        public object Call(object obj, string func, params object[] parameters)
        {
            return Call2(obj, func, parameters);
        }

        private object Call2(object obj, string func, object[] parameters)
        {
            return CallAs2(obj.GetType(), obj, func, parameters);
        }

        public object CallAs(Type type, object obj, string func, params object[] parameters)
        {
            return CallAs2(type, obj, func, parameters);
        }

        private object CallAs2(Type type, object obj, string func, object[] parameters)
        {
            var methInfo = type.GetMethod(func, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return methInfo?.Invoke(obj, parameters);
        }

        public object Get(object obj, string prop)
        {
            return GetAs(obj.GetType(), obj, prop);
        }

        public object GetAs(Type type, object obj, string prop)
        {
            var propInfo = type.GetProperty(prop, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return propInfo != null ? propInfo.GetValue(obj, null) : null;
        }

        public object GetEnum(string typeName, string name)
        {
            var type = GetType(typeName);
            var fieldInfo = type.GetField(name);
            return fieldInfo.GetValue(null);
        }
    }
}
