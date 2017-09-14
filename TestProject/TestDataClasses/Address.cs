using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TestProject.TestDataClasses
{
    [DebuggerDisplay("ZipCode = {ZipCode}, Street = {Street}, City = {City}")]
    internal class Address : IEquatable<Address>
    {
        [Copy]
        public int ZipCode { get; set; }
        [Copy]
        public string Street { get; set; }
        public string City { get; set; }

        public Address()
        {

        }

        public Address(int zip, string street, string city)
        {
            
            ZipCode = zip;
            Street = street;
            City = city;
        }

        public override bool Equals(object other)
        {
            return Equals(other as Address);
        }

        public bool Equals(Address other)
        {
            if (other != null)
            {
                return false;
            }
            if (object.ReferenceEquals(this, other)) return true;
            else
            {
                return this.ZipCode == other.ZipCode && this.Street == other.Street && this.City == other.City;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    internal class CopyAttribute : Attribute
    {
        
    }
}
