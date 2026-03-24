// SPDX-FileCopyrightText: 2026 Sukaretto
// SPDX-License-Identifier: AGPL-3.0-only

using HarmonyLib;
using com.ultrabit.bitheroes.core;

namespace antihero;

public static class Utils
{
    public static void Invoke(object obj, string methodName, params object[] args) =>
        AccessTools.Method(obj.GetType(), methodName).Invoke(obj, args);
    public static T Invoke<T>(object obj, string methodName, params object[] args) =>
        (T)AccessTools.Method(obj.GetType(), methodName).Invoke(obj, args);

    public static T? GetField<T>(object obj, string fieldName) where T : class =>
        AccessTools.Field(obj.GetType(), fieldName).GetValue(obj) as T;

    public static bool HasBait()
    {
        var character = GameData.instance?.PROJECT?.character;
        var bait = character?.getFishingBait();
        return bait != null && character?.getItemQty(bait) > 0;
    }
}
