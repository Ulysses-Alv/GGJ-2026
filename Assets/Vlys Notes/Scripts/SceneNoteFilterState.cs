namespace Vlys.Utilities.SceneNotes
{
    using System.Collections.Generic;

    [System.Serializable]
    public class SceneNoteFilterState
    {
        public List<SceneNoteCategory> visibleCategories = new();
        public List<Status> visibleStatuses = new();

        public bool isInitialized = false;

        public void EnsureInitialized()
        {
            if (isInitialized) return;

            if (visibleCategories.Count == 0)
            {
                foreach (SceneNoteCategory category in
                         System.Enum.GetValues(typeof(SceneNoteCategory)))
                {
                    visibleCategories.Add(category);
                }
            }

            if (visibleStatuses.Count == 0)
            {
                foreach (Status status in
                         System.Enum.GetValues(typeof(Status)))
                {
                    visibleStatuses.Add(status);
                }
            }
            isInitialized = true;
        }



        #region  Category
        public bool IsCategoryVisible(SceneNoteCategory category)
        {
            return visibleCategories.Contains(category);
        }

        public void ToggleCategory(SceneNoteCategory category, bool value)
        {
            if (value)
            {
                if (!visibleCategories.Contains(category))
                    visibleCategories.Add(category);
            }
            else
            {
                visibleCategories.Remove(category);
            }
        }

        public void SetAllCategories(bool value)
        {
            visibleCategories.Clear();

            if (value)
            {
                foreach (SceneNoteCategory category in
                         System.Enum.GetValues(typeof(SceneNoteCategory)))
                {
                    visibleCategories.Add(category);
                }
            }
        }

        public bool AreAllCategoriesActive()
        {
            foreach (SceneNoteCategory category in
                     System.Enum.GetValues(typeof(SceneNoteCategory)))
            {
                if (!visibleCategories.Contains(category))
                    return false;
            }

            return true;
        }
        #endregion

        #region  STATUS
        public bool IsStatusVisible(Status status)
        {
            return visibleStatuses.Contains(status);
        }

        public void ToggleStatus(Status status, bool value)
        {
            if (value)
            {
                if (!visibleStatuses.Contains(status))
                    visibleStatuses.Add(status);
            }
            else
            {
                visibleStatuses.Remove(status);
            }
        }

        public void SetAllStatuses(bool value)
        {
            visibleStatuses.Clear();

            if (value)
            {
                foreach (Status status in
                         System.Enum.GetValues(typeof(Status)))
                {
                    visibleStatuses.Add(status);
                }
            }
        }

        public bool AreAllStatusesActive()
        {
            foreach (Status status in
                     System.Enum.GetValues(typeof(Status)))
            {
                if (!visibleStatuses.Contains(status))
                    return false;
            }

            return true;
        }
        #endregion


        public bool ShouldBeVisible(SceneNoteCategory category, Status status = Status.Open)
        {
            if (!visibleCategories.Contains(category))
                return false;

            if (category != SceneNoteCategory.Bug &&
                category != SceneNoteCategory.Issue)
            {
                return true;
            }

            // Si es Bug o Issue → depende del status
            return visibleStatuses.Contains(status);
        }
    }
}
