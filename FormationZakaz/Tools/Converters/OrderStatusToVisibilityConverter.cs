using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FormationZakaz.Data;
using System.Windows.Data;
using System.Windows;

namespace FormationZakaz.Tools.Converters
{
    public class OrderStatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderStatus status && parameter is string param)
            {
                // Сравниваем статус с параметром
                if (param == "COMPLECTED" && status == OrderStatus.COMPLECTED)
                    return Visibility.Visible;
                if (param == "NOT_COMPLECTED" && status == OrderStatus.NOT_COMPLECTED)
                    return Visibility.Visible;
            }
            return Visibility.Collapsed; // Скрываем для Default или несовпадения
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
