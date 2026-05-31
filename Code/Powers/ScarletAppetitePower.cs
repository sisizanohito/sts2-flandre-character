using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace FlandreMod.Powers;

public sealed class ScarletAppetitePower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://flandremod/images/powers/link_power.png";

    public override string? CustomBigIconPath => "res://flandremod/images/powers/link_power.png";

    public override string? CustomBigBetaIconPath => "res://flandremod/images/powers/link_power.png";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature != Owner) return;
        if (delta >= 0m) return;

        Flash();
        await BloodshedPower.Add(
            new ThrowingPlayerChoiceContext(),
            Owner,
            Amount,
            Owner,
            null,
            silent: true);
    }
}
