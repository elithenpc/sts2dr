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

    // Every card gets its own portrait file. The art generator creates one
    // real Deltarune battle-sprite frame per card and a matching 4x big image.
    public override string PortraitPath => $"{ArtRoot}{CardName(Id.Entry)}.png";
    public override string CustomPortraitPath => $"{BigArtRoot}{CardName(Id.Entry)}.png";
    public override string BetaPortraitPath => PortraitPath;
}
