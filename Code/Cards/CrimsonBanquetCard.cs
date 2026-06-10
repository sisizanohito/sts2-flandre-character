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
public sealed class CrimsonBanquetCard : CustomCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("BloodshedCost", 25m),
        new DamageVar(22m, ValueProp.Move),
        new IntVar("Heal", 5m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BloodshedPower>()];

    public override string PortraitPath => "res://images/packed/card_portraits/ironclad/reaper.png";
    public override string BetaPortraitPath => PortraitPath;

    public CrimsonBanquetCard()
        : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        bool consumed = await BloodshedPower.TryConsume(
            choiceContext,
            Owner.Creature,
            DynamicVars["BloodshedCost"].IntValue,
            Owner.Creature,
            this);

        if (!consumed)
            return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_bloody_impact")
            .Execute(choiceContext);

        await CreatureCmd.Heal(Owner.Creature, DynamicVars["Heal"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(8m);
        DynamicVars["Heal"].UpgradeValueBy(2m);
    }
}
