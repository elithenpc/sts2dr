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

    private string ArtName => GetType().Name switch
    {
        nameof(Check) => "kris_check",
        nameof(Compliment) => "kris_compliment",
        nameof(Warn) => "kris_social",
        nameof(Joke) => "kris_social",
        nameof(Flirt) => "kris_compliment",
        nameof(Spare) => "kris_spare",
        nameof(HealPrayer) => "ralsei_healprayer",
        nameof(Healing) => "ralsei_heal",
        nameof(OkayHeal) => "ralsei_heal",
        nameof(BetterHeal) => "ralsei_heal",
        nameof(UltraHeal) => "ralsei_heal",
        nameof(UltimateHeal) => "ralsei_heal",
        nameof(DualHeal) => "dual",
        nameof(RudeBuster) => "susie_rudebuster",
        nameof(RedBuster) => "susie_redbuster",
        nameof(RudeSword) => "susie_rudebuster",
        nameof(Scythemare) => "susie_rudebuster",
        nameof(IceShock) => "kris_magic",
        nameof(SnowGrave) => "kris_magic",
        nameof(DualBuster) => "dual",
        nameof(LightUp) => "ralsei_healprayer",
        nameof(WakeKris) => "kris_magic",
        nameof(ReviveKris) => "kris_magic",
        nameof(ReviveSong) => "ralsei_healprayer",
        nameof(SleepMist) => "ralsei_healprayer",
        _ => "kris_social"
    };

    // The real Deltarune battle animations are prepared as PNG portraits by
    // tools/fetch_deltarune_sprites.py.
    public override string CustomPortraitPath => $"{ArtRoot}{ArtName}.png";
    public override string PortraitPath => $"{ArtRoot}{ArtName}.png";
    public override string BetaPortraitPath => PortraitPath;
}
