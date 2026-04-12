using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace FlandreMod.Keywords;

public static class DestructionEye
{
    [CustomEnum("DESTRUCTION_EYE")]
    [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword CustomType;
}
