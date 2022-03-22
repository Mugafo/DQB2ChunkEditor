using System.Text.Json;
using System.Text.Json.Serialization;

namespace DQB2ChunkEditor.Models;

public class Tile
{
    [JsonPropertyName("Type")]
    public Type Type { get; set; } = Type.Block;

    [JsonPropertyName("Id")]
    public ushort Id { get; set; } = 0;

    [JsonPropertyName("Name")]
    public string Name { get; set; } = "Unknown";

    [JsonPropertyName("Description")]
    public string Description { get; set; } = "N/A";

    [JsonIgnore]
    public string Image => $@"Data\Tiles\Blocks\{Id:0000}.png";
}
