using System;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace FlandreMod.Powers;

public sealed class BloodshedPower : CustomPowerModel
{
    private const int BaselineAmount = 1;

    public override string? CustomPackedIconPath => "res://images/powers/link_power.png";

    public override string? CustomBigIconPath => "res://images/powers/link_power.png";

    public override string? CustomBigBetaIconPath => "res://images/powers/link_power.png";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => CurrentBloodshed;

    public int CurrentBloodshed => Math.Max(0, Amount - BaselineAmount);

    public static int CurrentFor(Creature? creature)
    {
        return creature?.GetPower<BloodshedPower>()?.CurrentBloodshed ?? 0;
    }

    public static bool IsAtLeast(Creature? creature, int threshold)
    {
        return CurrentFor(creature) >= threshold;
    }

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature.CombatState != Owner.CombatState) return;
        if (delta >= 0m) return;

        int bloodshed = (int)Math.Floor(-delta);
        if (bloodshed <= 0) return;

        Flash();
        await PowerCmd.ModifyAmount(
            new ThrowingPlayerChoiceContext(),
            this,
            bloodshed,
            null,
            null,
            silent: true);
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return Task.CompletedTask;

        SetAmount(BaselineAmount, silent: true);
        return Task.CompletedTask;
    }
}
