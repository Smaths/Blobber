/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InfinityCode.UltimateEditorEnhancer.PostHeader
{
    public class HeaderNote : PostHeaderItem
    {
        private static Dictionary<GameObject, NoteItem> cachedNotes = new Dictionary<GameObject, NoteItem>();

        public override void OnBlockGUI(Object target)
        {
            if (!Prefs.inspectorNotes) return;
            if (EditorApplication.isPlaying) return;

            GameObject go = target as GameObject;
            if (go == null) return;

            NoteItem note = null;
            if (!cachedNotes.TryGetValue(go, out note)) return;
            if (!note.expanded) return;

            bool prevEmpty = note.isEmpty;

            EditorGUI.BeginChangeCheck();
            note.text = EditorGUILayout.TextArea(note.text);
            if (EditorGUI.EndChangeCheck())
            {
                if (prevEmpty)
                {
                    if (ReferenceManager.notes.FirstOrDefault(n => n.gid == note.gid) == null) ReferenceManager.notes.Add(note);
                }
                else if (note.isEmpty) ReferenceManager.notes.Remove(note);
                ReferenceManager.Save();
            }
        }

        public override void OnRowGUI(Object target)
        {
            if (!Prefs.inspectorNotes) return;
            if (EditorApplication.isPlaying) return;

            GameObject go = target as GameObject;
            if (go == null) return;

            NoteItem note = null;
            if (!cachedNotes.TryGetValue(go, out note))
            {
                string gid = GlobalObjectId.GetGlobalObjectIdSlow(go).ToString();
                note = ReferenceManager.notes.FirstOrDefault(n => n.gid == gid);
                if (note != null)
                {
                    cachedNotes[go] = note;
                }
                else
                {
                    note = new NoteItem
                    {
                        gid = gid
                    };
                    cachedNotes[go] = note;
                }
            }

            GUIContent content = note.isEmpty ? TempContent.Get(Icons.noteEmpty, "Add Note") : TempContent.Get(Icons.note, "Show/Hide Note");
            note.expanded = Toggle(content, note.expanded);
        }
    }
}