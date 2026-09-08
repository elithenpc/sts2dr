using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace DeltaruneActs.Cards;

[Pool(typeof(BasicCardPool))]
public partial class IceShock : DeltaruneActsCard
{
    public override CardId Id => DeltaruneActsCardIds.IceShock;
    public override TargetType TargetType => TargetType.AnyEnemy;
    public override int BaseCost => 16;
    public override CardType CardType => CardType.Skill;

    public override void OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target as Creature;
        if (target == null) return;
        SpendTp(choiceContext, BaseCost);
        DamageTarget(choiceContext, target, 16);
    }
}
