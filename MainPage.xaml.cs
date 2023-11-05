using System.Collections.ObjectModel;
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

            public override string ToString()
            {
                return $"{Firstname} {Surname}";
            }
        }

        public MainPage()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(PlayPage), typeof(PlayPage));
            BindingContext = this;
            UpdateList();
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
                    if (string.IsNullOrEmpty(Player1Firstname.Text) || string.IsNullOrEmpty(Player1Surname.Text) || string.IsNullOrEmpty(Player1Year.Text))
                    {
                        DisplayAlert("Error", "Please fill in all fields", "OK");
                        return;
                    }
                    else if (int.TryParse(Player1Year.Text, out int result) == false)
                    {
                        DisplayAlert("Error", "Please enter a valid year", "OK");
                        return;
                    }
                    newPlayer = new Player(Player1Firstname.Text, Player1Surname.Text, int.Parse(Player1Year.Text), 0, 0, 0);
                    break;
                case "player2add":
                    if (string.IsNullOrEmpty(Player2Firstname.Text) || string.IsNullOrEmpty(Player2Surname.Text) || string.IsNullOrEmpty(Player2Year.Text))
                    {
                        DisplayAlert("Error", "Please fill in all fields", "OK");
                        return;
                    }
                    else if (int.TryParse(Player2Year.Text, out int result2) == false)
                    {
                        DisplayAlert("Error", "Please enter a valid year", "OK");
                        return;
                    }
                    newPlayer = new Player(Player2Firstname.Text, Player2Surname.Text, int.Parse(Player2Year.Text), 0, 0, 0);
                    break;
            }

            string path = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(path, "player.json");

            void ClearEntries()
            {
                switch (button.ClassId)
                {
                    case "player1add":
                        Player1Firstname.Text = "";
                        Player1Surname.Text = "";
                        Player1Year.Text = "";
                        break;
                    case "player2add":
                        Player2Firstname.Text = "";
                        Player2Surname.Text = "";
                        Player2Year.Text = "";
                        break;
                }
            }

            void AddPlayerToFile(Player newPlayer, List<Player> playerCollection, string filePath)
            {
                // Add the new player to the collection
                playerCollection.Add(newPlayer);

                // Serialize the collection back to JSON
                var updatedJson = JsonSerializer.Serialize(playerCollection, new JsonSerializerOptions { WriteIndented = true });

                // Write the updated JSON back to the file
                File.WriteAllText(filePath, updatedJson);
            }

            // Check if the file already exists
            if (File.Exists(filePath))
            {
                // Read the existing file
                var json = File.ReadAllText(filePath);

                var playerCollection = new List<Player>();

                if (json != "")
                {
                    // Deserialize the JSON into a collection
                    playerCollection = JsonSerializer.Deserialize<List<Player>>(json) ?? new List<Player>();

                    var index = playerCollection.FindIndex(p => p.Firstname == newPlayer.Firstname && p.Surname == newPlayer.Surname);
                    if (index != -1)
                    {
                        DisplayAlert("Error", "Player already exists", "OK");
                    }
                    else
                    {
                        AddPlayerToFile(newPlayer, playerCollection, filePath);
                    }
                }
                else
                {
                    AddPlayerToFile(newPlayer, playerCollection, filePath);
                }

                ClearEntries();
                UpdateList();
            }

            // If the file doesn't exist, create it
            else
            {
                using (FileStream fs = File.Create(filePath))
                {

                }

                var playerCollection = new List<Player>
                {
                    // Add the new player to the collection
                    newPlayer
                };

                // Serialize the collection back to JSON
                var updatedJson = JsonSerializer.Serialize(playerCollection, new JsonSerializerOptions { WriteIndented = true });

                // Write the updated JSON back to the file
                File.WriteAllText(filePath, updatedJson);

                ClearEntries();
                UpdateList();
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

        public readonly Player DefaultPlayer = new Player { Firstname = "Choose player", Surname = "" };

        private void UpdateList()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(path, "player.json");
            var json = File.ReadAllText(filePath);
            var items = JsonSerializer.Deserialize<List<Player>>(json);
            var playerCollection = new ObservableCollection<Player> { DefaultPlayer };
            foreach (var item in items)
            {
                playerCollection.Add(item);
            }
            Player1Picker.ItemsSource = playerCollection;
            Player2Picker.ItemsSource = playerCollection;
        }

        void OnPlayer1PickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if (picker.SelectedIndex == 0)
            {
                picker.SelectedIndex = -1;  // Deselect the item
            }
        }

        void OnPlayer2PickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if (picker.SelectedIndex == 0)
            {
                picker.SelectedIndex = -1;  // Deselect the item
            }
        }
    }
}