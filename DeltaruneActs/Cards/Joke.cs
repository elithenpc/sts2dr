using DeltaruneActs.ACT;
using DeltaruneActs.TP;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace DeltaruneActs.Cards;

public sealed class Joke : DeltaruneActsCard
{
    public Joke() : base(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is not { IsDead: false } target || !TPManager.Spend(15))
            return;

        ActResolver.AddProgress(target, 10);
        await CreatureCmd.GainBlock(Owner.Creature, 5, ValueProp.Move, cardPlay);
        TPManager.Gain(5);
    }
}
