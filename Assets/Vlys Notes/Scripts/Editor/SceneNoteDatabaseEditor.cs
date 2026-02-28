namespace Vlys.Utilities.SceneNotes
{
#if UNITY_EDITOR
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.UIElements;
    using UnityEditor.UIElements;
    using System;
    using System.Globalization;

    [CustomEditor(typeof(SceneNoteDatabase))]
    public class SceneNoteDatabaseEditor : Editor
    {
        private SceneNoteDatabase database;
        private SceneNoteData targetNote;
        private SceneNoteData snapshot;

        [SerializeField] private VisualTreeAsset tree;
        [SerializeField] private StyleSheet style;

        private VisualElement root;
        private IVisualElementScheduledItem schedule1;
        private IVisualElementScheduledItem schedule2;
        private string currentSelectedId;

        private VisualElement headerContainer;
        private VisualElement statusContainerHeader;

        private VisualElement readOnlyField;
        private Label titleLabel;
        private Label statusLabelHeader;

        private TextField titleField;
        private TextField textField;
        private Vector3Field positionField;
        private EnumField statusField;
        private EnumField categoryField;
        private ObjectField linkedField;
        private Foldout foldoutMetadata;

        private Button enableEditButton;
        private Button applyButton;
        private Button postButton;
        private Button deleteButton;
        public static bool isActive { get; private set; }

        private string SelectedNoteId
        {
            get => SceneNoteSelection.Get(database);
            set => SceneNoteSelection.Set(database, value);
        }

        void OnEnable()
        {
            isActive = true;
            LoadUIIcon();
        }
        void OnDisable()
        {
            schedule1?.Pause();
            schedule2?.Pause();
            isActive = false;
        }

        public override VisualElement CreateInspectorGUI()
        {
            database = (SceneNoteDatabase)target;
            root = new VisualElement();

            Rebuild();

            schedule1 = root.schedule.Execute(CheckSelectionChange).Every(200);
            schedule2 = root.schedule.Execute(() =>
                {
                    if (HasValidSelection() && !targetNote.isLocked)
                    {
                        if (positionField == null) return;
                        positionField.SetValueWithoutNotify(targetNote.position);
                    }
                }).Every(50);


            return root;
        }

        private void Rebuild()
        {
            root.Clear();
            tree.CloneTree(root);
            root.styleSheets.Add(style);

            if (!HasValidSelection())
            {
                ShowEmptyState();
                return;
            }
            SetIcon(targetNote.category);
            CacheElements();
            BindNote();
            BindInteractions();
        }

        private void SetIcon(SceneNoteCategory noteCategory)
        {
            var icon = database.IconConfig.GetCategoryIcon(noteCategory);

            EditorGUIUtility.SetIconForObject(target, icon);
        }

        private void ShowEmptyState()
        {
            root.Clear();

            var help = new HelpBox(
                "Add a note to start editing.",
                HelpBoxMessageType.Info
            );

            help.style.marginTop = 8;
            root.Add(help);
        }

        private bool HasValidSelection()
        {
            var id = SelectedNoteId;

            if (string.IsNullOrEmpty(id))
                return false;

            targetNote = database.GetNoteById(id);

            return targetNote != null;
        }


        private void CheckSelectionChange()
        {
            bool changed = SelectedNoteId != currentSelectedId;

            if (changed)
            {
                currentSelectedId = SelectedNoteId;
                Rebuild();
            }
        }


        private void CacheElements()
        {
            headerContainer = root.Q<VisualElement>("headerContainer");
            statusContainerHeader = root.Q<VisualElement>("statusContainerHeader");
            titleLabel = root.Q<Label>("noteTitle");

            statusLabelHeader = root.Q<Label>("statusLabelHeader");

            readOnlyField = root.Q<VisualElement>("readonlyField");


            titleField = root.Q<TextField>("titleField");
            textField = root.Q<TextField>("textField");

            //positionField = root.Q<Vector3Field>("positionField");

            var _categoryContainer = root.Q<VisualElement>("categoryField");
            var _positionContainer = root.Q<VisualElement>("positionField");
            var _statusContainer = root.Q<VisualElement>("statusField");


            positionField = new Vector3Field("World Position");
            _positionContainer.Add(positionField);

            statusField = new EnumField();
            _statusContainer.Add(statusField);

            categoryField = new EnumField();
            _categoryContainer.Add(categoryField);
            _categoryContainer.name = "Category";


            //  statusField = root.Q<EnumField>("statusField");
            // categoryField = root.Q<EnumField>("categoryField");
            linkedField = root.Q<ObjectField>("linkedTransformField");

            var Foldout = root.Q<Foldout>("commentsFoldout");
            Foldout.value = false;
            Foldout.RegisterValueChangedCallback(OnOpenFoldOut);



            enableEditButton = root.Q<Button>("enableEditingButton");
            applyButton = root.Q<Button>("applyChangesButton");
            postButton = root.Q<Button>("postCommentButton");
            deleteButton = root.Q<Button>("deleteNoteButton");
        }



        private void BindNote()
        {
            titleLabel.text = targetNote.title;

            BindFields();

            SetupResizableComments();

            UpdateLockState();
            BindMetadata();
        }


        private void BindFields()
        {
            titleField.value = targetNote.title;
            textField.value = targetNote.text;
            textField.multiline = true;

            // Muestra posición relativa si hay linkedTransform, sino posición mundial
            positionField.value = targetNote.hasAttach ? targetNote.position : targetNote.WorldPosition;
            positionField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(database, "Move Scene Note");

                if (targetNote.LinkedTransform != null)
                {
                    // Al modificar el campo, actualizamos la posición relativa
                    targetNote.position = evt.newValue;
                }
                else
                {
                    targetNote.WorldPosition = evt.newValue;
                }

                EditorUtility.SetDirty(database);
                SceneView.RepaintAll();
            });

            statusField.Init(targetNote.status);

            statusField.value = targetNote.status;

            categoryField.Init(targetNote.category);
            categoryField.RegisterValueChangedCallback(evt =>
            {
                var value = (SceneNoteCategory)evt.newValue;
                categoryField.value = (SceneNoteCategory)evt.newValue;
                SetIcon(value);
                var isBugOrIssue = value == SceneNoteCategory.Bug || value == SceneNoteCategory.Issue;

                statusField.SetEnabled(isBugOrIssue);

                statusField.style.display = !isBugOrIssue ? DisplayStyle.None : DisplayStyle.Flex;

            });

            categoryField.value = targetNote.category;


            var isBugOrIssue = targetNote.category == SceneNoteCategory.Bug || targetNote.category == SceneNoteCategory.Issue;

            statusField.SetEnabled(isBugOrIssue);

            statusField.style.display = !isBugOrIssue ? DisplayStyle.None : DisplayStyle.Flex;

            linkedField.objectType = typeof(Transform);
            linkedField.allowSceneObjects = true;
            linkedField.value = targetNote.LinkedTransform;

            linkedField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(database, "Change Linked Transform");
                targetNote.LinkTo(evt.newValue as Transform);

                // Si ahora hay linkedTransform, convertimos la posición a local
                if (targetNote.LinkedTransform != null)
                    targetNote.position = targetNote.WorldPosition - targetNote.LinkedTransform.position;

                EditorUtility.SetDirty(database);
                SceneView.RepaintAll();
                UpdatePositionLabel();
            });
            UpdatePositionLabel();
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

        private void BindInteractions()
        {
            enableEditButton.text = "Enable Editing";
            applyButton.text = "Apply Changes";
            postButton.text = "Post Comment";
            deleteButton.text = "Delete Note";

            applyButton.AddToClassList("button-normal");
            deleteButton.AddToClassList("button-delete");

            enableEditButton.clicked += UnlockNote;
            applyButton.clicked += ApplyChanges;
            deleteButton.clicked += DeleteNote;

            enableEditButton.text = "";
            enableEditButton.style.backgroundImage = config.EditIcon;

            statusField.RegisterValueChangedCallback(OnStatusChanged);
            categoryField.RegisterValueChangedCallback(OnCategoryChanged);
            linkedField.RegisterValueChangedCallback(OnLinkedChanged);
        }

        private void OnStatusChanged(ChangeEvent<Enum> evt)
        {
            Undo.RecordObject(database, "Change Status");
            targetNote.status = (Status)evt.newValue;
            EditorUtility.SetDirty(database);
        }

        private void UnlockNote()
        {
            targetNote.Unlock();
            snapshot = targetNote.Clone();
            UpdateLockState();
        }

        private void ApplyChanges()
        {
            if (string.IsNullOrWhiteSpace(titleField.value))
            {
                EditorUtility.DisplayDialog(
                    "Invalid Title",
                    "The note must have a title before saving.",
                    "Ok"
                );

                Debug.LogWarning("SceneNote: Cannot save note without title.");
                return;
            }

            Undo.RecordObject(database, "Apply Scene Note Changes");

            targetNote.position = positionField.value;
            targetNote.title = titleField.value;
            targetNote.text = textField.value;

            targetNote.AddSystemChangeComment(
                snapshot,
                LocalUser.GetUserName(),
                LocalUser.GetUserId()
            );

            targetNote.Lock();

            EditorUtility.SetDirty(database);
            UpdateLockState();
        }


        private void DeleteNote()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Delete Scene Note",
                "Are you sure you want to delete this note?",
                "Delete",
                "Cancel"
            );

            if (!confirmed)
                return;

            Undo.RecordObject(database, "Delete Scene Note");

            database.SetNoteAsDeleted(targetNote);

            SceneNoteSelection.Clear(database);

            EditorUtility.SetDirty(database);

            Rebuild();
        }

        private void OnCategoryChanged(ChangeEvent<Enum> evt)
        {
            Undo.RecordObject(database, "Change Category");
            targetNote.category = (SceneNoteCategory)evt.newValue;
            EditorUtility.SetDirty(database);
        }

        private void OnLinkedChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            Undo.RecordObject(database, "Change Linked Object");

            targetNote.LinkTo(evt.newValue as Transform);
            EditorUtility.SetDirty(database);
            UpdatePositionLabel();
        }

        private void UpdateLockState()
        {
            bool editable = !targetNote.isLocked;

            titleField.SetEnabled(editable);
            textField.SetEnabled(editable);
            positionField.SetEnabled(editable);
            linkedField.SetEnabled(editable);
            categoryField.SetEnabled(editable);
            statusField.SetEnabled(editable);

            var isBugOrIssue = targetNote.category == SceneNoteCategory.Bug || targetNote.category == SceneNoteCategory.Issue;

            var readonlyComment = root.Q<Label>("readonlyCommentLabel");

            if (editable)
            {
                root.RemoveFromClassList("locked-layout");

                headerContainer.style.display = DisplayStyle.None;
                titleField.style.display = DisplayStyle.Flex;


                statusField.SetEnabled(isBugOrIssue);

                statusField.style.display = !isBugOrIssue ? DisplayStyle.None : DisplayStyle.Flex;

                textField.style.display = DisplayStyle.Flex;
                readOnlyField.style.display = DisplayStyle.None;
                readonlyComment.style.display = DisplayStyle.None;
                applyButton.style.display = DisplayStyle.Flex;
            }
            else
            {
                root.AddToClassList("locked-layout");

                headerContainer.style.display = DisplayStyle.Flex;


                if (!isBugOrIssue)
                    statusContainerHeader.style.display = DisplayStyle.None;
                else
                {
                    statusContainerHeader.style.display = DisplayStyle.Flex;
                    statusContainerHeader.style.backgroundColor = GetColorFromStatus(targetNote.status);
                }

                string statusText = ObjectNames.NicifyVariableName(targetNote.status.ToString());
                statusLabelHeader.text = statusText;
                statusLabelHeader.style.color = Color.black;
                titleLabel.text = targetNote.title;

                titleField.style.display = DisplayStyle.None;
                statusField.style.display = DisplayStyle.None;

                readOnlyField.style.display = DisplayStyle.Flex;
                readonlyComment.text = string.IsNullOrEmpty(targetNote.text) ? "Nothing Here..." : targetNote.text;
                readonlyComment.style.display = DisplayStyle.Flex;
                textField.style.display = DisplayStyle.None;

                applyButton.style.display = DisplayStyle.None;
            }
        }

        private StyleColor GetColorFromStatus(Status status)
        {
            return status switch
            {
                Status.Open => ColorUtils.HexToColor("6EE7A8"),
                Status.InProgress => ColorUtils.HexToColor("FFE66D"),
                Status.Resolved => ColorUtils.HexToColor("7ED6FC"),
                Status.Closed => ColorUtils.HexToColor("FF6B6B"),
                _ => Color.white
            };
        }

        private void UpdatePositionLabel()
        {
            positionField.label = targetNote.hasAttach ? "Local Position" : "World Position";
        }



        private void BindMetadata()
        {
            root.Q<Label>("authorLabel").text = "Author: " + targetNote.author;
            root.Q<Label>("createdLabel").text = "Created: " + targetNote.Timestamp;
            root.Q<Label>("branchLabel").text = "Branch: " + targetNote.gitBranch;
        }

        private HelpBox emptyCommentsHelpBox;
        private SceneNoteUIIconConfig config;

        private void OnOpenFoldOut(ChangeEvent<bool> evt)
        {
            if (evt.newValue)
            {
                SetupComments(root);
            }
        }
        private void SetupComments(VisualElement root)
        {
            var list = root.Q<ScrollView>("commentsList");
            list.Clear();
            var count = targetNote.comments.Count;

            if (count == 0)
            {
                emptyCommentsHelpBox = new HelpBox("Here you can add comments to keep a track on this note.", HelpBoxMessageType.Info);
                emptyCommentsHelpBox.style.unityTextAlign = TextAnchor.MiddleCenter;
                emptyCommentsHelpBox.style.alignSelf = Align.Center;
                emptyCommentsHelpBox.style.marginTop = 8;
                list.Add(emptyCommentsHelpBox);
            }
            else
            {
                foreach (var comment in targetNote.comments)
                {
                    list.Add(CreateCommentElement(comment));
                }
            }

            var newComment = root.Q<TextField>("newCommentField");
            newComment.multiline = true;

            var post = root.Q<Button>("postCommentButton");
            post.AddToClassList("button-normal");
            post.clicked -= OnPostClicked;
            post.clicked += OnPostClicked;
        }

        private void OnPostClicked()
        {
            var list = root.Q<ScrollView>("commentsList");
            var newComment = root.Q<TextField>("newCommentField");

            if (string.IsNullOrWhiteSpace(newComment.value))
                return;

            Undo.RecordObject(database, "Add Comment");

            var comment = targetNote.AddComment(
                newComment.value,
                LocalUser.GetUserName(),
                LocalUser.GetUserId()
            );

            EditorUtility.SetDirty(database);

            if (emptyCommentsHelpBox != null)
            {
                list.Remove(emptyCommentsHelpBox);
                emptyCommentsHelpBox = null;
            }

            list.Add(CreateCommentElement(comment));

            newComment.value = "";
            list.ScrollTo(list.contentContainer[list.contentContainer.childCount - 1]);
        }


        private void SetupResizableComments()
        {
            var container = root.Q<VisualElement>("commentsContainer");
            var handle = root.Q<VisualElement>("resizeHandle");

            float startHeight = 0f;
            float startMouseY = 0f;
            container.style.height = targetNote.comments.Count == 0 ? 50f : 100f;

            handle.RegisterCallback<MouseDownEvent>(evt =>
            {
                startHeight = container.resolvedStyle.height;
                startMouseY = evt.mousePosition.y;
                handle.CaptureMouse();
            });

            handle.RegisterCallback<MouseMoveEvent>(evt =>
            {
                if (!handle.HasMouseCapture())
                    return;

                float delta = evt.mousePosition.y - startMouseY;
                float newHeight = Mathf.Max(120f, startHeight + delta);
                container.style.height = newHeight;
            });

            handle.RegisterCallback<MouseUpEvent>(evt =>
            {
                handle.ReleaseMouse();
            });
        }

        private VisualElement CreateCommentElement(SceneNoteComment comment)
        {
            bool isCurrentUser = comment.authorId == LocalUser.GetUserId();

            var row = new VisualElement();
            row.AddToClassList("comment-row");
            row.AddToClassList(isCurrentUser ? "right" : "left");

            var bubble = new VisualElement();
            bubble.AddToClassList("comment-bubble");
            bubble.AddToClassList(isCurrentUser ? "self" : "other");

            var authorLabel = new Label(comment.author);
            authorLabel.AddToClassList("comment-author");

            var messageLabel = new Label(comment.message);
            messageLabel.AddToClassList("comment-message");

            var time = new DateTime(comment.timestampTicks);
            var timeLabel = new Label(time.ToString("g"));
            timeLabel.AddToClassList("comment-timestamp");

            Color bubbleColor = GetColorFromId(comment.authorId);
            bubble.style.backgroundColor = bubbleColor;

            Color textColor = GetContrastColor(bubbleColor);
            authorLabel.style.color = textColor;
            messageLabel.style.color = textColor;
            timeLabel.style.color = textColor;

            bubble.Add(authorLabel);
            bubble.Add(messageLabel);
            bubble.Add(timeLabel);

            row.Add(bubble);

            return row;
        }


        private Color GetColorFromId(string id)
        {
            int hash = id.GetHashCode();

            float hue = Mathf.Abs(hash % 1000) / 1000f;
            float saturation = 0.65f;
            float value = 0.75f;

            return Color.HSVToRGB(hue, saturation, value);
        }
        private Color GetContrastColor(Color background)
        {
            float luminance =
                0.2126f * background.r +
                0.7152f * background.g +
                0.0722f * background.b;

            return luminance > 0.6f ? Color.black : Color.white;
        }


    }

    public static class ColorUtils
    {
        // Hex string en formato "#RRGGBB" o "#RRGGBBAA"
        public static Color HexToColor(string hex)
        {
            if (!hex.StartsWith("#")) hex = "#" + hex;

            if (ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                return color;
            }
            else
            {
                Debug.LogWarning($"Hex inválido: {hex}, se devuelve Color.white");
                return Color.white;
            }
        }
    }
#endif
}
