using DeltaruneActs.TP;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DeltaruneActs.Cards;

public sealed class SleepMist : DeltaruneActsCard
{
    public SleepMist() : base(0, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!TPManager.Spend(32)) return Task.CompletedTask;
        foreach (var enemy in choiceContext.CombatState.Enemies)
            ActResolver.AddSpareProgress(enemy, 25);
        return Task.CompletedTask;
    }
}
