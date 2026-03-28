// SPDX-FileCopyrightText: 2026 Sukaretto
// SPDX-License-Identifier: AGPL-3.0-only

using antihero.Patches;
using com.ultrabit.bitheroes.core;
using com.ultrabit.bitheroes.ui.dungeon;
using com.ultrabit.bitheroes.ui.menu;
using HarmonyLib;
using MelonLoader;

namespace antihero.States;

public class DungeonState : State
{
    private static readonly HarmonyLib.Harmony Harmony = new("com.sukaretto.antihero.fishing");

    public override void OnEnter()
    {
        Harmony.Patch(
            AccessTools.Method(typeof(MenuInterfaceAutoPilotTile), nameof(MenuInterfaceAutoPilotTile.DoClick)),
            new HarmonyMethod(typeof(DungeonPatches), nameof(DungeonPatches.OnAutoPilotClick))
        );
    }

    public override void OnUpdate()
    {
        var dungeon = Dungeon.instance;
        if (dungeon == null || dungeon.extension.defeated)
        {
            MelonLogger.Msg("[DUNGEON] dungeon lost, stopping.");
            Mod.Instance.Transition(new IdleState());
            return;
        }

        bool inBattle = GameData.instance.PROJECT.battle != null;
        GameData.instance.PROJECT.character.autoPilot = inBattle;
        if (inBattle) return;

        if (dungeon.extension.waiting || dungeon.extension.paused) return;
        if (dungeon.IsCleared())
        {
            MelonLogger.Msg("[DUNGEON] dungeon cleared!");
            return;
        }

        var player = dungeon.GetPlayer(GameData.instance.PROJECT.character.id);

        var targetNode = GetNearestEnemyNode(dungeon, player);
        if (targetNode == null) return;

        var currentNode = dungeon.GetObjectNode(player.gameObject);
        dungeon.extension.DoObjectActivate([currentNode, targetNode]);
    }

    public override void OnExit()
    {
        Harmony.UnpatchAll(Harmony.Id);
        Mod.Instance.Panel?.DungeonToggle.SetIsOnWithoutNotify(false);
    }

    private static List<DungeonNode> GetShortestPath(Dungeon dungeon, DungeonNode from, DungeonNode to) =>
        Utils.Invoke<List<DungeonNode>>(dungeon, "GetShortestPath", from, to, false);

    private static DungeonNode GetNearestEnemyNode(Dungeon dungeon, DungeonPlayer player)
    {
        var objectNodes = Utils.GetField<List<DungeonNode>>(dungeon, "_objectNodes")!;
        var currentNode = dungeon.GetObjectNode(player.gameObject);

        List<DungeonNode>? shortestPath = null;
        foreach (var node in objectNodes.Where(n =>
                     !n.empty && n.obj?.type is DungeonObject.TYPE_ENEMY or DungeonObject.TYPE_BOSS &&
                     !n.obj.ignorePath))
        {
            var path = GetShortestPath(dungeon, currentNode, node);
            if (shortestPath == null || path.Count < shortestPath.Count)
                shortestPath = path;
        }

        return shortestPath?.LastOrDefault() ?? null!;
    }
}
