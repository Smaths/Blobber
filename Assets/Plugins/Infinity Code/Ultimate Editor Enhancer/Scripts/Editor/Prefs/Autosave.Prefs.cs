/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.Collections.Generic;
using UnityEditor;

namespace InfinityCode.UltimateEditorEnhancer
{
    public static partial class Prefs
    {
        public static bool saveScenesByTimer = true;
        public static bool saveScenesWhenEnteringPlaymode = true;
        public static int autosaveDelay = 600;

        public class AutosaveManager : PrefManager
        {
            public override IEnumerable<string> keywords
            {
                get { return new[] { "Autosave" }; }
            }

            public override float order
            {
                get { return Order.autosave; }
            }

            public override void Draw()
            {
                EditorGUILayout.LabelField("Autosave");

                EditorGUI.indentLevel++;

                saveScenesWhenEnteringPlaymode = EditorGUILayout.ToggleLeft("On Enter Playmode", saveScenesWhenEnteringPlaymode);
                saveScenesByTimer = EditorGUILayout.ToggleLeft("By Timer", saveScenesByTimer);

                EditorGUI.BeginDisabledGroup(!saveScenesByTimer);

                autosaveDelay = EditorGUILayout.IntField("Delay (seconds)", autosaveDelay);

                EditorGUI.EndDisabledGroup();


                EditorGUI.indentLevel--;
            }
            
            public static void SetState(bool state)
            {
                saveScenesByTimer = state;
                saveScenesWhenEnteringPlaymode = state;
            }
        }
    }
}