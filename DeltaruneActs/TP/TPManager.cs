namespace DeltaruneActs.TP;

public static class TPManager
{
    public const int MaxTP = 100;
    private static int _current;

    public static int Current => _current;
    public static event Action? Changed;

    public static bool CanSpend(int amount) => amount >= 0 && _current >= amount;

    public static bool Spend(int amount)
    {
        if (!CanSpend(amount))
            return false;

        _current -= amount;
        Changed?.Invoke();
        return true;
    }

    public static void Gain(int amount)
    {
        var next = Math.Clamp(_current + Math.Max(0, amount), 0, MaxTP);
        if (next == _current)
            return;

        _current = next;
        Changed?.Invoke();
    }

    public static void Reset()
    {
        _current = 0;
        Changed?.Invoke();
    }
}
