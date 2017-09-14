using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.Utils
{
    public interface INotificationSuspendable
    {
        void SuspendNotification();
        void ResumeNotification();
    }
}
