using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiniMetrics
{
    public interface IMetricsClient : IDisposable
    {
        void Send(string message);
    }

    public class NullMetricsClient : IMetricsClient
    {
        public void Dispose()
        {
        }

        public void Send(string message)
        {
        }
    }

    public class TcpMetricsClient : IMetricsClient
    {
        private readonly IPEndPoint _endpoint;
        private readonly BlockingCollection<string> _messages = new BlockingCollection<string>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private bool _disposed;
        private TcpClient _tcpClient;

        public TcpMetricsClient(IPAddress address, int port)
        {
            _endpoint = new IPEndPoint(address, port);

            StartProcessing();
        }

        public static TcpMetricsClient CreateFrom(string hostname, int port)
        {
            var ip = LookupIpAddress(hostname);

            return new TcpMetricsClient(ip, port);
        }

        public void Send(string message)
        {
            if (message == null)
            {
                return;
            }

            _messages.Add(message, _cts.Token);
        }

        ~TcpMetricsClient()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                try
                {
                    _cts.Cancel();

                    if (_tcpClient != null)
                    {
                        _tcpClient.GetStream().Close();
                        _tcpClient.Close();
                    }
                }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                {
                }

                _disposed = true;
            }
        }

        private void BuildClient()
        {
            _tcpClient = new TcpClient();
            _tcpClient.ExclusiveAddressUse = false;
        }

        private void EnsureClientIsConnected()
        {
            if (_tcpClient.Connected)
            {
                return;
            }

            try
            {
                _tcpClient.Connect(_endpoint);
            }
            catch (ObjectDisposedException)
            {
                BuildClient();
                _tcpClient.Connect(_endpoint);
            }
        }

        private void BackgroundWork()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                string message = _messages.Take(_cts.Token);

                var bytes = Encoding.UTF8.GetBytes(message);

                EnsureClientIsConnected();

                try
                {
                    _tcpClient.GetStream().Write(bytes, 0, bytes.Length);
                }
                catch (IOException exception)
                {
                    //TODO LOG
                }
                catch (ObjectDisposedException exception)
                {
                    //TODO LOG
                }
            }
        }

        private void StartProcessing()
        {
            BuildClient();

            Task.Factory.StartNew(BackgroundWork, TaskCreationOptions.LongRunning).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    //TODO LOG
                }
            },
            TaskContinuationOptions.OnlyOnFaulted);
        }

        private static IPAddress LookupIpAddress(string hostname)
        {
            if (string.IsNullOrEmpty(hostname))
            {
                throw new ArgumentException("Specified value is no valid hostname.", nameof(hostname));
            }

            IPHostEntry host = Dns.GetHostEntry(hostname);

            if (host.AddressList.Length == 0)
            {
                throw new InvalidOperationException("Unable to find an ip address for specified hostname.");
            }

            return host.AddressList[0];
        }
    }
}