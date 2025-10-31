using System;
using System.Globalization;
using Imui.Core;
using Imui.IO.Events;
using Imui.IO.Touch;
using Imui.Rendering;
using Imui.Style;
using UnityEngine;
using UnityEngine.Rendering;

// ReSharper disable BuiltInTypeReferenceStyle

namespace Imui.Controls
{
    [Flags]
    public enum ImNumericEditFlag
    {
        None = 0,
        PlusMinus = 1 << 0,
        Slider = 1 << 1,
        RightAdjacent = 1 << 2
    }

    public static class ImNumericEdit
    {
        // public static readonly ByteFilter FilterByte = new();
        // public static readonly Int16Filter FilterInt16 = new();
        // public static readonly Int32Filter FilterInt32 = new();
        // public static readonly Int64Filter FilterInt64 = new();
        // public static readonly SingleFilter FilterSingle = new();
        // public static readonly DoubleFilter FilterDouble = new();

        private static void GetIdAndRect(ImGui gui, ImSize size, out uint id, out ImRect rect)
        {
            gui.AddSpacingIfLayoutFrameNotEmpty();

            id = gui.GetNextControlId();
            rect = ImTextEdit.AddRect(gui, size, false, out _);
        }

        public static bool NumericEdit(this ImGui gui,
                                       ref byte value,
                                       ImSize size = default,
                                       ReadOnlySpan<char> format = default,
                                       byte step = 1,
                                       byte min = byte.MinValue,
                                       byte max = byte.MaxValue,
                                       ImNumericEditFlag flags = default)
        {
            GetIdAndRect(gui, size, out var id, out var rect);

            return NumericEdit(gui, id, ref value, rect, format, step, min, max, flags);
        }

        public static bool NumericEdit(this ImGui gui,
                                       ref short value,
                                       ImSize size = default,
                                       ReadOnlySpan<char> format = default,
                                       short step = 1,
                                       short min = short.MinValue,
                                       short max = short.MaxValue,
                                       ImNumericEditFlag flags = default)
        {
            GetIdAndRect(gui, size, out var id, out var rect);

            return NumericEdit(gui, id, ref value, rect, format, step, min, max, flags);
        }

        public static bool NumericEdit(this ImGui gui,
                                       ref int value,
                                       ImSize size = default,
                                       ReadOnlySpan<char> format = default,
                                       int step = 1,
                                       int min = int.MinValue,
                                       int max = int.MaxValue,
                                       ImNumericEditFlag flags = default)
        {
            GetIdAndRect(gui, size, out var id, out var rect);

            return NumericEdit(gui, id, ref value, rect, format, step, min, max, flags);
        }

        public static bool NumericEdit(this ImGui gui,
                                       ref long value,
                                       ImSize size = default,
                                       ReadOnlySpan<char> format = default,
                                       long step = 1L,
                                       long min = long.MinValue,
                                       long max = long.MaxValue,
                                       ImNumericEditFlag flags = default)
        {
            GetIdAndRect(gui, size, out var id, out var rect);

            return NumericEdit(gui, id, ref value, rect, format, step, min, max, flags);
        }

        public static bool NumericEdit(this ImGui gui,
                                       ref float value,
                                       ImSize size = default,
                                       ReadOnlySpan<char> format = default,
                                       float step = 0.1f,
                                       float min = float.MinValue,
                                       float max = float.MaxValue,
                                       ImNumericEditFlag flags = default)
        {
            GetIdAndRect(gui, size, out var id, out var rect);

            return NumericEdit(gui, id, ref value, rect, format, step, min, max, flags);
        }

        public static bool NumericEdit(this ImGui gui,
                                       ref double value,
                                       ImSize size = default,
                                       ReadOnlySpan<char> format = default,
                                       double step = 0.1d,
                                       double min = double.MinValue,
                                       double max = double.MaxValue,
                                       ImNumericEditFlag flags = default)
        {
            GetIdAndRect(gui, size, out var id, out var rect);

            return NumericEdit(gui, id, ref value, rect, format, step, min, max, flags);
        }

