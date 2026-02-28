namespace Vlys.Utilities.SceneNotes
{
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.UIElements;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor.UIElements;
    using System.Globalization;

    public class SceneNoteBrowserWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset tree;
        [SerializeField] private StyleSheet styleSheet;

        private List<SceneNoteData> allNotes = new List<SceneNoteData>();
        private List<SceneNoteData> filteredNotes = new List<SceneNoteData>();

        private ToolbarSearchField searchField;
        private HashSet<SceneNoteCategory> activeCategories = new HashSet<SceneNoteCategory>();
        private HashSet<Status> activeStatuses = new HashSet<Status>();
        private Foldout statusFoldout;
        private VisualElement statusContainer;

        private Foldout filterFoldout;
        private Foldout categoryFoldout;
        private ScrollView notesScrollView;
        private VisualElement categoryContainer;
       
        [MenuItem("Tools/Vlys Utilities/Scene Notes/Browser")]
        public static void ShowWindow()
        {
            var wnd = GetWindow<SceneNoteBrowserWindow>();
            wnd.titleContent = new GUIContent("Scene Note Browser");
        }

        void OnEnable()
        {
            InitializeCategories();
            InitializeStatuses();
        }
        private void InitializeCategories()
        {
            activeCategories.Clear();

            foreach (SceneNoteCategory category in System.Enum.GetValues(typeof(SceneNoteCategory)))
            {
                activeCategories.Add(category);
            }
        }
        private void InitializeStatuses()
        {
            activeStatuses.Clear();

            foreach (Status status in System.Enum.GetValues(typeof(Status)))
            {
                activeStatuses.Add(status);
            }
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;
            root.Clear();

            if (tree == null)
                return;

            tree.CloneTree(root);

            if (styleSheet != null)
                root.styleSheets.Add(styleSheet);

            searchField = root.Q<ToolbarSearchField>("searchField");
            searchField?.RegisterValueChangedCallback(evt =>
                {
                    FilterNotes(evt.newValue);
                });

            statusFoldout = root.Q<Foldout>("statusFoldout");
            statusContainer = root.Q<VisualElement>("statusContainer");
            BuildStatusFilter();


            filterFoldout = root.Q<Foldout>("FilterFoldout");
            categoryFoldout = root.Q<Foldout>("categoryFoldout");

            notesScrollView = root.Q<ScrollView>("notesScroll");

            categoryContainer = root.Q<VisualElement>("categoryContainer");
         
            BuildCategoryFilter();

            RefreshNotes();
        }      


        private void BuildStatusFilter()
        {
            statusFoldout.text = "Status Filter";
            statusFoldout.value = false;

            foreach (Status status in System.Enum.GetValues(typeof(Status)))
            {
                var toggle = new Toggle(status.ToString())
                {
                    value = activeStatuses.Contains(status)
                };

                toggle.RegisterValueChangedCallback(evt =>
                {
                    if (evt.newValue)
                        activeStatuses.Add(status);
                    else
                        activeStatuses.Remove(status);

                    RefreshNotes();
                });

                statusFoldout.Add(toggle);
            }
        }

        private VisualElement BuildCategoryFilter()
        {
            filterFoldout.text = "Filter";
            filterFoldout.value = false;

            categoryFoldout.text = "Category Filter";
            categoryFoldout.value = false;



            foreach (SceneNoteCategory category in System.Enum.GetValues(typeof(SceneNoteCategory)))
            {
                var toggle = new Toggle(category.ToString())
                {
                    value = activeCategories.Contains(category)
                };

                toggle.RegisterValueChangedCallback(evt =>
                {
                    if (evt.newValue)
                        activeCategories.Add(category);
                    else
                        activeCategories.Remove(category);

                    RefreshNotes();
                });

                categoryFoldout.Add(toggle);
            }

            return categoryFoldout;
        }


        private void RefreshNotes()
        {
            var db = SceneNoteDatabase.GetDatabase();

            if (db != null && db._notes != null)
                allNotes = db._notes;
            else
                allNotes = new List<SceneNoteData>();


            FilterNotes(searchField != null ? searchField.value : string.Empty);
        }

        private void FilterNotes(string query)
        {
            if (notesScrollView == null)
                return;

            notesScrollView.Clear();

            IEnumerable<SceneNoteData> queryFiltered;

            if (string.IsNullOrEmpty(query))
                queryFiltered = allNotes;
            else
                queryFiltered = allNotes
                    .Where(n => !string.IsNullOrEmpty(n.title) &&
                                n.title.ToLower().Contains(query.ToLower()));

            if (activeCategories.Count > 0)
                queryFiltered = queryFiltered
                    .Where(n => activeCategories.Contains(n.category));

            if (activeStatuses.Count > 0)
            {
                queryFiltered = queryFiltered
                    .Where(n =>
                        n.category != SceneNoteCategory.Bug &&
                        n.category != SceneNoteCategory.Issue
                        || activeStatuses.Contains(n.status)
                    );
            }


            filteredNotes = queryFiltered.ToList();

            for (int i = 0; i < filteredNotes.Count; i++)
            {
                 var noteElement = CreateNote(filteredNotes[i], i);
                 notesScrollView.Add(noteElement);
            }
        }



        private VisualElement CreateNote(SceneNoteData note, int index)
        {
            var noteEntry = new VisualElement();
            noteEntry.AddToClassList("note-entry");

            if (index % 2 == 0)
                noteEntry.AddToClassList("note-dark");
            else
                noteEntry.AddToClassList("note-light");

            var topRow = new VisualElement();
            topRow.AddToClassList("note-top-row");

            var titleLabel = new Label($"Title: {note.title}");
            titleLabel.AddToClassList("note-title");

            var categoryLabel = new Label($"Category: {note.category}");
            categoryLabel.AddToClassList("note-category");

            var statusLabel = new Label($"Status: {note.status}");
            statusLabel.AddToClassList("note-status");

            topRow.Add(titleLabel);
            topRow.Add(categoryLabel);
            topRow.Add(statusLabel);

            var middleRow = new VisualElement();
            middleRow.AddToClassList("note-middle-row");

            var linkedTransformField = new ObjectField("Linked Transform");
            linkedTransformField.objectType = typeof(Transform);
            linkedTransformField.AddToClassList("note-position-item");

            // Si SceneNoteData no tiene esta referencia, adaptarlo
            linkedTransformField.value = note.LinkedTransform;
            linkedTransformField.SetEnabled(false);

            var worldPositionField = new Vector3Field("World Pos");
            worldPositionField.value = note.WorldPosition;
            worldPositionField.SetEnabled(false);
            worldPositionField.AddToClassList("note-position-item");

            worldPositionField.RegisterValueChangedCallback(_ =>
            {
                worldPositionField.value = note.WorldPosition;
            });

            middleRow.Add(linkedTransformField);
            middleRow.Add(worldPositionField);

            var bottomRow = new VisualElement();
            bottomRow.AddToClassList("note-bottom-row");

            var foldout = new Foldout();
            foldout.text = "METADATA";
            foldout.value= false;
            var metadataContainer = new VisualElement();
            metadataContainer.AddToClassList("metadata");

            var authorLabel = new Label($"Author: {note.author}");
            authorLabel.AddToClassList("metadata-item");

            var dateLabel = new Label($"Date: {note.Timestamp.ToString("g")}");
            dateLabel.AddToClassList("metadata-item");

            var branchLabel = new Label($"Branch: {note.gitBranch}");
            branchLabel.AddToClassList("metadata-item");

            metadataContainer.Add(authorLabel);
            metadataContainer.Add(dateLabel);
            metadataContainer.Add(branchLabel);

            foldout.Add(metadataContainer);
            bottomRow.Add(foldout);

            var lowBottomRow = new VisualElement();
            lowBottomRow.AddToClassList("note-low-bottom-row");

            var focusButton = new Button(() => FocusNote(note));
            focusButton.text = "Go to Note";
            focusButton.AddToClassList("note-focus-button");

            lowBottomRow.Add(focusButton);

            noteEntry.Add(topRow);
            noteEntry.Add(middleRow);
            noteEntry.Add(bottomRow);
            noteEntry.Add(lowBottomRow);

            return noteEntry;
        }



        private void FocusNote(SceneNoteData note)
        {
            var sceneView = SceneView.lastActiveSceneView;

            if (sceneView != null)
                sceneView.LookAt(note.WorldPosition);
        }
    }
}
