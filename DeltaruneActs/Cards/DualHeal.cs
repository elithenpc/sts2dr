using DeltaruneActs.TP;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DeltaruneActs.Cards;

public sealed class DualHeal : DeltaruneActsCard
{
    public DualHeal() : base(0, CardType.Skill, CardRarity.Rare, TargetType.AllAllies) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!TPManager.Spend(50)) return;
        foreach (var ally in CombatState.Players)
            if (!ally.IsDead) await CreatureCmd.Heal(ally, 18);
    }
}
