using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using DQB2ChunkEditor.Controls;
using DQB2ChunkEditor.Models;
using Microsoft.Win32;

namespace DQB2ChunkEditor.Windows;

public partial class MainWindow : Window
{
    public ObservableProperty<LayerTile> SelectedTile { get; set; } = new();
    public ObservableCollection<ComboBoxTile> Tiles { get; set; } = new();
    private List<Tile> TileList { get; } = new();

    public MainWindow()
    {
        InitializeComponent();
        CreateDefaultTiles();
        CreateComboBoxTiles();
        DataContext = this;
    }

    /// <summary>
    /// Creates the default tile grid used for the chunk layers and their click event
    /// </summary>
    private void CreateDefaultTiles()
    {
        for (ushort i = 0; i < 1024; i++) // chunk layers are 32x32
        {
            var layerTile = new LayerTile
            {
                Id = i,
                Tile = new ObservableProperty<Tile>
                {
                    Value = new Tile()
                }
            };

            layerTile.TileButton.Click += (_, _) => { TileSelectEvent(layerTile); };

            LayerTiles.Children.Add(layerTile);
        }
    }

    /// <summary>
    /// Creates the dropdown selection tiles for changing a selected tile
    /// </summary>
    private void CreateComboBoxTiles()
    {
        var options = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        var json = File.ReadAllText(@"Data\Tiles.json");

        var tiles = JsonSerializer.Deserialize<TileList>(json, options);

        foreach (var tile in tiles.Tiles)
        {
            Tiles.Add(new ComboBoxTile
            {
                Tile = tile
            });

            TileList.Add(tile);
        }
    }

    /// <summary>
    /// Event handler for when a layer tile is clicked. Updates the tile modification section.
    /// </summary>
    /// <param name="layerTile"></param>
    private void TileSelectEvent(LayerTile layerTile)
    {
        if (layerTile.Tile.Value == null)
        {
            return;
        }

        SelectedTile.Value = layerTile;

        TileSelection.SelectionChanged -= TileUpdateEvent;
        TileSelection.SelectedIndex = layerTile.Tile.Value.Id;
        TileSelection.SelectionChanged += TileUpdateEvent;
    }

    /// <summary>
    /// Event handler for when a new tile is selected from the dropdown. Updates the map data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TileUpdateEvent(object sender, SelectionChangedEventArgs e)
    {
        if (SelectedTile.Value == null)
        {
            return;
        }

        var selectedTile = ((ComboBoxTile)e.AddedItems[0]!).Tile;

        ((LayerTile)LayerTiles.Children[SelectedTile.Value.Id]).Tile.Value = selectedTile;

        ChunkEditor.SetBlockValue(0, 0, SelectedTile.Value.Id, selectedTile.Id);
    }

    private void RefreshTiles(uint chunk, byte layer)
    {
        
        for (ushort i = 0; i < 1024; i++)
        {
            var blockId = ChunkEditor.GetBlockValue(chunk, layer, i);

            ((LayerTile)LayerTiles.Children[i]).Tile.Value = TileList.FirstOrDefault(t => t.Id == blockId);
        }
    }

    private void OpenFileEvent(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "STGDAT*.BIN|STGDAT*.BIN"
        };

        if (openFileDialog.ShowDialog() == false)
        {
            return;
        }

        ChunkEditor.LoadFile(openFileDialog.FileName);

        RefreshTiles(0, 0);
    }
    private void SaveFileEvent(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ChunkEditor.Filename))
        {
            return;
        }

        ChunkEditor.SaveFile();
    }
}
