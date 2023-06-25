
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace HeurekaGames.Utils
{
    public static class Heureka_AddDefineSymbols
    {
        /// <summary>
        /// Add define symbols as soon as Unity gets done compiling.
        /// </summary>
        public static void AddDefineSymbols(string[] Symbols)
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();

            var newDefines = Symbols.Except(allDefines);
            if (newDefines.Count() > 0)
            {
                Debug.Log($"Adding Compile Symbols {string.Join("; ", newDefines.ToArray())}");
                allDefines.AddRange(newDefines);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(
                    EditorUserBuildSettings.selectedBuildTargetGroup,
                    string.Join(";", allDefines.ToArray()));
            }
        }
    }
}