using System;
using System.Windows.Data;
using System.Windows.Input;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.ValueConverter
{
    public class BoolToWaitCursorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var waitCursor = value != null && (bool)value;

            return waitCursor ? Cursors.Wait : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}