using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FrodLib.Utils
{
    public class ActionScheduler
    {
        private readonly object m_lockObject = new object();
        private List<ScheduledItemHolder> m_scheduledItems = new List<ScheduledItemHolder>();
        private Timer m_timer;

        public ActionScheduler()
        {
            m_timer = new Timer(TimerTick, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// When the next tick will occure
        /// <para>It will return null if no actions are scheduled</para>
        /// </summary>
        public DateTime? NextScheduledActionTime
        {
            get
            {
                lock (m_lockObject)
                {
                    if (NumberOfScheduledActions <= 0)
                    {
                        return null;
                    }
                    return m_scheduledItems.First().Time;
                }
            }
        }

        /// <summary>
        /// The number of actions that are scheduled to run
        /// </summary>
        public int NumberOfScheduledActions
        {
            get
            {
                return m_scheduledItems.Count;
            }
        }

        /// <summary>
        /// Schedules an action to run once at a specific time
        /// </summary>
        /// <param name="action">The action to be runned at the specifed the time</param>
        /// <param name="time">The time the action should run</param>
        public void AddScheduledAction(Action action, TimeSpan time)
        {
            AddScheduledAction(action, time, false, false);
        }

        /// <summary>
        /// Schedules an action to run at a specific time
        /// </summary>
        /// <param name="action">The action to be runned at the specifed the time</param>
        /// <param name="time">The time the action should run</param>
        /// <param name="isRepeating">Set true if the action should run every day at the specified time</param>
        public void AddScheduledAction(Action action, TimeSpan time, bool isRepeating)
        {
            AddScheduledAction(action, time, isRepeating, false);
        }

        /// <summary>
        /// Schedules a task to run at a specific date and time
        /// <para>If that date and/or time has passed it will be runned directly</para>
        /// </summary>
        /// <param name="action">The action to be runned at the specifed date and time</param>
        /// <param name="time">The date and time the action should run</param>
        public void AddScheduledAction(Action action, DateTime time)
        {
            lock (m_lockObject)
            {
                m_scheduledItems.Add(new ScheduledItemHolder(action, time, false));
                UpdateNextTimePeriod();
            }
        }

        /// <summary>
        /// Removed all scheduled tasks
        /// </summary>
        public void ClearAllScheduledActions()
        {
            lock (m_lockObject)
            {
                m_scheduledItems.Clear();
                m_timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void AddScheduledAction(Action action, TimeSpan time, bool isRepeating, bool addDay)
        {
            DateTime now = Clock.Now;

            int days = 0;
            if (addDay || time < now.TimeOfDay)
            {
                days++;
            }

            now = now.Date.AddDays(days).Add(time);

            lock (m_lockObject)
            {
                m_scheduledItems.Add(new ScheduledItemHolder(action, now, isRepeating));
                UpdateNextTimePeriod();
            }
        }

        private void TimerTick(object state)
        {
            ScheduledItemHolder item;
            lock (m_lockObject)
            {
                item = m_scheduledItems[0];
                m_scheduledItems.RemoveAt(0);

                if (item.IsRepeating)
                {
                    AddScheduledAction(item.Action, item.Time.TimeOfDay, true, true);
                }
                else
                {
                    UpdateNextTimePeriod();
                }
            }

            Task.Factory.StartNew(item.Action);
        }

        private void UpdateNextTimePeriod()
        {
            m_scheduledItems.Sort();
            if (NumberOfScheduledActions <= 0)
            {
                m_timer.Change(Timeout.Infinite, Timeout.Infinite);
                return;
            }

            DateTime timeOfFirstItem = m_scheduledItems.First().Time;
            TimeSpan timeSpan = (timeOfFirstItem - Clock.Now);

            if (timeSpan.TotalMilliseconds < 0)
            {
                timeSpan = TimeSpan.Zero;
                m_timer.Change(0, Timeout.Infinite);
            }
            else
            {
                m_timer.Change(timeSpan, new TimeSpan(0, 0, 0, 0, -1));
            }
        }

        private class ScheduledItemHolder : IComparable<ScheduledItemHolder>
        {
            public ScheduledItemHolder(System.Action action, DateTime time, bool isRepeating)
            {
                this.Action = action;
                this.Time = time;
                this.IsRepeating = isRepeating;
            }

            public Action Action { get; private set; }

            public bool IsRepeating { get; private set; }

            public DateTime Time { get; private set; }

            public int CompareTo(ScheduledItemHolder other)
            {
                if (other == null) return 0;
                return Time.CompareTo(other.Time);
            }
        }
    }
}