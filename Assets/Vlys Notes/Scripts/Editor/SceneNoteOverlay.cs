namespace Vlys.Utilities.SceneNotes
{
    using System;
    using System.Collections.Generic;


#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.Hardware;
    using UnityEditor.Overlays;
    using UnityEngine;
    using UnityEngine.UIElements;

    [Overlay(typeof(SceneView), "Scene Notes")]
    public class SceneNoteOverlay : Overlay
    {

        public static SceneNoteFilterState filterState = new SceneNoteFilterState();
        public static bool isActive = true;

        List<Toggle> statusToggles = new();
        private SceneNoteUIIconConfig config;
        private VisualTreeAsset uxml;
        private StyleSheet uss;

        public override VisualElement CreatePanelContent()
        {
            isActive = true;
            filterState.EnsureInitialized();

            uxml = LoadUxml();
            uss = LoadUss();

            if (uxml == null)
                throw new Exception("No uxml");

            if (uss == null)
                throw new Exception("No uss");

            var root = new VisualElement();

            uxml.CloneTree(root);
            root.styleSheets.Add(uss);

            BuildUI(root);

            return root;
        }
        public override void OnCreated()
        {
            EditorApplication.update += CheckDisplayState;
        }

        void CheckDisplayState()
        {
            isActive = displayed;
        }




        private static VisualTreeAsset LoadUxml()
        {
            string[] guids = AssetDatabase.FindAssets("t:VisualTreeAsset SceneNoteOverlay");

            if (guids.Length == 0)
                return null;

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
        }
        private static StyleSheet LoadUss()
        {
            string[] guids = AssetDatabase.FindAssets("t:StyleSheet SceneNoteOverlay");

            if (guids.Length == 0)
                return null;

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
        }

        private void LoadUIIcon()
        {
            string[] guids = AssetDatabase.FindAssets("t:SceneNoteUIIconConfig");

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                config = AssetDatabase.LoadAssetAtPath<SceneNoteUIIconConfig>(path);
            }
        }

        private void BuildUI(VisualElement root)
        {
            LoadUIIcon();

            root.Clear();
            uxml.CloneTree(root);
            root.styleSheets.Add(uss);


            var createButton = root.Q<Button>("createButton");
            var masterCategoryToggle = root.Q<Toggle>("masterToggle");
            var categoryContainer = root.Q<VisualElement>("categoryContainer");
            var statusMasterToggle = root.Q<Toggle>("statusMasterToggle");
            var statusContainer = root.Q<VisualElement>("statusContainer");

            var FoldoutCat = root.Q<Foldout>("foldoutCategory");
            var FoldoutStatus = root.Q<Foldout>("foldoutStatus");

            FoldoutCat.value = false;
            FoldoutStatus.value = false;

            createButton.AddToClassList("is-not-Selected");
            SceneNoteTool.onCreationChanged += (isSelected) =>
            {

                if (isSelected)
                {
                    createButton.AddToClassList("isSelected");
                    createButton.RemoveFromClassList("is-not-Selected");
                }
                else
                {
                    createButton.RemoveFromClassList("isSelected");
                    createButton.AddToClassList("is-not-Selected");
                }
            };
            createButton.clicked += () =>
            {
                SceneNoteTool.SetCreateMode(true);
            };

#if UNITY_6000_0_OR_NEWER
            createButton.iconImage = config.AddIcon;
#endif

            masterCategoryToggle.value = filterState.AreAllCategoriesActive();

            masterCategoryToggle.RegisterValueChangedCallback(evt =>
            {
                filterState.SetAllCategories(evt.newValue);
                SceneView.RepaintAll();
                RefreshCategoryToggles(masterCategoryToggle);
            });

            togglesCategory.Clear();
            foreach (SceneNoteCategory category in Enum.GetValues(typeof(SceneNoteCategory)))
            {
                var toggle = new Toggle(category.ToString())
                {
                    value = filterState.ShouldBeVisible(category)
                };

                toggle.RegisterValueChangedCallback(evt =>
                {
                    filterState.ToggleCategory(category, evt.newValue);
                    SceneView.RepaintAll();

                });
                togglesCategory.Add(toggle);
                categoryContainer.Add(toggle);
            }


            statusMasterToggle.value = filterState.AreAllStatusesActive();

            statusMasterToggle.RegisterValueChangedCallback(evt =>
            {
                filterState.SetAllStatuses(evt.newValue);
                SceneView.RepaintAll();
                RefreshStatusToggles(statusMasterToggle);
            });

            statusToggles.Clear();

            foreach (Status status in Enum.GetValues(typeof(Status)))
            {
                var toggle = new Toggle(status.ToString())
                {
                    value = filterState.IsStatusVisible(status)
                };

                toggle.RegisterValueChangedCallback(evt =>
                {
                    filterState.ToggleStatus(status, evt.newValue);
                    SceneView.RepaintAll();

                });

                statusToggles.Add(toggle);
                statusContainer.Add(toggle);
            }


        }
        List<Toggle> togglesCategory = new();
        private void RefreshStatusToggles(Toggle masterToggle)
        {
            foreach (var toggle in statusToggles)
                toggle.SetEnabled(masterToggle.value);

            masterToggle.SetValueWithoutNotify(filterState.AreAllStatusesActive());
        }
        private void RefreshCategoryToggles(Toggle masterCategoryToggle)
        {
            foreach (var toggle in togglesCategory)
            {
                toggle.SetEnabled(masterCategoryToggle.value);
            }

            masterCategoryToggle.SetValueWithoutNotify(filterState.AreAllCategoriesActive());
        }

    }
#endif
}