namespace Vlys.Utilities.SceneNotes
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine.SceneManagement;

    public static class SceneNoteDeleteMenu
    {
        [MenuItem("Tools/Vlys Utilities/Scene Notes/Delete All Notes")]
        private static void DeleteAllNotes()
        {
            bool confirm = EditorUtility.DisplayDialog(
                "Delete All Notes",
                "Are you sure you want to delete all Scene Notes in this scene? \n This Could cause merge conflicts",
                "Yes",
                "Cancel"
            );

            if (!confirm)
                return;

            Scene activeScene = SceneManager.GetActiveScene();

            var db = SceneNoteDatabase.GetDatabase();
            int deletedCount = 0;

            db.ClearAllNotes();

            if (deletedCount > 0)
            {
                EditorSceneManager.MarkSceneDirty(activeScene);
                SceneView.RepaintAll();
            }
        }
    }
#endif
}
