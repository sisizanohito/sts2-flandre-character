using System;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using FlandreMod.Bloodshed;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace FlandreMod.Powers;

public sealed class BloodshedPower : CustomPowerModel
{
    private const int BaselineAmount = 1;

    public override string? CustomPackedIconPath => "res://flandremod/images/powers/link_power.png";

    public override string? CustomBigIconPath => "res://flandremod/images/powers/link_power.png";

    public override string? CustomBigBetaIconPath => "res://flandremod/images/powers/link_power.png";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => CurrentBloodshed;

    public int CurrentBloodshed => Math.Max(0, Amount - BaselineAmount);

    public static int CurrentFor(Creature? creature)
    {
        return BloodshedState.CurrentFor(creature);
    }

    public static bool IsAtLeast(Creature? creature, int threshold)
    {
        return BloodshedState.IsAtLeast(creature, threshold);
    }

    public static Task Add(PlayerChoiceContext choiceContext, Creature? creature, int amount, Creature? applier, CardModel? cardSource, bool silent = false)
    {
        BloodshedState.Add(creature?.Player, amount);
        return Task.CompletedTask;
    }

    public static Task<bool> TryConsume(PlayerChoiceContext choiceContext, Creature? creature, int amount, Creature? applier, CardModel? cardSource)
    {
        return Task.FromResult(BloodshedState.TryConsume(creature?.Player, amount));
    }
}
