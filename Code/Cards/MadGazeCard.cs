using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using FlandreMod.Characters;
using FlandreMod.Keywords;
using FlandreMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace FlandreMod.Cards;

[Pool(typeof(FlandreCharacterCardPool))]
public sealed class MadGazeCard : CustomCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [DestructionEye.CustomType];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<MadnessPower>(1m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MadnessPower>()];

    public override string PortraitPath => "res://images/packed/card_portraits/ironclad/seeing_red.png";
    public override string BetaPortraitPath => PortraitPath;

    public MadGazeCard()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.None)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MadnessPower>(choiceContext, Owner.Creature, DynamicVars["MadnessPower"].BaseValue, Owner.Creature, this);
        await DestructionEyeCardHelper.ApplyToRandomEnemy(choiceContext, this);

        if (IsUpgraded)
            await CardPileCmd.Draw(choiceContext, 1, Owner);
    }

    protected override void OnUpgrade()
    {
    }
}
