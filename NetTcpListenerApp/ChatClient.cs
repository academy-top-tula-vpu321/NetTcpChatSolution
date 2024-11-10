// Chat Server
// Class Client

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetTcpListenerApp
{
    public class ChatClient
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public StreamReader Reader { get; }
        public StreamWriter Writer { get; }

        TcpClient client;
        ChatServer server;
        public ChatClient(TcpClient client, ChatServer server) 
        {
            this.client = client;
            this.server = server;

            NetworkStream stream = client.GetStream();

            Reader = new(stream);
            Writer = new(stream);
        }

        public async Task WorkAsync()
        {
            try
            {
                string? clientName = await Reader.ReadLineAsync();
                string message = $"Client {clientName} join to chat";
                
                await server.SendingAsync(message, Id);
                Console.WriteLine(message);

                while(true)
                {
                    try
                    {
                        message = await Reader.ReadLineAsync();
                        if (message is null) continue;

                        message = $"{clientName}: {message}";

                        await server.SendingAsync(message, Id);
                        Console.WriteLine(message);
                    }
                    catch(Exception ex)
                    {
                        message = $"{clientName} disconnect";

                        await server.SendingAsync(message, Id);
                        Console.WriteLine(message);

                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                server.RemoveClient(Id);
            }
        }

        public void Close()
        {
            Writer.Close();
            Reader.Close();
            client.Close();
        }
    }
}
