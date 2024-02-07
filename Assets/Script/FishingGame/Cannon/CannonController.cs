using System;
using System.Collections;
using UnityEngine;

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

    [Header("Settings")]
    [SerializeField] private float _shootSpeed;
    [SerializeField] private GameObject[] _cannons;

    [SerializeField]private int _currentCannonIndex = 0;

    private bool _canShoot = true;
    private bool _isCursorOverButton = false;
    private Coroutine _shootCoroutine;
    private int _currentLevel = 0;
    private int _damageAmount = 0;
    private bool _autoShoot = false;

    public event Action CannonShoot;

    private PlayerManager _playerManager;

    void Update()
    {
        CheckCursorOverButton();
        HandleInputs();
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
        if(_isCursorOverButton)
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


    private void Shoot()
    {
        if ((_canShoot && !_isCursorOverButton) || (_isCursorOverButton && _autoShoot && _canShoot))
        {
            _shootCoroutine = StartCoroutine(ShootHandle());
        }

    }

    IEnumerator ShootHandle()
    {
        GameObject bulletObj = Instantiate(_bulletPrefabs[_currentCannonIndex], _shootPoint.position, _shootPoint.rotation, 
            CanvasInstance.Instance.GetMainCanvas().transform);

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