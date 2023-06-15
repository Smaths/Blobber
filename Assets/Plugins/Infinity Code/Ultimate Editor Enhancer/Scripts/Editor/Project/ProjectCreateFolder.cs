/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using UnityEditor;
using UnityEngine;

namespace InfinityCode.UltimateEditorEnhancer.ProjectTools
{
    [InitializeOnLoad]
    public static class ProjectCreateFolder
    {
        static ProjectCreateFolder()
        {
            ProjectItemDrawer.Register("CREATE_FOLDER", DrawButton);
        }

        private static void DrawButton(ProjectItem item)
        {
            if (!Prefs.projectCreateFolder) return;
            if (!item.isFolder) return;
            if (!item.hovered) return;
            if (!item.path.StartsWith("Assets")) return;

            Rect r = item.rect;
            r.xMin = r.xMax - 18;
            r.height = 16;

            item.rect.xMax -= 18;

            if (GUI.Button(r, TempContent.Get(Icons.addFolder, "Create Subfolder"), GUIStyle.none))
            {
                Selection.activeObject = item.asset;
                ProjectWindowUtil.CreateFolder();
            }
        }
    }
}