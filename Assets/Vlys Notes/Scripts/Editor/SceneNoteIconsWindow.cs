namespace Vlys.Utilities.SceneNotes
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;

    public class SceneNoteIconsWindow : EditorWindow
    {
        private SceneNoteIconConfig config;

        [MenuItem("Tools/Vlys Utilities/Scene Notes/Icon Settings")]
        public static void Open()
        {
            GetWindow<SceneNoteIconsWindow>("Scene Notes Icon Settings");
        }

        private void OnEnable()
        {
            if (config == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:SceneNoteIconConfig");

                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    config = AssetDatabase.LoadAssetAtPath<SceneNoteIconConfig>(path);
                }
            }
        }
        private void OnInspectorUpdate()
        {
            Repaint();
        }


        private void OnGUI()
        {
            if (config == null)
            {
                EditorGUILayout.HelpBox("No Style Config found.", MessageType.Warning);

                if (GUILayout.Button("Create Style Config"))
                {
                    config = CreateInstance<SceneNoteIconConfig>();
                    AssetDatabase.CreateAsset(config, "Assets/Vlys Notes/SceneNoteIconConfig.asset");
                    AssetDatabase.SaveAssets();
                }

                return;
            }

            EditorGUI.BeginChangeCheck();

            SerializedObject so = new(config);
            SerializedProperty prop = so.GetIterator();

            prop.NextVisible(true);

            while (prop.NextVisible(false))
            {
                EditorGUILayout.PropertyField(prop, true);
            }

            so.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(config);
                SceneView.RepaintAll();
              
            }
        }
       
    }
#endif
}
