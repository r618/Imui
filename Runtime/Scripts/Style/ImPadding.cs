using System;

namespace Imui.Style
{
    [Serializable]
    public struct ImPadding
    {
        public float Vertical => Top + Bottom;
        public float Horizontal => Left + Right;

        public float Left;
        public float Right;
        public float Top;
        public float Bottom;

        public ImPadding(float padding): this(padding, padding, padding, padding) { }

        public ImPadding(float left, float right, float top, float bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public void Add(float value)
        {
            Left += value;
            Right += value;
            Top += value;
            Bottom += value;
        }
        
        public void Add(in ImPadding padding)
        {
            Left += padding.Left;
            Right += padding.Right;
            Top += padding.Top;
            Bottom += padding.Bottom;
        }

        public static implicit operator ImPadding(float padding) => new(padding);

        public static ImPadding operator +(ImPadding padding, float value)
        {
            padding.Add(value);
            return padding;
        }
        
        public static ImPadding operator +(ImPadding padding, in ImPadding value)
        {
            padding.Add(in value);
            return padding;
        }

        public static ImPadding operator -(ImPadding padding, float value)
        {
            padding.Add(-value);
            return padding;
        }
        
        public static ImPadding operator -(ImPadding padding)
        {
            padding.Left = -padding.Left;
            padding.Right = -padding.Right;
            padding.Top = -padding.Top;
            padding.Bottom = -padding.Bottom;
            return padding;
        }
    }
}