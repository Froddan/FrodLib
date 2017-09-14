using FrodLib.MathUtils;
using FrodLib.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml;

namespace FrodLib.Semantics
{
    public enum TemperatureUnit
    {
        Celsius, Fahrenheit, Kelvin
    }


    public sealed class Temperature : ComparableSemanticType<double>, IEquatable<Temperature>, IComparable<Temperature>
    {
        public const float AbsoluteZeroCelcius = -273.15f;

        public const float AbsoluteZeroFahrenheit = -459.67f;

        public const float AbsoluteZeroKelvin = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double m_temperatureCelcius;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TemperatureUnit m_temperatureUnit;

        static Temperature()
        {
            var currentThreadCulture = CultureInfo.CurrentCulture;
            switch (currentThreadCulture.TwoLetterISOLanguageName)
            {
                case "AS": // American Samoa
                case "BS": // BAHAMAS
                case "BZ": // Belize
                case "KY": // Cayman Islands
                case "PW": // PALAU
                case "US": // USA
                    DefaultTemperatureUnit = TemperatureUnit.Fahrenheit;
                    break;
                default:
                    DefaultTemperatureUnit = TemperatureUnit.Celsius;
                    break;
            }

        }

        public Temperature(double temperature)
                    : this(temperature, DefaultTemperatureUnit)
        {

        }

        public Temperature(double temperature, TemperatureUnit unit) : base(null, temperature)
        {

            if (ValidateTemperateure(temperature, unit) == false)
            {
                throw new TemperatureBelowAbsoluteZeroException();
            }

            if (unit == TemperatureUnit.Fahrenheit)
            {
                temperature = FahrenheitToCelcius(temperature);
            }
            else if (unit == TemperatureUnit.Kelvin)
            {
                temperature = KelvinToCelsius(temperature);
            }

            m_temperatureCelcius = temperature;
            m_temperatureUnit = unit;
        }

        /// <summary>
        /// Sets or gets the default temperature unit that will be set if no explicit unit is passed to the constructor
        /// </summary>
        public static TemperatureUnit DefaultTemperatureUnit { get; set; }

        public TemperatureUnit Unit
        {
            get
            {
                return m_temperatureUnit;
            }
        }

        public override double Value
        {
            get
            {
                switch (m_temperatureUnit)
                {
                    case TemperatureUnit.Celsius:
                        return m_temperatureCelcius;
                    case TemperatureUnit.Fahrenheit:
                        return Math.Round(CelciusToFahrenheit(m_temperatureCelcius), 4);
                    case TemperatureUnit.Kelvin:
                        return Math.Round(CelciusToKelvin(m_temperatureCelcius), 4);
                    default:
                        throw new InvalidOperationException(StringResources.UnknownTemperatureUnit);
                }
            }
        }

        public static double CelciusToFahrenheit(double temperatureCelcius)
        {
            return temperatureCelcius * 1.8f + 32;
        }

        public static double CelciusToKelvin(double temperatureCelcius)
        {
            return temperatureCelcius + 273.15;
        }

        public static double FahrenheitToCelcius(double temperatureFahrenheit)
        {
            return (temperatureFahrenheit - 32) / 1.8f;
        }

        public static double FahrenheitToKelvin(double temperatureFahrenheit)
        {
            return (temperatureFahrenheit + 459.67) * (5 / 9);
        }

        public static Temperature FromCelcius(double temperatureCelcius)
        {
            return new Temperature(temperatureCelcius, TemperatureUnit.Celsius);
        }

        public static Temperature FromFahrenheit(double temperatureFahrenheit)
        {
            return new Temperature(temperatureFahrenheit, TemperatureUnit.Fahrenheit);
        }

        public static Temperature FromKelvin(double temperatureKelvin)
        {
            return new Temperature(temperatureKelvin, TemperatureUnit.Kelvin);
        }

        public static double KelvinToCelsius(double temperatureKelvin)
        {
            return temperatureKelvin - 273.15;
        }

        public static double KelvinToFahrenheit(double temperatureKelvin)
        {
            return (temperatureKelvin * (9 / 5)) - 459.67;
        }

