using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using FrodLib.Extensions;

namespace FrodLib
{
    using System.Globalization;
    using winRect = System.Windows.Rect;

    [ExcludeFromCodeCoverage]
    public class RectConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            if (sourceType == typeof(winRect))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                if (((string)value).Equals("empty", StringComparison.InvariantCultureIgnoreCase)) return Rect.Empty;
                string[] values = ((string)value).Split(';');
                if (values.Length == 4)
                {
                    try
                    {
                        Rect rect = new Rect();
                        rect.X = values[0].ToDouble();
                        rect.Y = values[1].ToDouble();
                        rect.Width = values[2].ToDouble();
                        rect.Height = values[3].ToDouble();
                        return rect;
                    }
                    catch (InvalidCastException) { }
                    catch (FormatException) { }
                }
            }

            else if(value is winRect)
            {
                var wRect = (winRect)value;
                return new Rect(wRect.X, wRect.Y, wRect.Width, wRect.Height);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Rect)
            {
                Rect rect = (Rect)value;
                return "{0};{1};{2};{3}".Format(rect.X, rect.Y, rect.Width, rect.Height);
            }
            else if (destinationType == typeof(winRect) && value is Rect)
            {
                var wRect = (Rect)value;
                return new winRect(wRect.X, wRect.Y, wRect.Width, wRect.Height);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            if (destinationType == typeof(winRect))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
    }
}