        public static bool NumericEdit(ImGui gui,
                                       uint id,
                                       ref byte value,
                                       ImRect rect,
                                       ReadOnlySpan<char> format = default,
                                       byte step = 1,
                                       byte min = byte.MinValue,
                                       byte max = byte.MaxValue,
                                       ImNumericEditFlag flags = default)
        {
            gui.Text("Not implemented", rect: rect);
            return false;

            // return NumericEditControl(gui, id, ref value, FilterByte, rect, format, step, min, max, flags);
        }

        public static bool NumericEdit(ImGui gui,
                                       uint id,
                                       ref short value,
                                       ImRect rect,
                                       ReadOnlySpan<char> format = default,
                                       short step = 1,
                                       short min = short.MinValue,
                                       short max = short.MaxValue,
                                       ImNumericEditFlag flags = default)
        {
            gui.Text("Not implemented", rect: rect);
            return false;

            //return NumericEditControl(gui, id, ref value, FilterInt16, rect, format, step, min, max, flags);
        }

        public static bool NumericEdit(ImGui gui,
                                       uint id,
                                       ref int value,
                                       ImRect rect,
                                       ReadOnlySpan<char> format = default,
                                       int step = 1,
                                       int min = int.MinValue,
                                       int max = int.MaxValue,
                                       ImNumericEditFlag flags = default)
        {
            var numberValue = new NumberValue(value);
            var changed = NumericEditControl(gui, id, ref numberValue, rect, format, step, min, max, flags);
            if (changed)
            {
                value = numberValue.valueInt32;
            }

            return changed;
        }

        public static bool NumericEdit(ImGui gui,
                                       uint id,
                                       ref long value,
                                       ImRect rect,
                                       ReadOnlySpan<char> format = default,
                                       long step = 1L,
                                       long min = long.MinValue,
                                       long max = long.MaxValue,
                                       ImNumericEditFlag flags = default)
        {
            gui.Text("Not implemented", rect: rect);
            return false;

            // return NumericEditControl(gui, id, ref value, FilterInt64, rect, format, step, min, max, flags);
        }

        public static bool NumericEdit(ImGui gui,
                                       uint id,
                                       ref float value,
                                       ImRect rect,
                                       ReadOnlySpan<char> format = default,
                                       float step = 0.1f,
                                       float min = float.MinValue,
                                       float max = float.MaxValue,
                                       ImNumericEditFlag flags = default)
        {
            var numberValue = new NumberValue(value);
            var changed = NumericEditControl(gui, id, ref numberValue, rect, format, step, min, max, flags);
            if (changed)
            {
                value = numberValue.valueSingle;
            }

            return changed;
        }

        public static bool NumericEdit(ImGui gui,
                                       uint id,
                                       ref double value,
                                       ImRect rect,
                                       ReadOnlySpan<char> format = default,
                                       double step = 0.1d,
                                       double min = double.MinValue,
                                       double max = double.MaxValue,
                                       ImNumericEditFlag flags = default)
        {
            gui.Text("Not implemented", rect: rect);
            return false;

            // return NumericEditControl(gui, id, ref value, FilterDouble, rect, format, step, min, max, flags);
        }

