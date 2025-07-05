using JetBrains.Annotations;
using Sfs2X.Entities.Data;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CannonController : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private Animator[] _animators; // use this when cannon has levels
    [SerializeField] private Animator _animator; // use this when cannon has only one levels
    [SerializeField] private Animator _rocketAnimator; // use this when cannon has only one levels
    [SerializeField] private GameObject[] _bulletPrefabs;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private RectTransform _transform;
    [SerializeField] private RectTransform[] _buttonRects;
    [SerializeField] private UILineRenderer _dottedLine;
    [SerializeField] private GameObject _lockImage;
    [SerializeField] private Image _lockedFishImg;
    

    [Header("Settings")]
    [SerializeField] private float _shootSpeed;
    [SerializeField] private GameObject[] _cannons;

    [SerializeField]private int _currentCannonIndex = 0;

    private bool _canControl = true;
    private bool _canShoot = true;
    private bool _isCursorOverButton = false;
    private Coroutine _shootCoroutine;
    private int _currentLevel = 0;
    private int _damageAmount = 0;
    private bool _autoShoot = false;

    public bool withLevel = false;

    public event Action CannonShoot;

    private PlayerManager _playerManager;
    private FishHealth _targetFish = null;

    void Update()
    {
        if(!_canControl)
        {
            return;
        }

        CheckCursorOverButton();
        HandleInputs();
        AutoAttack();
        SyncRotationAuto();
    }

    public void ToggleControl(bool toggle)
    {
        _canControl = toggle;
    }

    private void AutoAttack()
    {
        if (_autoShoot)
        {
            _dottedLine?.gameObject.SetActive(true);
            if(_lockImage != null) _lockImage.SetActive(true);

            
            
            
            if (_targetFish == null || !GeneratedFishManager.Instance.HasFish(_targetFish))
            {
                _targetFish = GeneratedFishManager.Instance.GetRandomFish();
                _dottedLine?.gameObject.SetActive(false);
                _lockedFishImg.gameObject.SetActive(false);
            }
            else
            {
                _dottedLine?.SetDir(_shootPoint.position, _targetFish.transform.position);
                if (_targetFish.GetFish().FishIcon != null && _lockedFishImg != null)
                {
                    _lockedFishImg.gameObject.SetActive(true);
                    _lockedFishImg.sprite = _targetFish.GetFish().FishIcon;
                }
            }
        }
        else if (!_autoShoot)
        {
            _targetFish = null;
            _dottedLine?.gameObject.SetActive(false);
            if (_lockImage != null) _lockImage.SetActive(false);
        }
    }

    public void ChangeType(int index)
    {
        _cannons[index].SetActive(true);

        if(index == 0)
        {
            _cannons[1].SetActive(false);
            
        }
        else if (index == 1)
        {
            _cannons[0].SetActive(false);
        }

        _currentCannonIndex = index;
    }

    public void SetPlayerManager(PlayerManager playerManager)
    {
        _playerManager = playerManager;
    }

    public void SetDamageAmount(int amount)
    {
        _damageAmount = amount;
    }

    public void SetLevel(int levelIndex)
    {
        _currentLevel = levelIndex;
    }

    private void HandleInputs()
    {
        if (Input.GetMouseButton(0))
        {
            SyncRotation();

        }

        if (Input.GetMouseButton(0) || _autoShoot)
        {
            Shoot();

        }

        if(Input.GetMouseButtonUp(0))
        {
            Shoot();
        }
    }

    public void ToggleAutoShoot()
    {


        if(_autoShoot)
        {
            _autoShoot = false;
        }
        else
        {
            _autoShoot = true;
            
        }

        Debug.Log(_autoShoot);
    }

    private void SyncRotation()
    {
        if(_isCursorOverButton || (withLevel && _autoShoot && GeneratedFishManager.Instance.HasFish(_targetFish)))
        {
            return;
        }

        Vector2 mousePosition = Input.mousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasInstance.Instance.GetMainCanvas().transform as RectTransform, 
            mousePosition, CanvasInstance.Instance.GetMainCanvas().worldCamera, out Vector2 localMousePosition);

        Vector2 direction = new Vector2(mousePosition.x - _transform.position.x, mousePosition.y - _transform.position.y);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        _transform.localRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void SyncRotationAuto()
    {
        if(withLevel && _autoShoot && _targetFish != null && GeneratedFishManager.Instance.HasFish(_targetFish))
        {
            Vector2 direction = new Vector2(_targetFish.transform.position.x - _transform.position.x, _targetFish.transform.position.y - _transform.position.y);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            _transform.localRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
    }

    public void SetRotationManually(float z)
    {
        _transform.rotation = Quaternion.Euler(0, 0, z);
    }

    void CheckCursorOverButton()
    {
        for (int i = 0; i < _buttonRects.Length; i++)
        {
            if(RectTransformUtility.RectangleContainsScreenPoint(_buttonRects[i], Input.mousePosition, null))
            {
                _isCursorOverButton = true;
                break;
            }
            else
            {
                _isCursorOverButton = false;
            }
        }
    
    }


    public void Shoot()
    {
        if ((_canShoot && !_isCursorOverButton) || (_isCursorOverButton && _autoShoot && _canShoot))
        {
            Debug.Log("CanShoot " + _canShoot);
            SFSObject obj = new SFSObject();
            obj.PutFloat("zRotation", _transform.eulerAngles.z);
            GlobalManager.Instance.SendToCurrentRoom("shoot", obj);
            _shootCoroutine = StartCoroutine(ShootHandle());
        }
    }

    public void ShootNetwork()
    {
        StartCoroutine(ShootHandle());
    }

    IEnumerator ShootHandle()
    {
        GameObject bulletObj = null;

        if (withLevel)
        {
            bulletObj = Instantiate(_bulletPrefabs[_currentLevel], _shootPoint.position, _shootPoint.rotation,
            CanvasInstance.Instance.GetMainCanvas().transform);

            if(_autoShoot && _targetFish != null && GeneratedFishManager.Instance.HasFish(_targetFish))
            {
                bulletObj.GetComponent<Bullet>().SetTargetFish(_targetFish);
            }
        }
        else
        {
            bulletObj = Instantiate(_bulletPrefabs[_currentCannonIndex], _shootPoint.position, _shootPoint.rotation,
            CanvasInstance.Instance.GetMainCanvas().transform);
        }

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.SetDamageAmount(_damageAmount);
        bullet.SetPlayerManager(_playerManager);

        if (_animators.Length > 0)
        {
            _animators[_currentLevel].SetTrigger("Shoot");
        }
        else
        {
            if(_currentCannonIndex == 0)
            {
                _animator.SetTrigger("Shoot");
            }
            else
            {
                _rocketAnimator.SetTrigger("Shoot");
            }

        }
        _canShoot = false;
        CannonShoot?.Invoke();

        

        yield return new WaitForSeconds(1/_shootSpeed);
        _canShoot = true;
    }

    private void OnEnable()
    {
        _canShoot = true;
    }

    private void OnDisable()
    {
        

        if(_shootCoroutine != null)
        {
            StopCoroutine(_shootCoroutine);
        }
    }
}