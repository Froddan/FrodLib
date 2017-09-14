using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrodLib.Utils
{
    internal delegate void TimerCallback(object state);

    class Timer : IDisposable
    {
        private TimerCallback m_callback;
        private object m_state;
        private CancellationTokenSource m_cancellationSource;

        internal Timer(TimerCallback callback, object state, int dueTime, int period)
        {
            m_callback = callback;
            m_state = state;
            Change(dueTime, period);
        }

        public void Change(TimeSpan dueTIme, TimeSpan period)
        {
            Change(dueTIme.Milliseconds, period.Milliseconds);
        }

        public void Change(int dueTime, int period)
        {
            if (dueTime == Timeout.Infinite && period == Timeout.Infinite)
            {
                if (m_cancellationSource != null) m_cancellationSource.Cancel();
                return;
            }
            else
            {
                if (m_cancellationSource != null)
                {
                    m_cancellationSource.Cancel();
                    m_cancellationSource.Dispose();
                }
                m_cancellationSource = new CancellationTokenSource();

                Task.Delay(dueTime, m_cancellationSource.Token).ContinueWith(async (t, s) =>
                {
                    var tuple = (Tuple<TimerCallback, object, CancellationTokenSource>)s;

                    while (!tuple.Item3.IsCancellationRequested)
                    {
                        tuple.Item1(tuple.Item2);
                        await Task.Delay(period, tuple.Item3.Token).ConfigureAwait(false);
                    }

                }, Tuple.Create(m_callback, m_state, m_cancellationSource), CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
            }
        }

        public void Dispose()
        {
            if (m_cancellationSource != null)
            {
                m_cancellationSource.Cancel();
                m_cancellationSource.Dispose();
            }
        }
    }
}
