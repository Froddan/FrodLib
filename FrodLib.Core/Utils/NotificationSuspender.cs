using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrodLib.Extensions;

namespace FrodLib.Utils
{
    public sealed class NotificationSuspender : IDisposable
    {
        private INotificationSuspendable m_suspendableNotification;

        public NotificationSuspender(INotificationSuspendable suspendableNotification)
        {
            ArgumentValidator.IsNotNull(suspendableNotification, nameof(suspendableNotification));

            m_suspendableNotification = suspendableNotification;
            m_suspendableNotification.SuspendNotification();
        }

        public void Dispose()
        {
            m_suspendableNotification.ResumeNotification();
        }
    }
}
