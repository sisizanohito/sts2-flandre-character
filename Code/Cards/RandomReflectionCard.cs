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
public sealed class RandomReflectionCard : CustomCardModel
{
    private const int HitCount = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MadnessPower>()];

    public override string PortraitPath => "res://images/packed/card_portraits/ironclad/thunderclap.png";
    public override string BetaPortraitPath => PortraitPath;

    public RandomReflectionCard()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.RandomEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null || Owner?.Creature == null)
            return;

        bool spreadToAllEnemies = MadnessPower.IsActiveFor(Owner.Creature);

        for (int i = 0; i < HitCount; i++)
        {
            var attack = DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .WithHitFx("vfx/vfx_attack_slash");

            attack = spreadToAllEnemies
                ? attack.TargetingAllOpponents(CombatState)
                : attack.TargetingRandomOpponents(CombatState, true);

            await attack.Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
