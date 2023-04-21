using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace EveSquadron.Views;

public partial class PlayerStatsView : UserControl
{
    public PlayerStatsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}