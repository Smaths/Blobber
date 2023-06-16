/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using InfinityCode.UltimateEditorEnhancer.UnityTypes;
using InfinityCode.UltimateEditorEnhancer.Windows;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.UltimateEditorEnhancer.Interceptors
{
    public class EnumPopupInterceptor: StatedInterceptor<EnumPopupInterceptor>
    {
        protected override MethodInfo originalMethod
        {
            get { return EditorGUIRef.doPopupMethod; }
        }

        public override bool state
        {
            get { return Prefs.searchInEnumFields; }
        }

        protected override string prefixMethodName
        {
            get { return nameof(DoPopupPrefix); }
        }

        private static void DoPopupPrefix(
            Rect position,
            int controlID,
            int selected,
            GUIContent[] popupValues,
            Func<int, bool> checkEnabled,
            GUIStyle style)
        {
            Event e = Event.current;
            if (e.type == EventType.MouseDown) ProcessMouseDown(position, controlID, selected, popupValues, e);
            else if (e.type == EventType.KeyDown) ProcessKeyDown(position, controlID, selected, popupValues, e);
        }

        private static bool HasSlashes(GUIContent[] popupValues)
        {
            for (int i = 0; i < popupValues.Length; i++)
            {
                string text = popupValues[i].text;
                if (!string.IsNullOrEmpty(text) && text.Contains("/")) return true;
            }

            return false;
        }

        internal static bool MainActionKeyForControl(Event e, int controlId)
        {
            if (GUIUtility.keyboardControl != controlId) return false;
            bool flag = e.alt || e.shift || e.command || e.control;
            return e.type == EventType.KeyDown && (e.keyCode == KeyCode.Space || e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter) && !flag;
        }

        private static void ProcessKeyDown(Rect position, int controlID, int selected, GUIContent[] popupValues, Event e)
        {
            if (!MainActionKeyForControl(e, controlID)) return;
            if (popupValues.Length < Prefs.searchInEnumFieldsMinValues) return;
            if (HasSlashes(popupValues)) return;
            
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                position.y = (float)(-19.0 + position.y - selected * 16);
            }

            object instance = Activator.CreateInstance(PopupCallbackInfoRef.type, controlID);
            PopupCallbackInfoRef.SetInstance(instance);
            FlatSelectorWindow.Show(position, popupValues, EditorGUI.showMixedValue ? -1 : selected).OnSelect += i => { PopupCallbackInfoRef.GetSetEnumValueDelegate(instance).Invoke(null, null, i); };
            e.Use();
        }

        private static void ProcessMouseDown(Rect position, int controlID, int selected, GUIContent[] popupValues, Event e)
        {
            if (e.button != 0 || !position.Contains(e.mousePosition)) return;
            if (popupValues.Length < Prefs.searchInEnumFieldsMinValues) return;
            if (HasSlashes(popupValues)) return;
            
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                position.y = (float)(-19.0 + position.y - selected * 16);
            }

            object instance = Activator.CreateInstance(PopupCallbackInfoRef.type, controlID);
            PopupCallbackInfoRef.SetInstance(instance);
            FlatSelectorWindow.Show(position, popupValues, EditorGUI.showMixedValue ? -1 : selected).OnSelect += i => { PopupCallbackInfoRef.GetSetEnumValueDelegate(instance).Invoke(null, null, i); };
            GUIUtility.keyboardControl = controlID;
            e.Use();
        }
    }
}