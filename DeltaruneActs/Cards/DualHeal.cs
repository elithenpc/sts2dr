using MegaCrit.Sts2.Core.Entities.Cards;

namespace DeltaruneActs.Cards;

[Pool(typeof(BasicCardPool))]
public partial class DualHeal : DeltaruneActsCard
{
    public override CardId Id => DeltaruneActsCardIds.DualHeal;
    public override TargetType TargetType => TargetType.AllAllies;
    public override int BaseCost => 50;
    public override CardType CardType => CardType.Skill;

    public override void OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SpendTp(choiceContext, BaseCost);
        HealAllAllies(choiceContext, 18);
    }
}
