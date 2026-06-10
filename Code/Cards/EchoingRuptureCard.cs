using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using FlandreMod.Characters;
using FlandreMod.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace FlandreMod.Cards;

[Pool(typeof(FlandreCharacterCardPool))]
public sealed class EchoingRuptureCard : CustomCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [DestructionEye.CustomType];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move)];

    public override string PortraitPath => "res://images/packed/card_portraits/ironclad/sever_soul.png";
    public override string BetaPortraitPath => PortraitPath;

    public EchoingRuptureCard()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        bool hadActiveEye = DestructionEyeCardHelper.HasAnyActiveEye(this);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (!hadActiveEye)
            return;

        await DestructionEyeCardHelper.ApplyToRandomEnemy(choiceContext, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
