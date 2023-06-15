/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using InfinityCode.UltimateEditorEnhancer.Integration;
using InfinityCode.UltimateEditorEnhancer.Windows;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.UltimateEditorEnhancer.HierarchyTools
{
    [InitializeOnLoad]
    public static class BookmarkButton
    {
        private static Texture2D offTexture;

        static BookmarkButton()
        {
            HierarchyItemDrawer.Register("BookmarkButton", OnHierarchyItem, HierarchyToolOrder.BOOKMARK);
        }

        private static void OnHierarchyItem(HierarchyItem item)
        {
            if (!Prefs.hierarchyBookmarks) return;
            if (Prefs.hierarchyIconsDisplayRule == HierarchyIconsDisplayRule.always) return;
            if (item.gameObject == null) return;

            Event e = Event.current;
            if (e.modifiers != EventModifiers.None) return;

            bool contain = Bookmarks.Contain(item.gameObject);
            if (!contain)
            {
                if (Prefs.hierarchyIconsDisplayRule == HierarchyIconsDisplayRule.onHover || !item.hovered) return;
            }

            if (offTexture == null) offTexture = Styles.isProSkin ? (Texture2D)Icons.starWhite : (Texture2D)Icons.starBlack;

            Rect rect = item.rect;
            Rect r = new Rect(rect.xMax - 16, rect.y, 16, rect.height);

            if (Cinemachine.ContainBrain(item.gameObject)) r.x -= 16;

            Texture2D texture = offTexture;
            string tooltip = "Add Bookmark";

            if (contain)
            {
                texture = (Texture2D)Icons.starYellow;
                tooltip = "Remove Bookmark";
            }

            GUIContent content = TempContent.Get(texture, tooltip);

            if (Prefs.hierarchyIconsDisplayRule == HierarchyIconsDisplayRule.onHover)
            {
                if (e.type == EventType.Repaint) GUIStyle.none.Draw(r, content, -1);
                return;
            }

            if (e.type == EventType.MouseUp && e.button == 1 && r.Contains(e.mousePosition))
            {
                Bookmarks.ShowWindow();
                e.Use();
            }

            if (GUI.Button(r, content, GUIStyle.none))
            {
                if (contain) Bookmarks.Remove(item.gameObject);
                else Bookmarks.Add(item.gameObject);
            }
        }
    }
}