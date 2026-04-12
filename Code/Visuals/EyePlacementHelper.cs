using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace FlandreMod.Visuals;

public static class EyePlacementHelper
{
    public static void TryPlaceRightOfTarget(Creature eye, Creature target, float scale = 0.6f)
    {
        var room = NCombatRoom.Instance;
        if (room == null) return;

        var eyeNode = room.GetCreatureNode(eye);
        var targetNode = room.GetCreatureNode(target);
        if (eyeNode == null || targetNode == null) return;

        float boundsWidth = targetNode.Visuals?.Bounds?.Size.X ?? 200f;
        float offset = boundsWidth * 0.5f + 30f;

        eyeNode.GlobalPosition = targetNode.GlobalPosition + new Vector2(offset, 0f);
        eyeNode.Scale = new Vector2(scale, scale);
    }
}

