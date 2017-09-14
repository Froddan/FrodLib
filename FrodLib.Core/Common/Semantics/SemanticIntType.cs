using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FrodLib.Semantics
{
    /// <summary>
    /// When deriving from this type, pass in the derived type in parameter Q.
    /// If you don't do that, then the static elements inside this class will be 
    /// shared by all object of all classes derived from this base class.
    /// </summary>
    /// <typeparam name="Q"></typeparam>
    public abstract class SemanticIntType<Q> : ComparableSemanticType<double> where Q : class
    {

        protected SemanticIntType(double value)
            : base(null, value)
        {
        }

        // -----------------------------------------------------------------
        // Binary operator 

        public static Q operator +(SemanticIntType<Q> b, SemanticIntType<Q> c)
        {
            if (EitherNull(b, c)) { return null; }
            return CreateQ(b.Value + c.Value);
        }

        public static Q operator -(SemanticIntType<Q> b, SemanticIntType<Q> c)
        {
            if (EitherNull(b, c)) { return null; }
            return CreateQ(b.Value - c.Value);
        }

        public static Q operator *(double b, SemanticIntType<Q> c)
        {
            if (c == null) { return null; }
            return CreateQ(b * c.Value);
        }

        public static Q operator *(SemanticIntType<Q> c, double b)
        {
            if (c == null) { return null; }
            return CreateQ(b * c.Value);
        }

        public static Q operator /(SemanticIntType<Q> c, double b)
        {
            if (c == null) { return null; }
            return CreateQ(c.Value / b);
        }

        // -----------------------------------------------------------------
        // Unary operator 

        public static Q operator -(SemanticIntType<Q> c)
        {
            if (c == null) { return null; }
            return CreateQ(-1 * c.Value);
        }

        private static Q CreateQ(double value)
        {
            object[] args = { value };
            object result = Activator.CreateInstance(typeof(Q), args);
            return (Q)result;
        }

        protected override void _readXml(XmlReader reader)
        {
            reader.MoveToContent();
            bool isEmptyElement = reader.IsEmptyElement; // (1)
            reader.ReadStartElement();
            if (!isEmptyElement) // (1)
            {
                if (reader.Name.Equals("Value", StringComparison.CurrentCultureIgnoreCase))
                {
                    Value = int.Parse(reader.Value);
                }
                reader.ReadEndElement();
            }
        }

        protected override void _writeXml(XmlWriter writer)
        {
            writer.WriteElementString("Value",
                Value.ToString());
        }

    }
}
