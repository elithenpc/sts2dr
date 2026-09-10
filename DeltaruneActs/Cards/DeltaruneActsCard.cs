using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace DeltaruneActs.Cards;

[Pool(typeof(ColorlessCardPool))]
public abstract class DeltaruneActsCard(int cost, CardType type, CardRarity rarity, TargetType target)
    : CustomCardModel(cost, type, rarity, target)
{
    private const string ArtRoot = "res://DeltaruneActs/images/card_portraits/";
    private const string BigArtRoot = "res://DeltaruneActs/images/card_portraits/big/";

    private string ArtName => GetType().Name switch
    {
        // Kris ACT cards use Kris's real battle ACT animation.
        nameof(Check) => "kris_act",
        nameof(Compliment) => "kris_act",
        nameof(Warn) => "kris_act",
        nameof(Joke) => "kris_act",
        nameof(Flirt) => "kris_act",
        nameof(Spare) => "kris_act",

        // Ralsei support / spell cards use his actual battle spell animation.
        nameof(HealPrayer) => "ralsei_spell",
        nameof(DualHeal) => "ralsei_spell",
        nameof(Pacify) => "ralsei_spell",
        nameof(ReviveKris) => "ralsei_spell",
        nameof(ReviveSong) => "ralsei_spell",
        nameof(LightUp) => "ralsei_spell",

        // Susie spell cards use her actual battle spell animation.
        nameof(Healing) => "susie_spell",
        nameof(OkayHeal) => "susie_spell",
        nameof(BetterHeal) => "susie_spell",
        nameof(UltraHeal) => "susie_spell",
        nameof(UltimateHeal) => "susie_spell",
        nameof(RudeBuster) => "susie_spell",
        nameof(RedBuster) => "susie_spell",
        nameof(DualBuster) => "susie_spell",
        nameof(RudeSword) => "susie_spell",
        nameof(Scythemare) => "susie_spell",
        nameof(WakeKris) => "susie_spell",

        // These are specifically Noelle's spells, so they must never use Kris art.
        nameof(IceShock) => "noelle_spell",
        nameof(SnowGrave) => "noelle_spell",
        nameof(SleepMist) => "noelle_spell",

        _ => "kris_act"
    };

    // BaseLib consumes the normal portrait for the card and the big portrait
    // when the card is enlarged. Both files are generated from the same real
    // Deltarune battle-sprite frame, so the artwork cannot disagree between views.
    public override string PortraitPath => $"{ArtRoot}{ArtName}.png";
    public override string CustomPortraitPath => $"{BigArtRoot}{ArtName}.png";
    public override string BetaPortraitPath => PortraitPath;
}
