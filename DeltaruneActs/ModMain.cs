using System.Reflection;
using Godot;
using HarmonyLib;
using DeltaruneActs.TP;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace DeltaruneActs;

[ModInitializer(nameof(Initialize))]
public partial class ModMain : Node
{
    public const string ModId = "DeltaruneActs";
    public const string ResPath = $"res://{ModId}";
    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var harmony = new Harmony(ModId);
        harmony.PatchAll(assembly);
        TPHooks.Install(harmony);

        ModelDb.Inject(typeof(Cards.Check));
        ModelDb.Inject(typeof(Cards.Compliment));
        ModelDb.Inject(typeof(Cards.Warn));
        ModelDb.Inject(typeof(Cards.Joke));
        ModelDb.Inject(typeof(Cards.Flirt));
        ModelDb.Inject(typeof(Cards.Spare));

        ModelDb.Inject(typeof(Cards.HealPrayer));
        ModelDb.Inject(typeof(Cards.Pacify));
        ModelDb.Inject(typeof(Cards.RudeBuster));
        ModelDb.Inject(typeof(Cards.RedBuster));
        ModelDb.Inject(typeof(Cards.IceShock));
        ModelDb.Inject(typeof(Cards.SnowGrave));
        ModelDb.Inject(typeof(Cards.DualHeal));
        ModelDb.Inject(typeof(Cards.ReviveSong));
        ModelDb.Inject(typeof(Cards.SleepMist));
        ModelDb.Inject(typeof(Cards.Scythemare));
        ModelDb.Inject(typeof(Cards.RudeSword));
        ModelDb.Inject(typeof(Cards.Healing));
        ModelDb.Inject(typeof(Cards.OkayHeal));
        ModelDb.Inject(typeof(Cards.BetterHeal));
        ModelDb.Inject(typeof(Cards.UltraHeal));
        ModelDb.Inject(typeof(Cards.UltimateHeal));
        ModelDb.Inject(typeof(Cards.DualBuster));
        ModelDb.Inject(typeof(Cards.LightUp));
        ModelDb.Inject(typeof(Cards.WakeKris));
        ModelDb.Inject(typeof(Cards.ReviveKris));

        Logger.Info("Deltarune Acts loaded: ACT + TP + full spell set online.");
    }
}
