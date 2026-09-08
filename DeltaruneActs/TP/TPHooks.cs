using System.Reflection;
using HarmonyLib;
using DeltaruneActs.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Hooks;

namespace DeltaruneActs.TP;

public static class TPHooks
{
    public static void Install(Harmony harmony)
    {
        var afterCardPlayed = typeof(Hook).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == nameof(Hook.AfterCardPlayed) && m.GetParameters().Length == 3);

        if (afterCardPlayed != null)
        {
            var postfix = typeof(TPHooks).GetMethod(nameof(AfterCardPlayed), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(afterCardPlayed, postfix: new HarmonyMethod(postfix));
        }

        var beforeCombatStart = typeof(Hook).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == nameof(Hook.BeforeCombatStart) && m.GetParameters().Length == 2);

        if (beforeCombatStart != null)
        {
            var prefix = typeof(TPHooks).GetMethod(nameof(BeforeCombatStart), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(beforeCombatStart, prefix: new HarmonyMethod(prefix));
        }
    }

    private static void AfterCardPlayed(object __0, object __1, CardPlay __2)
    {
        if (__2.Card is DeltaruneActsCard)
            return;

        switch (__2.Card.Type)
        {
            case CardType.Attack:
                TPManager.Gain(5);
                break;
            case CardType.Skill:
                TPManager.Gain(3);
                break;
            case CardType.Power:
                TPManager.Gain(2);
                break;
        }
    }

    private static void BeforeCombatStart() => TPManager.Reset();
}
