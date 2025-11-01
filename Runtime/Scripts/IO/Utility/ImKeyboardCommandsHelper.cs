using System;
using Imui.IO.Events;
using UnityEngine;

namespace Imui.IO.Utility
{
    [Flags]
    public enum ImKeyboardCommandFlag: uint
    {
        None = 0,
        Select = 1 << 0,
        JumpWord = 1 << 1,
        SelectAll = 1 << 2,
        Copy = 1 << 3,
        Paste = 1 << 4,
        Cut = 1 << 5,
        JumpEnd = 1 << 6
    }

    public static class ImKeyboardCommandsHelper
    {
        public static bool TryGetCommand(ImKeyboardEvent evt, out ImKeyboardCommandFlag command)
        {
            return SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX
                ? TryGetCommandMacOS(evt, out command)
                : TryGetCommandGeneric(evt, out command);
        }

        public static bool TryGetCommandMacOS(ImKeyboardEvent evt, out ImKeyboardCommandFlag result)
        {
            result = ImKeyboardCommandFlag.None;
            
            var arrow = (int)evt.Key >= 273 && (int)evt.Key <= 276;
            var option = evt.Modifiers.HasFlag(EventModifiers.Alt); // option key
            var command = evt.Modifiers.HasFlag(EventModifiers.Command);
            var shift = evt.Modifiers.HasFlag(EventModifiers.Shift);

            if (arrow && command && !option)
            {
                result |= ImKeyboardCommandFlag.JumpEnd;
            }
            else if (arrow && !command && option)
            {
                result |= ImKeyboardCommandFlag.JumpWord;
            }

            if (arrow && shift)
            {
                result |= ImKeyboardCommandFlag.Select;
            }

            if (command && evt.Key == KeyCode.A)
            {
                result |= ImKeyboardCommandFlag.SelectAll;
            }

            if (command && evt.Key == KeyCode.C)
            {
                result |= ImKeyboardCommandFlag.Copy;
            }
            
            if (command && evt.Key == KeyCode.V)
            {
                result |= ImKeyboardCommandFlag.Paste;
            }

            if (command && evt.Key == KeyCode.X)
            {
                result |= ImKeyboardCommandFlag.Cut;
            }

            return result != ImKeyboardCommandFlag.None;
        }

        public static bool TryGetCommandGeneric(ImKeyboardEvent evt, out ImKeyboardCommandFlag command)
        {
            command = ImKeyboardCommandFlag.None;

            var arrow = (int)evt.Key >= 273 && (int)evt.Key <= 276;
            var control = evt.Modifiers.HasFlag(EventModifiers.Control);

            if (arrow && evt.Modifiers.HasFlag(EventModifiers.Shift))
            {
                command |= ImKeyboardCommandFlag.Select;
            }
            
            if (arrow && control)
            {
                command |= ImKeyboardCommandFlag.JumpWord;
            }

            if (control && evt.Key == KeyCode.A)
            {
                command |= ImKeyboardCommandFlag.SelectAll;
            }

            if (control && evt.Key == KeyCode.C)
            {
                command |= ImKeyboardCommandFlag.Copy;
            }

            if (control && evt.Key == KeyCode.V)
            {
                command |= ImKeyboardCommandFlag.Paste;
            }

            if (control && evt.Key == KeyCode.X)
            {
                command |= ImKeyboardCommandFlag.Cut;
            }

            return command != ImKeyboardCommandFlag.None;
        }
    }
}