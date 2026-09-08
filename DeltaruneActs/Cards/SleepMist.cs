using MegaCrit.Sts2.Core.Entities.Cards;

namespace DeltaruneActs.Cards;

[Pool(typeof(BasicCardPool))]
public partial class SleepMist : DeltaruneActsCard
{
    public override CardId Id => DeltaruneActsCardIds.SleepMist;
    public override TargetType TargetType => TargetType.AllEnemies;
    public override int BaseCost => 32;
    public override CardType CardType => CardType.Skill;

    public override void OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SpendTp(choiceContext, BaseCost);
        MarkAllEnemiesTired(choiceContext, 25);
    }
}