        public static unsafe bool NumericEditControl(ImGui gui,
                                                     uint id,
                                                     ref NumberValue value,
                                                     ImRect rect,
                                                     ReadOnlySpan<char> format,
                                                     NumberValue step,
                                                     NumberValue min,
                                                     NumberValue max,
                                                     ImNumericEditFlag flags)
        {
            ImAssert.IsTrue(value.Type == min.Type && value.Type == max.Type && value.Type == step.Type,
                            "value.Type == min.Type && value.Type == max.Type && value.Type == step.Type");

            var delta = 0.0d;
            var hovered = gui.IsControlHovered(id);
            var active = gui.IsControlActive(id);
            var useSlider = (flags & ImNumericEditFlag.Slider) != 0;
            var usePlusMinusButtons = !useSlider && (flags & ImNumericEditFlag.PlusMinus) != 0;

            ref readonly var evt = ref gui.Input.MouseEvent;

            gui.PushId(id);

            var bufferId = gui.GetNextControlId();
            var sliderId = gui.GetNextControlId();

            if (!active && useSlider)
            {
                // (artem-s): when double clicking, pass control to text editor
                if (evt.LeftButton && (evt.Type != ImMouseEventType.Down || evt.Count < 2))
                {
                    var sliderMin = min.AsDouble();
                    var sliderMax = max.AsDouble();
                    var sliderStep = step.AsDouble();
                    var sliderDelta = NumericSlider(gui, id, sliderId, sliderMin, sliderMax, sliderStep, rect);

                    if (value.Type.IsInteger())
                    {
                        sliderDelta = Math.Round(sliderDelta);
                    }

                    delta += sliderDelta;
                }
            }

            if (usePlusMinusButtons)
            {
                delta = PlusMinusButtons(gui, ref rect) * step.AsDouble();
            }

            gui.PopId();

            ImTextEditBuffer textBuffer;

            if (active)
            {
                ref var editBuffer = ref gui.Storage.Get<EditBuffer>(bufferId);
                fixed (char* ptr = editBuffer.Buffer)
                {
                    var span = new Span<char>(ptr, EditBuffer.BUFFER_LENGTH);
                    textBuffer = new ImTextEditBuffer(span, editBuffer.Count, gui.Arena);
                }
            }
            else
            {
                var tempBuffer = gui.Arena.AllocArray<char>(EditBuffer.BUFFER_LENGTH);
                var tempBufferLength = value.Format(tempBuffer);

                textBuffer = new ImTextEditBuffer(tempBuffer[..tempBufferLength], gui.Arena);
            }

            var adjacency = usePlusMinusButtons ? ImAdjacency.Left : ImAdjacency.None;
            if ((flags & ImNumericEditFlag.RightAdjacent) != 0)
            {
                adjacency |= ImAdjacency.Right;
            }

            var changed = false;

            if (!active && useSlider)
            {
                ref readonly var style = ref gui.Style.TextEdit.Normal.Box;

                var align = gui.Style.TextEdit.Alignment;
                var radius = style.BorderRadius;

                if ((adjacency & ImAdjacency.Left) != 0)
                {
                    radius.BottomRight = 0;
                    radius.TopRight = 0;
                }
                else if ((adjacency & ImAdjacency.Right) != 0)
                {
                    radius.BottomLeft = 0;
                    radius.TopLeft = 0;
                }

                var halfVertPadding = Mathf.Max(rect.H - gui.TextDrawer.GetLineHeightFromFontSize(gui.Style.Layout.TextSize), 0.0f) / 2.0f;
                var textRect = rect.WithPadding(left: gui.Style.Layout.InnerSpacing,
                                                right: gui.Style.Layout.InnerSpacing,
                                                top: halfVertPadding,
                                                bottom: halfVertPadding);

                gui.Canvas.RectWithOutline(rect, style.BackColor, style.BorderColor, style.BorderThickness, radius);
                gui.Canvas.Text(textBuffer,
                                style.FrontColor,
                                textRect,
                                gui.Style.Layout.TextSize,
                                alignX: align.X,
                                alignY: align.Y,
                                false,
                                ImTextOverflow.Ellipsis);

                gui.RegisterControl(id, rect);

                if (hovered && evt is { Type: ImMouseEventType.Down, LeftButton: true, Count: >= 2 })
                {
                    gui.SetActiveControl(id, ImControlFlag.Draggable);
                }
            }
            else
            {
                changed = gui.TextEdit(id, ref textBuffer, rect, false, adjacency, ImTouchKeyboardType.Numeric);
            }

            if (changed)
            {
                ref var editBuffer = ref gui.Storage.Get<EditBuffer>(bufferId);
                editBuffer.Populate(textBuffer);
                
                if (NumberValue.TryParse(value.Type, textBuffer, out var newValue))
                {
                    value = newValue;
                }
            }

            if (delta != 0)
            {
                value.Add(delta);
                changed = true;
            }

            if (changed)
            {
                value.Clamp(min, max);
            }

            if (!active && gui.GetActiveControl() == id)
            {
                ref var editBuffer = ref gui.Storage.Get<EditBuffer>(bufferId);
                editBuffer.Populate(value);
            }

            return changed;
        }

