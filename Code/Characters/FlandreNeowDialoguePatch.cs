using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.Events;

namespace FlandreMod.Characters;

[HarmonyPatch(typeof(Neow), "DefineDialogues")]
public static class FlandreNeowDialoguePatch
{
    private const string FlandreCharacterId = "FLANDREMOD-FLANDRE_CHARACTER";
    private const string SfxSleepy = "event:/sfx/npcs/neow/neow_sleepy";
    private const string SfxWelcome = "event:/sfx/npcs/neow/neow_welcome";
    private const string SfxCurious = "event:/sfx/npcs/neow/neow_curious";

    [HarmonyPostfix]
    public static void DefineDialoguesPostfix(AncientDialogueSet __result)
    {
        if (__result.CharacterDialogues.ContainsKey(FlandreCharacterId))
        {
            return;
        }

        __result.CharacterDialogues[FlandreCharacterId] = new List<AncientDialogue>
        {
            new(SfxWelcome)
            {
                VisitIndex = 0
            },
            new(SfxCurious)
            {
                VisitIndex = 1
            },
            new(SfxSleepy)
        };

        Log.Warn($"[FlandreMod] Registered Neow dialogue for {FlandreCharacterId}.");
    }
}
