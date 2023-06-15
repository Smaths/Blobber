using UnityEngine;

namespace Extensions
{
    public static class MaterialExtensions
    {
        public static Material SetAlpha(this Material material, float toAlpha)
        {
            Color color = material.color;
            color.a = toAlpha;
            material.color = color;
            return material;
        }
    }
}