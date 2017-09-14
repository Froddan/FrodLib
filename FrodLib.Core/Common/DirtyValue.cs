using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib
{
    /// <summary>
    /// A value holder that holds an orginal value and marks the value as "dirty" if it has changed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct DirtyValue<T> : IEquatable<T>, IEquatable<DirtyValue<T>>
    {
        private T _orginalValue;
        private T _currentValue;

        private bool _isOrginalValueSet;

        public DirtyValue(T orginalValue)
            : this()
        {
            SetOrginalValue(orginalValue);
            _currentValue = _orginalValue;
        }

        public T OrginalValue
        {
            get
            {
                if (_isOrginalValueSet)
                {
                    return _orginalValue;
                }
                return default(T);
            }
        }

        /// <summary>
        /// Gets or sets the current value
        /// <para>If no value have been set then the value will also be set as the orginal value</para>
        /// </summary>
        public T Value
        {
            get
            {
                return _currentValue;
            }
            set
            {
                SetOrginalValue(value);
                _currentValue = value;
            }
        }

        public bool IsDirty
        {
            get
            {
                if (_isOrginalValueSet)
                {
                    if (_orginalValue == null && _currentValue == null)
                    {
                        return false;
                    }
                    else if (_orginalValue == null && _currentValue != null)
                    {
                        return true;
                    }
                    else if (_orginalValue != null && _currentValue == null)
                    {
                        return true;
                    }
                    else
                    {
                        return !_orginalValue.Equals(Value);
                    }
                }
                else
                {
                    return false;
                }
            }
        }


        private void SetOrginalValue(T value)
        {
            if (_isOrginalValueSet == false)
            {
                _orginalValue = value;
                _isOrginalValueSet = true;
            }
        }

        /// <summary>
        /// Rollbacks any changed value to to orginal
        /// </summary>
        public void Rollback()
        {
            if (_isOrginalValueSet)
            {
                _currentValue = _orginalValue;
            }
        }

        /// <summary>
        /// Commits the value and marks the current value as the orginal
        /// <para>All future checks if the value is dirty will now use the value that was current by the time of the call to this method</para>
        /// </summary>
        public void Commit()
        {
            if (_isOrginalValueSet)
            {
                _orginalValue = _currentValue;
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 27;
                if (_currentValue != null)
                {
                    hash = (hash * 269) ^ this._currentValue.GetHashCode();
                }
                return hash;
            }
        }

        public override string ToString()
        {
            if (_currentValue == null)
            {
                return string.Empty;
            }
            return _currentValue.ToString();
        }

        #region Equality Comparisson

        public static bool operator ==(DirtyValue<T> thisValue, DirtyValue<T> otherValue)
        {
            if (thisValue == null)
            {
                return false;
            }
            return thisValue.Equals(otherValue);
        }

        public static bool operator ==(DirtyValue<T> thisValue, T otherValue)
        {
            if (thisValue == null || thisValue.Value == null)
            {
                return false;
            }
            return thisValue.Value.Equals(otherValue);
        }

        public static bool operator ==(T thisValue, DirtyValue<T> otherValue)
        {
            if (thisValue == null)
            {
                return false;
            }
            return thisValue.Equals(otherValue._currentValue);
        }

        public static bool operator !=(DirtyValue<T> thisValue, DirtyValue<T> otherValue)
        {
            return !object.Equals(thisValue, otherValue);
        }

        public static bool operator !=(DirtyValue<T> thisValue, T otherValue)
        {
            if (thisValue.Value == null)
            {
                return false;
            }
            return !thisValue.Value.Equals(otherValue);
        }

        public static bool operator !=(T thisValue, DirtyValue<T> otherValue)
        {
            if (thisValue == null)
            {
                return false;
            }
            return !object.Equals(thisValue, otherValue._currentValue);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            else if (object.ReferenceEquals(this, obj)) return true;
            else if (obj is T)
            {
                return Equals((T)obj);
            }
            else if (obj is DirtyValue<T>)
            {
                return Equals((DirtyValue<T>)obj);
            }
            else
            {
                return false;
            }
        }


        public bool Equals(T other)
        {
            if (other == null) return false;
            else if (_currentValue != null)
            {
                return _currentValue.Equals(other);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(DirtyValue<T> other)
        {
            if (_currentValue != null)
            {
                return _currentValue.Equals(other._currentValue);
            }
            else
            {
                return false;
            }
        }

        #endregion Equality Comperisson

        public static implicit operator DirtyValue<T>(T value)
        {
            return new DirtyValue<T>(value);
        }

        public static explicit operator T(DirtyValue<T> dValue)
        {
            return dValue.Value;
        }
    }
}
