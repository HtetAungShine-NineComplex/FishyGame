using System.Collections;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private RectTransform _transform;

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

        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasInstance.Instance.GetMainCanvas().transform as RectTransform, 
            mousePosition, CanvasInstance.Instance.GetMainCanvas().worldCamera, out Vector2 localMousePosition);

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
        Instantiate(_bulletPrefab, _shootPoint.position, _shootPoint.rotation, CanvasInstance.Instance.GetMainCanvas().transform);
        _animator.SetTrigger("Shoot");
        _canShoot = false;

        yield return new WaitForSeconds(1/_shootSpeed);
        _canShoot = true;
    }
}