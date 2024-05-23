using System.Net;
using System.Net.Sockets;
using System.Text;
namespace AsyncServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var endPoint = new IPEndPoint(IPAddress.Any, 1234);
            socket.Bind(endPoint);
            socket.Listen(10);
            Console.WriteLine("Server has started");


            while (true)
            {
                var client = await socket.AcceptAsync();
                _ = Task.Run(() => HandleClientAsync(client));
            }
        }

        private static async void HandleClientAsync(Socket client)
        {
            var buf = new byte[1024];
            int bytesRead = await client.ReceiveAsync(buf, SocketFlags.None);
            string receivedMessage = Encoding.UTF8.GetString(buf, 0, bytesRead).Trim();
            Console.WriteLine($"Отримано від  {client?.RemoteEndPoint?.ToString()}: {receivedMessage}");

            string responseMessage;
            if (receivedMessage.Trim().ToUpper() == "DATA")
            {
                responseMessage = DateTime.Now.ToString("yyy-MM-dd");
            }
            else if (receivedMessage.Trim().ToUpper() == "TIME")
            {
                responseMessage = DateTime.Now.ToString("HH:mm:ss");
            }
            else
            {
                responseMessage = "Invalid request";
            }
           await client?.SendAsync(Encoding.UTF8.GetBytes(responseMessage), SocketFlags.None);
            client?.Shutdown(SocketShutdown.Both);
            client?.Close();

        }
    }
}

