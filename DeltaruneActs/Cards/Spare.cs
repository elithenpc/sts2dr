using DeltaruneActs.ACT;
using DeltaruneActs.TP;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DeltaruneActs.Cards;

public sealed class Spare : DeltaruneActsCard
{
    public Spare() : base(0, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is not { IsDead: false } target || !TPManager.Spend(50))
            return;

        if (!ActResolver.CanSpare(target))
        {
            await BlockCmd.Gain(10, this);
            return;
        }

        ActResolver.Remove(target);
        await CreatureCmd.Kill(target, force: true);
    }
}
