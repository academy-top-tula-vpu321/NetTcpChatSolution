// Chat Client

using System.Net;
using System.Net.Sockets;

string serverHost = "192.168.1.60";
int serverPort = 5000;

Console.Write("Will you configure the server? <y/n>");
string answer = Console.ReadLine();
if(answer != null && answer.ToLower() == "y")
{
    Console.Write("Input server ip: ");
    serverHost = Console.ReadLine();

    Console.Write("Input server port: ");
    serverPort = Int32.Parse(Console.ReadLine());
}

string name;
string message;
bool isDisconnect = false;

using (TcpClient client = new())
{
    Console.Write("Input nikname: ");
    name = Console.ReadLine();

    StreamReader? reader = null;
    StreamWriter? writer = null;

    try
    {
        client.Connect(serverHost, serverPort);
        reader = new(client.GetStream());
        writer = new(client.GetStream());

        if (reader is null || writer is null) return;

        Task.Run(() => ReceiveMessageAsync(reader));
        await SendMessageAsync(writer);

    }
    catch(Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

    writer.Close();
    reader.Close();
    client.Close();
}

async Task SendMessageAsync(StreamWriter writer)
{
    await writer.WriteLineAsync(name);
    await writer.FlushAsync();

    Console.WriteLine("Enter a message and press ENTER to send");

    while(true)
    {
        if (isDisconnect) return;

        message = Console.ReadLine();
        await writer.WriteLineAsync(message);
        await writer.FlushAsync();
    }
}

async Task ReceiveMessageAsync(StreamReader reader)
{
    while(true)
    {
        try
        {
            string msg = await reader.ReadLineAsync();
            
            if(string.IsNullOrEmpty(msg)) continue;

            if(OperatingSystem.IsWindows())
            {
                var position = Console.GetCursorPosition();
                int row = position.Top;
                int column = position.Left;

                Console.MoveBufferArea(0, row, column, 1, 0, row + 1);
                
                Console.SetCursorPosition(0, row);
                Console.WriteLine(msg);

                Console.SetCursorPosition(column, row + 1);
            }
            else
                Console.WriteLine(msg);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            isDisconnect = true;
            return;
        }
    }
}