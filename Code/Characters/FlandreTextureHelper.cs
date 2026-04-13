using System;
using System.IO;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace FlandreMod.Characters;

public static class FlandreTextureHelper
{
    public const string CharacterIdSuffix = "FLANDRE_CHARACTER";
    public const string CharacterIconTexturePath = "res://flandremod/Characters/FlandreCharacter/character_icon_flandre_character.png";
    public const string StartupSafeCharacterIconTexturePath = "res://images/ui/top_panel/character_icon_ironclad.png";
    public const string StartupSafeCharacterSelectIconPath = "res://images/packed/character_select/char_select_ironclad.png";
    public const string StartupSafeCharacterSelectLockedIconPath = "res://images/packed/character_select/char_select_ironclad_locked.png";
    public const string CharacterSelectIconPath = "res://flandremod/Characters/FlandreCharacter/char_select_flandre_character.png";
    public const string CharacterSelectLockedIconPath = "res://flandremod/Characters/FlandreCharacter/char_select_flandre_character_locked.png";
    public const string CharacterSelectBackgroundPath = "res://flandremod/Characters/FlandreCharacter/char_select_bg.png";
    public const string CombatTexturePath = "res://flandremod/Characters/FlandreCharacter/flandre_character.png";
    public const string TopbarTexturePath = "res://flandremod/Characters/FlandreCharacter/icon.png";
    public const string MapMarkerTexturePath = "res://flandremod/Characters/FlandreCharacter/map_marker_flandre_character.png";
    public const string StartupSafeMapMarkerTexturePath = "res://images/packed/map/icons/map_marker_ironclad.png";

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

        Texture2D? rawPackedTexture = LoadRawPackedTexture(path);
        if (rawPackedTexture != null)
            return rawPackedTexture;

        string absolutePath = ProjectSettings.GlobalizePath(path);
        if (File.Exists(absolutePath))
        {
            var image = new Image();
            if (image.Load(absolutePath) == Error.Ok)
                return ImageTexture.CreateFromImage(image);
        }

        return null;
    }

    private static Texture2D? LoadRawPackedTexture(string path)
    {
        using Godot.FileAccess? file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
        if (file == null)
            return null;

        byte[] bytes = file.GetBuffer((long)file.GetLength());
        if (bytes.Length == 0)
            return null;

        var image = new Image();
        string extension = Path.GetExtension(path).ToLowerInvariant();
        Error loadResult = extension switch
        {
            ".png" => image.LoadPngFromBuffer(bytes),
            ".jpg" or ".jpeg" => image.LoadJpgFromBuffer(bytes),
            ".webp" => image.LoadWebpFromBuffer(bytes),
            _ => Error.FileUnrecognized
        };

        if (loadResult != Error.Ok)
            return null;

        return ImageTexture.CreateFromImage(image);
    }
}
