/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Collections.Generic;
using InfinityCode.UltimateEditorEnhancer.JSON;
using InfinityCode.UltimateEditorEnhancer.Windows;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace InfinityCode.UltimateEditorEnhancer.InspectorTools
{
    public class EmptyInspector: InspectorInjector
    {
        private const string ELEMENT_NAME = "EmptyInspector";
        private const string SEARCHFIELD_NAME = "UEEEmptyInspectorSearchField";

        private static EmptyInspector instance;
        private static VisualElement visualElement;
        private static string filterText;

        public EmptyInspector()
        {
            EditorApplication.delayCall += InitInspector;
            WindowManager.OnMaximizedChanged += OnMaximizedChanged;
            Selection.selectionChanged += InitInspector;
        }

        private static void CreateButton(VisualElement parent, string submenu, string text)
        {
            ToolbarButton button = new ToolbarButton(() => EditorApplication.ExecuteMenuItem(submenu));
            button.text = text;
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
            button.style.left = 0;
            button.style.borderLeftWidth = button.style.borderRightWidth = 0;
            parent.Add(button);
        }

        private VisualElement CreateContainer(VisualElement parent)
        {
            VisualElement el = new VisualElement();
            el.style.borderBottomWidth = el.style.borderTopWidth = el.style.borderLeftWidth = el.style.borderRightWidth = 1;
            el.style.borderBottomColor = el.style.borderTopColor = el.style.borderLeftColor = el.style.borderRightColor = Color.gray;
            el.style.marginLeft = 3;
            el.style.marginRight = 5;
            parent.Add(el);
            return el;
        }

        private static void CreateLabel(VisualElement parent, string text)
        {
            Label label = new Label(text);
            label.style.marginTop = 10;
            label.style.marginLeft = label.style.marginRight = 3;
            label.style.paddingLeft = 5;
            parent.Add(label);
        }

        private void DrawFilterTextField()
        {
            GUILayout.BeginHorizontal();
            GUI.SetNextControlName(SEARCHFIELD_NAME);
            EditorGUI.BeginChangeCheck();
            filterText = GUILayoutUtils.ToolbarSearchField(filterText);
            if (EditorGUI.EndChangeCheck()) UpdateFilteredItems();

            if (GUILayout.Button(TempContent.Get(EditorIconContents.settings.image, "Settings"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                Settings.OpenEmptyInspectorSettings();
            }

            if (GUILayout.Button(TempContent.Get("?", "Help"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                Links.OpenDocumentation("empty-inspector");
            }

            GUILayout.EndHorizontal();
        }

        [InitializeOnLoadMethod]
        private static void Init()
        {
            instance = new EmptyInspector();
        }

        private void InitItems(VisualElement parent)
        {
            InitTopContent(parent);

            List<Group> groups = ReferenceManager.emptyInspectorItems;

            for (int i = 0; i < groups.Count; i++)
            {
                Group g = groups[i];
                if (!g.enabled || g.count == 0) continue;

                CreateLabel(parent, g.title);
                VisualElement container = CreateContainer(parent);
                for (int j = 0; j < g.items.Count; j++)
                {
                    Item item = g.items[j];
                    if (!item.enabled) continue;

                    CreateButton(container, item.menuPath, item.title);
                }
            }
        }

        private void InitTopContent(VisualElement parent)
        {
            VisualElement topContent = new VisualElement();
            parent.Add(topContent);

            Updater.CheckNewVersionAvailable();

            if (Updater.hasNewVersion)
            {
                ToolbarButton updateAvailable = new ToolbarButton(Updater.OpenWindow);
                updateAvailable.text = "A new version of Ultimate Editor Enhancer is available.\nClick to open the built-in update system.";
                updateAvailable.tooltip = "Update Available.\nClick to open the built-in update system.";
                updateAvailable.style.backgroundColor = (Color) new Color32(255, 0, 0, 128);
                updateAvailable.style.unityTextAlign = TextAnchor.MiddleCenter;
                updateAvailable.style.color = Color.white;
                updateAvailable.style.left = 0;
                updateAvailable.style.paddingTop = updateAvailable.style.paddingBottom = 5;
                updateAvailable.style.borderLeftWidth = updateAvailable.style.borderRightWidth = 0;
                topContent.Add(updateAvailable);
            }

            Label helpbox = new Label("Nothing selected");
            helpbox.style.backgroundColor = Color.gray;
            helpbox.style.height = 30;
            helpbox.style.unityTextAlign = TextAnchor.MiddleCenter;
            topContent.Add(helpbox);

            IMGUIContainer search = new IMGUIContainer(DrawFilterTextField);
            search.style.marginTop = 5;
            search.style.marginLeft = 5;
            search.style.marginRight = 5;
            parent.Add(search);
        }

        protected override bool OnInject(EditorWindow wnd, VisualElement mainContainer, VisualElement editorsList)
        {
            if (editorsList.parent[0].name == ELEMENT_NAME) editorsList.parent.RemoveAt(0);
            if (!Prefs.emptyInspector || ReferenceManager.emptyInspectorItems.Count == 0) return false;
            if (editorsList.childCount != 0 || float.IsNaN(editorsList.layout.width)) return false;

            if (visualElement == null)
            {
                visualElement = new VisualElement();
                visualElement.name = ELEMENT_NAME;
                InitItems(visualElement);
            }
            editorsList.parent.Insert(0, visualElement);
            filterText = "";
            UpdateFilteredItems();

            return true;
        }

        public static void ResetCachedItems()
        {
            if (visualElement != null)
            {
                if (instance != null)
                {
                    visualElement.Clear();
                    instance.InitItems(visualElement);
                }
                else visualElement = null;
            }
        }

        private void UpdateFilteredItems()
        {
            string t = filterText.Trim();
            if (string.IsNullOrEmpty(t))
            {
                for (int i = 2; i < visualElement.childCount; i += 2)
                {
                    visualElement[i].style.display = DisplayStyle.Flex;
                    VisualElement container = visualElement[i + 1];
                    container.style.display = DisplayStyle.Flex;
                    for (int j = 0; j < container.childCount; j++)
                    {
                        container[j].style.display = DisplayStyle.Flex;
                    }

                }
                return;
            }

            string pattern = SearchableItem.GetPattern(t);

            for (int i = 3; i < visualElement.childCount; i += 2)
            {
                VisualElement el = visualElement[i];

                bool hasVisible = false;
                VisualElement container = el as VisualElement;
                for (int j = 0; j < container.childCount; j++)
                {
                    ToolbarButton b = container[j] as ToolbarButton;
                    bool visible = SearchableItem.Match(pattern, b.text);
                    if (visible)
                    {
                        b.style.display = DisplayStyle.Flex;
                        hasVisible = true;
                    }
                    else b.style.display = DisplayStyle.None;
                }

                if (hasVisible)
                {
                    visualElement[i - 1].style.display = DisplayStyle.Flex;
                    el.style.display = DisplayStyle.Flex;
                }

                else
                {
                    visualElement[i - 1].style.display = DisplayStyle.None;
                    el.style.display = DisplayStyle.None;
                }
            }
        }

        [Serializable]
        public class Group
        {
            public string title;
            public bool enabled = true;
            public List<Item> items;

            public int count
            {
                get => items.Count;
            }

            public JsonObject json
            {
                get
                {
                    return Json.Serialize(this) as JsonObject;
                }
            }

            public Group()
            {

            }

            public Group(string title)
            {
                this.title = title;
                items = new List<Item>();
            }

            public Group(string title, List<Item> items)
            {
                this.title = title;
                this.items = items;
            }

            public void Add(Item item)
            {
                items.Add(item);
            }
        }

        [Serializable]
        public class Item
        {
            public string title;
            public string menuPath;
            public bool enabled = true;
        }
    }
}