using System;
using Imui.Rendering;
using UnityEngine;

namespace Imui.Style
{
    public static class ImStyleSheetBuilder
    {
        private const float LAYER_DELTA = 5.0f;
        private const float BORDER_LIGHTNESS_DELTA = -15.0f;
        private const float HOVERED_LIGHTNESS_DELTA = +3.0f;
        private const float PRESSED_LIGHTNESS_DELTA = -1.5f;

        private struct ThemeContext
        {
            public float Contrast;
            public float BorderContrast;
            public bool IsDark;
        }

        private static Color Ascend(this Color color, ThemeContext context) => color.ChangeLightness(context, LAYER_DELTA);
        private static Color HalfAscend(this Color color, ThemeContext context) => color.ChangeLightness(context, LAYER_DELTA / 2.0f);
        private static Color Descend(this Color color, ThemeContext context) => color.ChangeLightness(context, -LAYER_DELTA);
        private static Color ToHovered(this Color color, ThemeContext context) => color.ChangeLightness(context, HOVERED_LIGHTNESS_DELTA);
        private static Color ToPressed(this Color color, ThemeContext context) => color.ChangeLightness(context, PRESSED_LIGHTNESS_DELTA);
        private static Color Opaque(this Color color) => color.WithAlpha(1.0f);

        private static Color ToBorder(this Color color, ThemeContext context)
        {
            var delta = context.IsDark ? -BORDER_LIGHTNESS_DELTA : BORDER_LIGHTNESS_DELTA;
            delta *= (context.BorderContrast + 1);
            return color.ChangeLightness(context, delta);
        }

        private static Color EnsureLightnessDistance(this Color mainColor, Color otherColor, float minDistance = 20.0f)
        {
            var mainHcl = mainColor.ToHcl();
            var otherHcl = otherColor.ToHcl();
            var diff = mainHcl.Lightness - otherHcl.Lightness;

            if (Math.Abs(diff) < minDistance)
            {
                return mainHcl.SetLightness(diff > 0 ? otherHcl.Lightness + minDistance : otherHcl.Lightness - minDistance);
            }

            return mainColor;
        }

        private static Color ChangeLightness(this Color color, ThemeContext context, float percent)
        {
            return color.ToHcl().AddLightness(percent * (context.Contrast + 1)).ToRgb();
        }
        
        private static float GetBrightness(this Color color)
        {
            return 0.2125f * color.r + 0.7152f * color.g + 0.0722f * color.b;
        }

        public static ImThemePalette BuildPalette(in ImTheme theme)
        {
            var palette = new ImThemePalette();

            palette.IsDark = theme.Background.GetBrightness() < 0.5f;
            palette.Back = theme.Background;
            palette.Front = theme.Foreground;
            palette.Control = Color.Lerp(palette.IsDark ? theme.Background.ToHcl().AddLightness(+15.0f).ToRgb() : theme.Background.ToHcl().AddLightness(+5.0f).ToRgb(),
                                     theme.Control,
                                     theme.Control.a);
            palette.Control.a = 1.0f;
            palette.Accent = theme.Accent;
            palette.AccentFront = theme.Accent.GetBrightness() > 0.5f ? Color.black : Color.white;

            return palette;
        }

        public static ImStyleSheet BuildStyleSheet(in ImTheme theme)
        {
            var palette = BuildPalette(in theme);
            var sheet = BuildStyleSheet(in theme, in palette);

            return sheet;
        }

