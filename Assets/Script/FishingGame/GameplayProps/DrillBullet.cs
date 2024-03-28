using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillBullet : Bullet
{
    [SerializeField] private GameObject _firstPhaseSprite;
    [SerializeField] private GameObject _secondPhaseSpriteSprite;
    [SerializeField] private GameObject _explodeFX;

    protected override void Update()
    {
        base.Update();

        if (_secondPhaseSpriteSprite.activeSelf)
        {
            // Rotate the sprite like a fan
            // Replace speed with the rotation speed you want
            float speed = 200f;
            _secondPhaseSpriteSprite.transform.Rotate(0, 0, speed * Time.deltaTime);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fish"))
        {
            if (collision.TryGetComponent<FishHealth>(out FishHealth fish))
            {
                fish.InstantDie();
            }
        }
    }

   

    public override void ReflectDirection(bool isTop)
    {
        _bounceCount++;

        if(_bounceCount <= 5)
        {
            
        }
        else if(_bounceCount < 10)
        {
            _secondPhaseSpriteSprite.SetActive(true);
            _firstPhaseSprite.SetActive(false);
        }
        else
        {
            
            StartCoroutine(StartExplode());
        }

        if (isTop)
        {
            transform.Rotate(180, 0, 0, Space.World);
        }
        else
        {
            transform.Rotate(0, 180, 0, Space.World);
        }
    }

    IEnumerator StartExplode()
    {

        yield return new WaitForSeconds(0.5f);
        _move = false;
        
        _explodeFX.SetActive(true);
        yield return new WaitForSeconds(1f);
        Collider2D hit = Physics2D.OverlapCircle(_explodeFX.transform.position, 600f);

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

        Destroy(gameObject, 1f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_explodeFX.transform.position, 600f);
    }

}
