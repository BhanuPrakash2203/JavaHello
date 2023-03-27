using System;
using System.Globalization;
using IAFG.IA.VE.Impression.Core.Interface.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;

namespace IAFG.IA.VE.Impression.Core.Formatters
{
    public abstract class ValueFormatter : IValueFormatter
    {

        protected ValueFormatter(ICultureAccessor cultureAccessor, IDateBuilder dateBuilder)
        {
            CultureAccessor = cultureAccessor;
            DateBuilder = dateBuilder;
        }

        protected ICultureAccessor CultureAccessor { get; }
        protected IDateBuilder DateBuilder { get; }

        public string Format(object value)
        {
            var type = value.GetType();

            if (type == typeof(bool)) return Format(Convert.ToString(value));
            if (type == typeof(DateTime)) return Format(Convert.ToDateTime(value));
            if (type == typeof(int)) return Format(Convert.ToInt32(value));
            if (type == typeof(float)) return Format(Convert.ToSingle(value));
            if (type == typeof(double)) return Format(Convert.ToDouble(value));
            return Format(Convert.ToString(value));
        }

        public virtual string Format(int value) => value.ToString();

        public virtual string Format(string value) => value ?? string.Empty;

        public virtual string Format(double value) => value.ToString(CultureInfo.CurrentCulture);

        public virtual string Format(DateTime value) => DateBuilder.WithLongDateFormat().Build(value);

        public virtual string Format(float value) => value.ToString(CultureInfo.GetCultureInfo("FR-ca"));
    }
}