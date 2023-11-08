using System.Collections.ObjectModel;
using System.Text.Json;

namespace TicTacToe
{
    public partial class MainPage : ContentPage
    {
        // Strruct for the player information
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

        private async void PlayVS(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(PlayPage));
        }
        
        // Filepath for the player.json file
        private string GetFilePath()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(path, "player.json");
            return filePath;
        }

        // Adds a new player to the player.json file
        private void AddPlayer(object sender, EventArgs e)
        {
            Player newPlayer = new Player();
            Button button = (Button)sender;

            // Checks if the user has filled in all the fields
            // Creates a new player with the user input information
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

            // Used for clearing the entry fields after user clicking "Save new player"
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

            // Adds the new player to the JSON file
            void AddPlayerToFile(Player newPlayer, List<Player> playerCollection)
            {
                // Add the new player to the collection
                playerCollection.Add(newPlayer);

                // Serialize the collection back to JSON
                var updatedJson = JsonSerializer.Serialize(playerCollection, new JsonSerializerOptions { WriteIndented = true });

                // Write the updated JSON back to the file
                File.WriteAllText(GetFilePath(), updatedJson);
            }

            // Check if the file already exists
            if (File.Exists(GetFilePath()))
            {
                // Read the existing file
                var json = File.ReadAllText(GetFilePath());

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
                        AddPlayerToFile(newPlayer, playerCollection);
                    }
                }
                else
                {
                    AddPlayerToFile(newPlayer, playerCollection);
                }

                ClearEntries();
                UpdateList();
            }

            // If the file doesn't exist, create it
            else
            {
                using (FileStream fs = File.Create(GetFilePath()))
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
                File.WriteAllText(GetFilePath(), updatedJson);

                ClearEntries();
                UpdateList();
            }
        }

        // Deletes the player.json file, thus clearing all stats
        private void ClearStats(object sender, EventArgs e)
        {
            if (File.Exists(GetFilePath()))
            {
                File.Delete(GetFilePath());
            }
            UpdateList();
        }

        // "Choose player" option for the dropdown lists
        // AI opponent option for the dropdown lists
        public readonly Player DefaultPlayer = new Player { Firstname = "Choose player", Surname = "" };
        public readonly Player AiOpponent = new Player { Firstname = "_AI", Surname = "Opponent_", YearOfBirth = 6969 };

        // Updates the dropdown lists with the players from the JSON file
        private void UpdateList()
        {
            // Creating the file if it doesn't exist
            if (File.Exists(GetFilePath()) == false)
            {
                using (FileStream fs = File.Create(GetFilePath()))
                {

                }

                // Adding the AI opponent to the JSON file
                var playerCollection2 = new List<Player> { AiOpponent };
                var updatedJson = JsonSerializer.Serialize(playerCollection2, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(GetFilePath(), updatedJson);
            }
            // Reading, deserializing and adding the players to the dropdown lists
            var json = File.ReadAllText(GetFilePath());
            var items = new List<Player>();
            if (json != "")
            {
                items = JsonSerializer.Deserialize<List<Player>>(json);
            }
            var playerCollection = new ObservableCollection<Player> { DefaultPlayer };
            foreach (var item in items)
            {
                playerCollection.Add(item);
            }
            Player1Picker.ItemsSource = playerCollection;
            Player2Picker.ItemsSource = playerCollection;
            Player1Picker.SelectedIndex = 0;
            Player2Picker.SelectedIndex = 0;
        }

        // Public variables for the selected players
        public static bool IsPlayer1Ai = false;
        public static bool IsPlayer2Ai = false;
        public static string player1;
        public static string player2;

        // Disabling "Play" button until user has chosen two players
        // Not allowing the user to choose the same player twice
        // Changes the player variables according to the players user chose
        void OnPlayer1PickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if ((picker.SelectedIndex != 0 && Player2Picker.SelectedIndex != 0) &&
                (picker.SelectedIndex != -1 && Player2Picker.SelectedIndex != -1))
            {
                if ((picker.SelectedIndex == Player2Picker.SelectedIndex))
                {
                    DisplayAlert("Error", "Please choose two different players", "OK");
                    picker.SelectedIndex = 0;
                    return;
                }
                PlayVSButton.IsEnabled = true;
            }
            else
            {
                PlayVSButton.IsEnabled = false;
            }
            if (picker.SelectedIndex == 1)
            {
                IsPlayer1Ai = true;
                player1 = "_AI Opponent_";
            }
            else
            {
                IsPlayer1Ai = false;
                try
                {
                    player1 = picker.SelectedItem.ToString();
                }
                catch (NullReferenceException)
                {
                    player1 = "";
                }
            }
        }

        void OnPlayer2PickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if ((picker.SelectedIndex != 0 && Player1Picker.SelectedIndex != 0) &&
                (picker.SelectedIndex != -1 && Player1Picker.SelectedIndex != -1))
            {
                if (picker.SelectedIndex == Player1Picker.SelectedIndex)
                {
                    DisplayAlert("Error", "Please choose two different players", "OK");
                    picker.SelectedIndex = 0;
                    return;
                }
                PlayVSButton.IsEnabled = true;
            }
            else
            {
                PlayVSButton.IsEnabled = false;
            }
            if (picker.SelectedIndex == 1)
            {
                IsPlayer2Ai = true;
                player2 = "_AI Opponent_";
            }
            else
            {
                IsPlayer2Ai = false;
                try
                {
                    player2 = picker.SelectedItem.ToString();
                }
                catch (NullReferenceException)
                {
                    player2 = "";
                }
            }
        }
    }
}