        private static int PlusMinusButtons(ImGui gui, ref ImRect rect)
        {
            var border = gui.Style.Button.BorderThickness;
            var height = rect.H;
            var width = height;

            var plusBtnRect = rect.TakeRight(width, -border, out rect);
            var minusBtnRect = rect.TakeRight(width, -border, out rect);
            var delta = 0;

            if (gui.Button("-", minusBtnRect, flags: ImButtonFlag.ReactToHeldDown, ImAdjacency.Middle))
            {
                delta--;
            }

            if (gui.Button("+", plusBtnRect, flags: ImButtonFlag.ReactToHeldDown, ImAdjacency.Right))
            {
                delta++;
            }

            return delta;
        }

        private static void HandleDrag(in ImMouseEvent evt, ref double delta, double step, double min, double max, in ImRect rect)
        {
            if (evt.Delta.x == 0)
            {
                return;
            }

            delta = step == 0 ? Math.Min(max - min, rect.W) * evt.Delta.x / rect.W : step * Math.Sign(evt.Delta.x);
        }

        public static double NumericSlider(ImGui gui, uint hoveredId, uint id, double min, double max, double step, ImRect rect)
        {
            var hovered = gui.IsControlHovered(hoveredId);
            var active = gui.IsControlActive(id);
            var delta = 0.0d;

            gui.RegisterControl(id, rect);

            ref readonly var evt = ref gui.Input.MouseEvent;
            switch (evt.Type)
            {
                case ImMouseEventType.Down or ImMouseEventType.BeginDrag when evt.LeftButton && hovered:
                    gui.SetActiveControl(id, ImControlFlag.Draggable);
                    HandleDrag(in evt, ref delta, step, min, max, in rect);
                    gui.Input.UseMouseEvent();
                    break;

                case ImMouseEventType.Drag when active:
                    HandleDrag(in evt, ref delta, step, min, max, in rect);
                    gui.Input.UseMouseEvent();
                    break;

                case ImMouseEventType.Up when active:
                    gui.ResetActiveControl();
                    break;
            }

            return delta;
        }

        public unsafe struct EditBuffer
        {
            public const int BUFFER_LENGTH = 64;

            public fixed char Buffer[BUFFER_LENGTH];
            public int Count;

            public void Populate(ReadOnlySpan<char> str)
            {
                fixed (char* buf = Buffer)
                {
                    str.CopyTo(new Span<char>(buf, BUFFER_LENGTH));
                    Count = str.Length;
                }
            }
            
            public void Populate(NumberValue value)
            {
                fixed (char* buf = Buffer)
                {
                    Count = value.Format(new Span<char>(buf, BUFFER_LENGTH));
                }
            }
        }

        public enum NumberType
        {
            Byte,
            Int16,
            Int32,
            Int64,
            Single,
            Double
        }

        public static bool IsInteger(this NumberType type)
        {
            return type is NumberType.Byte or NumberType.Int16 or NumberType.Int32 or NumberType.Int64;
        }

        public struct NumberValue
        {
            private static readonly CultureInfo CultureInvariant = CultureInfo.InvariantCulture;

            // (artem-s): allow using comma as a decimal separator
            private static readonly CultureInfo CultureDeutsch = new("de");

            public static bool IsValid(NumberType type, ReadOnlySpan<char> span)
            {
                return span.Length > 0 && TryParse(type, span, out _);
            }

            public static bool TryParse(NumberType type, ReadOnlySpan<char> span, out NumberValue value)
            {
                if (span.Length == 0)
                {
                    value = new NumberValue(type);
                    return true;
                }

                switch (type)
                {
                    case NumberType.Single:
                        if (Single.TryParse(span, NumberStyles.Float, CultureDeutsch, out var valueSingle) ||
                            Single.TryParse(span, NumberStyles.Float, CultureInvariant, out valueSingle))
                        {
                            value = new NumberValue(valueSingle);
                            return true;
                        }
                        break;
                    case NumberType.Int32:
                        if (Int32.TryParse(span, NumberStyles.Integer, CultureInvariant, out var valueInt32))
                        {
                            value = new NumberValue(valueInt32);
                            return true;
                        }
                        break;
                }

                value = default;
                return false;
            }

