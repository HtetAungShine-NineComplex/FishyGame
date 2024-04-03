using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    private void FixedUpdate()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, new Vector2(1000, 1500), 0);

        if (hit != null && hit.gameObject.CompareTag("Fish"))
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.InstantDie();
            }
            else
            {
                Debug.LogWarning("Collided object does not implement IDamageable");
            }
        }
        else
        {
            Debug.LogWarning("No collision or collided with non-fish object");
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector2(1000, 1500));
    }

}
