using DeltaruneActs.TP;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DeltaruneActs.Cards;

public sealed class RudeSword : DeltaruneActsCard
{
    public RudeSword() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is null || cardPlay.Target.IsDead || !TPManager.Spend(50)) return;
        await DamageCmd.Attack(24).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
    }
}
