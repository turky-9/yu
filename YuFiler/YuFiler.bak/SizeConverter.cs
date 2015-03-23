using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;

namespace YuFiler
{
    public class SizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string ret = null;
            if(value == null)
                return ret;

            int size = 0;
            if (int.TryParse(value.ToString(), out size))
            {
                size /= 1024;
                ret = size.ToString() + "Kb";
            }
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
