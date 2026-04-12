using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace FlandreMod;

[ModInitializer("Init")]
public static class ModEntry
{
    private static Harmony? _harmony;

    public static void Init()
    {
        Log.Warn("[FlandreMod] Initializing...");

        _harmony = new Harmony("com.isiis.flandremod");
        _harmony.PatchAll();

        Log.Warn("[FlandreMod] Loaded successfully!");
    }
}
