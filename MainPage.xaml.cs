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

        private void PlayAI(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync(nameof(PlayPage));
        }

        private void AddPlayer(object sender, EventArgs e)
        {
            Player newPlayer = new Player();
            Button button = (Button)sender;
            switch (button.ClassId)
            {
                case "player1add":
                    newPlayer = new Player(Player1Firstname.Text, Player1Surname.Text, int.Parse(Player1Year.Text), 0, 0, 0);
                    break;
                case "player2add":
                    newPlayer = new Player(Player2Firstname.Text, Player2Surname.Text, int.Parse(Player2Year.Text), 0, 0, 0);
                    break;
            }

            string path = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(path, "player.json");

            if (File.Exists(filePath))
            {
                // Read the existing file
                var json = File.ReadAllText(filePath);

                var playerCollection = new List<Player>();

                if (json != "")
                {
                    // Deserialize the JSON into a collection
                    playerCollection = JsonSerializer.Deserialize<List<Player>>(json) ?? new List<Player>();
                }

                // Add the new player to the collection
                playerCollection.Add(newPlayer);

                // Serialize the collection back to JSON
                var updatedJson = JsonSerializer.Serialize(playerCollection, new JsonSerializerOptions { WriteIndented = true });

                // Write the updated JSON back to the file
                File.WriteAllText(filePath, updatedJson);
            }
            else
            {
                using (FileStream fs = File.Create(filePath))
                {

                }

                // Read the existing file
                var json = File.ReadAllText(filePath);

                var playerCollection = new List<Player>();

                if (json != "")
                {
                    // Deserialize the JSON into a collection
                    playerCollection = JsonSerializer.Deserialize<List<Player>>(json) ?? new List<Player>();
                }

                // Add the new player to the collection
                playerCollection.Add(newPlayer);

                // Serialize the collection back to JSON
                var updatedJson = JsonSerializer.Serialize(playerCollection, new JsonSerializerOptions { WriteIndented = true });

                // Write the updated JSON back to the file
                File.WriteAllText(filePath, updatedJson);
            }
        }

        private void ClearStats(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(path, "player.json");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}