import java.util.*;
import java.lang.reflect.*;
import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.nio.file.*;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.net.*;

//Command Codes:
//a: method invocation requests
//b: outputs or errors that needs to be returned to invocator
//c: left by this program at the last point of the file upto where it has read all requests

//Json Requests/Outputs Format:
//{"index":"1", "command code":"a", "method":"GreetingFunction-strhello"}
//
//EXPLANATION:- index is the index, command codes are discussed above, methods are like 'method_name' and then separate their parameters
//through a dash or a hyphen '-' and on the next side write the parameter value's class and then the value like
//'str' or 'string' for and then without space or any delimeter add the value. Same goes with 'int' for integer values such as
//'int8080' meaning '8080' is an integer class and then it could be handled accordingly here. These codes are safe with custom 
//functions to differentiate b/w class IDs and their values efficiently and the the overall system can be changed according 
//to your needs. Refer to this document for more info: //add the link to the document

class Program{

    //store all method names here in order to not check it again and again if needed multiple times.
    private static final Set<String> METHOD_NAMES = new HashSet<>();
    //run a static code block in the starting of the program to cache all the methods in the script and save them in the 
    //above variable.
    static{
        // Populate method names once during initialization
        for (java.lang.reflect.Method method: Program.class.getDeclaredMethods()){
            METHOD_NAMES.add(method.getName());
        }
    }

    //save the outout or return value or error produced by desired function here
    public static String returnValue; // could be anything, string/int/float/double/bool - type declaration will be used ("{datatype}{returnValue}", eg-> "int8080")

    //Save log's file path to create logs in a specified path
    public static String LogFilePath;
    //Invoc Request Data Format Declaration For Deserialization
    public static class InvocDataClass{
        public String CommandCode;
        public String MethodToInvocate;

        public InvocDataClass(String CMDCode, String Method){
            this.CommandCode = CMDCode;
            this.MethodToInvocate = Method;
        }
    }

    //create a log file in the same directory in order to store logs or function outputs and errors.
    public static void CreateLogFile(String name){
        //try to create a .txt file with the name passed as an argument
        try{
            String logFilePathStr = LogFilePath;
            Path logFilePath = Paths.get(logFilePathStr);

            Files.writeString(logFilePath, "" + System.lineSeparator(),
                StandardOpenOption.CREATE, StandardOpenOption.APPEND);

        }
        catch (IOException e){
            System.out.println("Error in creating file: " + e.getMessage());
        }
    }

    //write in log file function's overload
    public static void WriteInLogs(String line){
        WriteInLogs(line, "PRO");
    }

    //write in log txt file with the time and the date and the log code of a line.
    //call this directly to create a log file (if not already there) and then write in it as normal
    public static void WriteInLogs(String line, String logCode) {
        try {
            String baseDir = LogFilePath;
            Path dirPath = Paths.get(baseDir);
            if (!Files.exists(dirPath)) {
                Files.createDirectories(dirPath);
            }

            Path logFilePath = dirPath.resolve("LFTUCPCLogs.txt");

            LocalDateTime now = LocalDateTime.now();
            DateTimeFormatter formatter = DateTimeFormatter.ofPattern("HH:mm:ss - dd/MM/yyyy");
            String dateAndTimeFormatted = now.format(formatter);
            String finalLine = logCode + " ~ " + dateAndTimeFormatted + " =>\t" + line;

            Files.writeString(logFilePath, finalLine + System.lineSeparator(),
                StandardOpenOption.CREATE, StandardOpenOption.APPEND);
        } catch (IOException e) {
            System.out.println("Error writing to log file: " + e.getMessage());
        }
    }


    //parse the data and convert it to its right type with the help of its data class part sent with the value (ex: int8080 for 'int' '8080')
    public static Object ParseDataType(String RawParameter){
        //parse data into the data class and save it in java
        if(RawParameter.startsWith("string")){
            return RawParameter.substring("string".length());
        }
        else if(RawParameter.startsWith("int")){
            return Integer.parseInt(RawParameter.substring("int".length()));
        }
        else if(RawParameter.startsWith("float")){
            return Float.parseFloat(RawParameter.substring("float".length()));
        }
        else if(RawParameter.startsWith("bool")){
            if (RawParameter.substring("bool".length()).toLowerCase() == "true"){
                return true;
            }else{
                return false;
            }
        }
        return null;
    }

    //custom function for finding out if our string starts with any of the available prefixes that are provided through the parameters
    private static boolean StartsWithAny(String string, String... prefixes){
        for (String prefix : prefixes){
            if(string.startsWith(prefix)){
                return true;
            }
        }
        return false;
    }

    //return just the class of a value according to its passed datatype code
    public static Class<?> ParseDataClass(String RawParameter){
        //Map to transform inputs given to a signature of Java Language and then use it
        if(StartsWithAny(RawParameter, "string", "str")){
            return String.class;
        }
        else if(RawParameter.startsWith("int")){
            return int.class;
        }
        else if(RawParameter.startsWith("float")){
            return float.class;
        }
        else if(StartsWithAny(RawParameter, "bool", "boolean")){//custom function to check if a string has any one of the given prefixes
            return boolean.class;
        }
        return null;
    }

