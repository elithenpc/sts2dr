using DeltaruneActs.ACT;
using DeltaruneActs.TP;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace DeltaruneActs.Cards;

public sealed class Flirt : DeltaruneActsCard
{
    public Flirt() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is not { IsDead: false } target || !TPManager.Spend(30))
            return;

        ActResolver.AddProgress(target, 30);
        await CreatureCmd.GainBlock(Owner.Creature, 8, ValueProp.Move, cardPlay);
    }
}
