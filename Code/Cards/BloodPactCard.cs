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

namespace FlandreMod.Cards;

[Pool(typeof(FlandreCharacterCardPool))]
public sealed class BloodPactCard : CustomCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("BloodshedCost", 15m),
        new EnergyVar(2),
        new CardsVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EnergyHoverTip,
        HoverTipFactory.FromPower<BloodshedPower>()
    ];

    public override string PortraitPath => "res://images/packed/card_portraits/ironclad/bloodletting.png";
    public override string BetaPortraitPath => PortraitPath;

    public BloodPactCard()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool consumed = await BloodshedPower.TryConsume(
            choiceContext,
            Owner.Creature,
            DynamicVars["BloodshedCost"].IntValue,
            Owner.Creature,
            this);

        if (!consumed)
            return;

        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
