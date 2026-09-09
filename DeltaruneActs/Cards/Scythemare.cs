using DeltaruneActs.TP;
using DeltaruneActs.ACT;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DeltaruneActs.Cards;

public sealed class Scythemare : DeltaruneActsCard
{
    public Scythemare() : base(0, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!TPManager.Spend(40)) return Task.CompletedTask;
        foreach (var enemy in CombatState.Enemies)
            ActResolver.AddProgress(enemy, 100);
        return Task.CompletedTask;
    }
}
