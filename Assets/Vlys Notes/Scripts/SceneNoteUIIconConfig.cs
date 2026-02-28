using UnityEngine;

namespace Vlys.Utilities.SceneNotes
{
    [CreateAssetMenu(fileName = "SceneNoteUIIconConfig", menuName = "Vlys/SceneNoteSO/SceneNoteUIIconConfig")]
    public class SceneNoteUIIconConfig : ScriptableObject
    {
        [SerializeField] private Texture2D editIcon;
        [SerializeField] private Texture2D addIcon;

        public Texture2D EditIcon => editIcon;
        public Texture2D AddIcon => addIcon;
    }
}