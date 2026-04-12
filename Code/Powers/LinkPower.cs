using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace FlandreMod.Powers;

// Applied to the summoned eye. Relays a percentage of damage taken by the eye
// to the linked enemy, and optionally expires after a fixed number of owner-side turns.
public sealed class LinkPower : CustomPowerModel
{
    private const string RelayRatioVarName = "RelayRatio";

    public override string? CustomPackedIconPath => "res://images/powers/link_power.png";

    public override string? CustomBigIconPath => "res://images/powers/link_power.png";

    public override string? CustomBigBetaIconPath => "res://images/powers/link_power.png";

    public decimal DeathDamage
    {
        get => Amount;
        set => SetAmount((int)value);
    }

    public int RelayRatio
    {
        get => DynamicVars[RelayRatioVarName].IntValue;
        set => DynamicVars[RelayRatioVarName].BaseValue = value;
    }

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar(RelayRatioVarName, 50m)];

    // 0 or less means no expiry. Positive values count down at end of Owner side turns.
    public int RemainingTurns { get; set; }

    public override async Task AfterDeath(
        PlayerChoiceContext choiceContext,
        Creature creature,
        bool wasRemovalPrevented,
        float deathAnimLength)
    {
        if (creature == Target)
        {
            if (Owner.IsDead) return;
            await CreatureCmd.Kill(Owner);
            return;
        }

        if (creature != Owner) return;
        if (wasRemovalPrevented) return;

        Creature? linkedEnemy = Target;
        if (linkedEnemy == null || linkedEnemy.IsDead) return;
        if (DeathDamage <= 0) return;

        Flash();
        await CreatureCmd.Damage(
            choiceContext,
            linkedEnemy,
            DeathDamage,
            (ValueProp)0,
            null,
            null
        );
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (RemainingTurns <= 0) return;
        if (side != Owner.Side) return;

        RemainingTurns--;
        if (RemainingTurns <= 0 && !Owner.IsDead)
            await CreatureCmd.Kill(Owner);
    }

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature != Owner) return;
        if (delta >= 0m) return;

        Creature? linkedEnemy = Target;
        if (linkedEnemy == null || linkedEnemy.IsDead) return;

        decimal relayRatio = RelayRatio > 0 ? RelayRatio / 100m : 0.5m;
        decimal relayDamage = Math.Floor((-delta) * relayRatio);
        if (relayDamage <= 0) return;

        Flash();
        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            linkedEnemy,
            relayDamage,
            (ValueProp)0,
            null,
            null
        );
    }
}
