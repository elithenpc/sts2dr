# Card art and source material

The card portrait files in this directory's parent image tree are currently based on sprite material from DELTAModKit, a Deltarune decompilation/modding project.

Source repository:

- https://github.com/deltamodders/deltamodkit

DELTAModKit states that its repository contains assets from the free Steam demo and credits the sprites to Toby Fox and Royal Sciences LLC. This mod does not claim ownership of those source game assets.

## Sprite mapping

The current portrait set uses these source sprite concepts:

- Rude Buster beam: RudeBuster, DualBuster, RudeSword, Scythemare
- Red Buster beam: RedBuster
- Ice spell mist/snowflake: IceShock, SnowGrave, SleepMist
- Ralsei spell effect: HealPrayer, DualHeal, Healing, OkayHeal, BetterHeal, UltraHeal, UltimateHeal, ReviveSong, ReviveKris, WakeKris, LightUp
- Spare star: Pacify, Spare, Check, Compliment, Flirt, Warn, Joke

Some cards therefore intentionally reuse a source sprite because the source project does not provide a distinct one-to-one sprite for every card in this mod.

## File locations

The card classes use BaseLib's standard card image helpers. The corresponding files are stored under:

- `DeltaruneActs/images/card_portraits/<card>.png`
- `DeltaruneActs/images/card_portraits/big/<card>.png`

The base template documents the normal card-art size as 1000x760 and the full-art size as 606x852, while smaller portrait variants are supported as well. The currently committed source sprites are raw pixel-art assets rather than newly painted full-card illustrations, so they are intended as functional placeholder/source art rather than a finished custom card-art pass.

## Mechanics references

- https://deltarune.wiki/w/TP
- https://deltarune.wiki/w/Spells
- https://deltarune.wiki/w/Heal_Prayer
- https://deltarune.wiki/w/Rude_Buster
