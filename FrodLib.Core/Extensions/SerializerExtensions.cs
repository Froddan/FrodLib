using FrodLib.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FrodLib.Extensions
{
    public static class SerializerExtensions
    {

        /// <summary>Serializes an object of type T in to an xml string</summary>
        /// <typeparam name="T">Any class type</typeparam>
        /// <param name="obj">Object to serialize</param>
        /// <returns>A string that represents Xml, empty otherwise</returns>
        public static string XmlSerialize<T>(this T obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Serializes the object into a stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        public static void XmlSerialize<T>(this T obj, Stream stream)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stream, obj);
        }

        /// <summary>Deserializes an stream in to an object of Type T</summary>
        /// <typeparam name="T">Any class type</typeparam>
        /// <param name="stream">the stream containing the serialized data</param>
        /// <returns>A new object of type T is successful, null if failed</returns>
        public static T XmlDeserialize<T>(this Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stream);
        }

        /// <summary>Deserializes an xml string in to an object of Type T</summary>
        /// <typeparam name="T">Any class type</typeparam>
        /// <param name="xml">Xml as string to deserialize from</param>
        /// <returns>A new object of type T is successful, null if failed</returns>
        public static T XmlDeserialize<T>(this string xml)
        {
            if (xml == null) throw new ArgumentNullException("xml");

            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Deserializes an xml string to the specified type
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="type">The type to be deseialized into</param>
        /// <returns></returns>
        public static object XmlDeserialize(this string xml, Type type)
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new ArgumentNullException(StringResources.XMLStringCannotBeNull);
            }

            var serializer = new XmlSerializer(type);
            using (var reader = new StringReader(xml))
            {
                return serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Deserializes an xml string to the specified type
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="type">The type to be deseialized into</param>
        /// <returns></returns>
        public static object XmlDeserialize(this Stream stream, Type type)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var serializer = new XmlSerializer(type);
            return serializer.Deserialize(stream);
        }

        public static bool TryXmlDeserialize<T>(Stream stream, out T result)
        {
            bool success;
            try
            {
                result = XmlDeserialize<T>(stream);
                success = true;
            }
            catch (Exception)
            {
                result = default(T);
                success = false;
            }
            return success;
        }

        public static bool TryXmlDeserialize<T>(this string xml, out T result)
        {
            bool success;
            try
            {
                result = XmlDeserialize<T>(xml);
                success = true;
            }
            catch (InvalidOperationException)
            {
                result = default(T);
                success = false;
            }
            return success;
        }
    }
}
