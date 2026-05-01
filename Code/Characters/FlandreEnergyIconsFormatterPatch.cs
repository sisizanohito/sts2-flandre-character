using System;
using System.Linq;
using FlandreMod.Characters;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Localization.Formatters;
using MegaCrit.Sts2.Core.Runs;

namespace FlandreMod.Characters;

[HarmonyPatch(typeof(EnergyIconsFormatter), nameof(EnergyIconsFormatter.TryEvaluateFormat))]
public static class FlandreEnergyIconsFormatterPatch
{
    private const int TextEnergyIconSize = 24;
    private const string TextEnergyIconPath = FlandreTextureHelper.CardEnergySpritePath;

    public static bool Prefix(object formattingInfo, ref bool __result)
    {
        object? currentValue = Traverse.Create(formattingInfo).Property("CurrentValue").GetValue();
        if (currentValue == null || !TryGetCount(currentValue, out int count, out DynamicVar? dynamicVar, out string? colorPrefix))
        {
            return true;
        }

        string? prefix = string.IsNullOrEmpty(colorPrefix)
            ? RunManager.Instance.GetLocalCharacterEnergyIconPrefix()
            : colorPrefix;
        if (!string.Equals(prefix, FlandreTextureHelper.EnergyColorName, StringComparison.Ordinal))
        {
            return true;
        }

        string icon = $"[img={TextEnergyIconSize}x{TextEnergyIconSize}]{TextEnergyIconPath}[/img]";
        string text = count > 0 && count < 4
            ? string.Concat(Enumerable.Repeat(icon, count))
            : dynamicVar == null
                ? $"{count}{icon}"
                : dynamicVar.ToHighlightedString(inverse: false) + icon;

        Traverse.Create(formattingInfo).Method("Write", text).GetValue();
        __result = true;
        return false;
    }

    private static bool TryGetCount(object value, out int count, out DynamicVar? dynamicVar, out string? colorPrefix)
    {
        dynamicVar = null;
        colorPrefix = null;
        switch (value)
        {
            case EnergyVar energyVar:
                count = Convert.ToInt32(energyVar.PreviewValue);
                dynamicVar = energyVar;
                colorPrefix = energyVar.ColorPrefix;
                return true;
            case CalculatedVar calculatedVar:
                count = Convert.ToInt32(calculatedVar.Calculate(null));
                dynamicVar = calculatedVar;
                return true;
            case decimal decimalValue:
                count = (int)decimalValue;
                return true;
            case int intValue:
                count = intValue;
                return true;
            default:
                count = 0;
                return false;
        }
    }
}
