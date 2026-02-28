#if UNITY_EDITOR
namespace Vlys.Utilities.SceneNotes
{
    using System;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [InitializeOnLoad]
    public static class SceneNoteTool
    {
        private static bool isCreating;
        private static SceneNoteDatabase db;
        private static SceneNoteIconConfig styleConfig;

        private static Material material;
        private static Mesh quad;

        public static event Action<bool> onCreationChanged;

        static SceneNoteTool()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;
        }

        private static void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            db = null;
            SceneView.RepaintAll();
        }

        public static void SetCreateMode(bool value)
        {
            isCreating = value;
            onCreationChanged?.Invoke(isCreating);
        }

        private static void OnSceneGUI(SceneView view)
        {
            EnsureDatabase();
            if (db == null)
                return;

            LoadStyleConfig();
            if (styleConfig == null)
                return;

#if UNITY_2021_3_OR_NEWER && !UNITY_6000_0_OR_NEWER
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
#endif

            if (isCreating)
                HandleCreation();

            ProcessInput(view);
            RenderNotes(view);
        }

        private static void EnsureDatabase()
        {
            if (db == null)
                db = SceneNoteDatabase.GetDatabase();
        }

        private static void HandleCreation()
        {
            Event e = Event.current;

            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                Vector3 position = ray.origin + ray.direction * 5f;

                Undo.RecordObject(db, "Create Scene Note");

                SceneNoteData note = new SceneNoteData
                {
                    position = position,
                    text = ""
                };

                note.Initialize(Environment.UserName, GitHead.GetGitBranch());

                db.AddNote(note);

                SceneNoteSelection.Set(db, note.id);
                Selection.activeObject = db;

                EditorUtility.SetDirty(db);
                AssetDatabase.SaveAssets();

                isCreating = false;
                onCreationChanged?.Invoke(isCreating);

                e.Use();
            }
        }

       private static void ProcessInput(SceneView view)
        {
            if (!SceneNoteOverlay.isActive) return;

            Event e = Event.current;

            if (e.type != EventType.MouseUp || e.button != 0 || e.alt)
                return;

            if (GUIUtility.hotControl != 0)
                return;

            var hoveredId = GetHoveredNoteId(view);

            if (hoveredId != string.Empty)
            {
#if UNITY_2021_3_OR_NEWER && !UNITY_6000_0_OR_NEWER
        GUIUtility.hotControl = 0;
#endif
                SceneNoteSelection.Set(db, hoveredId);
                Selection.activeObject = db;
                EditorUtility.SetDirty(db);

                e.Use();
                SceneView.RepaintAll();
            }
            else
            {
                SceneNoteSelection.Clear(db);
            }
        }


        private static string GetHoveredNoteId(SceneView view)
        {
            Event e = Event.current;
            Camera cam = view.camera;
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);

            foreach (var note in db._notes)
            {
                if (!SceneNoteOverlay.filterState.ShouldBeVisible(note.category, note.status) || note.isDeleted)
                    continue;

                Quaternion rotation = Quaternion.LookRotation(cam.transform.forward);
                Vector3 forward = rotation * Vector3.forward;
                Vector3 notePos = note.WorldPosition;

                float distance = Vector3.Distance(cam.transform.position, notePos);
                float scale = styleConfig.constantIconSize ? distance * styleConfig.IconSize : styleConfig.IconSize;

                Plane plane = new Plane(forward, notePos);

                if (!plane.Raycast(mouseRay, out float enter))
                    continue;

                if (Physics.Raycast(mouseRay, enter))
                    continue;

                Vector3 hitPoint = mouseRay.GetPoint(enter);
                float halfSize = scale * 0.5f;
                Vector3 local = Quaternion.Inverse(rotation) * (hitPoint - notePos);

                if (Mathf.Abs(local.x) <= halfSize && Mathf.Abs(local.y) <= halfSize)
                    return note.id;
            }

            return string.Empty;
        }

        private static void RenderNotes(SceneView view)
        {
            if (!SceneNoteOverlay.isActive) return;

            if (quad == null)
                CreateQuad();

            if (material == null)
                material = new Material(Shader.Find("Unlit/Transparent"));

            Camera cam = view.camera;

            foreach (var note in db._notes)
            {
                if (!SceneNoteOverlay.filterState.ShouldBeVisible(note.category, note.status) || note.isDeleted)
                    continue;

                var textureIcon = db.IconConfig.GetCategoryIcon(note.category);
                if (textureIcon == null)
                    continue;

                Quaternion rotation = Quaternion.LookRotation(cam.transform.forward);
                Vector3 notePos = note.WorldPosition;

                float distance = Vector3.Distance(cam.transform.position, notePos);
                float scale = styleConfig.constantIconSize ? distance * styleConfig.IconSize : styleConfig.IconSize;
                float finalScale = scale;


                bool isSelected = SceneNoteSelection.Get(db) == note.id;
                DrawLabel(note, notePos, distance, finalScale, isSelected, cam);

                if (isSelected)
                    finalScale *= styleConfig.IconSelectedScale;

                material.mainTexture = textureIcon;
                material.color = isSelected ? Color.yellow : Color.white;
                material.SetPass(0);

                Matrix4x4 matrix = Matrix4x4.TRS(
                    notePos,
                    rotation,
                    new Vector3(finalScale, finalScale, 1f)
                );

                Graphics.DrawMeshNow(quad, matrix);

                if (isSelected && !note.isLocked)
                    DrawPositionHandle(note);
            }
        }

        private static void DrawLabel(SceneNoteData note, Vector3 notePos, float distance, float finalScale, bool isSelected, Camera cam)
        {
            if (isSelected)
                finalScale *= styleConfig.IconSelectedScale;

            float textOffset = styleConfig.TextHeight * finalScale;

            float baseFont = styleConfig.IconFontSize;
            float scaledFont = baseFont / distance * 10f;
            int fontSize = (int)Mathf.Clamp(scaledFont, 0, 50);

            GUIStyle centeredStyle = new(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = fontSize
            };

            centeredStyle.normal.textColor = styleConfig.FontColor;

            Vector3 textWorldPos = notePos + Vector3.up * textOffset;

            Handles.Label(textWorldPos, note.title, centeredStyle);
        }

        private static void DrawPositionHandle(SceneNoteData note)
        {
            Vector3 notePos = note.WorldPosition;

            EditorGUI.BeginChangeCheck();

            Vector3 newPos;

            if (note.LinkedTransform != null)
                newPos = Handles.PositionHandle(note.LinkedTransform.position + note.position, Quaternion.identity);
            else
                newPos = Handles.PositionHandle(notePos, Quaternion.identity);

            if (!EditorGUI.EndChangeCheck())
                return;

            Undo.RecordObject(db, "Move Scene Note");

            if (note.LinkedTransform != null)
                note.position = newPos - note.LinkedTransform.position;
            else
                note.WorldPosition = newPos;

            EditorUtility.SetDirty(db);
        }

        private static void LoadStyleConfig()
        {
            if (styleConfig != null)
                return;

            string[] guids = AssetDatabase.FindAssets("t:SceneNoteIconConfig");

            if (guids.Length == 0)
                return;

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            styleConfig = AssetDatabase.LoadAssetAtPath<SceneNoteIconConfig>(path);
        }

        private static void CreateQuad()
        {
            quad = new Mesh
            {
                vertices = new[]
                {
                    new Vector3(-0.5f,-0.5f,0),
                    new Vector3(0.5f,-0.5f,0),
                    new Vector3(-0.5f,0.5f,0),
                    new Vector3(0.5f,0.5f,0)
                },
                uv = new[]
                {
                    new Vector2(0,0),
                    new Vector2(1,0),
                    new Vector2(0,1),
                    new Vector2(1,1)
                },
                triangles = new[] { 0, 2, 1, 2, 3, 1 }
            };
        }
    }
}
#endif
