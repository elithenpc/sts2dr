using DeltaruneActs.ACT;
using DeltaruneActs.TP;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DeltaruneActs.Cards;

/// <summary>Ralsei-inspired mercy spell that converts TP into a large Spare-progress jump.</summary>
public sealed class Pacify : DeltaruneActsCard
{
    public Pacify() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is not { IsDead: false } target || !TPManager.Spend(16))
            return;

        ActResolver.AddProgress(target, 40);
        await BlockCmd.Gain(3, this);
    }
}
