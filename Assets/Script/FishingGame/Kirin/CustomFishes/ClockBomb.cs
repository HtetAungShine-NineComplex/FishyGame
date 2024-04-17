using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockBomb : Fish
{
    [SerializeField] private GameObject _fireEffect;

    public override void OnDead()
    {
        base.OnDead();

        GameObject fx = Instantiate(_fireEffect, transform.position, Quaternion.identity, CanvasInstance.Instance.GetMidGroundSpawn());
        Destroy(fx, 2f);
        Explode();

    }

    void Explode()
    {

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 300f);

        foreach (var hit in hits)
        {
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
    }
}
