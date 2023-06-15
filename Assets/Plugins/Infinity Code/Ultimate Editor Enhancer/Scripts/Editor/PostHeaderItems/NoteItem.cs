/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;

namespace InfinityCode.UltimateEditorEnhancer.PostHeader
{
    [Serializable]
    public class NoteItem
    {
        public string text;
        public string gid;
        public bool expanded;

        public bool isEmpty
        {
            get => string.IsNullOrEmpty(text);
        }
    }
}