using System;
using System.Runtime.CompilerServices;
using Imui.Core;

namespace Imui.Controls
{
    public ref struct ImGroup
    {
        private const float HALF_BORDER = 0.5f;
        
        [Flags]
        public enum ItemFlag: byte
        {
            None = 0,
            AdjacentToLeft = 1 << 0,
            AdjacentToRight = 1 << 1
        }

        public readonly struct Item
        {
            public readonly ImRect Rect;
            public readonly ItemFlag Flags;

            public Item(ImRect rect, ItemFlag flags)
            {
                Rect = rect;
                Flags = flags;
            }
        }

        public readonly ImGui Gui;
        public readonly int Count;

        private int index;
        private ImRect rest;
        private float nextSize;
        
        public ImGroup(ImGui gui, ImRect rect, int count)
        {
            Gui = gui;
            Count = count;

            rest = rect;
            index = 0;
            nextSize = 0;
        }

        public void SetNextSize(float size)
        {
            nextSize = size;
        }
        
        public Item GetNext()
        {
            var item = nextSize > 0 ? GetNext(nextSize) : GetNext(rest.W / (Count - index));
            nextSize = 0;
            return item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Item GetNext(float size)
        {
            var rect = rest.TakeLeft(size, out rest);
            var flags = ItemFlag.None;

            if (index < Count - 1)
            {
                rect.W += HALF_BORDER;
                flags |= ItemFlag.AdjacentToRight;
            }

            if (index > 0)
            {
                rect.X -= HALF_BORDER;
                rect.W += HALF_BORDER;
                flags |= ItemFlag.AdjacentToLeft;
            }
            
            index++;

            return new Item(rect, flags);
        }
    }

    public static class ImGroupUtility
    {
        public static void Apply(this ref ImRectRadius radius, ImGroup.ItemFlag flags)
        {
            if ((flags & ImGroup.ItemFlag.AdjacentToLeft) != 0)
            {
                radius.TopLeft = 0.0f;
                radius.BottomLeft = 0.0f;
            }
            
            if ((flags & ImGroup.ItemFlag.AdjacentToRight) != 0)
            {
                radius.TopRight = 0.0f;
                radius.BottomRight = 0.0f;
            }
        }
    }
}