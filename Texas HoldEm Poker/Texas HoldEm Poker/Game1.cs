/*
 * Author: Rony Verch
 * File Name: Texas HoldEm Poker.csproj
 * Project Name: Texas HoldEm Poker
 * Creation Date: December 17, 2014
 * Modified Date: January 18, 2015
 * Description: This is a Texas HoldEm Poker game with the ability to play against the Artificial Intelligece 
 *              or playing against your friends on the same network by connecting to a server
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Media;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Texas_HoldEm_Poker
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        /////////////////////////////////////////
        /////////////// Constants ///////////////
        /////////////////////////////////////////

        //Screen size
        const int SCREEN_WIDTH = 1200;
        const int SCREEN_HEIGHT = 900;

        //Button Constants
        const int FIRST_MENU_BUTTON = 0;
        const int SECOND_MENU_BUTTON = 1;
        const int THIRD_MENU_BUTTON = 2;
        const int FIRST_AI_PICK_BUTTON = 3;
        const int SECOND_AI_PICK_BUTTON = 4;
        const int THIRD_AI_PICK_BUTTON = 5;

        //Game states
        const int MAIN_MENU = 0;
        const int AI_GAME_TYPE = 1;
        const int AI_GAME = 2;
        const int PAUSE_GAME = 3;
        const int NETWORK_LOBBY = 4;
        const int NETWORK_GAME = 5;

        //Constants for the X start location of the chips for every player
        const int CHIP_START_1_X = 794;
        const int CHIP_START_2_X = 927;
        const int CHIP_START_3_X = 932;
        const int CHIP_START_4_X = 794;
        const int CHIP_START_5_X = 580;
        const int CHIP_START_6_X = 358;
        const int CHIP_START_7_X = 220;
        const int CHIP_START_8_X = 225;
        const int CHIP_START_9_X = 365;

        //Constants for the Y start location of the chips for every player
        const int CHIP_START_1_Y = 248;
        const int CHIP_START_2_Y = 319;
        const int CHIP_START_3_Y = 400;
        const int CHIP_START_4_Y = 475;
        const int CHIP_START_5_Y = 507;
        const int CHIP_START_6_Y = 477;
        const int CHIP_START_7_Y = 400;
        const int CHIP_START_8_Y = 318;
        const int CHIP_START_9_Y = 250;

        //Constants for the X and Y location of the main pile of chips
        const int CHIP_X_MAIN_STACK = 500;
        const int CHIP_Y_MAIN_STACK = 350;

        //Constant for the player numbers
        const int FIRST_PLAYER = 0;

        //Constant for zero chips
        const int ZERO_CHIPS = 0;

        //Constant for amount of time in a round
        const int ROUND_START_TIME = 20;

        //Constant for different amounts of players
        const int ONE_PLAYER = 1;
        const int NO_PLAYER = 0;

        //Constants for the amount of community cards there are
        const int NO_CARDS = 0;
        const int THREE_CARDS = 3;
        const int FIVE_CARDS = 5;

        /////////////////////////////////////////
        /////////////// Variables ///////////////
        /////////////////////////////////////////

        //Variables required for graphics
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Variables for the images of the main menu background and poker table
        Texture2D background;
        Texture2D pokerTable;

        //Variables for the card images 
        Texture2D[] deckOfCards = new Texture2D[52];
        Texture2D backOfCard;

        //Variables for the player seat images
        Texture2D emptySeat;
        Texture2D playerSeat;
        Texture2D currentPlayerSeat;

        //Variables for the poker chip images
        Texture2D whitePokerChip;
        Texture2D redPokerChip;
        Texture2D greenPokerChip;
        Texture2D bluePokerChip;
        Texture2D blackPokerChip;

        //Variables for the blind chips
        Texture2D bigBlindChip;
        Texture2D smallBlindChip;

        //Variable for drawing the rectangle of the button
        Texture2D buttons;

        //Variable for drawing the rectangle of the pause game background
        Texture2D pauseBackground;

        //Variable for the size of the background
        Rectangle backgroundSize;

        //Variables to store the button fonts
        SpriteFont menuBtnFont;
        SpriteFont menuBtnHoverFont;
        SpriteFont betBtnFont;
        SpriteFont betBtnHoverFont;
        SpriteFont playerInfoFont;
        SpriteFont pauseBtnFont;
        SpriteFont pauseBtnHoverFont;

        //Variables for text writers and readers which are required for saving and loading files
        TextWriter saveGameFile;
        TextReader loadGameFile;

        //Variable for the kayboard
        KeyboardState kb;

        //Variable for the background music
        Song backgroundMusic;

        //Variables for the different sound effects
        SoundPlayer usersTurnSound;

        //Variable for the new background worker needed to play and pause sound effects
        BackgroundWorker soundBW = new BackgroundWorker();

        //List for the size and location when drawing different buttons for the network lobby
        List<Rectangle> networkLobbyRec = new List<Rectangle>();

        //List for the size and location of the pause menu button
        List<Rectangle> pauseBtnRec = new List<Rectangle>();

        //List for the size and location of the player info ovals
        List<Rectangle> playerInfoRec = new List<Rectangle>();

        //List for the size and location of the main menu buttons 
        List<Rectangle> mainMenuRec = new List<Rectangle>();

        //List for the size and location of the main menu buttons which are hovering
        List<Rectangle> mainMenuHoverRec = new List<Rectangle>();

        //List for the size and location of the player's cards
        List<Rectangle> playerCardsRec = new List<Rectangle>();

        //Variable for if the background music is currently playing or not
        bool backgroundMusicPlaying = false;

        //Variable for if the sound effects for the user's turn was played
        bool usersTurnSoundPlayed = false;

        //Variable for the location of the save and load game file
        string saveFileLocation;

        //Variable for the current game state
        int gameState = MAIN_MENU;

        //Variables for the X and Y of the mouse
        int mouseX;
        int mouseY;

        //Variable that stores if the left mouse button is being held
        bool leftMouseHeld = false;

        //Variable that stores if a keyboard key is being held
        bool keyboardKeyHeld = false;

        //Variables for the size of the button
        int menuBtnXSize = 220;
        int menuBtnYSize = 90;

        //Variables for the size of the buttons when hovering over
        int menuHoverBtnXSize = 240;
        int menuHoverBtnYSize = 110;

        //Variables for the X and Y locations of the buttons
        int menuBtnXLocation = 490;
        int[] menuBtnYLocations = new int[6] { 160, 360, 560, 160, 360, 560 };

        //Variables for the X and Y locations for the buttons when hovering over them
        int menuHoverBtnXLocation = 480;
        int[] menuHoverBtnYLoactions = new int[6] { 150, 350, 550, 150, 350, 550 };

        //Array that stores if the buttons are currently being hovered over
        bool[] isMenuBtnHovering = new bool[6] { false, false, false, false, false, false };

        //Variables to store the size of each card
        int cardXSize = 60;
        int cardYSize = 75;

        //Variables for the X and Y locations of every card the players have
        int[] playerCardXLocations = new int[18] { 779, 842, 975, 1020, 1020, 1065, 760, 820, 537, 602, 309, 372, 229, 185, 230, 274, 380, 440 };
        int[] playerCardYLocations = new int[18] { 158, 175, 230, 275, 492, 448, 538, 518, 549, 549, 498, 518, 438, 395, 328, 285, 250, 229 };

        //Variables for the X and Y locations of every community card
        int communityCardYLocation = 250;
        int[] communityCardXLocations = new int[5] { 500, 560, 620, 680, 740 };

        //Variables for the sizes of the betting buttons
        int betBtnXSize = 160;
        int betBtnYSize = 40;

        //Variable for the size of the plus and minus buttons for when raising
        int raiseBtnSize = 65;

        //Variable for the X location of the minus buttons for when raising
        int minusBtnXLocation = 520;

        //Variables for the X and Y locations of the betting buttons
        int[] betBtnXLocation = new int[2] { 435, 615 };
        int[] betBtnYLocation = new int[3] { 740, 795, 850 };

        //Array for if the user is hovering over the bet buttons
        bool[] isBetBtnHovering = new bool[7] { false, false, false, false, false, false, false };

        //Variables for the size of the players information bar
        int playerInfoXSize = 150;
        int playerInfoYSize = 58;

        //Variables for the X and Y locations of the players information bars
        int[] playerInfoXLocation = new int[9] { 780, 1010, 1010, 785, 525, 260, 30, 30, 270 };
        int[] playerInfoYLocation = new int[9] { 100, 210, 500, 610, 650, 610, 500, 210, 100 };

        //Variables for the size of the pause button
        int pauseBtnXSize = 150;
        int pauseBtnYSize = 58;

        //Variables for the X and Y location of the pause button
        int pauseBtnXLocation = 120;
        int pauseBtnYLocation = 750;

        //Array variable for if the pause menu buttons are being hovered over
        bool[] isPauseMenuBtnHovering = new bool[3] { false, false, false };

        //Variable for if the user is hovering over the pause button
        bool isPauseBtnHovering = false;

        //Variables for the size of the connect to server button
        int connectBtnXSize = 170;
        int connectBtnYSize = 50;

        //Variables for the X and Y location of the connect to server button
        int connectBtnXLocation = 320;
        int connectBtnYLocation = 80;

        //Variable for if the connect button is being hovered over
        bool isConnectBtnHovering = false;

        //Variable for if the connect button was pressed
        bool isConnectBtnPressed = false;

        //Variables for the size of the box where the ip is typed into
        int ipAddressBoxXSize = 300;
        int ipAddressBoxYSize = 50;

        //Variables for the X and Y location of the box where the ip is typed into
        int ipAddressBoxXLocation = 10;
        int ipAddressBoxYLocation = 80;

        //Variable for if the user is typing the ip address
        bool isIpAddressTyping = false;

        //Variables for the size of the box where the name is typed into
        int nameBoxXSize = 300;
        int nameBoxYSize = 50;

        //Variables for the X and Y location of the box where the name is typed into
        int nameBoxXLocation = 10;
        int nameBoxYLocation = 140;

        //Variable for if the user is typing the name
        bool isNameTyping = false;

        //Variables for the size of the ready to start button
        int readyBtnXSize = 170;
        int readyBtnYSize = 50;

        //Variables for the X and Y location of the ready to start button
        int readyBtnXLocation = 320;
        int readyBtnYLocation = 140;

        //Variable for if the mouse is hovering over the ready button
        bool isReadyBtnHovering = false;

        //Variable for the size of the table where all the player names will be written in
        int namesTableXSize = 600;
        int namesTableYSize = 600;

        //Variables for the X and Y location of the names table
        int namesTableXLocation = 50;
        int namesTableYLocation = 240;

        //Variables for the X and Y size of each poker chip
        int pokerChipXSize = 40;
        int pokerChipYSize = 40;

        //Variable that stores the newly shuffled deck of cards
        int[] shuffleCards = new int[52];
        Random randInt = new Random();

        //Variable for the next avaialable card in the deck
        int nextCardInDeck = 0;

        //Variable for the amount of community cards currently shown
        int communityCardCount;

        //Variables for which players are currently left and how many players are left in total
        int playersLeft;
        bool[] playerPlaying = new bool[9] { true, true, true, true, true, true, true, true, true };

        //Variable for which player the user is controlling
        int userPlayer = 4;

        //Variable for if the player the user is controlling folded
        bool[] playersFolded = new bool[9] { false, false, false, false, false, false, false, false, false };

        //Variable for which player's turn it currently is
        int currentTurn = 0;

        //Variable for which player's are small and big blinds
        int smallBlindPlayer = 0;
        int bigBlindPlayer = 0;

        //Variable for the current Blind amount
        int blindAmount = 10;

        //Variable for the amount of turns that passed since the blinds were raised
        int turnCount = 0;

        //Lists for the total amount of money in each pot and which players can win that pot
        List<int> chipPots = new List<int>();
        List<bool[]> playersCanWin = new List<bool[]>();

        //Variables for the amount each player has to call to and the highest current bet
        int currentBet = 60;

        //Variable for the amount the user wants to currently raise to
        int userRaiseAmount = 0;

        //Variable for the amount each player gets from that hand, if one winner then it is equal to the total pot otherwise, gets split
        int currentPayOut = 0;

        //Variables for which player won 
        int playerWon = 0;

        //Variable for the amount of players which tied
        int playersTiedCount = 0;

        //List for which players tied to the player who won
        List<int> playersTied = new List<int>();

        //Variable for the total amount of money each player has and their current bets
        int[] playerMoney = new int[9] { 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000 };
        int[] playerBet = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        //Boolean variables for if the poker game was started and if a new round is needed to be set up
        bool newRound = false;
        bool gameStarted = false;

        //Boolean variable for if a new game was started and needs to bet set up
        bool newGame = false;

        //Variables for the amount of time the player has left to finish deciding
        Stopwatch playerTime = new Stopwatch();
        int timeLeft;
        int oldTimeLeft;

        //Variable for the amount of turns there was after the community cards were placed
        int playersTurnCount = 0;

        //Variable for the amount of time the AI will pretend to be thinking as it was a real person
        int timeAI;

        //Variables for flush and straight draws
        bool openEndedstraightDraw = false;
        bool insideStraightDraw = false;
        bool flushDraw = false;

        //Variables for when a player goes all in
        bool[] playersAllIn = new bool[9] { false, false, false, false, false, false, false, false, false };
        int playersAllInCount = 0;

        //Arrays for players bluffinh
        bool[] playersBluffChecked = new bool[9] { false, false, false, false, false, false, false, false, false };
        bool[] playersBluff = new bool[9] { false, false, false, false, false, false, false, false, false };

        //A 3Dimensional Array for the cards each player has, the first index represents the player number, the second index represents
        //The first or second card, and the third index represents the suit for the first or second card
        int[, ,] playerCards = new int[9, 2, 2];

        //An array for the value and suit of each community card
        int[,] communityCards = new int[5, 2];

        //Arrays for the quality of each player's hand and there kickers
        int[,] handQuality = new int[9, 3];
        int[] handKicker = new int[9];

        //List for the possible names the players can have
        List<string> possiblePlayerNames = new List<string>();

        //Array for the name that each AI player now has
        string[] playerNames = new string[9];

        //Variable for the ip address of the server
        string serverIP = "";

        //Variable for the socket of the client connecting to the server
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //Variable for if the client connected
        bool clientConnected = false;

        //Boolean variable for if the multiplayer game is currently in the lobby
        bool gameLobby = true;

        //Boolean for if the client tried connecting to the server
        bool connectAttempt = false;

        //Variable for the current client name
        string currentClientName = "";

        //List for the names of every client
        List<string> clientNames = new List<string>();

        //Array for which clients are currently ready to start the game
        bool[] clientsReady = new bool[9] { false, false, false, false, false, false, false, false, false };

        //Variable for if this client is ready to start the game
        bool currentClientReady = false;

        //Array of bytes where the recieved data is sent into
        byte[] recieveBuffer;

        //Variable for if the client has not entered a name
        bool noClientName = false;

        //Variable for if the client has entered a name but it's less then 3 letters
        bool clientNameTooShort = false;

        //Variable for the amount of clients which are currently ready to start
        int clientsReadyCount = 0;

        //Boolean variable for when a message needs to be send to the server with all the game info
        bool sendToServerNeeded = false;

        //Variable for if the message was already sent
        bool messageSent = true;
        bool replyRecieved = true;

        //Array of booleans for which players currently have there chips pushed
        bool[] playerChipsMoving = new bool[9] { false, false, false, false, false, false, false, false, false };

        //Variable for when all the chips need to join together
        bool playerChipsNeedJoin = false;

        //Variable for when all the chips joined together
        bool playerChipsJoined = true;

        //Variable for the rate of which the chips are moving
        int playerChipsXMoveRate = 0;
        int playerChipsYMoveRate = 0;

        //Variables for the X and Y location of the chips as they are moving
        int[] playerChipsXLocations = new int[9];
        int[] playerChipsYLocations = new int[9];

        //Variables for the max X and Y location of the chips which are moving
        int playerChipsMaxXLocation = 0;
        int playerChipsMaxYLocation = 0;

        //Variable for if the last player was checked in the current round
        bool lastPlayerCheck = false;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Sets the size of the XNA window
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;

            //Sets the mouse to be visible 
            IsMouseVisible = true;
        }



        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            //Creates the background worker which will be used for the sound
            soundBW.DoWork += new DoWorkEventHandler(soundBWDoWork);
            soundBW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(soundBWRunWorkerCompleted);
            soundBW.WorkerReportsProgress = true;
            soundBW.WorkerSupportsCancellation = true;
        }



        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //Loads the main menu background and poker table for the game
            background = this.Content.Load<Texture2D>("Background\\Main Background");
            pokerTable = this.Content.Load<Texture2D>("Background\\Poker Table V2");

            //Loads the picture of the back of the cards
            backOfCard = this.Content.Load<Texture2D>("Cards\\Back of Cards");

            //Loads the images for each card
            for (int i = 1; i <= 52; i++)
            {
                deckOfCards[i - 1] = this.Content.Load<Texture2D>("Cards\\Card" + i);
            }

            //Loads all the fonts for the games
            menuBtnFont = this.Content.Load<SpriteFont>("Text Fonts\\Menu Button Font");
            menuBtnHoverFont = this.Content.Load<SpriteFont>("Text Fonts\\Menu Button Hover Font");
            betBtnFont = this.Content.Load<SpriteFont>("Text Fonts\\Bet Button Font");
            betBtnHoverFont = this.Content.Load<SpriteFont>("Text Fonts\\Bet Button Hover Font");
            playerInfoFont = this.Content.Load<SpriteFont>("Text Fonts\\Player Info Font");
            pauseBtnFont = this.Content.Load<SpriteFont>("Text Fonts\\Pause Button Font");
            pauseBtnHoverFont = this.Content.Load<SpriteFont>("Text Fonts\\Pause Button Hover Font");

            //Loads the images for the different player seats
            emptySeat = this.Content.Load<Texture2D>("Player Info\\Empty Seat");
            playerSeat = this.Content.Load<Texture2D>("Player Info\\Player Seat");
            currentPlayerSeat = this.Content.Load<Texture2D>("Player Info\\Current Player Seat");

            //Loads the images for all the poker chips
            whitePokerChip = this.Content.Load<Texture2D>("Poker Chips\\White Poker Chip");
            redPokerChip = this.Content.Load<Texture2D>("Poker Chips\\Red Poker Chip");
            greenPokerChip = this.Content.Load<Texture2D>("Poker Chips\\Green Poker Chip");
            bluePokerChip = this.Content.Load<Texture2D>("Poker Chips\\Blue Poker Chip");
            blackPokerChip = this.Content.Load<Texture2D>("Poker Chips\\Black Poker Chip");

            //Loads the images for the blind chips
            bigBlindChip = this.Content.Load<Texture2D>("Blind Chips\\Big Blind");
            smallBlindChip = this.Content.Load<Texture2D>("Blind Chips\\Small Blind");

            //Sets the location of the save txt
            saveFileLocation = "Save Game\\Save Game File.txt";

            //Loads the music and sound effects for the game
            backgroundMusic = Content.Load<Song>("Audio\\Poker Background Music");
            usersTurnSound = new SoundPlayer("Audio\\Sound Effects\\Current Turn.wav");

            //Sets a new Texture2D for the buttons and gives it data
            buttons = new Texture2D(GraphicsDevice, 1, 1);
            buttons.SetData(new Color[] { Color.White });

            //Sets a new Texture2D for the pause background and gives it data
            pauseBackground = new Texture2D(GraphicsDevice, 1, 1);
            pauseBackground.SetData(new Color[] { Color.White });

            //Opens the Names.txt file to load all the names from it
            loadGameFile = new StreamReader("Names.txt");

            //Reads the first line to see the amount of names saved in the file
            int namesAmount = Convert.ToInt32(loadGameFile.ReadLine());

            //Loops through every name saved in the file
            for (int i = 0; i < namesAmount; i++)
            {
                //Adds each name to the possible player names list
                possiblePlayerNames.Add(loadGameFile.ReadLine().Trim());
            }

            //Closes the file where all the names are saved
            loadGameFile.Close();

            //Set the size of the background
            backgroundSize = new Rectangle(graphics.GraphicsDevice.Viewport.X, graphics.GraphicsDevice.Viewport.Y,
                graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);

            //Set the sizes and locations of the network lobby buttons
            networkLobbyRec.Add(new Rectangle(connectBtnXLocation, connectBtnYLocation, connectBtnXSize, connectBtnYSize));
            networkLobbyRec.Add(new Rectangle(connectBtnXLocation + 4, connectBtnYLocation + 4, connectBtnXSize - 8, connectBtnYSize - 8));
            networkLobbyRec.Add(new Rectangle(connectBtnXLocation - 6, connectBtnYLocation - 6, connectBtnXSize + 12, connectBtnYSize + 12));
            networkLobbyRec.Add(new Rectangle(connectBtnXLocation - 2, connectBtnYLocation - 2, connectBtnXSize + 4, connectBtnYSize + 4));
            networkLobbyRec.Add(new Rectangle(connectBtnXLocation, connectBtnYLocation, connectBtnXSize, connectBtnYSize));
            networkLobbyRec.Add(new Rectangle(connectBtnXLocation + 4, connectBtnYLocation + 4, connectBtnXSize - 8, connectBtnYSize - 8));
            networkLobbyRec.Add(new Rectangle(ipAddressBoxXLocation, ipAddressBoxYLocation, ipAddressBoxXSize, ipAddressBoxYSize));
            networkLobbyRec.Add(new Rectangle(ipAddressBoxXLocation + 4, ipAddressBoxYLocation + 4, ipAddressBoxXSize - 8, ipAddressBoxYSize - 8));
            networkLobbyRec.Add(new Rectangle(ipAddressBoxXLocation, ipAddressBoxYLocation, ipAddressBoxXSize, ipAddressBoxYSize));
            networkLobbyRec.Add(new Rectangle(ipAddressBoxXLocation + 4, ipAddressBoxYLocation + 4, ipAddressBoxXSize - 8, ipAddressBoxYSize - 8));
            networkLobbyRec.Add(new Rectangle(nameBoxXLocation, nameBoxYLocation, nameBoxXSize, nameBoxYSize));
            networkLobbyRec.Add(new Rectangle(nameBoxXLocation + 4, nameBoxYLocation + 4, nameBoxXSize - 8, nameBoxYSize - 8));
            networkLobbyRec.Add(new Rectangle(nameBoxXLocation, nameBoxYLocation, nameBoxXSize, nameBoxYSize));
            networkLobbyRec.Add(new Rectangle(nameBoxXLocation + 4, nameBoxYLocation + 4, nameBoxXSize - 8, nameBoxYSize - 8));
            networkLobbyRec.Add(new Rectangle(readyBtnXLocation - 6, readyBtnYLocation - 6, readyBtnXSize + 12, readyBtnYSize + 12));
            networkLobbyRec.Add(new Rectangle(readyBtnXLocation - 2, readyBtnYLocation - 2, readyBtnXSize + 4, readyBtnYSize + 4));
            networkLobbyRec.Add(new Rectangle(readyBtnXLocation, readyBtnYLocation, readyBtnXSize, readyBtnYSize));
            networkLobbyRec.Add(new Rectangle(readyBtnXLocation + 4, readyBtnYLocation + 4, readyBtnXSize - 8, readyBtnYSize - 8));
            networkLobbyRec.Add(new Rectangle(readyBtnXLocation, readyBtnYLocation, readyBtnXSize, readyBtnYSize));
            networkLobbyRec.Add(new Rectangle(readyBtnXLocation + 4, readyBtnYLocation + 4, readyBtnXSize - 8, readyBtnYSize - 8));
            networkLobbyRec.Add(new Rectangle(namesTableXLocation, namesTableYLocation, namesTableXSize, namesTableYSize));
            networkLobbyRec.Add(new Rectangle(namesTableXLocation + 4, namesTableYLocation + 4, namesTableXSize - 8, namesTableYSize - 8));
            networkLobbyRec.Add(new Rectangle(namesTableXLocation + Convert.ToInt32(namesTableXSize * 0.65) - 2, namesTableYLocation, 4, namesTableYSize));

            //Loops through every player that could be join to the game to draw a table with all the names
            for (int i = 1; i < clientsReady.Length + 1; i++)
            {
                networkLobbyRec.Add(new Rectangle(namesTableXLocation, namesTableYLocation + 60 * i, namesTableXSize, 4));
            }

            //Sets the size and location of the pause button
            pauseBtnRec.Add(new Rectangle(pauseBtnXLocation - 6, pauseBtnYLocation - 6, pauseBtnXSize + 12, pauseBtnYSize + 12));
            pauseBtnRec.Add(new Rectangle(pauseBtnXLocation - 4, pauseBtnYLocation - 4, pauseBtnXSize + 8, pauseBtnYSize + 8));
            pauseBtnRec.Add(new Rectangle(pauseBtnXLocation, pauseBtnYLocation, pauseBtnXSize, pauseBtnYSize));
            pauseBtnRec.Add(new Rectangle(pauseBtnXLocation + 2, pauseBtnYLocation + 2, pauseBtnXSize - 4, pauseBtnYSize - 4));

            //Sets the size and location of all the player info ovals using a loop that loops through every single seat that a player can possibly be seating in
            for (int i = 0; i < playerPlaying.Length; i++)
            {
                playerInfoRec.Add(new Rectangle(playerInfoXLocation[i], playerInfoYLocation[i], playerInfoXSize, playerInfoYSize));
            }

            //Sets the size and location of the main menu buttons using a loop that loops through every button 
            for (int i = FIRST_MENU_BUTTON; i < isMenuBtnHovering.Length; i++)
            {
                mainMenuHoverRec.Add(new Rectangle(menuHoverBtnXLocation, menuHoverBtnYLoactions[i], menuHoverBtnXSize, menuHoverBtnYSize));
                mainMenuRec.Add(new Rectangle(menuBtnXLocation, menuBtnYLocations[i], menuBtnXSize, menuBtnYSize));
            }

            //Sets the size and location of the main menu button borders using a loop that loops through every button 
            for (int i = FIRST_MENU_BUTTON; i < isMenuBtnHovering.Length; i++)
            {
                mainMenuHoverRec.Add(new Rectangle(menuHoverBtnXLocation + 7, menuHoverBtnYLoactions[i] + 7, menuHoverBtnXSize - 14, menuHoverBtnYSize - 14));
                mainMenuRec.Add(new Rectangle(menuBtnXLocation + 7, menuBtnYLocations[i] + 7, menuBtnXSize - 14, menuBtnYSize - 14));
            }

            //Sets the size and location of the cards for every player using a for loop that loops once for each card
            for (int i = 0; i < playerPlaying.Length * 2; i++)
            {
                playerCardsRec.Add(new Rectangle(playerCardXLocations[i], playerCardYLocations[i], cardXSize, cardYSize));
            }
        }



        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }



        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            kb = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

            //Gets the X and Y values for the mouse
            mouseX = Mouse.GetState().X;
            mouseY = Mouse.GetState().Y;

            // TODO: Add your update logic here
            //Checks if the left mouse button is not being held
            if (Mouse.GetState().LeftButton == ButtonState.Released && leftMouseHeld == true)
            {
                //Sets the mouse being held variable to false
                leftMouseHeld = false;
            }

            //Checks if a keyboard key variable is true
            if (keyboardKeyHeld == true)
            {
                //Checks if all this keys are not being held anymore
                if (kb.IsKeyUp(Keys.NumPad0) && kb.IsKeyUp(Keys.D0) && kb.IsKeyUp(Keys.NumPad1) && kb.IsKeyUp(Keys.D1) &&
                    kb.IsKeyUp(Keys.NumPad2) && kb.IsKeyUp(Keys.D2) && kb.IsKeyUp(Keys.NumPad3) && kb.IsKeyUp(Keys.D3) &&
                    kb.IsKeyUp(Keys.NumPad4) && kb.IsKeyUp(Keys.D4) && kb.IsKeyUp(Keys.NumPad5) && kb.IsKeyUp(Keys.D5) &&
                    kb.IsKeyUp(Keys.NumPad6) && kb.IsKeyUp(Keys.D6) && kb.IsKeyUp(Keys.NumPad7) && kb.IsKeyUp(Keys.D7) &&
                    kb.IsKeyUp(Keys.NumPad8) && kb.IsKeyUp(Keys.D8) && kb.IsKeyUp(Keys.NumPad9) && kb.IsKeyUp(Keys.D9) &&
                    kb.IsKeyUp(Keys.OemPeriod) && kb.IsKeyUp(Keys.Back) && kb.IsKeyUp(Keys.Q) && kb.IsKeyUp(Keys.W) &&
                    kb.IsKeyUp(Keys.E) && kb.IsKeyUp(Keys.R) && kb.IsKeyUp(Keys.T) && kb.IsKeyUp(Keys.Y) && kb.IsKeyUp(Keys.U) &&
                    kb.IsKeyUp(Keys.I) && kb.IsKeyUp(Keys.O) && kb.IsKeyUp(Keys.P) && kb.IsKeyUp(Keys.A) && kb.IsKeyUp(Keys.S) &&
                    kb.IsKeyUp(Keys.D) && kb.IsKeyUp(Keys.F) && kb.IsKeyUp(Keys.G) && kb.IsKeyUp(Keys.H) && kb.IsKeyUp(Keys.J) &&
                    kb.IsKeyUp(Keys.K) && kb.IsKeyUp(Keys.L) && kb.IsKeyUp(Keys.Z) && kb.IsKeyUp(Keys.X) && kb.IsKeyUp(Keys.C) &&
                    kb.IsKeyUp(Keys.V) && kb.IsKeyUp(Keys.B) && kb.IsKeyUp(Keys.N) && kb.IsKeyUp(Keys.M))
                {
                    //Sets the keyboard key variable to false
                    keyboardKeyHeld = false;
                }
            }

            //Switch statement for the current game state
            switch (gameState)
            {
                //If the game state is currently on main menu
                case MAIN_MENU:
                    //If the background music playing boolean is set to false
                    if (backgroundMusicPlaying == false)
                    {
                        //Set the boolean to true
                        backgroundMusicPlaying = true;

                        //Start playing the background music and set it to repeat
                        MediaPlayer.Play(backgroundMusic);
                        MediaPlayer.IsRepeating = true;
                    }

                    //Loops through the buttons of the first menu
                    for (int i = FIRST_MENU_BUTTON; i < FIRST_AI_PICK_BUTTON; i++)
                    {
                        //Checks if the X coordinate of the mouse is on the button
                        if (mouseX <= menuBtnXLocation + menuBtnXSize && mouseX >= menuBtnXLocation)
                        {
                            //Checks if the Y coordinate of the mouse is on the current button
                            if (mouseY >= menuBtnYLocations[i] && mouseY <= menuBtnYLocations[i] + menuBtnYSize)
                            {
                                //Sets the boolean to true that says that the mouse is currently hovering over the menu button
                                isMenuBtnHovering[i] = true;

                                //Checks if the user is pressing the left mouse button
                                if (Mouse.GetState().LeftButton == ButtonState.Pressed && leftMouseHeld == false)
                                {
                                    //Sets the left mouse being held variable to true
                                    leftMouseHeld = true;

                                    //Switch statement for the different possible menu buttons
                                    switch (i)
                                    {
                                        //Checks if it's the first button
                                        case FIRST_MENU_BUTTON:
                                            //Sets the game state to type of AI Game and resets the game
                                            gameState = AI_GAME_TYPE;
                                            break;
                                        //Checks if it's the second button
                                        case SECOND_MENU_BUTTON:
                                            //Sets the game state to network lobby
                                            gameState = NETWORK_LOBBY;

                                            //Checks for the server IP and saves it in the serverIP variable
                                            serverIP = GetLocalIP();
                                            break;
                                        //Checks if it's the third button
                                        case THIRD_MENU_BUTTON:
                                            //Exits the game
                                            this.Exit();
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                //Sets the mouse to not currently hovering over the menu button
                                isMenuBtnHovering[i] = false;
                            }
                        }
                        else
                        {
                            //Sets the mouse to not currently hovering over the menu button
                            isMenuBtnHovering[i] = false;
                        }
                    }
                    break;
                //If the game state is currently on type of AI Game menu
                case AI_GAME_TYPE:
                    //Loops through all the buttons for the type of AI Game Menu
                    for (int i = FIRST_AI_PICK_BUTTON; i < isMenuBtnHovering.Length; i++)
                    {
                        //Checks if the X coordinate of the mouse is on the button
                        if (mouseX <= menuBtnXLocation + menuBtnXSize && mouseX >= menuBtnXLocation)
                        {
                            //Checks if the Y coordinate of the mouse is on the current button
                            if (mouseY >= menuBtnYLocations[i] && mouseY <= menuBtnYLocations[i] + menuBtnYSize)
                            {
                                //Sets the boolean to true that says that the mouse is currently hovering over the menu button
                                isMenuBtnHovering[i] = true;

                                //Checks if the user is pressing the left mouse button
                                if (Mouse.GetState().LeftButton == ButtonState.Pressed && leftMouseHeld == false)
                                {
                                    //Sets the left mouse being held variable to true
                                    leftMouseHeld = true;

                                    //Switch statement for the different possible menu buttons
                                    switch (i)
                                    {
                                        //Checks if it's the first button
                                        case FIRST_AI_PICK_BUTTON:
                                            //Sets the game state to AI Game and resets the game
                                            gameState = AI_GAME;
                                            newGame = true;
                                            break;
                                        //Checks if it's the second button
                                        case SECOND_AI_PICK_BUTTON:
                                            //Sets the game state to AI Game and loads the game
                                            gameState = AI_GAME;
                                            LoadGame();
                                            break;
                                        //Checks if it's the third button
                                        case THIRD_AI_PICK_BUTTON:
                                            //Sets the game state back to main menu
                                            gameState = MAIN_MENU;
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                //Sets the mouse to not currently hovering over the menu button
                                isMenuBtnHovering[i] = false;
                            }
                        }
                        else
                        {
                            //Sets the mouse to not currently hovering over the menu button
                            isMenuBtnHovering[i] = false;
                        }
                    }
                    break;
                //If the game state is currently on AI Game or Network Game
                case AI_GAME:
                case NETWORK_GAME:
                    //Variable for the amount of players who called
                    int playersCalledCount = 0;

                    //Variable for the maximum all in amount
                    int maxAllInAmount = 0;

                    //Variable for the maximum a player can win
                    int maxPlayerWin = 0;

                    //Variable for when all the pots are decided and set
                    bool potsDone = false;

                    //Variable for the amount of players which folded
                    int playersFoldedCount = 0;

                    //If the game state is not on network game
                    if (gameState != NETWORK_GAME)
                    {
                        //Checks the location of the poker chips of the players and checks if they need to be moved
                        CheckMoneyChipLocation();
                    }

                    //If the current turn is equal to the user and the user did not go all in
                    if (currentTurn == userPlayer && playersAllIn[currentTurn] == false)
                    {
                        //If the sound effect was not played yet
                        if (usersTurnSoundPlayed == false)
                        {
                            //Set the sound effect to played
                            usersTurnSoundPlayed = true;

                            //Check if the background worker is not currently doing anything
                            if (!soundBW.IsBusy)
                            {
                                //Run the background worker to play the sound
                                soundBW.RunWorkerAsync();
                            }
                        }
                    }
                    //If the current turn is not equal to userplayer and or the user did go all in
                    else
                    {
                        //Set the sound effect to not played
                        usersTurnSoundPlayed = false;
                    }

                    //If the game state is current on network game
                    if (gameState == NETWORK_GAME)
                    {
                        //If the user's player number is equal to first player
                        if (userPlayer == FIRST_PLAYER)
                        {
                            //If the game has not started yet and a send to server is not needed
                            if (gameStarted == false && sendToServerNeeded == false)
                            {
                                //Resets the game
                                ResetGame();

                                //Sets the send to server to needed and sets the reply to recieved
                                sendToServerNeeded = true;
                                replyRecieved = true;
                            }
                        }
                    }
                    //If the game state is not currently on network game
                    else
                    {
                        //Checks if it's a new game
                        if (newGame == true)
                        {
                            //Resets the game
                            ResetGame();
                        }
                    }

                    //If the game state is currently on network game
                    if (gameState == NETWORK_GAME)
                    {
                        //If the user's player number is equal to first player
                        if (userPlayer == FIRST_PLAYER)
                        {
                            //If a new round has started and a send to server is not needed
                            if (newRound == true && sendToServerNeeded == false)
                            {
                                //Resets the round
                                ResetRound();

                                //Sets the send to server to needed and sets the reply to recieved
                                sendToServerNeeded = true;
                                replyRecieved = true;
                            }
                        }
                    }
                    //If the game state is not currently on network game
                    else
                    {
                        //If a new round has started
                        if (newRound == true)
                        {
                            //Resets the round
                            ResetRound();
                        }
                    }

                    //If the current turn is the user's
                    if (currentTurn == userPlayer)
                    {
                        //If the minimum raise amount is equal to 0
                        if (userRaiseAmount == ZERO_CHIPS)
                        {
                            //Sets the minimum raise amount to the current bet plus a blind
                            userRaiseAmount = currentBet + blindAmount;
                        }
                    }
                    //If the current turn is not the user's
                    else
                    {
                        //Sets the minimum raise amount to 0
                        userRaiseAmount = ZERO_CHIPS;
                    }

                    //If the game state is currently at network game
                    if (gameState == NETWORK_GAME)
                    {
                        //If the current turn is not the user's
                        if (currentTurn != userPlayer)
                        {
                            //Stop the timer
                            playerTime.Stop();
                        }
                        //If the current turn is the user's
                        else if (currentTurn == userPlayer)
                        {
                            //Check if the timer is not running
                            if (!playerTime.IsRunning)
                            {
                                //If the current player is all in
                                if (playersAllIn[currentTurn] == true)
                                {
                                    //Set the time left to 0 and old time left to 20
                                    timeLeft = 0;
                                    oldTimeLeft = ROUND_START_TIME;
                                }
                                //If the current player is not all in
                                else
                                {
                                    //Restart the timer and set both the time left and old time left to 20
                                    playerTime.Restart();
                                    timeLeft = ROUND_START_TIME;
                                    oldTimeLeft = ROUND_START_TIME;
                                }
                            }
                        }
                    }

                    //Checks what's the highest all in amount and saves it in the variable
                    maxAllInAmount = HighestPlayerMoney();

                    //If the game state is network game
                    if (gameState == NETWORK_GAME)
                    {
                        //If the timer is running and the current turn is the user's
                        if (playerTime.IsRunning == true && currentTurn == userPlayer)
                        {
                            //Set the time left to 20 subtracted by the amount of time that passed
                            timeLeft = ROUND_START_TIME - playerTime.Elapsed.Seconds;

                            //if the old time left is not equal to the current time left
                            if (oldTimeLeft != timeLeft)
                            {
                                //Sets the old time left to the current time left
                                oldTimeLeft = timeLeft;
                            }
                        }
                    }
                    //If the game state is not a network game
                    else
                    {
                        //Set's the time left to 20 subtracted by the amount of time that passed
                        timeLeft = ROUND_START_TIME - playerTime.Elapsed.Seconds;
                    }

                    //Calls the player betting options subprogram to check what the user want's to do if it's their turn
                    PlayerBettingOptions(maxAllInAmount);

                    //If the game state is not equal to a network game
                    if (gameState != NETWORK_GAME)
                    {
                        //Calls the check pause button subprogram to check if the pause button was pressed or being hovered over by the user
                        CheckPauseButton();
                    }

                    //Check's which players are all in and how many are all in
                    playersAllIn = PlayersAllIn();
                    playersAllInCount = AmountPlayersAllIn();

                    //Checks how many players folded
                    playersFoldedCount = CheckPlayersFoldedCount();

                    //Checks how many players called
                    playersCalledCount = CheckPlayersCalledCount();

                    //If the game state is currently at network game
                    if (gameState == NETWORK_GAME)
                    {
                        //If it's the user's turn
                        if (currentTurn == userPlayer)
                        {
                            //Call the time subprogram to check how much time is left for the current turn
                            CheckPlayerTime(playersFoldedCount);
                        }
                    }
                    else
                    {
                        //Call the time subprogram to check how much time is left for the current turn
                        CheckPlayerTime(playersFoldedCount);
                    }

                    //If the game state is currently at network game
                    if (gameState == NETWORK_GAME)
                    {
                        //If the amount of players turns is greater then or equal to the amount of players left and the current turn is the user's
                        if (playersTurnCount >= playersLeft && currentTurn == userPlayer)
                        {
                            //Send to server is needed
                            sendToServerNeeded = true;
                        }
                    }

                    //If the game state is AI Game or the game state is network game and the current user is the first player and the amount of players that called is equal 
                    //to the amount of players left minus the amount of players that folded
                    if (gameState == AI_GAME || gameState == NETWORK_GAME && userPlayer == FIRST_PLAYER && playersCalledCount == playersLeft - playersFoldedCount)
                    {
                        //Checks which players are all in and the amount of players that are all in
                        playersAllIn = PlayersAllIn();
                        playersAllInCount = AmountPlayersAllIn();

                        //if the amount of players folded is equal to the players left subtracted one
                        if (playersFoldedCount == playersLeft - ONE_PLAYER)
                        {
                            //If the game state is currently on network game
                            if (gameState == NETWORK_GAME)
                            {
                                //If the current turn is the first player's turn
                                if (currentTurn == FIRST_PLAYER)
                                {
                                    //Send to server is needed
                                    sendToServerNeeded = true;

                                    //If a last player check was not done
                                    if (lastPlayerCheck == false)
                                    {
                                        //Do the last player check
                                        CheckLastPlayer();

                                        //Set the last player check to be done
                                        lastPlayerCheck = true;
                                    }
                                }
                            }
                            //If the game state is current not on network game
                            else
                            {
                                //If a last player check was not done
                                if (lastPlayerCheck == false)
                                {
                                    //Do the last player check
                                    CheckLastPlayer();

                                    //Set the last player check to be done
                                    lastPlayerCheck = true;
                                }
                            }
                        }
                        //if the amount of players folded is not equal to the players left subtracted one
                        else
                        {
                            //If the amount of turns is greater then or equal the amount of players left
                            if (playersTurnCount >= playersLeft)
                            {
                                //If the amount of players that called is equal to the amount of players left or the amount of players called is equal to the amount of players 
                                //left subtracted by the amount of players that folded and the amount of players that called is not equal to 0
                                if (playersCalledCount == playersLeft || playersCalledCount == playersLeft - playersFoldedCount && playersCalledCount != NO_PLAYER)
                                {
                                    //if the game state is not on network game
                                    if (gameState != NETWORK_GAME)
                                    {
                                        //If the chips don't need to join and they already have joined
                                        if (playerChipsNeedJoin == false && playerChipsJoined == true)
                                        {
                                            //If the current bet is greater then 0
                                            if (currentBet > ZERO_CHIPS)
                                            {
                                                //Sets the chips to need to join and that they have not joined yet
                                                playerChipsNeedJoin = true;
                                                playerChipsJoined = false;

                                                //Set the speed and locations of the chips
                                                SetChipMovingValues(currentTurn);
                                            }
                                            //If the current bet is not greater then 0
                                            else
                                            {
                                                //Set the player chips to be joined
                                                playerChipsJoined = true;
                                            }
                                        }
                                    }

                                    //If the player chips are joined together or the game state is a network game
                                    if (playerChipsJoined == true || gameState == NETWORK_GAME)
                                    {
                                        //If the game state is not a network game
                                        if (gameState != NETWORK_GAME)
                                        {
                                            //Sets the chips to joined and and that they don't need to join anymore
                                            playerChipsJoined = true;
                                            playerChipsNeedJoin = false;

                                            //Loop for every player's chip
                                            for (int i = NO_PLAYER; i < playerChipsMoving.Length; i++)
                                            {
                                                //Sets the current player's chips to not moving
                                                playerChipsMoving[i] = false;
                                            }
                                        }

                                        //Sets the amount of turns that occured to 0
                                        playersTurnCount = NO_PLAYER;

                                        //Loop while the pots are not done with
                                        while (potsDone == false)
                                        {
                                            //If the chip pots count is equal to zero
                                            if (chipPots.Count == ZERO_CHIPS)
                                            {
                                                //Add a pot
                                                chipPots.Add(ZERO_CHIPS);
                                                playersCanWin.Add(new bool[playerPlaying.Length]);
                                            }

                                            //Sets the max amount a player can win
                                            maxPlayerWin = maxAllInAmount;

                                            //Loop for every single player
                                            for (int j = NO_PLAYER; j < playerBet.Length; j++)
                                            {
                                                //If the max the player can win is greater then the bet of the current player and the bet of the current player is not equal to 0
                                                //and the current player has not folded and is still playing
                                                if (maxPlayerWin > playerBet[j] && playerBet[j] != ZERO_CHIPS && playersFolded[j] == false && playerPlaying[j] == true)
                                                {
                                                    //Sets the max the player can win to the current bet
                                                    maxPlayerWin = playerBet[j];
                                                }
                                            }

                                            //Loop for every single player
                                            for (int j = NO_PLAYER; j < playerBet.Length; j++)
                                            {
                                                //if the current player has not folded and is still playing
                                                if (playersFolded[j] == false && playerPlaying[j] == true)
                                                {
                                                    //If the bet of the current player is greater then or equal to the max a player can win and the max is not equal to 0
                                                    if (playerBet[j] >= maxPlayerWin && maxPlayerWin != ZERO_CHIPS)
                                                    {
                                                        //Subtract the max a player can win from the current player's bet
                                                        playerBet[j] -= maxPlayerWin;

                                                        //Add to the pot the max amount a player can win with this pot
                                                        chipPots[chipPots.Count - ONE_PLAYER] += maxPlayerWin;

                                                        //Add which players are able to win this pot
                                                        playersCanWin[playersCanWin.Count - ONE_PLAYER][j] = true;
                                                    }
                                                }
                                                //if the current player has folded and/or is not playing anymore
                                                else
                                                {
                                                    //If the bet of the current player is smaller then the max a player can win and the max is not equal to 0
                                                    if (playerBet[j] < maxPlayerWin && maxPlayerWin != ZERO_CHIPS)
                                                    {
                                                        //Adds the bet of the current player to the pot and sets the bet back to 0
                                                        chipPots[chipPots.Count - ONE_PLAYER] += playerBet[j];
                                                        playerBet[j] = ZERO_CHIPS;
                                                    }
                                                    else
                                                    {
                                                        //Adds the bet of the current player to the pot and subtracts the max a player can win from the current player's bet
                                                        chipPots[chipPots.Count - ONE_PLAYER] += maxPlayerWin;
                                                        playerBet[j] -= maxPlayerWin;
                                                    }
                                                }
                                            }

                                            //Loop for every single player
                                            for (int j = NO_PLAYER; j < playerBet.Length; j++)
                                            {
                                                //If the current player's bet is greater then 0
                                                if (playerBet[j] > ZERO_CHIPS)
                                                {
                                                    //Add a new pot
                                                    chipPots.Add(ZERO_CHIPS);
                                                    playersCanWin.Add(new bool[playerPlaying.Length]);

                                                    //Stop the looping
                                                    j = playerBet.Length;
                                                }
                                                //If the current player's bet is not greater then 0
                                                else
                                                {
                                                    //If the current player from the loop is equal to the total amount of player's subtracted by 1
                                                    if (j == playerBet.Length - ONE_PLAYER)
                                                    {
                                                        //Sets the pots to be done with
                                                        potsDone = true;
                                                    }
                                                }
                                            }
                                        }

                                        //Sets the current bet to 0
                                        currentBet = ZERO_CHIPS;

                                        //If there are no community cards right now
                                        if (communityCardCount == NO_CARDS)
                                        {
                                            //Loop through the first 3 community cards
                                            for (int i = NO_CARDS; i < THREE_CARDS; i++)
                                            {
                                                //Give values to the current community card
                                                communityCards[communityCardCount, 0] = CardValue(shuffleCards[nextCardInDeck]);
                                                communityCards[communityCardCount, 1] = CardSuit(shuffleCards[nextCardInDeck]);

                                                //Add a community card and set the next card that's available in the deck to be used
                                                communityCardCount++;
                                                nextCardInDeck++;
                                            }
                                        }
                                        //If there are 5 community cards
                                        else if (communityCardCount == FIVE_CARDS)
                                        {
                                            //Check which player won
                                            playerWon = CheckWhoWon();

                                            //Variables for the player who can win and the amount of players who can win
                                            int playerWhoCanWin = 0;
                                            int amountPlayersCanWin = 0;

                                            //For loop for every single pot of chips
                                            for (int i = ZERO_CHIPS; i < chipPots.Count; i++)
                                            {
                                                //For loop for the players who can win each pot
                                                for (int j = NO_PLAYER; j < playersCanWin[i].Length; j++)
                                                {
                                                    //If the current player can win the pot
                                                    if (playersCanWin[i][j] == true)
                                                    {
                                                        //If the amount of players who tied is greater then 0
                                                        if (playersTiedCount > NO_PLAYER)
                                                        {
                                                            //Calculates the current pay out that each player will get
                                                            currentPayOut = Convert.ToInt32(chipPots[i] / (playersTiedCount + ONE_PLAYER));

                                                            //If the player who won is the current player
                                                            if (playerWon == j)
                                                            {
                                                                //Add the amount of the current pay out to the amount of money of the current player
                                                                playerMoney[j] += currentPayOut;
                                                            }

                                                            //For loop for all the players who tied
                                                            for (int k = NO_PLAYER; k < playersTied.Count; k++)
                                                            {
                                                                //Add the amount of the current payout to the current player's money and remove the player from being tied
                                                                playerMoney[playersTied[k]] += currentPayOut;
                                                                playersTied.RemoveAt(k);
                                                                k--;
                                                            }
                                                        }
                                                        //If the amount of players who tied is not greater then 0
                                                        else
                                                        {
                                                            //If the player who won is the current player
                                                            if (playerWon == j)
                                                            {
                                                                //Add the amount of the pot to the amount of money of the current player
                                                                playerMoney[j] += chipPots[i];
                                                            }
                                                            //If the player who won is not the current player
                                                            else
                                                            {
                                                                //Sets the amount of players who can win to none and the player who can win to nothing
                                                                amountPlayersCanWin = NO_PLAYER;
                                                                playerWhoCanWin = NO_PLAYER;

                                                                //Loop for every single player that can player
                                                                for (int k = NO_PLAYER; k < playersCanWin[i].Length; k++)
                                                                {
                                                                    //If the current player can win the pot
                                                                    if (playersCanWin[i][k] == true)
                                                                    {
                                                                        //Sets the player who can win equal to the current player and adds one to the amount of players who can win
                                                                        playerWhoCanWin = k;
                                                                        amountPlayersCanWin++;
                                                                    }
                                                                }

                                                                //If the amount of players who can win is equal to one player
                                                                if (amountPlayersCanWin == ONE_PLAYER)
                                                                {
                                                                    //Adds the amount of the pot to the amount of money the current player has
                                                                    playerMoney[playerWhoCanWin] += chipPots[i];
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            //Clears the pots and the players who can win
                                            chipPots.Clear();
                                            playersCanWin.Clear();

                                            //Loop for every single player
                                            for (int i = NO_PLAYER; i < playerMoney.Length; i++)
                                            {
                                                //If the current player has no money and they are playing
                                                if (playerMoney[i] == ZERO_CHIPS && playerPlaying[i] == true)
                                                {
                                                    //Set them to not be playing anymore and subtract one player from the amount of players that are playing
                                                    playerPlaying[i] = false;
                                                    playersLeft--;
                                                }
                                            }

                                            //Set the new round to be needed
                                            newRound = true;
                                        }
                                        //If the community card count is anything else
                                        else
                                        {
                                            //Adds the next community cards
                                            communityCards[communityCardCount, 0] = CardValue(shuffleCards[nextCardInDeck]);
                                            communityCards[communityCardCount, 1] = CardSuit(shuffleCards[nextCardInDeck]);

                                            //Sets the amount of community cards to be an aditional one and sets the next available card in the deck
                                            communityCardCount++;
                                            nextCardInDeck++;
                                        }

                                        //if the game state is currently on network game
                                        if (gameState == NETWORK_GAME)
                                        {
                                            //Send to server is needed
                                            sendToServerNeeded = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //If anything else
                    else
                    {
                        //If the last player was not checked yet
                        if (lastPlayerCheck == false)
                        {
                            //If the current turn is the user;s
                            if (currentTurn == userPlayer)
                            {
                                //If the amount of players who folded is equal to the amount of players left subtracted 1
                                if (playersFoldedCount == playersLeft - ONE_PLAYER)
                                {
                                    //Send to server is needed
                                    sendToServerNeeded = true;

                                    //Checks the last player
                                    CheckLastPlayer();

                                    //Sets the last player to be checked
                                    lastPlayerCheck = true;
                                }
                            }
                        }
                        //if the last player was checked
                        else
                        {
                            //Sets the end to server to be needed, the reply to recieved and the message to send
                            sendToServerNeeded = false;
                            replyRecieved = true;
                            messageSent = true;
                        }
                    }

                    //If a reply from the server was recieved
                    if (replyRecieved == true)
                    {
                        //If a send to the server is needed and a message was already sent before
                        if (sendToServerNeeded == true && messageSent == true)
                        {
                            //Sets the send to the server to needed anymore and the message to not be sent anymore
                            sendToServerNeeded = false;
                            messageSent = false;

                            //Calls the subprogram to send to the server
                            SendToServer();
                        }
                    }
                    break;
                //If the current game state is on the pause menu
                case PAUSE_GAME:
                    //Loop for every single pause menu button
                    for (int i = FIRST_AI_PICK_BUTTON; i < menuBtnYLocations.Length; i++)
                    {
                        //Checks if the X coordinate of the mouse is on the button
                        if (mouseX >= menuBtnXLocation && mouseX <= menuBtnXLocation + menuBtnXSize)
                        {
                            //Checks if the Y coordinate of the mouse is on the current button
                            if (mouseY >= menuBtnYLocations[i] && mouseY <= menuBtnYLocations[i] + menuBtnYSize)
                            {
                                //Sets the boolean to true that says that the mouse is currently hovering over the pause menu button
                                isPauseMenuBtnHovering[i - FIRST_AI_PICK_BUTTON] = true;

                                //Checks if the user is pressing the left mouse button
                                if (Mouse.GetState().LeftButton == ButtonState.Pressed && leftMouseHeld == false)
                                {
                                    //Sets the left mouse being held variable to true
                                    leftMouseHeld = true;

                                    //Switch statement for the current button that is being checcked
                                    switch (i)
                                    {
                                        //First pause menu button
                                        case FIRST_AI_PICK_BUTTON:
                                            //Sets the game state to AI game
                                            gameState = AI_GAME;

                                            //Starts the timer again
                                            playerTime.Start();
                                            break;
                                        //Second pause menu button
                                        case SECOND_AI_PICK_BUTTON:
                                            //Sets the game state back to AI game and resets the game
                                            gameState = AI_GAME;
                                            newGame = true;
                                            break;
                                        //Third pause menu button
                                        case THIRD_AI_PICK_BUTTON:
                                            //Saves the game
                                            SaveGame();

                                            //Sets the game state back to main menu
                                            gameState = MAIN_MENU;

                                            //Resets the game
                                            ResetGame();

                                            //Sets the game to not have started anymore and a send to the server is not needed anymore
                                            gameStarted = false;
                                            sendToServerNeeded = false;
                                            break;
                                    }
                                }
                            }
                            //If the mouse Y is anywhere else
                            else
                            {
                                //Sets the mouse to not currently hovering over the menu button
                                isPauseMenuBtnHovering[i - FIRST_AI_PICK_BUTTON] = false;
                            }
                        }
                        //If the mouse X is anywhere else 
                        else
                        {
                            //Sets the mouse to not currently hovering over the menu button
                            isPauseMenuBtnHovering[i - FIRST_AI_PICK_BUTTON] = false;
                        }
                    }
                    break;
                //If the current gamestate is equal to the network lobby
                case NETWORK_LOBBY:
                    //Checks if the X coordinate of the mouse is on the button
                    if (mouseX >= ipAddressBoxXLocation && mouseX <= ipAddressBoxXLocation + ipAddressBoxXSize)
                    {
                        //Checks if the Y coordinate of the mouse is on the current button
                        if (mouseY >= ipAddressBoxYLocation && mouseY <= ipAddressBoxYLocation + ipAddressBoxYSize)
                        {
                            //Checks if the user is pressing the left mouse button
                            if (Mouse.GetState().LeftButton == ButtonState.Pressed && leftMouseHeld == false)
                            {
                                //Sets the left mouse being held variable to true
                                leftMouseHeld = true;

                                //If the ip addressed is being typed
                                if (isIpAddressTyping == true)
                                {
                                    //Set it to not being typed anymore
                                    isIpAddressTyping = false;
                                }
                                //If the ip address is not being typed
                                else
                                {
                                    //Set the ip address to be typing name and the name to not be typing anymore
                                    isIpAddressTyping = true;
                                    isNameTyping = false;
                                }
                            }
                        }
                    }

                    //If the connect button is not pressed
                    if (isConnectBtnPressed == false)
                    {
                        //Checks if the X coordinate of the mouse is on the button
                        if (mouseX >= connectBtnXLocation && mouseX <= connectBtnXLocation + connectBtnXSize)
                        {
                            //Checks if the Y coordinate of the mouse is on the current button
                            if (mouseY >= connectBtnYLocation && mouseY <= connectBtnYLocation + connectBtnYSize)
                            {
                                //Sets the connect button to be hovering over
                                isConnectBtnHovering = true;

                                //Checks if the user is pressing the left mouse button
                                if (Mouse.GetState().LeftButton == ButtonState.Pressed && leftMouseHeld == false)
                                {
                                    //Sets the left mouse being held variable to true
                                    leftMouseHeld = true;

                                    //If the current client name is not equal to a blank text and the current client name is greater then or equal to 3 characters
                                    if (currentClientName != "" && currentClientName.Length >= 3)
                                    {
                                        //Start trying to connect
                                        ConnectLoop();
                                    }

                                    //If the client connected
                                    if (clientConnected == true)
                                    {
                                        //Sets the connect button to pressed
                                        isConnectBtnPressed = true;
                                    }
                                    //If the client is not connected
                                    else
                                    {
                                        //Sets the connect button to not be pressed
                                        isConnectBtnPressed = false;

                                        //If the current client name is not equal to a blank and is longer then 3 characters
                                        if (currentClientName != "" && currentClientName.Length >= 3)
                                        {
                                            //Set the connect attempt to true
                                            connectAttempt = true;
                                        }
                                        //If the current client name is equal to a blank and/or is shorter then 3 characters
                                        else
                                        {
                                            //If the client name is longer then 0 characters
                                            if (currentClientName.Length > 0)
                                            {
                                                //Sets that there is a client name but it's too short
                                                noClientName = false;
                                                clientNameTooShort = true;
                                            }
                                            //If the client name is not longer then 0 characters
                                            else
                                            {
                                                //Sets that there is no client name
                                                noClientName = true;
                                                clientNameTooShort = false;
                                            }
                                        }
                                    }
                                }
                            }
                            //If the mouse Y is anywhere else
                            else
                            {
                                //Sets the connect button to not be hovering
                                isConnectBtnHovering = false;
                            }
                        }
                        //If the mouse X is anywhere else
                        else
                        {
                            //Sets the connect button to not be hovering
                            isConnectBtnHovering = false;
                        }
                    }
                    //If the connect button is already pressed
                    else
                    {
                        //Sets the connect button to not be hovering
                        isConnectBtnHovering = false;
                    }

                    //Checks if the X coordinate of the mouse is on the button
                    if (mouseX >= nameBoxXLocation && mouseX <= nameBoxXLocation + nameBoxXSize)
                    {
                        //Checks if the Y coordinate of the mouse is on the current button
                        if (mouseY >= nameBoxYLocation && mouseY <= nameBoxYLocation + nameBoxYSize)
                        {
                            //Checks if the user is pressing the left mouse button
                            if (Mouse.GetState().LeftButton == ButtonState.Pressed && leftMouseHeld == false)
                            {
                                //Sets the left mouse being held variable to true
                                leftMouseHeld = true;

                                //If the name is current being typed
                                if (isNameTyping == true)
                                {
                                    //Sets the name to not being typed anymore
                                    isNameTyping = false;
                                }
                                //If the name is currently not being typed
                                else
                                {
                                    //Sets the name to being typed now and the ip address to not being typed
                                    isNameTyping = true;
                                    isIpAddressTyping = false;
                                }
                            }
                        }
                    }

                    //If the client connected
                    if (clientConnected)
                    {
                        //Checks if the X coordinate of the mouse is on the button
                        if (mouseX >= readyBtnXLocation && mouseX <= readyBtnXLocation + readyBtnXSize)
                        {
                            //Checks if the Y coordinate of the mouse is on the current button
                            if (mouseY >= readyBtnYLocation && mouseY <= readyBtnYLocation + readyBtnYSize)
                            {
                                //Sets the ready button to be hovering over
                                isReadyBtnHovering = true;

                                //Checks if the user is pressing the left mouse button
                                if (Mouse.GetState().LeftButton == ButtonState.Pressed && leftMouseHeld == false)
                                {
                                    //Sets the left mouse being held variable to true
                                    leftMouseHeld = true;

                                    //If the current client is ready
                                    if (currentClientReady)
                                    {
                                        //Set the current client to not be ready anymore and send that to the server
                                        currentClientReady = false;
                                        byte[] sendBuffer = Encoding.ASCII.GetBytes("false");
                                        clientSocket.Send(sendBuffer);
                                    }
                                    //If the current client is not ready
                                    else
                                    {
                                        //Set the current client to ready now and sends that to the server
                                        currentClientReady = true;
                                        byte[] sendBuffer = Encoding.ASCII.GetBytes("true");
                                        clientSocket.Send(sendBuffer);
                                    }
                                }
                            }
                            //If the mouse Y is anywhere else
                            else
                            {
                                //Sets the ready button to not be hovering over
                                isReadyBtnHovering = false;
                            }
                        }
                        //If the mouse X is anywhere else
                        else
                        {
                            //Sets the ready button to not be hovering over
                            isReadyBtnHovering = false;
                        }
                    }

                    //If the ip address is being typed
                    if (isIpAddressTyping == true)
                    {
                        //If the length of the ip is smaller then 15
                        if (serverIP.Length < 15)
                        {
                            //If the user is typing a 0
                            if (kb.IsKeyDown(Keys.NumPad0) && keyboardKeyHeld == false || kb.IsKeyDown(Keys.D0) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a 0 to the server ip
                                keyboardKeyHeld = true;
                                serverIP += "0";
                            }
                            //If the user is typing a 1 
                            else if (kb.IsKeyDown(Keys.NumPad1) && keyboardKeyHeld == false || kb.IsKeyDown(Keys.D1) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a 1 to the server ip
                                keyboardKeyHeld = true;
                                serverIP += "1";
                            }
                            //If the user is typing a 2
                            else if (kb.IsKeyDown(Keys.NumPad2) && keyboardKeyHeld == false || kb.IsKeyDown(Keys.D2) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a 2 to the server ip
                                keyboardKeyHeld = true;
                                serverIP += "2";
                            }
                            //If the user is typing a 3
                            else if (kb.IsKeyDown(Keys.NumPad3) && keyboardKeyHeld == false || kb.IsKeyDown(Keys.D3) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a 3 to the server ip
                                keyboardKeyHeld = true;
                                serverIP += "3";
                            }
                            //If the user is typing a 4
                            else if (kb.IsKeyDown(Keys.NumPad4) && keyboardKeyHeld == false || kb.IsKeyDown(Keys.D4) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a 4 to the server ip
                                keyboardKeyHeld = true;
                                serverIP += "4";
                            }
                            //If the user is typing a 5
                            else if (kb.IsKeyDown(Keys.NumPad5) && keyboardKeyHeld == false || kb.IsKeyDown(Keys.D5) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a 5 to the server ip
                                keyboardKeyHeld = true;
                                serverIP += "5";
                            }
                            //If the user is typing a 6
                            else if (kb.IsKeyDown(Keys.NumPad6) && keyboardKeyHeld == false || kb.IsKeyDown(Keys.D6) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a 6 to the server ip
                                keyboardKeyHeld = true;
                                serverIP += "6";
                            }
                            //If the user is typing a 7
                            else if (kb.IsKeyDown(Keys.NumPad7) && keyboardKeyHeld == false || kb.IsKeyDown(Keys.D7) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a 7 to the server ip
                                keyboardKeyHeld = true;
                                serverIP += "7";
                            }
                            //If the user is typing a 8
                            else if (kb.IsKeyDown(Keys.NumPad8) && keyboardKeyHeld == false || kb.IsKeyDown(Keys.D8) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a 8 to the server ip
                                keyboardKeyHeld = true;
                                serverIP += "8";
                            }
                            //If the user is typing a 9
                            else if (kb.IsKeyDown(Keys.NumPad9) && keyboardKeyHeld == false || kb.IsKeyDown(Keys.D9) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a 9 to the server ip
                                keyboardKeyHeld = true;
                                serverIP += "9";
                            }
                            //If the user is typing a .
                            else if (kb.IsKeyDown(Keys.OemPeriod) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a . to the server ip
                                keyboardKeyHeld = true;
                                serverIP += ".";
                            }
                        }

                        //If the user is pressing the backspace key
                        if (kb.IsKeyDown(Keys.Back) && keyboardKeyHeld == false)
                        {
                            //Sets the keyboard key to held
                            keyboardKeyHeld = true;

                            //If the server ip length is greater then 0
                            if (serverIP.Length > 0)
                            {
                                //Remove the last number or decimal of the ip
                                serverIP = serverIP.Remove(serverIP.Length - 1);
                            }
                        }
                    }

                    //If a name is being typed
                    if (isNameTyping == true)
                    {
                        //If the current client name is less then 8 characters
                        if (currentClientName.Length < 8)
                        {
                            //If the user is typing a Q
                            if (kb.IsKeyDown(Keys.Q) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a Q to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "q";
                            }
                            //If the user is typing a W
                            else if (kb.IsKeyDown(Keys.W) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a W to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "w";
                            }
                            //If the user is typing a E
                            else if (kb.IsKeyDown(Keys.E) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a E to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "e";
                            }
                            //If the user is typing a R
                            else if (kb.IsKeyDown(Keys.R) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a R to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "r";
                            }
                            //If the user is typing a T
                            else if (kb.IsKeyDown(Keys.T) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a T to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "t";
                            }
                            //If the user is typing a Y
                            else if (kb.IsKeyDown(Keys.Y) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a Y to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "y";
                            }
                            //If the user is typing a U
                            else if (kb.IsKeyDown(Keys.U) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a U to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "u";
                            }
                            //If the user is typing a I
                            else if (kb.IsKeyDown(Keys.I) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a I to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "i";
                            }
                            //If the user is typing a O
                            else if (kb.IsKeyDown(Keys.O) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a O to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "o";
                            }
                            //If the user is typing a P
                            else if (kb.IsKeyDown(Keys.P) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a P to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "p";
                            }
                            //If the user is typing a A
                            else if (kb.IsKeyDown(Keys.A) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a A to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "a";
                            }
                            //If the user is typing a S
                            else if (kb.IsKeyDown(Keys.S) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a S to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "s";
                            }
                            //If the user is typing a D
                            else if (kb.IsKeyDown(Keys.D) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a D to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "d";
                            }
                            //If the user is typing a F
                            else if (kb.IsKeyDown(Keys.F) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a F to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "f";
                            }
                            //If the user is typing a G
                            else if (kb.IsKeyDown(Keys.G) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a G to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "g";
                            }
                            //If the user is typing a H
                            else if (kb.IsKeyDown(Keys.H) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a H to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "h";
                            }
                            //If the user is typing a J
                            else if (kb.IsKeyDown(Keys.J) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a J to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "j";
                            }
                            //If the user is typing a K
                            else if (kb.IsKeyDown(Keys.K) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a K to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "k";
                            }
                            //If the user is typing a L
                            else if (kb.IsKeyDown(Keys.L) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a L to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "l";
                            }
                            //If the user is typing a Z
                            else if (kb.IsKeyDown(Keys.Z) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a Z to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "z";
                            }
                            //If the user is typing a X
                            else if (kb.IsKeyDown(Keys.X) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a X to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "x";
                            }
                            //If the user is typing a C
                            else if (kb.IsKeyDown(Keys.C) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a C to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "c";
                            }
                            //If the user is typing a V
                            else if (kb.IsKeyDown(Keys.V) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a V to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "v";
                            }
                            //If the user is typing a B
                            else if (kb.IsKeyDown(Keys.B) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a B to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "b";
                            }
                            //If the user is typing a N
                            else if (kb.IsKeyDown(Keys.N) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a N to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "n";
                            }
                            //If the user is typing a M
                            else if (kb.IsKeyDown(Keys.M) && keyboardKeyHeld == false)
                            {
                                //Sets the keyboard key to held and adds a M to the client name string
                                keyboardKeyHeld = true;
                                currentClientName += "m";
                            }
                        }

                        //If the user is pressing the backspace key
                        if (kb.IsKeyDown(Keys.Back) && keyboardKeyHeld == false)
                        {
                            //Sets the keyboard key to held
                            keyboardKeyHeld = true;

                            //If the current client name is greater then 0 characters
                            if (currentClientName.Length > 0)
                            {
                                //Removes the last character of the current client name string
                                currentClientName = currentClientName.Remove(currentClientName.Length - 1);
                            }
                        }
                    }
                    break;
            }

            base.Update(gameTime);
        }



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Clears the screen
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            base.Draw(gameTime);

            //Switch for the different possible game statess
            switch (gameState)
            {
                //If the game state is main menu
                case MAIN_MENU:
                    //Draws the main menu
                    DrawMainMenu();
                    break;
                //If the game state is AI Game Type menu
                case AI_GAME_TYPE:
                    //Draws the AI user pick menu
                    DrawAIUserPick();
                    break;
                //If the game state is AI game, pause game, or network game
                case AI_GAME:
                case PAUSE_GAME:
                case NETWORK_GAME:
                    //If the game started
                    if (gameStarted == true)
                    {
                        //Draws the cards for every player
                        DrawGameCards();

                        //Draws all the betting choices for the user
                        DrawBettingChoices();

                        //Draw all the player info for every player
                        DrawPlayerInfo();

                        //If the game state is currently not on network game
                        if (gameState != NETWORK_GAME)
                        {
                            //Draws the poker chips for the players and the pause buttons
                            DrawMoneyChips();
                            DrawPauseButton();
                        }
                    }

                    //If the game state is currently on the pause game
                    if (gameState == PAUSE_GAME)
                    {
                        //Draws the pause game menu
                        DrawPauseMenu();
                    }
                    break;
                //If the game state is network lobby
                case NETWORK_LOBBY:
                    //Draws the network lobby
                    DrawNetworkLobby();
                    break;
            }
        }



        //Pre: The server IP, and if the different buttons are pressed or being hovered over
        //Post: The Network Lobby is drawn
        //Description: A Procedure that draws everything in the network lobby
        private void DrawNetworkLobby()
        {
            //Variable for the location the names are going to be drawn in the table
            int drawLocation;

            //Starts the sprite batch
            spriteBatch.Begin();

            //Draws the background
            spriteBatch.Draw(background, backgroundSize, Color.White);

            //Draws a string telling the user what to do
            spriteBatch.DrawString(menuBtnFont, "Please press on either the name box or IP to type into them.", new Vector2(10, 10), Color.White);

            //If the connect button is pressed
            if (isConnectBtnPressed == true)
            {
                //If the client is connected
                if (clientConnected == true)
                {
                    //Draws the button with the string connected on it
                    spriteBatch.Draw(buttons, networkLobbyRec[0], Color.White);
                    spriteBatch.Draw(buttons, networkLobbyRec[1], Color.Gray);
                    spriteBatch.DrawString(menuBtnFont, "Connected", new Vector2(connectBtnXLocation + 8, connectBtnYLocation + 2), Color.White);
                }
            }
            //If the connect button is being hovered over
            else if (isConnectBtnHovering == true)
            {
                //Draws the button with the string connect on it
                spriteBatch.Draw(buttons, networkLobbyRec[2], Color.Red);
                spriteBatch.Draw(buttons, networkLobbyRec[3], Color.Black);
                spriteBatch.DrawString(menuBtnHoverFont, "Connect", new Vector2(connectBtnXLocation + 16, connectBtnYLocation), Color.Yellow);
            }
            //If the connect button is neither
            else
            {
                //Draws the connect button with the string connect on it
                spriteBatch.Draw(buttons, networkLobbyRec[4], Color.Red);
                spriteBatch.Draw(buttons, networkLobbyRec[5], Color.Black);
                spriteBatch.DrawString(menuBtnFont, "Connect", new Vector2(connectBtnXLocation + 22, connectBtnYLocation + 2), Color.White);
            }

            //If a connection was attempted
            if (connectAttempt == true)
            {
                //Draws the string telling the user to check their IP and try agaom
                spriteBatch.DrawString(menuBtnFont, "Please check your IP and try connecting again.", new Vector2(connectBtnXLocation + connectBtnXSize + 10, connectBtnYLocation + 2), Color.White);
            }

            //If the IP is being typed in
            if (isIpAddressTyping == true)
            {
                //Draws the box for the ip with a red outline
                spriteBatch.Draw(buttons, networkLobbyRec[6], Color.Red);
                spriteBatch.Draw(buttons, networkLobbyRec[7], Color.Black);
            }
            //If the IP is not being typed in
            else
            {
                //Draws the box for the ip with a white outline
                spriteBatch.Draw(buttons, networkLobbyRec[8], Color.White);
                spriteBatch.Draw(buttons, networkLobbyRec[9], Color.Black);
            }

            //If the name is being typed in
            if (isNameTyping == true)
            {
                //Draws the box for the name with a red outline
                spriteBatch.Draw(buttons, networkLobbyRec[10], Color.Red);
                spriteBatch.Draw(buttons, networkLobbyRec[11], Color.Black);
            }
            //If the name is not being typed in
            else
            {
                //Draws the box for the name with a white outline
                spriteBatch.Draw(buttons, networkLobbyRec[12], Color.White);
                spriteBatch.Draw(buttons, networkLobbyRec[13], Color.Black);
            }

            //Writes the server ip and the current client name
            spriteBatch.DrawString(menuBtnFont, serverIP, new Vector2(ipAddressBoxXLocation + 10, ipAddressBoxYLocation + 3), Color.White);
            spriteBatch.DrawString(menuBtnFont, currentClientName, new Vector2(nameBoxXLocation + 10, nameBoxYLocation + 3), Color.White);

            //If there is no client name and they trying connecting
            if (noClientName == true)
            {
                //Tell the user to type in a name
                spriteBatch.DrawString(menuBtnFont, "Please type in a name before connecting.", new Vector2(readyBtnXLocation + readyBtnXSize + 10, readyBtnYLocation + 2), Color.White);
            }
            //If there is a client name but it's too short when connecting
            else if (clientNameTooShort == true)
            {
                //Tell the user to check their name
                spriteBatch.DrawString(menuBtnFont, "Please check the size of the name you typed in.", new Vector2(readyBtnXLocation + readyBtnXSize + 10, readyBtnYLocation + 2), Color.White);
            }

            //If the client is connected
            if (clientConnected)
            {
                //If the ready button is being hovered over
                if (isReadyBtnHovering)
                {
                    //Draws the button big with a red outline
                    spriteBatch.Draw(buttons, networkLobbyRec[14], Color.Red);
                    spriteBatch.Draw(buttons, networkLobbyRec[15], Color.Black);
                }
                //If the button is not being hovered over
                else
                {
                    //Draws the button small with a red outline
                    spriteBatch.Draw(buttons, networkLobbyRec[16], Color.Red);
                    spriteBatch.Draw(buttons, networkLobbyRec[17], Color.Black);
                }
            }
            //If the client is not connected
            else
            {
                //Draws the ready button with a white outline
                spriteBatch.Draw(buttons, networkLobbyRec[18], Color.White);
                spriteBatch.Draw(buttons, networkLobbyRec[19], Color.Gray);
            }

            //Draws the ready text
            spriteBatch.DrawString(menuBtnFont, "Ready", new Vector2(readyBtnXLocation + 10, readyBtnYLocation + 10), Color.White);

            //Draws the table for the names
            spriteBatch.Draw(buttons, networkLobbyRec[20], Color.White);
            spriteBatch.Draw(buttons, networkLobbyRec[21], Color.Black);
            spriteBatch.Draw(buttons, networkLobbyRec[22], Color.White);

            //Draws the table for the names
            for (int i = 1; i < clientsReady.Length + 1; i++)
            {
                spriteBatch.Draw(buttons, networkLobbyRec[22 + i], Color.White);
            }

            //Draws the warning, the names column and if the player is ready column text
            spriteBatch.DrawString(menuBtnFont, "Name:", new Vector2(namesTableXLocation + 10, namesTableYLocation + 10), Color.White);
            spriteBatch.DrawString(menuBtnFont, "Ready?", new Vector2(namesTableXLocation + Convert.ToInt32(namesTableXSize * 0.65) + 8, namesTableYLocation + 10), Color.White);
            spriteBatch.DrawString(menuBtnFont, "Warning: Name has to be a minimum of 3 letters and a maximum of 8", new Vector2(nameBoxXLocation, nameBoxYLocation + nameBoxYSize + 6), Color.White);

            //Sets a location to draw
            drawLocation = namesTableYLocation + 64;

            //Loop for every client that connected
            for (int i = 0; i < clientNames.Count; i++)
            {
                //Draws the client name
                spriteBatch.DrawString(menuBtnFont, clientNames[i], new Vector2(namesTableXLocation + 10, drawLocation), Color.White);

                //If the client is ready
                if (clientsReady[i])
                {
                    //Draw that they are ready
                    spriteBatch.DrawString(menuBtnFont, "Yes", new Vector2(namesTableXLocation + Convert.ToInt32(namesTableXSize * 0.65) + 18, drawLocation), Color.White);
                }
                //If the client is not ready
                else
                {
                    //Draw that they are not ready
                    spriteBatch.DrawString(menuBtnFont, "No", new Vector2(namesTableXLocation + Convert.ToInt32(namesTableXSize * 0.65) + 18, drawLocation), Color.White);
                }

                //Updates the draw location
                drawLocation += 66;
            }

            //Ends the sprite batch
            spriteBatch.End();
        }



        //Pre: If the user is hovering over any buttons
        //Post: The pause button is drawn
        //Desc: A subprogram which draws the pause button in the AI game
        private void DrawPauseButton()
        {
            //Begins the sprite batch
            spriteBatch.Begin();

            //If the pause button is being hovered over
            if (isPauseBtnHovering == true)
            {
                //Draw the pause button big with the text in yellow colour
                spriteBatch.Draw(buttons, pauseBtnRec[0], Color.Red);
                spriteBatch.Draw(buttons, pauseBtnRec[1], Color.Black);
                spriteBatch.DrawString(pauseBtnHoverFont, "PAUSE", new Vector2(pauseBtnXLocation + 17, pauseBtnYLocation + 8), Color.Yellow);
            }
            //If the pause button is not being hovered over
            else
            {
                //Draw the pause button small with the text in white colour
                spriteBatch.Draw(buttons, pauseBtnRec[2], Color.Red);
                spriteBatch.Draw(buttons, pauseBtnRec[3], Color.Black);
                spriteBatch.DrawString(pauseBtnFont, "PAUSE", new Vector2(pauseBtnXLocation + 30, pauseBtnYLocation + 13), Color.White);
            }

            //Ens the sprite batch
            spriteBatch.End();
        }



        //Pre: The player names, the amount of money each player has, and the current turn
        //Post: All the info for every player is drawn with their names, and money and the player who's turn it currently is is drawn with a blue outline
        //Desc: A subprogram which draws all the info for every player
        private void DrawPlayerInfo()
        {
            //Variable for the length of the current name
            int playerNameLength;

            //Begins the sprite batch
            spriteBatch.Begin();

            //Loop for every player
            for (int i = 0; i < playerPlaying.Length; i++)
            {
                //If the current player is playing
                if (playerPlaying[i] == true)
                {
                    //If it's the current player's turn
                    if (i == currentTurn)
                    {
                        //Draws the seat with a blue outline
                        spriteBatch.Draw(currentPlayerSeat, playerInfoRec[i], Color.White);
                    }
                    //If it's not the current player's turn
                    else
                    {
                        //Draws the seat with a yellow outlined
                        spriteBatch.Draw(playerSeat, playerInfoRec[i], Color.White);
                    }

                    //Set's how long the player's name is
                    playerNameLength = playerNames[i].Length;

                    //Switch statement for how long the name is
                    switch (playerNameLength)
                    {
                        //If the name is 3 characters
                        case 3:
                            //Draws the player name
                            spriteBatch.DrawString(playerInfoFont, playerNames[i], new Vector2(playerInfoXLocation[i] + 50F, playerInfoYLocation[i] + 1F), Color.White);
                            break;
                        //If the name is 4 characters
                        case 4:
                            //Draws the player name
                            spriteBatch.DrawString(playerInfoFont, playerNames[i], new Vector2(playerInfoXLocation[i] + 45F, playerInfoYLocation[i] + 1F), Color.White);
                            break;
                        //If the name is 5 characters
                        case 5:
                            //Draws the player name
                            spriteBatch.DrawString(playerInfoFont, playerNames[i], new Vector2(playerInfoXLocation[i] + 39F, playerInfoYLocation[i] + 1F), Color.White);
                            break;
                        //If the name is 6 characters
                        case 6:
                            //Draws the player name
                            spriteBatch.DrawString(playerInfoFont, playerNames[i], new Vector2(playerInfoXLocation[i] + 38F, playerInfoYLocation[i] + 1F), Color.White);
                            break;
                        //If the name is 7 characters
                        case 7:
                            //Draws the player name
                            spriteBatch.DrawString(playerInfoFont, playerNames[i], new Vector2(playerInfoXLocation[i] + 27F, playerInfoYLocation[i] + 1F), Color.White);
                            break;
                        //If the name is 8 characters
                        case 8:
                            //Draws the player name
                            spriteBatch.DrawString(playerInfoFont, playerNames[i], new Vector2(playerInfoXLocation[i] + 23F, playerInfoYLocation[i] + 1F), Color.White);
                            break;
                        //If the name is 9 characters
                        case 9:
                            //Draws the player name
                            spriteBatch.DrawString(playerInfoFont, playerNames[i], new Vector2(playerInfoXLocation[i] + 17F, playerInfoYLocation[i] + 1F), Color.White);
                            break;
                        //If the name is 10 characters
                        case 10:
                            //Draws the player name
                            spriteBatch.DrawString(playerInfoFont, playerNames[i], new Vector2(playerInfoXLocation[i] + 11F, playerInfoYLocation[i] + 1F), Color.White);
                            break;
                    }

                    //If the player's money is greater then or equal to one thousand
                    if (playerMoney[i] >= 1000)
                    {
                        //Draws their money
                        spriteBatch.DrawString(playerInfoFont, "$" + playerMoney[i], new Vector2(playerInfoXLocation[i] + 37F, playerInfoYLocation[i] + 25F), Color.White);
                    }
                    //If the player's money is greater then or equal to 100
                    else if (playerMoney[i] >= 100)
                    {
                        //Draws the player's money
                        spriteBatch.DrawString(playerInfoFont, "$" + playerMoney[i], new Vector2(playerInfoXLocation[i] + 43F, playerInfoYLocation[i] + 25F), Color.White);
                    }
                    //If the player's money is greater then or equal to 10
                    else if (playerMoney[i] >= 10)
                    {
                        //Draws the player's money
                        spriteBatch.DrawString(playerInfoFont, "$" + playerMoney[i], new Vector2(playerInfoXLocation[i] + 48F, playerInfoYLocation[i] + 25F), Color.White);
                    }
                    //If the player's money is anything else
                    else
                    {
                        //Draws the player's money
                        spriteBatch.DrawString(playerInfoFont, "$" + playerMoney[i], new Vector2(playerInfoXLocation[i] + 55F, playerInfoYLocation[i] + 25F), Color.White);
                    }

                }
                //If the player is not playing
                else
                {
                    //Draw's an empty seat
                    spriteBatch.Draw(emptySeat, playerInfoRec[i], Color.White);
                }
            }

            //End the sprite batch
            spriteBatch.End();
        }



        //Pre: If the user is hovering over any buttons
        //Post: The pause menu is drawn
        //Desc: A subprogram which draws the pause menu
        private void DrawPauseMenu()
        {
            //Begins the sprite batch
            spriteBatch.Begin();

            //Draws the background
            spriteBatch.Draw(pauseBackground, backgroundSize, Color.Gray * 0.8F);

            //Loop for every button in the pause menu
            for (int i = FIRST_AI_PICK_BUTTON; i < menuBtnYLocations.Length; i++)
            {
                //If the current button is being hovered over
                if (isPauseMenuBtnHovering[i - FIRST_AI_PICK_BUTTON] == true)
                {
                    //Draws the button with it being hovered over
                    spriteBatch.Draw(buttons, new Rectangle(menuHoverBtnXLocation, menuHoverBtnYLoactions[i], menuHoverBtnXSize, menuHoverBtnYSize), Color.Red);
                    spriteBatch.Draw(buttons, new Rectangle(menuHoverBtnXLocation + 7, menuHoverBtnYLoactions[i] + 7, menuHoverBtnXSize - 14, menuHoverBtnYSize - 14), Color.Black);

                    //Switch statement for every pause menu button
                    switch (i)
                    {
                        //If it's the first button
                        case FIRST_AI_PICK_BUTTON:
                            //Writes resume on it
                            spriteBatch.DrawString(menuBtnHoverFont, "Resume", new Vector2(menuHoverBtnXLocation + 55, menuHoverBtnYLoactions[i] + 30), Color.Yellow);
                            break;
                        //If it's the second button
                        case SECOND_AI_PICK_BUTTON:
                            //Writes restart on it
                            spriteBatch.DrawString(menuBtnHoverFont, "Restart", new Vector2(menuHoverBtnXLocation + 59, menuHoverBtnYLoactions[i] + 30), Color.Yellow);
                            break;
                        //If it's the third button
                        case THIRD_AI_PICK_BUTTON:
                            //Writes save and exit on it
                            spriteBatch.DrawString(menuBtnHoverFont, "Save & Exit", new Vector2(menuHoverBtnXLocation + 20, menuHoverBtnYLoactions[i] + 30), Color.Yellow);
                            break;
                    }
                }
                //If the current button is not being hovered over
                else
                {
                    //Draws the button normally
                    spriteBatch.Draw(buttons, new Rectangle(menuBtnXLocation, menuBtnYLocations[i], menuBtnXSize, menuBtnYSize), Color.Red);
                    spriteBatch.Draw(buttons, new Rectangle(menuBtnXLocation + 7, menuBtnYLocations[i] + 7, menuBtnXSize - 14, menuBtnYSize - 14), Color.Black);

                    //Switch statement for every pause meny button
                    switch (i)
                    {
                        //If it's the first button
                        case FIRST_AI_PICK_BUTTON:
                            //Writes resume on it
                            spriteBatch.DrawString(menuBtnFont, "Resume", new Vector2(menuBtnXLocation + 50, menuBtnYLocations[i] + 22), Color.White);
                            break;
                        //If it's the second button
                        case SECOND_AI_PICK_BUTTON:
                            //Writes restart on it
                            spriteBatch.DrawString(menuBtnFont, "Restart", new Vector2(menuBtnXLocation + 56, menuBtnYLocations[i] + 22), Color.White);
                            break;
                        //If it's the third button
                        case THIRD_AI_PICK_BUTTON:
                            //Writes save and exit on it
                            spriteBatch.DrawString(menuBtnFont, "Save & Exit", new Vector2(menuBtnXLocation + 20, menuBtnYLocations[i] + 22), Color.White);
                            break;
                    }
                }
            }

            //Ends the sprite batch
            spriteBatch.End();
        }



        //Pre: The current player's bet
        //Post: All the chips are drawn for the AI Game
        //Desc: This subprogram draws all the chips when they are stationary or moving for the user to see the bet's
        private void DrawMoneyChips()
        {
            //Variables for the amounts of each chip to draw and how much money is being drawn in total
            int playerMoneyLeft = 0;
            int playerBlackChips = 0;
            int playerBlueChips = 0;
            int playerGreenChips = 0;
            int playerRedChips = 0;
            int playerWhiteChips = 0;

            //Variables for the X and Y loaction of the chips
            int chipXLocation = 0;
            int chipYLocation = 0;

            //Begins the sprite batch
            spriteBatch.Begin();

            //Loop for every player in the game
            for (int i = 0; i < playerPlaying.Length; i++)
            {
                //If the player is playing and their bet is greater then 0
                if (playerPlaying[i] == true && playerBet[i] > ZERO_CHIPS)
                {
                    //Give the total amount of money that's being drawn a value
                    playerMoneyLeft = playerBet[i];

                    //Calculate the amount of black chips being drawn using the AmountChipsDraw function and subtract that from the total amount of chips that need to be drawn
                    playerBlackChips = AmountChipsDraw(100, playerMoneyLeft);
                    playerMoneyLeft -= playerBlackChips * 100;

                    //Calculate the amount of blue chips being drawn using the AmountChipsDraw function and subtract that from the total amount of chips that need to be drawn
                    playerBlueChips = AmountChipsDraw(50, playerMoneyLeft);
                    playerMoneyLeft -= playerBlueChips * 50;

                    //Calculate the amount of green chips being drawn using the AmountChipsDraw function and subtract that from the total amount of chips that need to be drawn
                    playerGreenChips = AmountChipsDraw(25, playerMoneyLeft);
                    playerMoneyLeft -= playerGreenChips * 25;

                    //Calculate the amount of red chips being drawn using the AmountChipsDraw function and subtract that from the total amount of chips that need to be drawn
                    playerRedChips = AmountChipsDraw(10, playerMoneyLeft);
                    playerMoneyLeft -= playerRedChips * 10;

                    //Calculate the amount of white chips being drawn using the AmountChipsDraw function and subtract that from the total amount of chips that need to be drawn
                    playerWhiteChips = AmountChipsDraw(5, playerMoneyLeft);
                    playerMoneyLeft -= playerWhiteChips * 5;

                    //Gives a value to the current chips X and Y location
                    chipXLocation = playerChipsXLocations[i];
                    chipYLocation = playerChipsYLocations[i];

                    //Loop for every black chip that needs to be drawn
                    for (int j = FIRST_PLAYER; j < playerBlackChips; j++)
                    {
                        //Draws the black chip and subtracts one from the Y of the chips
                        spriteBatch.Draw(blackPokerChip, new Rectangle(chipXLocation, chipYLocation, pokerChipXSize, pokerChipYSize), Color.White);
                        chipYLocation--;
                    }

                    //Loop for every blue chip that needs to be drawn
                    for (int j = FIRST_PLAYER; j < playerBlueChips; j++)
                    {
                        //Draws the blue chip and subtracts one from the Y of the chips
                        spriteBatch.Draw(bluePokerChip, new Rectangle(chipXLocation, chipYLocation, pokerChipXSize, pokerChipYSize), Color.White);
                        chipYLocation--;

                    }

                    //Loop for every green chip that needs to be drawn
                    for (int j = FIRST_PLAYER; j < playerGreenChips; j++)
                    {
                        //Draws the green chip and subtracts one from the Y of the chips
                        spriteBatch.Draw(greenPokerChip, new Rectangle(chipXLocation, chipYLocation, pokerChipXSize, pokerChipYSize), Color.White);
                        chipYLocation--;
                    }

                    //Loop for every red chip that needs to be drawn
                    for (int j = FIRST_PLAYER; j < playerRedChips; j++)
                    {
                        //Draws the red chip and subtracts one from the Y of the chips
                        spriteBatch.Draw(redPokerChip, new Rectangle(chipXLocation, chipYLocation, pokerChipXSize, pokerChipYSize), Color.White);
                        chipYLocation--;
                    }

                    //Loop for every white chip that needs to be drawn
                    for (int j = FIRST_PLAYER; j < playerWhiteChips; j++)
                    {
                        //Draws the white chip and subtracts one from the Y of the chips
                        spriteBatch.Draw(whitePokerChip, new Rectangle(chipXLocation, chipYLocation, pokerChipXSize, pokerChipYSize), Color.White);
                        chipYLocation--;
                    }
                }
            }

            //If the amount of community cards is greater then 0
            if (communityCardCount > NO_CARDS)
            {
                //Give a value to the X and Y location of the chips
                chipXLocation = CHIP_X_MAIN_STACK;
                chipYLocation = CHIP_Y_MAIN_STACK;

                //Loop for every pot of chips
                for (int i = 0; i < chipPots.Count; i++)
                {
                    //Set the total amount of chips to draw to the current pot
                    playerMoneyLeft = chipPots[i];

                    //Calculate the amount of black chips being drawn using the AmountChipsDraw function and subtract that from the total amount of chips that need to be drawn
                    playerBlackChips = AmountChipsDraw(100, playerMoneyLeft);
                    playerMoneyLeft -= playerBlackChips * 100;

                    //Calculate the amount of blue chips being drawn using the AmountChipsDraw function and subtract that from the total amount of chips that need to be drawn
                    playerBlueChips = AmountChipsDraw(50, playerMoneyLeft);
                    playerMoneyLeft -= playerBlueChips * 50;

                    //Calculate the amount of green chips being drawn using the AmountChipsDraw function and subtract that from the total amount of chips that need to be drawn
                    playerGreenChips = AmountChipsDraw(25, playerMoneyLeft);
                    playerMoneyLeft -= playerGreenChips * 25;

                    //Calculate the amount of red chips being drawn using the AmountChipsDraw function and subtract that from the total amount of chips that need to be drawn
                    playerRedChips = AmountChipsDraw(10, playerMoneyLeft);
                    playerMoneyLeft -= playerRedChips * 10;

                    //Calculate the amount of white chips being drawn using the AmountChipsDraw function and subtract that from the total amount of chips that need to be drawn
                    playerWhiteChips = AmountChipsDraw(5, playerMoneyLeft);
                    playerMoneyLeft -= playerWhiteChips * 5;

                    //Loop for every black chip that needs to be drawn
                    for (int j = FIRST_PLAYER; j < playerBlackChips; j++)
                    {
                        //Draws the black chip and subtracts one from the Y of the chips
                        spriteBatch.Draw(blackPokerChip, new Rectangle(chipXLocation, chipYLocation, pokerChipXSize, pokerChipYSize), Color.White);
                        chipYLocation--;
                    }

                    //If the amount of black chips to draw is greater then 0
                    if (playerBlackChips > ZERO_CHIPS)
                    {
                        //Updates the X and Y location of the chips
                        chipXLocation += pokerChipXSize + Convert.ToInt32(pokerChipXSize / 2);
                        chipYLocation = CHIP_Y_MAIN_STACK;
                    }

                    //Loop for every blue chip that needs to be drawn
                    for (int j = FIRST_PLAYER; j < playerBlueChips; j++)
                    {
                        //Draws the blue chip and subtracts one from the Y of the chips
                        spriteBatch.Draw(bluePokerChip, new Rectangle(chipXLocation, chipYLocation, pokerChipXSize, pokerChipYSize), Color.White);
                        chipYLocation--;
                    }

                    //If the amount of blue chips to draw is greater then 0
                    if (playerBlueChips > ZERO_CHIPS)
                    {
                        //Updates the X and Y location of the chips
                        chipXLocation += pokerChipXSize + Convert.ToInt32(pokerChipXSize / 2);
                        chipYLocation = CHIP_Y_MAIN_STACK;
                    }

                    //Loop for every green chip that needs to be drawn
                    for (int j = FIRST_PLAYER; j < playerGreenChips; j++)
                    {
                        //Draws the green chip and subtracts one from the Y of the chips
                        spriteBatch.Draw(greenPokerChip, new Rectangle(chipXLocation, chipYLocation, pokerChipXSize, pokerChipYSize), Color.White);
                        chipYLocation--;
                    }

                    //If the amount of green chips to draw is greater then 0
                    if (playerGreenChips > ZERO_CHIPS)
                    {
                        //Updates the X and Y location of the chips
                        chipXLocation += pokerChipXSize + Convert.ToInt32(pokerChipXSize / 2);
                        chipYLocation = CHIP_Y_MAIN_STACK;
                    }

                    //Loop for every red chip that needs to be drawn
                    for (int j = FIRST_PLAYER; j < playerRedChips; j++)
                    {
                        //Draws the red chip and subtracts one from the Y of the chips
                        spriteBatch.Draw(redPokerChip, new Rectangle(chipXLocation, chipYLocation, pokerChipXSize, pokerChipYSize), Color.White);
                        chipYLocation--;
                    }

                    //If the amount of red chips to draw is greater then 0
                    if (playerRedChips > ZERO_CHIPS)
                    {
                        //Updates the X and Y location of the chips
                        chipXLocation += pokerChipXSize + Convert.ToInt32(pokerChipXSize / 2);
                        chipYLocation = CHIP_Y_MAIN_STACK;
                    }

                    //Loop for every white chip that needs to be drawn
                    for (int j = FIRST_PLAYER; j < playerWhiteChips; j++)
                    {
                        //Draws the white chip and subtracts one from the Y of the chips
                        spriteBatch.Draw(whitePokerChip, new Rectangle(chipXLocation, chipYLocation, pokerChipXSize, pokerChipYSize), Color.White);
                        chipYLocation--;
                    }

                    //Updates the X and Y location of the chips
                    chipXLocation = CHIP_X_MAIN_STACK;
                    chipYLocation = CHIP_Y_MAIN_STACK - pokerChipYSize - Convert.ToInt32(pokerChipYSize / 2);
                }
            }

            //Ends the sprite batch
            spriteBatch.End();
        }



        //Pre: If the user is hovering over any buttons
        //Post: The main menu is drawn
        //Desc: A subprogram which draws the main menu
        private void DrawMainMenu()
        {
            //Begins the sprite batch
            spriteBatch.Begin();

            //Draws the background for the main menu
            spriteBatch.Draw(background, backgroundSize, Color.White);

            //Loop for every button in the main menu
            for (int i = FIRST_MENU_BUTTON; i < FIRST_AI_PICK_BUTTON; i++)
            {
                //If the current button is being hovered over
                if (isMenuBtnHovering[i])
                {
                    //Draws the button with it enlarged
                    spriteBatch.Draw(buttons, mainMenuHoverRec[i], Color.Red);
                    spriteBatch.Draw(buttons, mainMenuHoverRec[i + isMenuBtnHovering.Length], Color.Black);

                    //Switch statement for which button is currently being checked
                    switch (i)
                    {
                        //If it's the first button
                        case FIRST_MENU_BUTTON:
                            //Draw the single player string
                            spriteBatch.DrawString(menuBtnHoverFont, "Single Player", new Vector2(menuHoverBtnXLocation + 12,
                                menuHoverBtnYLoactions[i] + 30), Color.Yellow);
                            break;
                        //If it's the second button
                        case SECOND_MENU_BUTTON:
                            //Draws the multiplayer string
                            spriteBatch.DrawString(menuBtnHoverFont, "Multiplayer", new Vector2(menuHoverBtnXLocation + 25,
                                menuHoverBtnYLoactions[i] + 30), Color.Yellow);
                            break;
                        //If it's the third button
                        case THIRD_MENU_BUTTON:
                            //Draws the quit game string
                            spriteBatch.DrawString(menuBtnHoverFont, "Quit Game", new Vector2(menuHoverBtnXLocation + 30,
                                menuHoverBtnYLoactions[i] + 30), Color.Yellow);
                            break;
                    }
                }
                //If the current button is not being hovered over
                else
                {
                    //Draws the button normally
                    spriteBatch.Draw(buttons, mainMenuRec[i], Color.Red);
                    spriteBatch.Draw(buttons, mainMenuRec[i + isMenuBtnHovering.Length], Color.Black);

                    //Switch statement for which button is currently being checked
                    switch (i)
                    {
                        //If it's the first button
                        case FIRST_MENU_BUTTON:
                            //Draw the single player string
                            spriteBatch.DrawString(menuBtnFont, "Single Player", new Vector2(menuBtnXLocation + 15,
                                menuBtnYLocations[i] + 22), Color.White);
                            break;
                        //If it's the second button
                        case SECOND_MENU_BUTTON:
                            //Draw the multiplayer string
                            spriteBatch.DrawString(menuBtnFont, "Multiplayer", new Vector2(menuBtnXLocation + 27,
                                 menuBtnYLocations[i] + 22), Color.White);
                            break;
                        //If it's the third button
                        case THIRD_MENU_BUTTON:
                            //Draw the quit game string
                            spriteBatch.DrawString(menuBtnFont, "Quit Game", new Vector2(menuBtnXLocation + 27,
                                 menuBtnYLocations[i] + 22), Color.White);
                            break;
                    }
                }
            }

            //Ends the sprite batch
            spriteBatch.End();
        }



        //Pre: If the user is hovering over any buttons
        //Post: The AI picking menu is drawn
        //Desc: A subprogram which draws the menu for the type of AI game to start, either new game or loading a game
        private void DrawAIUserPick()
        {
            //Begins the sprite batch
            spriteBatch.Begin();

            //Draws the background
            spriteBatch.Draw(background, backgroundSize, Color.White);

            //Loop for the menu buttons
            for (int i = FIRST_AI_PICK_BUTTON; i < isMenuBtnHovering.Length; i++)
            {
                //If the current button is being hovered over
                if (isMenuBtnHovering[i])
                {
                    //Draws the current button enlarged
                    spriteBatch.Draw(buttons, mainMenuHoverRec[i], Color.Red);
                    spriteBatch.Draw(buttons, mainMenuHoverRec[i + isMenuBtnHovering.Length], Color.Black);

                    //Switch statement for which button is currently being checked
                    switch (i)
                    {
                        //If it's the first button
                        case FIRST_AI_PICK_BUTTON:
                            //Draw the new game string
                            spriteBatch.DrawString(menuBtnHoverFont, "New Game", new Vector2(menuHoverBtnXLocation + 28,
                                menuHoverBtnYLoactions[i] + 30), Color.Yellow);
                            break;
                        //If it's the second button
                        case SECOND_AI_PICK_BUTTON:
                            //Draw the load game string
                            spriteBatch.DrawString(menuBtnHoverFont, "Load Game", new Vector2(menuHoverBtnXLocation + 27,
                                menuHoverBtnYLoactions[i] + 30), Color.Yellow);
                            break;
                        //If it's the third button
                        case THIRD_AI_PICK_BUTTON:
                            //Draw the go back string
                            spriteBatch.DrawString(menuBtnHoverFont, "Go Back", new Vector2(menuHoverBtnXLocation + 45,
                                menuHoverBtnYLoactions[i] + 30), Color.Yellow);
                            break;
                    }
                }
                //If the current button is not being hovered over
                else
                {
                    //Draws the current button normally
                    spriteBatch.Draw(buttons, mainMenuRec[i], Color.Red);
                    spriteBatch.Draw(buttons, mainMenuRec[i + isMenuBtnHovering.Length], Color.Black);

                    //Switch statement for which button is currently being check
                    switch (i)
                    {
                        //If it's the first button
                        case FIRST_AI_PICK_BUTTON:
                            //Draw the new game string
                            spriteBatch.DrawString(menuBtnFont, "New Game", new Vector2(menuBtnXLocation + 27,
                                menuBtnYLocations[i] + 22), Color.White);
                            break;
                        //If it's the second button
                        case SECOND_AI_PICK_BUTTON:
                            //Draw the load game string
                            spriteBatch.DrawString(menuBtnFont, "Load Game", new Vector2(menuBtnXLocation + 25,
                                menuBtnYLocations[i] + 22), Color.White);
                            break;
                        //If it's the third button
                        case THIRD_AI_PICK_BUTTON:
                            //Draw the go back string
                            spriteBatch.DrawString(menuBtnFont, "Go Back", new Vector2(menuBtnXLocation + 42,
                                menuBtnYLocations[i] + 22), Color.White);
                            break;
                    }
                }
            }

            //Ends the sprite batch
            spriteBatch.End();
        }



        //Pre: What game cards to draw
        //Post: The game cards are drawn for the user to see
        //Desc: A subprogram which draws all the cards (community and hand cards) for the user to see
        private void DrawGameCards()
        {
            //Variable for the current card that's being drawn
            int currentCard = 0;

            //Variable for where the card starts being drawn and the angle it's being drawn in
            float rotationAngle = 0;
            Vector2 origin;

            //Variables for the first and second cards of the user
            int usersFirstCard = userPlayer * 2;
            int usersSecondCard = usersFirstCard + 1;

            //Updates the values of the origin
            origin.X = 0;
            origin.Y = 0;

            //Begins the sprite batch
            spriteBatch.Begin();

            //Draws the poker table background
            spriteBatch.Draw(pokerTable, backgroundSize, Color.White);

            //Loop for every player that could be playing
            for (int i = 0; i < playerPlaying.Length; i++)
            {
                //If the current player is playing and they have not folded
                if (playerPlaying[i] == true && playersFolded[i] == false)
                {
                    //Loop for the first and second card of the current player
                    for (int j = 0; j < 2; j++)
                    {
                        //Switch statement for the current card that's being checked
                        switch (currentCard)
                        {
                            //If the current card is either the 1st or 2nd card
                            case 0:
                            case 1:
                                //Updates the rotation angle
                                rotationAngle = 0.28F;
                                break;
                            //If the current card is either the 3rd or 4th card
                            case 2:
                            case 3:
                                //Updates the rotation angle
                                rotationAngle = 0.8F;
                                break;
                            //If the current card is either the 5th or 6th card
                            case 4:
                            case 5:
                                //Updates the rotation angle
                                rotationAngle = 2.4F;
                                break;
                            //If the current card is either the 7th or 8th card
                            case 6:
                            case 7:
                                //Updates the rotation angle
                                rotationAngle = 5.95F;
                                break;
                            //If the current card is either the 11th or 12th card
                            case 10:
                            case 11:
                                //Updates the rotation angle
                                rotationAngle = 0.3F;
                                break;
                            //If the current card is either the 13th or 14th card
                            case 12:
                            case 13:
                                //Updates the rotation angle
                                rotationAngle = 0.8F;
                                break;
                            //If the current card is either the 15th or 16th card
                            case 14:
                            case 15:
                                //Updates the rotation angle
                                rotationAngle = 2.4F;
                                break;
                            //If the current card is either the 17th or 18th card
                            case 16:
                            case 17:
                                //Updates the rotation angle
                                rotationAngle = 2.8F;
                                break;
                        }

                        //If the current card that's being checked is the user's card
                        if (i == userPlayer && currentCard == usersFirstCard || currentCard == usersSecondCard)
                        {
                            //If the user is player 4
                            if (userPlayer == 4)
                            {
                                //Draws the card
                                spriteBatch.Draw(deckOfCards[PlayerCardToDraw(i, j)], playerCardsRec[currentCard], Color.White);
                            }
                            //if the user is not player 4
                            else
                            {
                                //Draws and rotates the card
                                spriteBatch.Draw(deckOfCards[PlayerCardToDraw(i, j)], playerCardsRec[currentCard], null, Color.White, rotationAngle, origin, SpriteEffects.None, 0);
                            }
                        }
                        //If the current card that's being checked is not the user's
                        else
                        {
                            //Draws and rotates the player's card
                            spriteBatch.Draw(backOfCard, playerCardsRec[currentCard], null, Color.White, rotationAngle, origin, SpriteEffects.None, 0);
                        }

                        //Adds on the the current card
                        currentCard++;
                    }
                }
                //If the current player is not playing anymore and/or they have already folded
                else
                {
                    //Adds two to the current card variable
                    currentCard += 2;
                }
            }

            //Loop for every single community card
            for (int i = 0; i < communityCardCount; i++)
            {
                //Draws the card
                spriteBatch.Draw(deckOfCards[CommunityCardToDraw(i)], new Rectangle(communityCardXLocations[i], communityCardYLocation,
                    cardXSize, cardYSize), Color.White);
            }

            //if there is 3 or more community cards
            if (communityCardCount >= 3)
            {
                //Draws the burn cards
                spriteBatch.Draw(backOfCard, new Rectangle(communityCardXLocations[0] - 80, communityCardYLocation,
                    cardXSize, cardYSize), Color.White);
            }

            //If there is 4 or more community cards
            if (communityCardCount >= 4)
            {
                //Draws the burn cards
                spriteBatch.Draw(backOfCard, new Rectangle(communityCardXLocations[0] - 80, communityCardYLocation - 1,
                    cardXSize, cardYSize), Color.White);
            }

            //If there is 5 community cards
            if (communityCardCount == 5)
            {
                //Draws the burn cards
                spriteBatch.Draw(backOfCard, new Rectangle(communityCardXLocations[0] - 80, communityCardYLocation - 2,
                    cardXSize, cardYSize), Color.White);
            }

            //Ends the sprite batch
            spriteBatch.End();
        }



        //Pre: If the user is hovering over any buttons
        //Post: The betting choices for the user are drawn
        //Desc: A subporgram which draws all the betting choices for the user so they know what they can do
        private void DrawBettingChoices()
        {
            //Variable for the current button that is being checked
            int betBtnNum = 0;

            //Begins a sprite batch
            spriteBatch.Begin();

            //Loop through the Y locations of the buttons
            for (int i = 0; i < betBtnYLocation.Length; i++)
            {
                //Loop through the X locations of the buttons
                for (int j = 0; j < betBtnXLocation.Length; j++)
                {
                    //If the user has folded or it is not the user's turn or the user has no money and they did not fold or the user is all in or the chips have not joined 
                    //together but, they need to join or the chips for the user are currently moving 
                    if (playersFolded[userPlayer] == true || currentTurn != userPlayer || playerMoney[userPlayer] == 0 && playersFolded[userPlayer] == false ||
                        playersAllIn[userPlayer] == true || playerChipsJoined == false && playerChipsNeedJoin == true || playerChipsMoving[userPlayer] == true)
                    {
                        //If the current button thats being checked is the plus button to raise
                        if (i == betBtnYLocation.Length - 1 && j == 0)
                        {
                            //Draws the plus button to raise
                            spriteBatch.Draw(buttons, new Rectangle(betBtnXLocation[j] + 10, betBtnYLocation[i], raiseBtnSize, betBtnYSize), Color.Black);
                            spriteBatch.Draw(buttons, new Rectangle(betBtnXLocation[j] + 12, betBtnYLocation[i] + 2, raiseBtnSize - 4, betBtnYSize - 4), Color.Gray);

                            //Draws the minus button to lower the raise
                            spriteBatch.Draw(buttons, new Rectangle(minusBtnXLocation, betBtnYLocation[i], raiseBtnSize, betBtnYSize), Color.Black);
                            spriteBatch.Draw(buttons, new Rectangle(minusBtnXLocation + 2, betBtnYLocation[i] + 2, raiseBtnSize - 4, betBtnYSize - 4), Color.Gray);
                        }
                        //If the current button thats being checked is not the plus button 
                        else
                        {
                            //Draws a regular button
                            spriteBatch.Draw(buttons, new Rectangle(betBtnXLocation[j], betBtnYLocation[i], betBtnXSize, betBtnYSize), Color.Black);
                            spriteBatch.Draw(buttons, new Rectangle(betBtnXLocation[j] + 2, betBtnYLocation[i] + 2, betBtnXSize - 4, betBtnYSize - 4), Color.Gray);
                        }
                    }
                    //If it's the users turn to go and they are able to make a move
                    else
                    {
                        //If the current button thats being checked is the plus button to raise
                        if (i == betBtnYLocation.Length - 1 && j == 0)
                        {
                            //If the current bet is smaller then the amount of money the user has
                            if (currentBet < playerMoney[userPlayer])
                            {
                                //If the plus button is being hovered over
                                if (isBetBtnHovering[4] == true)
                                {
                                    //Draws the enlarged button
                                    spriteBatch.Draw(buttons, new Rectangle(betBtnXLocation[j] + 4, betBtnYLocation[i] - 6, raiseBtnSize + 12, betBtnYSize + 12), Color.Red);
                                    spriteBatch.Draw(buttons, new Rectangle(betBtnXLocation[j] + 6, betBtnYLocation[i] - 4, raiseBtnSize + 8, betBtnYSize + 8), Color.Black);
                                }
                                //If the plus button is not being hovered over
                                else
                                {
                                    //Draws the normal button
                                    spriteBatch.Draw(buttons, new Rectangle(betBtnXLocation[j] + 10, betBtnYLocation[i], raiseBtnSize, betBtnYSize), Color.Red);
                                    spriteBatch.Draw(buttons, new Rectangle(betBtnXLocation[j] + 12, betBtnYLocation[i] + 2, raiseBtnSize - 4, betBtnYSize - 4), Color.Black);
                                }

                                //If the minus button is being hovered over
                                if (isBetBtnHovering[5] == true)
                                {
                                    //Draws the enlarged button
                                    spriteBatch.Draw(buttons, new Rectangle(minusBtnXLocation - 6, betBtnYLocation[i] - 6, raiseBtnSize + 12, betBtnYSize + 12), Color.Red);
                                    spriteBatch.Draw(buttons, new Rectangle(minusBtnXLocation - 4, betBtnYLocation[i] - 4, raiseBtnSize + 8, betBtnYSize + 8), Color.Black);
                                }
                                //If the minus button is not being hovered over
                                else
                                {
                                    //Draws the normal button
                                    spriteBatch.Draw(buttons, new Rectangle(minusBtnXLocation, betBtnYLocation[i], raiseBtnSize, betBtnYSize), Color.Red);
                                    spriteBatch.Draw(buttons, new Rectangle(minusBtnXLocation + 2, betBtnYLocation[i] + 2, raiseBtnSize - 4, betBtnYSize - 4), Color.Black);
                                }
                            }

                            //Adds one to the current button that's checked
                            betBtnNum++;
                        }
                        //If the current button that is being checked is not the plus button
                        else
                        {
                            //If the current bet is greater then or equal to the amount of money the player has
                            if (currentBet >= playerMoney[userPlayer])
                            {
                                //If it's the first row of buttons that is being checked
                                if (i == 0)
                                {
                                    //If the current button is being hovered over
                                    if (isBetBtnHovering[betBtnNum] == true)
                                    {
                                        //Draw the button enlarged
                                        spriteBatch.Draw(buttons, new Rectangle(betBtnXLocation[j] - 6, betBtnYLocation[i] - 6, betBtnXSize + 12, betBtnYSize + 12), Color.Red);
                                        spriteBatch.Draw(buttons, new Rectangle(betBtnXLocation[j] - 4, betBtnYLocation[i] - 4, betBtnXSize + 8, betBtnYSize + 8), Color.Black);
                                    }
                                    //If the current button is not being hovered over
                                    else
                                    {
                                        //Draw the button normally
                                        spriteBatch.Draw(buttons, new Rectangle(betBtnXLocation[j], betBtnYLocation[i], betBtnXSize, betBtnYSize), Color.Red);
                                        spriteBatch.Draw(buttons, new Rectangle(betBtnXLocation[j] + 2, betBtnYLocation[i] + 2, betBtnXSize - 4, betBtnYSize - 4), Color.Black);
                                    }
                                }
                            }
                            //If the current bet is not greater then or equal to the amount of money the player has
                            else
                            {
                                //If it's the second button on the second row that's being checked
                                if (i == 1 && j == 1)
                                {
                                    //Draws the button with a white border
                                    spriteBatch.Draw(buttons, new Rectangle(betBtnXLocation[j], betBtnYLocation[i], betBtnXSize, betBtnYSize), Color.White);
                                    spriteBatch.Draw(buttons, new Rectangle(betBtnXLocation[j] + 2, betBtnYLocation[i] + 2, betBtnXSize - 4, betBtnYSize - 4), Color.Black);
                                }
                                //If it's any other button
                                else
                                {
                                    //If the button is being hovered over
                                    if (isBetBtnHovering[betBtnNum] == true)
                                    {
                                        //Draws the button enlarged
                                        spriteBatch.Draw(buttons, new Rectangle(betBtnXLocation[j] - 6, betBtnYLocation[i] - 6, betBtnXSize + 12, betBtnYSize + 12), Color.Red);
                                        spriteBatch.Draw(buttons, new Rectangle(betBtnXLocation[j] - 4, betBtnYLocation[i] - 4, betBtnXSize + 8, betBtnYSize + 8), Color.Black);
                                    }
                                    //If it's not being hovered over
                                    else
                                    {
                                        //Draws the button normally
                                        spriteBatch.Draw(buttons, new Rectangle(betBtnXLocation[j], betBtnYLocation[i], betBtnXSize, betBtnYSize), Color.Red);
                                        spriteBatch.Draw(buttons, new Rectangle(betBtnXLocation[j] + 2, betBtnYLocation[i] + 2, betBtnXSize - 4, betBtnYSize - 4), Color.Black);
                                    }
                                }
                            }
                        }
                    }

                    //Adds one to the amount of buttons checked
                    betBtnNum++;
                }
            }

            //If it's not the user's turn or the user has folded or the user has no money and they have not folded or the user is all in
            if (currentTurn != userPlayer || playersFolded[userPlayer] || playerMoney[userPlayer] == 0 && playersFolded[userPlayer] == false || playersAllIn[userPlayer] == true)
            {
                //Draws all the text for the buttons with a light grey font 
                spriteBatch.DrawString(betBtnFont, "Check", new Vector2(betBtnXLocation[0] + 43, betBtnYLocation[0] + 4), Color.LightGray);
                spriteBatch.DrawString(betBtnFont, "Fold", new Vector2(betBtnXLocation[1] + 53, betBtnYLocation[0] + 4), Color.LightGray);
                spriteBatch.DrawString(betBtnFont, "Raise", new Vector2(betBtnXLocation[0] + 48, betBtnYLocation[1] + 4), Color.LightGray);
                spriteBatch.DrawString(betBtnFont, "$0", new Vector2(betBtnXLocation[1] + 63, betBtnYLocation[1] + 4), Color.LightGray);
                spriteBatch.DrawString(betBtnFont, "+", new Vector2(betBtnXLocation[0] + 36, betBtnYLocation[2] + 4), Color.LightGray);
                spriteBatch.DrawString(betBtnFont, "-", new Vector2(minusBtnXLocation + 28, betBtnYLocation[2] + 4), Color.LightGray);
                spriteBatch.DrawString(betBtnFont, "ALL IN", new Vector2(betBtnXLocation[1] + 33, betBtnYLocation[2] + 4), Color.LightGray);
            }
            //If it is the user's turn to pick options
            else
            {
                //If the current bet is greater then or equal to the amount of money that player has plus their bet
                if (currentBet >= playerMoney[userPlayer] + playerBet[userPlayer])
                {
                    //If the current button is being hovered over
                    if (isBetBtnHovering[0] == true)
                    {
                        //Draws the text in yellow and enlarged
                        spriteBatch.DrawString(betBtnHoverFont, "ALL IN", new Vector2(betBtnXLocation[0] + 27, betBtnYLocation[0] - 2), Color.Yellow);
                    }
                    //If it's not being hovered over
                    else
                    {
                        //Draws the string normally
                        spriteBatch.DrawString(betBtnFont, "ALL IN", new Vector2(betBtnXLocation[0] + 33, betBtnYLocation[0] + 4), Color.White);
                    }
                }
                //If the current bet is greater then the user's bet
                else if (currentBet > playerBet[userPlayer])
                {
                    //If the current button is being hovered over
                    if (isBetBtnHovering[0] == true)
                    {
                        //Draws the text in yellow and enlarged
                        spriteBatch.DrawString(betBtnHoverFont, "Call $" + (currentBet - playerBet[userPlayer]), new Vector2(betBtnXLocation[0] + 24, betBtnYLocation[0] - 2), Color.Yellow);
                    }
                    //If it's not being hovored over
                    else
                    {
                        //Draws the text normally
                        spriteBatch.DrawString(betBtnFont, "Call $" + (currentBet - playerBet[userPlayer]), new Vector2(betBtnXLocation[0] + 30, betBtnYLocation[0] + 4), Color.White);
                    }
                }
                //If bet's are anything else
                else
                {
                    //If the current button is being hovered over
                    if (isBetBtnHovering[0] == true)
                    {
                        //Draws the text in yellow and enlarged
                        spriteBatch.DrawString(betBtnHoverFont, "Check", new Vector2(betBtnXLocation[0] + 37, betBtnYLocation[0] - 2), Color.Yellow);
                    }
                    //If it's not being hovered over
                    else
                    {
                        //Draws the text normally
                        spriteBatch.DrawString(betBtnFont, "Check", new Vector2(betBtnXLocation[0] + 43, betBtnYLocation[0] + 4), Color.White);
                    }
                }

                //If the fold button is being hovered over
                if (isBetBtnHovering[1] == true)
                {
                    //Draws the fold text in yellow and enlarged
                    spriteBatch.DrawString(betBtnHoverFont, "Fold", new Vector2(betBtnXLocation[1] + 47, betBtnYLocation[0] - 2), Color.Yellow);
                }
                //If the fold button is not being hovered over
                else
                {
                    //Draws the folded text normally
                    spriteBatch.DrawString(betBtnFont, "Fold", new Vector2(betBtnXLocation[1] + 53, betBtnYLocation[0] + 4), Color.White);
                }

                //If the current bet is less then the amount of money the player has
                if (currentBet < playerMoney[userPlayer])
                {
                    //If the current button is being hovered over
                    if (isBetBtnHovering[2] == true)
                    {
                        //Draws the text in yellow and enlarged
                        spriteBatch.DrawString(betBtnHoverFont, "Raise", new Vector2(betBtnXLocation[0] + 42, betBtnYLocation[1] - 2), Color.Yellow);
                    }
                    //If the current button is not being hovered over
                    else
                    {
                        //Draws the text normally
                        spriteBatch.DrawString(betBtnFont, "Raise", new Vector2(betBtnXLocation[0] + 48, betBtnYLocation[1] + 4), Color.White);
                    }

                    //If the raise amount is greater then or equal to 1000
                    if (userRaiseAmount >= 1000)
                    {
                        //Draws the raise amount
                        spriteBatch.DrawString(betBtnFont, "$" + userRaiseAmount, new Vector2(betBtnXLocation[1] + 44, betBtnYLocation[1] + 4), Color.White);
                    }
                    //If the raise amount is greater then or equal to 100
                    else if (userRaiseAmount >= 100)
                    {
                        //Draws the raise amount
                        spriteBatch.DrawString(betBtnFont, "$" + userRaiseAmount, new Vector2(betBtnXLocation[1] + 50, betBtnYLocation[1] + 4), Color.White);
                    }
                    //If the raise amount is greater then or equal to 10
                    else if (userRaiseAmount >= 10)
                    {
                        //Draws the raise amount
                        spriteBatch.DrawString(betBtnFont, "$" + userRaiseAmount, new Vector2(betBtnXLocation[1] + 55, betBtnYLocation[1] + 4), Color.White);
                    }
                    //If the raise amount is anything else
                    else
                    {
                        //Draws the raise amount
                        spriteBatch.DrawString(betBtnFont, "$" + userRaiseAmount, new Vector2(betBtnXLocation[1] + 63, betBtnYLocation[1] + 4), Color.White);
                    }

                    //If the plus button is being hovered over
                    if (isBetBtnHovering[4] == true)
                    {
                        //Draw the plus text in yellow and enlarged
                        spriteBatch.DrawString(betBtnHoverFont, "+", new Vector2(betBtnXLocation[0] + 33, betBtnYLocation[2] - 2), Color.Yellow);
                    }
                    //If the plus button is not being hovered over
                    else
                    {
                        //Draws the plus text normally
                        spriteBatch.DrawString(betBtnFont, "+", new Vector2(betBtnXLocation[0] + 36, betBtnYLocation[2] + 4), Color.White);
                    }

                    //If the minus button is being hovered over
                    if (isBetBtnHovering[5] == true)
                    {
                        //Draw the minus text in yellow and enlarged
                        spriteBatch.DrawString(betBtnHoverFont, "-", new Vector2(minusBtnXLocation + 27, betBtnYLocation[2] - 2), Color.Yellow);
                    }
                    //If the minus button is not being hovered over
                    else
                    {
                        //Draws the minus text normally
                        spriteBatch.DrawString(betBtnFont, "-", new Vector2(minusBtnXLocation + 28, betBtnYLocation[2] + 4), Color.White);
                    }

                    //If the all in button is being hovered over
                    if (isBetBtnHovering[6] == true)
                    {
                        //Draws the all in text in yellow and enlarged
                        spriteBatch.DrawString(betBtnHoverFont, "ALL IN", new Vector2(betBtnXLocation[1] + 27, betBtnYLocation[2] - 2), Color.Yellow);
                    }
                    //If the all in button is not being hovered ovr
                    else
                    {
                        //Draws the all in text normally
                        spriteBatch.DrawString(betBtnFont, "ALL IN", new Vector2(betBtnXLocation[1] + 33, betBtnYLocation[2] + 4), Color.White);
                    }
                }
            }

            //End the sprite batch
            spriteBatch.End();
        }



        //Pre: The player number and the card number
        //Post: Returns the card that needs to be displayed
        //Desc: A function that returns the card which will be displayed for the user to see
        private int PlayerCardToDraw(int playerNumber, int cardNumber)
        {
            //Variable for what card will be displayed
            int cardToDisplay = 0;

            //Switch statement for the suit of the current card
            switch (playerCards[playerNumber, cardNumber, 1])
            {
                //If the suit is clubs
                case 1:
                    //Give a value for what card to display
                    cardToDisplay = playerCards[playerNumber, cardNumber, 0] - 1;
                    break;
                //If the suit is diamonds
                case 2:
                    //Give a value for what card to display
                    cardToDisplay = playerCards[playerNumber, cardNumber, 0] + 12;
                    break;
                //If the suit is hearts
                case 3:
                    //Give a value for what card to display
                    cardToDisplay = playerCards[playerNumber, cardNumber, 0] + 25;
                    break;
                //If the suit is spades
                case 4:
                    //Give a value for what card to display
                    cardToDisplay = playerCards[playerNumber, cardNumber, 0] + 38;
                    break;
            }

            //Return the card to display
            return cardToDisplay;
        }



        //Pre: The card number that will be displayed
        //Post: Returns the card that needs to be displayed
        //Desc: A function that returns the card which will be displayed for the user to see
        private int CommunityCardToDraw(int cardNumber)
        {
            //Variable for what card will be displayed
            int cardToDisplay = 0;

            //Switch statement for the suit of the current community card
            switch (communityCards[cardNumber, 1])
            {
                //If the suit is clubs
                case 1:
                    //Give a value for what card to display
                    cardToDisplay = communityCards[cardNumber, 0] - 1;
                    break;
                //If the suit is diamonds
                case 2:
                    //Give a value for what card to display
                    cardToDisplay = communityCards[cardNumber, 0] + 12;
                    break;
                //If the suit is hearts
                case 3:
                    //Give a value for what card to display
                    cardToDisplay = communityCards[cardNumber, 0] + 25;
                    break;
                //If the suit is spades
                case 4:
                    //Give a value for what card to display
                    cardToDisplay = communityCards[cardNumber, 0] + 38;
                    break;
            }

            //Return the card to display
            return cardToDisplay;
        }



        //Pre: The state of the background worker
        //Post: The sound effect is played
        //Desc: A subporgram which pauses the background music and plays sound effects
        private void soundBWDoWork(object sender, DoWorkEventArgs e)
        {
            //Pauses the background music and set the variable that says it is playing to false
            MediaPlayer.Pause();
            backgroundMusicPlaying = false;

            //Plays the sound effect
            //usersTurnSound.PlaySync();
        }



        //Pre: The state of the background worker
        //Post: The sound effect is stopped and the background music is resumed
        //Desc: A subprogram which stops the sound effect and resumes the background music
        private void soundBWRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Stops the sound effect from playing
            usersTurnSound.Stop();

            //If the current turn is not the users 
            if (currentTurn != userPlayer)
            {
                //Set the sound to not playing
                usersTurnSoundPlayed = false;
            }

            //Resume the background music and set the variable that says it is playing to true
            MediaPlayer.Resume();
            backgroundMusicPlaying = true;

            //Stop the background worker
            soundBW.CancelAsync();
        }



        //Pre: None
        //Post: The game is reset
        //Desc: A subprogram which resets the game
        private void ResetGame()
        {
            //Sets the game to started so everything will be drawn and new game and new round to false so it doesn't keep resetting
            gameStarted = true;
            newGame = false;
            newRound = false;

            //Sets the last player to not checked
            lastPlayerCheck = false;

            //If the game state is currently on network game
            if (gameState == NETWORK_GAME)
            {
                //Set the clients ready to 0
                clientsReadyCount = 0;

                //Loop for every client that could be ready
                for (int i = 0; i < clientsReady.Length; i++)
                {
                    //If the current client is ready
                    if (clientsReady[i])
                    {
                        //Add one to the amount of clients that are ready
                        clientsReadyCount++;
                    }
                }

                //Sets the amount of players left to the amount of clients who are ready
                playersLeft = clientsReadyCount;

                //For loop for the amount of clients who are playing
                for (int i = 0; i < clientsReadyCount; i++)
                {
                    //Resets all their values because it's a new game
                    playerPlaying[i] = true;
                    playersFolded[i] = false;
                    playersBluff[i] = false;
                    playersBluffChecked[i] = false;
                    playersAllIn[i] = false;
                    playerChipsMoving[i] = false;
                    playerMoney[i] = 1000;
                    playerBet[i] = 0;
                }

                //For loop for all the players that are not playing
                for (int i = clientsReadyCount; i < clientsReady.Length; i++)
                {
                    //Sets all their values to 0 or false because that are not playing
                    playerPlaying[i] = false;
                    playersFolded[i] = false;
                    playersBluff[i] = false;
                    playersBluffChecked[i] = false;
                    playersAllIn[i] = false;
                    playerChipsMoving[i] = false;
                    playerMoney[i] = 0;
                    playerBet[i] = 0;
                }
            }
            //If the game state is not on network game
            else
            {
                //Resets the amount of players left and which player the user is playing as
                playersLeft = 9;
                userPlayer = 4;

                //Loop for the 9 possible players playing
                for (int i = 0; i < playerPlaying.Length; i++)
                {
                    //Resets all their values because it's a new game
                    playerPlaying[i] = true;
                    playersFolded[i] = false;
                    playersBluff[i] = false;
                    playersBluffChecked[i] = false;
                    playersAllIn[i] = false;
                    playerChipsMoving[i] = false;
                    playerMoney[i] = 1000;
                    playerBet[i] = 0;
                }

                //Sets the chips to have joined together and that they don't need a join
                playerChipsJoined = true;
                playerChipsNeedJoin = false;
            }

            //Sets the next available card in the deck to 0
            nextCardInDeck = 0;

            //Sets the amount of comminuty cards to 0 and the amount of players who are all in to 0
            communityCardCount = 0;
            playersAllInCount = 0;

            //Clear all the Arrays of the game
            Array.Clear(shuffleCards, 0, shuffleCards.Length);
            Array.Clear(playerCards, 0, playerCards.Length);
            Array.Clear(communityCards, 0, communityCards.Length);
            Array.Clear(handQuality, 0, handQuality.Length);
            Array.Clear(handKicker, 0, handKicker.Length);

            //Clear the lists
            chipPots.Clear();
            playersCanWin.Clear();
            playersTied.Clear();

            //If the game state is no network game
            if (gameState == NETWORK_GAME)
            {
                //Gets randomly the small blind player
                smallBlindPlayer = randInt.Next(0, clientsReadyCount);

                //If the small blind player is the last player on the table
                if (smallBlindPlayer == clientsReadyCount - ONE_PLAYER)
                {
                    //Set the big blind player to be the first player
                    bigBlindPlayer = FIRST_PLAYER;
                }
                //If the small blind player is any other player
                else
                {
                    //Sets the big bling player to be the next possible player
                    bigBlindPlayer = NextPossibleTurn(smallBlindPlayer);
                }

                //If the big blind player is the last player on the table
                if (bigBlindPlayer == clientsReadyCount - 1)
                {
                    //Sets the current turn to be the first player
                    currentTurn = FIRST_PLAYER;
                }
                //If the big blind player is not the last player on the tablek
                else
                {
                    //Sets the current turn to be the next possible player
                    currentTurn = NextPossibleTurn(bigBlindPlayer);
                }
            }
            //If the game state is not on network game
            else
            {
                //Sets the current turn to be any one of the 9 players
                currentTurn = randInt.Next(FIRST_PLAYER, 9);

                //if the current turn is the first player
                if (currentTurn == FIRST_PLAYER)
                {
                    //Set the big blind to be the last player
                    bigBlindPlayer = 8;
                }
                //If the current turn is anything else
                else
                {
                    //Sets the big blind player to be one less then the current turn
                    bigBlindPlayer = currentTurn - ONE_PLAYER;
                }

                //If the big blind player is the first player
                if (bigBlindPlayer == FIRST_PLAYER)
                {
                    //Sets the small blind player to be the last player
                    smallBlindPlayer = 8;
                }
                //If the big blind player is any other playerk
                else
                {
                    //Sets the small blind player to be one less then the big blind player
                    smallBlindPlayer = bigBlindPlayer - 1;
                }
            }

            //Resets the blind amount and amount of turns passed since the blinds were changed
            blindAmount = 10;
            turnCount = 1;
            playersTurnCount = 0;

            //Resets the current bet, user raise amount, and current pay out
            currentBet = 0;
            userRaiseAmount = 0;
            currentPayOut = 0;

            //Resets the player who won and the amount of players who tied
            playerWon = 0;
            playersTiedCount = 0;

            //Resets all the times
            playerTime.Reset();
            timeLeft = ROUND_START_TIME;
            oldTimeLeft = ROUND_START_TIME;

            //If the game state is on network game
            if (gameState == NETWORK_GAME)
            {
                //Loop for every player that is playing
                for (int i = FIRST_PLAYER; i < clientsReadyCount; i++)
                {
                    //Sets the name of the player
                    playerNames[i] = clientNames[i];
                }
            }
            //If the game state is not on network game
            else
            {
                //Loop for every player that is playing
                for (int i = FIRST_PLAYER; i < playerNames.Length; i++)
                {
                    //If the current player is not the user
                    if (i != 4)
                    {
                        //Set the name of the player to be a random one out of the possible list 
                        playerNames[i] = possiblePlayerNames[randInt.Next(0, possiblePlayerNames.Count)];
                    }
                    //If the current player is the user
                    else
                    {
                        //Set the name of the player to user
                        playerNames[i] = "User";
                    }
                }
            }

            //Call the shuffle cards subprogram to shuffle the cards
            ShuffleCards();

            //Call the deal current round subprogram to deal the hands
            DealCurrentRound();

            //If the game state is not a network game
            if (gameState != NETWORK_GAME)
            {
                //Loop for every player
                for (int i = 0; i < playerPlaying.Length; i++)
                {
                    //Set the X and Y locations of the player's chips
                    SetChipMovingValues(i);
                }
            }

            //Sets the current bet equal to blind amount because of the big blind
            currentBet = blindAmount;

            //Sets the big blind and small bling player bets
            playerBet[bigBlindPlayer] = blindAmount;
            playerBet[smallBlindPlayer] = (int)(blindAmount / 2);

            //Deducts money from the small and big blind players
            playerMoney[bigBlindPlayer] -= blindAmount;
            playerMoney[smallBlindPlayer] -= (int)(blindAmount / 2);

            //Adds a pot for the chips to go into
            chipPots.Add(0);
            playersCanWin.Add(new bool[9]);
        }



        //Pre: None
        //Post: Resets all the variables for the round
        //Desc: A subprogram which resets the round so a new round is played
        private void ResetRound()
        {
            //Sets the new round to false
            newRound = false;

            //Sets the last player to not checked
            lastPlayerCheck = false;

            //For loop for every player
            for (int i = 0; i < playersFolded.Length; i++)
            {
                //Resets the values for every player so a new round can be started
                playersFolded[i] = false;
                playersBluff[i] = false;
                playersBluffChecked[i] = false;
                playersAllIn[i] = false;
                playerChipsMoving[i] = false;
                playerBet[i] = 0;
            }

            //Sets the community cards to none and resets the next card in deck variable
            communityCardCount = 0;
            nextCardInDeck = 0;

            //Sets the amount of players all in to none
            playersAllInCount = 0;

            //Clear all the Arrays of the game
            Array.Clear(playerCards, 0, playerCards.Length);
            Array.Clear(communityCards, 0, communityCards.Length);
            Array.Clear(handQuality, 0, handQuality.Length);
            Array.Clear(shuffleCards, 0, shuffleCards.Length);
            Array.Clear(handKicker, 0, handKicker.Length);

            //Clear the lists
            chipPots.Clear();
            playersCanWin.Clear();
            playersTied.Clear();

            //Gives a value to the big blind player, small blind player, and the current turn
            bigBlindPlayer = NextPossibleTurn(bigBlindPlayer);
            smallBlindPlayer = NextPossibleTurn(smallBlindPlayer);
            currentTurn = NextPossibleTurn(bigBlindPlayer);

            //If the turn amount is equal to 15
            if (turnCount == 15)
            {
                //Set the turn amount equal to 0
                turnCount = 0;

                //If the blind amount is equal to 20
                if (blindAmount == 20)
                {
                    //Set the blind amount equal to 50
                    blindAmount = 50;
                }
                //If the blind amount is anything else
                else
                {
                    //Set it to double it's current amount
                    blindAmount += blindAmount;
                }
            }
            //If it's not equal to 15
            else
            {
                //Add one to the turn count
                turnCount++;
            }

            //If the game state is not on network game
            if (gameState != NETWORK_GAME)
            {
                //Set the chips to joined and not needing to join anymore
                playerChipsJoined = true;
                playerChipsNeedJoin = false;
            }

            //Set the amount of turns this round to 0
            playersTurnCount = 0;

            //Set the current bet to 0, raise amount to 0, and the current pay out to 0
            currentBet = 0;
            userRaiseAmount = 0;
            currentPayOut = 0;

            //Set the player won to one and the amount of players tied to none
            playerWon = 0;
            playersTiedCount = 0;

            //Reset the times
            playerTime.Reset();
            timeLeft = ROUND_START_TIME;
            oldTimeLeft = ROUND_START_TIME;

            //Shuffle the cards
            ShuffleCards();

            //Deal the cards
            DealCurrentRound();

            //If the game state is not network game
            if (gameState != NETWORK_GAME)
            {
                //Loop for every player
                for (int i = 0; i < playerPlaying.Length; i++)
                {
                    //Set their chips location values
                    SetChipMovingValues(i);
                }
            }

            //Set the current bet equal to the blind amount
            currentBet = blindAmount;

            //Set the bets of the big and small blind players
            playerBet[bigBlindPlayer] = blindAmount;
            playerBet[smallBlindPlayer] = (int)(blindAmount / 2);

            //Deduct money from the big and small blind players
            playerMoney[bigBlindPlayer] -= blindAmount;
            playerMoney[smallBlindPlayer] -= (int)(blindAmount / 2);

            //Add a new pot
            chipPots.Add(0);
            playersCanWin.Add(new bool[9]);
        }



        //Pre: The current turn 
        //Post: The next players turn that it could be
        //Desc: A subprogram which returns the next player whose turn it could be
        private int NextPossibleTurn(int currentPlayerTurn)
        {
            //If the current player's turn is the last player
            if (currentPlayerTurn == 8)
            {
                //Loop for every player
                for (int i = 0; i < playerPlaying.Length; i++)
                {
                    //If the current player is playing and they have not folded
                    if (playerPlaying[i] == true && playersFolded[i] == false)
                    {
                        //Set the current player's turn to this player
                        currentPlayerTurn = i;

                        //Set the value of the i to the amount of players to stop the for loop
                        i = playerPlaying.Length;
                    }
                }
            }
            //If the current turn is not the last player's
            else
            {
                //Loop for all the players after the current turn
                for (int i = currentPlayerTurn + 1; i < playerPlaying.Length; i++)
                {
                    //If the current player is playing and they have not folded
                    if (playerPlaying[i] == true && playersFolded[i] == false)
                    {
                        //Set the current turn to this player
                        currentPlayerTurn = i;

                        //Stop the for loop
                        i = playerPlaying.Length;
                    }
                    //If the current player is not playing and/or they have folded
                    else
                    {
                        //If the player that's currently being checked is the last player
                        if (i == playerPlaying.Length - 1)
                        {
                            //Loop for all the players before the current turn
                            for (int j = 0; j < currentPlayerTurn; j++)
                            {
                                //if the current player is playing and has not folded
                                if (playerPlaying[j] == true && playersFolded[j] == false)
                                {
                                    //Set the current turn equal to this player
                                    currentPlayerTurn = j;

                                    //Stop the for loop
                                    j = playerPlaying.Length;
                                }
                            }
                        }
                    }
                }
            }

            //Return the next possible turn
            return currentPlayerTurn;
        }



        //Pre: The chip value and the amount of money the player has
        //Post: The amount of chips to draw will be returned
        //Desc: A subprogram to figure out how many of each chip to draw
        private int AmountChipsDraw(int chipValue, int playerMoney)
        {
            //Variable for the amount of the current chips to draw
            int amountToDraw = 0;

            //Loop for while the player's money is greater then the chip value
            while (playerMoney >= chipValue)
            {
                //Subtract the chip value from the player's money and add one to the amount of chips to draw
                playerMoney -= chipValue;
                amountToDraw++;
            }

            //Return the amount of chips to draw
            return amountToDraw;
        }


        //Pre: Each player's money 
        //Post: The highest amount of money a player has other then the user
        //Desc: A subprogram which returns the highest amount a player has other then the user
        private int HighestPlayerMoney()
        {
            //Variable for the highest amount of money a player has
            int highestMoney = 0;

            //Loop for every player
            for (int i = 0; i < playerMoney.Length; i++)
            {
                //If the current player is not the user and they have not folded
                if (i != userPlayer && playersFolded[i] == false)
                {
                    //If the player's money plus their bet is greater then the highest amount of money that's currently stored
                    if (playerMoney[i] + playerBet[i] > highestMoney)
                    {
                        //Set the highest amount of money to be equal to the current player's money plus their bet
                        highestMoney = playerMoney[i] + playerBet[i];
                    }
                }
            }

            //Return the highest player's money
            return highestMoney;
        }



        //Pre: All the data for the game which will be sent to all the other clients
        //Post: All the other clients will recieve all this information
        //Desc: A subprogram which sends to the server a message containing most of the information for the current game
        private void SendToServer()
        {
            //String variable of the current message to send
            string messageSend = "";

            //Add information of the game to the message
            messageSend += "1" + playersLeft;
            messageSend += newRound;
            messageSend += "@" + lastPlayerCheck;

            //Loop through every player and add to the message if each player is playing or not
            for (int i = 0; i < clientsReady.Length; i++)
            {
                messageSend += "@" + playerPlaying[i];
            }

            //Loop through every player and add to the message if each player is folded or not
            for (int i = 0; i < clientsReady.Length; i++)
            {
                messageSend += "@" + playersFolded[i];
            }

            //Loop through every player and add to the message if each player is bluffing or not
            for (int i = 0; i < clientsReady.Length; i++)
            {
                messageSend += "@" + playersBluff[i];
            }

            //Loop through every player and add to the message if each player was checked for bluffing
            for (int i = 0; i < clientsReady.Length; i++)
            {
                messageSend += "@" + playersBluffChecked[i];
            }

            //Loop through every player and add to the message if each player is all in or not
            for (int i = 0; i < clientsReady.Length; i++)
            {
                messageSend += "@" + playersAllIn[i];
            }

            //Loop through every player and add to the message each player's money
            for (int i = 0; i < clientsReady.Length; i++)
            {
                messageSend += "@" + playerMoney[i];
            }

            //Loop through every player and add to the message each player's bet
            for (int i = 0; i < clientsReady.Length; i++)
            {
                messageSend += "@" + playerBet[i];
            }

            //Add to the message information for the game
            messageSend += "@" + nextCardInDeck;
            messageSend += "@" + communityCardCount;
            messageSend += "@" + playersAllInCount;

            //Loop for every player
            for (int i = 0; i < playerPlaying.Length; i++)
            {
                //Loop for the first or second card for each player
                for (int j = 0; j < 2; j++)
                {
                    //Loop for the card number or suit of each card
                    for (int k = 0; k < 2; k++)
                    {
                        //Add to the message the current card
                        messageSend += "@" + playerCards[i, j, k];
                    }
                }
            }

            //Loop for every community card
            for (int i = 0; i < 5; i++)
            {
                //Loop for the card number or suit of each card
                for (int j = 0; j < 2; j++)
                {
                    //Add to the message the current community card
                    messageSend += "@" + communityCards[i, j];
                }
            }

            //Loop for every player
            for (int i = 0; i < playerPlaying.Length; i++)
            {
                //Loop for every hand quality value
                for (int j = 0; j < 3; j++)
                {
                    //Add to the message the current value
                    messageSend += "@" + handQuality[i, j];
                }
            }

            //Loop for all the hand kickers
            for (int i = 0; i < handKicker.Length; i++)
            {
                //Add the hand kickers to the message
                messageSend += "@" + handKicker[i];
            }

            //Add the amount of pots to the message
            messageSend += "@" + chipPots.Count;

            //loop for every pot
            for (int i = 0; i < chipPots.Count; i++)
            {
                //Add the current pot to the message
                messageSend += "@" + chipPots[i];

                //Loop for all the players
                for (int j = 0; j < playersCanWin[i].Length; j++)
                {
                    //Add what players have access to what pots
                    messageSend += "@" + playersCanWin[i][j];
                }
            }

            //Add to the message the amount of players that are tied
            messageSend += "@" + playersTied.Count;

            //Loop for every player that is tied
            for (int i = 0; i < playersTied.Count; i++)
            {
                //Add too the message each player that is tied
                messageSend += "@" + playersTied[i];
            }

            //Add to the message information about the current game
            messageSend += "@" + currentTurn;
            messageSend += "@" + bigBlindPlayer;
            messageSend += "@" + smallBlindPlayer;
            messageSend += "@" + blindAmount;
            messageSend += "@" + turnCount;
            messageSend += "@" + playersTurnCount;
            messageSend += "@" + currentBet;
            messageSend += "@" + currentPayOut;
            messageSend += "@" + playerWon;
            messageSend += "@" + playersTurnCount;
            messageSend += "@" + timeLeft;

            //Loop for every player
            for (int i = 0; i < playerNames.Length; i++)
            {
                //Add to the message each player's name
                messageSend += "@" + playerNames[i];
            }

            //Add to the message an @ so the client knows the message is done
            messageSend += "@";

            //Create the byte array of the message and send the array to the server
            byte[] sendBuffer = Encoding.ASCII.GetBytes(messageSend);
            clientSocket.Send(sendBuffer);

            //Set the message to send and a reply was not recieved yet
            messageSent = true;
            replyRecieved = false;
        }



        //Pre: None
        //Post: The last player is checked and given it's chips
        //Desc: A subprogram which checks who the last player is in the table and gives them all the pots
        private void CheckLastPlayer()
        {
            //variable for the last player
            int lastPlayer = 0;

            //Loop for all the players
            for (int i = 0; i < playersFolded.Length; i++)
            {
                //If the current player is playing
                if (playerPlaying[i] == true)
                {
                    //If the current player has not folded
                    if (playersFolded[i] == false)
                    {
                        //Set the last player to the current player
                        lastPlayer = i;
                    }
                }
            }

            //If the amount of pots is greater then 0
            if (chipPots.Count > 0)
            {
                //Loop for every player's bet
                for (int i = 0; i < playerBet.Length; i++)
                {
                    //Add to the last pot the current bet and set the bet to 0
                    chipPots[chipPots.Count - 1] += playerBet[i];
                    playerBet[i] = 0;
                }

                //Loop for all the pots
                for (int i = 0; i < chipPots.Count; i++)
                {
                    //Add the current pot to the amount of money of the last player and set the pot to 0
                    playerMoney[lastPlayer] += chipPots[i];
                    chipPots[i] = 0;
                }
            }

            //Set new round to tru indicating a new round needs to be started
            newRound = true;
        }



        //Pre: None
        //Post: The amount of players that called
        //Desc: A subprogram which returns the amount of player's that called
        private int CheckPlayersCalledCount()
        {
            //Variables for the amount of players that folded and called
            int playersFoldedCount = 0;
            int playersCalledCount = 0;

            //Gets the amount of players that folded
            playersFoldedCount = CheckPlayersFoldedCount();

            //Loop for all the players
            for (int i = 0; i < playersFolded.Length; i++)
            {
                //If the amount of turns that passed is greater then the players left subtracted by the amount of players that folded
                if (playersTurnCount >= playersLeft - playersFoldedCount)
                {
                    //Set the players called count to 0
                    playersCalledCount = 0;

                    //Loop for every player
                    for (int j = 0; j < playersFolded.Length; j++)
                    {
                        //If the current player did not fold and is still playing
                        if (playersFolded[j] == false && playerPlaying[j] == true)
                        {
                            //If the bet of the player is equal to the current bet or the current player's money is equal to 0
                            if (playerBet[j] == currentBet || playerMoney[j] == 0)
                            {
                                //Add one to the amount of players that called
                                playersCalledCount++;
                            }
                        }
                    }
                }
            }

            //Return the amount of players that called
            return playersCalledCount;
        }



        //Pre: None
        //Post: The X and Y locations of every chip is updated
        //Desc: A subprogram which updates all the locations of the chips if they are moving 
        private void CheckMoneyChipLocation()
        {
            //Variable for the amount of chips that joined together
            int amountJoined = 0;

            //Variables for the amount of players that are playing and amount of players that folded
            int playersPlayingCount = 0;
            int playersFoldedCount = 0;

            //If the chips don't need to join together
            if (playerChipsNeedJoin == false)
            {
                //If the current player is not all in
                if (playersAllIn[currentTurn] == false)
                {
                    //If the chips are moving for the current player
                    if (playerChipsMoving[currentTurn] == true)
                    {
                        //Switch for which player's turn it currently is
                        switch (currentTurn)
                        {
                            //If it's the first, second, third, or fourth turn
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                //If the X location plus the move rate is greater then the max location
                                if (playerChipsXLocations[currentTurn] + playerChipsXMoveRate > playerChipsMaxXLocation)
                                {
                                    //Add the move rate to the current X Location
                                    playerChipsXLocations[currentTurn] += playerChipsXMoveRate;
                                }
                                //If it's not greater then
                                else
                                {
                                    //Set the X location for the max X location
                                    playerChipsXLocations[currentTurn] = playerChipsMaxXLocation;
                                }
                                break;
                            //If it's the sixth, seventh, eigth, or ninth turn
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                                //If the X location plus the move rate is smaller then the max location
                                if (playerChipsXLocations[currentTurn] + playerChipsXMoveRate < playerChipsMaxXLocation)
                                {
                                    //Add the move rate to the current X Location
                                    playerChipsXLocations[currentTurn] += playerChipsXMoveRate;
                                }
                                //If it's not smaller then
                                else
                                {
                                    //Set the X location for the max X location
                                    playerChipsXLocations[currentTurn] = playerChipsMaxXLocation;
                                }
                                break;
                        }

                        //Switch for which player's turn it currently is
                        switch (currentTurn)
                        {
                            //If it's the first, second, eigth, or ninth turn
                            case 0:
                            case 1:
                            case 7:
                            case 8:
                                //If the Y location plus the move rate is smaller then the max location
                                if (playerChipsYLocations[currentTurn] + playerChipsYMoveRate < playerChipsMaxYLocation)
                                {
                                    //Add the move rate to the current Y Location
                                    playerChipsYLocations[currentTurn] += playerChipsYMoveRate;
                                }
                                //If it's not smaller then
                                else
                                {
                                    //Set the Y location for the max X location
                                    playerChipsYLocations[currentTurn] = playerChipsMaxYLocation;
                                }
                                break;
                            //If it's the third, fourth, fifth, sixth, or seventh turn
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                                //If the Y location plus the move rate is greater then the max location
                                if (playerChipsYLocations[currentTurn] + playerChipsYMoveRate > playerChipsMaxYLocation)
                                {
                                    //Add the move rate to the current Y Location
                                    playerChipsYLocations[currentTurn] += playerChipsYMoveRate;
                                }
                                //If it's not greater then
                                else
                                {
                                    //Set the Y location as the max Y location
                                    playerChipsYLocations[currentTurn] = playerChipsMaxYLocation;
                                }
                                break;
                        }

                        //If the current X location is equal to  the max X location and the current Y location is equal to the max Y location
                        if (playerChipsXLocations[currentTurn] == playerChipsMaxXLocation && playerChipsYLocations[currentTurn] == playerChipsMaxYLocation)
                        {
                            //Set the chips to not moving and update the current turn to the next turn
                            playerChipsMoving[currentTurn] = false;
                            currentTurn = NextPossibleTurn(currentTurn);
                        }
                    }
                }
                //If the player is all in
                else
                {
                    //Set the chips to not moving and update the current turn to the next turn
                    playerChipsMoving[currentTurn] = false;
                    currentTurn = NextPossibleTurn(currentTurn);
                }
            }
            //If the chips do need to join
            else
            {
                //If the chips do need to join and they have not joined yet
                if (playerChipsNeedJoin == true && playerChipsJoined == false)
                {
                    //Loop for every player
                    for (int i = 0; i < playerPlaying.Length; i++)
                    {
                        //If the current player is playing
                        if (playerPlaying[i] == true)
                        {
                            //If the x location of the current player is smaller then the max x location
                            if (playerChipsXLocations[i] < playerChipsMaxXLocation)
                            {
                                //If the X location plus the move rate is smaller then the max location
                                if (playerChipsXLocations[i] + playerChipsXMoveRate < playerChipsMaxXLocation)
                                {
                                    //Add the move rate to the current X Location
                                    playerChipsXLocations[i] += playerChipsXMoveRate;
                                }
                                //If it's not smaller then
                                else
                                {
                                    //Set the X location as the max X location
                                    playerChipsXLocations[i] = playerChipsMaxXLocation;
                                }
                            }
                            //If the x location of the current player is greater then the max x location
                            else
                            {
                                //If the X location minus the move rate is greater then the max location
                                if (playerChipsXLocations[i] - playerChipsXMoveRate > playerChipsMaxXLocation)
                                {
                                    //Subtract the move rate to the current X Location
                                    playerChipsXLocations[i] -= playerChipsXMoveRate;
                                }
                                //If it's not greater then
                                else
                                {
                                    //Set the X location as the max X location
                                    playerChipsXLocations[i] = playerChipsMaxXLocation;
                                }
                            }

                            //If the y location of the current player is smaller then the max x location
                            if (playerChipsYLocations[i] < playerChipsMaxYLocation)
                            {
                                //If the Y location plus the move rate is smaller then the max location
                                if (playerChipsYLocations[i] + playerChipsYMoveRate < playerChipsMaxYLocation)
                                {
                                    //Add the move rate to the current Y Location
                                    playerChipsYLocations[i] += playerChipsYMoveRate;
                                }
                                //If it's not smaller then
                                else
                                {
                                    //Set the y location as the max y location
                                    playerChipsYLocations[i] = playerChipsMaxYLocation;
                                }
                            }
                            //If the y location of the current player is greater then the max x location
                            else
                            {
                                //If the Y location minus the move rate is greater then the max location
                                if (playerChipsYLocations[i] - playerChipsYMoveRate > playerChipsMaxYLocation)
                                {
                                    //Subtract the move rate to the current Y Location
                                    playerChipsYLocations[i] -= playerChipsYMoveRate;
                                }
                                //If it's not greater then
                                else
                                {
                                    //Set the y location as the max y location
                                    playerChipsYLocations[i] = playerChipsMaxYLocation;
                                }
                            }
                        }
                    }

                    //Set the amount of players joined to 0 and the amount of players playing to 0
                    amountJoined = 0;
                    playersPlayingCount = 0;

                    //Loop for every player
                    for (int i = 0; i < playerPlaying.Length; i++)
                    {
                        //If the current player is playing
                        if (playerPlaying[i] == true)
                        {
                            //Add one to the amount of players that are playing
                            playersPlayingCount++;
                        }
                    }

                    //Calculate the amount of players that folded
                    playersFoldedCount = CheckPlayersFoldedCount();

                    //Loop for every player
                    for (int i = 0; i < playerPlaying.Length; i++)
                    {
                        //If the current player is playing
                        if (playerPlaying[i] == true)
                        {
                            //If the X location of the current chips is equal to the max X location and the Y location of the current chips is equal to the max Y 
                            //location and the player's bet is more then 0
                            if (playerChipsXLocations[i] == playerChipsMaxXLocation && playerChipsYLocations[i] == playerChipsMaxYLocation && playerBet[i] > 0)
                            {
                                //Add one to the amount that joined
                                amountJoined++;
                            }
                        }
                    }

                    //If the amount that joined is equal to the amount of players that are playing and the community card count is equal to 0 or the amount that joined
                    //is equal to the amount of players playing minus the amount of players that folded or if the current bet is equal to 0
                    if (amountJoined == playersPlayingCount && communityCardCount == 0 || amountJoined == playersPlayingCount - playersFoldedCount || currentBet == 0)
                    {
                        //Set the chips to joined
                        playerChipsJoined = true;
                    }
                }
            }
        }



        //Pre: None
        //Post: The pause button is checked for if the mouse is hovering over it or if it was clicked
        //Desc: A subprogram which checks the pause game button and if it was clicked or is being hovered over
        private void CheckPauseButton()
        {
            //If the mouse X is in between the location of the button
            if (mouseX >= pauseBtnXLocation && mouseX <= pauseBtnXLocation + pauseBtnXSize)
            {
                //If the mouse Y is in between the location of the button
                if (mouseY >= pauseBtnYLocation && mouseY <= pauseBtnYLocation + pauseBtnYSize)
                {
                    //Set the button to be hovering over
                    isPauseBtnHovering = true;

                    //If the left mouse button was pressed and is not being held
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && leftMouseHeld == false)
                    {
                        //Set it to being held
                        leftMouseHeld = true;

                        //Set the game state to pause game and stop the timer
                        gameState = PAUSE_GAME;
                        playerTime.Stop();
                    }
                }
                //If the Y mouse the mouse is anywhere else
                else
                {
                    //Set the button to be not hovering over
                    isPauseBtnHovering = false;
                }
            }
            //If the X mouse the mouse is anywhere else
            else
            {
                //Set the button to be not hovering over
                isPauseBtnHovering = false;
            }
        }



        //Pre: The max all in amount
        //Post: The game checks if the user picked any of their betting options
        //Desc: A subprogram which checks if the user picked any of their betting options
        private void PlayerBettingOptions(int maxAllInAmount)
        {
            //Variables for the current bet button that's being checked and the amount that is called
            int betBtnNum = 0;
            int callAmount = 0;

            //If the current player is not all in and it's the user's player
            if (playersAllIn[currentTurn] == false && currentTurn == userPlayer)
            {
                //Loop for every button's Y
                for (int i = 0; i < betBtnYLocation.Length; i++)
                {
                    //Loop for every button's X
                    for (int j = 0; j < betBtnXLocation.Length; j++)
                    {
                        //If the current button that's being checked is not equal to 3
                        if (betBtnNum != 3)
                        {
                            //If the user is able to pick an option for betting
                            if (playersFolded[userPlayer] == false && currentTurn == userPlayer && playerChipsNeedJoin == false && playerChipsJoined == true && playerChipsMoving[userPlayer] == false)
                            {
                                //If the current bet is smaller then the amount the user has plus their bet
                                if (currentBet < playerMoney[userPlayer] + playerBet[userPlayer])
                                {
                                    //If the plus button is being checked
                                    if (i == betBtnYLocation.Length - 1 && j == 0)
                                    {
                                        //While loop that stays running as long is the button that's being checked is less then 6
                                        while (betBtnNum < 6)
                                        {
                                            //If the mouse Y is in between the location of the button
                                            if (mouseY >= betBtnYLocation[i] && mouseY <= betBtnYLocation[i] + betBtnYSize)
                                            {
                                                //If the mouse X is in between the location of the button
                                                if (mouseX >= betBtnXLocation[0] + 10 && mouseX <= betBtnXLocation[0] + 10 + raiseBtnSize && betBtnNum == 4)
                                                {
                                                    //Set the current button to hovering
                                                    isBetBtnHovering[betBtnNum] = true;

                                                    //If the left mouse button was pressed and is not being held
                                                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && leftMouseHeld == false)
                                                    {
                                                        //Set the left mouse button to held
                                                        leftMouseHeld = true;

                                                        //If the raise amount plus the blind amount is smaller then or equal to the amount of money the user has and the max all in amount
                                                        if (userRaiseAmount + blindAmount <= playerMoney[userPlayer] && userRaiseAmount + blindAmount <= maxAllInAmount)
                                                        {
                                                            //Add the blind amount to the raise amount
                                                            userRaiseAmount += blindAmount;
                                                        }
                                                    }
                                                }
                                                //If the mouse X is in between the location of the button
                                                else if (mouseX >= minusBtnXLocation && mouseX <= minusBtnXLocation + raiseBtnSize && betBtnNum == 5)
                                                {
                                                    //Set the current button to hovering
                                                    isBetBtnHovering[betBtnNum] = true;

                                                    //If the left mouse button was pressed and is not being held
                                                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && leftMouseHeld == false)
                                                    {
                                                        //Set the left mouse button to held
                                                        leftMouseHeld = true;

                                                        //If the raise amount minus the blind amount is greater then or equal to the current bet
                                                        if (userRaiseAmount - blindAmount >= currentBet)
                                                        {
                                                            //Subtract the blind amount from the raise amount
                                                            userRaiseAmount -= blindAmount;
                                                        }
                                                    }
                                                }
                                                //If the mouse X is anywhere else
                                                else
                                                {
                                                    //Set the current button to not hovering
                                                    isBetBtnHovering[betBtnNum] = false;
                                                }
                                            }
                                            //If the mouse Y is anywhere else
                                            else
                                            {
                                                //Set the current button to not hovering
                                                isBetBtnHovering[betBtnNum] = false;
                                            }

                                            //Set the next button to be checked
                                            betBtnNum++;
                                        }

                                        //Set the previous button to be checked
                                        betBtnNum--;
                                    }
                                    //If the plus button is not being checked
                                    else
                                    {
                                        //If the mouse Y is in between the location of the button
                                        if (mouseY >= betBtnYLocation[i] && mouseY <= betBtnYLocation[i] + betBtnYSize)
                                        {
                                            //If the mouse X is in between the location of the button
                                            if (mouseX >= betBtnXLocation[j] && mouseX <= betBtnXLocation[j] + betBtnXSize)
                                            {
                                                //Set the current button to hovering
                                                isBetBtnHovering[betBtnNum] = true;

                                                //If the left mouse button was pressed and is not being held
                                                if (Mouse.GetState().LeftButton == ButtonState.Pressed && leftMouseHeld == false)
                                                {
                                                    //Set the left mouse button to held
                                                    leftMouseHeld = true;

                                                    //Switch statement for the current button being checked
                                                    switch (betBtnNum)
                                                    {
                                                        //If it's the first button
                                                        case 0:
                                                            //If the current bet is greater then the player's bet
                                                            if (currentBet > playerBet[userPlayer])
                                                            {
                                                                //Set the call amount to the difference between the two pets and add that amount to the player's bet and subtract it from the player's money
                                                                callAmount = currentBet - playerBet[userPlayer];
                                                                playerBet[userPlayer] += callAmount;
                                                                playerMoney[userPlayer] -= callAmount;

                                                                //Stop and reset the timer
                                                                playerTime.Stop();
                                                                timeLeft = ROUND_START_TIME;
                                                                oldTimeLeft = ROUND_START_TIME;

                                                                //Add one to the amount of turns that happened this round
                                                                playersTurnCount++;

                                                                //If the game state is not equal to network game
                                                                if (gameState != NETWORK_GAME)
                                                                {
                                                                    //Set the chips for the current player to moving and set their values
                                                                    playerChipsMoving[currentTurn] = true;
                                                                    SetChipMovingValues(currentTurn);
                                                                }
                                                                //If the game state is on network game
                                                                else
                                                                {
                                                                    //Update the current turn
                                                                    currentTurn = NextPossibleTurn(currentTurn);
                                                                }
                                                            }
                                                            //If the current bet is smaller then the player's bet
                                                            else
                                                            {
                                                                //Stop and reset the timer
                                                                playerTime.Stop();
                                                                timeLeft = ROUND_START_TIME;
                                                                oldTimeLeft = ROUND_START_TIME;

                                                                //Add one to the amount of turns that happened this round
                                                                playersTurnCount++;

                                                                //If the game state is not equal to network game
                                                                if (gameState != NETWORK_GAME)
                                                                {
                                                                    //Set the chips for the current player to moving and set their values
                                                                    playerChipsMoving[currentTurn] = true;
                                                                    SetChipMovingValues(currentTurn);
                                                                }
                                                                //If the game state is on network game
                                                                else
                                                                {
                                                                    //Update the current turn
                                                                    currentTurn = NextPossibleTurn(currentTurn);
                                                                }
                                                            }

                                                            //If the game state is equal to network game
                                                            if (gameState == NETWORK_GAME)
                                                            {
                                                                //Send to the server is needed
                                                                sendToServerNeeded = true;
                                                            }
                                                            break;
                                                        //If it's the second button
                                                        case 1:
                                                            //Set the user to folded
                                                            playersFolded[userPlayer] = true;

                                                            //Stop and reset the timer
                                                            playerTime.Stop();
                                                            timeLeft = ROUND_START_TIME;
                                                            oldTimeLeft = ROUND_START_TIME;

                                                            //Add one to the amount of turns that happened this round
                                                            playersTurnCount++;

                                                            //If the game state is not equal to network game
                                                            if (gameState != NETWORK_GAME)
                                                            {
                                                                //Set the chips for the current player to moving and set their values
                                                                playerChipsMoving[currentTurn] = true;
                                                                SetChipMovingValues(currentTurn);
                                                            }
                                                            //If the game state is on network game
                                                            else
                                                            {
                                                                //Update the current turn and set the send to server to be needed
                                                                currentTurn = NextPossibleTurn(currentTurn);
                                                            }

                                                            //If the game state is equal to network game
                                                            if (gameState == NETWORK_GAME)
                                                            {
                                                                //Send to the server is needed
                                                                sendToServerNeeded = true;
                                                            }
                                                            break;
                                                        //If it's the third button
                                                        case 2:
                                                            //Update the bet and the amount of money the user has with the raise amount
                                                            playerMoney[userPlayer] -= userRaiseAmount - playerBet[userPlayer];
                                                            playerBet[userPlayer] = userRaiseAmount;

                                                            //If the raise amount is greater then the current bet
                                                            if (userRaiseAmount > currentBet)
                                                            {
                                                                //Set the current bet equal to the raise amount
                                                                currentBet = userRaiseAmount;
                                                            }

                                                            //If the raise amount is equal to the max all in amount or the user amount is equal to the player's money plus their bet
                                                            if (userRaiseAmount == maxAllInAmount || userRaiseAmount == playerMoney[userPlayer] + playerBet[userPlayer])
                                                            {
                                                                //Set the current player to all in
                                                                playersAllIn[userPlayer] = true;
                                                            }

                                                            //Set the raise amount to 0
                                                            userRaiseAmount = 0;

                                                            //Reset the time
                                                            playerTime.Stop();
                                                            timeLeft = ROUND_START_TIME;
                                                            oldTimeLeft = ROUND_START_TIME;

                                                            //Add one to the amount of turns that happened this round
                                                            playersTurnCount++;

                                                            //If the game state is not equal to network game
                                                            if (gameState != NETWORK_GAME)
                                                            {
                                                                //Set the chips for the current player to moving and set their values
                                                                playerChipsMoving[currentTurn] = true;
                                                                SetChipMovingValues(currentTurn);
                                                            }
                                                            //If the game state is on network game
                                                            else
                                                            {
                                                                //Update the current turn
                                                                currentTurn = NextPossibleTurn(currentTurn);
                                                            }

                                                            //If the game state is equal to network game
                                                            if (gameState == NETWORK_GAME)
                                                            {
                                                                //Send to the server is needed
                                                                sendToServerNeeded = true;
                                                            }
                                                            break;
                                                        //If it's the seventh button
                                                        case 6:
                                                            //If the users money is greater then or equal to the max all in amount
                                                            if (playerMoney[userPlayer] >= maxAllInAmount)
                                                            {
                                                                //The rase amount is equal to the max all in amount
                                                                userRaiseAmount = maxAllInAmount;
                                                            }
                                                            //If the users money is less then the max all in amount
                                                            else
                                                            {
                                                                //The rase amount is equal to all the money the user has
                                                                userRaiseAmount = playerMoney[userPlayer] + playerBet[userPlayer];
                                                            }
                                                            break;
                                                    }
                                                }
                                            }
                                            //If the mouse X is anywhere else
                                            else
                                            {
                                                //Set the current button to not hovering
                                                isBetBtnHovering[betBtnNum] = false;
                                            }
                                        }
                                        //If the mouse Y is anywhere else
                                        else
                                        {
                                            //Set the current button to not hovering
                                            isBetBtnHovering[betBtnNum] = false;
                                        }
                                    }

                                    //Set the next button to be checked
                                    betBtnNum++;
                                }
                                //If the current bet is greater then the amount the user has plus their bet
                                else
                                {
                                    //If the first Y location is being checked
                                    if (i == 0)
                                    {
                                        //If the mouse Y is in between the location of the button
                                        if (mouseY >= betBtnYLocation[i] && mouseY <= betBtnYLocation[i] + betBtnYSize)
                                        {
                                            //If the mouse X is in between the location of the button
                                            if (mouseX >= betBtnXLocation[j] && mouseX <= betBtnXLocation[j] + betBtnXSize)
                                            {
                                                //Set the current button to hovering
                                                isBetBtnHovering[betBtnNum] = true;

                                                //If the left mouse button was pressed and is not being held
                                                if (Mouse.GetState().LeftButton == ButtonState.Pressed && leftMouseHeld == false)
                                                {
                                                    //Set the left mouse button to held
                                                    leftMouseHeld = true;

                                                    //Switch statement for the button that is being checked
                                                    switch (j)
                                                    {
                                                        //If it's the first button
                                                        case 0:
                                                            //Update their bet with their money and set their money to -
                                                            playerBet[userPlayer] += playerMoney[userPlayer];
                                                            playerMoney[userPlayer] = 0;

                                                            //Add one to the amount of turns that happened this round
                                                            playersTurnCount++;

                                                            //If the game state is not equal to network game
                                                            if (gameState != NETWORK_GAME)
                                                            {
                                                                //Set the chips for the current player to moving and set their values
                                                                playerChipsMoving[currentTurn] = true;
                                                                SetChipMovingValues(currentTurn);
                                                            }
                                                            //If the game state is on network game
                                                            else
                                                            {
                                                                //Update the current turn
                                                                currentTurn = NextPossibleTurn(currentTurn);
                                                            }

                                                            //If the game state is equal to network game
                                                            if (gameState == NETWORK_GAME)
                                                            {
                                                                //Send to the server is needed
                                                                sendToServerNeeded = true;
                                                            }
                                                            break;
                                                        //If it's the second button
                                                        case 1:
                                                            //Set the user to folded
                                                            playersFolded[userPlayer] = true;

                                                            //Add one to the amount of turns that happened this round
                                                            playersTurnCount++;

                                                            //If the game state is not equal to network game
                                                            if (gameState != NETWORK_GAME)
                                                            {
                                                                //Set the chips for the current player to moving and set their values
                                                                playerChipsMoving[currentTurn] = true;
                                                                SetChipMovingValues(currentTurn);
                                                            }
                                                            //If the game state is on network game
                                                            else
                                                            {
                                                                //Update the current turn
                                                                currentTurn = NextPossibleTurn(currentTurn);
                                                            }

                                                            //If the game state is equal to network game
                                                            if (gameState == NETWORK_GAME)
                                                            {
                                                                //Send to the server is needed
                                                                sendToServerNeeded = true;
                                                            }
                                                            break;
                                                    }
                                                }
                                            }
                                            //If the mouse X is anywhere else
                                            else
                                            {
                                                //Set the current button to not hovering
                                                isBetBtnHovering[betBtnNum] = false;
                                            }
                                        }
                                        //If the mouse Y is anywhere else
                                        else
                                        {
                                            //Set the current button to not hovering
                                            isBetBtnHovering[betBtnNum] = false;
                                        }

                                        //Set the next button to be checked
                                        betBtnNum++;
                                    }
                                }
                            }
                            //If the user is not able to pick an option for betting
                            else
                            {
                                //Loop for every button
                                for (int k = 0; k < isBetBtnHovering.Length; k++)
                                {
                                    //Set the current button to not hovering
                                    isBetBtnHovering[i] = false;
                                }
                            }
                        }
                        //If the current button that is being checked is equal to 3
                        else
                        {
                            //Set the next button to be checked
                            betBtnNum++;
                        }
                    }
                }
            }
        }



        //Pre: The current player
        //Post: The values for how fast and how far the chips are moving are set
        //Desc: A subprogram which updates the speed and how far the chips are going to be moving
        private void SetChipMovingValues(int currentPlayer)
        {
            //If the chips do not need to join
            if (playerChipsNeedJoin == false)
            {
                //Generate a random number
                int randNum = randInt.Next(0, 5);

                //Switch statement for the current player
                switch (currentPlayer)
                {
                    //If it's the first or second player
                    case 0:
                    case 1:
                        //Set the speed depending on the randomly generated number
                        switch (randNum)
                        {
                            case 0:
                                playerChipsXMoveRate = -1;
                                playerChipsYMoveRate = 1;
                                break;
                            case 1:
                                playerChipsXMoveRate = -2;
                                playerChipsYMoveRate = 2;
                                break;
                            case 2:
                                playerChipsXMoveRate = -3;
                                playerChipsYMoveRate = 3;
                                break;
                            case 3:
                                playerChipsXMoveRate = -4;
                                playerChipsYMoveRate = 4;
                                break;
                            case 4:
                                playerChipsXMoveRate = -5;
                                playerChipsYMoveRate = 5;
                                break;
                        }
                        break;
                    //If it's the third or fourth player
                    case 2:
                    case 3:
                        //Set the speed depending on the randomly generated number
                        switch (randNum)
                        {
                            case 0:
                                playerChipsXMoveRate = -1;
                                playerChipsYMoveRate = -1;
                                break;
                            case 1:
                                playerChipsXMoveRate = -2;
                                playerChipsYMoveRate = -2;
                                break;
                            case 2:
                                playerChipsXMoveRate = -3;
                                playerChipsYMoveRate = -3;
                                break;
                            case 3:
                                playerChipsXMoveRate = -4;
                                playerChipsYMoveRate = -4;
                                break;
                            case 4:
                                playerChipsXMoveRate = -5;
                                playerChipsYMoveRate = -5;
                                break;
                        }
                        break;
                    //If it's the fifth player
                    case 4:
                        //Set the X move rate to 0 because it only moves with Y 
                        playerChipsXMoveRate = 0;

                        //Set the speed depending on the randomly generated number
                        switch (randNum)
                        {
                            case 0:
                                playerChipsYMoveRate = -1;
                                break;
                            case 1:
                                playerChipsYMoveRate = -2;
                                break;
                            case 2:
                                playerChipsYMoveRate = -3;
                                break;
                            case 3:
                                playerChipsYMoveRate = -4;
                                break;
                            case 4:
                                playerChipsYMoveRate = -5;
                                break;
                        }
                        break;
                    //If it's the sixth or seventh player
                    case 5:
                    case 6:
                        //Set the speed depending on the randomly generated number
                        switch (randNum)
                        {
                            case 0:
                                playerChipsXMoveRate = 1;
                                playerChipsYMoveRate = -1;
                                break;
                            case 1:
                                playerChipsXMoveRate = 2;
                                playerChipsYMoveRate = -2;
                                break;
                            case 2:
                                playerChipsXMoveRate = 3;
                                playerChipsYMoveRate = -3;
                                break;
                            case 3:
                                playerChipsXMoveRate = 4;
                                playerChipsYMoveRate = -4;
                                break;
                            case 4:
                                playerChipsXMoveRate = 5;
                                playerChipsYMoveRate = -5;
                                break;
                        }
                        break;
                    //If it's the eigth or ninth player
                    case 7:
                    case 8:
                        //Set the speed depending on the randomly generated number
                        switch (randNum)
                        {
                            case 0:
                                playerChipsXMoveRate = 1;
                                playerChipsYMoveRate = 1;
                                break;
                            case 1:
                                playerChipsXMoveRate = 2;
                                playerChipsYMoveRate = 2;
                                break;
                            case 2:
                                playerChipsXMoveRate = 3;
                                playerChipsYMoveRate = 3;
                                break;
                            case 3:
                                playerChipsXMoveRate = 4;
                                playerChipsYMoveRate = 4;
                                break;
                            case 4:
                                playerChipsXMoveRate = 5;
                                playerChipsYMoveRate = 5;
                                break;
                        }
                        break;
                }


                //Switch statement for the current player
                switch (currentPlayer)
                {
                    //If it's the first player
                    case 0:
                        //Update from where the X and Y locations start
                        playerChipsXLocations[currentPlayer] = CHIP_START_1_X;
                        playerChipsYLocations[currentPlayer] = CHIP_START_1_Y;

                        //Set the max X and Y locations depending on the Y move rate
                        switch (playerChipsYMoveRate)
                        {
                            case 1:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 10;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 30;
                                break;
                            case 2:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 20;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 60;
                                break;
                            case 3:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 30;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 90;
                                break;
                            case 4:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 40;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 120;
                                break;
                            case 5:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 50;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 150;
                                break;
                        }
                        break;
                    //If it's the second player
                    case 1:
                        //Update from where the X and Y locations start
                        playerChipsXLocations[currentPlayer] = CHIP_START_2_X;
                        playerChipsYLocations[currentPlayer] = CHIP_START_2_Y;

                        //Set the max X and Y locations depending on the Y move rate
                        switch (playerChipsYMoveRate)
                        {
                            case 1:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 30;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 10;
                                break;
                            case 2:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 60;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 20;
                                break;
                            case 3:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 90;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 30;
                                break;
                            case 4:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 120;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 40;
                                break;
                            case 5:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 150;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 50;
                                break;
                        }
                        break;
                    //If it's the third player
                    case 2:
                        //Update from where the X and Y locations start
                        playerChipsXLocations[currentPlayer] = CHIP_START_3_X;
                        playerChipsYLocations[currentPlayer] = CHIP_START_3_Y;

                        //Set the max X and Y locations depending on the Y move rate
                        switch (playerChipsYMoveRate)
                        {
                            case -1:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 30;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 10;
                                break;
                            case -2:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 60;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 20;
                                break;
                            case -3:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 90;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 30;
                                break;
                            case -4:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 120;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 40;
                                break;
                            case -5:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 150;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 50;
                                break;
                        }
                        break;
                    //If it's the fourth player
                    case 3:
                        //Update from where the X and Y locations start
                        playerChipsXLocations[currentPlayer] = CHIP_START_4_X;
                        playerChipsYLocations[currentPlayer] = CHIP_START_4_Y;

                        //Set the max X and Y locations depending on the Y move rate
                        switch (playerChipsYMoveRate)
                        {
                            case -1:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 10;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 30;
                                break;
                            case -2:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 20;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 60;
                                break;
                            case -3:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 30;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 90;
                                break;
                            case -4:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 40;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 120;
                                break;
                            case -5:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] - 50;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 150;
                                break;
                        }
                        break;
                    //If it's the fifth player
                    case 4:
                        //Update from where the X and Y locations start
                        playerChipsXLocations[currentPlayer] = CHIP_START_5_X;
                        playerChipsYLocations[currentPlayer] = CHIP_START_5_Y;

                        //Set the max X and Y locations depending on the Y move rate
                        switch (playerChipsYMoveRate)
                        {
                            case -1:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer];
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 30;
                                break;
                            case -2:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer];
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 60;
                                break;
                            case -3:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer];
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 90;
                                break;
                            case -4:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer];
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 120;
                                break;
                            case -5:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer];
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 150;
                                break;
                        }
                        break;
                    //If it's the sixth player
                    case 5:
                        //Update from where the X and Y locations start
                        playerChipsXLocations[currentPlayer] = CHIP_START_6_X;
                        playerChipsYLocations[currentPlayer] = CHIP_START_6_Y;

                        //Set the max X and Y locations depending on the X move rate
                        switch (playerChipsXMoveRate)
                        {
                            case 1:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 10;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 30;
                                break;
                            case 2:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 20;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 60;
                                break;
                            case 3:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 30;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 90;
                                break;
                            case 4:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 40;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 120;
                                break;
                            case 5:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 50;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 150;
                                break;
                        }
                        break;
                    //If it's the seventh player
                    case 6:
                        //Update from where the X and Y locations start
                        playerChipsXLocations[currentPlayer] = CHIP_START_7_X;
                        playerChipsYLocations[currentPlayer] = CHIP_START_7_Y;

                        //Set the max X and Y locations depending on the X move rate
                        switch (playerChipsXMoveRate)
                        {
                            case 1:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 30;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 10;
                                break;
                            case 2:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 60;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 20;
                                break;
                            case 3:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 90;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 30;
                                break;
                            case 4:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 120;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 40;
                                break;
                            case 5:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 150;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] - 50;
                                break;
                        }
                        break;
                    //If it's the eigth player
                    case 7:
                        //Update from where the X and Y locations start
                        playerChipsXLocations[currentPlayer] = CHIP_START_8_X;
                        playerChipsYLocations[currentPlayer] = CHIP_START_8_Y;

                        //Set the max X and Y locations depending on the Y move rate
                        switch (playerChipsYMoveRate)
                        {
                            case 1:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 30;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 10;
                                break;
                            case 2:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 60;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 20;
                                break;
                            case 3:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 90;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 30;
                                break;
                            case 4:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 120;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 40;
                                break;
                            case 5:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 150;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 50;
                                break;
                        }
                        break;
                    //If it's the ninth player
                    case 8:
                        //Update from where the X and Y locations start
                        playerChipsXLocations[currentPlayer] = CHIP_START_9_X;
                        playerChipsYLocations[currentPlayer] = CHIP_START_9_Y;

                        //Set the max X and Y locations depending on the Y move rate
                        switch (playerChipsYMoveRate)
                        {
                            case 1:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 10;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 30;
                                break;
                            case 2:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 20;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 60;
                                break;
                            case 3:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 30;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 90;
                                break;
                            case 4:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 40;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 120;
                                break;
                            case 5:
                                playerChipsMaxXLocation = playerChipsXLocations[currentPlayer] + 50;
                                playerChipsMaxYLocation = playerChipsYLocations[currentPlayer] + 150;
                                break;
                        }
                        break;
                }
            }
            //If the chips do need to join
            else
            {

                //Update the move rate
                playerChipsXMoveRate = 3;
                playerChipsYMoveRate = 3;

                //Set the max X and Y locations
                playerChipsMaxXLocation = CHIP_X_MAIN_STACK;
                playerChipsMaxYLocation = CHIP_Y_MAIN_STACK;
            }
        }



        //Pre: None
        //Post: The amount of players that folded
        //Desc: A subprogram that returns the amount of players that folded
        private int CheckPlayersFoldedCount()
        {
            //Variable for the amount of players that folded
            int playersFoldedCount = 0;

            //Loop for every player
            for (int i = 0; i < playersFolded.Length; i++)
            {
                //If the current player folded
                if (playersFolded[i] == true)
                {
                    //Add one to the amount of players that folded
                    playersFoldedCount++;
                }
            }

            //Return the amount of players that folded
            return playersFoldedCount;
        }



        //Pre: None
        //Post: Amount of players that are all in
        //Desc: A subprogram that returns the amount of players that are all in
        private int AmountPlayersAllIn()
        {
            //Variable for the amount of players that are all int
            int playersAllInCount = 0;

            //Loop for every player
            for (int i = 0; i < playersAllIn.Length; i++)
            {
                //If the current player is all in
                if (playersAllIn[i] == true)
                {
                    //Add one to the amount fo players all in
                    playersAllInCount++;
                }
            }

            //Return the amount of players all in
            return playersAllInCount;
        }



        //Pre: None
        //Post: Which players are currently all in
        //Desc: A subprogram that returns an array of every player and if each player is all in or not
        private bool[] PlayersAllIn()
        {
            //Variable for the max all in amount
            int maxAllInAmount = 0;

            //Loop for every player
            for (int i = 0; i < playerMoney.Length; i++)
            {
                //If the current player did not fold and the current player's turn is not now
                if (i != currentTurn && playersFolded[i] == false)
                {
                    //If the money of the current player plus their bet is greater then the max all in amount
                    if (playerMoney[i] + playerBet[i] > maxAllInAmount)
                    {
                        //Set the max all in amount to be equal to the players money plus their bet
                        maxAllInAmount = playerMoney[i] + playerBet[i];
                    }
                }
            }

            //Loop for every player
            for (int i = 0; i < playersAllIn.Length; i++)
            {
                //if the current player is not all in
                if (playersAllIn[i] == false)
                {
                    //If the current player is playing an is not folded
                    if (playerPlaying[i] == true && playersFolded[i] == false)
                    {
                        //If the current players money is equal to 0 or their bet is equal to the max all in amount
                        if (playerMoney[i] == 0 || playerBet[i] == maxAllInAmount)
                        {
                            //Set the current player to all in
                            playersAllIn[i] = true;
                        }
                    }
                }
            }

            //Return the array of booleans that says which players are all in
            return playersAllIn;
        }



        //Pre: the amount of players that folded
        //Post: The times are checked for every player
        //Desc: A subprogram which checks the time for every player so they turn doesn't take for ever
        private void CheckPlayerTime(int playersFoldedCount)
        {
            //If the game state is not currently on network game
            if (gameState != NETWORK_GAME)
            {
                //If the current player is not folded and is playing
                if (playersFolded[currentTurn] == false && playerPlaying[currentTurn] == true)
                {
                    //if the time is running
                    if (playerTime.IsRunning == false)
                    {
                        //If the player is all in
                        if (playersAllIn[currentTurn] == true)
                        {
                            //Set the AI time to 0
                            timeAI = 0;
                        }
                        //If the player is not all in
                        else
                        {
                            //Restart the time
                            playerTime.Restart();

                            //Randomly generate the AI time
                            timeAI = randInt.Next(2, 6);
                        }
                    }

                    //If the current turn is not the user's
                    if (currentTurn != userPlayer)
                    {
                        //If the Ai time is equal to 0
                        if (timeAI == 0)
                        {
                            //Add one to the amount of turns passed this round
                            playersTurnCount++;

                            //Update the current turn
                            currentTurn = NextPossibleTurn(currentTurn);
                        }
                        //If the Ai time is equal to the amount of time that's passed
                        else if (timeAI == playerTime.Elapsed.Seconds)
                        {
                            //Add one to the amount of turns passed this round
                            playersTurnCount++;

                            //Call the AI Move subprogram for the AI to make their move
                            ArtificialIntelligenceMove();
                        }
                    }
                    //if the current turn is the user's
                    else
                    {
                        //If the AI time is equal to 0
                        if (timeAI == 0)
                        {
                            //Add one to the amount of turns passed this round
                            playersTurnCount++;

                            //Update the current turn
                            currentTurn = NextPossibleTurn(currentTurn);
                        }
                        //If the time left is equal to 0
                        else if (timeLeft == 0)
                        {
                            //If the current bet is less then or equal to the player's bet
                            if (currentBet <= playerBet[currentTurn])
                            {
                                //Add one to the amount of turns passed this round
                                playersTurnCount++;

                                //Stop and reset the time
                                playerTime.Stop();
                                timeLeft = 20;

                                //Set the chips to be moving for the current player and update their values
                                playerChipsMoving[currentTurn] = true;
                                SetChipMovingValues(currentTurn);
                            }
                            //If the current bet is anything else
                            else
                            {
                                //Stop and reset the time
                                playerTime.Stop();
                                timeLeft = 20;

                                //Set the current player to folded
                                playersFolded[userPlayer] = true;

                                //Set the chips to be moving for the current player and update their values
                                playerChipsMoving[currentTurn] = true;
                                SetChipMovingValues(currentTurn);
                            }
                        }
                    }
                }
                //If the current player is folded and/or is not playing
                else
                {
                    //Update the current turn
                    currentTurn = NextPossibleTurn(currentTurn);
                }
            }
            //If the game state is currently on network game
            else
            {
                //If the current player is not folded and is playing
                if (playersFolded[currentTurn] == false && playerPlaying[currentTurn] == true)
                {
                    //if the time is running
                    if (playerTime.IsRunning == false)
                    {
                        //If the current player is all in
                        if (playersAllIn[currentTurn] == true)
                        {
                            //Set the time left to 0 and the old time left to 20
                            timeLeft = 0;
                            oldTimeLeft = 20;
                        }
                        //If the current player is not all in
                        else
                        {
                            //Reset the time
                            playerTime.Restart();
                            timeLeft = 20;
                            oldTimeLeft = 20;
                        }
                    }

                    //If the time left is equal to 0 and the old time left is equal to 20
                    if (timeLeft == 0 && oldTimeLeft == 20)
                    {
                        //Add one to the amount of turns passed this round
                        playersTurnCount++;

                        //Update the current turn
                        currentTurn = NextPossibleTurn(currentTurn);

                        //Set the send to server to be needed
                        sendToServerNeeded = true;
                    }
                    //If the time left is equal to 0
                    else if (timeLeft == 0)
                    {
                        //Add one to the amount of turns passed this round
                        playersTurnCount++;

                        //Reset the time
                        playerTime.Stop();
                        timeLeft = 20;
                        oldTimeLeft = 20;

                        //If the current bet is less then or equal to the player's bet
                        if (currentBet <= playerBet[userPlayer])
                        {
                            //Update the current turn
                            currentTurn = NextPossibleTurn(currentTurn);
                        }
                        //If the current bet is greater then the player's bet
                        else
                        {
                            //Set the user to folded
                            playersFolded[userPlayer] = true;

                            //Update the current turn
                            currentTurn = NextPossibleTurn(currentTurn);
                        }

                        //Set the send to server to be needed
                        sendToServerNeeded = true;
                    }
                }
                //If the current player is folded and/or is not playing
                else
                {
                    //Update the current turn
                    currentTurn = NextPossibleTurn(currentTurn);

                    //Set the send to server to be needed
                    sendToServerNeeded = true;
                }
            }
        }



        //Pre: The Ip of the server to connect to
        //Post: The client wil be connected to the server
        //Desc: A subprogram that connects the Client to the server
        private void ConnectLoop()
        {
            //Try doing this code
            try
            {
                //Connect to the server on the port 100 and set the client to connected
                clientSocket.Connect(serverIP, 100);
                clientConnected = true;
            }
            //Catch any exceptions
            catch
            {
                //Set the client to not connected
                clientConnected = false;
            }

            //If the client is connected to the server
            if (clientSocket.Connected)
            {
                //Send to the server the client name
                byte[] sendBuffer = Encoding.ASCII.GetBytes(currentClientName);
                clientSocket.Send(sendBuffer);

                //Start recieving messages from the server
                RecieveMessage();
            }
        }



        //Pre: A connection with the server
        //Post: The client starts recieving messages from the server
        //Desc: A subprogram that makes it so the client can start recieving messages from the server
        private void RecieveMessage()
        {
            //Array of bytes for where the message will be saved into
            recieveBuffer = new byte[1024];

            //Starts recivig messages from the server
            clientSocket.BeginReceive(recieveBuffer, 0, recieveBuffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), clientSocket);
        }



        //Pre: The server send a message to the client
        //Post: The client recieves and converts the message
        //Desc: A subprogram that recieves the message from the server and converts it to be used for the client
        private void RecieveCallBack(IAsyncResult AR)
        {
            //Variables that store the message that was recieved
            string recievedText = "";
            string temp = "";

            //Variables for the amount of clients
            int clientsAmount = 0;
            int clientsCount = 0;

            //Variable for what variable is currently being updated
            int variableUpdatingCount = 0;

            //Variable for the amount of pots
            int chipPotsCount = 0;

            //Try doing this code
            try
            {
                //Recieves the current socket that is being worked with
                Socket socket = (Socket)AR.AsyncState;

                //Recieving from the server
                int recievedSize = socket.EndReceive(AR);

                //Array of bytes
                byte[] buffer = new byte[recievedSize];

                //Copies one array into the other
                Array.Copy(recieveBuffer, buffer, recievedSize);

                //Converts the recieved buffer into a string
                recievedText = Encoding.ASCII.GetString(buffer);

                //If the game is currently in the lobby
                if (gameLobby == true)
                {
                    //If the first character of the recieved text string is a 1
                    if (recievedText[0] == '1')
                    {
                        //Remove the 1
                        recievedText = recievedText.Remove(0, 1);

                        //Update the amount of clients
                        temp = Convert.ToString(recievedText[0]);
                        clientsAmount = Convert.ToInt32(temp);
                        recievedText = recievedText.Remove(0, 1);

                        //Clear the names of the clients
                        clientNames.Clear();

                        //Update the amount of names in the client names list
                        for (int i = 0; i < clientsAmount; i++)
                        {
                            clientNames.Add("");
                        }

                        //Update each name in the clients names list
                        for (int i = 0; i < recievedText.Length; i++)
                        {
                            if (clientsCount < clientNames.Count)
                            {
                                if (recievedText[i] != '@')
                                {
                                    clientNames[clientsCount] += recievedText[i];
                                    recievedText = recievedText.Remove(0, 1);
                                    i--;
                                }
                                else
                                {
                                    clientsCount++;
                                    recievedText = recievedText.Remove(0, 1);
                                    i--;
                                }
                            }
                            else
                            {
                                i = recievedText.Length;
                            }
                        }

                        //Set the clients count to 0 and the temp string to nothing
                        clientsCount = 0;
                        temp = "";

                        //Update each client with either them being ready or not
                        for (int i = 0; i < recievedText.Length; i++)
                        {
                            if (recievedText[i] != '@')
                            {
                                temp += recievedText[i];
                                recievedText = recievedText.Remove(0, 1);
                                i--;
                            }
                            else
                            {
                                clientsReady[clientsCount] = Convert.ToBoolean(temp);
                                temp = "";

                                clientsCount++;

                                recievedText = recievedText.Remove(0, 1);
                                i--;
                            }
                        }
                    }
                    //If the first character of the recieved text string is a 2
                    else if (recievedText[0] == '2')
                    {
                        //Remove the 2
                        recievedText = recievedText.Remove(0, 1);

                        //Set the temp string to nothing
                        temp = "";

                        //Updates every client with either a true or a false for them being either ready or not
                        for (int i = 0; i < recievedText.Length; i++)
                        {
                            if (recievedText[i] != '@')
                            {
                                temp += recievedText[i];
                            }
                            else
                            {
                                clientsReady[clientsCount] = Convert.ToBoolean(temp);

                                temp = "";

                                clientsCount++;
                            }
                        }

                        //Set the amount of clients ready to 0
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

                        //If every client is ready and there is 3 or more clients that are ready
                        if (clientsReadyCount == clientNames.Count && clientsReadyCount > 2)
                        {
                            //Have the network game start
                            gameLobby = false;
                            gameState = NETWORK_GAME;
                        }
                    }
                    //If the recieved text starts with a 3
                    else if (recievedText[0] == '3')
                    {
                        //Remove the 3
                        recievedText = recievedText.Remove(0, 1);

                        //Tell the user what player that are
                        userPlayer = Convert.ToInt16(recievedText);
                    }
                }
                //If the game is not in the lobby
                else
                {
                    //If the recieved text starts with a 1
                    if (recievedText[0] == '1')
                    {
                        //Remove the 1
                        recievedText = recievedText.Remove(0, 1);

                        //Get the amount of players that are left
                        temp = Convert.ToString(recievedText[0]);
                        playersLeft = Convert.ToInt32(temp);
                        recievedText = recievedText.Remove(0, 1);

                        //Set the temp string to nothing
                        temp = "";

                        //Update the new round variable to see if a new round needs to be started
                        for (int i = 0; i < recievedText.Length; i++)
                        {
                            if (recievedText[i] != '@')
                            {
                                temp += recievedText[i];
                                recievedText = recievedText.Remove(0, 1);
                                i--;
                            }
                            else
                            {
                                newRound = Convert.ToBoolean(temp);
                                temp = "";

                                recievedText = recievedText.Remove(0, 1);
                                i = recievedText.Length;
                            }
                        }

                        //Update the last player checked variable so each clients knows if the last player was already checked
                        for (int i = 0; i < recievedText.Length; i++)
                        {
                            if (recievedText[i] != '@')
                            {
                                temp += recievedText[i];
                                recievedText = recievedText.Remove(0, 1);
                                i--;
                            }
                            else
                            {
                                lastPlayerCheck = Convert.ToBoolean(temp);
                                temp = "";

                                recievedText = recievedText.Remove(0, 1);
                                i = recievedText.Length;
                            }
                        }

                        //Set the variable updating count to 0
                        variableUpdatingCount = 0;

                        //While loop to update 7 different variables
                        while (variableUpdatingCount < 7)
                        {
                            //Set the temp string to nothing and the amount of clients to 0
                            temp = "";
                            clientsCount = 0;

                            //Loop for every client
                            for (int j = 0; j < clientsReady.Length; j++)
                            {
                                //Loop for the recieved text string
                                for (int i = 0; i < recievedText.Length; i++)
                                {
                                    //If the current index of the recieved text string is not equal to a @
                                    if (recievedText[i] != '@')
                                    {
                                        //Add to the temp the current index and remove the index
                                        temp += recievedText[i];
                                        recievedText = recievedText.Remove(0, 1);
                                        i--;
                                    }
                                    //If the current index of the recieved text string is equal to a @
                                    else
                                    {
                                        //Switch statement to update the current variable that needs to be updated
                                        switch (variableUpdatingCount)
                                        {
                                            case 0:
                                                playerPlaying[clientsCount] = Convert.ToBoolean(temp);
                                                break;
                                            case 1:
                                                playersFolded[clientsCount] = Convert.ToBoolean(temp);
                                                break;
                                            case 2:
                                                playersBluff[clientsCount] = Convert.ToBoolean(temp);
                                                break;
                                            case 3:
                                                playersBluffChecked[clientsCount] = Convert.ToBoolean(temp);
                                                break;
                                            case 4:
                                                playersAllIn[clientsCount] = Convert.ToBoolean(temp);
                                                break;
                                            case 5:
                                                playerMoney[clientsCount] = Convert.ToInt32(temp);
                                                break;
                                            case 6:
                                                playerBet[clientsCount] = Convert.ToInt32(temp);
                                                break;
                                        }
                                        //Set the temp string to nothing
                                        temp = "";

                                        //Add one to the clients count variable
                                        clientsCount++;

                                        //Remove the current index of the recieved text string
                                        recievedText = recievedText.Remove(0, 1);
                                        i = recievedText.Length;
                                    }
                                }
                            }

                            //Add one to the variable updating count
                            variableUpdatingCount++;
                        }

                        //Set the temp string to nothing and the amount of variables updated to 0
                        temp = "";
                        variableUpdatingCount = 0;

                        //Loop for every index of the recieved text string
                        for (int i = 0; i < recievedText.Length; i++)
                        {
                            //If the current index is not an @
                            if (recievedText[i] != '@')
                            {
                                //Add to the temp the current index and remove the index
                                temp += recievedText[i];
                                recievedText = recievedText.Remove(0, 1);
                                i--;
                            }
                            //If the current index is an @
                            else
                            {
                                //Switch statement to update the current variable that needs to be updated
                                switch (variableUpdatingCount)
                                {
                                    case 0:
                                        nextCardInDeck = Convert.ToInt32(temp);
                                        break;
                                    case 1:
                                        communityCardCount = Convert.ToInt32(temp);
                                        break;
                                    case 2:
                                        playersAllInCount = Convert.ToInt32(temp);
                                        break;
                                }
                                //Set the temp string to nothing
                                temp = "";

                                //Add one to the variable updating count
                                variableUpdatingCount++;

                                //Remove the current index of the string
                                recievedText = recievedText.Remove(0, 1);

                                //If the variable updating count is equal to 3
                                if (variableUpdatingCount == 3)
                                {
                                    //Stop the for loop
                                    i = recievedText.Length;
                                }
                                //If it's not equal to 3
                                else
                                {
                                    //Subtract one from the current index that's being checked
                                    i--;
                                }
                            }
                        }

                        //Set the temp string to nothing
                        temp = "";

                        //Update the cards for every player with the suit and the card value
                        for (int i = 0; i < 9; i++)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                for (int k = 0; k < 2; k++)
                                {
                                    for (int l = 0; l < recievedText.Length; l++)
                                    {
                                        if (recievedText[l] != '@')
                                        {
                                            temp += recievedText[l];
                                            recievedText = recievedText.Remove(0, 1);
                                            l--;
                                        }
                                        else
                                        {
                                            playerCards[i, j, k] = Convert.ToInt32(temp);
                                            temp = "";

                                            recievedText = recievedText.Remove(0, 1);
                                            l = recievedText.Length;
                                        }
                                    }
                                }
                            }
                        }

                        //Set the temp string to nothing
                        temp = "";

                        //Update the community cards with the suits and card values
                        for (int i = 0; i < 5; i++)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                for (int k = 0; k < recievedText.Length; k++)
                                {
                                    if (recievedText[k] != '@')
                                    {
                                        temp += recievedText[k];
                                        recievedText = recievedText.Remove(0, 1);
                                        k--;
                                    }
                                    else
                                    {
                                        communityCards[i, j] = Convert.ToInt32(temp);
                                        temp = "";

                                        recievedText = recievedText.Remove(0, 1);
                                        k = recievedText.Length;
                                    }
                                }
                            }
                        }

                        //Set the temp string to nothing
                        temp = "";

                        //Update the hand quality's for every single player
                        for (int i = 0; i < 9; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                for (int k = 0; k < recievedText.Length; k++)
                                {
                                    if (recievedText[k] != '@')
                                    {
                                        temp += recievedText[k];
                                        recievedText = recievedText.Remove(0, 1);
                                        k--;
                                    }
                                    else
                                    {
                                        handQuality[i, j] = Convert.ToInt32(temp);
                                        temp = "";

                                        recievedText = recievedText.Remove(0, 1);
                                        k = recievedText.Length;
                                    }
                                }
                            }
                        }

                        //Set the temp string to nothing
                        temp = "";

                        //Update the kickers for every single player
                        for (int i = 0; i < handKicker.Length; i++)
                        {
                            for (int j = 0; j < recievedText.Length; j++)
                            {
                                if (recievedText[j] != '@')
                                {
                                    temp += recievedText[j];
                                    recievedText = recievedText.Remove(0, 1);
                                    j--;
                                }
                                else
                                {
                                    handKicker[i] = Convert.ToInt32(temp);
                                    temp = "";

                                    recievedText = recievedText.Remove(0, 1);
                                    j = recievedText.Length;
                                }
                            }
                        }

                        //Set the temp string to nothing
                        temp = "";

                        //Update the amount of pots that are in the current round
                        for (int i = 0; i < recievedText.Length; i++)
                        {
                            if (recievedText[i] != '@')
                            {
                                temp += recievedText[i];
                                recievedText = recievedText.Remove(0, 1);
                                i--;
                            }
                            else
                            {
                                chipPotsCount = Convert.ToInt32(temp);
                                temp = "";

                                recievedText = recievedText.Remove(0, 1);
                                i = recievedText.Length;
                            }
                        }

                        //Clear the chip pots list and the players that can win list
                        chipPots.Clear();
                        playersCanWin.Clear();

                        //Add more pots to the chip pots list so it's equal to the amount of pots that are avialable 
                        for (int i = 0; i < chipPotsCount; i++)
                        {
                            chipPots.Add(0);
                            playersCanWin.Add(new bool[9]);
                        }

                        //Loop for every pot
                        for (int i = 0; i < chipPots.Count; i++)
                        {
                            //Updates the current pot
                            for (int j = 0; j < recievedText.Length; j++)
                            {
                                if (recievedText[j] != '@')
                                {
                                    temp += recievedText[j];
                                    recievedText = recievedText.Remove(0, 1);
                                    j--;
                                }
                                else
                                {
                                    chipPots[i] = Convert.ToInt32(temp);
                                    temp = "";

                                    recievedText = recievedText.Remove(0, 1);
                                    j = recievedText.Length;
                                }
                            }

                            //Updates the players that can win the current pot
                            for (int j = 0; j < playersCanWin[i].Length; j++)
                            {
                                for (int k = 0; k < recievedText.Length; k++)
                                {
                                    if (recievedText[k] != '@')
                                    {
                                        temp += recievedText[k];
                                        recievedText = recievedText.Remove(0, 1);
                                        k--;
                                    }
                                    else
                                    {
                                        playersCanWin[i][j] = Convert.ToBoolean(temp);
                                        temp = "";

                                        recievedText = recievedText.Remove(0, 1);
                                        k = recievedText.Length;
                                    }
                                }
                            }
                        }

                        //Updates the amount of players that are currently tied to the winner
                        for (int i = 0; i < recievedText.Length; i++)
                        {
                            if (recievedText[i] != '@')
                            {
                                temp += recievedText[i];
                                recievedText = recievedText.Remove(0, 1);
                                i--;
                            }
                            else
                            {
                                playersTiedCount = Convert.ToInt32(temp);
                                temp = "";

                                recievedText = recievedText.Remove(0, 1);
                                i = recievedText.Length;
                            }
                        }

                        //Clears the players tied list
                        playersTied.Clear();

                        //Adds an index to the list for every player that is tied
                        for (int i = 0; i < playersTiedCount; i++)
                        {
                            playersTied.Add(0);
                        }

                        //Updates the player's tied list with every player that is tied
                        for (int i = 0; i < playersTied.Count; i++)
                        {
                            for (int j = 0; j < recievedText.Length; j++)
                            {
                                if (recievedText[j] != '@')
                                {
                                    temp += recievedText[j];
                                    recievedText = recievedText.Remove(0, 1);
                                    j--;
                                }
                                else
                                {
                                    playersTied[i] = Convert.ToInt32(temp);
                                    temp = "";

                                    recievedText = recievedText.Remove(0, 1);
                                    j = recievedText.Length;
                                }
                            }
                        }

                        //Sets the variable updating count to 0
                        variableUpdatingCount = 0;

                        //While loop for 11 variables that will be updated
                        while (variableUpdatingCount < 11)
                        {
                            //Set the temp string to nothing
                            temp = "";

                            //Loop for every index of the recieved text string
                            for (int i = 0; i < recievedText.Length; i++)
                            {
                                //If the current index is not an @
                                if (recievedText[i] != '@')
                                {
                                    //Add to the temp the current index and remove the index
                                    temp += recievedText[i];
                                    recievedText = recievedText.Remove(0, 1);
                                    i--;
                                }
                                //If the current index is an @
                                else
                                {
                                    //Switch statement to update the current variable that needs to be updated
                                    switch (variableUpdatingCount)
                                    {
                                        case 0:
                                            currentTurn = Convert.ToInt32(temp);
                                            break;
                                        case 1:
                                            bigBlindPlayer = Convert.ToInt32(temp);
                                            break;
                                        case 2:
                                            smallBlindPlayer = Convert.ToInt32(temp);
                                            break;
                                        case 3:
                                            blindAmount = Convert.ToInt32(temp);
                                            break;
                                        case 4:
                                            turnCount = Convert.ToInt32(temp);
                                            break;
                                        case 5:
                                            playersTurnCount = Convert.ToInt32(temp);
                                            break;
                                        case 6:
                                            currentBet = Convert.ToInt32(temp);
                                            break;
                                        case 7:
                                            currentPayOut = Convert.ToInt32(temp);
                                            break;
                                        case 8:
                                            playerWon = Convert.ToInt32(temp);
                                            break;
                                        case 9:
                                            playersTurnCount = Convert.ToInt32(temp);
                                            break;
                                        case 10:
                                            timeLeft = Convert.ToInt32(temp);
                                            break;
                                    }
                                    //Set the temp string to nothing
                                    temp = "";

                                    //Remove the first index from the recieved text string
                                    recievedText = recievedText.Remove(0, 1);
                                    i = recievedText.Length;
                                }
                            }

                            //Add one to the current variable that is being updated
                            variableUpdatingCount++;
                        }

                        //Set the temp string to nothing
                        temp = "";

                        //Updates every player's name
                        for (int i = 0; i < playerNames.Length; i++)
                        {
                            for (int j = 0; j < recievedText.Length; j++)
                            {
                                if (recievedText[j] != '@')
                                {
                                    temp += recievedText[j];
                                    recievedText = recievedText.Remove(0, 1);
                                    j--;
                                }
                                else
                                {
                                    playerNames[i] = temp;
                                    temp = "";

                                    recievedText = recievedText.Remove(0, 1);
                                    j = recievedText.Length;
                                }
                            }
                        }

                        //Sets the game to started
                        gameStarted = true;
                    }
                    //Sets the reply to recieved
                    replyRecieved = true;
                }

                //Start receiving again
                socket.BeginReceive(recieveBuffer, 0, recieveBuffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), socket);
            }
            //Catch any exceptions
            catch
            {
                //Don't do anything with the exceptions
            }
        }



        //Pre: None
        //Post: The AI player's move is decided
        //Desc: A subprogram that decides the move of the AI
        private void ArtificialIntelligenceMove()
        {
            //Variable for the min and max bet of the AI
            int playerMinBet = 0;
            int playerMaxBet = 0;

            //Variable for if the player is bluffing
            int playerBluffValue = 0;

            //Variable for the raise amount
            int raiseAmount = 0;

            //Variable for the max all in amount
            int maxAllInAmount = 0;

            //Loop for every player
            for (int i = 0; i < playerMoney.Length; i++)
            {
                //If it's not the current player's turn and the current playre did not fold
                if (i != currentTurn && playersFolded[i] == false)
                {
                    //If the current player's money plus their bet is greater then the current max all in amount
                    if (playerMoney[i] + playerBet[i] > maxAllInAmount)
                    {
                        //Update teh max all in amount with the current player's money plus their bet
                        maxAllInAmount = playerMoney[i] + playerBet[i];
                    }
                }
            }

            //Set the open ended draw, inside straight draw, and flush draw to false
            openEndedstraightDraw = false;
            insideStraightDraw = false;
            flushDraw = false;

            //Checks the hand quality of the player
            CheckHandQuality(currentTurn);

            //Switch statement for the amount of community cards
            switch (communityCardCount)
            {
                //If there is 0 community cards
                case 0:
                    //If the player was not checked for bluffing
                    if (playersBluffChecked[currentTurn] == false)
                    {
                        //Set the bluffed to check for the current player
                        playersBluffChecked[currentTurn] = true;

                        //Randomly generate a value
                        playerBluffValue = randInt.Next(1, 11);

                        //If the value is greater then 8 the player is bluffing
                        if (playerBluffValue > 8)
                        {
                            playersBluff[currentTurn] = true;
                        }
                    }

                    //Update the minium and maximum bet's of the player depending on their hand quality 
                    if (handQuality[currentTurn, 0] == 2)
                    {
                        if (handQuality[currentTurn, 1] == 14 || handQuality[currentTurn, 1] == 13)
                        {
                            playerMinBet = blindAmount * 2;
                            playerMaxBet = playerMoney[currentTurn] + playerBet[currentTurn];
                        }
                        else if (handQuality[currentTurn, 1] >= 10)
                        {
                            playerMinBet = blindAmount * 2;
                            playerMaxBet = blindAmount * 4;
                        }
                        else
                        {
                            playerMinBet = blindAmount;
                            playerMaxBet = blindAmount * 3;
                        }
                    }
                    else if (handQuality[currentTurn, 0] == 1 && handQuality[currentTurn, 1] >= 22)
                    {
                        if (handQuality[currentTurn, 1] >= 25)
                        {
                            playerMinBet = blindAmount;
                            playerMaxBet = playerMoney[currentTurn] + playerBet[currentTurn];
                        }
                        else
                        {
                            playerMinBet = blindAmount;
                            playerMaxBet = blindAmount * 4;
                        }
                    }
                    else
                    {
                        if (playersBluff[currentTurn] == true)
                        {
                            playerMinBet = blindAmount * 2;
                            playerMaxBet = blindAmount * 2;
                        }
                        else
                        {
                            playerMinBet = blindAmount;
                            playerMaxBet = blindAmount * 2;
                        }
                    }
                    break;
                //If there is 3 or 4 community cards
                case 3:
                case 4:
                    //Switch statement for the handquality of the current player to update the player's min and max bets depending on how good there hands are
                    switch (handQuality[currentTurn, 0])
                    {
                        case 10:
                        case 9:
                        case 8:
                        case 7:
                            playerMinBet = blindAmount * 7;
                            playerMaxBet = playerMoney[currentTurn] + playerBet[currentTurn];
                            break;
                        case 6:
                            if (handQuality[currentTurn, 1] >= 10)
                            {
                                if (handQuality[currentTurn, 1] >= 13)
                                {
                                    playerMinBet = blindAmount * 6;
                                    playerMaxBet = playerMoney[currentTurn] + playerBet[currentTurn];
                                }
                                else
                                {
                                    playerMinBet = blindAmount * 5;
                                    playerMaxBet = playerMoney[currentTurn] + playerBet[currentTurn];
                                }
                            }
                            else
                            {
                                playerMinBet = blindAmount * 4;
                                playerMaxBet = blindAmount * 14;
                            }
                            break;
                        case 5:
                            if (handQuality[currentTurn, 1] >= 10)
                            {
                                if (handQuality[currentTurn, 1] >= 13)
                                {
                                    playerMinBet = blindAmount * 5;
                                    playerMaxBet = playerMoney[currentTurn] + playerBet[currentTurn];
                                }
                                else
                                {
                                    playerMinBet = blindAmount * 4;
                                    playerMaxBet = playerMoney[currentTurn] + playerBet[currentTurn];
                                }
                            }
                            else
                            {
                                playerMinBet = blindAmount * 4;
                                playerMaxBet = blindAmount * 12;
                            }
                            break;
                        case 4:
                            if (handQuality[currentTurn, 1] >= 10)
                            {
                                if (handQuality[currentTurn, 1] >= 12)
                                {
                                    if (handQuality[currentTurn, 2] >= 12)
                                    {
                                        playerMinBet = blindAmount * 4;
                                        playerMaxBet = blindAmount * 10;
                                    }
                                    else if (handQuality[currentTurn, 2] >= 10)
                                    {
                                        playerMinBet = blindAmount * 3;
                                        playerMaxBet = blindAmount * 10;
                                    }
                                    else
                                    {
                                        playerMinBet = blindAmount * 3;
                                        playerMaxBet = blindAmount * 8;
                                    }
                                }
                                else
                                {
                                    playerMinBet = blindAmount * 2;
                                    playerMaxBet = blindAmount * 8;
                                }
                            }
                            else
                            {
                                playerMinBet = blindAmount * 2;
                                playerMaxBet = blindAmount * 8;
                            }
                            break;
                        case 3:
                            if (handQuality[currentTurn, 1] >= 10)
                            {
                                if (handQuality[currentTurn, 1] >= 12)
                                {
                                    if (handQuality[currentTurn, 2] >= 10)
                                    {
                                        playerMinBet = blindAmount * 2;
                                        playerMaxBet = blindAmount * 8;
                                    }
                                    else
                                    {
                                        if (handKicker[currentTurn] >= 12)
                                        {
                                            playerMinBet = blindAmount * 2;
                                            playerMaxBet = blindAmount * 6;
                                        }
                                        else
                                        {
                                            playerMinBet = blindAmount * 2;
                                            playerMaxBet = blindAmount * 5;
                                        }
                                    }
                                }
                                else
                                {
                                    playerMinBet = blindAmount;
                                    playerMaxBet = blindAmount * 4;
                                }
                            }
                            else
                            {
                                playerMinBet = blindAmount;
                                playerMaxBet = blindAmount * 4;
                            }
                            break;
                        case 2:
                            if (handQuality[currentTurn, 1] >= 11)
                            {
                                playerMinBet = blindAmount - blindAmount;
                                playerMaxBet = blindAmount * 4;
                            }
                            else
                            {
                                playerMinBet = blindAmount - blindAmount;
                                playerMaxBet = blindAmount * 2;
                            }

                            if (playersBluff[currentTurn] == true)
                            {
                                if (communityCardCount == 3)
                                {
                                    playerMinBet = blindAmount * 3;
                                    playerMaxBet = blindAmount * 3;
                                }
                                else
                                {
                                    playerMinBet = blindAmount * 5;
                                    playerMaxBet = blindAmount * 5;
                                }
                            }
                            break;
                        case 1:
                            if (playersBluff[currentTurn] == true)
                            {
                                if (communityCardCount == 3)
                                {
                                    playerMinBet = blindAmount * 2;
                                    playerMaxBet = blindAmount * 2;
                                }
                                else
                                {
                                    playerMinBet = blindAmount * 4;
                                    playerMaxBet = blindAmount * 4;
                                }
                            }
                            else
                            {
                                playerMinBet = blindAmount - blindAmount;
                                playerMaxBet = blindAmount;
                            }
                            break;
                    }

                    //If the community card count is equal to 3
                    if (communityCardCount == 3)
                    {
                        //If any draw is possible
                        if (flushDraw == true || openEndedstraightDraw == true || insideStraightDraw == true)
                        {
                            //Set their max bet to all their money
                            playerMaxBet = playerMoney[currentTurn] + playerBet[currentTurn];
                        }
                    }
                    //If the community card count is not equal to 3
                    else
                    {
                        //If a flush or open ended straight draw is possible
                        if (flushDraw == true || openEndedstraightDraw == true)
                        {
                            //Set their max bet to all their money
                            playerMaxBet = playerMoney[currentTurn] + playerBet[currentTurn];
                        }
                        //If an inside straight draw is possible
                        else if (insideStraightDraw == true)
                        {
                            //If the max bet of the player is smaller then the blind amount * 14
                            if (playerMaxBet < blindAmount * 14)
                            {
                                //Set the player's max bet to the blind amount * 14
                                playerMaxBet = blindAmount * 14;
                            }
                        }
                    }
                    break;
                //If there is 5 community cards
                case 5:
                    //Switch statement for the handquality of the current player to update the player's min and max bets depending on how good there hands are
                    switch (handQuality[currentTurn, 0])
                    {
                        case 10:
                        case 9:
                        case 8:
                        case 7:
                            playerMinBet = playerMoney[currentTurn] + playerBet[currentTurn];
                            playerMaxBet = playerMoney[currentTurn] + playerBet[currentTurn];
                            break;
                        case 6:
                        case 5:
                            if (handQuality[currentTurn, 1] >= 12)
                            {
                                playerMinBet = playerMoney[currentTurn] + playerBet[currentTurn];
                                playerMaxBet = playerMoney[currentTurn] + playerBet[currentTurn];
                            }
                            else
                            {
                                if (handQuality[currentTurn, 0] == 6)
                                {
                                    playerMinBet = playerMoney[currentTurn] + playerBet[currentTurn];
                                    playerMaxBet = playerMoney[currentTurn] + playerBet[currentTurn];
                                }
                                else
                                {
                                    playerMinBet = blindAmount * 7;
                                    playerMaxBet = playerMoney[currentTurn] + playerBet[currentTurn];
                                }
                            }
                            break;
                        case 4:
                            playerMinBet = blindAmount * 4;
                            playerMaxBet = Convert.ToInt32((playerMoney[currentTurn] + playerBet[currentTurn]) / 2);
                            break;
                        case 3:
                            playerMinBet = blindAmount * 2;
                            playerMaxBet = Convert.ToInt32((playerMoney[currentTurn] + playerBet[currentTurn]) / 4);
                            break;
                        case 2:
                            if (playersBluff[currentTurn] == true)
                            {
                                playerMinBet = blindAmount * 8;
                                playerMaxBet = blindAmount * 8;
                            }
                            else
                            {
                                playerMinBet = blindAmount - blindAmount;
                                playerMaxBet = blindAmount * 2;
                            }
                            break;
                        case 1:
                            if (playersBluff[currentTurn] == true)
                            {
                                playerMinBet = blindAmount * 6;
                                playerMaxBet = blindAmount * 6;
                            }
                            else
                            {
                                playerMinBet = blindAmount - blindAmount;
                                playerMaxBet = blindAmount;
                            }
                            break;
                    }
                    break;
            }

            //If the current bet is greater then the max bet of the player
            if (currentBet > playerMaxBet)
            {
                //If the max bet of the player is equal to or greater then all the player's money
                if (playerMaxBet >= playerMoney[currentTurn] + playerBet[currentTurn])
                {
                    //Set the player's bet to all their money and update the player's money to 0
                    playerBet[currentTurn] = playerMoney[currentTurn] + playerBet[currentTurn];
                    playerMoney[currentTurn] -= playerMoney[currentTurn];
                }
                //If the max bet of the player is less then all the player's money
                else
                {
                    //Set the player to folded
                    playersFolded[currentTurn] = true;
                }
            }
            //If the current bet is less then or equal to the max bet of the player
            else
            {
                //If the current bet is greater then the min player bet
                if (currentBet > playerMinBet)
                {
                    //Set the raise amount equal to the difference between the current bet and the current player's bet
                    raiseAmount = currentBet - playerBet[currentTurn];

                    //If the money of the current player is greater then or equal to the raise amount
                    if (playerMoney[currentTurn] >= raiseAmount)
                    {
                        //Add the amount fo the raise amount to the player's bet and subtract that money from the player's money
                        playerBet[currentTurn] += raiseAmount;
                        playerMoney[currentTurn] -= raiseAmount;
                    }
                    //If the money of the current player is less then the raise amount
                    else
                    {
                        //Add all the money of the player to the player's bet and set their money to 0
                        playerBet[currentTurn] += playerMoney[currentTurn];
                        playerMoney[currentTurn] = 0;
                    }
                }
                //If the current bet is less then or equal to the min player bet
                else
                {
                    //If the current bet is less then the min player bet
                    if (currentBet < playerMinBet)
                    {
                        //If the min bet is equal to the player's money plus their bet
                        if (playerMinBet == playerMoney[currentTurn] + playerBet[currentTurn])
                        {
                            //If the min bet is greater then the max all in amount
                            if (playerMinBet > maxAllInAmount)
                            {
                                //The current bet is equal to the max all in amount
                                currentBet = maxAllInAmount;
                            }
                            //If the min bet is less then or equal to the max all in amount
                            else
                            {
                                //The current bet is equal to the player's min bet
                                currentBet = playerMinBet;
                            }
                        }
                        //If the min bet is not equal to the player's money plus their bet
                        else
                        {
                            //The current bet is equal to the player's min bet
                            currentBet = playerMinBet;
                        }
                    }

                    //If the player's min bet is greater then the max all in amount
                    if (playerMinBet > maxAllInAmount)
                    {
                        //The raise amount is equal to the difference between the max all in amount and the player;s bet
                        raiseAmount = maxAllInAmount - playerBet[currentTurn];
                    }
                    //If the player's min bet is not greater then the max all in amount
                    else
                    {
                        //The raise amount is equal to the min player's bet minus their bet
                        raiseAmount = playerMinBet - playerBet[currentTurn];
                    }

                    //If the player's money is greater then or equal to the raise amount
                    if (playerMoney[currentTurn] >= raiseAmount)
                    {
                        //Add the amount of the raise amount to the player's bet and subtract that from the player's money
                        playerBet[currentTurn] += raiseAmount;
                        playerMoney[currentTurn] -= raiseAmount;
                    }
                    //If the player's money is not greater then or equal to the raise amount
                    else
                    {
                        //Add the amount of money the player has to the player's bet and set their money to 0
                        playerBet[currentTurn] += playerMoney[currentTurn];
                        playerMoney[currentTurn] = 0;
                    }
                }
            }

            //Stop and reset the timer
            playerTime.Stop();
            timeLeft = 20;
            oldTimeLeft = 20;

            //Set the chips to moving and update their values
            playerChipsMoving[currentTurn] = true;
            SetChipMovingValues(currentTurn);
        }



        //Pre: None
        //Post: The player who won and all the player's that tied to them
        //Desc: A subprogram that returns the player who won and also updates which player's tied to the player who won
        private int CheckWhoWon()
        {
            //Variable for the player who won
            int playerWon = 0;

            //Variable for the highest kicker
            int highestKicker = 0;

            //Array for the highest hand
            int[,] highestHand = new int[1, 3];

            //Clears the players tied list
            playersTied.Clear();

            //Loop for every player
            for (int i = 0; i < playerPlaying.Length; i++)
            {
                //If the current player is playing
                if (playerPlaying[i] == true)
                {
                    //Check their hand quality
                    CheckHandQuality(i);
                }
            }

            //Loop for all the players
            for (int i = 0; i < playerPlaying.Length; i++)
            {
                //If the current player is playing and is not folded
                if (playerPlaying[i] == true && playersFolded[i] == false)
                {
                    //If the first value of the hand quality of the current player is greater then the first value of the current highest hand
                    if (handQuality[i, 0] > highestHand[0, 0])
                    {
                        //Update the current highest hand with the current hand quality
                        highestHand[0, 0] = handQuality[i, 0];
                        highestHand[0, 1] = handQuality[i, 1];
                        highestHand[0, 2] = handQuality[i, 2];

                        //Update the player who is winning
                        playerWon = i;
                    }
                    //If the hand quality's are even
                    else if (handQuality[i, 0] == highestHand[0, 0])
                    {
                        if (handQuality[i, 1] > highestHand[0, 1] && handQuality[i, 1] != 0)
                        {
                            //Update the current highest hand with the current hand quality
                            highestHand[0, 0] = handQuality[i, 0];
                            highestHand[0, 1] = handQuality[i, 1];
                            highestHand[0, 2] = handQuality[i, 2];

                            //Update the player who is winning
                            playerWon = i;
                        }
                        //If the hand quality's are even
                        else if (handQuality[i, 1] == highestHand[0, 1] && handQuality[i, 1] != 0)
                        {
                            //If the first value for the hand quality is equal to 3
                            if (handQuality[i, 0] == 3)
                            {
                                //If the current kicker is higher then the highest kicker
                                if (handKicker[i] > highestKicker)
                                {
                                    //Update the current highest hand with the current hand quality
                                    highestHand[0, 0] = handQuality[i, 0];
                                    highestHand[0, 1] = handQuality[i, 1];
                                    highestHand[0, 2] = handQuality[i, 2];

                                    //Update the kickers
                                    highestKicker = handKicker[i];

                                    //Update the player who is winning
                                    playerWon = i;
                                }
                                //If the hand kickers are even
                                else if (handKicker[i] == highestKicker)
                                {
                                    //Add a player that tied
                                    playersTied.Add(i);
                                }
                            }
                            //If the second value of the hand quality is higher then that of the highest hand and the second value for the hand quality is not equal to 0
                            else if (handQuality[i, 2] > highestHand[0, 2] && handQuality[i, 2] != 0)
                            {
                                //Update the current highest hand with the current hand quality
                                highestHand[0, 0] = handQuality[i, 0];
                                highestHand[0, 1] = handQuality[i, 1];
                                highestHand[0, 2] = handQuality[i, 2];

                                //Update the player who is winning
                                playerWon = i;
                            }
                            //If the hand quality's are even
                            else if (handQuality[i, 2] == highestHand[0, 2] && handQuality[i, 2] != 0)
                            {
                                //Add a player that tied
                                playersTied.Add(i);
                            }
                            //If the third value for the hand quality is equal to 0
                            else if (handQuality[i, 2] == 0)
                            {
                                //Add a player that tied
                                playersTied.Add(i);
                            }
                        }
                        //If the second value for the hand quality is equal to 0
                        else if (handQuality[i, 1] == 0)
                        {
                            //Add a player that tied
                            playersTied.Add(i);
                        }
                    }
                }
            }

            //Update the amount of players that are tied
            playersTiedCount = playersTied.Count;

            //Loop for all the players that are tied
            for (int i = 0; i < playersTiedCount; i++)
            {
                //If the first value for the hand quality is equal to 3
                if (handQuality[playersTied[i], 0] == 3)
                {
                    //If highest hand does not match the hand quality of the player that tied
                    if (highestHand[0, 0] != handQuality[playersTied[i], 0] || highestHand[0, 1] != handQuality[playersTied[i], 1] ||
                    highestHand[0, 2] != handQuality[playersTied[i], 2] || highestKicker != handKicker[playersTied[i]])
                    {
                        //Remove the player that tied from the list
                        playersTied.RemoveAt(i);
                        playersTiedCount--;
                        i--;
                    }
                }
                //If highest hand does not match the hand quality of the player that tied
                else if (highestHand[0, 0] != handQuality[playersTied[i], 0] || highestHand[0, 1] != handQuality[playersTied[i], 1] ||
                    highestHand[0, 2] != handQuality[playersTied[i], 2])
                {
                    //Remove the player that tied from the list
                    playersTied.RemoveAt(i);
                    playersTiedCount--;
                    i--;
                }
            }

            //Return which player won
            return playerWon;
        }



        //Pre: All the information for the game to be saved
        //Post: The game is saved
        //Desc: A subprogram that saved the game
        private void SaveGame()
        {
            //Opens the file which the information will be saved in
            saveGameFile = new StreamWriter(saveFileLocation);

            //Saves information to the file
            saveGameFile.WriteLine(gameStarted);
            saveGameFile.WriteLine(newGame);
            saveGameFile.WriteLine(newRound);
            saveGameFile.WriteLine(playersLeft);
            saveGameFile.WriteLine(userPlayer);

            //Loop for all 9 players
            for (int i = 0; i < playerPlaying.Length; i++)
            {
                //Saves information to the file
                saveGameFile.WriteLine(playerPlaying[i]);
                saveGameFile.WriteLine(playersFolded[i]);
                saveGameFile.WriteLine(playersBluff[i]);
                saveGameFile.WriteLine(playersBluffChecked[i]);
                saveGameFile.WriteLine(playersAllIn[i]);
                saveGameFile.WriteLine(playerMoney[i]);
                saveGameFile.WriteLine(playerBet[i]);
            }

            //Saves information to the file
            saveGameFile.WriteLine(nextCardInDeck);
            saveGameFile.WriteLine(communityCardCount);
            saveGameFile.WriteLine(playersAllInCount);

            //Loop for every card that was shuffled
            for (int i = 0; i < shuffleCards.Length; i++)
            {
                //Saves every card in the file
                saveGameFile.WriteLine(shuffleCards[i]);
            }

            //Saves the cards for every single player
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        saveGameFile.WriteLine(playerCards[i, j, k]);
                    }
                }
            }

            //Saves every single community card
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    saveGameFile.WriteLine(communityCards[i, j]);
                }
            }

            //Saves the hand quality's for every player
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    saveGameFile.WriteLine(handQuality[i, j]);
                }
            }

            //Saves the kickers for every player
            for (int i = 0; i < handKicker.Length; i++)
            {
                saveGameFile.WriteLine(handKicker[i]);
            }

            //Saves the amount of pots
            saveGameFile.WriteLine(chipPots.Count);

            //Saves every pot and which players can which each pot
            for (int i = 0; i < chipPots.Count; i++)
            {
                saveGameFile.WriteLine(chipPots[i]);

                for (int j = 0; j < 9; j++)
                {
                    saveGameFile.WriteLine(playersCanWin[i][j]);
                }
            }

            //Saves the amount of players that are tied
            saveGameFile.WriteLine(playersTied.Count);

            //Saves each player that is tied
            for (int i = 0; i < playersTied.Count; i++)
            {
                saveGameFile.WriteLine(playersTied[i]);
            }

            //Saves information to the file
            saveGameFile.WriteLine(currentTurn);
            saveGameFile.WriteLine(bigBlindPlayer);
            saveGameFile.WriteLine(smallBlindPlayer);
            saveGameFile.WriteLine(blindAmount);
            saveGameFile.WriteLine(turnCount);
            saveGameFile.WriteLine(playersTurnCount);
            saveGameFile.WriteLine(currentBet);
            saveGameFile.WriteLine(userRaiseAmount);
            saveGameFile.WriteLine(currentPayOut);
            saveGameFile.WriteLine(playerWon);
            saveGameFile.WriteLine(playersTiedCount);

            //Saves all the names of every player to the file
            for (int i = 0; i < playerNames.Length; i++)
            {
                saveGameFile.WriteLine(playerNames[i]);
            }

            //Closes the file
            saveGameFile.Close();
        }



        //Pre: The file to load
        //Post: The file is loaded with all the information
        //Desc: A subprogram to load the file
        private void LoadGame()
        {
            //Variable for the amount of chip pots
            int chipPotsCount = 0;

            //Opens the file to read from it
            loadGameFile = new StreamReader(saveFileLocation);

            //Loads in information from the file and updates variables
            gameStarted = Convert.ToBoolean(loadGameFile.ReadLine());
            newGame = Convert.ToBoolean(loadGameFile.ReadLine());
            newRound = Convert.ToBoolean(loadGameFile.ReadLine());
            playersLeft = Convert.ToInt32(loadGameFile.ReadLine());
            userPlayer = Convert.ToInt32(loadGameFile.ReadLine());

            //Loop for all 9 players
            for (int i = 0; i < playerPlaying.Length; i++)
            {
                //Loads in information from the file and updates variables
                playerPlaying[i] = Convert.ToBoolean(loadGameFile.ReadLine());
                playersFolded[i] = Convert.ToBoolean(loadGameFile.ReadLine());
                playersBluff[i] = Convert.ToBoolean(loadGameFile.ReadLine());
                playersBluffChecked[i] = Convert.ToBoolean(loadGameFile.ReadLine());
                playersAllIn[i] = Convert.ToBoolean(loadGameFile.ReadLine());
                playerMoney[i] = Convert.ToInt32(loadGameFile.ReadLine());
                playerBet[i] = Convert.ToInt32(loadGameFile.ReadLine());
            }

            //Loads in information from the file and updates variables
            nextCardInDeck = Convert.ToInt32(loadGameFile.ReadLine());
            communityCardCount = Convert.ToInt32(loadGameFile.ReadLine());
            playersAllInCount = Convert.ToInt32(loadGameFile.ReadLine());

            //Updates every single card in the shuffle cards array
            for (int i = 0; i < shuffleCards.Length; i++)
            {
                shuffleCards[i] = Convert.ToInt32(loadGameFile.ReadLine());
            }

            //Updates the cards of every player
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        playerCards[i, j, k] = Convert.ToInt32(loadGameFile.ReadLine());
                    }
                }
            }

            //Updates the community cards
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    communityCards[i, j] = Convert.ToInt32(loadGameFile.ReadLine());
                }
            }

            //Updates the hand quality's of every player
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    handQuality[i, j] = Convert.ToInt32(loadGameFile.ReadLine());
                }
            }

            //Updates the kicker's for every player
            for (int i = 0; i < handKicker.Length; i++)
            {
                handKicker[i] = Convert.ToInt32(loadGameFile.ReadLine());
            }

            //Updates the amount of pots there are
            chipPotsCount = Convert.ToInt32(loadGameFile.ReadLine());

            //Clears the pots and players can win lists
            chipPots.Clear();
            playersCanWin.Clear();

            //Adds the amount of pots to the list
            for (int i = 0; i < chipPotsCount; i++)
            {
                chipPots.Add(0);
                playersCanWin.Add(new bool[9]);
            }

            //Updates every pot with the amount of chips it is and which players can win the current pot
            for (int i = 0; i < chipPots.Count; i++)
            {
                chipPots[i] = Convert.ToInt32(loadGameFile.ReadLine());

                for (int j = 0; j < 9; j++)
                {
                    playersCanWin[i][j] = Convert.ToBoolean(loadGameFile.ReadLine());
                }
            }

            //Updates the amount of players that are tied
            playersTiedCount = Convert.ToInt32(loadGameFile.ReadLine());

            //Clears the list of players that are tied
            playersTied.Clear();

            //Adds index's for every player that is tied
            for (int i = 0; i < playersTiedCount; i++)
            {
                playersTied.Add(0);
            }

            //Updates which player's are tied
            for (int i = 0; i < playersTied.Count; i++)
            {
                playersTied[i] = Convert.ToInt32(loadGameFile.ReadLine());
            }

            //Updates information for the game
            currentTurn = Convert.ToInt32(loadGameFile.ReadLine());
            bigBlindPlayer = Convert.ToInt32(loadGameFile.ReadLine());
            smallBlindPlayer = Convert.ToInt32(loadGameFile.ReadLine());
            blindAmount = Convert.ToInt32(loadGameFile.ReadLine());
            turnCount = Convert.ToInt32(loadGameFile.ReadLine());
            playersTurnCount = Convert.ToInt32(loadGameFile.ReadLine());
            currentBet = Convert.ToInt32(loadGameFile.ReadLine());
            userRaiseAmount = Convert.ToInt32(loadGameFile.ReadLine());
            currentPayOut = Convert.ToInt32(loadGameFile.ReadLine());
            playerWon = Convert.ToInt32(loadGameFile.ReadLine());
            playersTiedCount = Convert.ToInt32(loadGameFile.ReadLine());

            //Updates the names of every player
            for (int i = 0; i < playerNames.Length; i++)
            {
                playerNames[i] = loadGameFile.ReadLine();
            }

            //Closes the file
            loadGameFile.Close();

            //Resets the time
            playerTime.Reset();
            timeLeft = 20;
        }



        //Pre: None
        //Post: The cards are shuffled
        //Desc: A subprogram that shuffles all the cards
        private void ShuffleCards()
        {
            //Variables for the current card being sent and for the amount of cards set
            bool cardSet;
            int count;

            //Loop for every card in the deck
            for (int i = 0; i < shuffleCards.Length; i++)
            {
                //Set the card set to false
                cardSet = false;

                //Set the current card to 0
                shuffleCards[i] = 0;

                //While the card set is false stay in this loop
                while (cardSet == false)
                {
                    //generate a random number and store it as a card
                    shuffleCards[i] = randInt.Next(1, shuffleCards.Length + 1);

                    //Set the count to 0
                    count = 0;

                    //While the count is less then 52, stay in this loop
                    while (count < 52)
                    {
                        //If there is the same card twice in the deck
                        if (shuffleCards[i] == shuffleCards[count] && i != count)
                        {
                            //Stop the loop
                            count = shuffleCards.Length - 1;
                        }
                        //If the count is equal to one less then the length of the shuffle cards
                        else if (count == shuffleCards.Length - 1)
                        {
                            //Set the card set to true
                            cardSet = true;
                        }

                        //Add one to the count
                        count++;
                    }
                }
            }
        }



        //Pre: The card number
        //Post: The value of the card
        //Desc: A subprogram that returns that value of the card
        private int CardValue(int cardNum)
        {
            //Variable for the card value
            int cardValue;

            //Set the cardvalue equal to the card num mod 13
            cardValue = cardNum % 13;

            //If the card value is equal to 0
            if (cardValue == 0)
            {
                //Set it equal to 13
                cardValue = 13;
            }

            //Returns the card value
            return cardValue;
        }



        //Pre: The card number
        //Post: The suit of the card
        //Desc: A subprogram that returns the suit of the card
        private int CardSuit(int cardNum)
        {
            //Variable for the suit of the card
            int cardSuit = 0;

            //If the card num is greater then 0 and smaller then 14
            if (cardNum >= 1 && cardNum <= 13)
            {
                //Set the suit to clubs
                cardSuit = 1;
            }
            //If the card num is greater then 13 and smaller then 27
            else if (cardNum >= 14 && cardNum <= 26)
            {
                //Set the suit to diamonds
                cardSuit = 2;
            }
            //If the card num is greater then 26 and smaller then 40
            else if (cardNum >= 27 && cardNum <= 39)
            {
                //Set the suit to hearts
                cardSuit = 3;
            }
            //If the card num is greater then 39 and smaller then 53
            else if (cardNum >= 40 && cardNum <= 52)
            {
                //Set the suit to spades
                cardSuit = 4;
            }

            //Return the suit of the card
            return cardSuit;
        }



        //Pre: None
        //Post: The cards are dealt to all the player's
        //Desc: The subprogram deals the cards to every player that is playing
        private void DealCurrentRound()
        {
            //Variable for if the first card is being dealt and which card is currently being dealt
            int temp = 0;
            bool firstCard = true;

            //Loop for every card the player's can have
            for (int i = 0; i < playersLeft * 2; i++)
            {
                //If the first card is being dealt
                if (firstCard == true)
                {
                    //While the first card is being dealt loop this
                    while (firstCard == true)
                    {
                        //If the current player is playing
                        if (playerPlaying[temp] == true)
                        {
                            //Gives a value to the current player's card
                            playerCards[temp, 0, 0] = CardValue(shuffleCards[i]);
                            playerCards[temp, 0, 1] = CardSuit(shuffleCards[i]);

                            //Adds one to the temp and the next available card
                            temp++;
                            nextCardInDeck++;

                            //If teh temp is equal to 9
                            if (temp == 9)
                            {
                                //Set the first card to false and the temp back to 0
                                firstCard = false;
                                temp = 0;
                            }

                            break;
                        }
                        //If the current player is not playing
                        else
                        {
                            //Add one to the temp
                            temp++;

                            //If the temp is equal to 9
                            if (temp == 9)
                            {
                                //Set the first card to false and the temp back to 0
                                firstCard = false;
                                temp = 0;

                                //Subtract one from the index that is currently being checked
                                i--;
                            }
                        }
                    }
                }
                //If the first card is falsek
                else
                {
                    //While the temp is smaller then the amount of player's playing
                    while (temp < playerPlaying.Length)
                    {
                        //If the current player is playing
                        if (playerPlaying[temp] == true)
                        {
                            //Gives a value to the current player's card
                            playerCards[temp, 1, 0] = CardValue(shuffleCards[i]);
                            playerCards[temp, 1, 1] = CardSuit(shuffleCards[i]);

                            //Adds one to the temp and the next available card
                            nextCardInDeck++;
                            temp++;
                            break;
                        }
                        //If the current player is not playing
                        else
                        {
                            //Adds one to the temp variable
                            temp++;
                        }
                    }
                }
            }
        }



        //Pre: The current cards that are being checked
        //Post: The cards are organized from lowest to highest
        //Desc: This subprogram returns the cards that it's given but it re-organizes them from lowest to highest before returning them
        private int[,] OrganizeCards(int[,] currentCards)
        {
            //Temp array that stores the current cards that are being worked with
            int[,] tempArray = new int[1, 2];

            //Variable that keeps track of how many cards were already checked
            int temp;

            //Loop that goes on until broken
            while (true)
            {
                //Sets the temp to 0
                temp = 0;

                //Loop for every community card plus one
                for (int i = 0; i < communityCardCount + 1; i++)
                {
                    //Sets the temp array index's to 0
                    tempArray[0, 0] = 0;
                    tempArray[0, 1] = 0;

                    //If the next card is smaller then the current card
                    if (currentCards[i + 1, 0] < currentCards[i, 0])
                    {
                        //Sets the temp array equal to the current card
                        tempArray[0, 0] = currentCards[i, 0];
                        tempArray[0, 1] = currentCards[i, 1];

                        //Updates the current card with the next card
                        currentCards[i, 0] = currentCards[i + 1, 0];
                        currentCards[i, 1] = currentCards[i + 1, 1];

                        //Updates the next card with the temp array cards
                        currentCards[i + 1, 0] = tempArray[0, 0];
                        currentCards[i + 1, 1] = tempArray[0, 1];
                    }
                }

                //Loop for every community card plus 2
                for (int i = 0; i < communityCardCount + 2; i++)
                {
                    //If i + 1 is smaller then the community card amount + 2
                    if (i + 1 < communityCardCount + 2)
                    {
                        //If the current card is smaller then or equal to the next card
                        if (currentCards[i, 0] <= currentCards[i + 1, 0])
                        {
                            //Add one to the temp
                            temp++;
                        }
                    }
                }

                //If the temp is equal to community card count plus one
                if (temp == communityCardCount + 1)
                {
                    //Break the while loop
                    break;
                }
            }

            //Return the current cards array
            return currentCards;
        }



        //Pre: The Player Number
        //Post: The cards that need to be checked are set
        //Desc: This subprogram updates the current cards array with the cards that need to be checked
        private int[,] SetCardValues(int playerNum)
        {
            //Updates the current cards array with the amount of cards that need to be checked
            int[,] currentCards = new int[communityCardCount + 2, 2];

            //First card for current player
            currentCards[0, 0] = playerCards[playerNum, 0, 0];
            currentCards[0, 1] = playerCards[playerNum, 0, 1];

            //Second card for current player
            currentCards[1, 0] = playerCards[playerNum, 1, 0];
            currentCards[1, 1] = playerCards[playerNum, 1, 1];

            //Sets values in the currentCards array for the community cards
            for (int i = 2; i < communityCardCount + 2; i++)
            {
                //Loop for which one of the two cards is being checked
                for (int j = 0; j < 2; j++)
                {
                    //Update the current card array
                    currentCards[i, j] = communityCards[i - 2, j];
                }
            }

            //Return the current cards array
            return currentCards;
        }



        //Pre: None
        //Post: The local IP is returned
        //Desc: This subprogram finds the local IP and returns it
        private string GetLocalIP()
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



        //Pre: The player number
        //Post: The hand quality of the current hand
        //Desc: This subprogram calculates the hand quality of the current hand
        private void CheckHandQuality(int playerNum)
        {
            //Variables for straight
            int straightCount = 0;
            bool straightGoing = false;
            int[,] straightStart = new int[1, 2];

            //Temporarily used variable
            int temp;

            //Variable for the amount of pairs
            int pairCount = 0;

            //Variable for if the hands was checked
            bool handChecked = false;

            //Variables for the hand kicker total and count
            int handKickerTotal = 0;
            int handKickerCount = 0;

            //Variables for which hands the current cards have possible
            int threeOfKindCount = 0;
            bool fourOfKind = false;
            bool twoPair = false;
            bool pair = false;
            bool fullHouse = false;

            //Boolean for if a straight flush is possible 
            bool straightFlushPossible = true;

            //Array for the current cards
            int[,] currentCards = new int[communityCardCount + 2, 2];

            //Array for the number of each suit
            int[] numSuits = new int[4];

            //Array for the number of each card
            int[] numCards = new int[13];

            //Give values to the current cards array and organize the array
            currentCards = SetCardValues(playerNum);
            currentCards = OrganizeCards(currentCards);

            //Checks for amount of cards for each suit
            for (int i = 0; i < communityCardCount + 2; i++)
            {
                //Switch statement for which suit the current card is and adds one to that suit
                switch (currentCards[i, 1])
                {
                    case 1:
                        numSuits[0]++;
                        break;
                    case 2:
                        numSuits[1]++;
                        break;
                    case 3:
                        numSuits[2]++;
                        break;
                    case 4:
                        numSuits[3]++;
                        break;
                }
            }

            //For loop for the amount of suits
            for (int i = 0; i < numSuits.Length; i++)
            {
                //If the amount of the current suit is equal to 4
                if (numSuits[i] == 4)
                {
                    //Set the flush draw to be true
                    flushDraw = true;
                }
            }

            //Checks for a straight from highest to lowest
            for (int i = communityCardCount + 1; i > 0; i--)
            {
                //If the straight count is less then 5
                if (straightCount < 5)
                {
                    //If there is an ace and a kind and a straight was not started
                    if (currentCards[0, 0] == 1 && i == currentCards.Length - 1 && currentCards[i, 0] == 13 && straightGoing == false)
                    {
                        //Updates that the straight started
                        i = currentCards.Length;
                        straightStart[0, 0] = 14;
                        straightStart[0, 1] = 0;
                        straightGoing = true;
                        straightCount += 2;
                    }
                    //If there is one card after another
                    else if (currentCards[i, 0] - 1 == currentCards[i - 1, 0] || currentCards[i - 1, 0] == currentCards[i, 0] && straightGoing == true)
                    {
                        //If the straight was not started
                        if (straightGoing == false)
                        {
                            //Updates that the straight started
                            straightStart[0, 0] = currentCards[i, 0];
                            straightStart[0, 1] = i;
                            straightGoing = true;
                            straightCount += 2;
                        }
                        //If the straight was started and the next card follows the straight
                        else if (currentCards[i - 1, 0] != currentCards[i, 0])
                        {
                            //Add to the straight count
                            straightCount++;
                        }
                    }
                    //If the straight is not followed
                    else
                    {
                        //Reset the straight
                        straightStart[0, 0] = 0;
                        straightStart[0, 1] = 0;
                        straightGoing = false;
                        straightCount = 0;
                    }
                }

                //If the straight count is equal to 5
                if (straightCount == 4)
                {
                    //If the straight started at an ace
                    if (straightStart[0, 0] == 14)
                    {
                        //inside straight draw is true
                        insideStraightDraw = true;
                    }
                    //If the straight started anywhere else
                    else
                    {
                        //Open ended straight draw is true
                        openEndedstraightDraw = true;
                    }
                }
                //If the straight count is equal to a 2 or a 3
                else if (straightCount == 2 || straightCount == 3)
                {
                    //If the i is greater then 2
                    if (i > 2)
                    {
                        //If an inside straight draw is occuring
                        if (currentCards[i - 2, 0] != currentCards[i, 0] - 2)
                        {
                            if (currentCards[i - 3, 0] == currentCards[i, 0] - 3)
                            {
                                if (straightCount == 2 && i - 3 != 0 || straightCount == 3)
                                {
                                    //Set inside straight draw to true
                                    insideStraightDraw = true;
                                }
                            }
                        }
                    }
                }
            }

            //Checks for the amount of same cards
            for (int i = 0; i < communityCardCount + 2; i++)
            {
                //Adds one to the current card
                numCards[currentCards[i, 0] - 1]++;
            }

            //Sets the temp and the amount of pairs to 0
            temp = 0;
            pairCount = 0;

            //Checks if there is a four of a kind, full house, trips, or pair
            for (int i = 0; i < communityCardCount + 2; i++)
            {
                //Checks for a two pair
                if (numCards[currentCards[i, 0] - 1] == 2 && currentCards[i, 0] != temp)
                {
                    //Adds to the amount of pairs
                    pairCount++;

                    //Updates the temp
                    temp = currentCards[i, 0];

                    //If the pair count is greater then or equal to 2
                    if (pairCount >= 2)
                    {
                        //Set two pair to true
                        twoPair = true;
                    }
                }

                //If there is 4 of one card
                if (numCards[currentCards[i, 0] - 1] == 4)
                {
                    //Set four of a kind to true   
                    fourOfKind = true;
                }
                //If there is 3 of one card
                else if (numCards[currentCards[i, 0] - 1] == 3)
                {
                    //Add to i
                    i += 2;

                    //Add to the three of a kind countk
                    threeOfKindCount++;
                }
                //If there is 2 of one card
                else if (numCards[currentCards[i, 0] - 1] == 2)
                {
                    //Set pair to true
                    pair = true;
                }

                //If there is one three of a kind and a pair or there is more then one three of a kind
                if (threeOfKindCount == 1 && pair == true || threeOfKindCount > 1)
                {
                    //Set full house to true
                    fullHouse = true;
                }
            }

            //Reset the temp and hand quality values
            temp = 0;
            handQuality[playerNum, 0] = 0;
            handQuality[playerNum, 1] = 0;
            handQuality[playerNum, 2] = 0;

            //Loop for while the hand was not checked
            while (handChecked == false)
            {
                //Checks for a royal flush
                if (numSuits[0] >= 5 && straightCount == 5 || numSuits[1] >= 5 && straightCount == 5 || numSuits[2] >= 5 && straightCount == 5 ||
                    numSuits[3] >= 5 && straightCount == 5 && straightFlushPossible == true)
                {
                    //If the straight started at an ace
                    if (straightStart[0, 0] == 14)
                    {
                        //Checks if every card of the straight matches with the flush to see if there is a royal flush
                        for (int i = currentCards.Length - 1; i > 2; i--)
                        {
                            if (currentCards[i, 1] == currentCards[straightStart[0, 1], 1])
                            {
                                temp++;

                                if (temp == 4)
                                {
                                    handQuality[playerNum, 0] = 10;
                                    handQuality[playerNum, 1] = 1;

                                    handChecked = true;
                                }
                            }
                        }
                    }
                    //Check for a straight flush
                    else
                    {
                        //Checks if every of the straight matches with the flush to see if there is a straight flush
                        for (int i = straightStart[0, 1] - 1; i > straightStart[0, 1] - 5; i--)
                        {
                            if (currentCards[i, 1] == currentCards[straightStart[0, 1], 1])
                            {
                                temp++;

                                if (temp == 4)
                                {
                                    handQuality[playerNum, 0] = 9;
                                    handQuality[playerNum, 1] = straightStart[0, 0];

                                    handChecked = true;
                                }
                            }
                        }
                    }

                    //If the hand was not checked 
                    if (handChecked == false)
                    {
                        //Set the straight flush to not possible
                        straightFlushPossible = false;
                    }
                }
                //Check for four of a kind
                else if (fourOfKind == true)
                {
                    //Loop for every card that is being checked
                    for (int i = 0; i < communityCardCount + 2; i++)
                    {
                        //If there is 4 of one card
                        if (numCards[currentCards[i, 0] - 1] == 4)
                        {
                            //If the card is an ace
                            if (currentCards[i, 0] == 1)
                            {
                                //Sets the hand quality
                                handQuality[playerNum, 0] = 8;
                                handQuality[playerNum, 1] = 14;
                            }
                            //If the card is not an ace
                            else
                            {
                                //Sets the hand quality
                                handQuality[playerNum, 0] = 8;
                                handQuality[playerNum, 1] = currentCards[i, 0];
                            }

                            //Sets hand checked to true
                            handChecked = true;
                        }
                        //Sets the kicker for the four of a kind
                        else if (numCards[currentCards[i, 0] - 1] == 1 && handQuality[playerNum, 2] < currentCards[i, 0])
                        {
                            //If the kicker is an ace
                            if (currentCards[i, 0] == 1)
                            {
                                //Sets the hand quality
                                handQuality[playerNum, 2] = 14;
                            }
                            //If the kicker is not an ace
                            else
                            {
                                //Sets the hand quality
                                handQuality[playerNum, 2] = currentCards[i, 0];
                            }

                            //Sets hand checked to true
                            handChecked = true;
                        }
                    }

                    temp = 0;
                }
                //Check for a full house
                else if (fullHouse == true)
                {
                    //Loop for every card that is being checked
                    for (int i = 0; i < communityCardCount + 2; i++)
                    {
                        if (numCards[currentCards[i, 0] - 1] == 3 && handQuality[playerNum, 1] < currentCards[i, 0])
                        {
                            if (currentCards[i, 0] == 1)
                            {
                                //Sets the hand quality
                                handQuality[playerNum, 0] = 7;
                                handQuality[playerNum, 1] = 14;
                            }
                            else
                            {
                                //Sets the hand quality
                                handQuality[playerNum, 0] = 7;
                                handQuality[playerNum, 1] = currentCards[i, 0];
                            }

                            //Sets hand checked to true
                            handChecked = true;
                        }
                    }

                    //Loop for every card that is being checked
                    for (int i = 0; i < communityCardCount + 2; i++)
                    {
                        if (numCards[currentCards[i, 0] - 1] >= 2 && handQuality[playerNum, 2] < currentCards[i, 0]
                            && currentCards[i, 0] != handQuality[playerNum, 1])
                        {
                            if (currentCards[i, 0] == 1 && handQuality[playerNum, 1] != 14)
                            {
                                if (currentCards[i, 0] == 1)
                                {
                                    //Sets the hand quality
                                    handQuality[playerNum, 2] = 14;
                                }
                                else
                                {
                                    //Sets the hand quality
                                    handQuality[playerNum, 2] = currentCards[i, 0];
                                }

                                //Sets hand checked to true
                                handChecked = true;
                            }
                        }
                    }
                }
                //Checks for a flush
                else if (numSuits[0] >= 5 || numSuits[1] >= 5 || numSuits[2] >= 5 || numSuits[3] >= 5)
                {
                    for (int i = 1; i < 5; i++)
                    {
                        if (numSuits[i - 1] >= 5)
                        {
                            if (playerCards[playerNum, 0, 1] == i && playerCards[playerNum, 0, 0] == 1 || playerCards[playerNum, 1, 1] == i &&
                                playerCards[playerNum, 1, 0] == 1)
                            {
                                //Sets the hand quality
                                handQuality[playerNum, 0] = 6;
                                handQuality[playerNum, 1] = 14;

                                //Sets hand checked to true
                                handChecked = true;
                            }
                            else if (playerCards[playerNum, 0, 1] == i && playerCards[playerNum, 1, 0] < playerCards[playerNum, 0, 0])
                            {
                                //Sets the hand quality
                                handQuality[playerNum, 0] = 6;
                                handQuality[playerNum, 1] = playerCards[playerNum, 0, 0];

                                //Sets hand checked to true
                                handChecked = true;
                            }
                            else if (playerCards[playerNum, 1, 1] == i && playerCards[playerNum, 1, 0] > playerCards[playerNum, 0, 0])
                            {
                                //Sets the hand quality
                                handQuality[playerNum, 0] = 6;
                                handQuality[playerNum, 1] = playerCards[playerNum, 1, 0];

                                //Sets hand checked to true
                                handChecked = true;
                            }
                        }
                    }
                }
                //Checks for a straight 
                else if (straightCount == 5)
                {
                    //Sets the hand quality
                    handQuality[playerNum, 0] = 5;
                    handQuality[playerNum, 1] = straightStart[0, 0];

                    //Sets hand checked to true
                    handChecked = true;
                }
                //Checks for a three of a kind
                else if (threeOfKindCount == 1)
                {
                    handKickerCount = 0;
                    handKickerTotal = 0;

                    //Loop for every card that is being checked
                    for (int i = 0; i < communityCardCount + 2; i++)
                    {
                        if (numCards[currentCards[i, 0] - 1] == 3 && handQuality[playerNum, 1] < currentCards[i, 0])
                        {
                            if (currentCards[i, 0] == 1)
                            {
                                //Sets the hand quality
                                handQuality[playerNum, 0] = 4;
                                handQuality[playerNum, 1] = 14;
                            }
                            else
                            {
                                //Sets the hand quality
                                handQuality[playerNum, 0] = 4;
                                handQuality[playerNum, 1] = currentCards[i, 0];
                            }

                            //Sets hand checked to true
                            handChecked = true;
                        }
                        else if (numCards[currentCards[i, 0] - 1] == 1 && handKickerCount < 2)
                        {
                            if (currentCards[i, 0] == 1)
                            {
                                //Sets the hand kicker
                                handKickerTotal += 14;
                                handKickerCount++;
                            }
                            else
                            {
                                //Sets the hand kicker
                                handKickerTotal += currentCards[i, 0];
                                handKickerCount++;
                            }

                            //Sets hand checked to true
                            handChecked = true;
                        }
                    }

                    //Sets the hand quality
                    handQuality[playerNum, 2] = handKickerTotal;
                }
                //Check for a two pair
                else if (twoPair == true)
                {
                    //Loop for every card that is being checked
                    for (int i = 0; i < communityCardCount + 2; i++)
                    {
                        if (numCards[currentCards[i, 0] - 1] == 2 && handQuality[playerNum, 1] < currentCards[i, 0])
                        {
                            if (currentCards[i, 0] == 1)
                            {
                                //Sets the hand quality
                                handQuality[playerNum, 0] = 3;
                                handQuality[playerNum, 1] = 14;
                            }
                            else
                            {
                                //Sets the hand quality
                                handQuality[playerNum, 0] = 3;
                                handQuality[playerNum, 1] = currentCards[i, 0];
                            }

                            //Sets hand checked to true
                            handChecked = true;
                        }
                    }

                    for (int i = 0; i < communityCardCount + 2; i++)
                    {
                        if (numCards[currentCards[i, 0] - 1] == 2 && handQuality[playerNum, 2] < currentCards[i, 0])
                        {
                            if (currentCards[i, 0] != handQuality[playerNum, 1] && currentCards[i, 0] != 1)
                            {
                                //Sets the hand quality
                                handQuality[playerNum, 2] = currentCards[i, 0];
                            }
                        }
                    }
                }
                //Check for pair
                else if (pair == true)
                {
                    handKickerTotal = 0;
                    handKickerCount = 0;

                    //Loop for every card that is being checked
                    for (int i = 0; i < communityCardCount + 2; i++)
                    {
                        if (numCards[currentCards[i, 0] - 1] == 2 && handQuality[playerNum, 1] < currentCards[i, 0])
                        {
                            if (currentCards[i, 0] == 1)
                            {
                                //Sets the hand quality
                                handQuality[playerNum, 0] = 2;
                                handQuality[playerNum, 1] = 14;
                            }
                            else
                            {
                                //Sets the hand quality
                                handQuality[playerNum, 0] = 2;
                                handQuality[playerNum, 1] = currentCards[i, 0];
                            }

                            //Sets hand checked to true
                            handChecked = true;
                        }
                        else if (numCards[currentCards[i, 0] - 1] == 1 && handKickerCount < 3)
                        {
                            if (currentCards[i, 0] == 1)
                            {
                                //Sets the hand kicker
                                handKickerTotal += 14;
                                handKickerCount++;
                            }
                            else
                            {
                                //Sets the hand kicker
                                handKickerTotal += currentCards[i, 0];
                                handKickerCount++;
                            }

                            //Sets hand checked to true
                            handChecked = true;
                        }
                    }

                    //Sets the hand quality
                    handQuality[playerNum, 2] = handKickerTotal;
                }
                //Check for high card
                else
                {
                    temp = 0;

                    //Loop for every card that is being checked
                    for (int i = 0; i < communityCardCount + 2; i++)
                    {
                        if (currentCards[i, 0] == 1)
                        {
                            temp += 14;
                        }
                        else
                        {
                            temp += currentCards[i, 0];
                        }
                    }

                    //Sets hand checked to true
                    handChecked = true;

                    //Sets the hand quality
                    handQuality[playerNum, 0] = 1;
                    handQuality[playerNum, 1] = temp;
                }

                //If the hand quality value is equal to 3
                if (handQuality[playerNum, 0] == 3)
                {
                    //Loop for every card that is being checked
                    for (int i = communityCardCount + 1; i >= 0; i--)
                    {
                        if (handKicker[playerNum] < currentCards[i, 0] && numCards[currentCards[i, 0] - 1] == 1 && currentCards[i, 0] != 1 &&
                                handKicker[playerNum] != 1)
                        {
                            handKicker[playerNum] = currentCards[i, 0];
                        }
                        else if (handKicker[playerNum] == 14 || currentCards[i, 0] == 1 && numCards[currentCards[i, 0] - 1] == 1)
                        {
                            handKicker[playerNum] = 14;
                        }
                    }
                }

                //If the community card count is equal to 0
                if (communityCardCount == 0)
                {
                    //Sets hand checked to true
                    handChecked = true;
                }
            }
        }
    }
}

