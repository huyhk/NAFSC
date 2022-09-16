using FMS.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Megatech.NAFSC.WPFApp
{
    [ValueConversion(typeof(string), typeof(REFUEL_ITEM_STATUS))]
    public class StatusConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (targetType != typeof(REFUEL_ITEM_STATUS))
            //    throw new InvalidOperationException("The target must be a REFUEL_ITEM_STATUS.");

            var bValue = (REFUEL_ITEM_STATUS)value;

            FieldInfo field = value.GetType().GetField(value.ToString());
            object[] attribs = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attribs.Length > 0)
            {
                return ((DescriptionAttribute)attribs[0]).Description;
            }
            return string.Empty;

            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
