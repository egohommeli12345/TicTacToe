using System.Text.Json; 
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

            public Player(string firstname, string surname, int yearOfBirth, int wins, int losses, int draws)
            {
                Firstname = firstname;
                Surname = surname;
                YearOfBirth = yearOfBirth;
                Wins = wins;
                Losses = losses;
                Draws = draws;
            }
        }

        public MainPage()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(PlayPage), typeof(PlayPage));
            BindingContext = this;
        }

        private void PlayVS(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync(nameof(PlayPage));
        }

        private void SavePlayerInfo(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string filepath = Path.Combine(path, "player.json");

            Player player1 = new Player();
            player1.Firstname = Player1Firstname.Text;
            player1.Surname = Player1Surname.Text;
            player1.YearOfBirth = int.Parse(Player1Year.Text);
            /*player1.Wins = 0;
            player1.Losses = 0;
            player1.Draws = 0;*/
            string json = JsonSerializer.Serialize(player1);
            File.WriteAllText(filepath, json);
            
            Player player2 = new Player();
            player2.Firstname = Player2Firstname.Text;
            player2.Surname = Player1Surname.Text;
            player2.YearOfBirth = int.Parse(Player2Year.Text);
            /*player2.Wins = 0;
            player2.Losses = 0;
            player2.Draws = 0;*/
            json = JsonSerializer.Serialize(player2);
            File.WriteAllText(filepath, json);
        }
    }
}