        public static explicit operator float (Temperature temp)
        {
            return (float)temp.Value;
        }

        public int CompareTo(Temperature other)
        {
            return this.m_temperatureCelcius.CompareTo(other.m_temperatureCelcius);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is Temperature)
            {
                return Equals((Temperature)obj);
            }
            return base.Equals(obj);
        }

        public bool Equals(Temperature other)
        {
            if ((object)other == null) return false;
            return FrodMath.Equals(this.m_temperatureCelcius, other.m_temperatureCelcius, 0.00000001);

        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 269) ^ this.m_temperatureUnit.GetHashCode();
                hash = (hash * 269) ^ this.m_temperatureCelcius.GetHashCode();
                return hash;
            }
        }

        public Temperature ToCelsius()
        {
            return new Temperature(m_temperatureCelcius, TemperatureUnit.Celsius);
        }

        public Temperature ToFahrenheit()
        {
            return new Temperature(CelciusToFahrenheit(m_temperatureCelcius), TemperatureUnit.Fahrenheit);
        }

        public Temperature ToKelvin()
        {
            return new Temperature(CelciusToKelvin(m_temperatureCelcius), TemperatureUnit.Kelvin);
        }

        public override string ToString()
        {
            switch (m_temperatureUnit)
            {
                case TemperatureUnit.Celsius:
                    return m_temperatureCelcius + " °C";
                case TemperatureUnit.Fahrenheit:
                    return Math.Round(CelciusToFahrenheit(m_temperatureCelcius), 4) + " °F";
                case TemperatureUnit.Kelvin:
                    return Math.Round(CelciusToKelvin(m_temperatureCelcius), 4) + " K";
                default:
                    return StringResources.UnknownTemperatureUnit;
            }
        }

        protected override void _readXml(XmlReader reader)
        {
            reader.MoveToContent();
            bool isEmptyElement = reader.IsEmptyElement; // (1)
            if (!isEmptyElement) // (1)
            {
                for (int i = 0; i < 2; i++)
                {
                    reader.ReadStartElement();
                    string elementName = reader.Name;
                    if (elementName.Equals("Value", StringComparison.CurrentCultureIgnoreCase))
                    {
                        m_temperatureCelcius = double.Parse(reader.Value);
                    }
                    else if (elementName.Equals("Unit", StringComparison.CurrentCultureIgnoreCase))
                    {
                        m_temperatureUnit = (TemperatureUnit)Enum.Parse(typeof(TemperatureUnit), reader.Value, true);

                    }
                    reader.ReadEndElement();
                }
            }
        }

        protected override void _writeXml(XmlWriter writer)
        {
            writer.WriteElementString("Value",
                Value.ToString());
            writer.WriteElementString("Unit", Unit.ToString());
        }

        /// <summary>
        /// Check if both values have the same unit, else convert value 2 to the same unit as value one
        /// </summary>
        /// <param name="temp1"></param>
        /// <param name="temp2"></param>
        /// <returns>Returns true if value was converted</returns>
        private static bool ConvertToSameUnit(Temperature temp1, ref Temperature temp2)
        {
            if (temp1.Unit != temp2.Unit)
            {
                switch (temp1.Unit)
                {
                    case TemperatureUnit.Celsius:
                        temp2 = temp2.ToCelsius();
                        break;
                    case TemperatureUnit.Fahrenheit:
                        temp2 = temp2.ToFahrenheit();
                        break;
                    case TemperatureUnit.Kelvin:
                        temp2 = temp2.ToKelvin();
                        break;
                }

                return true;
            }

            return false;
        }

        private bool ValidateTemperateure(double temp, TemperatureUnit unit)
        {
            switch (unit)
            {
                case TemperatureUnit.Fahrenheit:
                    return ((float)temp) >= AbsoluteZeroFahrenheit;
                case TemperatureUnit.Kelvin:
                    return ((float)temp) >= AbsoluteZeroKelvin;
                case TemperatureUnit.Celsius:
                    return ((float)temp) >= AbsoluteZeroCelcius;
            }

            return true;
        }
        #region + OPERATOR

        public static Temperature operator +(Temperature temp1, Temperature temp2)
        {
            if (EitherNull(temp1, temp2)) throw new ArgumentNullException("Addition argument cannot be null");

            ConvertToSameUnit(temp1, ref temp2);

            var value1 = temp1.Value;
            var value2 = temp2.Value;
            return new Temperature(value1 + value2, temp1.m_temperatureUnit);
        }

        public static Temperature operator +(Temperature temp1, double valueToAdd)
        {
            var currTemp = temp1.Value;
            currTemp += valueToAdd;
            return new Temperature(currTemp, temp1.Unit);

        }

        #endregion

        #region - OPERATOR

        public static Temperature operator -(Temperature temp1, Temperature temp2)
        {
            if (EitherNull(temp1, temp2)) throw new ArgumentNullException("Subtraction argument cannot be null");

            ConvertToSameUnit(temp1, ref temp2);

            var value1 = temp1.Value;
            var value2 = temp2.Value;
            return new Temperature(value1 - value2, temp1.m_temperatureUnit);
        }

        public static Temperature operator -(Temperature temp1, double valueToSubtract)
        {
            var currTemp = temp1.Value;
            currTemp -= valueToSubtract;
            return new Temperature(currTemp, temp1.Unit);
        }

        #endregion

        #region * OPERATOR

        public static Temperature operator *(Temperature temp1, Temperature temp2)
        {
            if (EitherNull(temp1, temp2)) throw new ArgumentNullException("Multiplication argument cannot be null");

            ConvertToSameUnit(temp1, ref temp2);

            var value1 = temp1.Value;
            var value2 = temp2.Value;
            return new Temperature(value1 * value2, temp1.m_temperatureUnit);
        }

        public static Temperature operator *(Temperature temp1, double valueToMultiply)
        {
            var currTemp = temp1.Value;
            currTemp *= valueToMultiply;
            return new Temperature(currTemp, temp1.Unit);
        }

        #endregion

        #region / OPERATOR

        public static Temperature operator /(Temperature temp1, Temperature temp2)
        {
            if (EitherNull(temp1, temp2)) throw new ArgumentNullException("division argument cannot be null");
            TemperatureUnit oldUnit = temp2.Unit;
            if (ConvertToSameUnit(temp1, ref temp2))
            {
                var value1 = temp1.Value;
                var value2 = temp2.Value;

                if (value2 == 0) throw new DivideByZeroException(string.Format(StringResources.DivisionByZeroExceptionTextFormat, oldUnit, temp1.Unit));

                return new Temperature(value1 / value2, temp1.m_temperatureUnit);
            }
            else
            {
                var value1 = temp1.Value;
                var value2 = temp2.Value;

                if (value2 == 0) throw new DivideByZeroException();

                return new Temperature(value1 / value2, temp1.m_temperatureUnit);
            }
        }

        public static Temperature operator /(Temperature temp1, double valueToDivide)
        {
            if (valueToDivide == 0) throw new DivideByZeroException();
            var currTemp = temp1.Value;
            currTemp /= valueToDivide;
            return new Temperature(currTemp, temp1.Unit);
        }

        #endregion

        #region COMPARES

        public static bool operator !=(Temperature temp1, double @double)
        {

            return !temp1.Equals(@double);
        }

        public static bool operator <(Temperature temp1, double @double)
        {
            return temp1.m_temperatureCelcius < @double;
        }

        public static bool operator <=(Temperature temp1, double @double)
        {
            return temp1.m_temperatureCelcius <= @double;
        }

        public static bool operator ==(Temperature temp1, double @double)
        {
            return temp1.Equals(@double);
        }

        public static bool operator >(Temperature temp1, double @double)
        {
            return temp1.m_temperatureCelcius > @double;
        }

        public static bool operator >=(Temperature temp1, double @double)
        {

            return temp1.m_temperatureCelcius >= @double;
        }
        #endregion
    }

    public class TemperatureBelowAbsoluteZeroException : Exception
    {

        public TemperatureBelowAbsoluteZeroException() : base(StringResources.BelowAbsoluteZeroErrorMessage)
        {

        }

        public TemperatureBelowAbsoluteZeroException(string message)
            : base(message)
        {

        }

        public TemperatureBelowAbsoluteZeroException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
