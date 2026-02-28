namespace Vlys.Utilities.SceneNotes
{
#if UNITY_EDITOR
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Scene Notes/Icon Config")]
    public class SceneNoteIconConfig : ScriptableObject
    {
        [Header("Icon Appearance")]
        [SerializeField, Min(0.1f), Tooltip("Default size of the icon in pixels")]
        private float iconSize = 32f;

        [SerializeField, Min(0.1f), Tooltip("Font size used for icon labels")]
        private int iconFontSize = 13;

        [SerializeField, Min(0.1f), Tooltip("Scale applied when hovering over an icon")]
        private float iconHoverScale = 1.2f;

        [SerializeField, Min(0.1f), Tooltip("Scale applied when the icon is selected")]
        private float iconSelectedScale = 1.3f;

        [SerializeField, Tooltip("Default font color for the icon")]
        private float textHeight = 1f;

        [SerializeField, Tooltip("Default font color for the icon")]
        private Color fontColor = Color.white;

        [Space(10)]
       
        [Header("Icon Behavior")]
        [Tooltip("If false, icons will scale according to world space like normal GameObjects")]
        public bool constantIconSize = true;

        [Space(10)]
        [Header("Category Icons")]
        [SerializeField, Tooltip("List of icons for each category")]
        private List<CategoryIcon> categoryIcons = new();
      

        public float IconSize => iconSize;
        public int IconFontSize => iconFontSize;
        public float IconHoverScale => iconHoverScale;
        public float IconSelectedScale => iconSelectedScale;
        public Color FontColor => fontColor;

        public float TextHeight => textHeight;

        public Texture2D GetCategoryIcon(SceneNoteCategory category)
        {
            for (int i = 0; i < categoryIcons.Count; i++)
            {
                if (categoryIcons[i].category == category)
                    return categoryIcons[i].texture;
            }

            return null;
        }
       

    }
    [System.Serializable]
    public class CategoryIcon
    {
        public SceneNoteCategory category;
        public Texture2D texture;
    }
    [System.Serializable]
    public class StatusIcon
    {
        public Status status;
        public Texture2D texture;
    }
#endif
}