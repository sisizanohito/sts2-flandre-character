using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.addons.mega_text;

namespace FlandreMod.Bloodshed;

public partial class NBloodshedCounter : Control
{
    private static readonly Vector2 NormalPosition = new(-20f, 118f);
    private static readonly Vector2 StarsVisiblePosition = new(-20f, 150f);
    private static readonly Vector2 CounterSize = new(168f, 34f);
    private static readonly Color NormalTextColor = new(1f, 0.964706f, 0.886275f, 1f);
    private static readonly Color ZeroTextColor = new(0.95f, 0.32f, 0.32f, 1f);
    private static readonly Color OutlineColor = new(0.34f, 0.02f, 0.035f, 1f);

    private Player? _player;
    private BloodshedState.Counter? _counter;
    private MegaLabel? _label;
    private bool _initialized;
    private bool _disposed;

    private NBloodshedCounter()
    {
    }

    public static NBloodshedCounter Create(Player player)
    {
        NBloodshedCounter counter = new()
        {
            _player = player
        };

        counter.Initialize();
        return counter;
    }

    public override void _Ready()
    {
        Initialize();
    }

    public override void _ExitTree()
    {
        DisposeSubscriptions();
    }

    private void Initialize()
    {
        if (_initialized)
            return;

        _initialized = true;
        Size = CounterSize;
        PivotOffset = CounterSize * 0.5f;
        MouseFilter = MouseFilterEnum.Stop;

        _label = CreateLabel();
        AddChild(_label);

        if (_player != null)
        {
            _counter = BloodshedState.For(_player);
            _counter.Changed += OnBloodshedChanged;
            if (_player.PlayerCombatState != null)
            {
                _player.PlayerCombatState.StarsChanged += OnStarsChanged;
            }
        }

        LocString.SubscribeToLocaleChange(RefreshLabel);
        Connect(Node.SignalName.TreeExiting, Callable.From(DisposeSubscriptions));
        Connect(SignalName.MouseEntered, Callable.From(OnHovered));
        Connect(SignalName.MouseExited, Callable.From(OnUnhovered));

        UpdatePosition();
        RefreshLabel();
    }

    private void DisposeSubscriptions()
    {
        if (_disposed)
            return;

        _disposed = true;
        if (_counter != null)
        {
            _counter.Changed -= OnBloodshedChanged;
        }

        if (_player?.PlayerCombatState != null)
        {
            _player.PlayerCombatState.StarsChanged -= OnStarsChanged;
        }

        LocString.UnsubscribeToLocaleChange(RefreshLabel);
        NHoverTipSet.Remove(this);
    }

    private static MegaLabel CreateLabel()
    {
        MegaLabel label = new()
        {
            Name = "BloodshedLabel",
            AnchorRight = 1f,
            AnchorBottom = 1f,
            GrowHorizontal = (GrowDirection)2,
            GrowVertical = (GrowDirection)2,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            AutoSizeEnabled = true,
            MinFontSize = 14,
            MaxFontSize = 20,
            MouseFilter = MouseFilterEnum.Ignore
        };

        label.AddThemeFontOverride(ThemeConstants.Label.Font, PreloadManager.Cache.GetAsset<Font>("res://themes/kreon_bold_shared.tres"));
        label.AddThemeColorOverride(ThemeConstants.Label.FontColor, NormalTextColor);
        label.AddThemeColorOverride(ThemeConstants.Label.FontShadowColor, new Color(0f, 0f, 0f, 0.25f));
        label.AddThemeColorOverride(ThemeConstants.Label.FontOutlineColor, OutlineColor);
        label.AddThemeConstantOverride(ThemeConstants.Label.OutlineSize, 9);
        label.AddThemeConstantOverride(new StringName("shadow_offset_x"), 2);
        label.AddThemeConstantOverride(new StringName("shadow_offset_y"), 2);
        label.AddThemeFontSizeOverride(ThemeConstants.Label.FontSize, 20);
        return label;
    }

    private void OnBloodshedChanged(int oldValue, int newValue)
    {
        RefreshLabel();
    }

    private void OnStarsChanged(int oldStars, int newStars)
    {
        UpdatePosition();
    }

    private void RefreshLabel()
    {
        if (_label == null)
            return;

        int value = _player == null ? 0 : BloodshedState.CurrentFor(_player);
        string title = new LocString("powers", "BLOODSHED_POWER.title").GetFormattedText();
        _label.AddThemeColorOverride(ThemeConstants.Label.FontColor, value == 0 ? ZeroTextColor : NormalTextColor);
        _label.SetTextAutoSize($"{title}: {value}");
    }

    private void UpdatePosition()
    {
        bool hasStarsVisible = _player?.PlayerCombatState?.Stars > 0;
        Position = hasStarsVisible ? StarsVisiblePosition : NormalPosition;
    }

    private void OnHovered()
    {
        HoverTip hoverTip = new(
            new LocString("powers", "BLOODSHED_POWER.title"),
            new LocString("powers", "BLOODSHED_POWER.description"));

        NHoverTipSet.CreateAndShow(this, hoverTip, HoverTip.GetHoverTipAlignment(this));
    }

    private void OnUnhovered()
    {
        NHoverTipSet.Remove(this);
    }
}
