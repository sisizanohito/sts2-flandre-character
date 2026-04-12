using System;
using System.IO;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace FlandreMod.Characters;

public static class FlandreTextureHelper
{
    public const string CharacterIdSuffix = "FLANDRE_CHARACTER";
    public const string CharacterIconTexturePath = "res://flandremod/Characters/FlandreCharacter/character_icon_flandre_character.png";
    public const string CharacterSelectIconPath = "res://flandremod/Characters/FlandreCharacter/char_select_flandre_character.png";
    public const string CharacterSelectLockedIconPath = "res://flandremod/Characters/FlandreCharacter/char_select_flandre_character_locked.png";
    public const string CharacterSelectBackgroundPath = "res://flandremod/Characters/FlandreCharacter/char_select_bg.png";
    public const string CombatTexturePath = "res://flandremod/Characters/FlandreCharacter/flandre_character.png";
    public const string TopbarTexturePath = "res://flandremod/Characters/FlandreCharacter/icon.png";
    public const string MapMarkerTexturePath = "res://flandremod/Characters/FlandreCharacter/map_marker_flandre_character.png";

    public static bool IsFlandre(CharacterModel? character)
    {
        string? idEntry = character?.Id.Entry;
        return !string.IsNullOrEmpty(idEntry) && idEntry.EndsWith(CharacterIdSuffix, StringComparison.Ordinal);
    }

    public static bool IsFlandreCharacterId(string? characterId)
    {
        return !string.IsNullOrEmpty(characterId) && characterId.EndsWith(CharacterIdSuffix, StringComparison.Ordinal);
    }

    public static Texture2D? LoadTexture(string path)
    {
        Texture2D? texture = ResourceLoader.Load<Texture2D>(path, null, ResourceLoader.CacheMode.Reuse);
        if (texture != null)
            return texture;

        string absolutePath = ProjectSettings.GlobalizePath(path);
        if (File.Exists(absolutePath))
        {
            var image = new Image();
            if (image.Load(absolutePath) == Error.Ok)
                return ImageTexture.CreateFromImage(image);
        }

        return null;
    }
}
