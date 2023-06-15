/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using UnityEditor;
using UnityEngine;

namespace InfinityCode.UltimateEditorEnhancer.PostHeader
{
    public abstract class PostHeaderItem
    {
        public virtual int rowOrder
        {
            get => 0;
        }

        public virtual int blockOrder
        {
            get => 0;
        }

        public virtual bool Button(GUIContent content)
        {
            return GUILayout.Button(content, EditorStyles.toolbarButton, GUILayout.Width(20), GUILayout.Height(20));
        }

        public virtual void OnRowGUI(Object target)
        {

        }

        public virtual void OnBlockGUI(Object target)
        {

        }

        public virtual bool Toggle(GUIContent content, bool value)
        {
            return GUILayout.Toggle(value, content, EditorStyles.toolbarButton, GUILayout.Width(20), GUILayout.Height(20));
        }
    }
}