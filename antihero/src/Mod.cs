// SPDX-FileCopyrightText: 2026 Sukaretto
// SPDX-License-Identifier: AGPL-3.0-only

using MelonLoader;
using UniverseLib.UI;
using com.ultrabit.bitheroes.core;
using antihero.States;
using UnityEngine;
using UniverseLib.Config;

[assembly: MelonInfo(typeof(antihero.Mod), "antihero", "0.0.1", "Sukaretto")]
[assembly: MelonGame("Ultrabit", "Bit Heroes")]

namespace antihero;

public class Mod : MelonMod
{
    public static Mod Instance { get; private set; } = null!;

    private bool _isGameLoaded;

    private State _state = new IdleState();
    private UIBase? _ui;
    public Panel? Panel { get; private set; }

    public override void OnInitializeMelon() => Instance = this;

    private void OnInitializeUniverse()
    {
        _ui = UniversalUI.RegisterUI("com.sukaretto.antihero", null);
        Panel = new Panel(_ui);
        MelonLogger.Msg("ui initialized.");
    }

    public override void OnUpdate()
    {
        if (!_isGameLoaded && GameData.instance?.PROJECT?.character != null)
        {
            _isGameLoaded = true;
            MelonLogger.Msg("game loaded.");
            UniverseLib.Universe.Init(1f, OnInitializeUniverse, (_, _) => {}, new UniverseLibConfig());
        }

        // Panel exists only when game is loaded.
        if (Panel != null && Input.GetKeyDown(KeyCode.F9)) Panel.SetActive(!Panel.Enabled);

        _state.OnUpdate();
    }

    public void Transition(State next)
    {
        _state.OnExit();
        _state = next;
        _state.OnEnter();
    }
}