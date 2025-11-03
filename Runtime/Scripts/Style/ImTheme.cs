using System;
using UnityEngine;

namespace Imui.Style
{
    [Serializable]
    public struct ImTheme
    {
        public float TextSize;
        public float Spacing;
        public float InnerSpacing;
        public float Indent;
        public float ExtraRowHeight;
        public float ScrollBarSize;
        public float WindowBorderRadius;
        public float WindowBorderThickness;
        public float BorderRadius;
        public float BorderThickness;
        public float ReadOnlyColorMultiplier;

        public Color Background;
        public Color Foreground;
        public Color Control;
        public Color Accent;
        public float Contrast;
        public float BorderContrast;
    }
    
    public struct ImThemePalette
    {
        public bool IsDark;

        public Color Back;
        public Color Front;
        public Color Control;

        public Color Accent;
        public Color AccentFront;
    }

    public static class ImThemeBuiltin
    {
        public static ImTheme Wire()
        {
            return new ImTheme()
            {
                TextSize = 20f,
                Spacing = 3.25f,
                InnerSpacing = 5f,
                Indent = 12f,
                ExtraRowHeight = 4f,
                ScrollBarSize = 13f,
                WindowBorderRadius = 8f,
                WindowBorderThickness = 1f,
                BorderRadius = 5f,
                BorderThickness = 0.98f,
                ReadOnlyColorMultiplier = 0.9f,
                Background = new Color32(255, 255, 255, 255),
                Foreground = new Color32(30, 30, 30, 255),
                Accent = new Color32(194, 230, 255, 255),
                Control = new Color32(255, 255, 255, 255),
                Contrast = 0f,
                BorderContrast = 2f
            };
        }
        
        public static ImTheme LightTouch()
        {
            var theme = Light();

            theme.TextSize = 23f;
            theme.Spacing = 5f;
            theme.InnerSpacing = 6.5f;
            theme.ExtraRowHeight = 11f;

            return theme;
        }
        
        public static ImTheme DarkTouch()
        {
            var theme = Dark();

            theme.TextSize = 23f;
            theme.Spacing = 5f;
            theme.InnerSpacing = 6.5f;
            theme.ExtraRowHeight = 11f;

            return theme;
        }

        public static ImTheme Light()
        {
            return new ImTheme()
            {
                TextSize = 20f,
                Spacing = 3f,
                InnerSpacing = 5f,
                Indent = 12f,
                ExtraRowHeight = 4f,
                ScrollBarSize = 13f,
                WindowBorderRadius = 8f,
                WindowBorderThickness = 1f,
                BorderRadius = 5f,
                BorderThickness = 1f,
                ReadOnlyColorMultiplier = 0.9f,
                Background = new Color32(238, 238, 238, 255),
                Foreground = new Color32(30, 30, 30, 255),
                Accent = new Color32(0, 144, 242, 255),
                Control = new Color32(221, 221, 221, 255),
                Contrast = -0.03f,
                BorderContrast = 0.17f
            };
        }

        public static ImTheme Dark()
        {
            return new ImTheme
            {
                TextSize = 20f,
                Spacing = 3f,
                InnerSpacing = 5f,
                Indent = 12f,
                ExtraRowHeight = 4f,
                ScrollBarSize = 13f,
                WindowBorderRadius = 8f,
                WindowBorderThickness = 1f,
                BorderRadius = 5f,
                BorderThickness = 1f,
                ReadOnlyColorMultiplier = 0.7f,
                Background = new Color32(58, 58, 58, 255),
                Foreground = new Color32(224, 224, 224, 255),
                Accent = new Color32(17, 121, 200, 255),
                Control = new Color32(83, 83, 83, 255),
                Contrast = 0f,
                BorderContrast = -0.04f
            };
        }

        public static ImTheme Dear()
        {
            return new ImTheme
            {
                TextSize = 20f,
                Spacing = 3f,
                InnerSpacing = 3f,
                Indent = 8f,
                ExtraRowHeight = 4f,
                ScrollBarSize = 12f,
                WindowBorderRadius = 0f,
                WindowBorderThickness = 1f,
                BorderRadius = 0f,
                BorderThickness = 0f,
                ReadOnlyColorMultiplier = 0.7f,
                Background = new Color32(10, 10, 10, 242),
                Foreground = new Color32(255, 255, 255, 255),
                Accent = new Color32(89, 148, 243, 255),
                Control = new Color32(75, 114, 200, 118),
            };
        }

        public static ImTheme Orange()
        {
            return new ImTheme
            {
                TextSize = 20f,
                Spacing = 3f,
                InnerSpacing = 5f,
                Indent = 12f,
                ExtraRowHeight = 4f,
                ScrollBarSize = 13f,
                WindowBorderRadius = 8f,
                WindowBorderThickness = 1f,
                BorderRadius = 5f,
                BorderThickness = 1f,
                ReadOnlyColorMultiplier = 0.7f,
                Background = new Color32(17, 18, 18, 245),
                Foreground = new Color32(224, 224, 224, 255),
                Accent = new Color32(211, 85, 12, 255),
                Control = new Color32(0, 121, 255, 11),
            };
        }

        public static ImTheme Terminal()
        {
            return new ImTheme
            {
                TextSize = 18f,
                Spacing = 1f,
                InnerSpacing = 2f,
                Indent = 8f,
                ExtraRowHeight = 0f,
                ScrollBarSize = 15f,
                WindowBorderRadius = 0f,
                WindowBorderThickness = 1f,
                BorderRadius = 0f,
                BorderThickness = 1f,
                ReadOnlyColorMultiplier = 0.7f,
                Background = new Color32(0, 0, 0, 240),
                Foreground = new Color32(18, 255, 0, 255),
                Accent = new Color32(52, 224, 0, 255),
                Control = new Color32(0, 95, 3, 255),
                Contrast = 0f,
                BorderContrast = 0f
            };
        }
    }
}