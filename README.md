# Deltarune Acts for Slay the Spire 2

A Slay the Spire 2 content mod inspired by Deltarune's ACT, TP, and spell systems.

## What is included

### ACT system

- Check
- Compliment
- Warn
- Joke
- Flirt
- Spare
- Per-enemy Spare progress.
- Normal enemies require 100 Spare progress.
- Elites require 150.
- Bosses cannot currently be Spared.

### TP system

- Separate 0-100 TP resource.
- TP resets at the start of combat.
- Attacks generate 5 TP, skills 3 TP, and powers 2 TP.
- ACT cards and Deltarune spell cards spend TP rather than Energy.
- TP is capped at 100.

### Deltarune-inspired spells

The current card set includes:

- Heal Prayer
- Pacify
- Rude Buster
- Red Buster
- IceShock
- SnowGrave
- DualHeal
- ReviveSong
- SleepMist
- Scythemare
- RudeSword
- Healing
- OkayHeal
- BetterHeal
- UltraHeal
- UltimateHeal
- DualBuster
- LightUp
- WakeKris
- ReviveKris

The spell names and TP costs are inspired by Deltarune. Their Slay the Spire 2 effects are implemented as original mod effects rather than attempting to reproduce Deltarune's combat engine.

## Art

Card portrait files are stored in the standard BaseLib card portrait locations:

- `DeltaruneActs/images/card_portraits/`
- `DeltaruneActs/images/card_portraits/big/`

The repository currently uses extracted Deltarune sprite material for several spell concepts, with the closest matching source sprite reused for cards that do not have a one-to-one source sprite. See `DeltaruneActs/Art/README.md` for the source and mapping.

## Building

Requirements:

- Slay the Spire 2 installed through Steam.
- .NET 9 SDK.
- Godot/MegaDot 4.5.1 Mono if you want to use the Godot project tooling.
- BaseLib is pulled through NuGet and is also required by the mod manifest.

From the repository directory, run:

```powershell
dotnet build
```

The build copies the DLL, PDB, and manifest into the game's `mods/DeltaruneActs/` directory automatically when the Slay the Spire 2 installation is detected. The path discovery follows the current ModTemplate-StS2/BaseLib approach and also supports an explicit `Sts2Path` override.

If the game does not detect the mod, make sure BaseLib is installed in the game's `mods` directory first.

## Development status

The repository is a playable content-mod implementation, but it should be treated as a work in progress. The TP resource currently exists as backend state and is not yet rendered as a dedicated on-screen TP meter. Enemy-specific ACT reactions are also not yet specialised per enemy.

## Compatibility

The project follows the current `Alchyr/ModTemplate-StS2` content-mod structure and targets .NET 9 / Godot 4.5.1. Slay the Spire 2 is in Early Access, so game updates can require corresponding mod updates.
