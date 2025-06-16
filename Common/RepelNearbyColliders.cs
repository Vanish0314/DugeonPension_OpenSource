using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.Common
{
    public class RepelNearbyColliders : MonoBehaviour
    {
        [ShowInInspector] public static float detectionRadius = 0.8f;
        [ShowInInspector] public static float softZoneEdge = 0.7f;
        [ShowInInspector] public static float repulsionStrength = 1f;
        [ShowInInspector] public LayerMask repulsionMask;
        public const float minDistance = 0.05f;
        private Rigidbody2D rb;
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        void FixedUpdate()
        {
            var colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(detectionRadius,detectionRadius),0,repulsionMask);

            foreach (Collider2D col in colliders)
            {
                if (col.attachedRigidbody != null && col.attachedRigidbody != rb)
                {
                    Vector2 direction = (Vector2)(transform.position - col.transform.position);
                    float distance = direction.magnitude - softZoneEdge;

                    if (distance < minDistance)
                        distance = minDistance;

                    float forceMagnitude = repulsionStrength / (distance * distance);
                    Vector2 force = direction.normalized * forceMagnitude;

                    rb.AddForce(force);
                }
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, Vector3.one * detectionRadius);
        }
#endif
    }

}