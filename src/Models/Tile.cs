using System.Collections.Generic;
using System.Diagnostics;

namespace DQB2ChunkEditor.Models;

public class Tile
{
    public Type Type { get; set; } = Type.Block;
    public int Id { get; set; } = 0;
    public string Name { get; set; } = "Unknown";
    public string Description { get; set; } = "N/A";
    public string Image => $@"Data\Tiles\tiles_{Id:0000}.png";
}
