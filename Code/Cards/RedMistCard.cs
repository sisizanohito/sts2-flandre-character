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
public sealed class RedMistCard : CustomCardModel
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(2m),
        new BlockVar(6m, ValueProp.Move),
        new DamageVar(3m, ValueProp.Move),
        new IntVar("BloodshedThreshold", 12m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BloodshedPower>()];

    public override string PortraitPath => "res://images/packed/card_portraits/ironclad/seeing_red.png";
    public override string BetaPortraitPath => PortraitPath;

    public RedMistCard()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        VfxCmd.PlayOnCreatureCenter(Owner.Creature, "vfx/vfx_bloody_impact");
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            this);

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        if (CombatState == null || !BloodshedPower.IsAtLeast(Owner.Creature, DynamicVars["BloodshedThreshold"].IntValue))
            return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_bloody_impact")
            .SpawningHitVfxOnEachCreature()
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