        public static ImStyleSheet BuildStyleSheet(in ImTheme theme, in ImThemePalette palette)
        {
            var sheet = new ImStyleSheet();
            var ctx = new ThemeContext()
            {
                Contrast = theme.Contrast,
                BorderContrast = theme.BorderContrast,
                IsDark = palette.IsDark
            };
            
            // global

            sheet.Global.ReadOnlyModifier = theme.ReadOnlyColorMultiplier;
            
            // text

            sheet.Text.Color = palette.Front;

            // layout

            sheet.Layout.ExtraRowHeight = theme.ExtraRowHeight;
            sheet.Layout.Spacing = theme.Spacing;
            sheet.Layout.InnerSpacing = theme.InnerSpacing;
            sheet.Layout.TextSize = theme.TextSize;
            sheet.Layout.Indent = theme.Indent;

            // button

            sheet.Button.Alignment = new ImAlignment(0.5f, 0.5f);
            sheet.Button.BorderRadius = theme.BorderRadius;
            sheet.Button.BorderThickness = theme.BorderThickness;

            sheet.Button.Normal.BackColor = palette.Control;
            sheet.Button.Normal.FrontColor = palette.Front;
            sheet.Button.Normal.BorderColor = palette.Control.ToBorder(ctx);

            sheet.Button.Hovered.BackColor = palette.Control.ToHovered(ctx);
            sheet.Button.Hovered.FrontColor = palette.Front.ToHovered(ctx);
            sheet.Button.Hovered.BorderColor = palette.Control.ToHovered(ctx).ToBorder(ctx);

            sheet.Button.Pressed.BackColor = palette.Control.ToPressed(ctx);
            sheet.Button.Pressed.FrontColor = palette.Front.ToPressed(ctx);
            sheet.Button.Pressed.BorderColor = palette.Control.ToPressed(ctx).ToBorder(ctx);

            // window

            sheet.Window.ContentPadding = theme.Spacing;

            sheet.Window.ResizeHandleNormalColor = palette.Back.ToBorder(ctx);
            sheet.Window.ResizeHandleActiveColor = palette.Accent;
            sheet.Window.ResizeHandleSize = Mathf.Max(theme.BorderRadius * 2.0f, (theme.TextSize + theme.ExtraRowHeight) * 1.25f);

            sheet.Window.Box.BackColor = palette.Back;
            sheet.Window.Box.FrontColor = palette.Front;
            sheet.Window.Box.BorderRadius = theme.WindowBorderRadius;
            sheet.Window.Box.BorderThickness = theme.WindowBorderThickness;
            sheet.Window.Box.BorderColor = palette.Back.ToBorder(ctx).Opaque();

            sheet.Window.TitleBar.BackColor = palette.Back.Ascend(ctx).HalfAscend(ctx).Opaque();
            sheet.Window.TitleBar.FrontColor = palette.Front;
            sheet.Window.TitleBar.Alignment = new ImAlignment(0.5f, 0.5f);
            sheet.Window.TitleBar.Overflow = ImTextOverflow.Ellipsis;

            sheet.Window.TitleBar.CloseButton = sheet.Button;
            sheet.Window.TitleBar.CloseButton.BorderRadius = 999.9f;

            sheet.Window.TitleBar.CloseButton.Hovered.BackColor = palette.Accent.ToHovered(ctx);
            sheet.Window.TitleBar.CloseButton.Hovered.FrontColor = palette.AccentFront;
            sheet.Window.TitleBar.CloseButton.Hovered.BorderColor = palette.Accent.ToHovered(ctx).ToBorder(ctx);

            sheet.Window.TitleBar.CloseButton.Pressed.BackColor = palette.Accent.ToPressed(ctx);
            sheet.Window.TitleBar.CloseButton.Pressed.FrontColor = palette.AccentFront.ToPressed(ctx);
            sheet.Window.TitleBar.CloseButton.Pressed.BorderColor = palette.Accent.ToPressed(ctx).ToBorder(ctx);

            // text edit

            sheet.TextEdit.Normal.SelectionColor = palette.Accent.WithAlpha(0.25f);
            sheet.TextEdit.Normal.Box.BackColor = palette.Control;
            sheet.TextEdit.Normal.Box.FrontColor = palette.Front;
            sheet.TextEdit.Normal.Box.BorderColor = palette.Control.ToBorder(ctx);
            sheet.TextEdit.Normal.Box.BorderRadius = theme.BorderRadius / 2.0f;
            sheet.TextEdit.Normal.Box.BorderThickness = theme.BorderThickness;

            sheet.TextEdit.Selected.SelectionColor = sheet.TextEdit.Normal.SelectionColor;
            sheet.TextEdit.Selected.Box.BackColor = palette.Control.ToHovered(ctx);
            sheet.TextEdit.Selected.Box.FrontColor = palette.Front;
            sheet.TextEdit.Selected.Box.BorderColor = palette.Accent;
            sheet.TextEdit.Selected.Box.BorderRadius = theme.BorderRadius / 2.0f;
            sheet.TextEdit.Selected.Box.BorderThickness = theme.BorderThickness;

            sheet.TextEdit.CaretWidth = 2.0f;
            sheet.TextEdit.Alignment = new ImAlignment(0.0f, 0.0f);
            sheet.TextEdit.TextWrap = false;

            // scroll bar

            var barBack = palette.Control;
            var barBorder = (Color)sheet.Button.Normal.BorderColor;
            var barFront = barBorder.Ascend(ctx);

            sheet.Scroll.Size = (int)theme.ScrollBarSize;
            sheet.Scroll.BorderThickness = theme.BorderThickness;
            sheet.Scroll.HandlePadding = 1.0f;
            sheet.Scroll.BorderRadius = theme.BorderRadius;
            sheet.Scroll.MinHandleAspect = 2.0f;
            sheet.Scroll.VMargin = new ImPadding(theme.Spacing, 0, 0, 0);
            sheet.Scroll.HMargin = new ImPadding(0, 0, theme.Spacing, 0);

            sheet.Scroll.NormalState.BackColor = barBack;
            sheet.Scroll.NormalState.FrontColor = barFront.EnsureLightnessDistance(sheet.Scroll.NormalState.BackColor);
            sheet.Scroll.NormalState.BorderColor = barBorder;

            sheet.Scroll.HoveredState.BackColor = barBack;
            sheet.Scroll.HoveredState.FrontColor = palette.Accent.EnsureLightnessDistance(sheet.Scroll.HoveredState.BackColor);
            sheet.Scroll.HoveredState.BorderColor = barBorder;

            sheet.Scroll.PressedState.BackColor = barBack.ToPressed(ctx);
            sheet.Scroll.PressedState.FrontColor = palette.Accent.Ascend(ctx).EnsureLightnessDistance(sheet.Scroll.PressedState.BackColor);
            sheet.Scroll.PressedState.BorderColor = barBorder;

            // checkbox

            sheet.Checkbox.Normal = sheet.Button;
            sheet.Checkbox.CheckmarkScale = 0.6f;

            sheet.Checkbox.Checked = sheet.Button;
            sheet.Checkbox.Checked.Normal.BackColor = palette.Accent;
            sheet.Checkbox.Checked.Normal.FrontColor = palette.AccentFront;
            sheet.Checkbox.Checked.Normal.BorderColor = palette.Accent.ToBorder(ctx);

            sheet.Checkbox.Checked.Hovered.BackColor = palette.Accent.ToHovered(ctx);
            sheet.Checkbox.Checked.Hovered.FrontColor = palette.AccentFront.ToHovered(ctx);
            sheet.Checkbox.Checked.Hovered.BorderColor = palette.Accent.ToHovered(ctx).ToBorder(ctx);

            sheet.Checkbox.Checked.Pressed.BackColor = palette.Accent.ToPressed(ctx);
            sheet.Checkbox.Checked.Pressed.FrontColor = palette.AccentFront.ToPressed(ctx);
            sheet.Checkbox.Checked.Pressed.BorderColor = palette.Accent.ToPressed(ctx).ToBorder(ctx);

            // radiobox

            sheet.Radiobox.KnobScale = 0.5f;

            sheet.Radiobox.Normal = sheet.Checkbox.Normal;
            sheet.Radiobox.Normal.BorderRadius = 999.9f;
            sheet.Radiobox.Checked = sheet.Checkbox.Checked;
            sheet.Radiobox.Checked.BorderRadius = 999.9f;

            // slider

            var sliderRadius = theme.BorderRadius;

            sheet.Slider.BarThickness = 0.45f;
            sheet.Slider.TextOverflow = ImTextOverflow.Ellipsis;
            sheet.Slider.HeaderScale = 0.75f;

            sheet.Slider.Normal.BackColor = palette.Control;
            sheet.Slider.Normal.BorderColor = palette.Control.ToBorder(ctx);
            sheet.Slider.Normal.BorderThickness = theme.BorderThickness;
            sheet.Slider.Normal.BorderRadius = sliderRadius;
            sheet.Slider.Normal.FrontColor = palette.Front;

            sheet.Slider.Selected.BackColor = palette.Control.ToPressed(ctx);
            sheet.Slider.Selected.BorderColor = palette.Accent;
            sheet.Slider.Selected.BorderThickness = theme.BorderThickness;
            sheet.Slider.Selected.BorderRadius = sliderRadius;
            sheet.Slider.Selected.FrontColor = palette.Front;

            sheet.Slider.Fill = sheet.Slider.Normal;
            sheet.Slider.Fill.BackColor = palette.Accent;
            sheet.Slider.Fill.BorderColor = palette.Accent.ToBorder(ctx);

            sheet.Slider.Handle.BorderThickness = theme.BorderThickness;
            sheet.Slider.Handle.BorderRadius = theme.BorderRadius >= 1.0f ? 999.9f : 0.0f;
            sheet.Slider.HandleThickness = 1.0f;

            sheet.Slider.Handle.Normal.BackColor = palette.Control.Ascend(ctx);
            sheet.Slider.Handle.Normal.BorderColor = palette.Control.Ascend(ctx).ToBorder(ctx);

            sheet.Slider.Handle.Hovered = sheet.Slider.Handle.Normal;
            sheet.Slider.Handle.Hovered.BorderColor = palette.Accent;

            sheet.Slider.Handle.Pressed = sheet.Radiobox.Checked.Pressed;

            // listbox

            sheet.List.Box.BorderColor = palette.Control.ToBorder(ctx);
            sheet.List.Box.BackColor = palette.Front.WithAlpha(0.05f);
            sheet.List.Box.BorderRadius = theme.BorderRadius;
            sheet.List.Box.BorderThickness = theme.BorderThickness;
            sheet.List.Box.FrontColor = default;
            sheet.List.Padding = theme.Spacing;

            sheet.List.ItemNormal.BorderThickness = 0.0f;
            sheet.List.ItemNormal.BorderRadius = Math.Max(0, theme.BorderRadius - theme.Spacing);
            sheet.List.ItemNormal.Alignment = new ImAlignment(0.0f, 0.5f);
            sheet.List.ItemNormal.Overflow = ImTextOverflow.Ellipsis;

            sheet.List.ItemNormal.Normal.BackColor = palette.Front.WithAlpha(0.03f);
            sheet.List.ItemNormal.Normal.FrontColor = palette.Front;
            sheet.List.ItemNormal.Normal.BorderColor = default;

            sheet.List.ItemNormal.Hovered.BackColor = palette.Front.WithAlpha(0.094f);
            sheet.List.ItemNormal.Hovered.FrontColor = palette.Front;
            sheet.List.ItemNormal.Hovered.BorderColor = default;

            sheet.List.ItemNormal.Pressed.BackColor = palette.Front.WithAlpha(0.063f);
            sheet.List.ItemNormal.Pressed.FrontColor = palette.Front;
            sheet.List.ItemNormal.Pressed.BorderColor = default;

            sheet.List.ItemSelected.BorderThickness = 0.0f;
            sheet.List.ItemSelected.BorderRadius = Math.Max(0, theme.BorderRadius - theme.Spacing);
            sheet.List.ItemSelected.Alignment = new ImAlignment(0.0f, 0.5f);
            sheet.List.ItemSelected.Overflow = ImTextOverflow.Ellipsis;

            sheet.List.ItemSelected.Normal.BackColor = palette.Accent;
            sheet.List.ItemSelected.Normal.FrontColor = palette.AccentFront;
            sheet.List.ItemSelected.Normal.BorderColor = default;

            sheet.List.ItemSelected.Hovered.BackColor = palette.Accent.ToHovered(ctx);
            sheet.List.ItemSelected.Hovered.FrontColor = palette.AccentFront;
            sheet.List.ItemSelected.Hovered.BorderColor = default;

            sheet.List.ItemSelected.Pressed.BackColor = palette.Accent.ToPressed(ctx);
            sheet.List.ItemSelected.Pressed.FrontColor = palette.AccentFront;
            sheet.List.ItemSelected.Pressed.BorderColor = default;

            // foldout

            sheet.Foldout.ArrowScale = 0.6f;
            sheet.Foldout.Button = sheet.Button;
            sheet.Foldout.Button.Normal.BackColor = Color.Lerp(sheet.Window.Box.BackColor, sheet.Button.Normal.BackColor, 0.5f);
            sheet.Foldout.Button.Hovered.BackColor = Color.Lerp(sheet.Window.Box.BackColor, sheet.Button.Hovered.BackColor, 0.5f);
            sheet.Foldout.Button.Pressed.BackColor = Color.Lerp(sheet.Window.Box.BackColor, sheet.Button.Pressed.BackColor, 0.5f);
            sheet.Foldout.Button.BorderThickness = 0.0f;
            sheet.Foldout.Button.Alignment = new ImAlignment(0.0f, 0.5f);

            // trees

            sheet.Tree.ArrowScale = 0.6f;
            sheet.Tree.ItemNormal = sheet.List.ItemNormal;
            sheet.Tree.ItemNormal.Normal.BackColor = default;
            sheet.Tree.ItemSelected = sheet.List.ItemSelected;

            // dropdown
            sheet.Dropdown.ArrowScale = 0.6f;
            sheet.Dropdown.Button = sheet.Button;
            sheet.Dropdown.Button.Alignment = new ImAlignment(0.0f, 0.5f);
            sheet.Dropdown.Button.Overflow = ImTextOverflow.Ellipsis;

            // separator

            sheet.Separator.Thickness = Mathf.Max(1, theme.BorderThickness);
            sheet.Separator.Color = palette.Control.ToBorder(ctx).EnsureLightnessDistance(palette.Back);
            sheet.Separator.TextColor = sheet.Separator.Color;
            sheet.Separator.TextAlignment = new ImAlignment(0.1f, 0.5f);
            sheet.Separator.TextOverflow = ImTextOverflow.Ellipsis;
            sheet.Separator.TextMargin = new ImPadding(theme.Spacing, theme.Spacing, 0, 0);

            // tooltip

            sheet.Tooltip.OffsetPixels = new Vector2(40, -40);
            sheet.Tooltip.Padding = theme.InnerSpacing;
            sheet.Tooltip.Box.BackColor = palette.Back;
            sheet.Tooltip.Box.BorderColor = palette.Back.ToBorder(ctx);
            sheet.Tooltip.Box.BorderRadius = theme.BorderRadius;
            sheet.Tooltip.Box.BorderThickness = theme.BorderThickness;
            sheet.Tooltip.Box.FrontColor = palette.Front;
            sheet.Tooltip.AboveCursor = false;

            // menu

            sheet.Menu.Box = sheet.List.Box;
            sheet.Menu.Box.BackColor = palette.Back.Ascend(ctx);
            sheet.Menu.Padding = sheet.List.Padding;
            sheet.Menu.ItemNormal = sheet.List.ItemNormal;
            sheet.Menu.ItemNormal.Normal.BackColor = Color.clear;
            sheet.Menu.ItemActive = sheet.List.ItemSelected;
            sheet.Menu.ItemActive.Normal.BackColor.SetAlpha(0.8f);
            sheet.Menu.ArrowScale = 0.6f;
            sheet.Menu.CheckmarkScale = 0.6f;
            sheet.Menu.MinWidth = 50.0f;
            sheet.Menu.MinHeight = 10.0f;

            // menu bar

            sheet.MenuBar.ItemExtraWidth = theme.InnerSpacing * 6.0f;
            sheet.MenuBar.Box = sheet.Menu.Box;
            sheet.MenuBar.Box.BackColor = palette.Back.Ascend(ctx);
            sheet.MenuBar.Box.BorderRadius = 0.0f;
            sheet.MenuBar.Box.BorderThickness = sheet.Window.Box.BorderThickness;
            sheet.MenuBar.Box.BorderColor = sheet.Window.Box.BorderColor;
            sheet.MenuBar.ItemNormal = sheet.Menu.ItemNormal;
            sheet.MenuBar.ItemNormal.Alignment = new ImAlignment(0.5f, 0.5f);
            sheet.MenuBar.ItemNormal.BorderRadius = 0.0f;
            sheet.MenuBar.ItemNormal.BorderThickness = 0.0f;
            sheet.MenuBar.ItemActive = sheet.Menu.ItemActive;
            sheet.MenuBar.ItemActive.BorderRadius = 0.0f;
            sheet.MenuBar.ItemActive.BorderThickness = 0.0f;
            sheet.MenuBar.ItemActive.Alignment = new ImAlignment(0.5f, 0.5f);
            sheet.MenuBar.ItemActive.Normal.BackColor.SetAlpha(1.0f);


            // color picker

            sheet.ColorPicker.BorderColor = palette.Control.ToBorder(ctx);
            sheet.ColorPicker.BorderThickness = theme.BorderThickness;
            sheet.ColorPicker.PreviewCircleScale = 0.5f;

            // tab

            sheet.Tabs.IndicatorColor = palette.Accent;
            sheet.Tabs.Normal = sheet.Button;
            sheet.Tabs.Selected = sheet.Button;
            sheet.Tabs.Selected.Normal.BackColor = palette.Back;
            sheet.Tabs.Selected.Hovered.BackColor = sheet.Tabs.Selected.Normal.BackColor;

            sheet.Tabs.ContainerBox.BackColor = sheet.Tabs.Selected.Normal.BackColor;
            sheet.Tabs.ContainerBox.BorderColor = sheet.Tabs.Selected.Normal.BorderColor;
            sheet.Tabs.ContainerBox.BorderRadius = sheet.Tabs.Selected.BorderRadius;
            sheet.Tabs.ContainerBox.BorderRadius.TopLeft = 0;
            sheet.Tabs.ContainerBox.BorderRadius.TopRight = 0;
            sheet.Tabs.ContainerBox.BorderThickness = sheet.Tabs.Selected.BorderThickness;

            // table

            sheet.Table.CellPadding = theme.InnerSpacing;
            sheet.Table.BorderColor = sheet.Separator.Color;
            sheet.Table.SelectedColumnColor = palette.Accent;
            sheet.Table.BorderThickness = sheet.Separator.Thickness;
            sheet.Table.SelectedColumnThickness = sheet.Table.BorderThickness * 2;

            // end

            return sheet;
        }
    }
}