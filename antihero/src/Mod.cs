// SPDX-FileCopyrightText: 2026 Sukaretto
// SPDX-License-Identifier: AGPL-3.0-only

using antihero;
using antihero.States;
using com.ultrabit.bitheroes.core;
using MelonLoader;
using UnityEngine;
using UniverseLib;
using UniverseLib.Config;
using UniverseLib.UI;

[assembly: MelonInfo(typeof(Mod), "antihero", "0.0.1", "Sukaretto")]
[assembly: MelonGame("Ultrabit", "Bit Heroes")]

namespace antihero;

public class Mod : MelonMod
{
    public static Mod Instance { get; private set; } = null!;
    public State State = new IdleState();
    public Panel? Panel { get; private set; }
    private bool _isGameLoaded;
    private UIBase? _ui;

    public override void OnInitializeMelon() => Instance = this;

    public override void OnUpdate()
    {
        if (!_isGameLoaded && GameData.instance?.PROJECT?.character != null)
        {
            _isGameLoaded = true;
            MelonLogger.Msg("game loaded.");
            Universe.Init(1f, OnInitializeUniverse, (_, _) => { }, new UniverseLibConfig());
        }

        // Panel exists only when game is loaded.
        if (Panel != null && Input.GetKeyDown(KeyCode.F9)) Panel.SetActive(!Panel.Enabled);

        State.OnUpdate();
    }

    private void OnInitializeUniverse()
    {
        _ui = UniversalUI.RegisterUI("com.sukaretto.antihero", null);
        Panel = new Panel(_ui);
        MelonLogger.Msg("ui initialized.");
    }

    public void Transition(State next)
    {
        State.OnExit();
        State = next;
        State.OnEnter();
    }
}
