using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ValorantKillsChallenge
{
    internal class IntToStringConverter : IValueConverter
    {
        private object IntToString(object value)
        {
            if (value is int) return value.ToString();
            return "0";
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return IntToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
