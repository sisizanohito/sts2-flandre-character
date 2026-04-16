using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using FlandreMod.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace FlandreMod.Cards;

[HarmonyPatch]
public static class MadnessTargetTypePatch
{
    [HarmonyTargetMethods]
    public static IEnumerable<MethodBase> TargetMethods()
    {
        return AccessTools.AllTypes()
            .Where(type => type != null && typeof(CardModel).IsAssignableFrom(type))
            .Select(type => AccessTools.PropertyGetter(type, nameof(CardModel.TargetType)))
            .Where(method => method != null)!;
    }

    [HarmonyPostfix]
    public static void Postfix(CardModel __instance, ref TargetType __result)
    {
        if (__instance.Type != CardType.Attack) return;
        if (!__instance.IsMutable) return;

        var owner = __instance.Owner;
        if (!MadnessPower.IsActiveFor(owner?.Creature)) return;

        if (__result == TargetType.AnyEnemy || __result == TargetType.RandomEnemy)
            __result = TargetType.AllEnemies;
    }
}

[HarmonyPatch(typeof(CardModel), "OnPlayWrapper")]
public static class MadnessOnPlayWrapperPatch
{
    [HarmonyPrefix]
    public static void Prefix(CardModel __instance, ref Creature? target)
    {
        if (__instance.Type != CardType.Attack) return;
        if (target != null) return;

        var owner = __instance.Owner;
        var combatState = __instance.CombatState;
        if (combatState == null) return;
        if (!MadnessPower.IsActiveFor(owner?.Creature)) return;

        target = combatState.HittableEnemies.FirstOrDefault();
    }
}

[HarmonyPatch(typeof(AttackCommand), nameof(AttackCommand.Targeting))]
public static class MadnessAttackCommandTargetingPatch
{
    [HarmonyPrefix]
    public static bool Prefix(AttackCommand __instance, Creature? target, ref AttackCommand __result)
    {
        if (__instance.ModelSource is not CardModel card) return true;
        if (card.Type != CardType.Attack) return true;

        var owner = card.Owner;
        if (!MadnessPower.IsActiveFor(owner?.Creature)) return true;

        var combatState = __instance.Attacker?.CombatState ?? owner.Creature.CombatState;
        if (combatState == null) return true;

        __result = __instance.TargetingAllOpponents(combatState);
        return false;
    }
}

[HarmonyPatch]
public static class MadnessAttackCommandRandomTargetingPatch
{
    [HarmonyTargetMethod]
    public static MethodBase? TargetMethod()
    {
        return AccessTools.Method(
            typeof(AttackCommand),
            nameof(AttackCommand.TargetingRandomOpponents),
            [typeof(CombatState), typeof(bool)]);
    }

    [HarmonyPrefix]
    public static bool Prefix(AttackCommand __instance, CombatState combatState, bool allowDuplicates, ref AttackCommand __result)
    {
        if (__instance.ModelSource is not CardModel card) return true;
        if (card.Type != CardType.Attack) return true;

        var owner = card.Owner;
        if (!MadnessPower.IsActiveFor(owner?.Creature)) return true;

        __result = __instance.TargetingAllOpponents(combatState);
        return false;
    }
}

[HarmonyPatch]
public static class MadnessAttackCommandExecutePatch
{
    private static readonly FieldInfo? RandomTargetingField =
        AccessTools.Field(typeof(AttackCommand), "<IsRandomlyTargeted>k__BackingField");

    [HarmonyTargetMethod]
    public static MethodBase? TargetMethod()
    {
        return AccessTools.Method(typeof(AttackCommand), nameof(AttackCommand.Execute));
    }

    [HarmonyPrefix]
    public static void Prefix(AttackCommand __instance)
    {
        if (__instance.ModelSource is not CardModel card) return;
        if (card.Type != CardType.Attack) return;

        var owner = card.Owner;
        if (owner?.Creature == null) return;
        if (!__instance.IsRandomlyTargeted) return;
        if (!MadnessPower.IsActiveFor(owner.Creature)) return;
        if (RandomTargetingField == null) return;

        RandomTargetingField.SetValue(__instance, false);
    }
}
