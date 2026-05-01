using FlandreMod.Characters;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace FlandreMod.Cards;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.Portrait), MethodType.Getter)]
public static class FlandreCardPortraitPatch
{
    public static bool Prefix(CardModel __instance, ref Texture2D __result)
    {
        string? portraitPath = __instance switch
        {
            FlandreStrikeCard => "res://flandremod/images/cards/strike_flandre.png",
            FlandreDefendCard => "res://flandremod/images/cards/defend_flandre.png",
            _ => null
        };

        if (portraitPath == null)
            return true;

        Texture2D? texture = FlandreTextureHelper.LoadTexture(portraitPath);
        if (texture == null)
            return true;

        __result = texture;
        return false;
    }
}
