# Deltarune Acts for Slay the Spire 2

A content mod for Slay the Spire 2 inspired by Deltarune's ACT and TP concepts.

## Current prototype

- Six ACT cards: CHECK, COMPLIMENT, WARN, JOKE, FLIRT, and SPARE.
- Separate TP resource with a 100 TP cap.
- TP is gained from normal card play and resets when combat starts.
- ACT cards spend TP instead of Energy.
- ACT actions build per-enemy Spare progress.
- Normal enemies require 100 Spare progress.
- Enemies whose type is identified as Elite require 150.
- Bosses cannot currently be Spared.
- SPARE uses the game's creature kill/removal action after the threshold is reached.

## Development

This project follows the current `Alchyr/ModTemplate-StS2` content-mod structure and targets .NET 9 / Godot 4.5.1.

The project intentionally uses original code rather than copying Deltarune assets, music, or dialogue.

The next major piece is the in-game TP display and more detailed enemy-specific ACT reactions.
