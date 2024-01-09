using System.Collections;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private RectTransform _transform;
    [SerializeField] private Canvas canvas;

    [Header("Settings")]
    [SerializeField] private float _shootSpeed;

    private bool _canShoot = true;

    void Update()
    {
        HandleInputs();
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
        Vector2 mousePosition = Input.mousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition, canvas.worldCamera, out Vector2 localMousePosition);
        Debug.Log("Mouse X: " + localMousePosition.x + ", Mouse Y: " + localMousePosition.y);
        Vector2 direction = new Vector2(mousePosition.x - _transform.position.x, mousePosition.y - _transform.position.y);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        _transform.localRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }


    private void Shoot()
    {
        if (_canShoot)
        {
            StartCoroutine(ShootHandle());
        }

    }

    IEnumerator ShootHandle()
    {
        Instantiate(_bulletPrefab, _shootPoint.position, _shootPoint.rotation, canvas.transform);
        _animator.SetTrigger("Shoot");
        _canShoot = false;

        yield return new WaitForSeconds(1/_shootSpeed);
        _canShoot = true;
    }
}