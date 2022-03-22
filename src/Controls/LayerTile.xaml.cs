using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using DQB2ChunkEditor.Models;

namespace DQB2ChunkEditor.Controls;

public partial class LayerTile : UserControl
{
    public ushort Id { get; set; }
    public ObservableProperty<Tile> Tile { get; set; } = new();

    public LayerTile()
    {
        InitializeComponent();
        DataContext = this;
    }
}
