using System;
using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace FlandreMod.Bloodshed;

public static class BloodshedState
{
    private static readonly ConditionalWeakTable<Player, Counter> Counters = new();

    public static Counter For(Player player)
    {
        return Counters.GetValue(player, _ => new Counter());
    }

    public static int CurrentFor(Creature? creature)
    {
        return CurrentFor(creature?.Player);
    }

    public static int CurrentFor(Player? player)
    {
        return player == null ? 0 : For(player).Current;
    }

    public static bool IsAtLeast(Creature? creature, int threshold)
    {
        return CurrentFor(creature) >= threshold;
    }

    public static void Add(Player? player, int amount)
    {
        if (player == null || amount <= 0)
            return;

        Counter counter = For(player);
        counter.Set(counter.Current + amount);
    }

    public static bool TryConsume(Player? player, int amount)
    {
        if (player == null || amount <= 0)
            return false;

        Counter counter = For(player);
        if (counter.Current < amount)
            return false;

        counter.Set(counter.Current - amount);
        return true;
    }

    public static void Reset(Player? player)
    {
        if (player == null)
            return;

        For(player).Set(0);
    }

    public sealed class Counter
    {
        public int Current { get; private set; }

        public event Action<int, int>? Changed;

        internal void Set(int value)
        {
            value = Math.Max(0, value);
            if (Current == value)
                return;

            int previous = Current;
            Current = value;
            Changed?.Invoke(previous, Current);
        }
    }
}
