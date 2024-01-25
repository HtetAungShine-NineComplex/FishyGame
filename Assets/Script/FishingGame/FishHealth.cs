using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishHealth : MonoBehaviour,IDamageable
{
    public FishHealth Config;

    [SerializeField] private FishSO fishSO;
    [SerializeField] private Fish _fish;
    private int _maxHealth;

    private int _currentHealth;
    public bool _isDead = false;
    private void Start()
    {
        _maxHealth = fishSO.MaxHealth;
        _currentHealth = _maxHealth;
    }
    private void Awake()
    {
        
    }

    public bool Damage(int damage)
    {
        if(_isDead) return true;

        _currentHealth -= damage;

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

    public void Die()
    {
        _isDead = true;
        _fish.OnDead();
        Destroy(gameObject, 1f);
    }

    public Fish GetFish()
    {
        return _fish;
    }
}
