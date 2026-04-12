using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using FlandreMod.Characters;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace FlandreMod.Cards;

[Pool(typeof(FlandreCharacterCardPool))]
public sealed class EchoLinkCard : CustomCardModel
{
    public override string PortraitPath => "res://images/packed/card_portraits/ironclad/breakthrough.png";
    public override string BetaPortraitPath => PortraitPath;

    public EchoLinkCard()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        if (cardPlay.Target == null) return;

        int repeatCount = IsUpgraded ? 2 : 1;
        for (int i = 0; i < repeatCount; i++)
            await DestructionEyeCardHelper.ApplyToTarget(this, cardPlay.Target);
    }

    protected override void OnUpgrade()
    {
        // Upgrade is handled directly in OnPlay via the repeat count.
    }
}
