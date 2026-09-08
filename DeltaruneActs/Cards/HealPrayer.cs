using DeltaruneActs.TP;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DeltaruneActs.Cards;

/// <summary>
/// Ralsei-inspired support spell. The effect is original to this mod and uses
/// STS2's normal healing command rather than copying Deltarune implementation code.
/// </summary>
public sealed class HealPrayer : DeltaruneActsCard
{
    public HealPrayer() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is not Creature target || target.IsDead || !TPManager.Spend(32))
            return;

        await CreatureCmd.Heal(target, 12);
    }
}
