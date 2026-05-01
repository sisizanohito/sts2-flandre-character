using FlandreMod.Characters;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;

namespace FlandreMod.Cards;

[HarmonyPatch(typeof(AtlasResourceLoader))]
public static class FlandreEnergyAtlasLoaderPatch
{
    private static AtlasTexture? _cachedEnergyTexture;

    [HarmonyPrefix]
    [HarmonyPatch(nameof(AtlasResourceLoader._Exists))]
    public static bool ExistsPrefix(string path, ref bool __result)
    {
        if (!IsFlandreEnergySprite(path))
        {
            return true;
        }

        __result = true;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(AtlasResourceLoader._Load))]
    public static bool LoadPrefix(string path, ref Variant __result)
    {
        if (!IsFlandreEnergySprite(path))
        {
            return true;
        }

        AtlasTexture? texture = GetEnergyTexture();
        if (texture == null)
        {
            __result = 7L;
            return false;
        }

        __result = texture;
        return false;
    }

    private static bool IsFlandreEnergySprite(string path)
    {
        return path == FlandreTextureHelper.CardEnergySpritePath;
    }

    private static AtlasTexture? GetEnergyTexture()
    {
        if (_cachedEnergyTexture != null && GodotObject.IsInstanceValid(_cachedEnergyTexture))
        {
            return _cachedEnergyTexture;
        }

        Texture2D? atlas = FlandreTextureHelper.LoadTexture(FlandreTextureHelper.CardEnergyAtlasPath);
        if (atlas == null)
        {
            return null;
        }

        _cachedEnergyTexture = new AtlasTexture
        {
            Atlas = atlas,
            Region = new Rect2(0, 0, atlas.GetWidth(), atlas.GetHeight())
        };
        return _cachedEnergyTexture;
    }
}
