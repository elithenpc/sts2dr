using DeltaruneActs.TP;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DeltaruneActs.Cards;

/// <summary>Susie-inspired boss-style buster spell with a higher TP cost and damage.</summary>
public sealed class RedBuster : DeltaruneActsCard
{
    public RedBuster() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is null || cardPlay.Target.IsDead || !TPManager.Spend(60))
            return;

        await DamageCmd.Attack(28)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }
}
