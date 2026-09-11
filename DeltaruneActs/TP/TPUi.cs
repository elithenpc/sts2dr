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
    private readonly TPMeter _meter;
    private readonly Label _tpLabel;
    private readonly Label _percentLabel;

    public TPDisplay()
    {
        Name = "DeltaruneTPDisplay";
        MouseFilter = MouseFilterEnum.Ignore;
        ZIndex = 100;

        // Deltarune places the large TP meter on the left edge of the battle UI.
        AnchorLeft = 0f;
        AnchorRight = 0f;
        AnchorTop = 0.5f;
        AnchorBottom = 0.5f;
        OffsetLeft = 10f;
        OffsetRight = 106f;
        OffsetTop = -182f;
        OffsetBottom = 182f;

        _meter = new TPMeter
        {
            Position = new Vector2(60f, 18f),
            Size = new Vector2(22f, 328f)
        };
        AddChild(_meter);

        _tpLabel = MakeLabel("TP", 20);
        _tpLabel.Position = new Vector2(0f, 106f);
        _tpLabel.Size = new Vector2(52f, 34f);
        AddChild(_tpLabel);

        _percentLabel = MakeLabel("0%", 20);
        _percentLabel.Position = new Vector2(0f, 142f);
        _percentLabel.Size = new Vector2(52f, 36f);
        AddChild(_percentLabel);

        TPManager.Changed += Refresh;
        TreeExiting += OnTreeExiting;
    }

    private static Label MakeLabel(string text, int fontSize)
    {
        var label = new Label
        {
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            MouseFilter = MouseFilterEnum.Ignore
        };
        label.AddThemeFontSizeOverride("font_size", fontSize);
        label.AddThemeColorOverride("font_color", Colors.White);
        label.AddThemeColorOverride("font_shadow_color", Colors.Black);
        label.AddThemeConstantOverride("shadow_offset_x", 2);
        label.AddThemeConstantOverride("shadow_offset_y", 2);
        return label;
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
        const float height = 328f;

        // Pixel-style outer silhouette.
        var outer = new Rect2(0f, 0f, width, height);
        DrawRect(outer, Colors.Black, true);

        // Thin pale frame used by the battle meter.
        DrawRect(new Rect2(2f, 2f, width - 4f, height - 4f), new Color(0.82f, 0.82f, 0.76f, 1f), true);
        DrawRect(new Rect2(4f, 4f, width - 8f, height - 8f), new Color(0.12f, 0.09f, 0.07f, 1f), true);

        const float inset = 5f;
        var inner = new Rect2(inset, inset, width - inset * 2f, height - inset * 2f);

        // Empty portion stays dark. TP fills from the bottom upward.
        DrawRect(inner, new Color(0.18f, 0.16f, 0.13f, 1f), true);

        var amount = Math.Clamp(Value, 0, 100) / 100f;
        var fillHeight = inner.Size.Y * amount;
        if (fillHeight <= 0f)
            return;

        var fill = new Rect2(inner.Position.X, inner.End.Y - fillHeight, inner.Size.X, fillHeight);

        // Deltarune-style orange/yellow TP fill.
        DrawRect(fill, new Color(1f, 0.58f, 0.08f, 1f), true);

        // Bright top edge gives the fill its crisp pixel silhouette.
        var edgeY = inner.End.Y - fillHeight;
        DrawLine(
            new Vector2(inner.Position.X, edgeY),
            new Vector2(inner.End.X, edgeY),
            Colors.White,
            1f
        );
    }
}
