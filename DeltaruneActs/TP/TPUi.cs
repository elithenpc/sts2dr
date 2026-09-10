using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace DeltaruneActs.TP;

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
internal static class TPCombatUiPatch
{
    internal static readonly Dictionary<NCombatUi, TPDisplay> Displays = new();

    private static void Postfix(NCombatUi __instance, CombatState state)
    {
        if (Displays.TryGetValue(__instance, out var existing) && GodotObject.IsInstanceValid(existing))
            existing.QueueFree();

        var display = new TPDisplay();
        Displays[__instance] = display;
        __instance.AddChild(display);
        display.Refresh();
    }
}

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Deactivate))]
internal static class TPCombatUiDeactivatePatch
{
    private static void Postfix(NCombatUi __instance)
    {
        if (TPCombatUiPatch.Displays.TryGetValue(__instance, out var display) && GodotObject.IsInstanceValid(display))
            display.QueueFree();

        TPCombatUiPatch.Displays.Remove(__instance);
    }
}

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi._ExitTree))]
internal static class TPCombatUiExitTreePatch
{
    private static void Postfix(NCombatUi __instance)
    {
        if (TPCombatUiPatch.Displays.TryGetValue(__instance, out var display) && GodotObject.IsInstanceValid(display))
            display.QueueFree();

        TPCombatUiPatch.Displays.Remove(__instance);
    }
}

internal sealed partial class TPDisplay : Control
{
    private const string SpritePath = "res://DeltaruneActs/images/card_portraits/tp_kris.png";

    private readonly TPMeter _meter;
    private readonly Label _percentLabel;

    public TPDisplay()
    {
        Name = "DeltaruneTPDisplay";
        MouseFilter = MouseFilterEnum.Ignore;
        ZIndex = 100;

        // Deltarune's TP meter is a tall vertical gauge on the left side.
        AnchorLeft = 0f;
        AnchorRight = 0f;
        AnchorTop = 0.5f;
        AnchorBottom = 0.5f;
        OffsetLeft = 8f;
        OffsetRight = 118f;
        OffsetTop = -185f;
        OffsetBottom = 185f;

        _meter = new TPMeter
        {
            Position = new Vector2(54f, 36f),
            Size = new Vector2(26f, 288f)
        };
        AddChild(_meter);

        var tpLabel = new Label
        {
            Text = "TP",
            Position = new Vector2(2f, 128f),
            Size = new Vector2(46f, 28f),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        tpLabel.AddThemeFontSizeOverride("font_size", 17);
        tpLabel.AddThemeColorOverride("font_color", Colors.White);
        tpLabel.AddThemeColorOverride("font_shadow_color", Colors.Black);
        tpLabel.AddThemeConstantOverride("shadow_offset_x", 2);
        tpLabel.AddThemeConstantOverride("shadow_offset_y", 2);
        AddChild(tpLabel);

        _percentLabel = new Label
        {
            Text = "0%",
            Position = new Vector2(0f, 154f),
            Size = new Vector2(50f, 30f),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        _percentLabel.AddThemeFontSizeOverride("font_size", 17);
        _percentLabel.AddThemeColorOverride("font_color", Colors.White);
        _percentLabel.AddThemeColorOverride("font_shadow_color", Colors.Black);
        _percentLabel.AddThemeConstantOverride("shadow_offset_x", 2);
        _percentLabel.AddThemeConstantOverride("shadow_offset_y", 2);
        AddChild(_percentLabel);

        // Use a real Deltarune battle sprite as the TP ornament.
        var sprite = new TextureRect
        {
            Texture = GD.Load<Texture2D>(SpritePath),
            Position = new Vector2(76f, 6f),
            Size = new Vector2(34f, 50f),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            MouseFilter = MouseFilterEnum.Ignore
        };
        AddChild(sprite);

        TPManager.Changed += Refresh;
        TreeExiting += OnTreeExiting;
    }

    public void Refresh()
    {
        if (!GodotObject.IsInstanceValid(this))
            return;

        var current = Math.Clamp(TPManager.Current, 0, TPManager.MaxTP);
        _percentLabel.Text = $"{current}%";
        _meter.Value = current;
        _meter.QueueRedraw();
    }

    private void OnTreeExiting()
    {
        TPManager.Changed -= Refresh;
    }
}

internal sealed partial class TPMeter : Control
{
    public int Value { get; set; }

    public override void _Draw()
    {
        const float width = 22f;
        const float height = 286f;
        const float inset = 4f;

        var outer = new Rect2(0f, 0f, width, height);
        DrawRect(outer, new Color(0.03f, 0.03f, 0.04f, 1f), true);
        DrawRect(outer, new Color(0.95f, 0.95f, 0.95f, 1f), false, 2f);

        var inner = new Rect2(inset, inset, width - inset * 2f, height - inset * 2f);
        DrawRect(inner, new Color(0.15f, 0.06f, 0.02f, 1f), true);

        var amount = Math.Clamp(Value, 0, 100) / 100f;
        var fillHeight = inner.Size.Y * amount;
        if (fillHeight > 0f)
        {
            var fill = new Rect2(inner.Position.X, inner.End.Y - fillHeight, inner.Size.X, fillHeight);
            DrawRect(fill, new Color(1f, 0.58f, 0.05f, 1f), true);

            var edgeY = inner.End.Y - fillHeight;
            DrawLine(new Vector2(inner.Position.X, edgeY), new Vector2(inner.End.X, edgeY), Colors.White, 1f);
        }
    }
}
