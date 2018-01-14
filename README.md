# Texas HoldEm Poker

A Texas HoldEm Poker game with features such as playing an AI and, playing against others on the same network. The project includes the client, the application for playing Texas HoldEm Poker, both against AI or with others on the same network, and the server which is used for the different clients to communicate within a network.

The network IP address is given to the client. As a result, if an external server IP is available, running the server application on the external server would allow different networks to play together as long as these networks can connect to the external server.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine.

### Prerequisites

In order to run the project, Visual Studio is required. A version of Visual Studio that is compatible with XNA 4.0 is required. 

```
Install Visual Studio using the following URL:
https://www.visualstudio.com/downloads/
```

In order to run the application, XNA 4.0 is required to be installed. 

```
Install XNA 4.0 for Visual Studio using the following URL:
https://mxa.codeplex.com/releases
```

If the above URL doesn't contain XNA 4.0 for the version of Visual Studio that you have, it means that XNA 4.0 is not officially supported for your version of Visual Studio. 

## Running Application

In order to run the application, a copy of the repository has to be cloned. After cloned, and XNA 4.0 is installed, the following exe needs to be run:
  
```
Client:
/Texas HoldEm Poker/Texas HoldEm Poker/bin/x86/Debug/Texas HoldEm Poker.exe
Server (if playing with others):
/Server/Multiple Clients Server/bin/Debug/Multiple Clients Server.exe
```

Once the client is ran, the Texas HoldEm Poker game will start and the game will be playable either against the AI or against other players using the server. 

Playing against the AI, the single player option has to be chosen on the main menu and then, a new game can be started or existing game loaded.

Playing with other players, the multi-player option has to be chosen and the server needs to be ran. Within the menu, the IP address and a username will be required. The IP address has to be the same as that of the server and will be shown on the server as soon as the server is ran. Once the client has the correct IP from the server and the client wrote in a username, clicking the connect button will connect the client to the server. Once the client and server are connected, the ability to ready up will be enabled within the client. For the game to begin, all the players that are connected have to be ready. Once all the players are ready, the game will begin and no new players will be able to join the current game. 
