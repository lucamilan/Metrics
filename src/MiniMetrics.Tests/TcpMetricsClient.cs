using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MiniMetrics.Tests
{
    public class TcpMetricsClient : IDisposable
    {
        private readonly TcpListener _listener;

        public TcpMetricsClient()
        {
            _listener = new TcpListener(IPAddress.Loopback, MetricsOptions.GraphiteDefaultServerPort);
            _listener.Start();
        }

        [Fact]
        public void SendMessage()
        {
            const String message = "test";

            var tcs = new TaskCompletionSource<String>();
            var @event = new ManualResetEvent(false);
            MiniMetrics.TcpMetricsClient.StartAsync(IPAddress.Loopback, MetricsOptions.GraphiteDefaultServerPort)
                                        .ContinueWith(_ =>
                                                      {
                                                          if (_.Exception != null)
                                                          {
                                                              tcs.TrySetException(_.Exception.GetBaseException());
                                                              @event.Set();
                                                              return;
                                                          }

                                                          _.Result.OnMessageSent += (sender, args) =>
                                                                                    {
                                                                                        tcs.SetResult(args.Message);
                                                                                        @event.Set();
                                                                                    };
                                                          _.Result.Send(message);
                                                      })
                                        .ContinueWith(_ => tcs.Task)
                                        .Unwrap()
                                        .ContinueWith(_ =>
                                                      {
                                                          @event.WaitOne(TimeSpan.FromSeconds(10d));
                                                          Assert.Equal(message, _.Result);
                                                      })
                                        .Wait();
        }

        public void Dispose()
        {
            _listener.Stop();
        }
    }
}