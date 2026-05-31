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
public sealed class CataclysmicGazeCard : CustomCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move),
        new IntVar("EyeDamage", 8m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [DestructionEye.CustomType];

    public override string PortraitPath => "res://flandremod/images/cards/strike_flandre.png";
    public override string BetaPortraitPath => PortraitPath;

    public CataclysmicGazeCard()
        : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;

        IReadOnlyList<Creature> activeEyes = DestructionEyeCardHelper.GetActiveEyesSnapshot(this);
        foreach (Creature eye in activeEyes)
        {
            if (eye.IsDead) continue;

            await CreatureCmd.Damage(
                choiceContext,
                eye,
                DynamicVars["EyeDamage"].BaseValue,
                ValueProp.Move,
                Owner.Creature,
                this);
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_bloody_impact")
            .SpawningHitVfxOnEachCreature()
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["EyeDamage"].UpgradeValueBy(2m);
    }
}
