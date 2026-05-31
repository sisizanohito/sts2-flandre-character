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
public sealed class ScarletAppetiteCard : CustomCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ScarletAppetitePower>(2m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BloodshedPower>(),
        HoverTipFactory.FromPower<ScarletAppetitePower>()
    ];

    public override string PortraitPath => "res://images/packed/card_portraits/ironclad/rupture.png";
    public override string BetaPortraitPath => PortraitPath;

    public ScarletAppetiteCard()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ScarletAppetitePower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["ScarletAppetitePower"].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ScarletAppetitePower"].UpgradeValueBy(1m);
    }
}
