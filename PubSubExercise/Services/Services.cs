using CommonLib.Models;
using System.Net.Sockets;
using System.Text;

namespace ClientApp.Services
{
    public class ClientOperations : IOperation, IDisposable
    {
        public void Dispose()
        {
            if (TcpClient != null) TcpClient.Close();
        }

        public TcpClient? TcpClient { get; set; }

        public void Connect(string localIp, int port)
        {
            try
            {
                //Console.WriteLine($"Connecting to {localIp} with port {port} ...");
                if (TcpClient != null && TcpClient.Connected) return;
                TcpClient = new TcpClient(localIp, port);
                if (!TcpClient.Connected)
                    Console.WriteLine("Error on tcpClient connection!");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error on tcpClient connection: {ex.Message}");
                Console.ResetColor();
                throw new Exception(ex.Message);
            }
        }

        public void Execute<T>(T req) where T : ICommand
        {
            try
            {
                if (TcpClient != null && TcpClient.Connected)
                {
                    var subscribeBytes = Encoding.ASCII.GetBytes(CommandConverter.Serialize(req));
                    TcpClient.GetStream().Write(subscribeBytes, 0, subscribeBytes.Length);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Succeded!");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("Error on tcpClient connection!");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error on executing {nameof(req.ReqType)} request: {ex.Message}");
                Console.ResetColor();
                throw new Exception(ex.Message);
            }
        }
    }

    public interface IOperation
    {
        public TcpClient? TcpClient { get; set; }
        void Connect(string localIp, int port);
        void Execute<T>(T req) where T : ICommand;
    }
}
