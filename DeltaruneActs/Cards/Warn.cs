using DeltaruneActs.ACT;
using DeltaruneActs.TP;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DeltaruneActs.Cards;

public sealed class Warn : DeltaruneActsCard
{
    public Warn() : base(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is not { IsDead: false } target || !TPManager.Spend(20))
            return;

        ActResolver.AddProgress(target, 15);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), target, 1, Owner.Creature, this);
    }
}
