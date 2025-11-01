using System;

namespace Imui.Style
{
    // (artem-s): this was a bad idea, I should probably get rid of it
    [Flags]
    public enum ImAdjacency
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8,
        Middle = Left | Right
    }
}