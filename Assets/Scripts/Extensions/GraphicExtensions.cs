using UnityEngine;
using UnityEngine.UI;

namespace Extensions
{
    public static class GraphicExtensions
    {
        public static T SetAlpha<T> (this T graphic, float newAlpha)
            where T : Graphic
        {
            Color color = graphic.color;
            color.a = newAlpha;
            graphic.color = color;
            return graphic;
        }
    }
}