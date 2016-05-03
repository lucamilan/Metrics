using System;
using System.Net;
using System.Net.Sockets;

namespace MiniMetrics.Net
{
    public class OutbountChannelFactory
    {
        private readonly IPAddress _address;
        private readonly Int32 _port;

        internal OutbountChannelFactory(IPAddress address, Int32 port)
        {
            _address = address;
            _port = port;
        }

        public IOutbountChannel Build()
        {
            return new OutbountChannel(new TcpClient { ExclusiveAddressUse = false },
                                       _address,
                                       _port);
        }

        public IOutbountChannel BuildAutoRecoverable()
        {
            return BuildAutoRecoverable(TimeSpan.FromSeconds(5d));
        }

        public IOutbountChannel BuildAutoRecoverable(TimeSpan recoverySlice)
        {
            return new AutoRecoveryOutbountChannel(new TcpClient { ExclusiveAddressUse = false },
                                                   _address,
                                                   _port,
                                                   recoverySlice);
        }
    }
}