            public static void Clamp(ref NumberValue value, in NumberValue min, in NumberValue max)
            {
                ImAssert.IsTrue(value.Type == min.Type && value.Type == max.Type, "value.Type == min.Type && value.Type == max.Type");

                switch (value.Type)
                {
                    case NumberType.Int32:
                        value.valueInt32 = Math.Clamp(value.valueInt32, min.valueInt32, max.valueInt32);
                        break;
                    case NumberType.Single:
                        value.valueSingle = Math.Clamp(value.valueSingle, min.valueSingle, max.valueSingle);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            public readonly NumberType Type;

            public Int32 valueInt32;
            public Single valueSingle;

            public NumberValue(Int32 value)
            {
                Type = NumberType.Int32;
                valueInt32 = value;
                valueSingle = 0;
            }

            public NumberValue(Single value)
            {
                Type = NumberType.Single;
                valueInt32 = 0;
                valueSingle = value;
            }

            private NumberValue(NumberType type)
            {
                Type = type;
                valueInt32 = 0;
                valueSingle = 0;
            }

            public double AsDouble()
            {
                switch (Type)
                {
                    case NumberType.Int32:
                        return valueInt32;
                    case NumberType.Single:
                        return valueSingle;
                    default:
                        throw new NotImplementedException();
                }
            }
            
            public int Format(Span<char> span)
            {
                switch (Type)
                {
                    case NumberType.Int32:
                    {
                        return valueInt32.TryFormat(span, out var written) ? written : 0;
                    }
                    case NumberType.Single:
                    {
                        return valueSingle.TryFormat(span, out var written) ? written : 0;
                    }
                    default:
                        throw new NotImplementedException();
                }
            }

            public void Add(double value)
            {
                switch (Type)
                {
                    case NumberType.Int32:
                        valueInt32 += (Int32)value;
                        break;
                    case NumberType.Single:
                        valueSingle += (Single)value;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            public void Clamp(NumberValue min, NumberValue max)
            {
                Clamp(ref this, in min, in max);
            }

            public static implicit operator NumberValue(Int32 value) => new(value);
            public static implicit operator NumberValue(Single value) => new(value);
        }

        // public abstract class NumericFilter<T>: ImTextEditFilter
        // {
        //     public override string GetFallbackString()
        //     {
        //         return "0";
        //     }
        //
        //     public override bool IsValid(ReadOnlySpan<char> buffer)
        //     {
        //         return TryParse(buffer, out _);
        //     }
        //
        //     public virtual bool TryParse(ReadOnlySpan<char> buffer, out T value)
        //     {
        //         if (buffer.IsEmpty)
        //         {
        //             value = default;
        //             return true;
        //         }
        //
        //         return TryParseNonEmpty(in buffer, out value);
        //     }
        //
        //     public virtual bool IsInteger => true;
        //
        //     public abstract double AsDouble(T value);
        //     public abstract T Add(T value0, double value1);
        //     public abstract T Clamp(T value, T min, T max);
        //     public abstract bool TryParseNonEmpty(in ReadOnlySpan<char> buffer, out T value);
        //     public abstract bool TryFormat(Span<char> buffer, T value, out int length, ReadOnlySpan<char> format);
        //
        //     protected double Add(double value0, double value1, double min, double max)
        //     {
        //         var result = value0 + value1;
        //         if (result > max)
        //         {
        //             result = max;
        //         }
        //         else if (result < min)
        //         {
        //             result = min;
        //         }
        //
        //         return result;
        //     }
        // }
        //
        // public sealed class ByteFilter: NumericFilter<Byte>
        // {
        //     public override double AsDouble(byte value) => value;
        //     public override Byte Add(Byte value0, double value1) => (Byte)Add(value0, value1, Byte.MinValue, Byte.MaxValue);
        //     public override Byte Clamp(Byte value, Byte min, Byte max) => value > max ? max : value < min ? min : value;
        //
        //     public override bool TryParseNonEmpty(in ReadOnlySpan<char> buffer, out Byte value) =>
        //         Byte.TryParse(buffer, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        //
        //     public override bool TryFormat(Span<char> buffer, Byte value, out int length, ReadOnlySpan<char> format) =>
        //         value.TryFormat(buffer, out length, format);
        // }
        //
        // public sealed class Int16Filter: NumericFilter<Int16>
        // {
        //     public override double AsDouble(Int16 value) => value;
        //     public override Int16 Add(Int16 value0, double value1) => (Int16)Add(value0, value1, byte.MinValue, byte.MaxValue);
        //     public override Int16 Clamp(Int16 value, Int16 min, Int16 max) => value > max ? max : value < min ? min : value;
        //
        //     public override bool TryParseNonEmpty(in ReadOnlySpan<char> buffer, out Int16 value) =>
        //         Int16.TryParse(buffer, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        //
        //     public override bool TryFormat(Span<char> buffer, Int16 value, out int length, ReadOnlySpan<char> format) =>
        //         value.TryFormat(buffer, out length, format);
        // }
        //
        // public sealed class Int32Filter: NumericFilter<Int32>
        // {
        //     public override double AsDouble(Int32 value) => value;
        //     public override Int32 Add(Int32 value0, double value1) => (Int32)Add(value0, value1, Int32.MinValue, Int32.MaxValue);
        //     public override Int32 Clamp(Int32 value, Int32 min, Int32 max) => value > max ? max : value < min ? min : value;
        //
        //     public override bool TryParseNonEmpty(in ReadOnlySpan<char> buffer, out Int32 value) =>
        //         Int32.TryParse(buffer, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        //
        //     public override bool TryFormat(Span<char> buffer, Int32 value, out int length, ReadOnlySpan<char> format) =>
        //         value.TryFormat(buffer, out length, format);
        // }
        //
        // public sealed class Int64Filter: NumericFilter<Int64>
        // {
        //     public override double AsDouble(Int64 value) => value;
        //     public override Int64 Add(Int64 value0, double value1) => (Int64)Add(value0, value1, Int64.MinValue, Int64.MaxValue);
        //     public override Int64 Clamp(Int64 value, Int64 min, Int64 max) => value > max ? max : value < min ? min : value;
        //
        //     public override bool TryParseNonEmpty(in ReadOnlySpan<char> buffer, out Int64 value) =>
        //         Int64.TryParse(buffer, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        //
        //     public override bool TryFormat(Span<char> buffer, Int64 value, out int length, ReadOnlySpan<char> format) =>
        //         value.TryFormat(buffer, out length, format);
        // }
        //
        // public sealed class SingleFilter: NumericFilter<Single>
        // {
        //     public override bool IsInteger => false;
        //
        //     public override double AsDouble(Single value) => value;
        //     public override Single Add(Single value0, double value1) => (Single)Add(value0, value1, Single.MinValue, Single.MaxValue);
        //     public override Single Clamp(Single value, Single min, Single max) => value > max ? max : value < min ? min : value;
        //
        //     public override bool TryParseNonEmpty(in ReadOnlySpan<char> buffer, out Single value)
        //     {
        //         return Single.TryParse(buffer, NumberStyles.Float, CultureInfo.InvariantCulture, out value) ||
        //                Single.TryParse(buffer, NumberStyles.Float, DeutschCulture, out value);
        //     }
        //
        //     public override bool TryFormat(Span<char> buffer, Single value, out int length, ReadOnlySpan<char> format) =>
        //         value.TryFormat(buffer, out length, format.IsEmpty ? "G" : format);
        // }
        //
        // public sealed class DoubleFilter: NumericFilter<Double>
        // {
        //     public override bool IsInteger => false;
        //
        //     public override double AsDouble(Double value) => value;
        //     public override Double Add(Double value0, double value1) => (Double)Add(value0, value1, Double.MinValue, Double.MaxValue);
        //     public override Double Clamp(Double value, Double min, Double max) => value > max ? max : value < min ? min : value;
        //
        //     public override bool TryParseNonEmpty(in ReadOnlySpan<char> buffer, out Double value)
        //     {
        //         return Double.TryParse(buffer, NumberStyles.Float, CultureInfo.InvariantCulture, out value) ||
        //                Double.TryParse(buffer, NumberStyles.Float, DeutschCulture, out value);
        //     }
        //
        //     public override bool TryFormat(Span<char> buffer, Double value, out int length, ReadOnlySpan<char> format) =>
        //         value.TryFormat(buffer, out length, format.IsEmpty ? "G" : format);
        // }
    }
}