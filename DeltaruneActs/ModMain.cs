using System.Reflection;
using Godot;
using HarmonyLib;
using DeltaruneActs.TP;
using MegaCrit.Sts2.Core.Modding;

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

        Logger.Info("Deltarune Acts loaded: ACT + TP + full spell set online.");
    }
}
