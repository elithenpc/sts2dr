using DeltaruneActs.TP;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DeltaruneActs.Cards;

public sealed class UltimateHeal : DeltaruneActsCard
{
    public UltimateHeal() : base(0, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly) { }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is not Creature target || target.IsDead || !TPManager.Spend(100)) return;
        await CreatureCmd.Heal(target, 60);
    }
}
