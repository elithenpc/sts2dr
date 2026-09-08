using MegaCrit.Sts2.Core.Entities.Cards;

namespace DeltaruneActs.Cards;

[Pool(typeof(BasicCardPool))]
public partial class Scythemare : DeltaruneActsCard
{
    public override CardId Id => DeltaruneActsCardIds.Scythemare;
    public override TargetType TargetType => TargetType.AllEnemies;
    public override int BaseCost => 40;
    public override CardType CardType => CardType.Skill;

    public override void OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SpendTp(choiceContext, BaseCost);
        SpareTiredEnemies(choiceContext);
    }
}
