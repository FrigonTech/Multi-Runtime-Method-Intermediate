
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
public static TcpClient client = new TcpClient();
public static NetworkStream clientStream;
public static int port = 54321;

public static string message = "";

public static bool ConnectToRuntime()
{
    MakeMethodInvocRequest();
    try
    {
        client.Connect(IPAddress.Loopback, port);
        clientStream = client.GetStream();
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine("err connecting: " + ex);
        return false;
    }

}

public static void SendToRuntime()
{
    try
    {
        using (var writer = new StreamWriter(clientStream, new System.Text.UTF8Encoding(false), leaveOpen: true))
        {
            writer.WriteLine(message);  // automatically adds \n
            writer.Flush();
            Console.WriteLine("Sent message to server: " + message);
        }     
    }
    catch (IOException ex)
    {
        Console.WriteLine("err sending message: " + ex);
    }
}

public static string GetReplyFromRuntime()
{
    string returnResponse = "--$null";
    try
    {
        byte[] buffer = new byte[1024];
        int bytesRead = clientStream.Read(buffer, 0, buffer.Length);
        returnResponse = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine("Message received: " + returnResponse);
    }
    catch (IOException ex)
    {
        Console.WriteLine("err receiving response: " + ex);
    }

    return returnResponse;
}

public static bool DisconnectFromServer(bool ShutdoownServer)
{
    try
    {
        message = "exit";
        SendToRuntime();
        clientStream.Close();
        client.Close();
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine("Err disconnecting: " + ex);
        return false;
    }
    
}

public static void MakeMethodInvocRequest()
{
    // Launch Java client (original process control)
    var javaProcess = new ProcessStartInfo
    {
        FileName = "cmd.exe",
        Arguments = $"/c javac test.java && java -cp \".;lib/*\" Program {port}",
        /*
        "/c {compiler} {filename} && {runtime} -cp \".;{lib folder}/*\" {entry-class} {arguments}"
        
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        */
        UseShellExecute = false,
        CreateNoWindow = true
    };
    try
    {
        var process = Process.Start(javaProcess);

        Console.WriteLine("Started Java Server.");
    }
    catch (Exception e)
    {
        Console.WriteLine("Error starting Java server: " + e.Message);
    }
}

bool exit = false;
ConnectToRuntime();
while (!exit)
{
    Console.Write("LFTUC CSX Interop controller: ");
    string line = Console.ReadLine();

    if (line.Contains("-"))
    {
        Console.WriteLine(line);
        message = line.Trim();
        SendToRuntime();
        string returns = GetReplyFromRuntime();
        Console.WriteLine("returned: "+returns.Trim());
    }
    else if (line.Contains("exit"))
    {
        DisconnectFromServer(true); //disconnect and additionally shutdown the server as well
        break;
    }
}