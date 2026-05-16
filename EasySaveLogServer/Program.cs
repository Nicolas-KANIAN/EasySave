using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using EasyLog;

namespace EasySaveLogServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Configure the server's Logger
            Logger.Instance.Format = LogFormat.Json;
            Logger.Instance.Destination = LogDestination.Local;

            int port = 12345;
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            Console.WriteLine($"[Docker Server] Listening on port {port}...");
            Console.WriteLine($"[Info] Using the EasyLog library for formatting.");

            while (true)
            {
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Accept Error] {ex.Message}");
                }
            }
        }

        static async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using NetworkStream stream = client.GetStream();
                using StreamReader reader = new StreamReader(stream, Encoding.UTF8);

                while (client.Connected)
                {
                    string? jsonLogEntry = await reader.ReadLineAsync();

                    if (jsonLogEntry != null)
                    {
                        var entry = JsonSerializer.Deserialize<LogEntry>(jsonLogEntry);

                        if (entry != null)
                        {
                            Logger.Instance.WriteDailyLog(entry);
                            Console.WriteLine($"[Log Received] {entry.BackupName} - {entry.Timestamp}");
                        }
                    }
                    else break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Client Error] {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}