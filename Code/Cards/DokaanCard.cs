using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using FlandreMod.Characters;
using FlandreMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace FlandreMod.Cards;

[Pool(typeof(FlandreCharacterCardPool))]
public sealed class DokaanCard : CustomCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(28m, ValueProp.Move)];

    public override string PortraitPath => "res://images/packed/card_portraits/ironclad/bludgeon.png";
    public override string BetaPortraitPath => PortraitPath;

    public DokaanCard()
        : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool isMadness =
            Owner.Creature.HasPower<MadnessPower>() ||
            MadnessPower.IsResolvingFor(Owner.Creature);

        if (!isMadness)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_explosion")
                .Execute(choiceContext);
            return;
        }

        IReadOnlyList<Creature>? enemies = CombatState?.HittableEnemies;
        if (enemies == null) return;

        foreach (Creature enemy in enemies)
        {
            if (enemy.IsDead) continue;

            await CreatureCmd.Damage(choiceContext, enemy, DynamicVars.Damage, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(8m);
    }
}
