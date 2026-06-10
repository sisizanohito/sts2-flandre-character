using FlandreMod.Characters;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace FlandreMod.Bloodshed;

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
public static class BloodshedClotVisualPatch
{
    private const string ClotNodeName = "FlandreBloodshedClot";

    public static void Postfix(CombatState state)
    {
        var player = LocalContext.GetMe(state);
        if (player?.Character is not FlandreCharacter)
            return;

        NCreatureVisuals? visuals = NCombatRoom.Instance?.GetCreatureNode(player.Creature)?.Visuals;
        if (visuals == null || visuals.GetNodeOrNull<NBloodshedClot>(ClotNodeName) != null)
            return;

        NBloodshedClot clot = NBloodshedClot.Create(player, visuals);
        clot.Name = ClotNodeName;
        visuals.AddChild(clot);
    }
}
