using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using FlandreMod.Characters;
using FlandreMod.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace FlandreMod.Cards;

[Pool(typeof(FlandreCharacterCardPool))]
public sealed class ShatteredGlintCard : CustomCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [DestructionEye.CustomType];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new IntVar("EyeDamage", 3m)
    ];

    public override string PortraitPath => "res://images/packed/card_portraits/silent/dagger_spray.png";
    public override string BetaPortraitPath => PortraitPath;

    public ShatteredGlintCard()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        Creature? linkedEye = DestructionEyeCardHelper.FindActiveEyeForTarget(this, cardPlay.Target);
        if (linkedEye == null)
            return;

        await CreatureCmd.Damage(
            choiceContext,
            linkedEye,
            DynamicVars["EyeDamage"].BaseValue,
            ValueProp.Move,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["EyeDamage"].UpgradeValueBy(2m);
    }
}
