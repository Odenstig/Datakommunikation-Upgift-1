using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Uppgift1.Client;

//Client

Console.OutputEncoding = Encoding.UTF8;

//Basic object to be sent between client and server
Person person = new();

//Creates new TcpClient
using TcpClient client = new();

//Connects to our Server
await client.ConnectAsync("localhost", 5006);

//Creates new stream to allow reading/writing between client and server
using var stream = client.GetStream();
var reader = new StreamReader(stream, new UTF8Encoding(false));
var writer = new StreamWriter(stream, new UTF8Encoding(false));

//Some values for our object
Console.Write("Name: ");
person.Name = Console.ReadLine();
Console.Write("Email: ");
person.Email = Console.ReadLine();
Console.Write("Address: ");
person.Address = Console.ReadLine();

//Serializes our object into Json 
var json = JsonSerializer.Serialize(person);

//Writes our serialized data to Streambuffer
await writer.WriteAsync(json);
//Clears Streambuffer and writes to server
await writer.FlushAsync();

while (true)
{
    //Creates and allocates bytes
    var buffer = new byte[1024];

    //Reads and stores received bytes in memory
    int received = stream.Read(buffer);

    //Converts stored bytes to string
    string data = Encoding.UTF8.GetString(buffer.AsSpan(0, received));

    //Deserializes data into person object
    var jsonDeserialized = JsonSerializer.Deserialize<Person>(data);

    //Serializes person object again readying it to be sent to stream
    var jsonSerialized = JsonSerializer.Serialize(jsonDeserialized);

    //Writes out data echoed from server
    Console.Write("\r\n");
    Console.WriteLine("Server echoed: {0}", jsonSerialized);
}
