using System.Reflection;
using Godot;
using HarmonyLib;
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

        ModelDb.Inject(typeof(Cards.Check));
        ModelDb.Inject(typeof(Cards.Compliment));
        ModelDb.Inject(typeof(Cards.Warn));
        ModelDb.Inject(typeof(Cards.Joke));
        ModelDb.Inject(typeof(Cards.Flirt));
        ModelDb.Inject(typeof(Cards.Spare));

        Logger.Info("Deltarune Acts loaded: ACT + TP system online.");
    }
}
