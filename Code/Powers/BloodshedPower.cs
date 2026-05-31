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

    public override string? CustomPackedIconPath => "res://flandremod/images/powers/link_power.png";

    public override string? CustomBigIconPath => "res://flandremod/images/powers/link_power.png";

    public override string? CustomBigBetaIconPath => "res://flandremod/images/powers/link_power.png";

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

    public static async Task Add(PlayerChoiceContext choiceContext, Creature? creature, int amount, Creature? applier, CardModel? cardSource, bool silent = false)
    {
        if (creature == null || amount <= 0)
            return;

        BloodshedPower? power = creature.GetPower<BloodshedPower>();
        if (power == null)
        {
            power = await PowerCmd.Apply<BloodshedPower>(choiceContext, creature, BaselineAmount, applier, cardSource, silent: true);
            power?.SetAmount(BaselineAmount, silent: true);
        }

        if (power == null)
            return;

        await PowerCmd.ModifyAmount(choiceContext, power, amount, applier, cardSource, silent: silent);
    }

    public static async Task<bool> TryConsume(PlayerChoiceContext choiceContext, Creature? creature, int amount, Creature? applier, CardModel? cardSource)
    {
        BloodshedPower? power = creature?.GetPower<BloodshedPower>();
        if (power == null || power.CurrentBloodshed < amount)
            return false;

        await PowerCmd.ModifyAmount(choiceContext, power, -amount, applier, cardSource);
        return true;
    }

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature.CombatState != Owner.CombatState) return;
        if (delta >= 0m) return;

        int bloodshed = (int)Math.Floor(-delta);
        if (bloodshed <= 0) return;

        Flash();
        await Add(
            new ThrowingPlayerChoiceContext(),
            Owner,
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
