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
    public List<Tile> TileList { get; } = new();
    public ObservableCollection<ComboBoxTile> TileComboBoxList { get; set; } = new();
    public ObservableProperty<Tile> SelectedTile { get; set; } = new();
    public ObservableProperty<LayerTile> SelectedLayerTile { get; set; } = new();
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

                layerTile.TileButton.Click += (_, _) => { LayerTile_OnClick(layerTile); };

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

            for (var i = 0; i < tiles!.Tiles.Count; i++)
            {
                TileComboBoxList.Add(new ComboBoxTile
                {
                    Id = i,
                    Tile = tiles.Tiles[i]
                });

                TileList.Add(tiles.Tiles[i]);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
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

                ((LayerTile)LayerTiles.Children[i]).Tile.Value = TileList.FirstOrDefault(t => t.Id % 2048 == blockId)/* ?? TileList[^1]*/;
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
    private void LayerTile_OnClick(LayerTile layerTile)
    {
        try
        {
            if (layerTile.Tile.Value == null)
            {
                return;
            }

            SelectedTile.Value = layerTile.Tile.Value;
            SelectedLayerTile.Value = layerTile;
            TileComboBox.SelectedIndex = TileComboBoxList.FirstOrDefault(t => t.Tile.Id == layerTile.Tile.Value.Id)!.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// Event handler for when a new tile is selected from the dropdown. Updates the selected tile.
    /// </summary>
    private void TileComboBox_OnSelectionChange(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (SelectedLayerTile.Value == null)
            {
                return;
            }

            var selectedTile = ((ComboBoxTile)e.AddedItems[0]!).Tile;

            SelectedTile.Value = selectedTile;

            ((LayerTile)LayerTiles.Children[SelectedLayerTile.Value.Id]).Tile.Value = selectedTile;

            ChunkEditor.SetBlockValue(ChunkValue.Value, LayerValue.Value, SelectedLayerTile.Value.Id, selectedTile.Id);
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
            MessageBox.Show(ex.Message, "Failed to open file", MessageBoxButton.OK, MessageBoxImage.Error);
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
            MessageBox.Show(ex.Message, "Failed to save file", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Change the selected chunk area. Updates the layer tiles.
    /// </summary>
    private void ChunkInput_OnMouseUp(Object sender, MouseButtonEventArgs e)
    {
        try
        {
            var inputValueDialog = new InputValue
            {
                MaxValue = ChunkEditor.ChunkCount - 1,
                CurrentValue = ChunkValue.Value
            };

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
            var inputValueDialog = new InputValue
            {
                CurrentValue = LayerValue.Value
            };

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
