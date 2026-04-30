using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace FlandreMod.Powers;

[HarmonyPatch]
public static class CustomPowerIconPatch
{
    private static readonly string ModDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
    private static readonly string? GameDirectory = Directory.GetParent(ModDirectory)?.Parent?.Parent?.FullName;

    [HarmonyPatch(typeof(PowerModel), nameof(PowerModel.Icon), MethodType.Getter)]
    [HarmonyPrefix]
    public static bool IconPrefix(PowerModel __instance, ref Texture2D __result)
    {
        string? iconPath = GetCustomIconPath(__instance, big: false, beta: false);
        if (iconPath == null)
            return true;

        Texture2D? texture = TryLoadTexture(iconPath);
        if (texture == null)
            return true;

        __result = texture;
        return false;
    }

    [HarmonyPatch(typeof(PowerModel), nameof(PowerModel.Icon), MethodType.Getter)]
    [HarmonyPostfix]
    public static void IconPostfix(PowerModel __instance, ref Texture2D __result)
    {
        string? iconPath = GetCustomIconPath(__instance, big: false, beta: false);
        if (iconPath == null) return;
        Texture2D? texture = TryLoadTexture(iconPath);
        if (texture != null)
            __result = texture;
    }

    [HarmonyPatch(typeof(PowerModel), nameof(PowerModel.BigIcon), MethodType.Getter)]
    [HarmonyPrefix]
    public static bool BigIconPrefix(PowerModel __instance, ref Texture2D __result)
    {
        string? iconPath = GetCustomIconPath(__instance, big: true, beta: false);
        string? betaIconPath = GetCustomIconPath(__instance, big: true, beta: true);
        string? packedIconPath = GetCustomIconPath(__instance, big: false, beta: false);
        if (iconPath == null && betaIconPath == null && packedIconPath == null)
            return true;

        Texture2D? texture = TryLoadTexture(iconPath)
            ?? TryLoadTexture(betaIconPath)
            ?? TryLoadTexture(packedIconPath);
        if (texture == null)
            return true;

        __result = texture;
        return false;
    }

    [HarmonyPatch(typeof(PowerModel), nameof(PowerModel.BigIcon), MethodType.Getter)]
    [HarmonyPostfix]
    public static void BigIconPostfix(PowerModel __instance, ref Texture2D __result)
    {
        string? iconPath = GetCustomIconPath(__instance, big: true, beta: false);
        string? betaIconPath = GetCustomIconPath(__instance, big: true, beta: true);
        string? packedIconPath = GetCustomIconPath(__instance, big: false, beta: false);
        Texture2D? texture = TryLoadTexture(iconPath)
            ?? TryLoadTexture(betaIconPath)
            ?? TryLoadTexture(packedIconPath);
        if (texture != null)
            __result = texture;
    }

    [HarmonyPatch(typeof(NPower), "_Ready")]
    [HarmonyPostfix]
    public static void NPowerReadyPostfix(NPower __instance)
    {
        ApplyNodeOverride(__instance, "Ready");
    }

    [HarmonyPatch(typeof(NPower), "Reload")]
    [HarmonyPostfix]
    public static void NPowerReloadPostfix(NPower __instance)
    {
        ApplyNodeOverride(__instance, "Reload");
    }

    private static string? GetCustomIconPath(PowerModel power, bool big, bool beta)
    {
        string id = power.Id?.Entry ?? string.Empty;
        return id switch
        {
            "MADNESS_POWER" => "res://images/powers/madness_power.png",
            "LINK_POWER" => "res://images/powers/link_power.png",
            "BLOODSHED_POWER" => "res://images/powers/link_power.png",
            _ => power switch
            {
                MadnessPower => "res://images/powers/madness_power.png",
                LinkPower => "res://images/powers/link_power.png",
                BloodshedPower => "res://images/powers/link_power.png",
                _ => null
            }
        };
    }

    private static void ApplyNodeOverride(NPower nPower, string source)
    {
        PowerModel? model = nPower.Model;
        string? smallIconPath = GetCustomIconPath(model, big: false, beta: false);
        if (smallIconPath == null)
            return;

        Texture2D? smallTexture = TryLoadTexture(smallIconPath);
        if (smallTexture == null)
            return;

        TextureRect? icon = nPower.GetNodeOrNull<TextureRect>("%Icon") ?? nPower.GetNodeOrNull<TextureRect>("Icon");
        if (icon != null)
            icon.Texture = smallTexture;

        Texture2D? bigTexture = TryLoadTexture(GetCustomIconPath(model, big: true, beta: false))
            ?? TryLoadTexture(GetCustomIconPath(model, big: true, beta: true))
            ?? smallTexture;
        CpuParticles2D? powerFlash = nPower.GetNodeOrNull<CpuParticles2D>("%PowerFlash") ?? nPower.GetNodeOrNull<CpuParticles2D>("PowerFlash");
        if (powerFlash != null && bigTexture != null)
            powerFlash.Texture = bigTexture;
    }

    private static Texture2D? TryLoadTexture(string? resourcePath)
    {
        if (string.IsNullOrWhiteSpace(resourcePath))
            return null;

        try
        {
            Texture2D? fromResource = ResourceLoader.Load<Texture2D>(resourcePath, null, ResourceLoader.CacheMode.Reuse);
            if (fromResource != null)
                return fromResource;
        }
        catch
        {
            // Fall back to disk-based loading below.
        }

        foreach (string filePath in GetCandidateFilePaths(resourcePath))
        {
            if (!File.Exists(filePath))
                continue;

            var image = new Image();
            if (image.Load(filePath) != Error.Ok)
                continue;

            return ImageTexture.CreateFromImage(image);
        }
        return null;
    }

    private static IEnumerable<string> GetCandidateFilePaths(string resourcePath)
    {
        string trimmed = resourcePath.StartsWith("res://", StringComparison.OrdinalIgnoreCase)
            ? resourcePath["res://".Length..]
            : resourcePath;
        trimmed = trimmed.Replace('/', Path.DirectorySeparatorChar);

        if (!string.IsNullOrEmpty(GameDirectory))
            yield return Path.Combine(GameDirectory, trimmed);

        if (!string.IsNullOrEmpty(ModDirectory))
        {
            yield return Path.Combine(ModDirectory, trimmed);
            if (trimmed.StartsWith($"images{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                yield return Path.Combine(ModDirectory, "flandremod", trimmed);

            string current = ModDirectory;
            for (int i = 0; i < 5; i++)
            {
                DirectoryInfo? parent = Directory.GetParent(current);
                if (parent == null)
                    break;

                current = parent.FullName;
                yield return Path.Combine(current, trimmed);
                if (trimmed.StartsWith($"images{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                    yield return Path.Combine(current, "flandremod", trimmed);
            }
        }
    }

}
