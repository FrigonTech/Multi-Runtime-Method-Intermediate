
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

public static string LogsFilePath = "C:/Users/DESKTOP-A/Desktop/test-environment/";

public static string Message = ""; // Use JSON if needed

public static bool ExitIntermediate = false;

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
        StartJavaProcess();
        // Loop to accept one client at a time
        while (!ExitIntermediate)
        {
            client = server.AcceptTcpClient(); // Blocks until a client connects
            Console.WriteLine($"Client connected at {DateTime.Now:yyyy-MM-dd HH:mm:ss} IST");
            // Keep client open until it disconnects
            while (client != null && client.Connected)
            {
                SendCommand();//sends command
                Console.WriteLine("got from client: " + ReceiveReturnedValue());
            }

            break;
        }
        EndService();
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
            byte[] responseBytes = Encoding.UTF8.GetBytes(Message + Environment.NewLine);
            client.GetStream().Write(responseBytes, 0, responseBytes.Length);
            Console.WriteLine($"Sent to client: {Message} at {DateTime.Now:yyyy-MM-dd HH:mm:ss} IST");
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

public static string ReceiveReturnedValue()
{
    string returns = "";
    if (client != null && client.Connected)
    {
        try
        {
            using (StreamReader reader = new StreamReader(client.GetStream()))
            {
                returns = reader.ReadLine();
                Console.WriteLine($"Received from client: {returns} at {DateTime.Now:yyyy-MM-dd HH:mm:ss} IST");
            }
        }
        catch (IOException e)
        {
            Console.WriteLine("Error receiving message: " + e.Message);
        }
    }
    else
    {
        Console.WriteLine("No client connected, can't receive message :|");
    }

    return returns;
}

public static void EndService()
{
    try
    {
        if (client != null || !client.Connected)
        {
            Message = "exit";
            SendCommand();// sends graceful exit code
            client.GetStream().Close();
            client.Close();
            Console.WriteLine($"Client connection closed at {DateTime.Now:yyyy-MM-dd HH:mm:ss} IST");
        }
    }
    catch (IOException e)
    {
        Console.WriteLine("Error during graceful exit: " + e.Message);
    }
    catch (Exception e)
    {
        Console.WriteLine("Error during cleanup: " + e.Message);
    }
}

public static void StartJavaProcess()
{
    // Launch Java client (original process control)
    var javaProcess = new ProcessStartInfo
    {
        FileName = "cmd.exe",
        Arguments = $"/c javac test.java && java -cp \".;lib/*\" Program {port} {LogsFilePath}",
        /*
        "/c {compiler} {filename} && {runtime} -cp \".;{lib folder}/*\" {entry-class} {arguments}"
        */
        //RedirectStandardOutput = true,
        //RedirectStandardError = true,
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
        Message = line.Trim();
        InitiateService();
    }
    else if (line.Contains("exit"))
    {
        ExitIntermediate = true;
        break;
    }
}