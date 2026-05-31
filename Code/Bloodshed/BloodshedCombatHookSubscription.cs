using System;
using System.Collections.Generic;
using System.Linq;
using FlandreMod.Characters;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace FlandreMod.Bloodshed;

public static class BloodshedCombatHookSubscription
{
    private const string SubscriberId = "flandremod:bloodshed";
    private static bool _isRegistered;

    public static void Register()
    {
        if (_isRegistered)
            return;

        ModHelper.SubscribeForCombatStateHooks(SubscriberId, GetFlandreCharacters);
        _isRegistered = true;
    }

    private static IEnumerable<AbstractModel> GetFlandreCharacters(CombatState combatState)
    {
        if (combatState.Players.All(player => player.Character is not FlandreCharacter))
            return Array.Empty<AbstractModel>();

        return combatState.Players
            .Select(player => player.Character)
            .OfType<FlandreCharacter>()
            .Distinct()
            .Cast<AbstractModel>();
    }
}
