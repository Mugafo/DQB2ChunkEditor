using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DQB2ChunkEditor.Controls;
using DQB2ChunkEditor.Models;

namespace DQB2ChunkEditor.Windows;

public partial class MainWindow : Window
{
    public ObservableProperty<LayerTile> SelectedTile { get; set; } = new();
    public ObservableProperty<Tile> DropdownTile { get; set; } = new();
    public ObservableCollection<ComboBoxTile> Tiles { get; set; } = new();

    public MainWindow()
    {
        InitializeComponent();
        CreateDefaultTiles();
        DataContext = this;
    }

    /// <summary>
    /// Creates the default tile grid used for the chunk layers and their click event
    /// </summary>
    public void CreateDefaultTiles()
    {
        for (var i = 0; i < 1024; i++) // chunk layers are 32x32
        {
            var layerTile = new LayerTile
            {
                Id = i,
                Tile = new ObservableProperty<Tile>()
                {
                    Value = new Tile
                    {
                        Id = i,
                        Name = i.ToString()
                    }
                }
            };

            layerTile.TileButton.Click += (_, _) => { TileSelectEvent(layerTile); };

            LayerTiles.Children.Add(layerTile);

            Tiles.Add(new ComboBoxTile
            {
                Tile = layerTile.Tile.Value
            });
        }
    }

    /// <summary>
    /// Event handler for when a layer tile is clicked. Updates the tile modification section.
    /// </summary>
    /// <param name="layerTile"></param>
    private void TileSelectEvent(LayerTile layerTile)
    {
        SelectedTile.Value = layerTile;
        TileSelection.SelectedIndex = layerTile.Tile.Value!.Id;
        DropdownTile.Value = layerTile.Tile.Value;
    }

    private void TileUpdateEvent(object sender, SelectionChangedEventArgs e)
    {
        if (SelectedTile.Value == null)
        {
            return;
        }

        var newSelectedTile = ((ComboBoxTile)e.AddedItems[0]!).Tile;

        var id = SelectedTile.Value.Id;
        ((LayerTile)LayerTiles.Children[id]).Tile.Value = newSelectedTile;
        DropdownTile.Value = newSelectedTile;
    }
}
