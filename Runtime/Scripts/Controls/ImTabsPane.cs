using System;
using System.Runtime.CompilerServices;
using Imui.Core;

namespace Imui.Controls
{
    [Flags]
    public enum ImTabsPaneFlags
    {
        None = 0,
        NonScrollableContent = 1 << 0
    }
    
    public struct ImTabsPaneState
    {
        public ImRect Content;
        public uint Selected;
        public ImTabsPaneFlags Flags;
    }

    public static unsafe class ImTabsPane
    {
        public static ImRect AddButtonRect(ImGui gui, ReadOnlySpan<char> label, ImSize size)
        {
            if (size.Mode == ImSizeMode.Fixed)
            {
                return gui.AddLayoutRect(size.Width, size.Height);
            }

            var width = gui.MeasureTextSize(label, GetTextSettings(gui)).x;

            return gui.AddLayoutRect(width + gui.Style.Layout.Indent * 2, GetTabBarHeight(gui));
        }

        public static void BeginTabsPane(this ImGui gui, ImRect rect, ImTabsPaneFlags flags = ImTabsPaneFlags.None)
        {
            var id = gui.GetNextControlId();
            var state = gui.BeginScopeUnsafe<ImTabsPaneState>(id);
            var buttonsRect = rect.TakeTop(GetTabBarHeight(gui), out state->Content);

            state->Flags = flags;

            gui.Layout.Push(ImAxis.Horizontal, buttonsRect);
            gui.BeginScrollable();
        }

        public static void EndTabsPane(this ImGui gui)
        {
            gui.EndScrollable(ImScrollFlag.HideVerBar | ImScrollFlag.HideHorBar);
            gui.Layout.Pop();

            gui.EndScope<ImTabsPaneState>();
        }

        public static bool BeginTab(this ImGui gui, ReadOnlySpan<char> label)
        {
            var id = gui.GetNextControlId();
            var rect = AddButtonRect(gui, label, default);
            var state = gui.GetCurrentScopeUnsafe<ImTabsPaneState>();

            if (state->Selected == 0)
            {
                state->Selected = id;
            }

            if (TabBarButton(gui, id, state->Selected == id, label, rect))
            {
                state->Selected = id;
            }

            if (state->Selected != id)
            {
                return false;
            }

            gui.Box(state->Content, in gui.Style.Tabs.ContainerBox);

            if (gui.Style.Tabs.SeparatorThickness > 0.0f)
            {
                var sepRect = state->Content.TakeTop(gui.Style.Tabs.SeparatorThickness);
                gui.Canvas.Rect(sepRect, gui.Style.Tabs.SeparatorColor);
            }
            else
            {
                CoverSeam(gui, state->Content, rect);
            }

            gui.PushId(id);
            gui.Layout.Push(ImAxis.Vertical, state->Content.WithPadding(gui.Style.Layout.Spacing));

            if ((state->Flags & ImTabsPaneFlags.NonScrollableContent) == 0)
            {
                gui.PushId(id);
                gui.BeginScrollable();
                gui.PopId();
            }
            
            var maskRect = state->Content.WithPadding(gui.Style.Tabs.ContainerBox.BorderThickness);
            gui.Canvas.PushRectMask(maskRect, gui.Style.Tabs.ContainerBox.BorderRadius);
            gui.Canvas.PushClipRect(maskRect);

            return true;
        }

        public static void EndTab(this ImGui gui)
        {
            var state = gui.GetCurrentScopeUnsafe<ImTabsPaneState>();

            gui.Canvas.PopClipRect();
            gui.Canvas.PopRectMask();

            // TODO (artem-s): add flag dynamically resize tab's container to fit content 
            // var rect = gui.Layout.GetWholeRect();
            // var state = gui.PeekControlScopePtr<ImTabsPaneState>();
            //
            // state->Content = rect;
            
            if ((state->Flags & ImTabsPaneFlags.NonScrollableContent) == 0)
            {
                gui.EndScrollable();
            }

            gui.Layout.Pop();
            gui.PopId();
        }

        public static bool TabBarButton(ImGui gui, uint id, bool selected, ReadOnlySpan<char> label, ImRect rect)
        {
            ref readonly var buttonStyle = ref (selected ? ref gui.Style.Tabs.Selected : ref gui.Style.Tabs.Normal);

            var clicked = gui.Button(id, rect, in buttonStyle, out var state);
            var frontColor = ImButton.GetStateFrontColor(in buttonStyle, state);
            var textSettings = GetTextSettings(gui);
            
            gui.Text(label, in textSettings, frontColor, rect);
            
            return clicked;
        }

        private static void CoverSeam(ImGui gui, ImRect content, ImRect button)
        {
            var rect = button.TakeBottom(gui.Style.Button.BorderThickness + gui.Style.Tabs.ContainerBox.BorderThickness);

            rect.Y -= gui.Style.Tabs.ContainerBox.BorderThickness * 2.0f;
            rect.H += gui.Style.Tabs.ContainerBox.BorderThickness * 2.0f;
            rect.X += gui.Style.Button.BorderThickness;
            rect.W -= gui.Style.Button.BorderThickness * 2;

            gui.Canvas.Rect(rect, gui.Style.Tabs.ContainerBox.BackColor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ImTextSettings GetTextSettings(ImGui gui) => new(gui.Style.Layout.TextSize, 0.5f, 0.5f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetTabBarHeight(ImGui gui) => gui.GetRowHeight();
    }
}