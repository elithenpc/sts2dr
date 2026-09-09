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

internal sealed partial class TPDisplay : PanelContainer
{
    private readonly Label _label;

    public TPDisplay()
    {
        Name = "DeltaruneTPDisplay";
        MouseFilter = MouseFilterEnum.Ignore;
        ZIndex = 100;
        AnchorLeft = 0f;
        AnchorRight = 0f;
        AnchorTop = 0f;
        AnchorBottom = 0f;
        OffsetLeft = 22f;
        OffsetRight = 190f;
        OffsetTop = 22f;
        OffsetBottom = 68f;

        var panel = new StyleBoxFlat
        {
            BgColor = new Color(0.06f, 0.07f, 0.11f, 0.92f),
            BorderColor = new Color(0.45f, 0.85f, 1f, 0.9f),
            BorderWidthLeft = 2,
            BorderWidthRight = 2,
            BorderWidthTop = 2,
            BorderWidthBottom = 2,
            CornerRadiusTopLeft = 8,
            CornerRadiusTopRight = 8,
            CornerRadiusBottomLeft = 8,
            CornerRadiusBottomRight = 8
        };
        AddThemeStyleboxOverride("panel", panel);

        var margin = new MarginContainer();
        margin.AddThemeConstantOverride("margin_left", 10);
        margin.AddThemeConstantOverride("margin_right", 10);
        margin.AddThemeConstantOverride("margin_top", 5);
        margin.AddThemeConstantOverride("margin_bottom", 5);
        AddChild(margin);

        _label = new Label
        {
            Text = "TP: 0 / 100",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        _label.AddThemeColorOverride("font_color", new Color(0.7f, 0.92f, 1f));
        margin.AddChild(_label);

        TPManager.Changed += Refresh;
        TreeExiting += OnTreeExiting;
    }

    public void Refresh()
    {
        if (!GodotObject.IsInstanceValid(this))
            return;

        _label.Text = $"TP: {TPManager.Current} / {TPManager.MaxTP}";
    }

    private void OnTreeExiting()
    {
        TPManager.Changed -= Refresh;
    }
}
