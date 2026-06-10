using System;
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
public sealed class SanguineOverflowCard : CustomCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new IntVar("BloodshedStep", 10m),
        new IntVar("BonusDamage", 2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BloodshedPower>()];

    public override string PortraitPath => "res://images/packed/card_portraits/ironclad/pummel.png";
    public override string BetaPortraitPath => PortraitPath;

    public SanguineOverflowCard()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        int step = DynamicVars["BloodshedStep"].IntValue;
        int tiers = step <= 0 ? 0 : BloodshedPower.CurrentFor(Owner.Creature) / step;
        int totalDamage = (int)DynamicVars.Damage.BaseValue + tiers * DynamicVars["BonusDamage"].IntValue;

        await DamageCmd.Attack(totalDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_bloody_impact")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BonusDamage"].UpgradeValueBy(1m);
    }
}
