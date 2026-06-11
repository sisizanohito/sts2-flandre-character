using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using FlandreMod.Bloodshed;
using FlandreMod.Cards;
using FlandreMod.Powers;
using FlandreMod.Relics;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace FlandreMod.Characters;

public class FlandreCharacter : CustomCharacterModel
{
    public const string CharacterId = "FlandreCharacter";

    public static readonly Color Color = new Color("b01010");

    public override Color NameColor => Color;
    public override Color MapDrawingColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;

    public override int StartingHp => 75;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<FlandreStrikeCard>(),
        ModelDb.Card<FlandreStrikeCard>(),
        ModelDb.Card<FlandreStrikeCard>(),
        ModelDb.Card<FlandreStrikeCard>(),
        ModelDb.Card<FlandreDefendCard>(),
        ModelDb.Card<FlandreDefendCard>(),
        ModelDb.Card<FlandreDefendCard>(),
        ModelDb.Card<FlandreDefendCard>(),
        ModelDb.Card<EchoLinkCard>(),
        ModelDb.Card<EchoLinkCard>(),
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<DestructionEyeRelic>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<FlandreCharacterCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<FlandreCharacterRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<FlandreCharacterPotionPool>();

    public override List<string> GetArchitectAttackVfx() =>
    [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];

    public override string CustomVisualPath => "res://flandremod/Characters/FlandreCharacter/flandre_character.tscn";
    // Use a known-good trail scene so preview cards always complete their fly-out cleanup.
    public override string CustomTrailPath => "res://scenes/vfx/card_trail_ironclad.tscn";
    public override string CustomIconPath => "res://flandremod/Characters/FlandreCharacter/flandre_character_icon.tscn";
    public override string CustomIconTexturePath => FlandreTextureHelper.StartupSafeCharacterIconTexturePath;
    public override string CustomRestSiteAnimPath => "res://flandremod/Characters/FlandreCharacter/flandre_character_rest_site.tscn";
    public override string CustomMerchantAnimPath => "res://flandremod/Characters/FlandreCharacter/flandre_character_merchant.tscn";
    public override string CustomCharacterSelectBg => "res://flandremod/Characters/FlandreCharacter/char_select_bg_flandre_character.tscn";
    public override string CustomCharacterSelectIconPath => FlandreTextureHelper.StartupSafeCharacterSelectIconPath;
    public override string CustomCharacterSelectLockedIconPath => FlandreTextureHelper.StartupSafeCharacterSelectLockedIconPath;
    public override string CustomCharacterSelectTransitionPath => "res://materials/transitions/ironclad_transition_mat.tres";
    public override string CustomMapMarkerPath => FlandreTextureHelper.StartupSafeMapMarkerTexturePath;
    public override string? CustomEnergyCounterPath => "res://scenes/combat/energy_counters/ironclad_energy_counter.tscn";
    protected override IEnumerable<string> ExtraAssetPaths => [FlandreTextureHelper.CardEnergySpritePath];
    public override bool ShouldReceiveCombatHooks => true;

    public override Task BeforeCombatStart()
    {
        MadnessPower.ClearResolutionTracking();
        return Task.CompletedTask;
    }

    public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (delta >= 0m || creature.CombatState == null)
            return Task.CompletedTask;

        int bloodshed = (int)Math.Floor(-delta);
        if (bloodshed <= 0)
            return Task.CompletedTask;

        foreach (Player player in creature.CombatState.Players)
        {
            if (player.Character is FlandreCharacter)
            {
                BloodshedState.Add(player, bloodshed);
            }
        }

        return Task.CompletedTask;
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Character is FlandreCharacter)
        {
            BloodshedState.Reset(player);
        }

        return Task.CompletedTask;
    }
}

public sealed class FlandreCharacterCardPool : CustomCardPoolModel
{
    public override string Title => FlandreCharacter.CharacterId;
    public override float H => 0f;
    public override float S => 1f;
    public override float V => 1f;
    public override Color DeckEntryCardColor => FlandreCharacter.Color;
    public override bool IsColorless => false;
    public override string EnergyColorName => FlandreTextureHelper.EnergyColorName;
    public override string? BigEnergyIconPath => "res://flandremod/Characters/FlandreCharacter/ui/flandre_character_energy_icon.png";
    public override string? TextEnergyIconPath => "res://flandremod/Characters/FlandreCharacter/ui/text_flandre_character_energy_icon.png";
}

public class FlandreCharacterRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => FlandreTextureHelper.EnergyColorName;
    public override string? BigEnergyIconPath => "res://flandremod/Characters/FlandreCharacter/ui/flandre_character_energy_icon.png";
    public override string? TextEnergyIconPath => "res://flandremod/Characters/FlandreCharacter/ui/text_flandre_character_energy_icon.png";
}

public class FlandreCharacterPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => FlandreTextureHelper.EnergyColorName;
    public override string? BigEnergyIconPath => "res://flandremod/Characters/FlandreCharacter/ui/flandre_character_energy_icon.png";
    public override string? TextEnergyIconPath => "res://flandremod/Characters/FlandreCharacter/ui/text_flandre_character_energy_icon.png";
}
