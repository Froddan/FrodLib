using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq.Expressions;
using FrodLib.Utils;

namespace TestProject.TestDataClasses
{
    [DebuggerDisplay("Name = {Name}, Age = {Age}, ShoeSize = {ShoeSize}, Address = {Address}")]
    internal class Person : NotificationSuspendableBase, IComparable<Person>, IEquatable<Person>, INotifyPropertyChanged, INotificationSuspendable
    {
        private int m_shoeSize;

        public int ShoeSize
        {
            get { return m_shoeSize; }
            set
            {
                m_shoeSize = value;
                RaisePropertyChanged(() => ShoeSize);
            }
        }

        private int m_age;

        public int Age
        {
            get { return m_age; }
            set
            {
                m_age = value;
                RaisePropertyChanged(() => Age);
            }
        }


        private string m_name;

        public string Name
        {
            get { return m_name; }
            set
            {
                m_name = value;
                RaisePropertyChanged(() => Name);
            }
        }


        private Address m_address;

        public Address Address
        {
            get { return m_address; }
            set
            {
                m_address = value;
                RaisePropertyChanged(() => Address);
            }
        }


        public Person()
        {

        }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public Person(Address address)
        {
            Address = address;
        }

        public int CompareTo(Person other)
        {
            int nameDiff = this.Name.CompareTo(other.Name);
            if (nameDiff == 0)
            {
                return this.Age.CompareTo(other.Age);
            }
            else
            {
                return nameDiff;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Person);
        }

        public override string ToString()
        {
            return string.Format("Name = {0}, Age = {1}, ShoeSize = {2}, Address = {3}", Name, Age, ShoeSize, Address);
        }

        public bool Equals(Person other)
        {
            if (other == null)
            {
                return false;
            }
            if (object.ReferenceEquals(this, other)) return true;
            else
            {
                return this.Name == other.Name && this.Age == other.Age && this.ShoeSize == other.ShoeSize;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
