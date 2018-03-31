using System;
using System.Threading;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;

namespace Slink
{
    public abstract class BaseQueue : IDisposable
    {
        protected bool Running, ShouldStop;
        protected CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        public BaseQueue()
        {
            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
        }

        public bool IsRunning() { return Running; }

        public virtual void Current_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }


        public abstract void Start();
        public virtual void Stop()
        {
            ShouldStop = true;
        }
        public virtual void Reset()
        {
            Running = false;
            ShouldStop = false;

            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = null;

            CancellationTokenSource = new CancellationTokenSource();

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
            {
                CancellationTokenSource?.Cancel();
                CancellationTokenSource?.Dispose();
                CancellationTokenSource = null;

                CrossConnectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;
            }
            disposedValue = true;
        }


        ~BaseQueue()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Stop();
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
