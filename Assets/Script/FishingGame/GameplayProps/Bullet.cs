using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private Transform[] _netSpawnPoints;
    [SerializeField] private GameObject _netPrefab;
    [SerializeField] private RectTransform _rectTransform;

    private int _damageAmount;

    void Update()
    {
        MoveBullet();
    }

    public void SetDamageAmount(int amount)
    {
        _damageAmount = amount;
    }

    void MoveBullet()
    {
        //transform.Translate(Vector3.up * speed * Time.deltaTime * 100);

        Vector2 movementDirection = _rectTransform.rotation * Vector2.up;

        // Move the bullet using anchoredPosition
        _rectTransform.anchoredPosition += movementDirection * speed * Time.deltaTime * 100;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Fish"))
        {
            if(collision.GetComponent<IDamageable>().Damage(_damageAmount))
            {
                OnCaughtFish();
            }

            OnHitFish();

            Destroy(gameObject);
        }
    }

    private void OnHitFish()
    {
        foreach (Transform t in _netSpawnPoints)
        {
            Instantiate(_netPrefab, t.position, Quaternion.identity, CanvasInstance.Instance.GetMainCanvas().transform);
        }
    }

    private void OnCaughtFish()
    {
        CoinManager.Instance.ShowSilverCoin(transform.position, 5);
    }
}
