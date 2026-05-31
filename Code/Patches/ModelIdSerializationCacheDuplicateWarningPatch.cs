using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

namespace FlandreMod.Patches;

[HarmonyPatch]
internal static class ModelIdSerializationCacheDuplicateWarningPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        Type? comparerType = typeof(ModelIdSerializationCache).GetNestedType("<>c", BindingFlags.NonPublic);
        MethodInfo? target = comparerType?.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(IsModelIdComparer);

        if (target != null)
        {
            yield return target;
        }
    }

    private static bool IsModelIdComparer(MethodInfo method)
    {
        if (method.ReturnType != typeof(int))
        {
            return false;
        }

        ParameterInfo[] parameters = method.GetParameters();
        return parameters.Length == 2
            && IsModelModTuple(parameters[0].ParameterType)
            && IsModelModTuple(parameters[1].ParameterType);
    }

    private static bool IsModelModTuple(Type type)
    {
        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(ValueTuple<,>))
        {
            return false;
        }

        Type[] arguments = type.GetGenericArguments();
        return arguments[0] == typeof(Type) && arguments[1] == typeof(Mod);
    }

    private static bool Prefix((Type Type, Mod? Mod) p1, (Type Type, Mod? Mod) p2, ref int __result)
    {
        Type type1 = p1.Type;
        Type type2 = p2.Type;
        Mod? mod1 = p1.Mod;
        Mod? mod2 = p2.Mod;

        int nameComparison = string.CompareOrdinal(type1.Name, type2.Name);
        if (nameComparison != 0)
        {
            __result = nameComparison;
            return false;
        }

        if (mod1 != null && mod2 == null)
        {
            __result = 1;
            return false;
        }

        if (mod1 == null && mod2 != null)
        {
            __result = -1;
            return false;
        }

        if (mod1 == null && mod2 == null)
        {
            __result = 0;
            return false;
        }

        string? modId1 = mod1!.manifest?.id;
        string? modId2 = mod2!.manifest?.id;

        int modComparison = string.CompareOrdinal(modId1, modId2);
        if (modComparison == 0 && !ReferenceEquals(type1, type2))
        {
            Log.Warn($"Two AbstractModels {type1} and {type2} from mod {modId1} share an ID! This might break multiplayer.");
        }

        __result = modComparison;
        return false;
    }
}
