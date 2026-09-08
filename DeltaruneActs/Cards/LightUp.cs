using DeltaruneActs.TP;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DeltaruneActs.Cards;

public sealed class LightUp : DeltaruneActsCard
{
    public LightUp() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        TPManager.Spend(1);
        return Task.CompletedTask;
    }
}
