using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FrodLib.Semantics
{
    public interface ISemanticType
    {

    }

    public interface ISemanticType<T> : ISemanticType
    {
        T Value { get; }
    }

    public abstract class SemanticType<T> : ISemanticType<T>, IEquatable<ISemanticType<T>>, IXmlSerializable
    {

        public virtual T Value { get; protected set; }

        protected SemanticType(Func<T, bool> isValidLambda, T value)
        {
            if ((Object)value == null)
            {
                throw new ArgumentException(string.Format("Trying to use null as the value of a {0}", this.GetType()));
            }

            if ((isValidLambda != null) && !isValidLambda(value))
            {
                throw new ArgumentException(string.Format("Trying to set a {0} to {1} which is invalid", this.GetType(), value));
            }

            Value = value;
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types. 
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            return (Value.Equals(((ISemanticType<T>)obj).Value));
        }

        public bool Equals(ISemanticType<T> other)
        {
            if (other == null) { return false; }

            return (Value.Equals(other.Value));
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(SemanticType<T> a, ISemanticType<T> b)
        {
            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            // Have to cast to object, otherwise you recursively call this == operator.
            if (EitherNull(a,b))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static bool operator !=(SemanticType<T> a, ISemanticType<T> b)
        {
            return !(a == b);
        }

        public static explicit operator T(SemanticType<T> semType)
        {
            return semType.Value;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }


        protected static bool EitherNull(ISemanticType<T> a, ISemanticType<T> b)
        {
            if (((object)a == null) || ((object)b == null))
            {
                return true;
            }

            return false;
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            _readXml(reader);
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            _writeXml(writer);
        }

        protected virtual void _readXml(XmlReader reader)
        {
            reader.MoveToContent();
            bool isEmptyElement = reader.IsEmptyElement; // (1)
            reader.ReadStartElement();
            if (!isEmptyElement) // (1)
            {
                Value = (T)Convert.ChangeType(reader.Value, typeof(T), null);
                reader.ReadEndElement();
            }
        }

        protected virtual void _writeXml(XmlWriter writer)
        {
            writer.WriteElementString("Value",
                Value.ToString());
        }

    }

    public abstract class ComparableSemanticType<T> : SemanticType<T>, IComparable<ISemanticType<T>> where T : IComparable<T>
    {
        protected ComparableSemanticType(Func<T, bool> isValidLambda, T value): base(isValidLambda, value)
        {

        }

        public int CompareTo(ISemanticType<T> other)
        {
            if (other == null) { return 1; }
            return this.Value.CompareTo(other.Value);
        }

        public static bool operator <=(ComparableSemanticType<T> semType1, ISemanticType<T> semType2)
        {
            if (EitherNull(semType1, semType2)) return false;
            return semType1.Value.CompareTo(semType2.Value) <= 0;
        }

        public static bool operator >=(ComparableSemanticType<T> semType1, ISemanticType<T> semType2)
        {
            if (EitherNull(semType1, semType2)) return false;
            return semType1.Value.CompareTo(semType2.Value) >= 0;
        }

        public static bool operator <(ComparableSemanticType<T> semType1, ISemanticType<T> semType2)
        {
            if (EitherNull(semType1, semType2)) return false;
            return semType1.Value.CompareTo(semType2.Value) <= 0;
        }

        public static bool operator >(ComparableSemanticType<T> semType1, ISemanticType<T> semType2)
        {
            if (EitherNull(semType1, semType2)) return false;
            return semType1.Value.CompareTo(semType2.Value) >= 0;
        }

        public static bool operator <=(ISemanticType<T> semType1, ComparableSemanticType<T> semType2)
        {
            if (EitherNull(semType1, semType2)) return false;
            return semType1.Value.CompareTo(semType2.Value) <= 0;
        }

        public static bool operator >=(ISemanticType<T> semType1, ComparableSemanticType<T> semType2)
        {
            if (EitherNull(semType1, semType2)) return false;
            return semType1.Value.CompareTo(semType2.Value) >= 0;
        }

        public static bool operator <(ISemanticType<T> semType1, ComparableSemanticType<T> semType2)
        {
            if (EitherNull(semType1, semType2)) return false;
            return semType1.Value.CompareTo(semType2.Value) <= 0;
        }

        public static bool operator >(ISemanticType<T> semType1, ComparableSemanticType<T> semType2)
        {
            if (EitherNull(semType1, semType2)) return false;
            return semType1.Value.CompareTo(semType2.Value) >= 0;
        }

    }
}
