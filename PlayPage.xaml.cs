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
    }

    bool gameStarted = false;
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (gameStarted == false)
        {
            await AiStart();
            gameStarted = true;
        }
    }

    // Creating variables used to track the game progress
    int turncounter = 0;
    int[] board = new int[9];

    public async Task AiStart()
    {
        if (MainPage.IsPlayer1Ai == true)
        {
            await AiOpponent();
        }
    }

    // Function which tracks turns and checks if a player has won
    private async void OnButtonClicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;

        if (button.Text != "")
        {
            await DisplayAlert("Alert", "Choose another square", "OK");
        }
        else
        {
            if (turncounter % 2 == 0)
            {
                button.Text = "X";
                whosturn.Text = $"{MainPage.player2}'s turn";
                board[ButtonLocation(button)] = 1;
                turncounter++;
                if (MainPage.IsPlayer2Ai == true && CheckIfWon(board) == false)
                {
                    await AiOpponent();
                }
            }
            else
            {
                button.Text = "O";
                whosturn.Text = $"{MainPage.player1}'s turn";
                board[ButtonLocation(button)] = 2;
                turncounter++;
                if (MainPage.IsPlayer1Ai == true && CheckIfWon(board) == false)
                {
                    await AiOpponent();
                }
            }
        }

        // Check if a player has won
        if (CheckIfWon(board) == true)
        {
            if (turncounter % 2 == 0)
            {
                UpdateStats(MainPage.player1, 0, 0, 1);
                UpdateStats(MainPage.player2, 1, 0, 0);
                await DisplayAlert("Alert", $"{MainPage.player2} won!", "Return to menu");
            }
            else
            {
                UpdateStats(MainPage.player1, 1, 0, 0);
                UpdateStats(MainPage.player2, 0, 0, 1);
                await DisplayAlert("Alert", $"{MainPage.player1} won!", "Return to menu");
            }
            ResetGame();
            ReturnToMenu();
        }
        else if (turncounter == 9)
        {
            await DisplayAlert("Alert", "It's a tie!", "Return to menu");
            UpdateStats(MainPage.player1, 0, 1, 0);
            UpdateStats(MainPage.player2, 0, 1, 0);
            ResetGame();
            ReturnToMenu();
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

    // Resets the game
    private void ResetGame()
    {
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

    private void UpdateStats(string winner, int win, int draw, int lose)
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
            playerCollection[playerIndex] = playerToUpdate; // Set the updated player back into the list

            // Serialize and write the updated list back to the JSON file
            string updatedJsonString = JsonSerializer.Serialize(playerCollection, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, updatedJsonString);
        }
    }

    private void ReturnToMenu()
    {
        Shell.Current.GoToAsync(nameof(MainPage));
    }

    // Random delay function for AI opponent. Makes the game more realistic.
    private static async Task RandomDelay()
    {
        Random rnd = new Random();
        int delay = rnd.Next(500, 2001); // Random delay between 500ms and 2000ms
        await Task.Delay(delay);
    }
}