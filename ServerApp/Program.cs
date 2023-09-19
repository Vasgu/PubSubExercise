using CommonLib.Resources;
using ServerApp.Services;
using System.Net.Sockets;
using System.Net;
using System.Text;
using ServerApp.Models;

namespace ServerApp
{
    internal static class Program
    {
        private static void Main()
        {
            var server = new Server();
            server.Start();
        }
    }

    public class Server
    {
        private readonly List<Channel> _channels = new();
        private readonly TcpListener _listener = new(IPAddress.Parse(Configurations.LocalIp), Configurations.Port);

        public void Start()
        {
            _listener.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                var client = _listener.AcceptTcpClient();
                var clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
        }

        private void HandleClient(object? clientObj)
        {
            if (clientObj == null) throw new ArgumentNullException(nameof(clientObj));

            try
            {
                var client = (TcpClient)clientObj;

                using (var stream = client.GetStream())
                {
                    var buffer = new byte[client.ReceiveBufferSize];
                    var bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var payload = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    if (string.IsNullOrEmpty(payload)) return;

                    ServerOperations.Execute(payload, _channels);
                }

                client.Close();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error handling client: {ex.Message}");
                Console.ResetColor();
                throw new Exception(ex.Message);
            }
        }
    }
}