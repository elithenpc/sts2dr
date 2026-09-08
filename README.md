# Deltarune Acts for Slay the Spire 2

A content mod for Slay the Spire 2 inspired by Deltarune's ACT, TP, and spell concepts.

## Current prototype

### ACT system

- CHECK
- COMPLIMENT
- WARN
- JOKE
- FLIRT
- SPARE
- Per-enemy Spare progress.
- Normal enemies require 100 progress.
- Elites require 150.
- Bosses cannot currently be Spared.

### TP system

- Separate 0-100 TP resource.
- TP resets when combat starts.
- Normal attacks, skills, and powers generate TP.
- ACT cards and spells spend TP instead of Energy.

### Deltarune-inspired spells

- **Heal Prayer**: 32 TP, heals a selected ally for 12 HP.
- **Rude Buster**: 50 TP, deals 18 damage to one enemy.
- **Pacify**: 16 TP, adds 40 Spare progress to one enemy.
- **Red Buster**: 60 TP, deals 28 damage to one enemy.

The TP costs for Heal Prayer, Rude Buster, and Pacify follow their corresponding Deltarune costs. The actual Slay the Spire 2 effects and damage/healing numbers are original to this mod.

## Art

The repository does not bundle ripped Deltarune sprites, screenshots, music, or other game assets. Spell artwork is intended to be original fan-made art based on the concepts.

Mechanics references are documented in `DeltaruneActs/Art/README.md`.

## Development

This project follows the current `Alchyr/ModTemplate-StS2` content-mod structure and targets .NET 9 / Godot 4.5.1.

Next major pieces are the in-game TP display, richer enemy-specific ACT reactions, and final original card artwork.
