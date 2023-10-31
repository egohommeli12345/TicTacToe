using System.Windows.Input;

namespace TicTacToe
{
    public partial class AppShell : Shell
    {
        public ICommand ShowStatsCommand => new Command(async () =>
        {
            await Shell.Current.GoToAsync(nameof(Stats));
        });

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Stats), typeof(Stats));
            BindingContext = this;
        }
    }
}