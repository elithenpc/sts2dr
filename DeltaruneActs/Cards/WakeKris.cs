using DeltaruneActs.TP;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DeltaruneActs.Cards;

public sealed class WakeKris : DeltaruneActsCard
{
    public WakeKris() : base(0, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly) { }
    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        TPManager.Spend(16);
        return Task.CompletedTask;
    }
}
