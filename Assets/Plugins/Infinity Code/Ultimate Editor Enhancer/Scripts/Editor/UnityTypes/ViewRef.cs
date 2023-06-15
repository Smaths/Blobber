/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEngine;

namespace InfinityCode.UltimateEditorEnhancer.UnityTypes
{
    public static class ViewRef
    {
        private static MethodInfo _setMinMaxSizesMethod;
        private static Type _type;

        private static MethodInfo setMinMaxSizesMethod
        {
            get
            {
                if (_setMinMaxSizesMethod == null)
                {
                    _setMinMaxSizesMethod = Reflection.GetMethod(type, "SetMinMaxSizes", new[] { typeof(Vector2), typeof(Vector2) }, Reflection.InstanceLookup);
                }

                return _setMinMaxSizesMethod;
            }
        }

        public static Type type
        {
            get
            {
                if (_type == null) _type = Reflection.GetEditorType("View");
                return _type;
            }
        }

        public static void SetMinMaxSizes(object view, Vector2 min, Vector2 max)
        {
            setMinMaxSizesMethod.Invoke(view, new object[] {min, max});
        }
    }
}