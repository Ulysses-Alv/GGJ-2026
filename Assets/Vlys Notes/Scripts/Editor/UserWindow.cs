using System;
using UnityEditor;

namespace Vlys.Utilities.SceneNotes
{
    public static class LocalUser
    {
        private const string IdKey = "VLYS_NOTES_LOCAL_USER_ID";

        public static string GetUserId()
        {
            if (!EditorPrefs.HasKey(IdKey))
                EditorPrefs.SetString(IdKey, Guid.NewGuid().ToString());

            return EditorPrefs.GetString(IdKey);
        }

        public static string GetUserName()
        {
            return Environment.UserName;
        }
    }
}