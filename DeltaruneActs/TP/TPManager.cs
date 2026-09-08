namespace DeltaruneActs.TP;

public static class TPManager
{
    public const int MaxTP = 100;
    private static int _current;

    public static int Current => _current;

    public static bool CanSpend(int amount) => amount >= 0 && _current >= amount;

    public static bool Spend(int amount)
    {
        if (!CanSpend(amount))
            return false;

        _current -= amount;
        return true;
    }

    public static void Gain(int amount) => _current = Math.Clamp(_current + Math.Max(0, amount), 0, MaxTP);
    public static void Reset() => _current = 0;
}
