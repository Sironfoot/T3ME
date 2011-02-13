using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Timers;

namespace MvcLibrary.Utils
{
    public abstract class Scheduler<T> where T : Scheduler<T>
    {
        private static volatile T _instance = null;
        private static readonly object _syncObject = new object();

        public static T Instance()
        {
            if (_instance == null)
            {
                lock (_syncObject)
                {
                    if (_instance == null)
                    {
                        Type teesType = typeof(T);

                        try
                        {
                            _instance = (T)teesType.InvokeMember(teesType.Name,
                                BindingFlags.CreateInstance | BindingFlags.Instance |
                                BindingFlags.NonPublic, null, null, null,
                                CultureInfo.InvariantCulture);
                        }
                        catch (MissingMethodException)
                        {
                            string message = teesType.FullName + " must use either a " +
                                "private or protected constructor to be a Singleton.";
                            throw new TypeLoadException(message);
                        }
                    }
                }
            }

            return _instance;
        }

        protected abstract void Execute(DateTime runTime);

        protected abstract void Error(Exception ex, DateTime exceptionTime);

        private TimeSpan _interval = TimeSpan.FromMinutes(10.0);

        public TimeSpan Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        private bool _timerRunning = false;

        private Timer Timer = null;

        public void Start()
        {
            if (Timer == null || !_timerRunning)
            {
                lock (this)
                {
                    if (Timer == null)
                    {
                        Timer = new Timer(1);
                        Timer.AutoReset = false;
                        Timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
                    }

                    if (!_timerRunning)
                    {
                        Timer.Start();
                        _timerRunning = true;
                    }
                }
            }
        }

        public void Stop()
        {
            if (Timer != null && _timerRunning)
            {
                lock (this)
                {
                    if (Timer != null && _timerRunning)
                    {

                    }
                }
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Timer.Stop();

            try
            {
                Execute(e.SignalTime);
            }
            catch (Exception ex)
            {
                try
                {
                    Error(ex, e.SignalTime);
                }
                catch
                {
                    Timer.Stop();
                }
            }

            Timer.Interval = _interval.TotalMilliseconds;
            Timer.Start();
        }
    }
}