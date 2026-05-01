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
public sealed class CrimsonAdvanceCard : CustomCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(3m),
        new CardsVar(2),
        new IntVar("BloodshedThreshold", 10m),
        new EnergyVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EnergyHoverTip,
        HoverTipFactory.FromPower<BloodshedPower>()
    ];

    public override string PortraitPath => "res://images/packed/card_portraits/ironclad/bloodletting.png";
    public override string BetaPortraitPath => PortraitPath;

    public CrimsonAdvanceCard()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
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

        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);

        if (!BloodshedPower.IsAtLeast(Owner.Creature, DynamicVars["BloodshedThreshold"].IntValue))
            return;

        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.HpLoss.UpgradeValueBy(-1m);
    }
}
