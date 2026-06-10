using System;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace FlandreMod.Bloodshed;

public partial class NBloodshedClot : Node2D
{
    private const int CircleSegments = 28;
    private const int FullSizeBloodshed = 60;
    private const float MinimumRadius = 11f;
    private const float MaximumRadius = 43f;
    private const float HeadClearance = 34f;
    private static readonly Vector2 FallbackPosition = new(0f, -354f);
    private static readonly Color ShadowColor = new(0.12f, 0f, 0.015f, 0.42f);
    private static readonly Color CoreColor = new(0.47f, 0.015f, 0.035f, 0.88f);
    private static readonly Color DarkLobeColor = new(0.27f, 0f, 0.025f, 0.9f);
    private static readonly Color BrightLobeColor = new(0.72f, 0.04f, 0.055f, 0.7f);
    private static readonly Color GlintColor = new(1f, 0.25f, 0.22f, 0.42f);

    private Player? _player;
    private BloodshedState.Counter? _counter;
    private Tween? _growthTween;
    private Polygon2D? _shadow;
    private Polygon2D? _core;
    private Polygon2D? _leftLobe;
    private Polygon2D? _rightLobe;
    private Polygon2D? _lowerLobe;
    private Polygon2D? _highlight;
    private Polygon2D? _smallDrop;
    private Polygon2D? _leftDrop;
    private Polygon2D? _rightDrop;
    private Polygon2D? _glint;
    private int _value;
    private bool _initialized;
    private bool _disposed;

    private NBloodshedClot()
    {
    }

    public static NBloodshedClot Create(Player player, NCreatureVisuals visuals)
    {
        NBloodshedClot clot = new()
        {
            _player = player,
            Position = CalculatePosition(visuals),
            ZIndex = 20
        };

        clot.Initialize();
        return clot;
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
        Visible = false;
        ProcessMode = ProcessModeEnum.Inherit;
        CreateShapeNodes();

        if (_player != null)
        {
            _counter = BloodshedState.For(_player);
            _counter.Changed += OnBloodshedChanged;
            ApplyValue(_counter.Current, animateGrowth: false);
        }

        Connect(Node.SignalName.TreeExiting, Callable.From(DisposeSubscriptions));
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

        _growthTween?.Kill();
        _growthTween = null;
    }

    private void OnBloodshedChanged(int oldValue, int newValue)
    {
        ApplyValue(newValue, animateGrowth: newValue > oldValue);
    }

    private void ApplyValue(int value, bool animateGrowth)
    {
        _value = Math.Max(0, value);
        Visible = _value > 0;

        if (!Visible)
        {
            _growthTween?.Kill();
            _growthTween = null;
            Scale = Vector2.One;
            return;
        }

        RefreshShape();
        if (animateGrowth)
        {
            _growthTween?.Kill();
            Scale = new Vector2(1.14f, 0.9f);
            _growthTween = CreateTween();
            _growthTween.TweenProperty(this, "scale", Vector2.One, 0.18)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);
        }
    }

    private void CreateShapeNodes()
    {
        _shadow = CreateBlob("Shadow", ShadowColor);
        _core = CreateBlob("Core", CoreColor);
        _leftLobe = CreateBlob("LeftLobe", DarkLobeColor);
        _rightLobe = CreateBlob("RightLobe", CoreColor);
        _lowerLobe = CreateBlob("LowerLobe", DarkLobeColor);
        _highlight = CreateBlob("Highlight", BrightLobeColor);
        _smallDrop = CreateBlob("SmallDrop", DarkLobeColor);
        _leftDrop = CreateBlob("LeftDrop", CoreColor);
        _rightDrop = CreateBlob("RightDrop", BrightLobeColor);
        _glint = CreateBlob("Glint", GlintColor);

        AddChild(_shadow);
        AddChild(_core);
        AddChild(_leftLobe);
        AddChild(_rightLobe);
        AddChild(_lowerLobe);
        AddChild(_highlight);
        AddChild(_smallDrop);
        AddChild(_leftDrop);
        AddChild(_rightDrop);
        AddChild(_glint);
    }

    private void RefreshShape()
    {
        float radius = GetRadius(_value);

        SetCircle(_shadow, new Vector2(2f, 4f), radius * 1.08f);
        SetCircle(_core, Vector2.Zero, radius * 0.9f);
        SetCircle(_leftLobe, new Vector2(-radius * 0.32f, -radius * 0.12f), radius * 0.48f);
        SetCircle(_rightLobe, new Vector2(radius * 0.26f, -radius * 0.08f), radius * 0.42f);
        SetCircle(_lowerLobe, new Vector2(radius * 0.05f, radius * 0.24f), radius * 0.38f);
        SetCircle(_highlight, new Vector2(-radius * 0.16f, -radius * 0.2f), radius * 0.2f);
        SetCircle(_smallDrop, new Vector2(radius * 0.4f, radius * 0.38f), radius * 0.2f);
        SetCircle(_leftDrop, new Vector2(-radius * 0.52f, radius * 0.26f), radius * 0.22f);
        SetCircle(_rightDrop, new Vector2(radius * 0.52f, -radius * 0.2f), radius * 0.18f);
        SetCircle(_glint, new Vector2(-radius * 0.22f, -radius * 0.32f), Math.Max(2.5f, radius * 0.08f));

        SetVisible(_smallDrop, _value >= 15);
        SetVisible(_leftDrop, _value >= 30);
        SetVisible(_rightDrop, _value >= 45);
    }

    private static Vector2 CalculatePosition(NCreatureVisuals visuals)
    {
        Control? bounds = visuals.Bounds;
        if (bounds == null || bounds.Size == Vector2.Zero)
            return FallbackPosition;

        return new Vector2(bounds.Position.X + bounds.Size.X * 0.5f, bounds.Position.Y - HeadClearance);
    }

    private static float GetRadius(int value)
    {
        float t = Mathf.Clamp(value / (float)FullSizeBloodshed, 0f, 1f);
        return Mathf.Lerp(MinimumRadius, MaximumRadius, Mathf.Sqrt(t));
    }

    private static Polygon2D CreateBlob(string name, Color color)
    {
        return new Polygon2D
        {
            Name = name,
            Color = color
        };
    }

    private static void SetCircle(Polygon2D? polygon, Vector2 center, float radius)
    {
        if (polygon == null)
            return;

        Vector2[] points = new Vector2[CircleSegments];
        for (int i = 0; i < CircleSegments; i++)
        {
            float angle = MathF.PI * 2f * i / CircleSegments;
            points[i] = center + new Vector2(MathF.Cos(angle) * radius, MathF.Sin(angle) * radius);
        }

        polygon.Polygon = points;
    }

    private static void SetVisible(Polygon2D? polygon, bool visible)
    {
        if (polygon != null)
        {
            polygon.Visible = visible;
        }
    }
}
