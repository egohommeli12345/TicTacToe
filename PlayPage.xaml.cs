using System.Diagnostics.Metrics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TicTacToe;

public partial class PlayPage : ContentPage
{
    public PlayPage()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        _player1Timer = new Timer(TimerCallback, 1, Timeout.Infinite, 1000);
        _player2Timer = new Timer(TimerCallback, 2, Timeout.Infinite, 1000);
    }

    // Creating variables used to track the game progress
    int turncounter = 0;
    int[] board = new int[9];
    bool gameStarted = false;
    bool gameEnded = false;

    private Timer _player1Timer;
    private Timer _player2Timer;
    private int _player1SecondsElapsed;
    private int _player2SecondsElapsed;

    // If the player is AI, it will start the game
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!gameStarted && !gameEnded)
        {
            whosturn.Text = $"{MainPage.player1}'s turn";
            StartPlayerTimer(1);
            await AiStart();
            gameStarted = true;
        }
    }

    // On exit, dispose of the timers
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _player1Timer?.Dispose();
        _player2Timer?.Dispose();
    }

    // If player1 is AI, it will start the game
    public async Task AiStart()
    {
        if (MainPage.IsPlayer1Ai == true)
        {
            await AiOpponent();
        }
    }

    // Marks the button with an X or O depending on whose turn it is
    private async void OnButtonClicked(object sender, EventArgs e)
    {
        if (gameEnded) return;

        Button button = (Button)sender;

        if (button.Text != "")
        {
            await DisplayAlert("Alert", "Choose another square", "OK");
        }
        else
        {
            if (turncounter % 2 == 0)
            {
                StopPlayerTimer(1);
                StartPlayerTimer(2);
                button.Text = "X";
                whosturn.Text = $"{MainPage.player2}'s turn";
                board[ButtonLocation(button)] = 1;
                turncounter++;
                if (MainPage.IsPlayer2Ai == true && CheckIfWon(board) == false && turncounter < 9)
                {
                    await AiOpponent();
                }
            }
            else
            {
                StopPlayerTimer(2);
                StartPlayerTimer(1);
                button.Text = "O";
                whosturn.Text = $"{MainPage.player1}'s turn";
                board[ButtonLocation(button)] = 2;
                turncounter++;
                if (MainPage.IsPlayer1Ai == true && CheckIfWon(board) == false && turncounter < 9)
                {
                    await AiOpponent();
                }
            }
        }

        // Check if a player has won
        if (CheckIfWon(board) == true)
        {
            StopPlayerTimer(1);
            StopPlayerTimer(2);
            gameEnded = true;
            if (turncounter % 2 == 0)
            {
                await DisplayAlert("Alert", $"{MainPage.player2} won!", "Return to menu");
                UpdateStats(MainPage.player1, 0, 0, 1, _player1SecondsElapsed);
                UpdateStats(MainPage.player2, 1, 0, 0, _player2SecondsElapsed);
            }
            else
            {
                await DisplayAlert("Alert", $"{MainPage.player1} won!", "Return to menu");
                UpdateStats(MainPage.player1, 1, 0, 0, _player1SecondsElapsed);
                UpdateStats(MainPage.player2, 0, 0, 1, _player2SecondsElapsed);
            }
            ResetGame();
            await ReturnToMenu();
        }
        else if (turncounter == 9 && CheckIfWon(board) == false)
        {
            gameEnded = true;
            await DisplayAlert("Alert", "It's a tie!", "Return to menu");
            UpdateStats(MainPage.player1, 0, 1, 0, _player1SecondsElapsed);
            UpdateStats(MainPage.player2, 0, 1, 0, _player2SecondsElapsed);
            ResetGame();
            await ReturnToMenu();
        }
    }

    // Checks if the array has a winning combination returning true if it does
    private bool CheckIfWon(int[] board)
    {
        // Check rows
        for (int i = 0; i <= 6; i += 3)
        {
            if (board[i] != 0 && board[i] == board[i + 1] && board[i] == board[i + 2])
                return true;
        }

        // Check columns
        for (int i = 0; i < 3; i++)
        {
            if (board[i] != 0 && board[i] == board[i + 3] && board[i] == board[i + 6])
                return true;
        }

        // Check diagonals
        if (board[0] != 0 && board[0] == board[4] && board[0] == board[8])
            return true;

        if (board[2] != 0 && board[2] == board[4] && board[2] == board[6])
            return true;

        // No winner found
        return false;
    }

    // Resets the game; variables and buttons to starting values
    private void ResetGame()
    {
        _player1SecondsElapsed = 0;
        _player2SecondsElapsed = 0;
        turncounter = 0;
        board = new int[9];
        string buttonId = "button";
        for (int i = 0; i < 9; i++)
        {
            buttonId = buttonId + i.ToString();
            Button button = (Button)FindByName(buttonId);
            button.Text = "";
            buttonId = "button";
        }
    }

    // Returns the location of the button as an integer
    private int ButtonLocation(Button button)
    {
        string elementId = button.ClassId;
        elementId = elementId.Replace("button", "");
        int buttonlocation = int.Parse(elementId);
        return buttonlocation;
    }

    // AI opponent, which is not quite AI. It randomly chooses a square to place an O. May or may not win...
    // Simulates button clicks
    private async Task AiOpponent()
    {
        await RandomDelay();
        Random random = new Random();
        int rnd = random.Next(0, 9);
        while (board[rnd] != 0)
        {
            rnd = random.Next(0, 9);
        }
        string buttonId = "button" + rnd.ToString();
        Button button = (Button)FindByName(buttonId);
        OnButtonClicked(button, new EventArgs());
    }

    // Updates the player stats in the JSON file
    private void UpdateStats(string winner, int win, int draw, int lose, int playTime)
    {
        string[] names = winner.Split(' ');

        string firstName = names[0];
        string lastName = names.Length > 1 ? names[1] : string.Empty;

        string path = AppDomain.CurrentDomain.BaseDirectory;
        string filePath = Path.Combine(path, "player.json");

        string jsonString = File.ReadAllText(filePath);
        List<MainPage.Player> playerCollection = JsonSerializer.Deserialize<List<MainPage.Player>>(jsonString);

        // Find the index of the player in the list
        int playerIndex = playerCollection.FindIndex(p => p.Firstname == firstName && p.Surname == lastName);

        // Check if the player was found
        if (playerIndex != -1)
        {
            // Retrieve the player, modify it, and then set it back into the list
            MainPage.Player playerToUpdate = playerCollection[playerIndex];
            playerToUpdate.Wins += win;
            playerToUpdate.Draws += draw;
            playerToUpdate.Losses += lose;
            playerToUpdate.Playtime += playTime;
            playerCollection[playerIndex] = playerToUpdate; // Set the updated player back into the list

            // Serialize and write the updated list back to the JSON file
            string updatedJsonString = JsonSerializer.Serialize(playerCollection, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, updatedJsonString);
        }
    }

    private async Task ReturnToMenu()
    {
        await Shell.Current.GoToAsync(nameof(MainPage));
    }

    // Random delay function for AI opponent. Makes the game more realistic.
    private static async Task RandomDelay()
    {
        Random rnd = new Random();
        int delay = rnd.Next(500, 2001); // Random delay between 500ms and 2000ms
        await Task.Delay(delay);
    }

    // Timer functions
    private void StartPlayerTimer(int playerNumber)
    {
        if (playerNumber == 1)
        {
            _player1Timer.Change(0, 1000);
        }
        else
        {
            _player2Timer.Change(0, 1000);
        }
    }

    private void StopPlayerTimer(int playerNumber)
    {
        if (playerNumber == 1)
        {
            _player1Timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
        else
        {
            _player2Timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }

    private void TimerCallback(object state)
    {
        int playerNumber = (int)state;
        if (playerNumber == 1)
        {
            _player1SecondsElapsed++;
        }
        else
        {
            _player2SecondsElapsed++;
        }
    }
}