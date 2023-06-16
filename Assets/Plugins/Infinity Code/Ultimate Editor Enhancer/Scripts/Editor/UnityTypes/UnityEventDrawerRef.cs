/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace InfinityCode.UltimateEditorEnhancer.UnityTypes
{
    public static class UnityEventDrawerRef
    {
        private static MethodInfo _buildPopupListMethod;
        private static MethodInfo _generatePopUpForTypeMethod;

        private static MethodInfo buildPopupListMethod
        {
            get
            {
                if (_buildPopupListMethod == null)
                {
                    _buildPopupListMethod = Reflection.GetMethod(
                        type, 
                        "BuildPopupList", 
                        new[] { typeof(Object), typeof(UnityEventBase), typeof(SerializedProperty) }, 
                        Reflection.StaticLookup);
                }

                return _buildPopupListMethod;
            }
        }
        
        private static MethodInfo generatePopUpForTypeMethod
        {
            get
            {
                if (_generatePopUpForTypeMethod == null)
                {
                    _generatePopUpForTypeMethod = Reflection.GetMethod(
                        type, 
                        "GeneratePopUpForType", 
                        new[] { typeof(GenericMenu), typeof(Object), typeof(string), typeof(SerializedProperty), typeof(Type[]) }, 
                        Reflection.StaticLookup);
                }

                return _generatePopUpForTypeMethod;
            }
        }

        public static Type type
        {
            get { return typeof(UnityEventDrawer); }
        }

        public static GenericMenu BuildPopupList(Object target, UnityEventBase dummyEvent, SerializedProperty listener)
        {
            return (GenericMenu) buildPopupListMethod.Invoke(
                null, 
                new object[]
                {
                    target, 
                    dummyEvent, 
                    listener
                }); 
        }

        public static void GeneratePopUpForType(
            GenericMenu menu,
            Object target,
            string targetName,
            SerializedProperty listener,
            Type[] delegateArgumentsTypes)
        {
            generatePopUpForTypeMethod.Invoke(
                null, 
                new object[]
                {
                    menu, 
                    target, 
                    targetName, 
                    listener, 
                    delegateArgumentsTypes
                });
        }
    }
}