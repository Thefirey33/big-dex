# Big Dex!

This game uses the NikoDex's API to make a game that resembles the game "Big Money!", produced by PopCap Games.

This is just a side project that's being worked on.

## How it works

The game uses C# for the generally heavy networking and storage, meanwhile using GDScript for the lighter decorations.

This game makes use of it's own custom HTTP system, Godot's HTTPRequest system is not used, as it's too locked to one node per communication only. So a custom HTTP system was made.

The `NikoDexApi` class handles all of NikoDex API requests, `NikoStorageManager` stores and compresses the Niko data, `ImageCreator` creates textures for the imported Nikosonas.

The Nikosona Image and JSON data are stored in RAM, but for later use they are cached by `NikoStorageManager`, so the NikoDex API doesn't constantly recieve requests for the same Nikosonas.

## Requirements

- Godot Engine 4.6.3 (Mono C#)
- .NET SDK 10.0 (What was used to make this game) or .NET SDK 8.0 LTS (Minimum Requirement)
