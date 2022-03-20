using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQB2ChunkEditor;

public static class ChunkEditor
{
    private const int HeaderLength = 272; // everything after the header is compressed data
    private const ushort LayerSize = 1024; // chunk layers are 32x32
    private const ushort LayerHeight = 96; // chunk layers are up to 96? blocks high
}
