using System.Windows.Input;

namespace TicTacToe
{
    public partial class AppShell : Shell
    {
        public ICommand NavigateToSettingsCommand => new Command(async () =>
        {
            await Shell.Current.GoToAsync(nameof(SettingsPage));
        });

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            BindingContext = this;
        }
    }
}