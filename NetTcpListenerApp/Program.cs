// Chat Server
using NetTcpListenerApp;
using System.Net;
using System.Net.Sockets;

ChatServer server = new ChatServer();
await server.ListenAsync();

