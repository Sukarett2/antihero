// SPDX-FileCopyrightText: 2026 Sukaretto
// SPDX-License-Identifier: AGPL-3.0-only

using MelonLoader;
using HarmonyLib;
using com.ultrabit.bitheroes.core;
using com.ultrabit.bitheroes.ui;
using com.ultrabit.bitheroes.ui.instance.fishing;
using antihero.Patches;

namespace antihero.States;

public class FishingState : State
{
    private static readonly HarmonyLib.Harmony Harmony = new("com.sukaretto.antihero.fishing");

    public override void OnEnter()
    {
        Harmony.Patch(
            AccessTools.Method(typeof(WindowGenerator), nameof(WindowGenerator.NewFishingCaptureWindow)),
            prefix: new HarmonyMethod(typeof(FishingPatches), nameof(FishingPatches.OnNewFishingCaptureWindow))
        );
        Harmony.Patch(
            AccessTools.Method(typeof(InstanceFishingInterface), nameof(InstanceFishingInterface.DoFishingStart)),
            postfix: new HarmonyMethod(typeof(FishingPatches), nameof(FishingPatches.OnDoFishingStart))
        );
        Harmony.Patch(
            AccessTools.Method(typeof(InstanceFishingInterface), nameof(InstanceFishingInterface.DoFishingCasting)),
            postfix: new HarmonyMethod(typeof(FishingPatches), nameof(FishingPatches.OnDoFishingCasting))
        );
        Harmony.Patch(
            AccessTools.Method(typeof(InstanceFishingInterface), "OnFishingBobberLanded"),
            prefix: new HarmonyMethod(typeof(FishingPatches), nameof(FishingPatches.OnFishingBobberLanded))
        );
        Harmony.Patch(
            AccessTools.Method(typeof(InstanceFishingInterface), nameof(InstanceFishingInterface.DoFishingCatchStart)),
            postfix: new HarmonyMethod(typeof(FishingPatches), nameof(FishingPatches.OnDoFishingCatchStart))
        );
        Harmony.Patch(
            AccessTools.Method(typeof(InstanceFishingInterface), nameof(InstanceFishingInterface.DoFishingCatchComplete)),
            postfix: new HarmonyMethod(typeof(FishingPatches), nameof(FishingPatches.OnDoFishingCatchComplete))
        );

        var fishing = GameData.instance.PROJECT.instance.instanceFishingInterface;
        if (Utils.GetField<object>(fishing, "_startScreen") == null) return;
        fishing.DoFishingStart();
    }

    public override void OnUpdate()
    {
        if (GameData.instance.PROJECT.instance.instanceFishingInterface != null) return;
        MelonLogger.Msg("[FISHING] fishing interface lost.");
        Mod.Instance.Transition(new IdleState());
    }

    public override void OnExit()
    {
        Harmony.UnpatchAll(Harmony.Id);
        Mod.Instance.Panel?.FishingToggle.SetIsOnWithoutNotify(false);
    }
}