namespace Vlys.Utilities.SceneNotes
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;

    public class SceneNoteStyleWindow : EditorWindow
    {
        private SceneNoteStyleConfig config;

       /* [MenuItem("Vlys/Scene Notes/Style Settings")]
        public static void Open()
        {
            GetWindow<SceneNoteStyleWindow>("Scene Notes Style");
        }*/

        private void OnEnable()
        {
            if (config == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:SceneNoteStyleConfig");

                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    config = AssetDatabase.LoadAssetAtPath<SceneNoteStyleConfig>(path);
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
                    config = CreateInstance<SceneNoteStyleConfig>();
                    AssetDatabase.CreateAsset(config, "Assets/Vlys Notes/SceneNoteStyleConfig.asset");
                    AssetDatabase.SaveAssets();
                }

                return;
            }

            EditorGUI.BeginChangeCheck();

            SerializedObject so = new SerializedObject(config);
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
