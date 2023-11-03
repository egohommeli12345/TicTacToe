using System.Diagnostics.Metrics;
using System.Security.Cryptography;

namespace TicTacToe;

public partial class PlayPage : ContentPage
{
    public PlayPage()
    {
        InitializeComponent();
    }


    int turncounter = 0;
    int[] board = new int[9];

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
            }
            else
            {
                button.Text = "O";
                whosturn.Text = "Player 1's turn";
            }
        }
        turncounter++;
        CheckIfWon();
    }

    private void CheckIfWon()
    {

    }

    private void ResetGame()
    {
        turncounter = 0;
        board = new int[9];
    }

    private int ButtonLocation(Button button)
    {
        string elementName = AutomationProperties.GetName(button);
        elementName = elementName.Replace("button", "");
        int buttonlocation = int.Parse(elementName);
        return buttonlocation;
    }
}