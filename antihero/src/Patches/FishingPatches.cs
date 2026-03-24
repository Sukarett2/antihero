// SPDX-FileCopyrightText: 2026 Sukaretto
// SPDX-License-Identifier: AGPL-3.0-only

using HarmonyLib;
using MelonLoader;
using UnityEngine;
using System.Collections;
using com.ultrabit.bitheroes.ui;
using com.ultrabit.bitheroes.ui.instance.fishing;
using com.ultrabit.bitheroes.model.fishing;
using com.ultrabit.bitheroes.model.item;
using antihero.States;

namespace antihero.Patches;

public static class FishingPatches
{
    [HarmonyPatch(typeof(WindowGenerator), nameof(WindowGenerator.NewFishingCaptureWindow))]
    [HarmonyPrefix]
    public static bool OnNewFishingCaptureWindow() => false;

    [HarmonyPatch(typeof(InstanceFishingInterface), nameof(InstanceFishingInterface.DoFishingStart))]
    [HarmonyPostfix]
    public static void OnDoFishingStart(InstanceFishingInterface __instance)
    {
        // Fishing without bait kicks the player from the server.
        if (!Utils.HasBait())
        {
            MelonLogger.Msg("[FISHING] ran out of bait, stopping.");
            Mod.Instance.Transition(new IdleState());
            return;
        }
        MelonCoroutines.Start(DeferredDoFishingCasting(__instance));
    }

    private static IEnumerator DeferredDoFishingCasting(InstanceFishingInterface instance)
    {
        yield return new WaitForSeconds(1f); // TODO: Check if state changed for a return.
        instance.DoFishingCasting();
    }

    [HarmonyPatch(typeof(InstanceFishingInterface), nameof(InstanceFishingInterface.DoFishingCasting))]
    [HarmonyPostfix]
    public static void OnDoFishingCasting(InstanceFishingInterface __instance)
    {
        Utils.GetField<InstanceFishingCastingScreen>(__instance, "_castingScreen")
            ?.gameObject.SetActive(false);

        var player = __instance.GetPlayer();
        var distance = UnityEngine.Random.Range(player.GetFishingDistanceMin(), player.GetFishingDistanceMax());
        __instance.instance.extension.DoFishingCast(distance);
    }

    [HarmonyPatch(typeof(InstanceFishingInterface), "OnFishingBobberLanded")]
    [HarmonyPrefix]
    public static bool OnFishingBobberLanded(InstanceFishingInterface __instance)
    {
        __instance.GetPlayer().BOBBER_COMPLETE.RemoveAllListeners();
        Utils.Invoke(__instance, "OnFishingCatching");
        return false;
    }

    [HarmonyPatch(typeof(InstanceFishingInterface), nameof(InstanceFishingInterface.DoFishingCatchStart))]
    [HarmonyPostfix]
    public static void OnDoFishingCatchStart(InstanceFishingInterface __instance)
    {
        Utils.GetField<InstanceFishingCatchingScreen>(__instance, "_catchingScreen")
            ?.gameObject.SetActive(false);

        var best = __instance.GetPlayer().fishingData.itemRef.barRef.chances
            .OrderByDescending(c => c.perc)
            .FirstOrDefault();
        __instance.DoCatchSend(best);
    }

    [HarmonyPatch(typeof(InstanceFishingInterface), nameof(InstanceFishingInterface.DoFishingCatchComplete))]
    [HarmonyPostfix]
    public static void OnDoFishingCatchComplete(FishingItemRef itemRef, List<ItemData> items, int weight, bool success)
    {
        if (!success)
        {
            MelonLogger.Msg("[FISHING] catch failed.");
            return;
        }

        var item = items[0];
        MelonLogger.Msg($"[FISHING] item: {item.itemRef.name} x{item.qty}");
    }
}
