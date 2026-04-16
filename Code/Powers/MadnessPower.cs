using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace FlandreMod.Powers;

public sealed class MadnessPower : CustomPowerModel
{
    private static readonly HashSet<Creature> ResolvingOwners = [];

    public override string? CustomPackedIconPath => "res://images/powers/madness_power.png";

    public override string? CustomBigIconPath => "res://images/powers/madness_power.png";

    public override string? CustomBigBetaIconPath => "res://images/powers/madness_power.png";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public static bool IsResolvingFor(Creature creature)
    {
        return ResolvingOwners.Contains(creature);
    }

    public static bool IsActiveFor(Creature? creature)
    {
        if (creature == null)
            return false;

        return creature.HasPower<MadnessPower>() || IsResolvingFor(creature);
    }

    public bool Affects(CardModel card)
    {
        if (card.Owner?.Creature != Owner) return false;
        return card.Type == CardType.Attack;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (!cardPlay.IsFirstInSeries) return;
        if (cardPlay.IsAutoPlay) return;
        if (!Affects(cardPlay.Card)) return;

        ResolvingOwners.Add(Owner);
        await PowerCmd.Decrement(this);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (!cardPlay.IsLastInSeries) return Task.CompletedTask;
        if (!Affects(cardPlay.Card)) return Task.CompletedTask;

        ResolvingOwners.Remove(Owner);
        return Task.CompletedTask;
    }

}
