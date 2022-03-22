using System;
using System.IO;
using System.Text;
using Ionic.Zlib;

namespace DQB2ChunkEditor;

public static class ChunkEditor
{
    private const uint LayerSize = 0x400; // chunk layers are 32x32
    private const uint LayerHeight = 0x60; // chunk layers are up to 96 blocks high
    private const uint HeaderLength = 0x110; // everything after the header is compressed data
    private const uint ChunkSize = 0x30000; // chunk size is layer size * layer height * block size (2 bytes)
    private const uint BlockStart = 0x183FEF0; // starting address for the block data
    private const string ExpectedFileHeader = "aerC"; // expected file header for the map

    private static byte[] _headerBytes = new byte[HeaderLength];
    private static byte[] _uncompressedBytes = null!;

    public static string Filename { get; set; } = null!;

    public static void LoadFile(string filename)
    {
        var fileBytes = File.ReadAllBytes(filename);
        var headerBytes = new byte[4];
        Array.Copy(fileBytes, 0, headerBytes, 0, 4);

        var actualHeader = Encoding.UTF8.GetString(headerBytes);

        if (ExpectedFileHeader != actualHeader)
        {
            return;
        }

        var compressedBytes = new byte[fileBytes.Length - HeaderLength];

        Array.Copy(fileBytes, HeaderLength, compressedBytes, 0, compressedBytes.Length);
        Array.Copy(fileBytes, _headerBytes, HeaderLength);

        _uncompressedBytes = ZlibStream.UncompressBuffer(compressedBytes);

        Filename = filename;
    }
    public static void SaveFile()
    {
        var compressedBytes = ZlibStream.CompressBuffer(_uncompressedBytes);
        var outputFileBytes = new byte[HeaderLength + compressedBytes.Length];

        Array.Copy(_headerBytes, outputFileBytes, HeaderLength);
        Array.Copy(compressedBytes, 0, outputFileBytes, HeaderLength, compressedBytes.Length);

        File.WriteAllBytes(Filename, outputFileBytes);
    }

    private static uint GetByteIndex(uint chunk, byte layer, ushort tile)
    {
        return (uint)(BlockStart + (chunk * ChunkSize) + (layer * LayerSize * 2) + (tile * 2));
    }

    public static ushort GetBlockValue(uint chunk, byte layer, ushort tile)
    {
        var index = GetByteIndex(chunk, layer, tile);

        var blockBytes = new byte[2];
        blockBytes[0] = _uncompressedBytes[index];
        blockBytes[1] = _uncompressedBytes[index + 1];

        return BitConverter.ToUInt16(blockBytes);
    }

    public static void SetBlockValue(uint chunk, byte layer, ushort tile, ushort blockId)
    {
        var index = GetByteIndex(chunk, layer, tile);

        _uncompressedBytes[index] = (byte)blockId;
        _uncompressedBytes[index + 1] = (byte)(blockId >> 8);
    }
}
