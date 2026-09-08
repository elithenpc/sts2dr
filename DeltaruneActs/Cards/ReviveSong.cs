using MegaCrit.Sts2.Core.Entities.Cards;

namespace DeltaruneActs.Cards;

[Pool(typeof(BasicCardPool))]
public partial class ReviveSong : DeltaruneActsCard
{
    public override CardId Id => DeltaruneActsCardIds.ReviveSong;
    public override TargetType TargetType => TargetType.AnyAlly;
    public override int BaseCost => 84;
    public override CardType CardType => CardType.Skill;

    public override void OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SpendTp(choiceContext, BaseCost);
        HealTarget(choiceContext, cardPlay.Target as Creature, 25);
    }
}
