using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishHealth : MonoBehaviour,IDamageable
{
    public FishHealth Config;

    [SerializeField] private FishSO fishSO;
    [SerializeField] private Fish _fish;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private float _destroyDelay = 1f;
    [SerializeField] private bool _canDieInstantly = true;
    [SerializeField] private bool isLionTurtle;
    [SerializeField] private bool isDragon;
    private int _maxHealth;

    private int _currentHealth;
    public bool _isDead = false;

    private void Start()
    {
        _maxHealth = fishSO.MaxHealth;
        _currentHealth = _maxHealth;
    }

    public bool Damage(int damage)
    {
        if(_isDead) return true;

        _currentHealth -= damage;
        _fish.OnHit();

        if(_currentHealth <= 0)
        {
            Die();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void InstantDie()
    {
        if(_canDieInstantly == false) return;

        Debug.Log("InstantDie");
        Die();
    }

    public void Die()
    {
        if (_audio != null) _audio.Play();
        _isDead = true;
        _fish.OnDead();
        GeneratedFishManager.Instance.RemoveFish(this);
        
        if (isLionTurtle || isDragon == true)
        {
            Destroy(gameObject, 60f); //Hardcode for LionTurtle death effect duration
        }
        else Destroy(gameObject, _destroyDelay);
    }

    public Fish GetFish()
    {
        return _fish;
    }
}
