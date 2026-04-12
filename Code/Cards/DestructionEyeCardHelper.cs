using System;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using FlandreMod.Monsters;
using FlandreMod.Powers;
using FlandreMod.Visuals;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace FlandreMod.Cards;

internal static class DestructionEyeCardHelper
{
    private const decimal BaseRelayRatio = 50m;
    private const decimal DeathDamageIncrement = 8m;
    private const int MinimumEyeHp = 5;

    public static async Task ApplyToTarget(CardModel source, Creature target)
    {
        var ownerCreature = source.Owner?.Creature;
        var combatState = source.CombatState;
        if (ownerCreature == null || combatState == null) return;
        if (target.IsDead || target.Monster is LinkedDummyMonsterModel) return;

        Creature? existingEye = combatState.Enemies.FirstOrDefault(e =>
            !e.IsDead &&
            e.Monster is LinkedDummyMonsterModel &&
            e.GetPower<LinkPower>()?.Target == target);

        if (existingEye != null)
        {
            LinkPower? existingLink = existingEye.GetPower<LinkPower>();
            if (existingLink != null)
                await PowerCmd.ModifyAmount(existingLink, DeathDamageIncrement, ownerCreature, source);

            SafePlayOnCreatureCenter(existingEye, "vfx/vfx_explosion");
            SafePlayOnCreatureCenter(target, "vfx/vfx_gaze");
            return;
        }

        Creature eye = await CreatureCmd.Add<LinkedDummyMonsterModel>(combatState);
        await InitializeEyeHp(eye, target);

        try
        {
            EyePlacementHelper.TryPlaceRightOfTarget(eye, target);
        }
        catch (Exception)
        {
            // Visual-only fallback: do not interrupt card resolution.
        }

        var linkPower = (LinkPower)ModelDb.Power<LinkPower>().ToMutable();
        linkPower.Target = target;
        linkPower.RelayRatio = (int)BaseRelayRatio;
        await PowerCmd.Apply(linkPower, eye, amount: DeathDamageIncrement, applier: ownerCreature, cardSource: source);

        SafePlayOnCreatureCenter(target, "vfx/vfx_gaze");
        SafePlayOnCreatureCenter(eye, "vfx/vfx_block");
    }

    public static async Task ApplyToRandomEnemy(CardModel source)
    {
        var runState = source.Owner?.RunState;
        var combatState = source.CombatState;
        if (runState == null || combatState == null) return;

        var enemies = combatState.HittableEnemies
            .Where(enemy => enemy.Monster is not LinkedDummyMonsterModel)
            .ToList();
        if (enemies.Count == 0) return;

        Creature? target = runState.Rng.CombatTargets.NextItem(enemies);
        if (target == null) return;

        await ApplyToTarget(source, target);
    }

    public static async Task ApplyToAllEnemies(CardModel source)
    {
        var combatState = source.CombatState;
        if (combatState == null) return;

        var enemies = combatState.HittableEnemies
            .Where(enemy => enemy.Monster is not LinkedDummyMonsterModel)
            .ToList();

        foreach (Creature enemy in enemies)
            await ApplyToTarget(source, enemy);
    }

    public static bool HasAnyActiveEye(CardModel source)
    {
        var combatState = source.CombatState;
        if (combatState == null) return false;

        return combatState.Enemies.Any(enemy =>
            !enemy.IsDead &&
            enemy.Monster is LinkedDummyMonsterModel &&
            enemy.GetPower<LinkPower>() != null);
    }

    public static Creature? FindActiveEyeForTarget(CardModel source, Creature target)
    {
        var combatState = source.CombatState;
        if (combatState == null) return null;

        return combatState.Enemies.FirstOrDefault(enemy =>
            !enemy.IsDead &&
            enemy.Monster is LinkedDummyMonsterModel &&
            enemy.GetPower<LinkPower>()?.Target == target);
    }

    private static void SafePlayOnCreatureCenter(Creature creature, string path)
    {
        try
        {
            VfxCmd.PlayOnCreatureCenter(creature, path);
        }
        catch (Exception)
        {
            // Visual-only failure should never interrupt card resolution.
        }
    }

    private static async Task InitializeEyeHp(Creature eye, Creature target)
    {
        int desiredMaxHp = Math.Max(MinimumEyeHp, (int)Math.Ceiling(target.MaxHp * 0.05m));
        int extraHp = desiredMaxHp - eye.MaxHp;
        if (extraHp <= 0) return;

        await CreatureCmd.GainMaxHp(eye, extraHp);
        await CreatureCmd.Heal(eye, extraHp);
    }
}
