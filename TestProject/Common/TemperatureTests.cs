using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrodLib.Semantics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace FrodLib.Tests
{
    [TestClass]
    public class TemperatureTests
    {
        
        [TestMethod]
        [TestCategory("Temperature")]
        public void TemperatureConstructorTest()
        {
            for(int i = -150;  i<= 150; i += 10)
            {
                Temperature celcius = new Temperature(i, TemperatureUnit.Celsius);
                double value = celcius.Value;
                Assert.AreEqual(i, value, "Celsius Constructor");
            }

            for (int i = -150; i <= 150; i += 10)
            {
                Temperature celcius = new Temperature(i, TemperatureUnit.Fahrenheit);
                double value = celcius.Value;
                Assert.AreEqual(i, value, "Fahrenheit Constructor");
            }

            for (int i = 0; i <= 300; i += 10)
            {
                Temperature celcius = new Temperature(i, TemperatureUnit.Kelvin);
                double value = celcius.Value;
                Assert.AreEqual(i, value, "Kelvin Constructor");
            }

        }

        [TestMethod]
        [TestCategory("Temperature")]
        public void TemperatureToCelsiusTest()
        {
            //Fahrenheit -> Celsius
            for (int i = 0, j = 0; i <= 1000; i += 10, j++)
            {
                Temperature fahrenheitTemp = new Temperature(i, TemperatureUnit.Fahrenheit);
                double expectedCelcius = -17.77778 + (j * 5.55556);
                expectedCelcius = Math.Round(expectedCelcius, 2);

                Temperature celsiusTemp = fahrenheitTemp.ToCelsius();
                double celciusDouble = Math.Round(celsiusTemp.Value, 2);
                Assert.AreEqual(expectedCelcius, celciusDouble, "Fahrenheit ("+i+ " °F) -> Celcius");
            }

            //Kelvin -> Celsius
            for (int i = 0, j = 0; i <= 1000; i += 10, j++)
            {
                Temperature kelvinTemp = new Temperature(i, TemperatureUnit.Kelvin);
                double expectedCelsius = -273.15 + (j * 10);
                expectedCelsius = Math.Round(expectedCelsius, 2);

                Temperature celsiusTemp = kelvinTemp.ToCelsius();
                double celciusDouble = Math.Round(celsiusTemp.Value, 2);
                Assert.AreEqual(expectedCelsius, celciusDouble, "Kelvin (" + i + " K) -> Celsius");
            }

        }

        [TestMethod]
        [TestCategory("Temperature")]
        public void TemperatureToFahrenheitTest()
        {
            // Celcius -> Fahrenheit
            for (int i = 0, j = 0; i <= 1000; i += 10, j++)
            {
                Temperature celciusTemp = new Temperature(i, TemperatureUnit.Celsius);
                double expectedFahrenheit = 32.0 + (j * 18);
                expectedFahrenheit = Math.Round(expectedFahrenheit, 2);

                Temperature fahrenheitTemp = celciusTemp.ToFahrenheit();
                double FahrenheitDouble = Math.Round(fahrenheitTemp.Value, 2);
                Assert.AreEqual(expectedFahrenheit, FahrenheitDouble, "Celsius (" + i + " °C) -> Fahrenheit");
            }

            // Kelvin -> Fahrenheit
            for (int i = 0, j = 0; i <= 1000; i += 10, j++)
            {
                Temperature kelvinTemp = new Temperature(i, TemperatureUnit.Kelvin);
                double expectedFahrenheit = -459.67 + (j * 18);
                expectedFahrenheit = Math.Round(expectedFahrenheit, 2);

                Temperature fahrenheitTemp = kelvinTemp.ToFahrenheit();
                double FahrenheitDouble = Math.Round(fahrenheitTemp.Value, 2);
                Assert.AreEqual(expectedFahrenheit, FahrenheitDouble, "Kelvin ("+i+" K) -> Fahrenheit");
            }
        }


        [TestMethod]
        [TestCategory("Temperature")]
        public void TemperatureToKelvinTest()
        {
            //Fahrenheit -> Kelvin
            for (int i = 0, j = 0; i <= 1000; i += 10, j++)
            {
                Temperature fahrenheitTemp = new Temperature(i, TemperatureUnit.Fahrenheit);
                double expectedKevlin = 255.37222 + (j * 5.55556);
                expectedKevlin = Math.Round(expectedKevlin, 2);

                Temperature kelvinTemp = fahrenheitTemp.ToKelvin();
                double kelvinDouble = Math.Round(kelvinTemp.Value, 2);
                Assert.AreEqual(expectedKevlin, kelvinDouble, "Fahrenheit (" + i + " °F) -> Kelvin");
            }

            
            // Celcius -> Kelvin
            for (int i = 0, j = 0; i <= 1000; i += 10, j++)
            {
                Temperature celciusTemp = new Temperature(i, TemperatureUnit.Celsius);
                double expectedKelvin = 273.15 + (j * 10);
                expectedKelvin = Math.Round(expectedKelvin, 2);

                Temperature kelvinTemp = celciusTemp.ToKelvin();
                double kelvinDouble = Math.Round(kelvinTemp.Value, 2);
                Assert.AreEqual(expectedKelvin, kelvinDouble, "Celsius (" + i + " °C) -> Fahrenheit");
            }
        }


        [TestMethod]
        [TestCategory("Temperature")]
        public void TemperatureAddTest()
        {
            const int factor = 15;
            TemperatureUnit[] tempUnits = { TemperatureUnit.Celsius, TemperatureUnit.Fahrenheit, TemperatureUnit.Kelvin };

            for(int i = 0; i< tempUnits.Length;i++)
            {
                for (int j = 0; j < 1000; j += 10)
                {
                    double expected = j + factor;
                    Temperature temperature = new Temperature(j, tempUnits[i]);
                    temperature = temperature + factor;

                    Assert.AreEqual(expected, temperature.Value, "(double) " + j + " " + tempUnits[i]);

                    Temperature expectedTemperature = new Temperature(j + factor, tempUnits[i]);
                    temperature = new Temperature(j, tempUnits[i]);
                    var temperatureFactor = new Temperature(factor, tempUnits[i]);
                    temperature = temperature + temperatureFactor;

                    Assert.AreEqual(expected, temperature.Value, "(Temperature) " + j + " " + tempUnits[i]);
                }
            }
        }

        [TestMethod]
        [TestCategory("Temperature")]
        public void TemperatureSubtractTest()
        {
            const int factor = 15;
            TemperatureUnit[] tempUnits = { TemperatureUnit.Celsius, TemperatureUnit.Fahrenheit, TemperatureUnit.Kelvin };

            for (int i = 0; i < tempUnits.Length; i++)
            {
                for (int j = factor; j < 1000; j += 10)
                {
                    double expected = j - factor;
                    Temperature temperature = new Temperature(j, tempUnits[i]);
                    temperature = temperature - factor;

                    Assert.AreEqual(expected, temperature.Value, "(double) " + j + " " + tempUnits[i]);

                    Temperature expectedTemperature = new Temperature(j - factor, tempUnits[i]);
                    temperature = new Temperature(j, tempUnits[i]);
                    var temperatureFactor = new Temperature(factor, tempUnits[i]);
                    temperature = temperature - temperatureFactor;

                    Assert.AreEqual(expected, temperature.Value, "(Temperature) " + j + " " + tempUnits[i]);
                }
            }
        }

        [TestMethod]
        [TestCategory("Temperature")]
        public void TemperatureMultiplicationTest()
        {
            const double factor = 15.0d;
            TemperatureUnit[] tempUnits = { TemperatureUnit.Celsius, TemperatureUnit.Fahrenheit, TemperatureUnit.Kelvin };

            for (int i = 0; i < tempUnits.Length; i++)
            {
                for (int j = 0; j < 1000; j += 10)
                {
                    double expected = j * factor;
                    Temperature temperature = new Temperature(j, tempUnits[i]);
                    temperature = temperature * factor;

                    Assert.AreEqual(expected, temperature.Value, "(double) " + j + " " + tempUnits[i]);

                    Temperature expectedTemperature = new Temperature(j * factor, tempUnits[i]);
                    temperature = new Temperature(j, tempUnits[i]);
                    var temperatureFactor = new Temperature(factor, tempUnits[i]);
                    temperature = temperature * temperatureFactor;

                    Assert.AreEqual(expected, temperature.Value, "(Temperature) " + j + " " + tempUnits[i]);
                }
            }
        }

        [TestMethod]
        [TestCategory("Temperature")]
        public void TemperatureDivisionTest()
        {
            const double factor = 15.0;
            TemperatureUnit[] tempUnits = { TemperatureUnit.Celsius, TemperatureUnit.Fahrenheit, TemperatureUnit.Kelvin };

            for (int i = 0; i < tempUnits.Length; i++)
            {
                for (int j = 0; j < 1000; j += 10)
                {
                    double expected = Math.Round(j / factor, 3);
                    Temperature temperature = new Temperature(j, tempUnits[i]);
                    temperature = temperature / factor;

                    Assert.AreEqual(expected, Math.Round(temperature.Value, 3), "(double) " + j + " " + tempUnits[i]);

                    Temperature expectedTemperature = new Temperature(j * factor, tempUnits[i]);
                    temperature = new Temperature(j, tempUnits[i]);
                    var temperatureFactor = new Temperature(factor, tempUnits[i]);
                    temperature = temperature / temperatureFactor;

                    Assert.AreEqual(expected, Math.Round(temperature.Value,3), "(Temperature) " + j + " " + tempUnits[i]);
                }

                try
                {
                    Temperature temperature2 = new Temperature(10, tempUnits[i]);
                    temperature2 = temperature2 / 0;
                    Assert.Fail("Divide by zero (double) " + tempUnits[i]);
                }
                catch (DivideByZeroException)
                { }

                try
                {
                    Temperature temperature2 = new Temperature(10, tempUnits[i]);
                    var temperature3 = new Temperature(0, tempUnits[i]);
                    temperature2 = temperature2 / temperature3;
                    Assert.Fail("Divide by zero (temperature) " + tempUnits[i]);
                }
                catch (DivideByZeroException)
                { }
            }
        }
    }
}
