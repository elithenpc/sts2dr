using MegaCrit.Sts2.Core.Entities.Creatures;

namespace DeltaruneActs.ACT;

public static class ActResolver
{
    private static readonly Dictionary<Creature, int> SpareProgress = new();

    public static int GetProgress(Creature target) => SpareProgress.TryGetValue(target, out var value) ? value : 0;

    public static int AddProgress(Creature target, int amount)
    {
        if (target.IsDead)
            return GetProgress(target);

        var value = Math.Max(0, GetProgress(target) + amount);
        SpareProgress[target] = value;
        return value;
    }

    public static bool CanSpare(Creature target)
    {
        if (target.IsDead || target.Monster is null)
            return false;

        var typeName = target.Monster.GetType().Name;
        if (typeName.Contains("Boss", StringComparison.OrdinalIgnoreCase))
            return false;

        var threshold = typeName.Contains("Elite", StringComparison.OrdinalIgnoreCase) ? 150 : 100;
        return GetProgress(target) >= threshold;
    }

    public static void Remove(Creature target) => SpareProgress.Remove(target);
    public static void Reset() => SpareProgress.Clear();
}
