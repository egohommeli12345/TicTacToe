namespace TicTacToe
{
    public partial class MainPage : ContentPage
    {
        public struct Player
        {
            public string Firstname { get; set; }
            public string Surname { get; set; }
            public int YearOfBirth { get; set; }
            public int Wins { get; set; }
            public int Losses { get; set; }
            public int Draws { get; set; }
        }

        public MainPage()
        {
            InitializeComponent();
        }

    }
}