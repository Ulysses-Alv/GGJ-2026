namespace Vlys.Utilities.SceneNotes
{

    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

    [Serializable]
    public class SceneNoteData
    {
        public Vector3 position;

        public string title;
        public string text;

        public Sprite icon;
        public float iconScale = 1f;
        public float currentScale = 1f;

        [SerializeField] private Transform linkedTransform;
        public Transform LinkedTransform => linkedTransform;

        public SceneNoteCategory category = SceneNoteCategory.Comment;
        public Status status = Status.Open;

        public string gitBranch;
        public string author;
        public long timestampTicks;

        public bool isLocked = true;
        public bool hasAttach => LinkedTransform != null;

        public List<SceneNoteComment> comments
        {
            get
            {
                _comments ??= new();
                return _comments.OrderBy(c => c.timestampTicks).ToList();
            }
            private set { }
        }
        [SerializeField] private List<SceneNoteComment> _comments;

        public DateTime Timestamp => new(timestampTicks);

        public string id;
        public bool isDeleted = false;

        public void EnsureId()
        {
            EnsureCommentsIds();
            if (string.IsNullOrEmpty(id))
                id = System.Guid.NewGuid().ToString();
        }

        // ------------------- WorldPosition -------------------
        public Vector3 WorldPosition
        {
            get => LinkedTransform != null ? LinkedTransform.position + position : position;
            set
            {
                if (LinkedTransform != null)
                    position = value - LinkedTransform.position;
                else
                    position = value;
            }
        }

        public void LinkTo(Transform target)
        {
            if (target == null)
            {
                linkedTransform = null;
                return;
            }

            Vector3 currentWorld = WorldPosition;
            linkedTransform = target;
            position = currentWorld - LinkedTransform.position;
        }

        public void Initialize(string userName, string gitBranch)
        {
            author = userName;
            timestampTicks = DateTime.Now.Ticks;
            status = Status.Open;
            isLocked = false;
            this.gitBranch = gitBranch;
        }

        public void Lock() => isLocked = true;
        public void Unlock() => isLocked = false;

        public SceneNoteComment AddComment(string message, string author, string authorId)
        {
            var comment = new SceneNoteComment
            {
                id = System.Guid.NewGuid().ToString(),
                message = message,
                author = author,
                authorId = authorId,
                timestampTicks = DateTime.Now.Ticks
            };
            _comments ??= new();
            _comments.Add(comment);
            return comment;
        }

        public void AddSystemChangeComment(SceneNoteData before, string userName, string authorId)
        {
            if (before == null) return;

            StringBuilder builder = new();
            if (before.text != text && !string.IsNullOrEmpty(before.text))
                builder.AppendLine($"Text changed: \"{before.text}\" → \"{text}\"");

            if (before.status != status)
                builder.AppendLine($"Status changed: {before.status} → {status}");

            if (before.position != position && before.position != Vector3.zero)
                builder.AppendLine($"Position changed: {before.position} → {position}");

            if (before.status != status)
                builder.AppendLine($"Status changed: {before.status} → {status}");

            if (before.category != category)
                builder.AppendLine($"Category changed: {before.category} → {category}");

            if (builder.Length > 0)
                AddComment(builder.ToString(), userName, authorId);
        }

        public SceneNoteData Clone()
        {
            return new SceneNoteData
            {
                position = position,
                title = title,
                text = text,
                icon = icon,
                iconScale = iconScale,
                category = category,
                status = status,
                author = author,
                timestampTicks = timestampTicks,
                isLocked = isLocked,
                isDeleted = isDeleted,
                linkedTransform = LinkedTransform
            };
        }

        public void EnsureCommentsIds()
        {
            foreach (var comment in comments)
                comment.EnsureId();
        }
    }

    [Serializable]
    public class SceneNoteComment
    {
        public string id;
        public string message;
        public string author;
        public string authorId;
        public long timestampTicks;

        public void EnsureId()
        {
            if (string.IsNullOrEmpty(id))
                id = Guid.NewGuid().ToString();
        }

        public void Initialize(string msg, string user)
        {
            message = msg;
            author = user;
            timestampTicks = DateTime.Now.Ticks;
        }
    }

    public enum SceneNoteCategory
    {
        Comment,
        Bug,
        Issue,
        Documentation,
        Other
    }

    public enum Status
    {
        Open,
        InProgress,
        Resolved,
        Closed,
    }

}
