using System;
using Imui.IO.Events;
using UnityEngine;

namespace Imui.IO.Touch
{
    public enum ImTouchKeyboardType
    {
        Default,
        Numeric
    }

    public struct ImTouchKeyboardSettings
    {
        public bool Muiltiline;
        public ImTouchKeyboardType Type;
        public int CharactersLimit;
        public RangeInt Selection;
    }

    public class ImTouchKeyboard: IDisposable
    {
        private const int TOUCH_KEYBOARD_CLOSE_FRAMES_THRESHOLD = 3;

        public TouchScreenKeyboard TouchKeyboard;

        private uint touchKeyboardOwner;
        private int touchKeyboardRequestFrame;

        public void RequestTouchKeyboard(uint owner, ReadOnlySpan<char> text, ImTouchKeyboardSettings settings)
        {
#if UNITY_WEBGL
            // TODO (artem-s): fix touch keyboard handling for webgl
            return;
#endif

            if (!TouchScreenKeyboard.isSupported)
            {
                return;
            }

            if (owner != touchKeyboardOwner && TouchKeyboard != null)
            {
                TouchKeyboard.active = false;
                TouchKeyboard = null;
                touchKeyboardOwner = 0;
            }

            if (TouchKeyboard == null)
            {
                touchKeyboardOwner = owner;
                TouchKeyboard = TouchScreenKeyboard.Open(
                    new string(text),
                    GetType(settings.Type),
                    false,
                    settings.Muiltiline);
                
                TouchKeyboard.characterLimit = settings.CharactersLimit;
            }

            if (!TouchKeyboard.active)
            {
                TouchKeyboard.active = true;
            }

            if (TouchKeyboard.active && TouchKeyboard.canSetSelection)
            {
                var prev = TouchKeyboard.selection;
                var changed = prev.start != settings.Selection.start || prev.length != settings.Selection.length;
                if (changed)
                {
                    // (artem-s): setting selection does allocate currently edited string for range checking
                    TouchKeyboard.selection = settings.Selection;
                }
            }
            
            touchKeyboardRequestFrame = Time.frameCount;
        }

        private TouchScreenKeyboardType GetType(ImTouchKeyboardType type)
        {
            switch (type)
            {
                case ImTouchKeyboardType.Numeric:
                    return TouchScreenKeyboardType.DecimalPad;
                default:
                    return TouchScreenKeyboardType.Default;
            }
        }

        public void HandleTouchKeyboard(out ImTextEvent textEvent)
        {
            textEvent = default;

            if (TouchKeyboard != null)
            {
                var shouldHide = Mathf.Abs(Time.frameCount - touchKeyboardRequestFrame) > TOUCH_KEYBOARD_CLOSE_FRAMES_THRESHOLD;

                switch (TouchKeyboard.status)
                {
                    case TouchScreenKeyboard.Status.Visible when TouchScreenKeyboard.inputFieldAppearance == TouchScreenKeyboard.InputFieldAppearance.AlwaysHidden:
                        if (Input.inputString?.Length > 0)
                        {
                            var text = TouchKeyboard.text;
                            RangeInt? selection = TouchKeyboard.canGetSelection ? TouchKeyboard.selection : null;
                            textEvent = new ImTextEvent(ImTextEventType.Set, text, selection);
                        }
                        break;
                    case TouchScreenKeyboard.Status.Canceled:
                        textEvent = new ImTextEvent(ImTextEventType.Cancel);
                        shouldHide = true;
                        break;
                    case TouchScreenKeyboard.Status.Done:
                        textEvent = new ImTextEvent(ImTextEventType.Submit, TouchKeyboard.text);
                        shouldHide = true;
                        break;
                }

                if (shouldHide)
                {
                    TouchKeyboard.active = false;
                    TouchKeyboard = null;
                }
            }
        }

        public void Dispose()
        {
            if (TouchKeyboard != null)
            {
                TouchKeyboard.active = false;
                TouchKeyboard = null;
            }
        }
    }
}