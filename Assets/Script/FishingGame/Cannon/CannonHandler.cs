using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _withLevel = false;
    [SerializeField] private int _increaseStep = 50; //adjust based on match type
    [SerializeField] private int _maxAmount = 600; //adjust based on match type

    [Header("References")]
    [SerializeField] private GameObject[] _cannonObjs;
    [SerializeField] private GameObject _laserCannon;
    [SerializeField] private GameObject _laserPowerUp;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private CannonController _cannonController;
    [SerializeField] private AudioSource _audioSource;

    public event Action<int> AmountChanged;

    private int _currentCannonIndex = 0;
    private bool _isLaserCannon = false;

    private int _amount;
    private LaserCannonController _laserCannonController;

    private int Amount
    {
        get { return _amount; }
        set { _amount = value; 
        
            if(_amount > _maxAmount)
            {
                _amount = _increaseStep;
            }
            else if(_amount <= _increaseStep)
            {
                _amount = _increaseStep;
            }

            AmountChanged?.Invoke(_amount);
        }
    }

    private void Awake()
    {
        AmountChanged += n => OnAmountChange();
    }

    private void Start()
    {
        _laserCannonController = _laserCannon.GetComponentInChildren<LaserCannonController>();

        Amount = _increaseStep;

        _cannonController.SetPlayerManager(_playerManager);
        
        _laserCannonController.LaserShoot += OnShoot;
    }

    private void OnEnable()
    {
        Amount = _increaseStep;

        _cannonController.SetPlayerManager(_playerManager);

        _cannonController.CannonShoot += OnShoot;
    }

    private void OnDisable()
    {
        _cannonController.CannonShoot -= OnShoot;
        _laserCannonController.LaserShoot -= OnShoot;
    }

    public void IncreaseAmount()
    {
        Amount += _increaseStep;
    }
    
    public void DecreaseAmount()
    {
        Amount -= _increaseStep;
    }

    private void OnShoot()
    {
        _playerManager.UseCoin(Amount);

        if(!_isLaserCannon)
        {
            _audioSource.Stop();
            _audioSource.Play();
        }
    }

    private void OnAmountChange()
    {
        if(_withLevel)
        {
            if (Amount < 200)
            {
                SetCannonLevel(0);
            }
            else if (Amount < 300)
            {
                SetCannonLevel(1);
            }
            else if (Amount < 500)
            {
                SetCannonLevel(2);
            }
            else
            {
                SetCannonLevel(3);
            }
        }
        

        _cannonController.SetDamageAmount(Amount);
        _laserCannonController?.SetDamageAmount(Amount);
    }

    private void SetCannonLevel(int levelIndex)
    {
        if( levelIndex >= _cannonObjs.Length )
        {
            return;
        }

        foreach( GameObject obj in _cannonObjs )
        {
            obj.SetActive(false);
        }

        _cannonObjs[levelIndex].SetActive(true);
        _cannonController.SetLevel(levelIndex);
    }

    public void SwapWeapon()
    {
        _currentCannonIndex++;

        if(_currentCannonIndex > 2)
        {
            _currentCannonIndex = 0;
        }

        if(_currentCannonIndex < 2)
        {
            _laserCannon.SetActive(false);
            _cannonController.gameObject.SetActive(true);
            _cannonController.ChangeType(_currentCannonIndex);
            _isLaserCannon = false;
        }
        else
        {
            _cannonController.gameObject.SetActive(false);
            _laserCannon.SetActive(true);
            _isLaserCannon = true;
        }

        Debug.Log(_currentCannonIndex);
    }

    public void ActiveLaserCannon()
    {
        StartCoroutine(LaserCannon()); 
    }

    IEnumerator LaserCannon()
    {
        yield return new WaitForSeconds(2f);

        _laserPowerUp.SetActive(true);
        _laserCannon.SetActive(false);
        _cannonController.gameObject.SetActive(false);
    }
}
