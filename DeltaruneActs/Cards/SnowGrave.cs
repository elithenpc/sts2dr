using DeltaruneActs.TP;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DeltaruneActs.Cards;

public sealed class SnowGrave : DeltaruneActsCard
{
    public SnowGrave() : base(0, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is null || cardPlay.Target.IsDead || !TPManager.Spend(100)) return;
        await DamageCmd.Attack(60).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
    }
}
