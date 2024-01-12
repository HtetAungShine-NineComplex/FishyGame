using System;
using System.Collections;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private Animator[] _animators;
    [SerializeField] private GameObject[] _bulletPrefabs;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private RectTransform _transform;
    [SerializeField] private RectTransform[] _buttonRects;

    [Header("Settings")]
    [SerializeField] private float _shootSpeed;

    private bool _canShoot = true;
    private bool _isCursorOverButton = false;
    private Coroutine _shootCoroutine;
    private int _currentLevel = 0;
    private int _damageAmount = 0;

    public event Action CannonShoot;

    void Update()
    {
        CheckCursorOverButton();
        HandleInputs();
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
            Shoot();
        }

        if(Input.GetMouseButtonUp(0))
        {
            Shoot();
        }
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
        if (_canShoot && !_isCursorOverButton)
        {
            _shootCoroutine = StartCoroutine(ShootHandle());
        }

    }

    IEnumerator ShootHandle()
    {
        GameObject bulletObj = Instantiate(_bulletPrefabs[_currentLevel], _shootPoint.position, _shootPoint.rotation, 
            CanvasInstance.Instance.GetMainCanvas().transform);
        bulletObj.GetComponent<Bullet>().SetDamageAmount(_damageAmount);
        _animators[_currentLevel].SetTrigger("Shoot");
        _canShoot = false;
        CannonShoot?.Invoke();

        yield return new WaitForSeconds(1/_shootSpeed);
        _canShoot = true;
    }

    private void OnEnable()
    {
        _animators[_currentLevel].ResetTrigger("Shoot");
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