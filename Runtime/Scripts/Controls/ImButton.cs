using System;
using System.Runtime.CompilerServices;
using Imui.Core;
using Imui.IO.Events;
using Imui.Style;
using UnityEngine;

namespace Imui.Controls
{
    public enum ImButtonState
    {
        Normal,
        Hovered,
        Pressed
    }

    [Flags]
    public enum ImButtonFlag
    {
        None = 0,
        ActOnPressMouse = 1 << 0,
        ActOnPressTouch = 1 << 1,
        ReactToHeldDown = 1 << 2,
        ReactToRightButton = 1 << 3,
        ReactToAnyButton = 1 << 4,
        ActOnPress = ActOnPressMouse | ActOnPressMouse,
    }

    public static class ImButton
    {
        private static ImRect AddRect(ImGui gui, ImSize size, ReadOnlySpan<char> label)
        {
            if (size.Mode == ImSizeMode.Fit || (size.Mode == ImSizeMode.Auto && gui.Layout.Axis == ImAxis.Horizontal))
            {
                var textSettings = CreateTextSettings(gui);
                var textSize = gui.MeasureTextSize(label, in textSettings);
                var rectSize = textSize;

                rectSize.x += gui.Style.Layout.InnerSpacing * 2;
                rectSize.y += gui.Style.Layout.ExtraRowHeight;

                return gui.Layout.AddRect(rectSize);
            }

            return gui.AddSingleRowRect(size);
        }

