using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FrodLib.Utils
{

    /// <summary>
    /// A base class for suspending notifications on property changed.
    /// <para>When suspended all notifications will be queued up and sent once the notifications are released</para>
    /// </summary>
    public abstract class NotificationSuspendableBase : NotificationBase, INotificationSuspendable
    {
        public bool NotificationSuspended { get; private set; }
        private IList<string> suspendedNotifications = new List<string>();

        public bool QueueNotificationsWhileSuspended { get; set; }

        public NotificationSuspendableBase()
        {
            QueueNotificationsWhileSuspended = true;
        }

        void INotificationSuspendable.SuspendNotification()
        {
            NotificationSuspended = true;
        }

        void INotificationSuspendable.ResumeNotification()
        {
            OnResumeNotification();
            NotificationSuspended = false;

            foreach (var suspendedNotification in suspendedNotifications.Distinct())
            {
                base.RaisePropertyChanged(suspendedNotification);
            }

            suspendedNotifications.Clear();
        }

        protected virtual void OnResumeNotification()
        {

        }

        protected sealed override void RaisePropertyChanged(string propertyName)
        {
            if (NotificationSuspended)
            {
                if (QueueNotificationsWhileSuspended)
                {
                    suspendedNotifications.Add(propertyName);
                }
            }
            else
            {
                base.RaisePropertyChanged(propertyName);
            }
        }
    }
}
