using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.sts2.Core.Nodes.TopBar;
using System.Collections.Generic;

namespace FlandreMod.Characters;

[HarmonyPatch]
public static class FlandreVisualTextureFixPatch
{
    private const float IdleBobAmplitude = 4f;
    private const float IdleScaleAmplitude = 0.015f;

    private sealed class IdleAnimState
    {
        public Vector2 BasePosition;
        public Vector2 BaseScale;
        public Tween? IdleTween;
    }

    private static readonly Dictionary<ulong, IdleAnimState> _idleStates = new();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NCreatureVisuals), nameof(NCreatureVisuals._Ready))]
    public static void CreatureVisualReadyPostfix(NCreatureVisuals __instance)
    {
        if (!__instance.Name.ToString().Contains("FlandreCharacter"))
        {
            return;
        }

        var visuals = __instance.GetNodeOrNull<Sprite2D>("%Visuals");
        if (visuals == null || visuals.Texture != null)
        {
            return;
        }

        var texture = FlandreTextureHelper.LoadTexture(FlandreTextureHelper.CombatTexturePath);
        Log.Warn($"[FlandreMod] Combat Visual Texture Load path={FlandreTextureHelper.CombatTexturePath} ok={(texture != null)}");
        if (texture != null)
        {
            visuals.Texture = texture;
        }

        // Enable a lightweight idle animation for the static sprite setup.
        var instanceKey = __instance.GetInstanceId();
        _idleStates[instanceKey] = new IdleAnimState
        {
            BasePosition = visuals.Position,
            BaseScale = visuals.Scale,
            IdleTween = StartIdleTween(visuals)
        };
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NCreatureVisuals), nameof(NCreatureVisuals._ExitTree))]
    public static void CreatureVisualExitTreePostfix(NCreatureVisuals __instance)
    {
        _idleStates.Remove(__instance.GetInstanceId());
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CreatureCmd), nameof(CreatureCmd.TriggerAnim))]
    public static void CreatureTriggerAnimPostfix(Creature creature, string triggerName)
    {
        if (!TryGetFlandreState(creature, out var state, out var visuals))
        {
            return;
        }

        if (triggerName == "Attack")
        {
            PlayImpulseTween(visuals, state, new Vector2(22f, -6f), new Vector2(0.045f, -0.025f));
        }
        else if (triggerName == "Hit")
        {
            PlayImpulseTween(visuals, state, new Vector2(-16f, 4f), new Vector2(-0.04f, 0.02f));
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NTopBarPortrait), nameof(NTopBarPortrait.Initialize))]
    public static void TopBarPortraitInitializePostfix(NTopBarPortrait __instance, Player player)
    {
        var idEntry = player?.Character?.Id.Entry;
        if (string.IsNullOrEmpty(idEntry) || !idEntry.EndsWith("FLANDRE_CHARACTER"))
        {
            return;
        }

        var texture = FlandreTextureHelper.LoadTexture(FlandreTextureHelper.TopbarTexturePath);
        Log.Warn($"[FlandreMod] TopBar Icon Texture Load path={FlandreTextureHelper.TopbarTexturePath} ok={(texture != null)} id={idEntry}");
        if (texture == null)
        {
            return;
        }

        var applied = false;
        foreach (var child in __instance.GetChildren())
        {
            if (child is not TextureRect rect)
            {
                continue;
            }

            // Apply to empty rects, or to the explicit flandre icon node if it exists.
            if (rect.Texture == null || rect.Name.ToString().Contains("FlandreCharacterIcon"))
            {
                rect.Texture = texture;
                EnsureIconRectVisible(rect, __instance);
                applied = true;
            }
        }

        // Fallback: try recursive lookup by name in case hierarchy differs (e.g. Neow/topbar variants).
        var named = __instance.FindChild("FlandreCharacterIcon", recursive: true, owned: false) as TextureRect;
        if (named != null)
        {
            named.Texture = texture;
            EnsureIconRectVisible(named, __instance);
            applied = true;
        }

        if (!applied)
        {
            Log.Warn("[FlandreMod] TopBar icon patch found no TextureRect target to apply.");
        }
    }

    private static void EnsureIconRectVisible(TextureRect rect, Control parent)
    {
        if (rect.Size.X <= 0 || rect.Size.Y <= 0)
        {
            rect.Position = Vector2.Zero;
            rect.Size = parent.Size;
            rect.CustomMinimumSize = parent.Size;
        }

        rect.ExpandMode = TextureRect.ExpandModeEnum.FitWidthProportional;
        rect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
    }

    private static bool TryGetFlandreState(Creature creature, out IdleAnimState state, out Sprite2D visuals)
    {
        state = null!;
        visuals = null!;
        var nCreature = NCombatRoom.Instance?.GetCreatureNode(creature);
        var visualsNode = nCreature?.Visuals;
        if (visualsNode == null || !visualsNode.Name.ToString().Contains("FlandreCharacter"))
        {
            return false;
        }

        var key = visualsNode.GetInstanceId();
        if (!_idleStates.TryGetValue(key, out var found) || found == null)
        {
            return false;
        }

        visuals = visualsNode.GetNodeOrNull<Sprite2D>("%Visuals");
        if (visuals == null)
        {
            return false;
        }

        state = found;
        return true;
    }

    private static Tween StartIdleTween(Sprite2D visuals)
    {
        var basePos = visuals.Position;
        var baseScale = visuals.Scale;
        var pulseScale = baseScale + new Vector2(IdleScaleAmplitude, -IdleScaleAmplitude * 0.5f);
        var settleScale = baseScale + new Vector2(-IdleScaleAmplitude * 0.6f, IdleScaleAmplitude * 0.3f);

        var tween = visuals.CreateTween().SetLoops();

        tween.Parallel().TweenProperty(visuals, "position:y", basePos.Y + IdleBobAmplitude, 0.95)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);
        tween.Parallel().TweenProperty(visuals, "scale", pulseScale, 0.95)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);

        tween.Parallel().TweenProperty(visuals, "position:y", basePos.Y - IdleBobAmplitude, 1.05)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);
        tween.Parallel().TweenProperty(visuals, "scale", settleScale, 1.05)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);

        return tween;
    }

    private static void PlayImpulseTween(Sprite2D visuals, IdleAnimState state, Vector2 kickOffset, Vector2 kickScale)
    {
        var startPos = visuals.Position;
        var startScale = visuals.Scale;

        var tween = visuals.CreateTween();
        tween.TweenProperty(visuals, "position", startPos + kickOffset, 0.08)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        tween.Parallel().TweenProperty(visuals, "scale", startScale + kickScale, 0.08)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);

        tween.TweenProperty(visuals, "position", state.BasePosition, 0.16)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.InOut);
        tween.Parallel().TweenProperty(visuals, "scale", state.BaseScale, 0.16)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.InOut);
    }
}
