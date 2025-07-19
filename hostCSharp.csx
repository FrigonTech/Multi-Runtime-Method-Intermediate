
using System.Diagnostics;
using System.Text.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text;
//*import System.Exception.frigontech;*

public static int commandCodeIndex = 0;
public static string fileName = "command_Invoc_Requests.json";
public static TcpClient client;
public static int port = 54321;

string response = ""; // Use JSON if needed

public class MethodInvocRequest
{
    public string index { get; set; }
    public string commandCode { get; set; }
    public string MethodAndParameter { get; set; }

    public MethodInvocRequest(string codename, string function)
    {
        commandCode = codename;
        MethodAndParameter = function;
    }
}

public static void InitiateService()
{
    try
    {
        var server = new TcpListener(IPAddress.Loopback, port);
        server.Start();
        Console.WriteLine($"Server started on 127.0.0.1:{port} at {DateTime.Now:yyyy-MM-dd HH:mm:ss} IST");
        // Loop to accept one client at a time
        while (true)
        {
            MakeMethodInvocRequest("lmai");
            client = server.AcceptTcpClient(); // Blocks until a client connects
            Console.WriteLine($"Client connected at {DateTime.Now:yyyy-MM-dd HH:mm:ss} IST");
            // Keep client open until it disconnects
            while (client != null && client.Connected)
            {
                SendCommand();//sends command
                Console.WriteLine($"Client disconnected at {DateTime.Now:yyyy-MM-dd HH:mm:ss} IST");
                EndService();
                System.Threading.Thread.Sleep(100); // Prevent tight loop
            }
            
            break;
        }
    }
    catch (SocketException e)
    {
        Console.WriteLine("Socket error: " + e.Message);
    }
}

public static void SendCommand()
{
    if (client != null && client.Connected)
    {
        try
        {
            string response = ""; // Use JSON if needed
            byte[] responseBytes = Encoding.UTF8.GetBytes(response + Environment.NewLine);
            client.GetStream().Write(responseBytes, 0, responseBytes.Length);
            Console.WriteLine($"Sent to client: {response} at {DateTime.Now:yyyy-MM-dd HH:mm:ss} IST");
        }
        catch (IOException e)
        {
            Console.WriteLine("Error sending message: " + e.Message);
        }
    }
    else
    {
        Console.WriteLine("No client connected, can't send message :|");
    }
}

public static void EndService()
{
    try
    {
        if (client != null || !client.Connected)
        {
            client.GetStream().Close();
            client.Close();
            Console.WriteLine($"Client connection closed at {DateTime.Now:yyyy-MM-dd HH:mm:ss} IST");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine("Error during cleanup: " + e.Message);
    }
}

public static void MakeMethodInvocRequest(string function)
{
    // Launch Java client (original process control)
    var javaProcess = new ProcessStartInfo
    {
        FileName = "cmd.exe",
        Arguments = $"/c javac test.java && java -cp \".;lib/*\" Program {port}",
        /*
        "/c {compiler} {filename} && {runtime} -cp \".;{lib folder}/*\" {entry-class} {arguments}"
        */
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };
    try
    {
        var process = Process.Start(javaProcess);

        // Read and print output live
        Task.Run(() => Console.WriteLine(process.StandardOutput.ReadToEnd()));
        Task.Run(() => Console.WriteLine(process.StandardError.ReadToEnd()));
        
        Console.WriteLine("Started Java client.");
    }
    catch (Exception e)
    {
        Console.WriteLine("Error starting Java client: " + e.Message);
    }
}

bool exit = false;

while (!exit)
{
    Console.Write("LFTUC CSX Interop controller: ");
    string line = Console.ReadLine();

    if (line.Contains("-"))
    {
        Console.WriteLine(line);
        InitiateService();
    }
    else if (line.Contains("exit"))
    {
        EndService();
        break;
    }
}