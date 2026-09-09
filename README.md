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
- An in-combat `TP: current / 100` display is shown in the combat UI.

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
- MegaDot/Godot 4.5.1 Mono for exporting the resource `.pck`.
- BaseLib is pulled through NuGet and is also required by the mod manifest.

For a normal code build:

```powershell
dotnet build
```

For a complete mod package including the card images, make a local build settings file first:

```powershell
Copy-Item local.props.template local.props
```

Then edit `local.props` and set `GodotPath` to your MegaDot/Godot 4.5.1 Mono executable. The current STS2 modding setup uses Godot/MegaDot 4.5.1 for `.pck` export. citeturn3search8turn3search6

Then run:

```powershell
dotnet publish
```

Publishing exports `DeltaruneActs.pck` and copies the DLL, PDB, manifest, and PCK into the detected `mods/DeltaruneActs/` directory. STS2 modding templates use the same publish-to-PCK workflow for resource assets. citeturn2search0turn2search6

If Slay the Spire 2 is installed in a non-standard Steam library, set `Sts2Path` in `local.props`. BaseLib's current path-discovery setup likewise supports a local path override. citeturn3search4

BaseLib must be installed in the game's `mods` directory before loading this mod.

## Development status

The main ACT, TP, spell, portrait, and combat UI systems are implemented. Enemy-specific ACT reactions are still generic rather than specialised per enemy, and the card art is currently source-sprite based rather than a finished custom card-art pass.

## Compatibility

The project follows the current `Alchyr/ModTemplate-StS2` content-mod structure and targets .NET 9 / Godot 4.5.1. Slay the Spire 2 is in Early Access, so game updates can require corresponding mod updates. The game is actively receiving updates, so the mod's compiled API compatibility should be checked after major game updates. citeturn1search1turn1search6
