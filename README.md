## DQB2 Chunk Editor

Dragon Quest Builders 2 Map Chunk Editor based on the map flattener from https://github.com/turtle-insect/DQB2

### Requirements
[.NET 6 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0/runtime)

### Save Data

Steam save files should be located in `C:\Users\<user>\Documents\My Games\DRAGON QUEST BUILDERS II\Steam\<steam id>\SD\`

B00, B01, B02 are the corresponding save slots for the data.

STGDAT01.BIN is the Isle of Awakening map data.

Backups are not made when editing, so make sure to keep a backup of the save folders.

### Information

Able to edit all chunks and layers. Some issues with unmapped blocks due to duplication. Still need to add color and block shapes as well. I have the ID's but still need to add functionality for it.

"Items" are more difficult than simple blocks and I am still researching the data structure. Looks like there are a lot of different things that go into those, placement direction, shadows, block tile effects.

<img src="./src/Images/Screenshot.png" data-canonical-src="./src/Images/Screenshot.png" width="958" height="517" />
