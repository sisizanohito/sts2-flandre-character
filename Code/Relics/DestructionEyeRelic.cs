using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using FlandreMod.Characters;
using FlandreMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;

namespace FlandreMod.Relics;

[Pool(typeof(FlandreCharacterRelicPool))]
public sealed class DestructionEyeRelic : CustomRelicModel
{
    private int _pendingEnergy;
    private bool _triggeredThisTurn;

    public override RelicRarity Rarity => RelicRarity.Starter;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar("Energy", 1)];
    public override string PackedIconPath => "res://images/atlases/relic_atlas.sprites/burning_blood.tres";
    protected override string PackedIconOutlinePath => "res://images/atlases/relic_outline_atlas.sprites/burning_blood.tres";
    protected override string BigIconPath => "res://images/relics/burning_blood.png";

    public override bool IsAllowed(IRunState runState)
    {
        // Starting relic should not appear as a duplicate reward.
        return runState.Players.All(player => player.GetRelicById(Id) == null);
    }

    public override async Task BeforeCombatStart()
    {
        _pendingEnergy = 0;
        _triggeredThisTurn = false;

        BloodshedPower? bloodshedPower = await PowerCmd.Apply<BloodshedPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            1m,
            Owner.Creature,
            null,
            silent: true);
        bloodshedPower?.SetAmount(1, silent: true);
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Creature.Player) return;

        Owner.Creature.GetPower<BloodshedPower>()?.SetAmount(1, silent: true);

        if (_pendingEnergy > 0)
        {
            var granted = _pendingEnergy;
            _pendingEnergy = 0;
            Flash();
            await PlayerCmd.GainEnergy(granted, player);
        }

        _triggeredThisTurn = false;
    }

    public override Task AfterDeath(
        PlayerChoiceContext choiceContext,
        Creature creature,
        bool wasRemovalPrevented,
        float deathAnimLength)
    {
        if (wasRemovalPrevented || _triggeredThisTurn) return Task.CompletedTask;
        if (Owner.Creature.IsDead || creature.Side == Owner.Creature.Side) return Task.CompletedTask;

        var combatState = Owner.Creature.CombatState;
        if (combatState == null || combatState.CurrentSide != Owner.Creature.Side) return Task.CompletedTask;

        _triggeredThisTurn = true;
        _pendingEnergy++;
        Flash();
        return Task.CompletedTask;
    }
}
