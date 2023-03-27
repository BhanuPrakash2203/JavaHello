using System;
using IAFG.IA.VE.Impression.Core.Interface.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;

namespace IAFG.IA.VE.Impression.Core.Formatters
{
    public class DateBuilder : IDateBuilder
    {
        private readonly ICultureAccessor _cultureAccessor;
        private const string FrenchDateFormat = "yyyy-MM-dd";
        private const string DefaultDateFormat = "M/d/yyyy";
        private const string FrenchLongDateFormat = "d MMMM yyyy";
        private const string DefaultLongDateFormat = "MMMM d, yyyy";
        private const string FrenchTimeFormat = " - HH:mm";
        private const string DefaultTimeFormat = " - hh:mm tt";
        private const string MasqueLong = "********";
        private const string MasqueCourt = "**";

        private bool _shouldUseInvariantCulture;
        private bool _shouldUseShortDateFormat;
        private bool _shouldUseLongDateFormat;
        private bool _shouldUsetimeFormat;
        private bool _shouldBePrivate;

        public DateBuilder(ICultureAccessor cultureAccessor)
        {
            _cultureAccessor = cultureAccessor;
        }

        public IDateBuilder WithShortDateFormat()
        {
            _shouldUseShortDateFormat = true;
            return this;
        }

        public IDateBuilder WithLongDateFormat()
        {
            _shouldUseLongDateFormat = true;
            return this;
        }

        public IDateBuilder WithTime()
        {
            _shouldUsetimeFormat = true;
            return this;
        }

        public IDateBuilder WithInvariantCulture()
        {
            _shouldUseInvariantCulture = true;
            return this;
        }

        public IDateBuilder WithPrivacy()
        {
            _shouldBePrivate = true;
            return this;
        }

        public string Build(DateTime value)
        {
            if (_shouldUseShortDateFormat && _shouldUseLongDateFormat)
                throw new InvalidOperationException("On ne peut pas sélectionner le short time format et le long time format en même temps");

            var format = IsFrenchCulture() ? GetFrenchFormat() : GetDefaultFormat();
            var result = _shouldUseInvariantCulture ? value.ToString(format, System.Globalization.CultureInfo.InvariantCulture) : value.ToString(format);

            ResetFlags();
            return result;
        }

        private void ResetFlags()
        {
            _shouldUsetimeFormat = false;
            _shouldUseInvariantCulture = false;
            _shouldUseLongDateFormat = false;
            _shouldUseShortDateFormat = false;
            _shouldBePrivate = false;
        }

        private string GetDefaultFormat()
        {
            string format;

            if (_shouldUseShortDateFormat)
                format = _shouldBePrivate ? DefaultDateFormat.Replace("M", MasqueCourt).Replace("d", MasqueCourt) : DefaultDateFormat;
            else
                format = _shouldBePrivate ? DefaultLongDateFormat.Replace("MMMM", MasqueLong).Replace("d", MasqueCourt) : DefaultLongDateFormat;

            if (_shouldUsetimeFormat)
                format = $"{format}{DefaultTimeFormat}";

            return format;
        }

        private string GetFrenchFormat()
        {
            string format;

            if (_shouldUseShortDateFormat)
                format = _shouldBePrivate ? FrenchDateFormat.Replace("MM", MasqueCourt).Replace("dd", MasqueCourt) : FrenchDateFormat;
            else
                format = _shouldBePrivate ? FrenchLongDateFormat.Replace("d", MasqueCourt).Replace("MMMM", MasqueLong) : FrenchLongDateFormat;

            if (_shouldUsetimeFormat)
                format = $"{format}{FrenchTimeFormat}";

            return format;
        }

        private bool IsFrenchCulture() => Equals(_cultureAccessor.GetCultureInfo(), CultureHelper.FrenchCulture);
    }
}