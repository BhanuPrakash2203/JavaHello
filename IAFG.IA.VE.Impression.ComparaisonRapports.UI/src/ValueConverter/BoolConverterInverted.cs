using System;
using System.Windows.Data;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.ValueConverter
{
    public class BoolConverterInverted : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null || !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}