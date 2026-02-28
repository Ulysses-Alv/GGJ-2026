#if UNITY_EDITOR

namespace Vlys.Utilities.SceneNotes
{
using UnityEditor;
    public static class SceneNoteSelection
    {
        private static string GetKey(SceneNoteDatabase db)
        {
            return "SceneNote_Selected_" + db.GetInstanceID();
        }

        public static string Get(SceneNoteDatabase db)
        {
            return EditorPrefs.GetString(GetKey(db), string.Empty);
        }

        public static void Set(SceneNoteDatabase db, string id)
        {
            EditorPrefs.SetString(GetKey(db), id);
        }

        public static void Clear(SceneNoteDatabase db)
        {
            EditorPrefs.DeleteKey(GetKey(db));
        }
    }
}
#endif
