using UnityEngine;

namespace Imui.IO.Events
{
    public enum ImTextEventType
    {
        None,
        Cancel,
        Submit,
        Set
    }

    public readonly struct ImTextEvent
    {
        public readonly ImTextEventType Type;
        public readonly string Text;
        public readonly RangeInt? Selection;

        public ImTextEvent(ImTextEventType type, RangeInt? selection = null)
        {
            Type = type;
            Text = null;
            Selection = selection;
        }

        public ImTextEvent(ImTextEventType type, string text, RangeInt? selection = null)
        {
            Type = type;
            Text = text;
            Selection = selection;
        }
        
        public override string ToString()
        {
            return $"type:{Type} text:{Text}";
        }
    }
}