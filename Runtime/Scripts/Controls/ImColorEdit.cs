using System;
using Imui.Core;
using Imui.Style;
using UnityEngine;

namespace Imui.Controls
{
    public static class ImColorEdit
    {
        public static ImRect AddRect(ImGui gui, ImSize size)
        {
            if (size.Mode is ImSizeMode.Auto or ImSizeMode.Fit)
            {
                var threeCharWidth = gui.TextDrawer.GetCharacterAdvance('9', gui.Style.Layout.TextSize) * 3;
                var compLetterWidth = gui.GetRowHeight() * 0.75f;
                var textWidth = threeCharWidth + gui.Style.Layout.InnerSpacing * 2 + compLetterWidth;
                var minWidth = textWidth * 5 + gui.Style.Layout.InnerSpacing * 4;
                var width = Mathf.Max(gui.GetLayoutWidth(), minWidth);

                return gui.Layout.AddRect(width, gui.GetRowHeight());
            }

            return gui.AddSingleRowRect(size);
        }

        public static Color ColorEdit(this ImGui gui, Color color, ImSize size = default)
        {
            ColorEdit(gui, ref color, size);
            return color;
        }

        public static bool ColorEdit(this ImGui gui, ref Color color, ImSize size = default)
        {
            gui.AddSpacingIfLayoutFrameNotEmpty();

            var id = gui.GetNextControlId();
            var rect = AddRect(gui, size);

            return ColorEdit(gui, id, ref color, rect);
        }

        public static bool ColorEdit(this ImGui gui, uint id, ref Color color, ImRect rect)
        {
            gui.PushId(id);

            var rId = gui.GetNextControlId();
            var gId = gui.GetNextControlId();
            var bId = gui.GetNextControlId();
            var aId = gui.GetNextControlId();
            var cId = gui.GetNextControlId();
            
            Span<ImRect> rects = stackalloc ImRect[5];
            rect.SplitHorizontal(ref rects, rects.Length, gui.Style.Layout.Spacing);

            var changed = false;
            var col32 = (Color32)color;
            var align = gui.Style.TextEdit.Alignment.X;

            try
            {
                gui.Style.TextEdit.Alignment.X = 0.5f;

                changed |= ColorComponent(gui, rId, in rects[0], Color.red, 'R', ref col32.r);
                changed |= ColorComponent(gui, gId, in rects[1], Color.green, 'G', ref col32.g);
                changed |= ColorComponent(gui, bId, in rects[2], Color.blue, 'B', ref col32.b);
                changed |= ColorComponent(gui, aId, in rects[3], gui.Style.Text.Color, 'A', ref col32.a);
                
                if (changed)
                {
                    color = col32;
                }
            }
            finally
            {
                gui.Style.TextEdit.Alignment.X = align;
            }

            changed |= gui.ColorPickerButton(cId, ref color, rects[4], ImColorButtonFlag.AlphaOnePreview);

            gui.PopId();

            return changed;
        }

        private static unsafe bool ColorComponent(ImGui gui, uint id, in ImRect rect, Color32 color, char letter, ref byte value)
        {
            var group = new ImGroup(gui, rect, 2);
            group.SetNextSize(gui.GetRowHeight() * 0.75f);
            var letterItem = group.GetNext();
            var textStyle = new ImTextSettings(gui.Style.Layout.TextSize * 0.75f, 0.5f, 0.5f);
            var textColor = Color32.Lerp(gui.Style.Text.Color, color, 0.25f);
            var componentText = new ReadOnlySpan<char>(&letter, 1);
            var componentStyle = gui.Style.TextEdit.Normal.Box;
            
            componentStyle.BorderRadius.Apply(letterItem.Flags);
            
            gui.Box(letterItem.Rect, in componentStyle);
            gui.Text(componentText, textStyle, textColor, letterItem.Rect);
            
            return group.NumericEdit(id, ref value, flags: ImNumericEditFlag.Slider);
        }
    }
}