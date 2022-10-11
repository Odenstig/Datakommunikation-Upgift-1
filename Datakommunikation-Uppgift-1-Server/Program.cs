using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Uppgift1.Client;

//Server

Console.OutputEncoding = Encoding.UTF8;

//Creates new endpoint on localhost with port 5006
var endpoint = new IPEndPoint(IPAddress.Loopback, 5006);

//Creates new socket with the endpoint we created above, Stream SocketType to establish connection with client, and required Tcp Protocol
using var socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

//Binds socket to endpoint
socket.Bind(endpoint);

//Places socket in listening state, listens to client requests
socket.Listen();
Console.WriteLine("listening on port {0}", endpoint.Port);

//Awaits client connection
var handler = await socket.AcceptAsync(CancellationToken.None);

Console.Write("\r\n");
Console.WriteLine("Connection Secured");

while (true)
{
    //Creates and allocates bytes to buffer
    var buffer = new byte[2048];

    //Checks how many bytes have been received, fills buffer
    var receivedByteLength = await handler.ReceiveAsync(buffer, SocketFlags.None);

    //Converts received data on buffer to an array
    var data = buffer.Take(receivedByteLength).ToArray();

    //Converts data from bytes to string 
    string response = Encoding.UTF8.GetString(data);

    //Deserializes data into person object
    var jsonDeserialized = JsonSerializer.Deserialize<Person>(data);

    //Serializes our person object, readying it for server response
    var jsonSerialized = JsonSerializer.Serialize(jsonDeserialized);

    //Converts serialized data back into bytes for server response
    buffer = Encoding.UTF8.GetBytes(jsonSerialized);

    Console.Write("\r\n");
    Console.WriteLine("Server recieved: {0}", response);

    Console.Write("\r\n");
    Console.WriteLine("Deserialized Name: {0}", jsonDeserialized.Name);
    Console.WriteLine("Deserialized Email: {0}", jsonDeserialized.Email);
    Console.WriteLine("Deserialized Address: {0}", jsonDeserialized.Address);

    //Sends Server response back to client
    await handler.SendAsync(buffer, SocketFlags.None);
}