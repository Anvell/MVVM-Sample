using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Globalization;

namespace MVVMSample.Converters {

    abstract class BaseConverter : MarkupExtension {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }

    [ValueConversion(typeof(object), typeof(string))]
    class TimeConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var info = (DateTime)value;
            if (info != null)
            {
                if (info != DateTime.MinValue) return info.ToString("yyyy-MM-dd");
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime time;
            if (DateTime.TryParse((string)value, out time))
            {
                return time;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
    }

    [ValueConversion(typeof(object), typeof(string))]
    class BooleanToStringConverter : BaseConverter, IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if ((bool)value) {
                return "Yes";
            }
            return "No";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}
