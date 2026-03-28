// SPDX-FileCopyrightText: 2026 Sukaretto
// SPDX-License-Identifier: AGPL-3.0-only

using com.ultrabit.bitheroes.ui.menu;
using HarmonyLib;

namespace antihero.Patches;

public static class DungeonPatches
{
    // Idiot-proof
    [HarmonyPatch(typeof(MenuInterfaceAutoPilotTile), nameof(MenuInterfaceAutoPilotTile.DoClick))]
    [HarmonyPrefix]
    public static bool OnAutoPilotClick() => false;
}
