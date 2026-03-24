// SPDX-FileCopyrightText: 2026 Sukaretto
// SPDX-License-Identifier: AGPL-3.0-only

using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Panels;
using antihero.States;
using com.ultrabit.bitheroes.core;

namespace antihero;

public class Panel(UIBase owner) : PanelBase(owner)
{
    public override string Name => "AntiHero";
    public override int MinWidth => 100;
    public override int MinHeight => 200;
    public override Vector2 DefaultAnchorMin => new(0.25f, 0.25f);
    public override Vector2 DefaultAnchorMax => new(0.75f, 0.75f);
    public override bool CanDragAndResize => true;

    public Toggle FishingToggle = null!;
    public Toggle DungeonToggle = null!;

    protected override void ConstructPanelContent()
    {
        {
            var fishingSection = UIFactory.CreateVerticalGroup(
                ContentRoot, "FishingSection",
                forceWidth: true,
                forceHeight: false,
                childControlWidth: true,
                childControlHeight: true,
                spacing: 4,
                padding: new Vector4(8, 8, 8, 8),
                bgColor: new Color(0.1255f, 0.1255f, 0.1255f, 1f)
            );

            UIFactory.CreateToggle(fishingSection, "FishingToggle", out FishingToggle, out Text label);
            label.text = "Auto Fishing";
            FishingToggle.SetIsOnWithoutNotify(false);
            FishingToggle.onValueChanged.AddListener(isOn =>
                Mod.Instance.Transition(isOn ? new FishingState() : new IdleState()));
            UIFactory.CreateLabel(fishingSection, "FishingText",
                "Must be at a fishing spot with bait.", TextAnchor.MiddleLeft,
                new Color(0.6f, 0.6f, 0.6f, 1f), fontSize: 12);
        }

        {
            var dungeonSection = UIFactory.CreateVerticalGroup(
                ContentRoot, "DungeonSection",
                forceWidth: true,
                forceHeight: false,
                childControlWidth: true,
                childControlHeight: true,
                spacing: 4,
                padding: new Vector4(8, 8, 8, 8),
                bgColor: new Color(0.1255f, 0.1255f, 0.1255f, 1f)
            );

            UIFactory.CreateToggle(dungeonSection, "DungeonToggle", out DungeonToggle, out Text label);
            label.text = "Auto Dungeon";
            DungeonToggle.SetIsOnWithoutNotify(false);
            DungeonToggle.onValueChanged.AddListener(isOn =>
                Mod.Instance.Transition(isOn ? new DungeonState() : new IdleState()));
            UIFactory.CreateLabel(dungeonSection, "DungeonText",
                "Must be in a dungeon.", TextAnchor.MiddleLeft,
                new Color(0.6f, 0.6f, 0.6f, 1f), fontSize: 12);
        }
    }

    public override void Update()
    {
        base.Update();

        var project = GameData.instance.PROJECT;
        DungeonToggle.interactable = project.dungeon != null;
        FishingToggle.interactable =
            project.instance.instanceFishingInterface !=null && Utils.HasBait();
    }
}