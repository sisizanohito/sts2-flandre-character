using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace FlandreMod.Cards;

[HarmonyPatch]
public static class CardHoverTipPatch
{
    private static System.Reflection.MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(NCardHolder), "CreateHoverTips");
    }

    [HarmonyPrefix]
    public static bool Prefix(NCardHolder __instance, ref bool __state)
    {
        if (__instance.CardNode == null)
            return false;

        var hoverTips = __instance.CardNode.Model.HoverTips.ToList();
        if (NHoverTipSet.shouldBlockHoverTips)
        {
            __state = true;
            NHoverTipSet.shouldBlockHoverTips = false;
        }

        var hoverTipSet = NHoverTipSet.CreateAndShow(__instance, hoverTips);
        hoverTipSet.SetAlignmentForCardHolder(__instance);
        return false;
    }

    [HarmonyPostfix]
    public static void Postfix(bool __state)
    {
        if (__state)
            NHoverTipSet.shouldBlockHoverTips = true;
    }
}

[HarmonyPatch]
public static class SelectedCardHoverTipPatch
{
    private static System.Reflection.MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(NSelectedHandCardHolder), "CreateHoverTips");
    }

    [HarmonyPrefix]
    public static bool Prefix(NSelectedHandCardHolder __instance, ref bool __state)
    {
        if (__instance.CardNode == null)
            return false;

        var hoverTips = __instance.CardNode.Model.HoverTips.ToList();
        if (NHoverTipSet.shouldBlockHoverTips)
        {
            __state = true;
            NHoverTipSet.shouldBlockHoverTips = false;
        }

        var hoverTipSet = NHoverTipSet.CreateAndShow(__instance, hoverTips);
        hoverTipSet.SetAlignmentForCardHolder(__instance);
        return false;
    }

    [HarmonyPostfix]
    public static void Postfix(bool __state)
    {
        if (__state)
            NHoverTipSet.shouldBlockHoverTips = true;
    }
}
