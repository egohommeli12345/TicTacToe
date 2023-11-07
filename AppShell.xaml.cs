using System.Text.Json;
using System.Windows.Input;

namespace TicTacToe
{
    public partial class AppShell : Shell
    {
        public ICommand OpenStatsCommand => new Command(async () =>
        {
            await Shell.Current.GoToAsync(nameof(Stats));
        });

        public ICommand ExitCommand => new Command(Application.Current.Quit);

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Stats), typeof(Stats));
            BindingContext = this;
        }
    }
}