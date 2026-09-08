using DeltaruneActs.ACT;
using DeltaruneActs.TP;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace DeltaruneActs.Helpers;

public static class ActHelpers
{
    public static bool SpendTp(int cost) => TPManager.Spend(cost);

    public static void Progress(Creature target, int amount) => ActResolver.AddProgress(target, amount);
}
