using System;
using System.Linq;
using System.Windows.Data;
using System.Windows;
using System.Drawing;

namespace pepperSoft
{
    public  class ColorToBrushConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string input = value as string;
            if (input != null && input.Count() > 1)
            {
                switch (input[0])
                {
                    case '#':
                        Color _color;
                        try
                        {
                            _color = ColorTranslator.FromHtml(input);
                        }
                        catch (Exception e)
                        {
                            _color = ColorTranslator.FromHtml("#FFFFFF");
                        }
                        return new System.Windows.Media.SolidColorBrush(ToMediaColor(_color));
                    default:
                        return DependencyProperty.UnsetValue;
                }
            }
            else return DependencyProperty.UnsetValue;

        }

        private  System.Windows.Media.Color ToMediaColor(Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}
