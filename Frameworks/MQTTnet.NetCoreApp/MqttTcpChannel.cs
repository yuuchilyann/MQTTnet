using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using MQTTnet.Core.Channel;
using MQTTnet.Core.Client;
using MQTTnet.Core.Exceptions;

namespace MQTTnet
{
    public class MqttTcpChannel : IMqttCommunicationChannel, IDisposable
    {
        private readonly Socket _socket;

        public MqttTcpChannel()
        {
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        public MqttTcpChannel(Socket socket)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }

        public async Task ConnectAsync(MqttClientOptions options)
        {
            try
            {
                await _socket.ConnectAsync(options.Server, options.Port);
            }
            catch (SocketException exception)
            {
                throw new MqttCommunicationException(exception);
            }
        }

        public async Task DisconnectAsync()
        {
            try
            {
                _socket.Dispose();
                await Task.FromResult(0);
            }
            catch (SocketException exception)
            {
                throw new MqttCommunicationException(exception);
            }
        }

        public async Task WriteAsync(byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));

            try
            {
                await _socket.SendAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
            }
            catch (SocketException exception)
            {
                throw new MqttCommunicationException(exception);
            }
        }

        public async Task ReadAsync(byte[] buffer)
        {
            try
            {
                var buffer2 = new ArraySegment<byte>(buffer);
                await _socket.ReceiveAsync(buffer2, SocketFlags.None);
            }
            catch (SocketException exception)
            {
                throw new MqttCommunicationException(exception);
            }
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }
    }
}