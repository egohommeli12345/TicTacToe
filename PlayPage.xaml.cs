using System.Diagnostics.Metrics;
using System.Security.Cryptography;

namespace TicTacToe;

public partial class PlayPage : ContentPage
{
    public PlayPage()
    {
        InitializeComponent();
    }

    // Creating variables used to track the game progress
    int turncounter = 0;
    int[] board = new int[9];

    // Function which tracks turns and checks if a player has won
    private void OnButtonClicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;

        if (button.Text != "")
        {
            DisplayAlert("Alert", "Choose another square", "OK");
        }
        else
        {
            if (turncounter % 2 == 0)
            {
                button.Text = "X";
                whosturn.Text = "Player 2's turn";
                board[ButtonLocation(button)] = 1;
            }
            else
            {
                button.Text = "O";
                whosturn.Text = "Player 1's turn";
                board[ButtonLocation(button)] = 2;
            }
        }
        turncounter++;

        // Check if a player has won
        if (CheckIfWon(board) == true)
        {
            if (turncounter % 2 == 0)
            {
                DisplayAlert("Alert", "Player 2 won!", "OK");
            }
            else
            {
                DisplayAlert("Alert", "Player 1 won!", "OK");
            }
            ResetGame();
        }
        else if (turncounter == 9)
        {
            DisplayAlert("Alert", "It's a tie!", "OK");
            ResetGame();
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
}