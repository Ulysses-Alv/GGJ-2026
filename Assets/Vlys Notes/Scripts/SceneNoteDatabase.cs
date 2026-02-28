namespace Vlys.Utilities.SceneNotes
{

#if UNITY_EDITOR
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using System.Collections.Generic;
    using System.IO;
    using System;

    public class SceneNoteDatabase : ScriptableObject
    {
        public List<SceneNoteData> _notes => notes;
        [SerializeField] private List<SceneNoteData> notes = new();
        [SerializeField] private SceneNoteIconConfig _iconConfig;
        private static string folder;

        public SceneNoteIconConfig IconConfig
        {
            get
            {
                if (_iconConfig == null)
                {
                    string[] guids = AssetDatabase.FindAssets("t:SceneNoteIconConfig");

                    if (guids.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        _iconConfig = AssetDatabase.LoadAssetAtPath<SceneNoteIconConfig>(path);
                    }
                }
                return _iconConfig;
            }
            set { }
        }

        public SceneNoteData GetNoteById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return notes.Find(n => n.id == id);
        }

        public void AddNote(SceneNoteData note)
        {
            notes.Add(note);
            note.EnsureId();
        }

        public static SceneNoteDatabase GetDatabase()
        {
            var scene = EditorSceneManager.GetActiveScene();

            if (string.IsNullOrEmpty(scene.path))
                return null;

            string scenePath = scene.path;
            string sceneDirectory = Path.GetDirectoryName(scenePath);
            string sceneFolder = Path.Combine(sceneDirectory, scene.name);
            string assetPath = Path.Combine(sceneFolder, scene.name + "_Notes.asset");

            assetPath = assetPath.Replace("\\", "/");
            var db = AssetDatabase.LoadAssetAtPath<SceneNoteDatabase>(assetPath);

            if (db == null)
            {
                if (!AssetDatabase.IsValidFolder(sceneFolder))
                {
                    string parentFolder = sceneDirectory.Replace("\\", "/");
                    AssetDatabase.CreateFolder(parentFolder, scene.name);
                }

                db = CreateInstance<SceneNoteDatabase>();
                AssetDatabase.CreateAsset(db, assetPath);
                AssetDatabase.SaveAssets();
            }
            return db;
        }    

        public void ClearAllNotes()
        {
            notes.Clear();
        }

        public void SetNoteAsDeleted(SceneNoteData targetNote)
        {
            var index = notes.IndexOf(targetNote);
            notes[index].isDeleted = true;
        }
    }
#endif
}