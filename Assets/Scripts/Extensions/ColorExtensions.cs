using UnityEngine;

namespace Extensions
{
    public static class ColorExtensions
    {
        public static Color clearWhite = new Color(1, 1, 1, 0);

        public static Color ToClearAlpha(this Color color) => color * clearWhite;

        public static Color ToOpaque(this Color color) => new Color(color.r, color.g, color.b, 1);
    }
}