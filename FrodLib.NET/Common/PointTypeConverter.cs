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
    using winPoint = System.Windows.Point;

    [ExcludeFromCodeCoverage]
    public class PointConverter : TypeConverter
    {
        // Overrides the CanConvertFrom method of TypeConverter.
        // The ITypeDescriptorContext interface provides the context for the
        // conversion. Typically, this interface is used at design time to 
        // provide information about the design-time container.
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            if (sourceType == typeof(winPoint))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }
        // Overrides the ConvertFrom method of TypeConverter.
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string[] values = ((string)value).Split(';');
                if (values.Length == 2)
                {
                    try
                    {
                        Point point = new Point();
                        point.X = values[0].ToDouble();
                        point.Y = values[1].ToDouble();
                        return point;
                    }
                    catch (InvalidCastException) { }
                    catch (FormatException) { }
                }
            }
            else if (value is winPoint)
            {
                var wPoint = (winPoint)value;
                return new Point(wPoint.X, wPoint.Y);
            }
            return base.ConvertFrom(context, culture, value);
        }
        // Overrides the ConvertTo method of TypeConverter.
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                Point point = (Point)value;
                return point.X + ";" + point.Y;
            }
            else if (destinationType == typeof(winPoint))
            {
                var wPoint = (Point)value;
                return new winPoint(wPoint.X, wPoint.Y);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            if (destinationType == typeof(winPoint))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
    }
}
