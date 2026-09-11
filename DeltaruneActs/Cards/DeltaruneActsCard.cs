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

    private static string CardName(string entry) => entry.RemovePrefix().ToLowerInvariant();

    public int RequiredTP => GetType().Name switch
    {
        nameof(HealPrayer) => 32,
        nameof(DualHeal) => 50,
        nameof(Pacify) => 16,
        nameof(ReviveKris) => 16,
        nameof(ReviveSong) => 84,
        nameof(LightUp) => 1,
        nameof(Healing) => 75,
        nameof(OkayHeal) => 50,
        nameof(BetterHeal) => 75,
        nameof(UltraHeal) => 85,
        nameof(UltimateHeal) => 100,
        nameof(RudeBuster) => 50,
        nameof(RedBuster) => 60,
        nameof(DualBuster) => 100,
        nameof(RudeSword) => 50,
        nameof(Scythemare) => 40,
        nameof(WakeKris) => 16,
        nameof(IceShock) => 16,
        nameof(SnowGrave) => 100,
        nameof(SleepMist) => 32,
        nameof(Spare) => 50,
        _ => 0
    };

    public override string PortraitPath => $"{ArtRoot}{CardName(Id.Entry)}.png";
    public override string CustomPortraitPath => $"{BigArtRoot}{CardName(Id.Entry)}.png";
    public override string BetaPortraitPath => PortraitPath;
}
