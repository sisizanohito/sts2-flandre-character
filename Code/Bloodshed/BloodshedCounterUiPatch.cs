using System;
using System.Linq;
using System.Reflection;
using FlandreMod.Characters;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace FlandreMod.Bloodshed;

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
public static class BloodshedCounterUiPatch
{
    private const string CounterNodeName = "FlandreBloodshedCounter";
    private static readonly FieldInfo? EnergyCounterField = AccessTools.Field(typeof(NCombatUi), "_energyCounter");

    public static void Postfix(NCombatUi __instance, CombatState state)
    {
        Player? player = LocalContext.GetMe(state);
        if (player?.Character is not FlandreCharacter)
        {
            player = state.Players.FirstOrDefault(candidate => candidate.Character is FlandreCharacter);
        }

        if (player?.Character is not FlandreCharacter)
            return;

        Node? energyCounterNode = EnergyCounterField?.GetValue(__instance) as Node;
        energyCounterNode ??= __instance.EnergyCounterContainer
            .GetChildren()
            .OfType<Node>()
            .FirstOrDefault(child => child.Name.ToString().Contains("EnergyCounter", StringComparison.Ordinal));

        if (energyCounterNode == null)
            return;

        if (energyCounterNode.GetNodeOrNull<NBloodshedCounter>(CounterNodeName) != null)
            return;

        NBloodshedCounter counter = NBloodshedCounter.Create(player);
        counter.Name = CounterNodeName;
        energyCounterNode.AddChild(counter);
    }
}
