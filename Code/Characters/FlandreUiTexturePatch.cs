using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Reaction;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Saves;

namespace FlandreMod.Characters;

[HarmonyPatch]
public static class FlandreUiTexturePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NMapMarker), nameof(NMapMarker.Initialize))]
    public static void MapMarkerInitializePostfix(NMapMarker __instance, Player player)
    {
        if (!FlandreTextureHelper.IsFlandre(player?.Character))
            return;

        Texture2D? texture = FlandreTextureHelper.LoadTexture(FlandreTextureHelper.MapMarkerTexturePath);
        if (texture != null)
            __instance.Texture = texture;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NReactionWheel), "_Input")]
    public static void ReactionWheelInputPostfix(NReactionWheel __instance)
    {
        if (!__instance.Visible)
            return;

        Player? localPlayer = Traverse.Create(__instance).Field<Player?>("_localPlayer").Value;
        if (!FlandreTextureHelper.IsFlandre(localPlayer?.Character))
            return;

        TextureRect? marker = Traverse.Create(__instance).Field<TextureRect>("_marker").Value;
        Texture2D? texture = FlandreTextureHelper.LoadTexture(FlandreTextureHelper.MapMarkerTexturePath);
        if (marker != null && texture != null)
            marker.Texture = texture;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NContinueRunInfo), "ShowInfo")]
    public static void ContinueRunInfoShowInfoPostfix(NContinueRunInfo __instance, SerializableRun save)
    {
        string? characterId = save?.Players.Count > 0 ? save.Players[0].CharacterId.Entry : null;
        if (!FlandreTextureHelper.IsFlandreCharacterId(characterId))
            return;

        TextureRect? icon = __instance.GetNodeOrNull<TextureRect>("%CharacterIcon");
        Texture2D? texture = FlandreTextureHelper.LoadTexture(FlandreTextureHelper.CharacterIconTexturePath);
        if (icon != null && texture != null)
            icon.Texture = texture;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NAncientDialogueLine), "SetCharacterAsSpeaker")]
    public static void AncientDialogueCharacterPostfix(NAncientDialogueLine __instance)
    {
        CharacterModel? character = Traverse.Create(__instance).Field<CharacterModel>("_character").Value;
        if (!FlandreTextureHelper.IsFlandre(character))
            return;

        TextureRect? icon = __instance.GetNodeOrNull<TextureRect>("%CharacterIcon/Icon");
        Texture2D? texture = FlandreTextureHelper.LoadTexture(FlandreTextureHelper.CharacterIconTexturePath);
        if (icon != null && texture != null)
            icon.Texture = texture;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NMultiplayerPlayerState), nameof(NMultiplayerPlayerState._Ready))]
    public static void MultiplayerPlayerStateReadyPostfix(NMultiplayerPlayerState __instance)
    {
        if (!FlandreTextureHelper.IsFlandre(__instance.Player?.Character))
            return;

        TextureRect? icon = __instance.GetNodeOrNull<TextureRect>("%CharacterIcon");
        Texture2D? texture = FlandreTextureHelper.LoadTexture(FlandreTextureHelper.CharacterIconTexturePath);
        if (icon != null && texture != null)
            icon.Texture = texture;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NRemoteLobbyPlayer), "RefreshVisuals")]
    public static void RemoteLobbyPlayerRefreshVisualsPostfix(NRemoteLobbyPlayer __instance)
    {
        CharacterModel? character = Traverse.Create(__instance).Field<CharacterModel>("_character").Value;
        if (!FlandreTextureHelper.IsFlandre(character))
            return;

        TextureRect? icon = __instance.GetNodeOrNull<TextureRect>("%CharacterIcon");
        Texture2D? texture = FlandreTextureHelper.LoadTexture(FlandreTextureHelper.CharacterIconTexturePath);
        if (icon != null && texture != null)
            icon.Texture = texture;
    }
}
