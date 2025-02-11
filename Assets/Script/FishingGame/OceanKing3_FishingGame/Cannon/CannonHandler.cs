using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private GameObject _drillPowerUp;
    [SerializeField] private GameObject _hammerSmash;
    [SerializeField] private GameObject _hammerSmashTitle;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private CannonController _cannonController;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Image _cannonBaseOneImg;
    [SerializeField] private Image _cannonBaseTwoImg;

    public event Action<int> AmountChanged;

    private int _currentCannonIndex = 0;
    private bool _isLaserCannon = false;

    private int _amount;
    private LaserCannonController _laserCannonController;

    private bool _isPowerUpActive = false;

    public CannonController cannonController => _cannonController;

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
        _cannonController.withLevel = _withLevel;
    }

    private void Start()
    {
        if(_laserCannon != null)
        {
            _laserCannonController = _laserCannon.GetComponentInChildren<LaserCannonController>();
        }

        Amount = _increaseStep;

        _cannonController.SetPlayerManager(_playerManager);
        
        if(_laserCannonController != null)
        {
            _laserCannonController.LaserShoot += OnShoot;
        }
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

        if (_laserCannonController != null)
        {
            _laserCannonController.LaserShoot -= OnShoot;
        }
        
    }

    public void ActiveByOtherPowerUp()
    {
        _isPowerUpActive = true;
        _cannonController.ToggleControl(false);
    }

    public void DisablePowerUp()
    {
        _isPowerUpActive = false;
        _cannonController.ToggleControl(true);
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

    public void OnNetworkShoot(float rotation)
    {
        cannonController.SetRotationManually(rotation);
        cannonController.ShootNetwork();
    }

    private void OnAmountChange()
    {
        if(_withLevel)
        {
            if (Amount < 300)
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
        SetCannon();
    }

    public void SetCannon()
    {
        _isPowerUpActive = false;

        if (_currentCannonIndex > 2)
        {
            _currentCannonIndex = 0;
        }

        if (_currentCannonIndex < 2)
        {
            if(_currentCannonIndex == 1)
            {
                _cannonBaseOneImg.enabled = false;
                _cannonBaseTwoImg.enabled = true;
            }
            else
            {
                _cannonBaseOneImg.enabled = true;
                _cannonBaseTwoImg.enabled = false;
            }
            _laserCannon.SetActive(false);
            _cannonController.gameObject.SetActive(true);
            _cannonController.ChangeType(_currentCannonIndex);
            _isLaserCannon = false;
        }
        else
        {
            _cannonBaseOneImg.enabled = true;
            _cannonBaseTwoImg.enabled = false;
            _cannonController.gameObject.SetActive(false);
            _laserCannon.SetActive(true);
            _isLaserCannon = true;
        }
    }

    public void ActivePowerUp(PowerUpType type)
    {
        if(_isPowerUpActive)
        {
            return;
        }

        StartCoroutine(ActivePowerUpCannon(type));
    }

    IEnumerator ActivePowerUpCannon(PowerUpType type)
    {
        _isPowerUpActive = true;

        yield return new WaitForSeconds(2f);

        switch (type)
        {
            case PowerUpType.Laser:
                _laserPowerUp.SetActive(true);
                _laserCannon.SetActive(false);
                _cannonController.gameObject.SetActive(false);
                break;

            case PowerUpType.Drill:
                _drillPowerUp.SetActive(true);
                _laserCannon.SetActive(false);
                _cannonController.gameObject.SetActive(false);
                break;

            case PowerUpType.Hammer:
                StartCoroutine(HammerSmashEffect());
                break;

            default:
                break;
        }
    }

    IEnumerator HammerSmashEffect()
    {
        for (int i = 0; i < 3; i++)
        {
            _hammerSmash.SetActive(false);
            yield return new WaitForSeconds(1f);

            _hammerSmash.SetActive(true);
            _hammerSmashTitle.SetActive(true);

            yield return new WaitForSeconds(2f);

            List<FishHealth> fishList = GeneratedFishManager.Instance.GetGeneratedFishList();

            for (int a = 0; a < fishList.Count; a++)
            {
                fishList[a].InstantDie();
            }

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1f);

        _isPowerUpActive = false;

        _hammerSmash.SetActive(false);
        _hammerSmashTitle.SetActive(false);
    }
}
