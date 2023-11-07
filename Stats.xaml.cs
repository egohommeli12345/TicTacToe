using System.Collections.ObjectModel;
using System.Text.Json;

namespace TicTacToe;

public partial class Stats : ContentPage
{
	public Stats()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ReadJSON();
    }

    private void ReadJSON()
	{
        string path = AppDomain.CurrentDomain.BaseDirectory;
        string filePath = Path.Combine(path, "player.json");
        // Deserialize your JSON here and navigate to the Stats
        string jsonString = File.ReadAllText(filePath);
        List<MainPage.Player> playersList = JsonSerializer.Deserialize<List<MainPage.Player>>(jsonString);

        ObservableCollection<MainPage.Player> players = new ObservableCollection<MainPage.Player>(playersList);
        playersListView.ItemsSource = players;
    }

    private void Back_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(MainPage));
    }
}