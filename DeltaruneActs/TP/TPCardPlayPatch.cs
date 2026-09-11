using DeltaruneActs.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace DeltaruneActs.TP;

/// <summary>
/// Uses the game's native CardModel.CanPlay gate so TP-starved cards cannot
/// enter the play queue. The card therefore behaves like an ordinary
/// unplayable card until enough TP is available.
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.CanPlay))]
internal static class TPCardPlayPatch
{
    private static void Postfix(CardModel __instance, ref bool __result)
    {
        if (__instance is DeltaruneActsCard card && card.RequiredTP > 0 && !TPManager.CanSpend(card.RequiredTP))
            __result = false;
    }
}
