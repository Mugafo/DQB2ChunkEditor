using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using DQB2ChunkEditor.Models;

namespace DQB2ChunkEditor.Controls;

public partial class ComboBoxTile : UserControl
{
    public int Id { get; set; } = 0;
    public Tile Tile { get; set; } = new();

    public ComboBoxTile()
    {
        InitializeComponent();
        DataContext = this;
    }
}
