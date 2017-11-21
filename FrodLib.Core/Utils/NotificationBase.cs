using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using FrodLib.Utils;
using FrodLib.Resources;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace FrodLib.Utils
{
    /// <summary>
    /// A base for view models that implements INotifyPropertyChanged with a type safe way to raise the property changed event
    /// </summary>
    [DataContract]
    public abstract class NotificationBase : INotifyPropertyChanged
    {
        [IgnoreDataMember]
        private PropertyChangedManager<NotificationBase> propertyChangedManager;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { CheckCreatePropertyChangedManager(ref propertyChangedManager).AddHandler(value); }
            remove { CheckCreatePropertyChangedManager(ref propertyChangedManager).RemoveHandler(value); }
        }

        protected NotificationBase()
        {
            CheckCreatePropertyChangedManager(ref propertyChangedManager);
        }

        protected PropertyChangedManager<NotificationBase> CheckCreatePropertyChangedManager(ref PropertyChangedManager<NotificationBase> manager)
        {
            if (manager == null)
            {
                manager = propertyChangedManager = new PropertyChangedManager<NotificationBase>(this);
            }
            return manager;
        }

        [IgnoreDataMember]
        private Lazy<Dictionary<string, bool>> m_isUpdatingProperty = new Lazy<Dictionary<string, bool>>();

        protected void RaisePropertyChanged([CallerMemberName]string propertyName = null, params string[] propertyNames)
        {
            RaisePropertyChanged(propertyName);
            for (int i = 0; i < propertyNames.Length; i++)
            {
                RaisePropertyChanged(propertyNames[i]);
            }
        }

        protected virtual void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (propertyName == null) return;
            ValidatePropertyName(propertyName);

            CheckCreatePropertyChangedManager(ref propertyChangedManager).NotifyChanged(propertyName);
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> property)
        {
            RaisePropertyChanged(PropertyNameHelper.GetPropertyName(property));
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> property, params Expression<Func<T>>[] properties)
        {
            RaisePropertyChanged(PropertyNameHelper.GetPropertyName(property));
            for (int i = 0; i < properties.Length; i++)
            {
                RaisePropertyChanged(properties[i]);
            }
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void ValidatePropertyName(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) return;
            var type = this.GetType();
            var pInfo = type.GetTypeInfo().GetDeclaredProperty(propertyName);
            if (pInfo == null)
            {
                throw new InvalidOperationException(string.Format(StringResources.PropertyNotExist, propertyName, type));
            }
        }

        public void RaisePropertyChanged<T>(Expression<Func<T, object>> property)
        {
            string propertyName = PropertyNameHelper.GetPropertyNameFrom(property);
            RaisePropertyChanged(propertyName);
        }

        private bool EnterPropertyNameUpdateLock(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) return true; // No lock required as no prop changed will be raised
            if (m_isUpdatingProperty == null) m_isUpdatingProperty = new Lazy<Dictionary<string, bool>>();
            var updatingPropHolder = m_isUpdatingProperty.Value;
            lock (updatingPropHolder)
            {
                bool isLockHeld;
                if (updatingPropHolder.TryGetValue(propertyName, out isLockHeld))
                {
                    if (isLockHeld)
                    {
                        // we are already updating this property
                        return false;
                    }
                    else
                    {
                        updatingPropHolder[propertyName] = true;
                        return true; // lock aquired
                    }
                }
                else
                {
                    updatingPropHolder.Add(propertyName, true);
                    return true; // lock aquired
                }
            }
        }

        private void LeavePropertyNameUpdateLock(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName) || m_isUpdatingProperty == null) return;

            var updatingPropHolder = m_isUpdatingProperty.Value;
            lock (updatingPropHolder)
            {
                if (updatingPropHolder.ContainsKey(propertyName))
                {
                    updatingPropHolder[propertyName] = false;
                }
            }
        }

        private PropertyInfo GetPropertyInfoFromExpression<T>(Expression<Func<T, object>> exp)
        {
            var unaryExp = exp.Body as UnaryExpression;
            if (unaryExp != null)
            {
                var memExp = unaryExp.Operand as MemberExpression;
                if (memExp != null)
                {
                    return memExp.Member as PropertyInfo;
                }
            }
            else
            {
                var memExp = exp.Body as MemberExpression;
                if (memExp != null)
                {
                    return memExp.Member as PropertyInfo;
                }
            }
            return null;
        }

        public bool SetValue<T, K>(Expression<Func<T, object>> property, K value)
        {
            var propInfo = GetPropertyInfoFromExpression(property);
            if (propInfo != null)
            {
                var currValue = propInfo.GetValue(this, null);
                if (object.Equals(currValue, value)) return false;
                propInfo.SetValue(this, value, null);
                return true;
            }
            return false;
        }

        protected bool SetValue<T, K>(ref K valueHolder, K value, Expression<Func<T>> property = null)
        {
            var propName = PropertyNameHelper.GetPropertyName(property) ?? string.Empty;
            return SetValue(ref valueHolder, value, propName);
        }

        protected virtual bool SetValue<T>(ref T valueHolder, T value, [CallerMemberName] string propertyName = null)
        {


            if (EnterPropertyNameUpdateLock(propertyName))
            {
                try
                {
                    if (object.Equals(valueHolder, value)) return false;
                    valueHolder = value;
                    if (string.IsNullOrEmpty(propertyName)) return true;
                    RaisePropertyChanged(propertyName);
                }
                finally
                {
                    LeavePropertyNameUpdateLock(propertyName);
                }
            }
            else
            {
                //We are already updating this Property, No recursion allowed
                return false;
            }

            return true;
        }

        protected virtual bool SetValue<T>(ref DirtyValue<T> valueHolder, T value, [CallerMemberName] string propertyName = null)
        {

            if (EnterPropertyNameUpdateLock(propertyName))
            {
                try
                {
                    if (object.Equals(valueHolder.Value, value)) return false;
                    valueHolder.Value = value;
                    if (string.IsNullOrEmpty(propertyName)) return true;
                    ValidatePropertyName(propertyName);
                    RaisePropertyChanged(propertyName);
                }
                finally
                {
                    LeavePropertyNameUpdateLock(propertyName);
                }
            }
            else
            {
                //We are already updating this Property, No recursion allowed
                return false;
            }

            return true;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    propertyChangedManager.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~NotificationBase() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public virtual void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}