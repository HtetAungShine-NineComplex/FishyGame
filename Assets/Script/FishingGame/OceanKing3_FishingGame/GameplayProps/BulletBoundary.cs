using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBoundary : MonoBehaviour
{
    [SerializeField] private bool _isTop;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Destroy(collision.gameObject);

        if (collision.TryGetComponent<Bullet>(out Bullet bullet))
        {
            bullet.ReflectDirection(_isTop);
        }
    }
}
