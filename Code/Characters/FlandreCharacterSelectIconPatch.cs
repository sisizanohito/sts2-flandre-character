using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

namespace FlandreMod.Characters;

[HarmonyPatch]
public static class FlandreCharacterSelectIconPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NCharacterSelectButton), nameof(NCharacterSelectButton.Init))]
    public static void InitPostfix(NCharacterSelectButton __instance, CharacterModel character)
    {
        Log.Warn($"[FlandreMod] CharSelect Init id={character?.Id.Entry} type={character?.GetType().Name} locked={__instance.IsLocked}");
        TryApplyCustomIcon(__instance, character);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NCharacterSelectButton), nameof(NCharacterSelectButton.LockForAnimation))]
    public static void LockForAnimationPostfix(NCharacterSelectButton __instance)
    {
        TryApplyCustomIcon(__instance, __instance.Character);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NCharacterSelectButton), nameof(NCharacterSelectButton.DebugUnlock))]
    public static void DebugUnlockPostfix(NCharacterSelectButton __instance)
    {
        TryApplyCustomIcon(__instance, __instance.Character);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NCharacterSelectButton), nameof(NCharacterSelectButton.UnlockIfPossible))]
    public static void UnlockIfPossiblePostfix(NCharacterSelectButton __instance)
    {
        TryApplyCustomIcon(__instance, __instance.Character);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NCharacterSelectScreen), nameof(NCharacterSelectScreen.SelectCharacter))]
    public static void SelectCharacterPostfix(NCharacterSelectScreen __instance, CharacterModel characterModel)
    {
        var idEntry = characterModel?.Id.Entry;
        if (string.IsNullOrEmpty(idEntry) || !idEntry.EndsWith("FLANDRE_CHARACTER"))
        {
            return;
        }

        var bgNode = __instance.GetNodeOrNull<TextureRect>($"AnimatedBg/{idEntry}_bg");
        if (bgNode == null)
        {
            return;
        }

        if (bgNode.Texture != null)
        {
            return;
        }

        var bgTexture = FlandreTextureHelper.LoadTexture(FlandreTextureHelper.CharacterSelectBackgroundPath);
        Log.Warn($"[FlandreMod] CharSelect Bg Load path={FlandreTextureHelper.CharacterSelectBackgroundPath} ok={(bgTexture != null)} id={idEntry}");
        if (bgTexture != null)
        {
            bgNode.Texture = bgTexture;
        }
    }

    private static void TryApplyCustomIcon(NCharacterSelectButton button, CharacterModel character)
    {
        var idEntry = character?.Id.Entry;
        if (string.IsNullOrEmpty(idEntry) || !idEntry.EndsWith("FLANDRE_CHARACTER"))
        {
            return;
        }

        var icon = button.GetNodeOrNull<TextureRect>("%Icon");
        if (icon == null)
        {
            return;
        }

        var path = button.IsLocked
            ? FlandreTextureHelper.CharacterSelectLockedIconPath
            : FlandreTextureHelper.CharacterSelectIconPath;
        var texture = FlandreTextureHelper.LoadTexture(path);

        Log.Warn($"[FlandreMod] CharSelect Icon Load path={path} ok={(texture != null)} id={idEntry}");
        if (texture != null)
        {
            icon.Texture = texture;
        }
    }

}