    //this method checks if the input is in the right format ("method_Name-(datatype.value),(datatype.value),...")
    public static void RunMethod(String arg){
        //check if the method sent through argument has a parameters
            if(arg.contains("-")){

                //if the argument has parameters according to the structure, then extract it
                String[] splice_of_arg = arg.trim().split("-");

                //extract the method name from if arg conatins parameters
                String methodName = splice_of_arg[0];

                // extract the string that has all the parameters like stringarg,int80,float80.0
                String methodParameterSplice = splice_of_arg[1];

                //initialize a list to contain all parameters separtely
                List<Object> parsedParams = new ArrayList<>();
                //store theclass of the parameters separetely in same order
                List<Class<?>> paramTypes = new ArrayList<>(); // Added to track parameter types

                //check if there are any parameters. (if yes) then are there are multiple of them.
                boolean containsParams = (methodParameterSplice.length() > 0)? true : false;
                boolean multipleParams = (containsParams)? ((methodParameterSplice.contains(","))? true: false) : false;

                //check if method asked for exists
                if(METHOD_NAMES.contains(methodName)){
                    //check if arg passed contains any parameter(s) for this method
                    if(containsParams){
                        //check if multiple parameters are passed and launch the method accordingly
                        if(multipleParams){
                            //split the parameters through (,) delimeter and save them separately
                            String[] methodParameters = methodParameterSplice.split(",");
                            for (String param: methodParameters){
                                //add parsed data to the parameters list
                                parsedParams.add(ParseDataType(param));

                                //add class of the parsed parameters to the class list
                                paramTypes.add(ParseDataClass(param));
                            }
                            //print all passed parameters for debugging
                            System.out.println("Method Params: " + parsedParams);                           
                        }
                        //take the one and only parameter and save it to launch themethod accordingly
                        else{
                            parsedParams.add(ParseDataType(methodParameterSplice));
                            //print all passed parameters for debugging
                            System.out.println("Method Param: " + parsedParams);
                        }

                        //the saved parameters whether multiple or single doesn't make a difference
                        try {
                            Method method = Program.class.getDeclaredMethod(methodName, paramTypes.toArray(new Class[0]));
                            method.invoke(null, parsedParams.toArray());
                        } catch (Exception e) {
                            System.out.println("Error invoking " + methodName + ": " + e.getMessage());
                            WriteInLogs("Error invoking " + methodName + ": " + e.getMessage(), "ERR");
                        }

                    }else{
                        System.out.println("No parameter.");

                        //now launching the method with no parameters
                        try {
                            Method method = Program.class.getDeclaredMethod(methodName); // No paramTypes needed
                            method.invoke(null); // No parameters passed
                        } catch (Exception e) {
                            System.out.println("Error invoking " + methodName + ": " + e.getMessage());
                            WriteInLogs("Error invoking " + methodName + ": " + e.getMessage(), "ERR");
                        }
                    }
                }

                
            }else{
                System.out.println("Java running: No Method");
            }
    }
    //**entry point**//
    //ENTRY POINT of the program (this code block runs as the start point of the script because it has the 'main' method signature as the name).
    public static void main(String[] args) {
        if (args.length >= 2) { // arg[0]: contains the port of the localhost server; arg[1]: contains the path for log file (eg->"C://Users/Downloads/log.txt")
            int port;
            try {
                port = Integer.parseInt(args[0]);
                WriteInLogs("port: " + port);
            } catch (Exception ex) {
                port = 54321; // fallback port
                System.out.println("Can't parse port: " + ex);
            }

            LogFilePath = args[1]; //assign the second arg to as the value to LogFilePath

            try (
                //the below socket, out and in are allocated to this try statement's code block and will be automatically removed from memory one the block exits or finished execution
                Socket socket = new Socket(InetAddress.getLoopbackAddress(), port);
                BufferedReader in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
                BufferedWriter out = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream()));
            ) {
                System.out.println("Connection to CSX server successful");

                // ❗ Wait indefinitely for message (blocking call)
                String receivedMessage;
                while (!(receivedMessage = in.readLine()).equals("exit")) {
                    if (!receivedMessage.trim().isEmpty() && receivedMessage.contains("-")) {
                        System.out.println("Received: " + receivedMessage);
                        RunMethod(receivedMessage);
                        out.write(returnValue);
                        out.flush();
                        WriteInLogs(receivedMessage);
                        break; // 🔁 Exit after logging the first valid message
                    }
                }

            } catch (IOException ex) {
                System.out.println("Can't connect to CSX server or read from socket: " + ex);
            }
        } else {
            System.out.println("No port argument provided.");
        }
    }


    //method created to call for checking if the system is working as expected or not
    public static void StartLFTUCServer(String IPAddress, int port){
        StartLFTUCServer(IPAddress, port, 94.26f, true);
    }

    public static void StartLFTUCServer(String IPAddress, int port, float swim, boolean lmao){
        String message = ("Starting Server At: " + IPAddress + ":" + port + "/" + swim + "\\" + lmao);
        returnValue = "string" + message; //if you wanna parse your output, include its datatype as a prefix with no space
        System.out.println(returnValue);
        WriteInLogs(returnValue);
    }
}