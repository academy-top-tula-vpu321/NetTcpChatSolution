// Chat Server
// Class Server

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetTcpListenerApp
{
    public class ChatServer
    {
        TcpListener server;
        List<ChatClient> clients = new();

        string host;
        int port = 5000;

        public async Task ListenAsync()
        {
            try
            {
                var hostName = Dns.GetHostName();
                var addrreses = Dns.GetHostAddresses(hostName, AddressFamily.InterNetwork);
                server = new(addrreses[0], port);
                host = (server.LocalEndpoint as IPEndPoint).Address.ToString();

                server.Start();
                Console.WriteLine($"Server {host} starting");

                while(true)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    ChatClient chatClient = new ChatClient(client, this);
                    clients.Add(chatClient);

                    Task.Run(chatClient.WorkAsync);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                this.Stop();
            }
        }
        public async Task SendingAsync(string message, string id)
        {
            foreach(var client in clients)
            {
                if(id != client.Id)
                {
                    await client.Writer.WriteLineAsync(message);
                    await client.Writer.FlushAsync();
                }
            }
        }
        public void RemoveClient(string id)
        {
            ChatClient? client = clients.FirstOrDefault(c => c.Id == id);
            if(client is not null)
                clients.Remove(client);
            client.Close();
        }
        public void Stop()
        {
            foreach (var client in clients)
                client.Close();
            
            server.Stop();
        }
    }
}
