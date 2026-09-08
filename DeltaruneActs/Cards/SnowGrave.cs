using MegaCrit.Sts2.Core.Entities.Cards;

namespace DeltaruneActs.Cards;

[Pool(typeof(BasicCardPool))]
public partial class SnowGrave : DeltaruneActsCard
{
    public override CardId Id => DeltaruneActsCardIds.SnowGrave;
    public override TargetType TargetType => TargetType.AnyEnemy;
    public override int BaseCost => 100;
    public override CardType CardType => CardType.Skill;

    public override void OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target as Creature;
        if (target == null) return;
        SpendTp(choiceContext, BaseCost);
        DamageTarget(choiceContext, target, 60);
    }
}
