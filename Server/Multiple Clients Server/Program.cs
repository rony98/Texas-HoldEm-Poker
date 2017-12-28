/*
 *Author: Rony Verch
 *File Name: Multiple Clients Server.csproj
 *Project Name: Server
 *Creation Date: December 17, 2014
 *Modified Date: January 18, 2015
 *Description: This is the server for the Texas HoldEm Poker game and this server will need to be running for the different clients 
 *             to connect to so a multiplayer game can be played.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Multiple_Clients_Server
{
    class Program
    {
        //List of sockets where each client's socket will be stored
        static List<Socket> clientSockets = new List<Socket>();

        //Variable for the server socket
        static Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //List for the client names and array for which names were added
        static List<string> clientNames = new List<string>();
        static bool[] clientNameAdded = new bool[9] { false, false, false, false, false, false, false, false, false };

        //Variable for if the server state is on game lobby or not
        static bool gameLobby = true;

        //Array for which clients are currently ready
        static bool[] clientsReady = new bool[9] { false, false, false, false, false, false, false, false, false };

        //Variable for the amount of clients that are ready
        static int clientsReadyCount = 0;

        //Byte arrays for the sent and recieve buffers
        static byte[] buffer = new byte[1024];
        static byte[] sentBuff;

        //Variables for the amount of clients that were sent a message and the text that was recieved
        static int clientSentCount = 0;
        static string textRecieved;


        static void Main(string[] args)
        {
            //Set the title on the console to server
            Console.Title = "Server";

            //Write the IP
            Console.WriteLine("The server's IP is: " + GetLocalIP());

            //Start setting up server
            SetupServer();

            //Read the next line so the console doesn't close
            Console.ReadLine();
        }



        //Pre: None
        //Post: The server is set up
        //Desc: This subprogram sets up the server and starts listening and accepting clients that are trying to join
        private static void SetupServer()
        {
            //Write that the server is being set up
            Console.WriteLine("Setting up server...");

            //Bind the server socket
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 100));

            //Start listening to clients
            serverSocket.Listen(1);

            //Begin accepting clients that are trying to join
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
        }



        //Pre: A client tried to connect
        //Post: The client is accepted
        //Desc: This subprogram accepts the connection from the clients
        private static void AcceptCallBack(IAsyncResult AR)
        {
            //Ends the accept of the client
            Socket socket = serverSocket.EndAccept(AR);

            //Writes that a client connected
            Console.WriteLine("Connected");

            //Adds the socket of the client
            clientSockets.Add(socket);

            //Begins recieving messages from the client
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), socket);

            //If the amount of clients is less then 9
            if (clientSockets.Count < 9)
            {
                //Begins accepting more clients
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
            }
        }



        //Pre: None
        //Post: The local IP is returned
        //Desc: This subprogram finds the local IP and returns it
        private static string GetLocalIP()
        {
            //Creates a new IPHostEntry to find all the IP addresses for the current network
            IPHostEntry host;

            //Gets all the IP Addresses for the current network
            host = Dns.GetHostEntry(Dns.GetHostName());

            //For each IP address in the host address list
            foreach (IPAddress ip in host.AddressList)
            {
                //If the address family of the current ip is InterNetwork
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    //Returns the IP
                    return ip.ToString();
                }
            }

            //Returns the Local IP
            return "127.0.0.1";
        }



        //Pre: The socket that is being checked
        //Post: The index value of the socket
        //Desc: This subprogram returns the index value of the socker
        private static int ClientIndexValue(Socket socket)
        {
            //Loop for every socket that is connected
            for (int i = 0; i < clientSockets.Count; i++)
            {
                //If the socket that is being checked is equal to the current socket
                if (socket == clientSockets[i])
                {
                    //Return the index value
                    return i;
                }
            }

            //Return the index value
            return 0;
        }



        //Pre: The client sent a message
        //Post: The message was recieved and worked with
        //Desc: This subprogram recieves the messages from the clients and re-directs it to every other client when the poker game is on
        private static void RecieveCallBack(IAsyncResult AR)
        {
            //Variables for the amount if clients that were sent a message
            clientSentCount = 0;

            //String for what message is being sent
            string messageSend = "";

            //If the sent buff is not empty
            if (sentBuff != null)
            {
                //Clear the sent buff
                Array.Clear(sentBuff, 0, sentBuff.Length);
            }

            //If the game lobby is true
            if (gameLobby == true)
            {
                //Try doing this code
                try
                {
                    //Get's the socket that is currently being worked with
                    Socket socket = (Socket)AR.AsyncState;

                    //Gets the size of the recieved message and end the recieve
                    int recievedSize = socket.EndReceive(AR);

                    //Creates a new byte array with the size of the message size
                    byte[] recievedBuff = new byte[recievedSize];

                    //Copies one array into the other
                    Array.Copy(buffer, recievedBuff, recievedSize);

                    //Converts the recieved byte array into a string
                    textRecieved = Encoding.ASCII.GetString(recievedBuff);

                    //If the current client was not added
                    if (clientNameAdded[ClientIndexValue(socket)] == false)
                    {
                        //Write that the client name was recieved
                        Console.WriteLine("Client Name Recieved: " + textRecieved);

                        //Set the current client name to added
                        clientNameAdded[ClientIndexValue(socket)] = true;

                        //Add the client name
                        clientNames.Add(textRecieved);

                        //Set the messageSend string to send the client names of every client
                        messageSend = "1" + Convert.ToString(clientNames.Count) + clientNames[0];

                        //Update the string with every the name of every client
                        for (int i = 1; i < clientNames.Count; i++)
                        {
                            messageSend += "@";
                            messageSend += clientNames[i];
                        }

                        //Update the string with either a true or a false depending on if the current client is currently ready or not
                        for (int i = 0; i < clientsReady.Length; i++)
                        {
                            if (clientsReady[i])
                            {
                                messageSend += "@" + "true";
                            }
                            else
                            {
                                messageSend += "@" + "false";
                            }
                        }

                        //Convert the message into a byte array
                        sentBuff = Encoding.ASCII.GetBytes(messageSend);

                        //Set the amount of clients sent to 0
                        clientSentCount = 0;

                        ////Loop until the loop is broken
                        //while (true)
                        //{
                        //    //If the current client is connected
                        //    if (clientSockets[clientSentCount].Connected)
                        //    {
                                //Begin sending the current client
                                clientSockets[clientSentCount].BeginSend(sentBuff, 0, sentBuff.Length, SocketFlags.None, new AsyncCallback(SendCallBack), socket);

                        //        break;
                        //    }
                        //    //If the current client is not connected
                        //    else
                        //    {
                        //        //Add one to the amount of clients sent
                        //        clientSentCount++;
                        //    }
                        //}
                    }
                    //If the current client was added
                    else
                    {
                        //If the text that was recieved is a true
                        if (textRecieved == "true")
                        {
                            //Display to the user
                            Console.WriteLine(clientNames[ClientIndexValue(socket)] + " is ready to start.");
                        }
                        //If the text that was recieved is a false
                        else if (textRecieved == "false")
                        {
                            //Display to the user
                            Console.WriteLine(clientNames[ClientIndexValue(socket)] + " is not ready to start.");
                        }

                        //If the text that was recieved is either a true or a false 
                        if (textRecieved == "true" || textRecieved == "false")
                        {
                            //Update the boolean for if the client is currently ready or not
                            clientsReady[ClientIndexValue(socket)] = Convert.ToBoolean(textRecieved);
                        }

                        //If the first client is ready
                        if (clientsReady[0])
                        {
                            //Update the message send string
                            messageSend = "2" + "true";
                        }
                        //If the first client is not ready
                        else
                        {
                            //Update the message send string
                            messageSend = "2" + "false";
                        }

                        //Loop for every client other then the first
                        for (int i = 1; i < clientsReady.Length; i++)
                        {
                            //If the current client is ready
                            if (clientsReady[i])
                            {
                                //Update the message send string
                                messageSend += "@" + "true";
                            }
                            //If the current client is not ready
                            else
                            {
                                //Update the message send string
                                messageSend += "@" + "false";
                            }
                        }

                        //Set the clients ready count to 0
                        clientsReadyCount = 0;

                        //Loop for every client
                        for (int i = 0; i < clientsReady.Length; i++)
                        {
                            //If the current client is ready
                            if (clientsReady[i])
                            {
                                //Add one to the amount of clients that are ready
                                clientsReadyCount++;
                            }
                        }

                        //If the clients ready count is equal to the amount of clients that joined and the amount of clients that joined is greater then 2
                        if (clientsReadyCount == clientNames.Count && clientsReadyCount > 2)
                        {
                            //Set the game lobby to false
                            gameLobby = false;

                            //Loop for every client
                            for (int i = 0; i < clientSockets.Count; i++)
                            {
                                //Byte array for giving each player their player numbers
                                byte[] clientTurnNumBuffer;

                                //String Variable for sending each client their player number's
                                string tempString = "3" + ClientIndexValue(clientSockets[i]);

                                //Converting the string into a byte array
                                clientTurnNumBuffer = Encoding.ASCII.GetBytes(tempString);

                                //If the current client is connected
                                if (clientSockets[i].Connected)
                                {
                                    //Send the client their player number
                                    clientSockets[i].Send(clientTurnNumBuffer);
                                }
                            }
                        }

                        //Convert the string into a byte array
                        sentBuff = Encoding.ASCII.GetBytes(messageSend);

                        //Set the amount of clients sent to 0
                        clientSentCount = 0;

                        ////Loop until the loop is broken
                        //while (true)
                        //{
                        //    //If the current client is connected
                        //    if (clientSockets[clientSentCount].Connected)
                        //    {
                                //Begin sending the current client
                                clientSockets[clientSentCount].BeginSend(sentBuff, 0, sentBuff.Length, SocketFlags.None, new AsyncCallback(SendCallBack), socket);

                        //        break;
                        //    }
                        //    //If the current client is not connected
                        //    else
                        //    {
                        //        //Add one to the amount of clients sent
                        //        clientSentCount++;
                        //    }
                        //}
                    }

                    //Begin recieving from the current client again
                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), socket);
                }
                //Catch any exceptions
                catch (Exception ex)
                {
                    //Display the exception
                    Console.WriteLine(ex.Message);
                }
            }
            //If the game lobby is equal to false
            else
            {
                //Try doing this code
                try
                {
                    //Write that a message was recieved
                    Console.WriteLine("Recieved Message");

                    //Get's the socket that is currently being worked with
                    Socket socket = (Socket)AR.AsyncState;

                    //Gets the size of the recieved message and end the recieve
                    int recievedSize = socket.EndReceive(AR);

                    //Creates a new byte array with the size of the message size
                    byte[] recievedBuff = new byte[recievedSize];

                    //Copies one array into the other
                    Array.Copy(buffer, recievedBuff, recievedSize);

                    //Converts the recieved byte array into a string
                    textRecieved = Encoding.ASCII.GetString(recievedBuff);

                    //The sent buff is equal to the recieved buff
                    sentBuff = recievedBuff;

                    //The clients sent count is equal to 0
                    clientSentCount = 0;

                    ////Loop until the loop is broken
                    //while (true)
                    //{
                    //    //If the current client is connected
                    //    if (clientSockets[clientSentCount].Connected)
                    //    {
                            //Begin sending the current client
                            clientSockets[clientSentCount].BeginSend(sentBuff, 0, sentBuff.Length, SocketFlags.None, new AsyncCallback(SendCallBack), null);

                    //        break;
                    //    }
                    //    //If the current client is not connected
                    //    else
                    //    {
                    //        //Add one to the amount of clients sent
                    //        clientSentCount++;
                    //    }
                    //}

                    //Begins recieving messages again
                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), socket);
                }
                //Catch any exceptions
                catch (Exception ex)
                {
                    //Display the exception
                    Console.WriteLine(ex.Message);
                }
            }
        }



        //Pre: The send message
        //Post: The message is sent to the client
        //Desc: A subprogram which sends  message to the client it's currently sending too
        private static void SendCallBack(IAsyncResult AR)
        {
            //Try doing this code
            try
            {
                //Add one to the amount of clients sent
                clientSentCount++;

                //If the current client sent count is less then the amount of clients there are
                if (clientSentCount < clientSockets.Count)
                {
                    //Begin sending the current client
                    clientSockets[clientSentCount].BeginSend(sentBuff, 0, sentBuff.Length, SocketFlags.None, new AsyncCallback(SendCallBack), null);
                }
            }
            //Catch any exceptions
            catch (Exception ex)
            {
                //Display the exception
                Console.WriteLine(ex.Message);
            }
        }
    }
}
