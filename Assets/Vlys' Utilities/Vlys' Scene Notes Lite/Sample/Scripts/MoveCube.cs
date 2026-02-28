namespace Vlys.SceneNotes.Sample
{
    using UnityEngine;

    [ExecuteAlways]
    public class MoveCube : MonoBehaviour
    {
        [SerializeField] private float radius = 2f;
        [SerializeField] private float speed = 1f;
        [SerializeField] private Transform centerToOrbit;

        private float angle = 0f;

        void Update()
        {
            if (centerToOrbit == null) return;
            angle += speed * Time.deltaTime;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            transform.position = new Vector3(x, 0, z) + centerToOrbit.position;
        }

        void OnDrawGizmos()
        {

            if (centerToOrbit == null) return;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(centerToOrbit.position, radius);
        }
    }

}