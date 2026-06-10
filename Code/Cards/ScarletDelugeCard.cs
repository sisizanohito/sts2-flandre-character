using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using FlandreMod.Characters;
using FlandreMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace FlandreMod.Cards;

[Pool(typeof(FlandreCharacterCardPool))]
public sealed class ScarletDelugeCard : CustomCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move),
        new IntVar("BloodshedStep", 5m),
        new IntVar("BonusDamage", 2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BloodshedPower>()];

    public override string PortraitPath => "res://images/packed/card_portraits/ironclad/immolate.png";
    public override string BetaPortraitPath => PortraitPath;

    public ScarletDelugeCard()
        : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;

        int currentBloodshed = BloodshedPower.CurrentFor(Owner.Creature);
        if (currentBloodshed > 0)
        {
            await BloodshedPower.TryConsume(
                choiceContext,
                Owner.Creature,
                currentBloodshed,
                Owner.Creature,
                this);
        }

        int step = DynamicVars["BloodshedStep"].IntValue;
        int tiers = step <= 0 ? 0 : currentBloodshed / step;
        int totalDamage = (int)DynamicVars.Damage.BaseValue + tiers * DynamicVars["BonusDamage"].IntValue;

        await DamageCmd.Attack(totalDamage)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_bloody_impact")
            .SpawningHitVfxOnEachCreature()
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