        public static bool Button(this ImGui gui, ReadOnlySpan<char> label, ImSize size = default, ImButtonFlag flags = ImButtonFlag.None)
        {
            gui.AddSpacingIfLayoutFrameNotEmpty();

            var rect = AddRect(gui, size, label);
            return Button(gui, label, rect, flags);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Button(this ImGui gui,
                                  ReadOnlySpan<char> label,
                                  ImRect rect,
                                  ImButtonFlag flags = ImButtonFlag.None,
                                  ImAdjacency adjacency = ImAdjacency.None)
        {
            var id = gui.GetNextControlId();

            return Button(gui, id, label, rect, out _, flags, adjacency);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Button(this ImGui gui,
                                  ImRect rect,
                                  out ImButtonState state,
                                  ImButtonFlag flags = ImButtonFlag.None,
                                  ImAdjacency adjacency = ImAdjacency.None)
        {
            var id = gui.GetNextControlId();

            return Button(gui, id, rect, in gui.Style.Button, out state, flags, adjacency);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Button(this ImGui gui,
                                  uint id,
                                  ReadOnlySpan<char> label,
                                  ImRect rect,
                                  ImButtonFlag flag = ImButtonFlag.None,
                                  ImAdjacency adjacency = default)
        {
            return Button(gui, id, label, rect, out _, flag, adjacency);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Button(this ImGui gui,
                                  uint id,
                                  ReadOnlySpan<char> label,
                                  ImRect rect,
                                  out ImButtonState state,
                                  ImButtonFlag flag = ImButtonFlag.None,
                                  ImAdjacency adjacency = default)
        {
            return Button(gui, id, label, rect, in gui.Style.Button, out state, flag, adjacency);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Button(this ImGui gui,
                                  uint id,
                                  ReadOnlySpan<char> label,
                                  ImRect rect,
                                  in ImStyleButton style,
                                  out ImButtonState state,
                                  ImButtonFlag flag = ImButtonFlag.None,
                                  ImAdjacency adjacency = default)
        {
            var clicked = Button(gui, id, rect, in style, out state, flag, adjacency);
            var textSettings = CreateTextSettings(gui, in style);
            var textColor = GetStateFrontColor(in style, state);
            var textRect = CalculateContentRect(gui, rect);

            gui.Canvas.Text(label, textColor, textRect, in textSettings);

            return clicked;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Button(this ImGui gui,
                                  uint id,
                                  ImRect rect,
                                  out ImButtonState state,
                                  ImButtonFlag flag = ImButtonFlag.None,
                                  ImAdjacency adjacency = default)
        {
            return Button(gui, id, rect, in gui.Style.Button, out state, flag, adjacency);
        }

        public static bool Button(this ImGui gui,
                                  uint id,
                                  ImRect rect,
                                  in ImStyleButton baseStyle,
                                  out ImButtonState state,
                                  ImButtonFlag flag = ImButtonFlag.None,
                                  ImAdjacency adjacency = default)
        {
            var hovered = gui.IsControlHovered(id);
            var pressed = gui.IsControlActive(id);
            var clicked = false;

            gui.RegisterControl(id, rect);

            state = pressed ? ImButtonState.Pressed : hovered ? ImButtonState.Hovered : ImButtonState.Normal;

            gui.Box(rect, MakeBoxStyle(in baseStyle, state).MakeAdjacent(adjacency));

            if (gui.IsReadOnly)
            {
                return false;
            }

            ref readonly var evt = ref gui.Input.MouseEvent;

            var leftButton = evt.LeftButton ||
                             (flag & ImButtonFlag.ReactToAnyButton) != 0 ||
                             ((flag & ImButtonFlag.ReactToRightButton) != 0 && evt.Button == 1);

            var actOnPress =
                (evt.Device == ImMouseDevice.Mouse & (flag & ImButtonFlag.ActOnPressMouse) != 0) |
                (evt.Device == ImMouseDevice.Touch & (flag & ImButtonFlag.ActOnPressTouch) != 0);


            switch (evt.Type)
            {
                case ImMouseEventType.Down when leftButton && hovered:
                    if (actOnPress)
                    {
                        clicked = true;
                        gui.Input.UseMouseEvent();
                    }
                    else if (!pressed)
                    {
                        gui.SetActiveControl(id);
                        gui.Input.UseMouseEvent();
                    }

                    break;

                case ImMouseEventType.Up when pressed:
                    gui.ResetActiveControl();
                    clicked = hovered;

                    if (clicked & actOnPress)
                    {
                        gui.Input.UseMouseEvent();
                    }

                    break;

                case ImMouseEventType.Hold when pressed && (flag & ImButtonFlag.ReactToHeldDown) != 0:
                    clicked = true;
                    gui.Input.UseMouseEvent();
                    break;
            }

            return clicked;
        }

        public static bool InvisibleButton(this ImGui gui, ImRect rect, ImButtonFlag flag = ImButtonFlag.None)
        {
            var id = gui.GetNextControlId();

            return InvisibleButton(gui, id, rect, flag);
        }

        public static bool InvisibleButton(this ImGui gui, ImRect rect, out ImButtonState state, ImButtonFlag flag = ImButtonFlag.None)
        {
            var id = gui.GetNextControlId();

            return InvisibleButton(gui, id, rect, out state, flag);
        }

        public static bool InvisibleButton(this ImGui gui, uint id, ImRect rect, ImButtonFlag flag = ImButtonFlag.None)
        {
            return InvisibleButton(gui, id, rect, out _, flag);
        }

        public static bool InvisibleButton(this ImGui gui, uint id, ImRect rect, out ImButtonState state, ImButtonFlag flag = ImButtonFlag.None)
        {
            var hovered = gui.IsControlHovered(id);
            var pressed = gui.IsControlActive(id);
            var clicked = false;

            state = pressed ? ImButtonState.Pressed : hovered ? ImButtonState.Hovered : ImButtonState.Normal;

            gui.RegisterControl(id, rect);

            if (gui.IsReadOnly)
            {
                return false;
            }

            ref readonly var evt = ref gui.Input.MouseEvent;

            var leftButton = evt.Button == 0 ||
                             (flag & ImButtonFlag.ReactToAnyButton) != 0 ||
                             ((flag & ImButtonFlag.ReactToRightButton) != 0 && evt.Button == 1);

            var actOnPress =
                (evt.Device == ImMouseDevice.Mouse & (flag & ImButtonFlag.ActOnPressMouse) != 0) |
                (evt.Device == ImMouseDevice.Touch & (flag & ImButtonFlag.ActOnPressTouch) != 0);

            switch (evt.Type)
            {
                case ImMouseEventType.Down when leftButton && !pressed && hovered && actOnPress:
                    clicked = true;
                    gui.Input.UseMouseEvent();
                    break;

                case ImMouseEventType.Down when leftButton && !pressed && hovered:
                    gui.SetActiveControl(id);
                    gui.Input.UseMouseEvent();
                    break;

                case ImMouseEventType.Up when pressed:
                    gui.ResetActiveControl();
                    clicked = hovered;

                    if (clicked)
                    {
                        gui.Input.UseMouseEvent();
                    }

                    break;
            }

            return clicked;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 GetStateFrontColor(in ImStyleButton style, ImButtonState state)
        {
            return state switch
            {
                ImButtonState.Hovered => style.Hovered.FrontColor,
                ImButtonState.Pressed => style.Pressed.FrontColor,
                _ => style.Normal.FrontColor
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 GetStateBorderColor(in ImStyleButton style, ImButtonState state)
        {
            return state switch
            {
                ImButtonState.Hovered => style.Hovered.BorderColor,
                ImButtonState.Pressed => style.Pressed.BorderColor,
                _ => style.Normal.BorderColor
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ImTextSettings CreateTextSettings(ImGui gui) => CreateTextSettings(gui, in gui.Style.Button);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ImTextSettings CreateTextSettings(ImGui gui, in ImStyleButton style)
        {
            return new ImTextSettings(gui.Style.Layout.TextSize, style.Alignment, false, style.Overflow);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ImRect CalculateContentRect(ImGui gui, ImRect buttonRect)
        {
            buttonRect.X += gui.Style.Layout.InnerSpacing;
            buttonRect.W -= gui.Style.Layout.InnerSpacing * 2;

            return buttonRect;
        }

        public static ImStyleBox MakeBoxStyle(in ImStyleButton style, ImButtonState state)
        {
            ref readonly var stateStyle = ref GetStateStyle(in style, state);

            return new ImStyleBox
            {
                BackColor = stateStyle.BackColor,
                FrontColor = stateStyle.FrontColor,
                BorderColor = stateStyle.BorderColor,
                BorderThickness = style.BorderThickness,
                BorderRadius = style.BorderRadius
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly ImStyleButtonState GetStateStyle(in ImStyleButton style, ImButtonState state)
        {
            switch (state)
            {
                case ImButtonState.Hovered:
                    return ref style.Hovered;
                case ImButtonState.Pressed:
                    return ref style.Pressed;
                default:
                    return ref style.Normal;
            }
        }
    }
}