using UnityEngine;

namespace Imui.Style
{
    public struct ImColorHcl
    {
        public float Hue;
        public float Chroma;
        public float Lightness;
        public float Alpha;

        public ImColorHcl(Vector3 hcl, float alpha)
        {
            Hue = hcl.x;
            Chroma = hcl.y;
            Lightness = hcl.z;
            Alpha = alpha;
        }

        public ImColorHcl(float h, float c, float l, float alpha)
        {
            Hue = h;
            Chroma = c;
            Lightness = l;
            Alpha = alpha;
        }

        public ImColorHcl AddLightness(float lightness)
        {
            return new ImColorHcl(Hue, Chroma, Mathf.Clamp(Lightness + lightness, 0, 100), Alpha);
        }

        public ImColorHcl SetLightness(float lightness)
        {
            return new ImColorHcl(Hue, Chroma, Mathf.Clamp(lightness, 0, 100), Alpha);
        }

        public ImColorHcl AddAlpha(float value)
        {
            return new ImColorHcl(Hue, Chroma, Lightness, Mathf.Clamp01(Alpha + value));
        }

        public ImColorHcl MulAlpha(float value)
        {
            return new ImColorHcl(Hue, Chroma, Lightness, Mathf.Clamp01(Alpha * value));
        }

        public ImColorHcl SetAlpha(float value)
        {
            return new ImColorHcl(Hue, Chroma, Lightness, Mathf.Clamp01(value));
        }

        public static implicit operator Color(ImColorHcl col)
        {
            return col.ToRgb();
        }
    }

    public static class ImColorHclUtility
    {
        public static ImColorHcl ToHcl(this Color color) => new(RgbToHcl(color), color.a);

        public static Color ToRgb(this ImColorHcl hcl)
        {
            var rgba = HclToRgb(hcl.Hue, hcl.Chroma, hcl.Lightness);
            rgba.a = hcl.Alpha;
            return rgba;
        }
        
        private const float XN = 95.047f;
        private const float YN = 100.000f;
        private const float ZN = 108.883f;

        private static float PivotXyz(float n)
        {
            return (n > 0.008856f) ? Mathf.Pow(n, 1f / 3f) : (7.787f * n + 16f / 116f);
        }

        private static float InversePivotXyz(float n)
        {
            var n3 = Mathf.Pow(n, 3);
            return (n3 > 0.008856f) ? n3 : (n - 16f / 116f) / 7.787f;
        }

        private static Vector3 RgbToXyz(Color c)
        {
            var R = SrgbToLinear(c.r) * 100f;
            var G = SrgbToLinear(c.g) * 100f;
            var B = SrgbToLinear(c.b) * 100f;

            return new Vector3(R * 0.4124f + G * 0.3576f + B * 0.1805f,
                               R * 0.2126f + G * 0.7152f + B * 0.0722f,
                               R * 0.0193f + G * 0.1192f + B * 0.9505f);
        }

        private static Color XyzToRgb(Vector3 xyz)
        {
            var x = xyz.x / 100f;
            var y = xyz.y / 100f;
            var z = xyz.z / 100f;

            var R = x * 3.2406f + y * -1.5372f + z * -0.4986f;
            var G = x * -0.9689f + y * 1.8758f + z * 0.0415f;
            var B = x * 0.0557f + y * -0.2040f + z * 1.0570f;

            return new Color(Mathf.Clamp01(LinearToSrgb(R)),
                             Mathf.Clamp01(LinearToSrgb(G)),
                             Mathf.Clamp01(LinearToSrgb(B)),
                             1f);
        }

        private static Vector3 XyzToLab(Vector3 xyz)
        {
            var fx = PivotXyz(xyz.x / XN);
            var fy = PivotXyz(xyz.y / YN);
            var fz = PivotXyz(xyz.z / ZN);

            var L = 116f * fy - 16f;
            var a = 500f * (fx - fy);
            var b = 200f * (fy - fz);

            return new Vector3(L, a, b);
        }

        private static Vector3 LabToXyz(Vector3 lab)
        {
            var fy = (lab.x + 16f) / 116f;
            var fx = fy + (lab.y / 500f);
            var fz = fy - (lab.z / 200f);

            var xr = InversePivotXyz(fx);
            var yr = InversePivotXyz(fy);
            var zr = InversePivotXyz(fz);

            return new Vector3(xr * XN, yr * YN, zr * ZN);
        }

        public static Vector3 RgbToHcl(Color color)
        {
            var lab = XyzToLab(RgbToXyz(color));

            var h = Mathf.Atan2(lab.z, lab.y); // atan2(b, a)
            if (h < 0)
            {
                h += 2f * Mathf.PI;
            }

            var hue = h * Mathf.Rad2Deg;

            var chroma = Mathf.Sqrt(lab.y * lab.y + lab.z * lab.z);
            var luminance = lab.x;

            return new Vector3(hue, chroma, luminance); // HCL
        }

        public static Color HclToRgb(float hue, float chroma, float luminance)
        {
            var hRad = hue * Mathf.Deg2Rad;

            var a = Mathf.Cos(hRad) * chroma;
            var b = Mathf.Sin(hRad) * chroma;

            var lab = new Vector3(luminance, a, b);
            var xyz = LabToXyz(lab);

            return XyzToRgb(xyz);
        }

        private static float SrgbToLinear(float c)
        {
            return (c <= 0.04045f) ? c / 12.92f : Mathf.Pow((c + 0.055f) / 1.055f, 2.4f);
        }

        private static float LinearToSrgb(float c)
        {
            return (c <= 0.0031308f) ? 12.92f * c : 1.055f * Mathf.Pow(c, 1.0f / 2.4f) - 0.055f;
        }
    }
}