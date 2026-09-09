using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace DeltaruneActs.Cards;

[Pool(typeof(ColorlessCardPool))]
public abstract class DeltaruneActsCard(int cost, CardType type, CardRarity rarity, TargetType target)
    : CustomCardModel(cost, type, rarity, target)
{
    private static string CardName(string entry) => entry.RemovePrefix().ToLowerInvariant();

    public override string CustomPortraitPath => $"res://DeltaruneActs/images/card_portraits/big/{CardName(Id.Entry)}.png";
    public override string PortraitPath => $"res://DeltaruneActs/images/card_portraits/{CardName(Id.Entry)}.png";
    public override string BetaPortraitPath => PortraitPath;
}
