using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace FlandreMod.Powers;

public sealed class SanguineGuardPower : CustomPowerModel
{
    private const string BloodshedStepVarName = "BloodshedStep";

    public override string? CustomPackedIconPath => "res://flandremod/images/powers/link_power.png";

    public override string? CustomBigIconPath => "res://flandremod/images/powers/link_power.png";

    public override string? CustomBigBetaIconPath => "res://flandremod/images/powers/link_power.png";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar(BloodshedStepVarName, 10m)];

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != Owner.Side) return;

        int step = DynamicVars[BloodshedStepVarName].IntValue;
        if (step <= 0) return;

        int block = (BloodshedPower.CurrentFor(Owner) / step) * Amount;
        if (block <= 0) return;

        Flash();
        await CreatureCmd.GainBlock(Owner, block, ValueProp.Move, null);
    }
}
