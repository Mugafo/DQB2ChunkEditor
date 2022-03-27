using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DQB2ChunkEditor.Controls;
using DQB2ChunkEditor.Models;
using Microsoft.Win32;

namespace DQB2ChunkEditor.Windows;

public partial class MainWindow : Window
{
    public ObservableProperty<LayerTile> SelectedTile { get; set; } = new();
    public ObservableCollection<ComboBoxTile> Tiles { get; set; } = new();
    private List<Tile> TileList { get; } = new();
    public ObservableProperty<short> ChunkValue { get; set; } = new() { Value = 0};
    public ObservableProperty<byte> LayerValue { get; set; } = new() { Value = 0 };

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
        try
        {
            for (short i = 0; i < 1024; i++) // layers are 32x32
            {
                var layerTile = new LayerTile
                {
                    Id = i,
                    Tile = new ObservableProperty<Tile>
                    {
                        Value = new Tile()
                    }
                };

                layerTile.TileButton.Click += (_, _) => { TileButton_OnClick(layerTile); };

                LayerTiles.Children.Add(layerTile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// Creates the dropdown selection tiles for changing a selected tile
    /// </summary>
    private void CreateComboBoxTiles()
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private void RefreshTiles(short chunk, byte layer)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ChunkEditor.Filename))
            {
                return;
            }

            for (short i = 0; i < 1024; i++)
            {
                var blockId = ChunkEditor.GetBlockValue(chunk, layer, i);

                ((LayerTile)LayerTiles.Children[i]).Tile.Value = TileList.FirstOrDefault(t => t.Id == blockId) ?? TileList[^1];
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// Event handler for when a layer tile is clicked. Updates the tile selection section.
    /// </summary>
    private void TileButton_OnClick(LayerTile layerTile)
    {
        try
        {
            if (layerTile.Tile.Value == null)
            {
                return;
            }

            SelectedTile.Value = layerTile;

            //TileSelection.SelectionChanged -= TileUpdateEvent;
            TileSelection.SelectedIndex = layerTile.Tile.Value.Id;
            //TileSelection.SelectionChanged += TileUpdateEvent;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// Event handler for when a new tile is selected from the dropdown. Updates the selected tile.
    /// </summary>
    private void TileSelection_OnSelectionChange(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (SelectedTile.Value == null)
            {
                return;
            }

            var selectedTile = ((ComboBoxTile)e.AddedItems[0]!).Tile;

            ((LayerTile)LayerTiles.Children[SelectedTile.Value.Id]).Tile.Value = selectedTile;

            ChunkEditor.SetBlockValue(ChunkValue.Value, LayerValue.Value, SelectedTile.Value.Id, selectedTile.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// Opens a dialog for map file
    /// </summary>
    private void OpenFile_OnClick(object sender, RoutedEventArgs e)
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// Saves the current map bytes to file
    /// </summary>
    private void SaveFile_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ChunkEditor.Filename))
            {
                return;
            }

            ChunkEditor.SaveFile();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// Change the selected chunk area. Updates the layer tiles.
    /// </summary>
    private void ChunkInput_OnMouseUp(Object sender, MouseButtonEventArgs e)
    {
        try
        {
            var inputValueDialog = new InputValue();

            inputValueDialog.MaxValue = ChunkEditor.ChunkCount - 1;

            if (inputValueDialog.ShowDialog() == false ||
                !short.TryParse(inputValueDialog.ResponseText, out var value) ||
                value < 0 ||
                value >= ChunkEditor.ChunkCount)
            {
                return;
            }

            ChunkValue.Value = value;

            RefreshTiles(ChunkValue.Value, LayerValue.Value);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// Change the selected layer for a chunk. Updates the layer tiles.
    /// </summary>
    private void LayerInput_OnMouseUp(Object sender, MouseButtonEventArgs e)
    {
        try
        {
            var inputValueDialog = new InputValue();

            if (inputValueDialog.ShowDialog() == false ||
                !byte.TryParse(inputValueDialog.ResponseText, out var value) ||
                value is < 0 or > 95)
            {
                return;
            }

            LayerValue.Value = value;

            RefreshTiles(ChunkValue.Value, LayerValue.Value);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}
